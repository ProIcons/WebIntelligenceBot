namespace WebIntelligence.Common.Requests;

public sealed record AddReminderRequest(
    ulong DiscordUserId,
    ulong DiscordChannelId,
    TimeSpan TimeSpan,
    string Message
) : IRequest<IResult>;

public sealed record DeleteReminderRequest(
    ulong DiscordUserId,
    int ReminderId
) : IRequest<IResult>;

public sealed record DeleteAllRemindersRequest(
    ulong DiscordUserId
) : IRequest<IResult>;

public sealed record GetRemindersRequest(
    ulong DiscordUserId
) : IRequest<Result<List<ReminderDto>>>;

public sealed record GetAllRemindersRequest(
) : IRequest<Result<List<ReminderDto>>>;

public sealed record GetReminderRequest(
    ulong DiscordUserId,
    int ReminderId
) : IRequest<Result<ReminderDto>>;

public sealed record UserHasReminderRequest(
    ulong DiscordUserId,
    int ReminderId
) : IRequest<Result<bool>>;

public sealed record InvalidateGetRemindersRequest(
    ulong DiscordUserId
) : IRequest;

public sealed record SendReminderRequest(
    ReminderDto Reminder
) : IRequest;