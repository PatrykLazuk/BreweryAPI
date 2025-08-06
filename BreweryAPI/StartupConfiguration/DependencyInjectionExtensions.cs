using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BreweryAPI.Logic;
using BreweryAPI.Logic.Interfaces;
using BreweryAPI.Repositories;
using BreweryAPI.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BreweryAPI.StartupConfiguration
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddBreweryDependencies(this IServiceCollection services)
        {
            services.AddScoped<IBreweryLogic, BreweryLogic>();
            // Register the EF Core repository
            services.AddScoped<IBreweryRepository, BreweryEfRepository>();

            // Uncomment the following line if you want to use an external API repository instead
            // services.AddHttpClient<IBreweryRepository, BreweryApiRepository>(client =>
            // {
            //     client.BaseAddress = new Uri("https://api.openbrewerydb.org/v1/");
            // });

            return services;
        }
    }
}