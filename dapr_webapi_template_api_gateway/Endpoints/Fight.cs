using Dapr.Client;
using FastEndpoints;

namespace dapr_webapi_template_api_gateway.Endpoints
{
    public class Fight(IHttpClientFactory factory, ILogger<Fight> logger) : EndpointWithoutRequest<FightResponse>
    {
        public override void Configure()
        {
            Get("fight");
            AllowAnonymous();
        }

        public override async Task<FightResponse> HandleAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Fight is called");

            var httpHeroesClient = factory.CreateClient("Heroes");
            var httpVillainsClient = factory.CreateClient("Villains");

            var heroTask = httpHeroesClient.GetFromJsonAsync<Character>("heroes", cancellationToken);
            var villainTask = httpVillainsClient.GetFromJsonAsync<Character>("villains", cancellationToken);

            //var heroTask = daprClient.InvokeMethodAsync<Character>(System.Net.Http.HttpMethod.Get, Constants.HeroApi, "heroes", cancellationToken);
            //var villainTask = daprClient.InvokeMethodAsync<Character>(System.Net.Http.HttpMethod.Get, Constants.VillainApi, "villains", cancellationToken);

            await Task.WhenAll(heroTask, villainTask);

            var heroResponse = await heroTask;
            var villainResponse = await villainTask;

            
            Response = new FightResponse
            {
                HeroName = heroResponse.Name,
                VillainName = villainResponse.Name
            };
            
            return Response;
        }
    }

    public class Character
    {
        public string? Name { get; set; }
    }
}
