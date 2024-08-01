using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace dapr_webapi_template_client.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            

            await builder.Build().RunAsync();
        }
    }
}
