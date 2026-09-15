using Microsoft.EntityFrameworkCore;
using TechCart.Products.Application.Abstractions;
using TechCart.Products.Domain.Entities;
using TechCart.SharedKernel;

namespace TechCart.Products.Infrastructure.Repositories;

public class ProductReadRepository : IProductReadRepository
{
    private readonly ProductsDbContext _dbContext;
    public ProductReadRepository(ProductsDbContext dbContext) => _dbContext = dbContext;

    public async Task<PagedResult<ProductRowDto>> SearchAsync(ProductSearchFilter filter, CancellationToken ct)
    {
        // tum degerler true: kullanici hangi filtreleri sectiyse urun listesine hepsi uygulanir.
        // ornegin Kategori: Telefon, Marka: Samsung, Renk: Siyah -> sadece siyah Samsung telefonlar listelenir
        var query = ApplyFilters(_dbContext.Products.AsNoTracking(), filter,
            includeCategory: true, includeBrand: true, includeColor: true);

        // filtreye uyan toplam urun sayisi
        var totalCount = await query.CountAsync(ct);

        query = filter.SortBy switch
        {
            "price_asc" => query.OrderBy(p => p.Price),
            "price_desc" => query.OrderByDescending(p => p.Price),
            "newest" => query.OrderByDescending(p => p.CreatedAt),
            _ => query.OrderByDescending(p => p.CreatedAt),
        };

        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(p => new ProductRowDto(p.Id, p.BrandId, p.Name, p.Price, p.VatRate))
            .ToListAsync(ct);

        return new PagedResult<ProductRowDto>(items, totalCount, filter.Page, filter.PageSize);
    }

    public async Task<List<CategoryFacetDto>> GetCategoryFacetsAsync(ProductSearchFilter filter, CancellationToken ct)
    {
        var query = ApplyFilters(_dbContext.Products.AsNoTracking(), filter,
            includeCategory: false, includeBrand: true, includeColor: true); // kendi boyutu hariç

        return await query
            .GroupBy(p => p.CategoryId)
            .Select(g => new CategoryFacetDto(g.Key, g.Count()))
            .ToListAsync(ct);
    }

    public async Task<List<BrandFacetDto>> GetBrandFacetsAsync(ProductSearchFilter filter, CancellationToken ct)
    {
        var query = ApplyFilters(_dbContext.Products.AsNoTracking(), filter,
            includeCategory: true, includeBrand: false, includeColor: true);

        return await query
            .GroupBy(p => p.BrandId)
            .Select(g => new BrandFacetDto(g.Key, g.Count()))
            .ToListAsync(ct);
    }

    public async Task<List<ColorFacetDto>> GetColorFacetsAsync(ProductSearchFilter filter, CancellationToken ct)
    {
        var query = ApplyFilters(_dbContext.Products.AsNoTracking(), filter,
            includeCategory: true, includeBrand: true, includeColor: false);

        return await query
            .Where(p => p.Color != "") // boş renk bilgisi facet'te anlamsız
            .GroupBy(p => p.Color)
            .Select(g => new ColorFacetDto(g.Key, g.Count()))
            .ToListAsync(ct);
    }

    public async Task<(decimal Min, decimal Max)> GetPriceRangeAsync(ProductSearchFilter filter, CancellationToken ct)
    {
        var query = ApplyFilters(_dbContext.Products.AsNoTracking(), filter,
            includeCategory: true, includeBrand: true, includeColor: true, includePrice: false);

        if (!await query.AnyAsync(ct)) return (0, 0);

        return (await query.MinAsync(p => p.Price, ct), await query.MaxAsync(p => p.Price, ct));
    }

    public Task<ProductDetailRowDto?> GetByIdAsync(Guid id, CancellationToken ct)
        => _dbContext.Products
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new ProductDetailRowDto(p.Id, p.CategoryId, p.BrandId, p.Name, p.Model,
                p.Description, p.Specs, p.Price, p.VatRate))
            .FirstOrDefaultAsync(ct);

    public Task<List<string>> SearchProductNamesAsync(string term, int limit, CancellationToken ct)
        => _dbContext.Products
            .AsNoTracking()
            .Where(p => EF.Functions.ILike(p.Name, $"%{term}%"))
            .OrderBy(p => p.Name)
            .Select(p => p.Name)
            .Take(limit)
            .ToListAsync(ct);

    // Butun ortak filtreleri uygular.
    private static IQueryable<Product> ApplyFilters(IQueryable<Product> query, ProductSearchFilter filter,
        bool includeCategory, bool includeBrand, bool includeColor, bool includePrice = true)
    {
        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            query = query.Where(p => EF.Functions.ILike(p.Name, $"%{filter.SearchTerm}%"));

        if (includeCategory && filter.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == filter.CategoryId.Value);

        if (includeBrand && filter.BrandId.HasValue)
            query = query.Where(p => p.BrandId == filter.BrandId.Value);

        if (includeColor && !string.IsNullOrWhiteSpace(filter.Color))
            query = query.Where(p => p.Color == filter.Color);

        if (includePrice)
        {
            if (filter.MinPrice.HasValue) query = query.Where(p => p.Price >= filter.MinPrice.Value);
            if (filter.MaxPrice.HasValue) query = query.Where(p => p.Price <= filter.MaxPrice.Value);
        }

        return query;
    }
}