using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebIntelligence.Common.Requests;

namespace WebIntelligence.Services.HostedServices;

public class RemindersHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<RemindersHostedService> _logger;

    public RemindersHostedService(IServiceScopeFactory serviceScopeFactory, ILogger<RemindersHostedService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await ProcessReminders(true, stoppingToken);
        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessReminders(false, stoppingToken);
        }

        _logger.LogInformation($"{nameof(RemindersHostedService)} is terminating...");
    }

    private async Task ProcessReminders(bool discard, CancellationToken ctx)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var services = scope.ServiceProvider;

            var mediator = services.GetRequiredService<IMediator>();
            await ProcessReminders(mediator, discard, ctx);
        }
    }

    private async Task ProcessReminders(IMediator mediator, bool discard, CancellationToken stoppingToken)
    {
        var reminders = await mediator.Send(new GetAllRemindersRequest(), stoppingToken);

        var processableReminders = reminders.Entity!.Where(x => x.RemindAt <= DateTime.Now);

        foreach (var reminder in processableReminders)
        {
            if (discard && (DateTime.Now - reminder.RemindAt) < TimeSpan.FromMinutes(1) || !discard)
            {
                await mediator.Send(new SendReminderRequest(reminder), stoppingToken);
            }

            await mediator.Send(new DeleteReminderRequest(reminder.UserId, reminder.Id), stoppingToken);
        }

        await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"{nameof(RemindersHostedService)} is stopping.");
        await base.StopAsync(stoppingToken);
        _logger.LogInformation($"{nameof(RemindersHostedService)} is stopped.");
    }
}