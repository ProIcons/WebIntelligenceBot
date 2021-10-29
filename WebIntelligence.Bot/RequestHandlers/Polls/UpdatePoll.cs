using LazyCache;
using MediatR;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Core;
using WebIntelligence.Bot.Helpers;
using WebIntelligence.Common.Requests;

namespace WebIntelligence.Bot.RequestHandlers.Polls;

public class UpdatePollRequestHandler : WebIntelligenceAsyncRequestHandler<UpdatePollRequest>
{
    private readonly IDiscordRestChannelAPI _channelApi;

    public UpdatePollRequestHandler(IMediator mediator, IAppCache appCache, IDiscordRestChannelAPI channelApi) : base(mediator, appCache)
    {
        _channelApi = channelApi;
    }

    protected override async Task Handle(UpdatePollRequest request, CancellationToken cancellationToken)
    {
        var components = request.Poll.Finalized
            ? new List<IMessageComponent>()
            : PollHelper.GetPollSelectMenu(request.Poll);

        await _channelApi.EditMessageAsync(
            new Snowflake(request.Poll.ChannelId),
            new Snowflake(request.Poll.MessageHandle),
            embeds: new[] { PollHelper.GetPollEmbed(request.Poll, request.Poll.Finalized) },
            components: components,
            ct: cancellationToken
        );

        await Mediator.Send(new CachePollProjectionRequest(request.Poll), cancellationToken);
    }
}