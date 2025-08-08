using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BreweryAPI.Models;

namespace BreweryAPI.StartupConfiguration
{
    public static class CorsExtensions
    {
        public static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(Constants.Cors.DefaultPolicyName, policy =>
                {
                    var corsOrigins = configuration[Constants.Configuration.CorsOriginsKey];
                    
                    if (string.IsNullOrEmpty(corsOrigins))
                    {
                        // Use default dev origins
                        var origins = Constants.Cors.DevelopmentOrigins.Split(',');
                        policy.WithOrigins(origins)
                              .AllowAnyMethod()
                              .AllowAnyHeader()
                              .AllowCredentials();
                    }
                    else if (corsOrigins == "*")
                    {
                        // Allow all origins (careful in production)
                        policy.AllowAnyOrigin()
                              .AllowAnyMethod()
                              .AllowAnyHeader();
                    }
                    else
                    {
                        // Use configured origins
                        var origins = corsOrigins.Split(',');
                        policy.WithOrigins(origins)
                              .AllowAnyMethod()
                              .AllowAnyHeader()
                              .AllowCredentials();
                    }
                });
            });

            return services;
        }
    }
}
