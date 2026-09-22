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
        //  iyzicodan bu token'ın gercek sonucuni aliriz
        var result = await paymentGateway.RetrieveCheckoutResultAsync(command.Token, ct);
        
        var order = await orderWriteRepository.GetByIdAsync(command.OrderId, ct)
                    ?? throw new OrderNotFoundForPaymentException(command.OrderId.ToString());

        // Güvenlik katmanı: orderId artık URL'den (imzasız) geliyor —
        // ödenen tutarın gerçekten bu siparişin tutarıyla eşleştiğini
        // doğrulamadan devam etmiyoruz.
        if (result.IsSuccessful && Math.Abs(result.PaidPrice - order.Total) > 0.01m)
        {
            throw new InvalidOperationException(
                $"Ödenen tutar ({result.PaidPrice}) siparişin tutarıyla ({order.Total}) eşleşmiyor.");
        }
        // bu ödeme zaten daha önce işlendi mi diye bakariz 
        var alreadyProcessed = await paymentWriteRepository.ExistsByProviderReferenceAsync(result.ProviderPaymentId, ct);
        if (alreadyProcessed)
            return result.IsSuccessful; 

        if (result.IsSuccessful)
        {
            // eger awaitingPayment ise paid olarak degistir 
            var markedAsPaid = await orderWriteRepository.TryMarkAsPaidAsync(order.Id, ct);

            if (markedAsPaid)
            {
                // Sipariş satırlarını çekip, her ürünün rezervasyonunu kalici olarak düşürur
                var orderItems = await listOrderItemsHandler.Handle(new ListOrderItemsQuery(order.Id), ct);
    
                // orderitems icinden productid degeri bulunan siparis satirlarini getirir ve urunler tek tek dolasilir
                foreach (var item in orderItems.Where(i => i.ProductId.HasValue))
                {
                    // her urun icin ConfirmReservationHandler metodu cagirilir. 
                    // siparise ait urun adedi hem gercek stoktan hem de rezerve stoktan duser//
                    // handler islemi sonrasi urunun guncel stok miktari updatedStock olarak dondurulur
                    var updatedStock = await confirmReservationHandler.Handle(
                        new ConfirmReservationCommand(item.ProductId!.Value, item.Quantity), ct);

                    // MassTransit’in publishEndpoint nesnesi üzerinden RabbitMQ’ya yayımlanır.
                    // Event içerisinde ürün kimliği ve yeni stok miktarı bulunur:
                    // Products modülündeki consumer bu event’i dinleyerek ilgili ürünü bulur ve:product.SetStock(updatedStock);
                    // metodunu çağırır. Böylece Inventory modülündeki gerçek stok ile Products modülündeki gösterim amaçlı stok bilgisi eşitlenir.
                    await publishEndpoint.Publish(
                        new ProductStockChangedEvent(item.ProductId!.Value, updatedStock), ct);
                }
            }

            var payment = Payment.CreateSucceeded(order.Id, Provider, result.PaidPrice, "TRY",
                result.ProviderPaymentId, tdsReference: null);
            await paymentWriteRepository.AddAsync(payment, ct);
        }
        else
        {
            var payment = Payment.CreateFailed(order.Id, Provider, result.PaidPrice, "TRY",
                result.ProviderPaymentId, result.FailureReason ?? "Bilinmeyen hata");
            await paymentWriteRepository.AddAsync(payment, ct);
        }

        await paymentWriteRepository.SaveChangesAsync(ct);

        return result.IsSuccessful;
    }
}