using TechCart.Products.Domain.Repositories;

namespace TechCart.Products.Application.UpdateStock;

public record UpdateProductStockCommand(Guid ProductId, int NewStock);

public class UpdateProductStockHandler(IProductWriteRepository productWriteRepository)
{
    public async Task Handle(UpdateProductStockCommand command, CancellationToken ct)
    {
        var product = await productWriteRepository.GetByIdAsync(command.ProductId, ct);

        // Ürün silinmiş olabilir — bu durumda sessizce hiçbir şey yapmıyoruz,
        // stok senkronizasyonunun tüm siparişi/ödemeyi başarısız kılmasına gerek yok.
        if (product is null)
            return;

        product.SetStock(command.NewStock);
        await productWriteRepository.SaveChangesAsync(ct);
    }
}