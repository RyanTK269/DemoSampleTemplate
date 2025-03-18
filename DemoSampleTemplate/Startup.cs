using DemoSampleTemplate.Core.Exceptions;
using DemoSampleTemplate.Core.Middlewares;
using DemoSampleTemplate.Middlewares;
using DemoSampleTemplate.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.OpenApi.Models;
using System.IO.Compression;
using System.Reflection;

namespace DemoSampleTemplate
{
    public static class Startup
    {
        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
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

            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            //services.AddBllServices(configuration);
            services.AddMemoryCache();
            services.AddHealthChecks();

            services.AddResponseCaching();
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
            });

            //services.Configure<BrotliCompressionProviderOptions>(options =>
            //{
            //    options.Level = CompressionLevel.SmallestSize;
            //});
            //services.Configure<GzipCompressionProviderOptions>(options =>
            //{
            //    options.Level = CompressionLevel.SmallestSize;
            //});

            services.AddExceptionHandler<RequestMiddleware>();
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
            //app.UseExceptionHandler(opt => { });
            //app.UseMiddleware<ExceptionHandlerMiddleware>();

            app.UseHttpsRedirection();

            //app.UseSpaStaticFiles(new StaticFileOptions()
            //{
            //    RequestPath = "/dist"
            //});

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            //app.UseCors(ONLINE_EXAM_CORS_POLICY);
            app.MapControllers();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
            //app.UseSpa(spa => { });
            app.UseResponseCompression();

            //app.UseMiddleware<ExceptionMiddleware>();

            app.Run();
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
