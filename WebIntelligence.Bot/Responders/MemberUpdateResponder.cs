using MediatR;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.Core;
using Remora.Discord.Gateway.Responders;
using WebIntelligence.Bot.Extensions;
using WebIntelligence.Bot.Helpers;
using WebIntelligence.Common.Requests;

namespace WebIntelligence.Bot.Responders;

public class MemberUpdateResponder : IResponder<IGuildMemberUpdate>
{
    private readonly DiscordCache _discordCache;
    private readonly IMediator _mediator;

    public MemberUpdateResponder(IMediator mediator,
        DiscordCache discordCache)
    {
        _mediator = mediator;
        _discordCache = discordCache;
    }

    public async Task<Result> RespondAsync(IGuildMemberUpdate gatewayEvent, CancellationToken ct = new())
    {
        var user = gatewayEvent.User;

        var selfMember = _discordCache.GetGuildSelfMember(gatewayEvent.GuildID);
        var newMember = new GuildMember(
            new Optional<IUser>(gatewayEvent.User),
            gatewayEvent.Nickname,
            gatewayEvent.Avatar,
            gatewayEvent.Roles,
            gatewayEvent.JoinedAt!.Value,
            gatewayEvent.PremiumSince,
            gatewayEvent.IsDeafened.HasValue && gatewayEvent.IsDeafened.Value,
            gatewayEvent.IsMuted.HasValue && gatewayEvent.IsMuted.Value,
            gatewayEvent.IsPending.HasValue && gatewayEvent.IsPending.Value
        );

        if (selfMember.User.HasValue && user.ID == selfMember.User.Value!.ID)
            _discordCache.SetGuildSelfMember(gatewayEvent.GuildID, newMember);
        else
            _discordCache.SetGuildMember(gatewayEvent.GuildID.Value, user.ID.Value, newMember);

        await _mediator.Send(
            new UpdateUserRequest(
                gatewayEvent.GuildID.Value,
                user.ID.Value,
                user.Username,
                user.Discriminator.ToPaddedDiscriminator(),
                gatewayEvent.Nickname.HasValue ? gatewayEvent.Nickname.Value : null,
                gatewayEvent.JoinedAt),
            ct);

        return Result.FromSuccess();
    }
}