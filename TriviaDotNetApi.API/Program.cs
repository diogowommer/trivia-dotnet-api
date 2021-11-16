using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Polly;
using Autofac.Extensions.DependencyInjection;

namespace TriviaDotNetApi.API
{
    public class Program
    {
        public static readonly string Namespace = typeof(Program).Namespace;
        public static readonly string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);
        public static void Main(string[] args)
        {
            var configuration = GetConfiguration();

            Log.Logger = CreateSerilogLogger(configuration);

            var webHost = CreateWebHostBuilder(configuration, args);

            var policy = Policy.Handle<Exception>().WaitAndRetryForever(retryAttempt =>
                                                                TimeSpan.FromSeconds(10),
                                                                (exception, retryCount, timeSpan) => {
                                                                    Log.Logger.Error("-----------------------  Retrying " + retryCount + " Time");
                                                                });
            webHost.Run();
        }

        public static IHost CreateWebHostBuilder(IConfiguration configuration, string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webHostBuilder => {
                    webHostBuilder
                      .UseConfiguration(configuration)
                      .UseStartup<Startup>();
                })
                .Build();

        private static Serilog.ILogger CreateSerilogLogger(IConfiguration configuration)
        {

            return new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.WithProperty("ApplicationContext", AppName)
                .Enrich.FromLogContext()
                .WriteTo.Console()           
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }

        public static IConfiguration GetConfiguration()
        {
            var appSettings = "appsettings.json";

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            if (!string.IsNullOrWhiteSpace(environment))
            {
                appSettings = string.Format("appsettings.{0}.json", environment);
            }

            if (String.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("DB_USER")))
                Environment.SetEnvironmentVariable("DB_USER", "sa");

            if (String.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("DB_PWD")))
                Environment.SetEnvironmentVariable("DB_PWD", "huR*P!Zb952eHV");

            if (String.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("DB_HOST")))
                Environment.SetEnvironmentVariable("DB_HOST", environment == "Development" ? "localhost,1400" : "sqlserver");

            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile(appSettings, optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            return builder.Build();
        }

    }
}