using LazyCache;
using MediatR;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Core;
using WebIntelligence.Bot.Helpers;
using WebIntelligence.Common.Requests;

namespace WebIntelligence.Bot.RequestHandlers.Reminders;

public class SendReminderHandler : WebIntelligenceAsyncRequestHandler<SendReminderRequest>
{
    private readonly IDiscordRestChannelAPI _channelApi;

    public SendReminderHandler(IMediator mediator, IAppCache appCache, IDiscordRestChannelAPI channelApi) : base(mediator, appCache)
    {
        _channelApi = channelApi;
    }

    protected override async Task Handle(SendReminderRequest request, CancellationToken cancellationToken)
    {
        var reminder = request.Reminder;

        var embed = new Embed
        {
            Title = "Reminder",
            Description = reminder.Message
        };

        await _channelApi.CreateMessageAsync(
            new Snowflake(reminder.DiscordChannelId),
            DiscordFormatter.UserIdToMention(reminder.UserId),
            embeds: new[] { embed },
            ct: cancellationToken
        );
    }
}