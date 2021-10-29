using AutoMapper;
using WebIntelligence.Common.Helpers;
using WebIntelligence.Common.Models;
using WebIntelligence.Common.Requests;
using WebIntelligence.Services.Helpers;

namespace WebIntelligence.Services.RequestHandlers.Polls;

public class CreatePollRequestHandler : WebIntelligenceRequestHandler<CachePollProjectionRequest>, IRequestHandler<CreatePollRequest, Result<PollDto>>
{
    public CreatePollRequestHandler(WebIntelligenceContext db, IMediator mediator, IAppCache appCache, IMapper mapper) : base(db, mediator, appCache, mapper)
    {
    }

    public async Task<Result<PollDto>> Handle(CreatePollRequest request, CancellationToken cancellationToken)
    {
        var startedTime = DateTimeOffset.UtcNow;
        var pollOptions = request.Options.Select(x => new PollOption()
        {
            Value = x,
        }).ToList();
        var poll = new Poll
        {
            ChannelId = request.ChannelId,
            StartedTime = startedTime,
            EndingTime = startedTime.Add(request.Duration ?? TimeSpan.FromMinutes(5)),
            Question = request.Question,
            Options = pollOptions
        };

        var entity = Db.Polls.Add(poll);

        await Db.SaveChangesAsync(cancellationToken);

        var pollDto = Mapper.Map<PollDto>(entity.Entity);

        var idResult = await Mediator.Send(new SendPollRequest(pollDto), cancellationToken);
        if (idResult.IsSuccess)
        {
            entity.Entity.MessageHandle = idResult.Entity;

            await Db.SaveChangesAsync(cancellationToken);
        }

        pollDto = pollDto with { MessageHandle = entity.Entity.MessageHandle };

        await Mediator.Send(new CachePollProjectionRequest(pollDto), cancellationToken);

        return Results.Success(pollDto);
    }


    protected override void Handle(CachePollProjectionRequest request)
    {
        var projections = AppCache
            .GetOrAdd(
                CacheKey.GetPollProjectionsCacheKey(),
                () => new List<PollDto>())
            .Where(x => x.Id != request.Poll.Id)
            .ToList();

        projections.Add(request.Poll);
        AppCache.Add(CacheKey.GetPollProjectionsCacheKey(), projections);
    }
}