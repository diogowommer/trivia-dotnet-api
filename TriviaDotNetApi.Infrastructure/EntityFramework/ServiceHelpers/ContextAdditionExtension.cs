using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TriviaDotNetApi.Infrastructure.EntityFramework.Context;
using System;
using CrossCutting.SeedWork.Classes;

namespace TriviaDotNetApi.Infrastructure.EntityFramework.ServiceHelpers
{
    public static class ContextAdditionExtension
    {
        public static IServiceCollection AddEFDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddDbContext<EFDbContext>(options =>
                {
                    options.UseSqlServer(configuration.GetConnectionString(nameof(EFDbContext)),
                        sqlServerOptionsAction: sqlOptions =>
                        {
                            sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                            sqlOptions.MigrationsHistoryTable(EFDbContext.DEFAULT_MIGRATIONS_TABLE, nameof(TriviaDotNetApi));
                        });
                }, ServiceLifetime.Scoped);

            services.AddScoped<DbContextBase>(provider => provider.GetService<EFDbContext>());

            return services;
        }
    }
}
