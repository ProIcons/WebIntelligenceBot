using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebIntelligence.Common.Helpers;
using WebIntelligence.Common.Models;
using WebIntelligence.Common.Requests;
using WebIntelligence.Services.Helpers;

namespace WebIntelligence.Services.RequestHandlers.Polls;

public class GetPollRequestHandler :
    WebIntelligenceRequestHandler,
    IRequestHandler<GetPollRequest, Result<PollDto>>,
    IRequestHandler<GetActivePollsRequest, List<PollDto>>,
    IRequestHandler<GetPollProjectionsRequest, List<PollDto>>,
    IRequestHandler<DoesPollExistRequest, bool>,
    IRequestHandler<InvalidatePollCacheRequest>
{
    public GetPollRequestHandler(WebIntelligenceContext db, IMediator mediator, IAppCache appCache, IMapper mapper) : base(db, mediator, appCache, mapper)
    {
    }

    public async Task<Result<PollDto>> Handle(GetPollRequest request, CancellationToken cancellationToken)
    {
        var pollExists = await Handle(new DoesPollExistRequest(request.Id), cancellationToken);

        if (!pollExists)
            return Results.Fail<PollDto>("Poll does not exist");

        var poll = await AppCache.GetOrAddAsync(CacheKey.GetPollCacheKey(request.Id),
            () => GetPoll(request.Id, cancellationToken),
            DateTimeOffset.UtcNow.AddMinutes(5));


        return Results.Success(Mapper.Map<Domain.Model.Poll, PollDto>(poll));
    }

    public async Task<bool> Handle(DoesPollExistRequest request, CancellationToken cancellationToken)
        => await AppCache.GetOrAddAsync(
            CacheKey.GetPollCacheKey(request.Id),
            () => GetPoll(request.Id, cancellationToken),
            DateTimeOffset.UtcNow.AddDays(30)
        ) != null;

    public Task<Unit> Handle(InvalidatePollCacheRequest request, CancellationToken cancellationToken)
    {
        AppCache.Remove(CacheKey.GetPollCacheKey(request.Id));
        return Unit.Task;
    }

    public async Task<List<PollDto>> Handle(GetActivePollsRequest request, CancellationToken cancellationToken)
    {
        var polls = await AppCache.GetOrAddAsync(
            CacheKey.GetActivePollsCacheKey(),
            () => GetActivePolls(cancellationToken),
            DateTimeOffset.UtcNow.AddDays(30)
        );

        return polls.Select(x => Mapper.Map<PollDto>(x)).ToList();
    }


    public Task<List<PollDto>> Handle(GetPollProjectionsRequest request, CancellationToken cancellationToken)
    {
        return AppCache.GetOrAddAsync(
            CacheKey.GetPollProjectionsCacheKey(),
            () => Handle(new GetActivePollsRequest(), cancellationToken));
    }


    private async Task<Poll> GetPoll(Guid pollId, CancellationToken ct)
        => await Db.Polls
            .Where(x => x.Id == pollId)
            .IgnoreAutoIncludes()
            .Include(x => x.UserVotes)
            .Include(x => x.Options)
            .SingleOrDefaultAsync(ct);

    private async Task<List<Poll>> GetActivePolls(CancellationToken ct)
        => await Db.Polls
            .Where(x => !x.Finalized)
            .IgnoreAutoIncludes()
            .Include(x => x.UserVotes)
            .Include(x => x.Options)
            .ToListAsync(ct);
}