namespace WebIntelligence.Common.Requests;

public sealed record GetUserRequest(
    ulong DiscordUserId
) : IRequest<Result<GetUserResponse>>;

public sealed record UserExistsRequest(
    ulong DiscordUserId
) : IRequest<bool>;

public sealed record InvalidateUserExistsRequest(
    ulong DiscordUserId
) : IRequest;

public sealed record EnsureUserExistsRequest(
    ulong DiscordUserId
) : IRequest;

public sealed record GetDiffForUserRequest(
    ulong DiscordUserId,
    string DiscordUsername,
    string DiscordDiscriminator,
    string? DiscordNickname
) : IRequest<GetUserDiffResponse>;

public sealed record UpdateUserRequest(
    ulong DiscordGuildId,
    ulong DiscordUserId,
    string DiscordUsername,
    string DiscordDiscriminator,
    string? DiscordNickname,
    DateTimeOffset? JoinedDateTime
) : IRequest;

public sealed record AddUserRequest(
    ulong DiscordGuildId,
    ulong DiscordUserId,
    string DiscordUsername,
    string DiscordDiscriminator,
    string? DiscordNickname,
    DateTimeOffset JoinedDateTime
) : IRequest;