using Serilog;
namespace DemoSampleTemplate
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;
            builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));
            builder.Services.ConfigureServices(configuration);
            var app = builder.Build();
            app.Configure();

            //try
            //{
            //    Log.Information("Application Starting up!");
            //    CreateHostBuilder(args).Build().Run();
            //}
            //catch (Exception Ex)
            //{
            //    Log.Fatal(Ex, "Application failed to start properly");
            //}
            //finally
            //{
            //    Log.CloseAndFlush();
            //}
        }

        //public static IHostBuilder CreateHostBuilder(string[] args)
        //{
        //    return Host.CreateDefaultBuilder(args)
        //        .UseSerilog((context, services, configuration) => configuration
        //        .ReadFrom.Configuration(context.Configuration)
        //        .ReadFrom.Services(services)
        //        .Enrich.FromLogContext()
        //        .WriteTo.Console())
        //        .ConfigureWebHostDefaults(webBuilder =>
        //        {
        //            webBuilder.UseStartup<Startup>();
        //        });
        //}
    }
}
