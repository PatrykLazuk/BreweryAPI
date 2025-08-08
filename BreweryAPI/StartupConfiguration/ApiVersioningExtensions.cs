using Asp.Versioning;
using Microsoft.Extensions.DependencyInjection;

namespace BreweryAPI.StartupConfiguration
{
    public static class ApiVersioningExtensions
    {
        public static IServiceCollection AddApiVersioningWithDefaults(this IServiceCollection services)
        {
            services.AddApiVersioning(o =>
            {
                o.DefaultApiVersion = new ApiVersion(1, 0);
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.ReportApiVersions = true;
                o.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddMvc();

            return services;
        }
    }
}
