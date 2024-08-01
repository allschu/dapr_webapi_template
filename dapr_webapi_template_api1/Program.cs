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


            var app = builder.Build();
            
            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();

            var summaries = new[]
            {
                "Bane", "Black Mask", "Clayface", "Deadshot", "Hush", "Joker", "Balmy", "Hot", "Sweltering", "Scorching"
            };

            app.MapGet("/villains", (HttpContext httpContext) =>
            {
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
