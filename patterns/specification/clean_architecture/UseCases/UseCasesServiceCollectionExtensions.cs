using App.UseCases;

namespace Microsoft.Extensions.DependencyInjection;

public static class UseCasesServiceCollectionExtensions
{
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        services.AddTransient<GetProductsHandler>();
        
        return services;
    }
}