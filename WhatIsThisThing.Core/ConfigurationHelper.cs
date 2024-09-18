using Microsoft.Extensions.Configuration;
using System.IO;

namespace WhatIsThisThing.Core
{
    public static class ConfigurationHelper
    {
        public static IConfigurationRoot GetConfiguration()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

#if DEBUG
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")))
            {
                environment = "Development";
            }
#endif

            var basePath = AppContext.BaseDirectory;

            return new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddEnvironmentVariables()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .Build();
        }
    }
}
