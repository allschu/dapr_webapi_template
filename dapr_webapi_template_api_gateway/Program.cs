using FastEndpoints;
using FastEndpoints.Swagger;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

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

            builder.Services.AddLogging(configure =>
            {
                configure.AddOpenTelemetry(options =>
                {
                    options.IncludeScopes = true;
                    options.ParseStateValues = true;
                    options.IncludeFormattedMessage = true;
                    options.AddOtlpExporter();
                    options.AddConsoleExporter()
                        .SetResourceBuilder(
                            ResourceBuilder.CreateDefault()
                                .AddService(typeof(Program).Assembly.GetName().Name));
                });
            });


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
