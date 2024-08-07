using Dapr.Client;
using FastEndpoints;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace dapr_webapi_template_api_gateway.Endpoints
{
    public class Fight(IHttpClientFactory factory, ILogger<Fight> logger, ActivitySource activitySource) : EndpointWithoutRequest<FightResponse>
    {

        public override void Configure()
        {
            Get("fight");
            AllowAnonymous();
        }

        public override async Task<FightResponse> HandleAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Fight is called");

            using (var activity = activitySource.StartActivity("FightOperation"))
            {

                var httpHeroesClient = factory.CreateClient("Heroes");
                var httpVillainsClient = factory.CreateClient("Villains");

                var heroTask = await httpHeroesClient.GetFromJsonAsync<Character>("heroes", cancellationToken);
                activity?.SetTag("Hero response", heroTask.Name);
                
                var villainTask = await httpVillainsClient.GetFromJsonAsync<Character>("villains", cancellationToken);
                activity?.SetTag("Villain response", villainTask.Name);


                //var heroTask = daprClient.InvokeMethodAsync<Character>(System.Net.Http.HttpMethod.Get, Constants.HeroApi, "heroes", cancellationToken);
                //var villainTask = daprClient.InvokeMethodAsync<Character>(System.Net.Http.HttpMethod.Get, Constants.VillainApi, "villains", cancellationToken);

                activity?.AddEvent(new ActivityEvent("Send response back"));

                Response = new FightResponse
                {
                    HeroName = heroTask.Name,
                    VillainName = villainTask.Name
                };

                return Response;
            }
        }
    }

    public class Character
    {
        public string? Name { get; set; }
    }
}
