using AutoMapper;
using WebIntelligence.Common.Requests;

namespace WebIntelligence.Services.RequestHandlers.Users;

public class AddUserHandler : WebIntelligenceAsyncRequestHandler<AddUserRequest>
{
    public AddUserHandler(WebIntelligenceContext db, IMediator mediator, IAppCache appCache, IMapper mapper) : base(db, mediator, appCache, mapper)
    {
    }

    protected override async Task Handle(AddUserRequest request, CancellationToken cancellationToken)
    {
        var userExists = await Mediator.Send(new UserExistsRequest(request.DiscordUserId), cancellationToken);

        if (userExists)
            return;

        var user = new User
        {
            Id = request.DiscordUserId,
            FirstSeenDateTime = request.JoinedDateTime,
            JoinedGuildDateTime = request.JoinedDateTime,
            UsernameWithDiscriminator = $"{request.DiscordUsername}#{request.DiscordDiscriminator}",
            Nickname = request.DiscordNickname,
        };

        Db.Add(user);

        await Db.SaveChangesAsync(cancellationToken);

        await Mediator.Send(new InvalidateUserExistsRequest(request.DiscordUserId), cancellationToken);
    }
}