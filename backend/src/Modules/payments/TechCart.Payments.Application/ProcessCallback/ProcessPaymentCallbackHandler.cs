using MassTransit;
using TechCart.EventBus.Events;
using TechCart.Inventory.Application.Confirm;
using TechCart.Inventory.Application.Dtos.RequestDtos;
using TechCart.OrderItems.Application.Dtos.RequestDtos;
using TechCart.OrderItems.Application.List;
using TechCart.Orders.Domain.Repositories;
using TechCart.Payments.Application.Abstractions;
using TechCart.Payments.Application.Dtos.RequestDtos;
using TechCart.Payments.Domain.Entities;
using TechCart.Payments.Domain.Exceptions;
using TechCart.Payments.Domain.Repositories;

namespace TechCart.Payments.Application.ProcessCallback;

public class ProcessPaymentCallbackHandler(
    IPaymentGateway paymentGateway,
    IPaymentWriteRepository paymentWriteRepository,
    IOrderWriteRepository orderWriteRepository,
    ListOrderItemsHandler listOrderItemsHandler,
    ConfirmReservationHandler confirmReservationHandler,
    IPublishEndpoint publishEndpoint)
{
    private const string Provider = "iyzico";

        public async Task<bool> Handle(ProcessPaymentCallbackCommand command, CancellationToken ct)
    {
        // callback ile gelen token kullanilarak odeme sonucunu alir
        var result = await paymentGateway.RetrieveCheckoutResultAsync(command.Token, ct);

        // command.OrderId ile siparis aranir. 
        var order = await orderWriteRepository.GetByIdAsync(command.OrderId, ct)
            ?? throw new OrderNotFoundForPaymentException(command.OrderId.ToString());

        // odeme basarili ise odenen tutar ile order.Total karsilastirilir. Fark 0,01’den büyükse işlem durdurulur.
        if (result.IsSuccessful && Math.Abs(result.PaidPrice - order.Total) > 0.01m)
        {
            throw new InvalidOperationException(
                $"Ödenen tutar ({result.PaidPrice}) siparişin tutarıyla ({order.Total}) eşleşmiyor.");
        }

        // bu odemenin daha once islenip islenmedigine bakar. ProviderPaymentId ile ayni id'de bir odeme kaydi varsa callback'i tekrar islemez
        var alreadyProcessed = await paymentWriteRepository.ExistsByProviderReferenceAsync(result.ProviderPaymentId, ct);
        if (alreadyProcessed)
            return result.IsSuccessful;

        // odeme basarisizsa sebebi ile birlikte bir Paymnet kaydi olusturulur
        if (!result.IsSuccessful)
        {
            var failedPayment = Payment.CreateFailed(order.Id, Provider, result.PaidPrice, "TRY",
                result.ProviderPaymentId, result.FailureReason ?? "Bilinmeyen hata");
            await paymentWriteRepository.AddAsync(failedPayment, ct);
            await paymentWriteRepository.SaveChangesAsync(ct);
            return false;
        }

        // bu islem siparis hala awaitingPayment durumunda ise basarili olmali. kullanici islemi iptal etmis olabilri,
        // beklenen surede islemi gerceklestirmemis olabilr
        var markedAsPaid = await orderWriteRepository.TryMarkAsPaidAsync(order.Id, ct);

        if (!markedAsPaid)
        {
            // iyzico parayı GERÇEKTEN aldı ama sipariş artık geçersiz —
            // bunu sistemimiz için başarısız sayıyoruz. 
            var lateSuccessPayment = Payment.CreateFailed(order.Id, Provider, result.PaidPrice, "TRY",
                result.ProviderPaymentId, "Sipariş süresi dolduğu için iptal edilmişti, ödeme geçersiz sayıldı.");
            await paymentWriteRepository.AddAsync(lateSuccessPayment, ct);
            await paymentWriteRepository.SaveChangesAsync(ct);
            return false;
        }

        var orderItems = await listOrderItemsHandler.Handle(new ListOrderItemsQuery(order.Id), ct);

        foreach (var item in orderItems.Where(i => i.ProductId.HasValue))
        {
            var updatedStock = await confirmReservationHandler.Handle(
                new ConfirmReservationCommand(item.ProductId!.Value, item.Quantity), ct);

            await publishEndpoint.Publish(
                new ProductStockChangedEvent(item.ProductId!.Value, updatedStock), ct);
        }

        var succeededPayment = Payment.CreateSucceeded(order.Id, Provider, result.PaidPrice, "TRY",
            result.ProviderPaymentId, tdsReference: null, maskedCardNumber: result.MaskedCardNumber);
        await paymentWriteRepository.AddAsync(succeededPayment, ct);
        await paymentWriteRepository.SaveChangesAsync(ct);

        return true;
    }}