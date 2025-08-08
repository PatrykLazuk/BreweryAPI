using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace BreweryAPI
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .CreateLogger();

            try
            {
                Log.Information("Starting BreweryAPI");

                // Setup Serilog
                builder.Host.UseSerilog();

                var startup = new Startup(builder.Configuration);
                startup.ConfigureServices(builder.Services);

                var app = builder.Build();
                
                // Get logger for startup
                var logger = app.Services.GetRequiredService<ILogger<Startup>>();
                startup.Configure(app, builder.Environment, logger);

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application BreweryAPI failed to start");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}