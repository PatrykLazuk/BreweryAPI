using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using BreweryAPI.Data;
using BreweryAPI.Models;

namespace BreweryAPI.StartupConfiguration
{
    public static class DatabaseExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<BreweryDbContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

            return services;
        }

        public static async Task InitializeDatabaseAsync(this IApplicationBuilder app, ILogger logger)
        {
            try
            {
                using var scope = app.ApplicationServices.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<BreweryDbContext>();
                var dbLogger = scope.ServiceProvider.GetRequiredService<ILogger<BreweryDbContext>>();
                
                db.Database.Migrate(); // Create DB and apply migrations
                await DbSeeder.SeedAsync(db, dbLogger);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to initialize database during startup");
                // Continue startup even if DB init fails
            }
        }
    }
}
