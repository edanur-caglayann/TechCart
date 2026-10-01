using TechCart.OrderItems.Application.Dtos.RequestDtos;
using TechCart.OrderItems.Application.List;
using TechCart.Orders.Domain.Enums;
using TechCart.Orders.Domain.Exceptions;
using TechCart.Orders.Domain.Repositories;
using TechCart.Payments.Application.Abstractions; 
using TechCart.Payments.Application.Dtos.RequestDtos;
using TechCart.Payments.Contracts.ResponseDtos;
using TechCart.Users.Application.Dtos.RequestDtos;
using TechCart.Users.Application.Profile;

namespace TechCart.Payments.Application.InitiateCheckout;

public class InitiateCheckoutHandler(
    IOrderWriteRepository orderWriteRepository,
    GetMyProfileHandler getMyProfileHandler,
    ListOrderItemsHandler listOrderItemsHandler,
    IPaymentGateway paymentGateway)
{
    public async Task<CheckoutInitResponse> Handle(InitiateCheckoutCommand command, CancellationToken ct)
    {
        // ilgili kullaniciya ait siparisi bulur
        var order = await orderWriteRepository.GetByIdAsync(command.OrderId, ct);

        if (order is null || order.UserId != command.UserId)
            throw new InvalidOperationException("Sipariş bulunamadı.");
        
        // siparis gercekten odeme bekliyor mu
        if (order.Status != OrderStatus.AwaitingPayment)
            throw new OrderNotAwaitingPaymentException(order.Id, order.Status.ToString());

        // bu kullanicinin bilgilerini getirir
        var profile = await getMyProfileHandler.Handle(new GetMyProfileQuery(command.UserId), ct);
        
        // siparisin satirlarinlarini listeler
        var orderItems = await listOrderItemsHandler.Handle(new ListOrderItemsQuery(order.Id), ct);
        var basketItems = orderItems // iyzico'nun istedigi basket formatina donusturur
            .Select(item => (item.ProductName, item.UnitPrice * item.Quantity))
            .ToList();
        
        // her denemede farkli siparis kimlgii
        var conversationId = $"{order.Id}_{Guid.NewGuid():N}"[..44]; // iyzico conversationId alanı için makul bir uzunluk sınırı
        
        // tum datalar toplamntiktan sonra iyzicoya form olusturmasi icin yollanir
        var result = await paymentGateway.InitializeCheckoutAsync( 
            conversationId: order.Id.ToString(),
            price: order.Total,
            buyerName: profile.FirstName,
            buyerSurname: profile.LastName,
            buyerEmail: profile.Email,
            buyerPhone: order.ShippingPhone,
            buyerAddress: order.ShippingAddressLine,
            buyerCity: order.ShippingCity,
            callbackUrl: $"http://localhost:5223/api/payments/iyzico/callback?orderId={order.Id}",            basketItems: basketItems,
            ct: ct);

        return result;
    }
}