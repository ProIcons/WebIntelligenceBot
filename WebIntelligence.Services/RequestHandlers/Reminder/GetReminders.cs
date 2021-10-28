using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebIntelligence.Common.Helpers;
using WebIntelligence.Common.Models;
using WebIntelligence.Common.Requests;

namespace WebIntelligence.Services.RequestHandlers.Reminder;

public class GetRemindersHandler :
    WebIntelligenceRequestHandler<InvalidateGetRemindersRequest>,
    IRequestHandler<GetReminderRequest, Result<ReminderDto>>,
    IRequestHandler<UserHasReminderRequest, Result<bool>>,
    IRequestHandler<GetRemindersRequest, Result<List<ReminderDto>>>,
    IRequestHandler<GetAllRemindersRequest, Result<List<ReminderDto>>>
{
    public GetRemindersHandler(WebIntelligenceContext db, IMediator mediator, IAppCache appCache, IMapper mapper) : base(db, mediator, appCache, mapper)
    {
    }


    public async Task<Result<List<ReminderDto>>> Handle(GetRemindersRequest request, CancellationToken cancellationToken)
    {
        var result = await AppCache.GetOrAddAsync(
            BuildGetRemindersWithId(request.DiscordUserId),
            () => GetRemindersByUserId(request.DiscordUserId),
            DateTimeOffset.UtcNow.AddDays(30)
        );

        return Results.Success(result
            .Select(entity => Mapper.Map<UserReminder, ReminderDto>(entity))
            .ToList()
        );
    }

    public async Task<Result<List<ReminderDto>>> Handle(GetAllRemindersRequest request, CancellationToken cancellationToken)
    {
        var result = await AppCache.GetOrAddAsync(
            BuildGetReminders(),
            () => GetReminders(),
            DateTimeOffset.UtcNow.AddDays(30)
        );

        return Results.Success(result.Select(reminder => Mapper.Map<UserReminder, ReminderDto>(reminder)).ToList());
    }

    public async Task<Result<ReminderDto>> Handle(GetReminderRequest request, CancellationToken cancellationToken)
    {
        var result = await AppCache.GetOrAddAsync(
            BuildGetRemindersWithId(request.DiscordUserId),
            () => GetRemindersByUserId(request.DiscordUserId),
            DateTimeOffset.UtcNow.AddDays(30)
        );
        try
        {
            return Results.Success(Mapper.Map<UserReminder, ReminderDto>(result.Single(x => x.Id == request.ReminderId)));
        }
        catch (Exception)
        {
            return Results.Fail<ReminderDto>($"User doesn't have a reminder with id: {request.ReminderId}");
        }
    }

    public async Task<Result<bool>> Handle(UserHasReminderRequest request, CancellationToken cancellationToken)
    {
        var result = await AppCache.GetOrAddAsync(
            BuildGetRemindersWithId(request.DiscordUserId),
            () => GetRemindersByUserId(request.DiscordUserId),
            DateTimeOffset.UtcNow.AddDays(30)
        );

        return Results.Success(result.Any(x => x.Id == request.ReminderId));
    }

    protected override void Handle(InvalidateGetRemindersRequest request)
    {
        AppCache.Remove(BuildGetRemindersWithId(request.DiscordUserId));
        AppCache.Remove(BuildGetReminders());
    }

    private async Task<List<UserReminder>> GetReminders() =>
        await Db.UserReminders.ToListAsync();

    private async Task<List<UserReminder>> GetRemindersByUserId(ulong userId) =>
        await Db.UserReminders.Where(x => x.UserId == userId).ToListAsync();

    private static string BuildGetReminders() =>
        $"{nameof(GetRemindersHandler)}/{nameof(GetReminders)}";

    private static string BuildGetRemindersWithId(ulong discordUserId) =>
        $"{nameof(GetRemindersHandler)}/{nameof(GetRemindersByUserId)}/{discordUserId}";
}