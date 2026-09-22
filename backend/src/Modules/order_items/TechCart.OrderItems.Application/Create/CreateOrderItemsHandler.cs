using TechCart.OrderItems.Application.Dtos.RequestDtos;
using TechCart.OrderItems.Domain.Entities;
using TechCart.OrderItems.Domain.Repositories;

namespace TechCart.OrderItems.Application.Create;

// siaprise ait urun satirlarini olusturup veritabanina toplu olarak kaydeder
public class CreateOrderItemsHandler(IOrderItemWriteRepository orderItemWriteRepository)
{
    public async Task Handle(CreateOrderItemsCommand command, CancellationToken ct)
    {
        // komut icindeki items listesini select ile dolasir. her liste elemani icin yani her siparis urunu icin
        // OrderItem.create() metodunu cagirip OrderItem entitysi olusturur
        var orderItems = command.Items.Select(line => OrderItem.Create(
            command.OrderId, line.ProductId, line.ProductName, line.ProductModel,
            line.Quantity, line.UnitPrice, line.VatRate, line.VatAmount)).ToList();

        await orderItemWriteRepository.AddRangeAsync(orderItems, ct);
        await orderItemWriteRepository.SaveChangesAsync(ct);
    }
}