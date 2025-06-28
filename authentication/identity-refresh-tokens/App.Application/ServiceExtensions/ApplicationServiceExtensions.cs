using App.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace App.Application.ServiceExtensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddScoped<IJwtService, JwtService>();
        return services;
    }
}