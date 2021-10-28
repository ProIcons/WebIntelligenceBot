using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebIntelligence.Common.Requests;

namespace WebIntelligence.Services.RequestHandlers.Users;

public class UpdateUserHandler : WebIntelligenceAsyncRequestHandler<UpdateUserRequest>
{
    public UpdateUserHandler(WebIntelligenceContext db, IMediator mediator, IAppCache appCache, IMapper mapper) : base(db, mediator, appCache, mapper)
    {
    }

    protected override async Task Handle(UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var user = await Db.Users
            .SingleOrDefaultAsync(x => x.Id == request.DiscordUserId, cancellationToken: cancellationToken);

        if (user is null)
            return;

        user.JoinedGuildDateTime = request.JoinedDateTime;
        user.UsernameWithDiscriminator = $"{request.DiscordUsername}#{request.DiscordDiscriminator}";
        user.Nickname = request.DiscordNickname;

        await Db.SaveChangesAsync(cancellationToken);
    }
}