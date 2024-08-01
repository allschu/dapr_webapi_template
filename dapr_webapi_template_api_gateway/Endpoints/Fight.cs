using Dapr.Client;
using FastEndpoints;

namespace dapr_webapi_template_api_gateway.Endpoints
{
    public class Fight(DaprClient daprClient) : EndpointWithoutRequest<FightResponse>
    {
        public override void Configure()
        {
            Get("fight");
            AllowAnonymous();
        }

        public override async Task<FightResponse> HandleAsync(CancellationToken cancellationToken)
        {

            var heroTask = daprClient.InvokeMethodAsync<Character>(System.Net.Http.HttpMethod.Get, Constants.HeroApi, "heroes", cancellationToken);
            var villainTask = daprClient.InvokeMethodAsync<Character>(System.Net.Http.HttpMethod.Get, Constants.VillainApi, "villains", cancellationToken);

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
