using MediatR;
using Remora.Commands.Groups;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Commands.Conditions;
using Remora.Discord.Commands.Contexts;
using WebIntelligence.Bot.Helpers;
using WebIntelligence.Bot.Services;

namespace WebIntelligence.Bot.CommandGroups;

[RequireContext(ChannelContext.Guild)]
public class WebIntelligenceCommandGroup : CommandGroup
{
    protected readonly ICommandContext CommandContext;
    protected readonly IMediator Mediator;
    protected readonly IDiscordRestGuildAPI GuildApi;
    protected readonly DiscordAvatarHelper DiscordAvatarHelper;
    protected readonly UserFeedbackService UserFeedbackService;

    public WebIntelligenceCommandGroup(ICommandContext commandContext, IMediator mediator, IDiscordRestGuildAPI guildApi,
        DiscordAvatarHelper discordAvatarHelper, UserFeedbackService userFeedbackService)
    {
        CommandContext = commandContext;
        Mediator = mediator;
        GuildApi = guildApi;
        DiscordAvatarHelper = discordAvatarHelper;
        UserFeedbackService = userFeedbackService;
    }
}