using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Autofac;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using System.Reflection;
using AutoMapper;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;
using TriviaDotNetApi.Infrastructure.EntityFramework.ServiceHelpers;
using TriviaDotNetApi.Infrastructure.EntityFramework.Repository;
using TriviaDotNetApi.Domain.AggregatesModel;
using TriviaDotNetApi.Infrastructure.EntityFramework.Extensions;
using TriviaDotNetApi.Application.Services;

namespace TriviaDotNetApi.API
{
    public class Startup
    {
        private const string ApplicationAssembly = "TriviaDotNetApi.Application";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ReplaceDBConfig();

            services.AddCustomSwagger(Configuration)
                .AddEFDbContext(Configuration)
                .AddHealthChecks(Configuration)
                .AddAutoMapper(cfg => cfg.AllowNullCollections = true, Assembly.Load(ApplicationAssembly))
                .AddLocalization(options => options.ResourcesPath = "Resources")
                .AddCustomMvc().AddControllers()
                    .AddViewLocalization()
                    .AddDataAnnotationsLocalization()
                    .AddApplicationPart(typeof(Startup).Assembly);

            services.AddLogging();

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new List<CultureInfo>
                    {
                        new CultureInfo("en-US"),
                        new CultureInfo("pt-BR")
                    };

                options.DefaultRequestCulture = new RequestCulture(Environment.GetEnvironmentVariable("DEFAULT_CULTURE") ?? "en-US");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            });

            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<GzipCompressionProvider>();

            });

            services.AddMvc();

            services.AddApiVersioning(
                    options =>
                    {
                        options.ReportApiVersions = false;
                        options.AssumeDefaultVersionWhenUnspecified = true;
                        options.DefaultApiVersion = new ApiVersion(1, 0);

                    });
            services.AddVersionedApiExplorer(
                options =>
                {
                    options.GroupNameFormat = "'v'VVV";
                    options.SubstituteApiVersionInUrl = true;
                });

            services.AddMvc().AddJsonOptions(options => {

                options.JsonSerializerOptions.MaxDepth = 256;

            });


        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.AddMediatorModules(Configuration);
            builder.RegisterType<TriviaDotNetApiSingleActionRepository>()
                .As<ITriviaSingleActionRepository>()
                .InstancePerLifetimeScope();
        }

            private void ReplaceDBConfig()
        {
            Configuration["ConnectionStrings:IntegrationPersistenceEventDbContext"] =
               Configuration["ConnectionStrings:IntegrationPersistenceEventDbContext"]
               .Replace("__DB_USER__", Environment.GetEnvironmentVariable("DB_USER"))
               .Replace("__DB_PWD__", Environment.GetEnvironmentVariable("DB_PWD"))
               .Replace("__DB_HOST__", Environment.GetEnvironmentVariable("DB_HOST"));

            Configuration["ConnectionStrings:EFDbContext"] =
                Configuration["ConnectionStrings:EFDbContext"]
                .Replace("__DB_USER__", Environment.GetEnvironmentVariable("DB_USER"))
                .Replace("__DB_PWD__", Environment.GetEnvironmentVariable("DB_PWD"))
                .Replace("__DB_HOST__", Environment.GetEnvironmentVariable("DB_HOST"));

            Configuration["ConnectionStrings:DefaultConnection"] =
                Configuration["ConnectionStrings:DefaultConnection"]
                .Replace("__DB_USER__", Environment.GetEnvironmentVariable("DB_USER"))
                .Replace("__DB_PWD__", Environment.GetEnvironmentVariable("DB_PWD"))
                .Replace("__DB_HOST__", Environment.GetEnvironmentVariable("DB_HOST"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostEnvironment env, IApiVersionDescriptionProvider provider)
        {

            AddCultureInfos(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            var pathBase = Configuration["PATH_BASE"];
            if (!string.IsNullOrEmpty(pathBase))
            {              
                app.UsePathBase(pathBase);
            }

            app.UseCors("CorsPolicy");

            app.UseRouting();

            app.UseResponseCompression();

            AddHealthChecks(app);

            AddSwagger(app, pathBase, provider);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            PerformPersistenceMigration(app);
        }

        private static void PerformPersistenceMigration(IApplicationBuilder app)
        {
            app.ApplicationServices.PerformEFDbContextMigration();
        }

        private static void AddCultureInfos(IApplicationBuilder app)
        {
            var supportedCultures = new[]
            {
                new CultureInfo("en-US"),
                new CultureInfo("pt-BR")
            };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(Environment.GetEnvironmentVariable("DEFAULT_CULTURE") ?? "en-US"),
                // Formatting numbers, dates, etc.
                SupportedCultures = supportedCultures,
                // UI strings that we have localized.
                SupportedUICultures = supportedCultures
            });
        }

        private static void AddHealthChecks(IApplicationBuilder app)
        {
            app.UseHealthChecks("/liveness", new HealthCheckOptions
            {
                Predicate = r => r.Name.Contains("self")
            });

            app.UseHealthChecks("/hc", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

        }

        private static void AddSwagger(IApplicationBuilder app, string pathBase, IApiVersionDescriptionProvider provider)
        {
            app.UseSwagger();
            app.UseSwaggerUI(
                options =>
                {
                    // build a swagger endpoint for each discovered API version
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        if (description.GroupName != "v1")
                            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                        else
                            options.SwaggerEndpoint($"{ (!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty) }/swagger/v1/swagger.json", "TriviaDotNetApi.API V1");
                    }

                    options.OAuthClientId("TriviaDotNetApi");
                    options.OAuthAppName("Trivia Swagger UI");
                });
        }
    }
    static class CustomExtensionsMethods
    {

        public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            var hcBuilder = services.AddHealthChecks();

            hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());

            hcBuilder
                .AddSqlServer(
                    configuration["ConnectionStrings:EFDbContext"],
                    name: "DB-check",
                    tags: new string[] { "db" });

            return services;
        }

        public static IServiceCollection AddCustomMvc(this IServiceCollection services)
        {

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .WithOrigins("*")
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });

            // Add framework services.
            services.AddMvc(options =>
            {
            }).AddControllersAsServices();

            return services;
        }

        public static IServiceCollection AddCustomSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(options =>
            {
                options.OperationFilter<RemoveVersionFromParameter>();
                options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "TriviaDotNetApi HTTP API",
                    Version = "v1",
                    Description = "The TriviaDotNetApi Service HTTP API"
                });

                options.SwaggerDoc("v2", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "TriviaDotNetApi HTTP API",
                    Version = "v2",
                    Description = "The TriviaDotNetApi Service HTTP API v2"
                });
            });

            return services;
        }
    }
}