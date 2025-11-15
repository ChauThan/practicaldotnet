using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
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
            }
            else
            {
                services.AddSingleton<ILinkRepository, InMemoryLinkRepository>();
            }
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
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}