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


            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();

            var summaries = new[]
            {
                "Batman", "Superman", "Robin", "Alfred", "Nightwing", "Batgirl", "Talia al Ghul", "Jason Todd", "Stephanie Brown", "Bluebird"
            };

            app.MapGet("/heroes", (HttpContext httpContext) =>
            {
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
