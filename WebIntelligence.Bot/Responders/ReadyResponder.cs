using MediatR;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Gateway.Commands;
using Remora.Discord.API.Objects;
using Remora.Discord.Gateway;
using Remora.Discord.Gateway.Responders;
using WebIntelligence.Bot.Helpers;
using WebIntelligence.Common.Requests;

namespace WebIntelligence.Bot.Responders;

public class ReadyResponder : IResponder<IReady>
{
    private readonly BotState _botState;
    private readonly DiscordCache _discordCache;
    private readonly DiscordGatewayClient _discordGatewayClient;
    private readonly IDiscordRestGuildAPI _guildApi;
    private readonly IDiscordRestUserAPI _userApi;
    private readonly IMediator _mediator;

    public ReadyResponder(DiscordGatewayClient discordGatewayClient, DiscordCache discordCache,
        IDiscordRestGuildAPI guildApi, BotState botState, IMediator mediator)
    {
        _discordGatewayClient = discordGatewayClient;
        _discordCache = discordCache;
        _guildApi = guildApi;
        _botState = botState;
        _mediator = mediator;
    }

    public async Task<Result> RespondAsync(IReady gatewayEvent, CancellationToken ct = new())
    {
        _discordCache.SetSelfSnowflake(gatewayEvent.User.ID);

        var tasks = new List<Task>();
        foreach (var unavailableGuild in gatewayEvent.Guilds)
            tasks.Add(CacheGuild(unavailableGuild, gatewayEvent.User, ct));

        await Task.WhenAll(tasks);

        _botState.IsCacheReady = true;

        var updateCommand = new UpdatePresence(ClientStatus.Online, false, null, new IActivity[]
        {
            new Activity("for everything", ActivityType.Watching)
        });

        _discordGatewayClient.SubmitCommand(updateCommand);

        return Result.FromSuccess();
    }

    private async Task CacheGuild(IUnavailableGuild unavailableGuild, IUser user, CancellationToken ct = default)
    {
        var guildMember = await _guildApi.GetGuildMemberAsync(unavailableGuild.GuildID, user.ID, ct);
        if (guildMember.IsSuccess)
            _discordCache.SetGuildSelfMember(unavailableGuild.GuildID, guildMember.Entity);

        var guild = await _guildApi.GetGuildAsync(unavailableGuild.GuildID, true, ct);
        if (!guild.IsSuccess)
            return;

        _discordCache.SetGuildRoles(guild.Entity.ID, guild.Entity.Roles);
        var users = await _guildApi.ListGuildMembersAsync(unavailableGuild.GuildID, 1000, ct: ct);
        foreach (IGuildMember member in users.Entity)
        {
            await _mediator.Send(new AddUserRequest(
                unavailableGuild.GuildID.Value,
                member.User.Value.ID.Value,
                member.User.Value.Username,
                member.User.Value.Discriminator.ToString(),
                member.Nickname.Value,
                DateTimeOffset.UtcNow
            ), ct);
        }

        var channels = guild.Entity.Channels.HasValue
            ? guild.Entity.Channels.Value
            : (await _guildApi.GetGuildChannelsAsync(guild.Entity.ID, ct)).Entity!;

        _discordCache.SetGuildChannels(guild.Entity.ID, channels);

        var everyoneRole = guild.Entity.Roles.Single(x => x.Name == "@everyone");
        _discordCache.SetEveryoneRole(guild.Entity.ID, everyoneRole);
    }
}