using TechCart.Addresses.Application.Detail;
using TechCart.Addresses.Application.Dtos.RequestDtos;
using TechCart.CartItems.Application.Clear;
using TechCart.CartItems.Application.Get;
using TechCart.CartItems.Contracts;
using TechCart.CartItems.Contracts.Dtos.ResponseDtos;
using TechCart.Inventory.Application.Dtos.RequestDtos;
using TechCart.Inventory.Application.Release;
using TechCart.Inventory.Application.Reserve;
using TechCart.OrderItems.Application.Create;
using TechCart.OrderItems.Application.Dtos.RequestDtos;
using TechCart.Orders.Application.CreateOrderNumber;
using TechCart.Orders.Application.Dtos.RequestDtos;
using TechCart.Orders.Contracts.ResponseDtos;
using TechCart.Orders.Domain.Entities;
using TechCart.Orders.Domain.Exceptions;
using TechCart.Orders.Domain.Repositories;

namespace TechCart.Orders.Application.CreateOrder;

public class CreateOrderHandler(
    IOrderWriteRepository orderWriteRepository,
    GetCartHandler getCartHandler,
    ClearCartHandler clearCartHandler,
    GetAddressDetailHandler getAddressDetailHandler,
    ReserveStockHandler reserveStockHandler,
    ReleaseReservationHandler releaseReservationHandler,
    CreateOrderItemsHandler createOrderItemsHandler)
{
    public async Task<OrderResponse> Handle(CreateOrderCommand command, CancellationToken ct)
    {
        // kullanicinin sepeti getirilir
        var cart = await getCartHandler.Handle(new GetCartQuery(command.UserId), ct);

        // sepette hic urun yoksa
        if (cart.Items.Count == 0)
            throw new EmptyCartException();

        // teslimat adresi getirilir
        var address = await getAddressDetailHandler.Handle(
            new GetAddressDetailQuery(command.AddressId, command.UserId), ct);

        var reservedItems = new List<CartItemResponse>();

        // sepetteki urunler dolasilir
        foreach (var item in cart.Items)
        {
            // her urun icin stok rezervsyonu yapilir
            var reserved = await reserveStockHandler.Handle(
                new ReserveStockCommand(item.ProductId, item.Quantity), ct);

            // herhangi bir ururnun stogu yetersiz olursa daha once rezeve edilen urunlerin rezervasyonalri geri alinir
            if (!reserved)
            {
                foreach (var toRelease in reservedItems)
                    await releaseReservationHandler.Handle(
                        new ReleaseReservationCommand(toRelease.ProductId, toRelease.Quantity), ct);

                throw new InsufficientStockException(item.ProductId, item.Stock);
            }

            reservedItems.Add(item);
        }
        // siparis numarasi olusturulur
        var orderNumber = OrderNumberGenerator.Generate();
        
        // siparis olusturulur
        var order = Order.Create(command.UserId, orderNumber, cart.Subtotal, cart.VatTotal,
            cart.ShippingFee, cart.Total, address.FullName, address.Phone, address.City,
            address.District, address.AddressLine, address.PostalCode);

        await orderWriteRepository.AddAsync(order, ct);
        await orderWriteRepository.SaveChangesAsync(ct);

        // sepetteki urunler Select ile dolasilarak OrderItemsLines listesine donusturulur
        var orderItemLines = cart.Items.Select(item => new OrderItemLine(
            item.ProductId, item.Name, item.Model, item.Quantity, item.UnitPrice, item.VatRate, item.VatAmount)).ToList();

        // her urun icin ayni OrderId degerine bagli bir OrderItem kaydi olusturulur
        await createOrderItemsHandler.Handle(new CreateOrderItemsCommand(order.Id, orderItemLines), ct);
        //siparis ve siparis urunleri basariyla olusturulduktan sonra kullanici sepeti temizlenir
        await clearCartHandler.Handle(new ClearCartCommand(command.UserId), ct);
        
        // siparis ile ilgili bilgiler clienta gonderilir
        return new OrderResponse(order.Id, order.OrderNumber, order.Status.ToString(), order.CreatedAt,
            order.Subtotal, order.VatTotal, order.ShippingFee, order.Total,
            order.ShippingFullName, order.ShippingPhone, order.ShippingCity,
            order.ShippingDistrict, order.ShippingAddressLine, order.ShippingPostalCode,
            Payment: null, // sipariş yeni oluştu, henüz ödeme yok
            orderItemLines.Select(line => new OrderItemLineResponse(
                line.ProductId, line.ProductName, line.ProductModel, line.Quantity, line.UnitPrice, line.VatRate, line.VatAmount)).ToList()); }
}