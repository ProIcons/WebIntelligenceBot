using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebIntelligence.Common.Models;
using WebIntelligence.Common.Requests;
using WebIntelligence.Services.Helpers;

namespace WebIntelligence.Services.RequestHandlers.Polls;

public class FinalizePollRequestHandler : WebIntelligenceAsyncRequestHandler<FinalizePollRequest>
{
    public FinalizePollRequestHandler(WebIntelligenceContext db, IMediator mediator, IAppCache appCache, IMapper mapper) : base(db, mediator, appCache, mapper)
    {
    }

    protected override async Task Handle(FinalizePollRequest request, CancellationToken cancellationToken)
    {
        var poll = await Db.Polls
            .Include(x => x.Options)
            .Include(x => x.UserVotes)
            .SingleOrDefaultAsync(x => x.Id == request.Poll.Id, cancellationToken);
        if (poll == null)
        {
            return;
        }

        poll.Finalized = true;

        await Db.SaveChangesAsync(cancellationToken);

        await Mediator.Send(new UpdatePollRequest(Mapper.Map<PollDto>(poll)), cancellationToken);

        AppCache.Remove(CacheKey.GetPollCacheKey(poll.Id));

        var projections = await Mediator.Send(new GetPollProjectionsRequest(), cancellationToken);

        AppCache.Add(CacheKey.GetPollProjectionsCacheKey(), projections.Where(x => x.Id != poll.Id).ToList());
    }
}