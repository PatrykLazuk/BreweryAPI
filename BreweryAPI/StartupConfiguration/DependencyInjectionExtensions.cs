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
            //services.AddScoped<IBreweryRepository, BreweryApiRepository>();
            services.AddHttpClient<IBreweryRepository, BreweryApiRepository>(client =>
            {
                client.BaseAddress = new Uri("https://api.openbrewerydb.org/v1/");
            });

            return services;
        }
    }
}