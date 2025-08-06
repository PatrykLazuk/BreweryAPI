using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BreweryAPI.Infrastructure;
using Microsoft.AspNetCore.Builder;

namespace BreweryAPI.StartupConfiguration
{
    public static class ErrorHandlingExtensions
    {
        public static IApplicationBuilder UseGlobalErrorHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}