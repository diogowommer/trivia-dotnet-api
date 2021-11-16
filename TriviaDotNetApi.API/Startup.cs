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
using TriviaDotNetApi.Infrastructure.EntityFramework.Context;
using Autofac.Extensions.DependencyInjection;
using TriviaDotNetApi.Infrastructure.EntityFramework.Repository;
using TriviaDotNetApi.Domain.AggregatesModel;
using TriviaDotNetApi.Infrastructure.EntityFramework.Extensions;
using Saunter;
using Saunter.AsyncApiSchema.v2;
using Saunter.Generation;
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
                //.AddPersistenceEventDbContext(Configuration)
                //.AddLocalPersistenceServices(ApplicationAssembly)
                //.AddCommonEventBusServices()
                //.AddIntegrationEvents(Configuration)
                .AddHealthChecks(Configuration)
                //.AddScopeManagement(Configuration)
                .AddAutoMapper(cfg => cfg.AllowNullCollections = true, Assembly.Load(ApplicationAssembly))
                .AddLocalization(options => options.ResourcesPath = "Resources")
                .AddCustomMvc().AddControllers()
                    .AddViewLocalization()
                    .AddDataAnnotationsLocalization()
                    .AddApplicationPart(typeof(Startup).Assembly);

            //services.AddTransient<IIntegrationEventHeaderBuilder, MessageHeaderBuilder>();

            services.AddLogging();

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new List<CultureInfo>
                    {
                        new CultureInfo("en-US"),
                        new CultureInfo("fr"),
                        new CultureInfo("pt-BR")
                    };

                options.DefaultRequestCulture = new RequestCulture(Environment.GetEnvironmentVariable("DEFAULT_CULTURE") ?? "pt-BR");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            });


            services.AddAsyncApiSchemaGeneration(options =>
            {
                // Specify example type(s) from assemblies to scan.
                options.AssemblyMarkerTypes = Assembly.Load("TriviaDotNetApi.Application").GetExportedTypes();

                // Build as much (or as little) of the AsyncApi document as you like.
                // Saunter will generate Channels, Operations, Messages, etc, but you
                // may want to specify Info here.
                options.AsyncApi = new AsyncApiDocument
                {
                    Info = new Info("TriviaDotNetApi API", "1.0.0")
                    {
                        Description = "TriviaDotNetApi Async Interface.",
                        License = new License("Apache 2.0")
                        {
                            Url = "https://www.apache.org/licenses/LICENSE-2.0"
                        }
                    }
                    ,
                    //Servers =
                    //    {
                    //        { "RabbitMQ", new Server("localhost", "amqp") }
                    //    }
                };
            });

            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<GzipCompressionProvider>();

            });



            services.AddMvc();

            /*
                Versionamento de serviços
            */
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
            /*
              Fim Versionamento de serviços
            */


            services.AddMvc().AddJsonOptions(options => {

                options.JsonSerializerOptions.MaxDepth = 256;

            });


        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.AddMediatorModules(Configuration);
            //builder.AddTenantInAutoFac(Configuration);
            //builder.AddMassTransitModule(GetEventBusConfiguration(Configuration, nameof(EFDbContext)));

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

        //private EventbusConfiguration GetEventBusConfiguration(IConfiguration configuration, string dbContextName)
        //{
        //    var consumersAssembly = Assembly.Load(ApplicationAssembly);

        //    return new EventbusConfiguration(
        //       configuration["EventBusConnection"],
        //       "/",
        //       configuration["EventBusUserName"],
        //       configuration["EventBusPassword"],
        //       consumersAssembly,
        //       appConfig: configuration,
        //       dbContextName: dbContextName
        //   );
        //}

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
                //loggerFactory.CreateLogger<Startup>().LogDebug("Using PATH BASE '{pathBase}'", pathBase);
                app.UsePathBase(pathBase);
            }

            app.UseCors("CorsPolicy");

            //app.UseMvc();

            app.UseRouting();

            app.UseResponseCompression();

            AddHealthChecks(app);

            AddSwagger(app, pathBase, provider);

            //app.UseMultitenancy(nameof(EFDbContext), Configuration);

            app.UseMiddleware<AsyncApiMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            PerformPersistenceMigration(app);

            //ConfigureEventBus(app);
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
                new CultureInfo("fr"),
                new CultureInfo("pt-BR")
            };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(Environment.GetEnvironmentVariable("DEFAULT_CULTURE") ?? "pt-BR"),
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

                    /*

                    Depois de Migrar todo front end para v2, deve trocar o foreach acima por este aqui

                    foreach (var description in provider.ApiVersionDescriptions)
                    {       
                        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());                       
                    }
                    
                    */

                    options.OAuthClientId("TriviaDotNetApi");
                    options.OAuthAppName("Trivia Swagger UI");
                });
        }

        //private void ConfigureEventBus(IApplicationBuilder app)
        //{
        //    var eventBus = app.ApplicationServices.GetRequiredService<IBusControl>();

        //    eventBus.Start();
        //}

    }
    static class CustomExtensionsMethods
    {

        public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            var hcBuilder = services.AddHealthChecks();

            hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());

            //hcBuilder
            //    .AddSqlServer(
            //        configuration["ConnectionStrings:IntegrationPersistenceEventDbContext"],
            //        name: "integration-persistence-DB-check",
            //        tags: new string[] { "ipdb" });

            hcBuilder
                .AddSqlServer(
                    configuration["ConnectionStrings:EFDbContext"],
                    name: "mes-DB-check",
                    tags: new string[] { "mesdb" });

            //amqp://user:pass@host:10000/vhost
            //hcBuilder
            //        .AddRabbitMQ(
            //            String.Format("amqp://{0}:{1}@{2}:5672/%2f",
            //            configuration["EventBusUserName"],
            //            configuration["EventBusPassword"],
            //            configuration["EventBusConnection"]),
            //            name: "TriviaDotNetApi-rabbitmqbus-check",
            //            tags: new string[] { "rabbitmqbus" });


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
                //options.Filters.Add(typeof(TenantAutomaticMigrateActionFilter));
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