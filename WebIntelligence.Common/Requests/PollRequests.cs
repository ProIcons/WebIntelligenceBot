namespace WebIntelligence.Common.Requests;

public record GetPollRequest(
    Guid Id
) : IRequest<Result<PollDto>>;

public record GetActivePollsRequest(
) : IRequest<List<PollDto>>;

public record InvalidatePollCacheRequest(
    Guid Id
) : IRequest;

public record PollProcessVoteRequest(
    ulong ChannelId,
    ulong UserId,
    string CustomId,
    string SelectedValue
) : IRequest<IResult>;

public record DoesPollExistRequest(
    Guid Id
) : IRequest<bool>;

public record CreatePollRequest(
    string Question,
    List<string> Options,
    ulong ChannelId,
    ulong StartedBy,
    TimeSpan? Duration
) : IRequest<Result<PollDto>>;

public record CachePollProjectionRequest(
    PollDto Poll
) : IRequest;

public record GetPollProjectionsRequest(
) : IRequest<List<PollDto>>;

public record SendPollRequest(
    PollDto Poll
) : IRequest<Result<ulong>>;

public record UpdatePollRequest(
    PollDto Poll
) : IRequest;

public record FinalizePollRequest(
    PollDto Poll
) : IRequest;