using DemoSampleTemplate.Logging;
using Serilog;
namespace DemoSampleTemplate
{
    public class Program
    {
        private static Microsoft.Extensions.Logging.ILogger? _logger;

        public static void Main(string[] args)
        {
            _logger = DefaultLogging.BuildDefaultLogger();
            try
            {
                _logger?.LogInformation("Application Starting up!");
                var builder = WebApplication.CreateBuilder(args);
                var configuration = builder.Configuration;
                builder.Host.UseSerilog((context, services, configuration) =>
                    configuration.ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                    .WriteTo.Console());
                builder.Host.ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseIISIntegration();
                    webBuilder.UseIIS();
                });
                builder.Services.ConfigureServices(configuration);
                var app = builder.Build();
                app.Configure();
            }
            catch (Exception ex)
            {
                DefaultLogging.HandleMainException(_logger, LogLevel.Critical, ex);
            }
            finally
            {
                _logger?.LogInformation("Application Ended!");
            }
        }
    }
}
