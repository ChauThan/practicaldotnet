using System.Reflection;
using App.Core;
using App.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>(opt =>
        {
            opt.UseSqlite("Data Source=app.db");
        });

        services.AddAutoMapper(config => 
            config.AddMaps(Assembly.GetExecutingAssembly()));

        services.AddScoped<IProductRepository, ProductRepository>();
        
        return services;
    }
}