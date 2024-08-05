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
                providerBuilder.AddOtlpExporter(o =>
                {
                    o.Endpoint = new Uri("http://otel-collector.default.svc.cluster.local:4317");
                }).SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
                        .AddService(typeof(Program).Assembly.GetName().Name));
            });

            otelSetup.WithTracing(config =>
            {
                config.AddAspNetCoreInstrumentation();
                config.AddHttpClientInstrumentation();
                config.AddOtlpExporter(o =>
                {
                    o.Endpoint = new Uri("http://otel-collector.default.svc.cluster.local:4317");
                }).SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
                        .AddService(typeof(Program).Assembly.GetName().Name));
            });

            otelSetup.WithLogging(config =>
            {
                config.AddOtlpExporter(o =>
                {
                    o.Endpoint = new Uri("http://otel-collector.default.svc.cluster.local:4317");
                }).SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
                        .AddService(typeof(Program).Assembly.GetName().Name));
            });

            builder.Services.AddLogging(config =>
            {
                config.AddOpenTelemetry(options =>
                {
                    options.IncludeScopes = true;
                    options.ParseStateValues = true;
                    options.IncludeFormattedMessage = true;
                    options.AddOtlpExporter(o =>
                    {
                        o.Endpoint = new Uri("http://otel-collector.default.svc.cluster.local:4317");
                    }).SetResourceBuilder(
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

            //builder.Services.AddDaprClient();
            builder.Services.AddHttpClient("Heroes", httpClient =>
            {
                httpClient.BaseAddress = new Uri("http://api2-service.default.svc.cluster.local:8083/");
            });

            builder.Services.AddHttpClient("Villains", httpClient =>
            {
                httpClient.BaseAddress = new Uri("http://api1-service.default.svc.cluster.local:8082/");
            });

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
