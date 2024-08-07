using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace dapr_webapi_template_api2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.WebHost.UseUrls("http://0.0.0.0:8080");

            // Add services to the container.
            builder.Services.AddAuthorization();

            var otelSetup = builder.Services.AddOpenTelemetry();

            otelSetup.WithMetrics(providerBuilder =>
            {
                providerBuilder.AddMeter(typeof(Program).Assembly.GetName().Name);
                providerBuilder.AddMeter("Microsoft.AspNetCore.Hosting");
                providerBuilder.AddMeter("Microsoft.AspNetCore.Server.Kestrel");
                providerBuilder.AddOtlpExporter(o =>
                {
                    o.Endpoint = new Uri("http://otel-collector.default.svc.cluster.local:4317");
                }).SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
                        .AddService(typeof(Program).Assembly.GetName().Name));
            });
            otelSetup.WithLogging(logConfig =>
            {

                logConfig.AddOtlpExporter(o =>
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


            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();

            var summaries = new[]
            {
                "Batman", "Superman", "Robin", "Alfred", "Nightwing", "Batgirl", "Talia al Ghul", "Jason Todd", "Stephanie Brown", "Bluebird"
            };

            app.MapGet("/heroes", (HttpContext httpContext, ILogger<Program> logger) =>
            {
                logger.LogInformation("Getting heroes");
                var heroes = Enumerable.Range(1, 5).Select(index =>
                    new Hero
                    {
                        Name = summaries[Random.Shared.Next(summaries.Length)]
                    })
                    .FirstOrDefault();
                return heroes;
            });

            app.Run();
        }
    }
}
