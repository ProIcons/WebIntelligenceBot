using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebIntelligence.Common.Requests;

namespace WebIntelligence.Services.RequestHandlers.Users;

public class DoesUserExistHandler : WebIntelligenceRequestHandler<InvalidateUserExistsRequest>, IRequestHandler<UserExistsRequest, bool>
{
    public DoesUserExistHandler(WebIntelligenceContext db, IMediator mediator, IAppCache appCache, IMapper mapper) : base(db, mediator, appCache, mapper)
    {
    }

    public async Task<bool> Handle(UserExistsRequest request, CancellationToken cancellationToken)
    {
        return await AppCache
            .GetOrAddAsync(GetCacheKey(request.DiscordUserId),
                () => Db.Users.AnyAsync(x => x.Id == request.DiscordUserId, cancellationToken),
                DateTimeOffset.UtcNow.AddDays(30));
    }

    protected override void Handle(InvalidateUserExistsRequest request)
    {
        AppCache.Remove(GetCacheKey(request.DiscordUserId));
    }

    private static string GetCacheKey(ulong discordUserId)
    {
        return $"{nameof(EnsureUserExistsHandler)}/{nameof(UserExistsRequest)}/{discordUserId}";
    }
}