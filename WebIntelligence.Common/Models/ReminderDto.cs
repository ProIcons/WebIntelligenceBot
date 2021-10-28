namespace WebIntelligence.Common.Models;

public sealed record ReminderDto
(
    int Id,
    ulong UserId,
    ulong DiscordChannelId,
    DateTimeOffset CreatedAt,
    DateTimeOffset RemindAt,
    string Message
);