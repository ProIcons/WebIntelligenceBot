using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.Gateway.Responders;
using WebIntelligence.Bot.Helpers;

namespace WebIntelligence.Bot.Responders;

public class GuildUpdateResponder : IResponder<IGuildUpdate>
{
    private readonly DiscordCache _discordCache;

    public GuildUpdateResponder(DiscordCache discordCache)
    {
        _discordCache = discordCache;
    }

    public Task<Result> RespondAsync(IGuildUpdate gatewayEvent, CancellationToken ct = new())
    {
        _discordCache.SetGuildRoles(gatewayEvent.ID, gatewayEvent.Roles);
        return Task.FromResult(Result.FromSuccess());
    }
}