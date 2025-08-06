using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BreweryAPI.Logic;
using BreweryAPI.Logic.Interfaces;
using BreweryAPI.Repositories;
using BreweryAPI.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BreweryAPI.StartupConfiguration
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddBreweryDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            string dataSource = configuration["DataSource"] ?? "Database";
            if (dataSource.Equals("Api", StringComparison.OrdinalIgnoreCase))
            {
                services.AddHttpClient<IBreweryRepository, BreweryApiRepository>(client =>
                {
                    client.BaseAddress = new Uri("https://api.openbrewerydb.org/v1/");
                });
            }
            else
            {
                services.AddScoped<IBreweryRepository, BreweryEfRepository>();
            }

            services.AddScoped<IBreweryLogic, BreweryLogic>();

            return services;
        }
    }
}