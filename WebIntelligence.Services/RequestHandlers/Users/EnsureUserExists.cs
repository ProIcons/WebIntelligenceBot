using AutoMapper;
using WebIntelligence.Common.Requests;

namespace WebIntelligence.Services.RequestHandlers.Users;

public class EnsureUserExistsHandler : WebIntelligenceAsyncRequestHandler<EnsureUserExistsRequest>
{
    public EnsureUserExistsHandler(WebIntelligenceContext db, IMediator mediator, IAppCache appCache, IMapper mapper) : base(db, mediator, appCache, mapper)
    {
    }

    protected override async Task Handle(EnsureUserExistsRequest request, CancellationToken cancellationToken)
    {
        var userExists = await Mediator.Send(new UserExistsRequest(request.DiscordUserId), cancellationToken);

        if (userExists)
            return;

        var user = new User
        {
            Id = request.DiscordUserId,
            FirstSeenDateTime = DateTimeOffset.UtcNow,
        };

        Db.Add(user);

        await Db.SaveChangesAsync(cancellationToken);

        await Mediator.Send(new InvalidateUserExistsRequest(request.DiscordUserId), cancellationToken);
    }
}