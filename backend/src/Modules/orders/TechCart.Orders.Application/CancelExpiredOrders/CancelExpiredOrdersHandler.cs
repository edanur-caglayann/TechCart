using TechCart.Inventory.Application.Dtos.RequestDtos;
using TechCart.Inventory.Application.Release;
using TechCart.OrderItems.Application.Dtos.RequestDtos;
using TechCart.OrderItems.Application.List;
using TechCart.Orders.Domain.Repositories;

namespace TechCart.Orders.Application.CancelExpiredOrders;

public class CancelExpiredOrdersHandler(
    IOrderWriteRepository orderWriteRepository,
    ListOrderItemsHandler listOrderItemsHandler,
    ReleaseReservationHandler releaseReservationHandler)
{
    // rezervasyon suresi tanimlariz
    private static readonly TimeSpan ReservationDuration = TimeSpan.FromMinutes(1);

    // 15 dk boyunca odemesi tamamlanmayan siparisleri (AwaitingPaymnet duurmunda olan) otomatik iptal eder 
    public async Task<int> Handle(CancellationToken ct)
    {
        // simdiki UTC zamanindan 15 dk ciakrilir. cutoffTime degiskenine atilri
        var cutoffTime = DateTime.UtcNow - ReservationDuration;
        // cutoffTime zamanindan once olusturulmus ve haal bekleyen siparisler suresi dolmus kabul edilir
        var expiredOrders = await orderWriteRepository.GetExpiredAwaitingPaymentOrdersAsync(cutoffTime, ct);

        // suresi dolmus siparisler alinir
        foreach (var order in expiredOrders)
        {
            // siparisin urunleri alinir
            var orderItems = await listOrderItemsHandler.Handle(new ListOrderItemsQuery(order.Id), ct);
            
            foreach (var item in orderItems.Where(i => i.ProductId.HasValue))
            {
                // ayrilmis stoklar serbest birakilir
                await releaseReservationHandler.Handle(
                    new ReleaseReservationCommand(item.ProductId!.Value, item.Quantity), ct);
            }

            order.Cancel();
        }

        await orderWriteRepository.SaveChangesAsync(ct);

        return expiredOrders.Count;
    }
}