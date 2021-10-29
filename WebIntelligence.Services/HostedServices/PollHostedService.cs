using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebIntelligence.Common.Requests;

namespace WebIntelligence.Services.HostedServices;

public class PollHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public PollHostedService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            await ProcessPolls(mediator, stoppingToken);
        }
    }

    private async Task ProcessPolls(IMediator mediator,
        CancellationToken lgtmCtx = default)
    {
        var projectedPolls = await mediator.Send(new GetPollProjectionsRequest(), lgtmCtx);
        if (projectedPolls == null || projectedPolls.Count == 0)
        {
            await Task.Delay(TimeSpan.FromSeconds(2), lgtmCtx);
            return;
        }

        foreach (var projectedPoll in projectedPolls)
        {
            var remainderTimeSpan = (projectedPoll.EndingTime - DateTimeOffset.UtcNow).Duration();

            if (projectedPoll.EndingTime > DateTimeOffset.UtcNow)
            {
                var actualPoll = await mediator.Send(new GetPollRequest(projectedPoll.Id), lgtmCtx);
                if (actualPoll.Entity == projectedPoll && remainderTimeSpan > TimeSpan.FromMinutes(1))
                    continue;

                await mediator.Send(new UpdatePollRequest(actualPoll.Entity), lgtmCtx);
            }
            else
            {
                await mediator.Send(new FinalizePollRequest(projectedPoll), lgtmCtx);
            }
        }

        await Task.Delay(TimeSpan.FromSeconds(2), lgtmCtx);
    }
}