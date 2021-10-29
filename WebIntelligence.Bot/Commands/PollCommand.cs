using MediatR;
using Remora.Commands.Attributes;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Commands.Conditions;
using Remora.Discord.Commands.Contexts;
using WebIntelligence.Bot.CommandGroups;
using WebIntelligence.Bot.Helpers;
using WebIntelligence.Bot.Models;
using WebIntelligence.Bot.Services;
using WebIntelligence.Common.Requests;

namespace WebIntelligence.Bot.Commands;

[Group("poll")]
public class PollCommand : WebIntelligenceCommandGroup
{
    public PollCommand(ICommandContext commandContext, IMediator mediator, IDiscordRestGuildAPI guildApi, DiscordAvatarHelper discordAvatarHelper,
        UserFeedbackService userFeedbackService) : base(commandContext, mediator, guildApi, discordAvatarHelper, userFeedbackService)
    {
    }

    [Command("create"), RequireContext(ChannelContext.Guild),
     RequireDiscordPermission(DiscordPermission.Administrator)]
    public async Task<Result<IUserMessage>> Create(
        string question,
        string option1,
        string option2,
        string? option3 = default,
        string? option4 = default,
        TimeSpan? duration = default
    )
    {
        var options = new List<string>() { option1, option2 };
        if (option3 != default)
            options.Add(option3);
        if (option4 != default)
            options.Add(option4);

        await Mediator.Send(new CreatePollRequest(
            question,
            options,
            CommandContext.ChannelID.Value,
            CommandContext.User.ID.Value,
            duration
        ));

        return Result<IUserMessage>.FromSuccess(new TextMessage("Poll has started!"));
    }
}