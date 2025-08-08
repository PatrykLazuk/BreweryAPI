using System;
using BreweryAPI.Models;
using BreweryAPI.StartupConfiguration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BreweryAPI
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration) => _configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddMemoryCache();
            services.AddCorsPolicy(_configuration);
            services.AddDatabase(_configuration);
            services.AddApiVersioningWithDefaults();
            services.AddBreweryDependencies(_configuration);
            services.AddJwtAuthentication(_configuration);
            services.AddAuthorization();
        }

        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            app.UseGlobalErrorHandling();

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();
            app.UseSecurityHeaders();
            app.UseRouting();
            app.UseCors(Constants.Cors.DefaultPolicyName);
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Setup database
            await app.InitializeDatabaseAsync(logger);
        }
    }
}