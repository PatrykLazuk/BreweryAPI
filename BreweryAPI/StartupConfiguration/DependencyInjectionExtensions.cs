using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BreweryAPI.Logic;
using BreweryAPI.Logic.Interfaces;
using BreweryAPI.Models;
using BreweryAPI.Repositories;
using BreweryAPI.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Polly;
using Polly.Extensions.Http;

namespace BreweryAPI.StartupConfiguration
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddBreweryDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            string dataSource = configuration[Constants.Configuration.DataSourceKey] ?? Constants.Configuration.DefaultDataSource;
            if (dataSource.Equals(DataSource.Api.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                // Setup HTTP client with resilience
                services.AddHttpClient<IBreweryRepository, BreweryApiRepository>(client =>
                {
                    client.BaseAddress = new Uri(Constants.Api.BaseUrl);
                    client.Timeout = TimeSpan.FromSeconds(Constants.Api.TimeoutSeconds);
                    client.DefaultRequestHeaders.Add("User-Agent", Constants.Api.UserAgent);
                })
                .AddPolicyHandler(GetRetryPolicy())
                .AddPolicyHandler(GetCircuitBreakerPolicy());
            }
            else
            {
                services.AddScoped<IBreweryRepository, BreweryEfRepository>();
            }

            services.AddScoped<IBreweryLogic, BreweryLogic>();

            return services;
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(Constants.Http.RetryCount, retryAttempt => 
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(Constants.Http.CircuitBreakerThreshold, TimeSpan.FromSeconds(Constants.Http.CircuitBreakerTimeoutSeconds));
        }
    }
}