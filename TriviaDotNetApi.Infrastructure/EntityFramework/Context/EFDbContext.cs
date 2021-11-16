using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using TriviaDotNetApi.Domain.AggregatesModel;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using CrossCutting.SeedWork.Classes;

namespace TriviaDotNetApi.Infrastructure.EntityFramework.Context
{
    public class EFDbContext : DbContextBase
    {
        public const string DEFAULT_SCHEMA = nameof(TriviaDotNetApi);

        public static string DEFAULT_MIGRATIONS_TABLE => "__EFMigrationsHistory";
        public string UniqueId { get; }
                
        public DbSet<TriviaItem> TriviaItem { get; set; }

        public EFDbContext(DbContextOptions<EFDbContext> options, IMediator mediator) : base(options, mediator)
        {
            UniqueId = Guid.NewGuid().ToString();

            this.Database.SetCommandTimeout(180);

            Debug.WriteLine("MESDbContext::ctor ->" + GetHashCode());
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema(DEFAULT_SCHEMA);            
        }

        public override void Dispose() =>
            base.Dispose();
    }

    public class EFDbContextDesignFactory : IDesignTimeDbContextFactory<EFDbContext>
    {
        public EFDbContext CreateDbContext(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            //var connectionString = builder.GetSection("Configuration").GetConnectionString(nameof(EFDbContext));
            var connectionString = builder.GetConnectionString(nameof(EFDbContext));

            var optionsBuilder = new DbContextOptionsBuilder<EFDbContext>().UseSqlServer(connectionString);

            return new EFDbContext(optionsBuilder.Options, new NoMediator());
        }

        public class NoMediator : IMediator
        {
            public Task Publish(object notification, CancellationToken cancellationToken = default)
            {
                return Task.CompletedTask;
            }

            public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
            {
                return Task.CompletedTask;
            }

            public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
            {
                return Task.FromResult<TResponse>(default);
            }

            public Task<object> Send(object request, CancellationToken cancellationToken = default)
            {
                return Task.FromResult<object>(default);
            }
        }
    }
}
