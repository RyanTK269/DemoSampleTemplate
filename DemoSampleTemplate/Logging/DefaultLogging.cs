using Azure.Identity;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;

namespace DemoSampleTemplate.Logging
{
    public static class DefaultLogging
    {
        public static Microsoft.Extensions.Logging.ILogger BuildDefaultLogger(string loggerName = null)
        {
            if (string.IsNullOrEmpty(loggerName))
            {
                loggerName = "DefaultLogger";
            }

            var serilogLogger = BuildDefaultSerilogLogger();
            var logFactory = new LoggerFactory().AddSerilog(serilogLogger);
            var logger = logFactory.CreateLogger(loggerName);
            return logger;
        }

        public static void HandleMainException(Microsoft.Extensions.Logging.ILogger logger, LogLevel level, Exception ex, string message = null)
        {
            if (logger == null || ex == null) return;

            var type = ex.GetType();
            var msg = $"{message ?? string.Empty} - {ex.Message}";

            if (type == typeof(CredentialUnavailableException) ||
                type == typeof(AuthenticationFailedException) ||
                type == typeof(AuthenticationRequiredException) ||
                type == typeof(Azure.RequestFailedException))
            {
                logger.Log(level, ex, $"ERR090: Application failed to start due to {ex.Source}. {msg}");
            }
            else
            {
                logger.Log(level, ex, $"ERR099: Application failed to start. {msg}");
            }
        }

        private static Logger BuildDefaultSerilogLogger()
        {
            var configuration = BuildDefaultConfiguration();

            var serviceName = configuration?.GetValue<string>("ApplicationInfo:ServiceName") ?? string.Empty;
            var traceGuid = Guid.NewGuid();

            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("ServiceName", serviceName)
                .Enrich.WithProperty("TraceGuid", traceGuid)
                .WriteTo.Console()
                .CreateLogger();
            return logger;
        }

        private static IConfiguration BuildDefaultConfiguration()
        {
            var configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
                        .AddEnvironmentVariables()
                        .Build();

            return configuration;
        }
    }
}
