using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebIntelligence.Common.Helpers;
using WebIntelligence.Common.Requests;
using WebIntelligence.Common.Responses;

namespace WebIntelligence.Services.RequestHandlers.Users;

public class GetDiffForUserHandler : WebIntelligenceRequestHandler, IRequestHandler<GetDiffForUserRequest, GetUserDiffResponse>
{
    public GetDiffForUserHandler(WebIntelligenceContext db, IMediator mediator, IAppCache appCache, IMapper mapper) : base(db, mediator, appCache, mapper)
    {
    }

    public async Task<GetUserDiffResponse> Handle(GetDiffForUserRequest request, CancellationToken cancellationToken)
    {
        var diffMessages = new List<string>();

        var user = await Db.Users
            .Where(x => x.Id == request.DiscordUserId)
            .Select(x => new
            {
                x.Id,
                x.UsernameWithDiscriminator,
                x.Nickname
            }).SingleOrDefaultAsync(cancellationToken: cancellationToken);

        if (user is null)
            return new GetUserDiffResponse(false, diffMessages);

        var handle = DiscordHandleHelper.BuildHandle(request.DiscordUsername, request.DiscordDiscriminator);

        if (!string.IsNullOrWhiteSpace(user.UsernameWithDiscriminator) && user.UsernameWithDiscriminator != handle)
        {
            diffMessages.Add($"Changed handle from {user.UsernameWithDiscriminator} to {handle}");
        }

        if (user.Nickname != request.DiscordNickname)
        {
            if (string.IsNullOrWhiteSpace(user.Nickname))
            {
                diffMessages.Add($"Set nickname to {request.DiscordNickname}");
            }
            else
            {
                diffMessages.Add($"Changed nickname from {user.Nickname} to {request.DiscordNickname}");
            }
        }

        return new GetUserDiffResponse(diffMessages.Any(), diffMessages);
    }
}