using TechCart.Products.Application.Dtos.RequestDtos;
using TechCart.Products.Application.ListForIndexing;
using TechCart.Search.Application.Abstractions;
using TechCart.Search.Application.Documents;

namespace TechCart.Search.Application.Reindex;

// PostgreSQL’den bir seferde kaç ürün okunacağını belirliyor.
// sayfa buyuklugu 500. Cok kucuk olursa cok fazla ayri istek atilir. Cok buyuk olursa yuk olusturur
public record ReindexProductsCommand(int PageSize = 500);

public record ReindexProductsResult(int TotalIndexed);

// PostgreSQL'deki urunleri sayfa sayfa okuyup, her urunu dokumana donusturerek 
// Elasticsearch'teki urun indeksini bastan doldurur.
public class ReindexProductsHandler(
    IProductSearchIndex productSearchIndex,
    ListProductsForIndexingHandler listProductsForIndexingHandler)
{
    public async Task<ReindexProductsResult> Handle(ReindexProductsCommand command, CancellationToken ct)
    {
        // İndeksi sıfırlıyoruz — silinmiş ürünleri remizler
        await productSearchIndex.RecreateAsync(ct);

        var totalIndexed = 0;
        var page = 1;

        while (true)
        {
            // product modulunden ham urun satirlarini alir. ilk turda (1,500), ikinci turda (2,500)
            var items = await listProductsForIndexingHandler.Handle(
                new ListProductsForIndexingQuery(page, command.PageSize), ct);

            // bos sayfaya ulasinca artik okunacak urun kalmadi birak
            if (items.Count == 0)
                break; 
            
            // okunan her urun ToDocument ile ProductSearchDocument nesnesine cevrilir. ToList() ile donusturulmus dokumanlari liste olarak verir
            var documents = items.Select(ProductSearchDocumentMapper.ToDocument).ToList();
           
            // Elasticsearch’e yazar. o sayfadaki urunlerin her biri icin bir dokuman olusturuldu.
            // IndexManyAsync fonks ile Toplu olarak Elasticsearch'e yazilir
            await productSearchIndex.IndexManyAsync(documents, ct);

            // kac urun yazildigini takip edip en son postgres'teki gercek urun sayisiyla karsilastirarak dogrulariz
            totalIndexed += documents.Count;
            page++;
        }

        return new ReindexProductsResult(totalIndexed);
    }
}