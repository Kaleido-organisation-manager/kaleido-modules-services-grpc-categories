using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using static Kaleido.Grpc.Categories.GrpcCategories;

namespace Kaleido.Modules.Services.Grpc.Categories.Client.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCategoryClient(this IServiceCollection services, string connectionString)
    {
        var channel = GrpcChannel.ForAddress(connectionString);
        var client = new GrpcCategoriesClient(channel);
        services.AddSingleton(client);
        return services;
    }
}
