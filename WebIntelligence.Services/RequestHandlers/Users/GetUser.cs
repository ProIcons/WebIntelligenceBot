using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebIntelligence.Common.Helpers;
using WebIntelligence.Common.Models;
using WebIntelligence.Common.Requests;
using WebIntelligence.Common.Responses;

namespace WebIntelligence.Services.RequestHandlers.Users;

public class GetUserHandler : WebIntelligenceRequestHandler, IRequestHandler<GetUserRequest, Result<GetUserResponse>>
{
    private const int NUMBER_OF_DAYS_TO_LOOK_BACK = 30;

    public GetUserHandler(WebIntelligenceContext db, IMediator mediator, IAppCache appCache, IMapper mapper) : base(db, mediator, appCache, mapper)
    {
    }

    public async Task<Result<GetUserResponse>> Handle(GetUserRequest request, CancellationToken cancellationToken)
    {
        var userExists = await Mediator.Send(new UserExistsRequest(request.DiscordUserId), cancellationToken);

        if (!userExists)
            return Results.Fail<GetUserResponse>("User does not exist");

        var user = await AppCache.GetOrAddAsync(BuildGetUserCacheKey(request.DiscordUserId),
            () => GetUser(request.DiscordUserId),
            DateTimeOffset.UtcNow.AddMinutes(5));


        return Results.Success(new GetUserResponse(user));
    }

    private static string BuildGetUserCacheKey(ulong discordUserId)
    {
        return $"{nameof(GetUserHandler)}/{nameof(GetUser)}/{discordUserId}";
    }

    private async Task<UserDto> GetUser(ulong discordUserId)
    {
        return await Db.Users
            .Where(x => x.Id == discordUserId)
            .Select(x => Mapper.Map<UserDto>(x))
            .SingleAsync();
    }
}