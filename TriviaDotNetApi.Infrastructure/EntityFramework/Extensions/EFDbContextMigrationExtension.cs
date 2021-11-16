using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TriviaDotNetApi.Infrastructure.EntityFramework.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace TriviaDotNetApi.Infrastructure.EntityFramework.Extensions
{
    public static class EFDbContextMigrationExtension
    {
        public static IServiceProvider PerformEFDbContextMigration(this IServiceProvider services)
        {
            using var serviceScope = services.GetService<IServiceScopeFactory>().CreateScope();
            var context = serviceScope.ServiceProvider.GetRequiredService<EFDbContext>();
            context.Database.Migrate();

            return services;
        }
    }
}
