using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechCart.Search.Application.Abstractions;
using TechCart.Search.Application.Reindex;

namespace TechCart.Search.Infrastructure;

public static class SearchModule
{
    // API ile Elasticsearch arasındaki iletişim aracını hazırlar
    public static IServiceCollection AddSearchModule(this IServiceCollection services, IConfiguration configuration)
    {
        var url = configuration["Elasticsearch:Url"] ?? "http://localhost:9200";

        services.AddSingleton(_ => new ElasticsearchClient(new ElasticsearchClientSettings(new Uri(url))));
        services.AddSingleton<IProductSearchIndex, ProductSearchIndex>();
        services.AddScoped<ReindexProductsHandler>();
        return services;
    }
}