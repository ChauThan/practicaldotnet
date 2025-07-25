﻿using App.Application.Abstractions;
using App.Domain;
using App.Infrastructure.Persistence;
using App.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace App.Infrastructure.ServiceExtensions;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.Password.RequiredUniqueChars = 0;

            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = false; // Temporirily disable email confirmation for demo purposes
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        // Register application services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ISignInService, SignInService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IRoleService, RoleService>();

        return services;
    }
}
