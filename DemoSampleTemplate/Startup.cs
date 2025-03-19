using DemoSampleTemplate.Core.Exceptions;
using DemoSampleTemplate.Core.Middlewares;
using DemoSampleTemplate.Middlewares;
using DemoSampleTemplate.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.OpenApi.Models;
using System.IO.Compression;
using System.Reflection;
using System.Threading.RateLimiting;

namespace DemoSampleTemplate
{
    public static class Startup
    {
        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMemoryCache();
            services.AddHealthChecks();
            services.AddResponseCaching();
            services.AddResponseCompression();
            services.AddExceptionHandler<RequestMiddleware>();
            services.AddRateLimiter();
        }

        public static void Configure(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();

                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("swagger/v1/swagger.json", "Sample template");
                    options.RoutePrefix = "";
                });
                app.MapHealthChecks("/health");
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseResponseCompression();
            app.UseResponseCaching();
            app.UseRateLimiter();
            app.MapControllers();

            app.Run();
        }

        public static void AddSwaggerGen(this IServiceCollection services)
        {
            services.AddSwaggerGen(setup =>
            {
                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    BearerFormat = "JWT",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    Description = "Put your JWT Bearer token on textbox below!",

                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme,
                    }
                };

                setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

                setup.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { jwtSecurityScheme, Array.Empty<string>() }
                });
            });
        }

        public static void AddResponseCompression(this IServiceCollection services)
        {
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.EnableForHttps = true;
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes;
            });

            services.Configure<BrotliCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.SmallestSize;
            });
            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.SmallestSize;
            });
        }

        public static void AddRateLimiter(this IServiceCollection services)
        {
            services.AddRateLimiter(rateLimiterOptions =>
            {
                rateLimiterOptions.AddFixedWindowLimiter("fixed", options =>
                {
                    options.PermitLimit = 10;
                    options.Window = TimeSpan.FromSeconds(10);
                    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    options.QueueLimit = 5;
                });
                rateLimiterOptions.AddSlidingWindowLimiter("sliding", options =>
                {
                    options.PermitLimit = 10;
                    options.Window = TimeSpan.FromSeconds(10);
                    options.SegmentsPerWindow = 2;
                    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    options.QueueLimit = 5;
                });
                rateLimiterOptions.AddTokenBucketLimiter("token", options =>
                {
                    options.TokenLimit = 100;
                    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    options.QueueLimit = 5;
                    options.ReplenishmentPeriod = TimeSpan.FromSeconds(10);
                    options.TokensPerPeriod = 20;
                    options.AutoReplenishment = true;
                });
                rateLimiterOptions.AddConcurrencyLimiter("concurrency", options =>
                {
                    options.PermitLimit = 10;
                    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    options.QueueLimit = 5;
                });
            });
        }
    }

    //public class Startup
    //{
    //    public Startup(IConfiguration configuration)
    //    {
    //        Configuration = configuration;
    //    }

    //    public IConfiguration Configuration { get; }

    //    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    //    {
    //        services.AddControllers();
    //        services.AddEndpointsApiExplorer();
    //        services.AddSwaggerGen(setup =>
    //        {
    //            var jwtSecurityScheme = new OpenApiSecurityScheme
    //            {
    //                BearerFormat = "JWT",
    //                Name = "Authorization",
    //                In = ParameterLocation.Header,
    //                Type = SecuritySchemeType.Http,
    //                Scheme = JwtBearerDefaults.AuthenticationScheme,
    //                Description = "Put your JWT Bearer token on textbox below!",

    //                Reference = new OpenApiReference
    //                {
    //                    Id = JwtBearerDefaults.AuthenticationScheme,
    //                    Type = ReferenceType.SecurityScheme,
    //                }
    //            };

    //            setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

    //            setup.AddSecurityRequirement(new OpenApiSecurityRequirement
    //        {
    //            { jwtSecurityScheme, Array.Empty<string>() }
    //        });
    //        });

    //        services.AddAutoMapper(Assembly.GetExecutingAssembly());
    //        //services.AddBllServices(configuration);
    //        services.AddMemoryCache();
    //        services.AddHealthChecks();
    //    }

    //    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
    //    {
    //        if (env.IsDevelopment())
    //        {
    //            app.UseSwagger();

    //            app.UseSwaggerUI(options =>
    //            {
    //                options.SwaggerEndpoint("swagger/v1/swagger.json", "Sample template");
    //                options.RoutePrefix = "";
    //            });
    //            //app.MapHealthChecks("/health");
    //        }
    //        app.UseMiddleware<ExceptionHandlerMiddleware>();

    //        app.UseHttpsRedirection();

    //        //app.UseSpaStaticFiles(new StaticFileOptions()
    //        //{
    //        //    RequestPath = "/dist"
    //        //});

    //        app.UseRouting();
    //        app.UseAuthentication();
    //        app.UseAuthorization();
    //        //app.UseCors(ONLINE_EXAM_CORS_POLICY);
    //        app.UseEndpoints(endpoints => endpoints.MapControllers());
    //        //app.UseSpa(spa => { });

    //        //app.UseMiddleware<ExceptionMiddleware>();
    //    }
    //}
}
