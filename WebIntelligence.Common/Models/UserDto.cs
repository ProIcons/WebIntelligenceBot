namespace WebIntelligence.Common.Models;

public sealed record UserDto(
    ulong Id,
    string? UsernameWithDiscriminator,
    string? Nickname,
    DateTimeOffset? JoinedGuildDateTime,
    DateTimeOffset FirstSeenDateTime
);