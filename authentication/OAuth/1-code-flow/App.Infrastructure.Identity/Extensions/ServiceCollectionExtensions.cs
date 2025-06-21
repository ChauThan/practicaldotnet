using App.Infrastructure.Identity.Data;
using App.Infrastructure.Identity.IdentityServerConfig;
using App.Infrastructure.Identity.IdentityServerConfig.Services;
using App.Infrastructure.Identity.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;

namespace App.Infrastructure.Identity.Extensions;

/// <summary>
/// Extension methods for registering identity and IdentityServer services.
/// </summary>
public static class ServiceCollectionExtensions
{
    private const string SigningKeyDirectory = "../.config/keys";
    private const string SigningKeyFileName = "tempkey.jwk";

    /// <summary>
    /// Adds EF Core Identity and configures password and sign-in options.
    /// </summary>
    public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 4;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        return services;
    }

    /// <summary>
    /// Adds and configures Duende IdentityServer with in-memory resources and signing credentials.
    /// </summary>
    public static IServiceCollection AddIdentityServerConfig(this IServiceCollection services)
    {
        var builder = services.AddIdentityServer(options =>
        {
            options.Events.RaiseErrorEvents = true;
            options.Events.RaiseInformationEvents = true;
            options.Events.RaiseFailureEvents = true;
            options.Events.RaiseSuccessEvents = true;
            options.EmitStaticAudienceClaim = true;
        })
        .AddInMemoryIdentityResources(Config.IdentityResources)
        .AddInMemoryApiScopes(Config.ApiScopes)
        .AddInMemoryClients(Config.Clients)
        .AddAspNetIdentity<ApplicationUser>();

        // Ensure signing key exists and add as signing credential
        var signingCredentials = EnsureOrCreateSigningKey(SigningKeyDirectory, SigningKeyFileName);
        builder.AddSigningCredential(signingCredentials);

        services.AddTransient<IProfileService, ProfileService>();
        return services;
    }

    /// <summary>
    /// Ensures a JWK signing key exists at the given path, creates one if missing, and returns SigningCredentials.
    /// </summary>
    private static SigningCredentials EnsureOrCreateSigningKey(string directory, string fileName)
    {
        var jwkDir = Path.GetFullPath(directory);
        var jwkPath = Path.Combine(jwkDir, fileName);

        if (!Directory.Exists(jwkDir))
            Directory.CreateDirectory(jwkDir);

        if (!File.Exists(jwkPath))
        {
            using var rsa = RSA.Create(2048);
            var parameters = rsa.ExportParameters(true);
            var jsonWebKey = JsonWebKeyConverter.ConvertFromRSASecurityKey(new RsaSecurityKey(parameters));
            jsonWebKey.Alg = SecurityAlgorithms.RsaSha256;
            var jwkJson = JsonSerializer.Serialize(jsonWebKey);
            File.WriteAllText(jwkPath, jwkJson);
        }

        var loadedJwkJson = File.ReadAllText(jwkPath);
        var loadedJsonWebKey = new JsonWebKey(loadedJwkJson);
        return new SigningCredentials(loadedJsonWebKey, SecurityAlgorithms.RsaSha256);
    }
}
