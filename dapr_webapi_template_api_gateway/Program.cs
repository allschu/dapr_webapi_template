using FastEndpoints;
using FastEndpoints.Swagger;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Serilog;

namespace dapr_webapi_template_api_gateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();


            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost.UseUrls("http://0.0.0.0:8080");

            var otelSetup = builder.Services.AddOpenTelemetry();
            otelSetup.WithMetrics(providerBuilder =>
            {
                providerBuilder.AddMeter(typeof(Program).Assembly.GetName().Name);
                providerBuilder.AddMeter("Microsoft.AspNetCore.Hosting");
                providerBuilder.AddMeter("Microsoft.AspNetCore.Server.Kestrel");
                providerBuilder.AddPrometheusExporter();
            });

            otelSetup.WithTracing(config =>
            {
                config.AddAspNetCoreInstrumentation();
                config.AddHttpClientInstrumentation();
                config.AddOtlpExporter();
            });

            builder.Host.UseSerilog();
            
            // Add services to the container.
            builder.Services.AddAuthorization();
            
            builder.Services
                .AddFastEndpoints()
                .SwaggerDocument(options =>
                {
                    options.DocumentSettings = settings =>
                    {
                        settings.Title = "API Gateway";
                    };
                });

            builder.Services.AddDaprClient();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseFastEndpoints()
               .UseSwaggerGen();

            app.MapPrometheusScrapingEndpoint();

            app.Run();
        }
    }
}
