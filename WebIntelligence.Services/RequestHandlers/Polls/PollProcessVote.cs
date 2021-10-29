using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebIntelligence.Common.Helpers;
using WebIntelligence.Common.Requests;

namespace WebIntelligence.Services.RequestHandlers.Polls;

public class PollProcessVoteRequestHandler : WebIntelligenceRequestHandler, IRequestHandler<PollProcessVoteRequest, IResult>
{
    public PollProcessVoteRequestHandler(WebIntelligenceContext db, IMediator mediator, IAppCache appCache, IMapper mapper)
        : base(db, mediator, appCache, mapper)
    {
    }

    public async Task<IResult> Handle(PollProcessVoteRequest request, CancellationToken ctx)
    {
        var pollResult = await Mediator.Send(new GetPollRequest(Guid.Parse(request.CustomId)), ctx);

        if (!pollResult.IsSuccess)
        {
            return pollResult;
        }

        var poll = pollResult.Entity;


        // if (pollResult.Entity.UserVotes.Any(x => x.UserId == request.UserId && poll.ChannelId == request.ChannelId))
        // {
        //     return Results.Fail("Can't vote twice");
        // }

        if (pollResult.Entity.UserVotes.Any(x => x.UserId == request.UserId && x.PollId == poll.Id))
        {
            var existingUserVote = await Db.UserVotes
                .Where(x => x.PollId == poll.Id && x.UserId == request.UserId)
                .SingleAsync(ctx);

            Db.UserVotes.Remove(existingUserVote);
        }

        var userVote = new UserVote
        {
            UserId = request.UserId,
            PollId = poll.Id,
            PollOptionId = Guid.Parse(poll.Options
                .Where(x => x.Id == request.SelectedValue)
                .Select(x => x.Id)
                .Single())
        };

        Db.UserVotes.Add(userVote);

        await Db.SaveChangesAsync(ctx);

        await Mediator.Send(new InvalidatePollCacheRequest(Guid.Parse(request.CustomId)), ctx);

        return Results.Success();
    }
}