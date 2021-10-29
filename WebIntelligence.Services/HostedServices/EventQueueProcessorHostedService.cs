using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WebIntelligence.Services.HostedServices;

public class EventQueueProcessorHostedService : BackgroundService
{
    private readonly ILogger<EventQueueProcessorHostedService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IEventQueue _eventQueue;

    public EventQueueProcessorHostedService(ILogger<EventQueueProcessorHostedService> logger,
        IServiceScopeFactory serviceScopeFactory,
        IEventQueue eventQueue)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
        _eventQueue = eventQueue;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var queuedItem =
                await _eventQueue.Dequeue(stoppingToken);

            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var services = scope.ServiceProvider;

                var mediator = services.GetRequiredService<IMediator>();

                await mediator.Send(queuedItem, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error occurred executing {queuedItem}", nameof(queuedItem));
            }
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Event queue processor is stopping.");
        await base.StopAsync(stoppingToken);
        _logger.LogInformation("Event queue processor is stopped.");
    }
}