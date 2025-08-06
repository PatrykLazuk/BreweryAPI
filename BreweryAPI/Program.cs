using System;
using Microsoft.AspNetCore.Builder;
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

                // Configure Serilog as the logging provider
                builder.Host.UseSerilog();

                var startup = new Startup(builder.Configuration);
                startup.ConfigureServices(builder.Services);

                var app = builder.Build();
                startup.Configure(app, builder.Environment);

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