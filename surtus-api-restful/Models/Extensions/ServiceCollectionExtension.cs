using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace surtus_api_restful.Models
{
    public static class ServiceCollectionExtension
    {
        public static void AddSurtusContext(this IServiceCollection services, IConfiguration configuration, bool isDev)
        {
            var connString = configuration.GetValue<string>("ConnectionString") ?? throw new Exception("Must specify a connection string in the appsettings with the name 'ConnectionString'");
            services.AddDbContext<SurtusDbContext>(opts =>
            {
                if (isDev)
                {
                    opts.EnableSensitiveDataLogging();
                }
                //opts.UseSqlServer(connString);
                opts.UseSqlServer(connString,
                sqlServerOptionsAction: sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 10,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
                });
                opts.EnableDetailedErrors();
            });
        }
    }
}
