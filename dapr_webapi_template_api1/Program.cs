using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace dapr_webapi_template_api1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost.UseUrls("http://0.0.0.0:8080");

            // Add services to the container.
            builder.Services.AddAuthorization();

            builder.Services.AddLogging(configure =>
            {
                configure.AddOpenTelemetry(options =>
                {
                    options.IncludeScopes = true;
                    options.ParseStateValues = true;
                    options.IncludeFormattedMessage = true;
                    options.AddOtlpExporter();
                });
            });

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
                "Bane", "Black Mask", "Clayface", "Deadshot", "Hush", "Joker", "Balmy", "Hot", "Sweltering", "Scorching"
            };

            app.MapGet("/villains", (HttpContext httpContext, ILogger<Program> logger) =>
            {
                logger.LogInformation("Getting villains");

                var forecast = Enumerable.Range(1, 5).Select(index =>
                    new Villain
                    {
                        Name = summaries[Random.Shared.Next(summaries.Length)]
                    })
                    .FirstOrDefault();
                return forecast;
            });

            app.Run();
        }
    }
}
