using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebIntelligence.Common.Requests;

namespace WebIntelligence.Services.HostedServices;

public class RemindersHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public RemindersHostedService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var services = scope.ServiceProvider;

        var mediator = services.GetRequiredService<IMediator>();

        await ProcessReminders(mediator, true, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessReminders(mediator, false, stoppingToken);
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
}