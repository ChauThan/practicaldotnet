using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using System.Text;
using ShortLink.Infrastructure.Identity;
using ShortLink.Api.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using ShortLink.Infrastructure.Repositories;
using ShortLink.Infrastructure;
using ShortLink.Core.Interfaces;
using ShortLink.Core.Services;

namespace ShortLink.Api
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            // Register ShortLink.Core services
            services.AddSingleton<ILinkService, LinkService>();

            // Toggle repository type — keep InMemory by default for test/development
            var useEf = _configuration.GetValue<bool>("UseEfRepository", false);
            if (useEf)
            {
                services.AddDbContext<ShortLinkDbContext>(options =>
                    options.UseSqlite(_configuration.GetConnectionString("DefaultConnection") ?? "Data Source=shortlink.db"));

                services.AddScoped<ILinkRepository, EfLinkRepository>();
                // Identity (requires EF-backed DB)
                services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<ShortLinkDbContext>()
                    .AddDefaultTokenProviders();

                // Token service needs UserManager, only register when Identity is configured
                services.AddScoped<ITokenService, TokenService>();
            }
            else
            {
                services.AddSingleton<ILinkRepository, InMemoryLinkRepository>();
            }


            // Authentication and authorization will still be registered so we can protect admin endpoints when UseEfRepository is enabled.

            var jwtSection = _configuration.GetSection("Jwt");
            var jwtKey = jwtSection.GetValue<string>("Key");
            var jwtIssuer = jwtSection.GetValue<string>("Issuer");
            var jwtAudience = jwtSection.GetValue<string>("Audience");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey ?? string.Empty)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience
                };
            });

            services.AddAuthorization();


        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}