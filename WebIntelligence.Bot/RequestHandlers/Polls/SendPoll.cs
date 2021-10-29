using LazyCache;
using MediatR;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Core;
using WebIntelligence.Bot.Helpers;
using WebIntelligence.Common.Helpers;
using WebIntelligence.Common.Requests;

namespace WebIntelligence.Bot.RequestHandlers.Polls;

public class SendPollRequestHandler : WebIntelligenceRequestHandler, IRequestHandler<SendPollRequest, Result<ulong>>
{
    private readonly IDiscordRestChannelAPI _channelApi;

    public SendPollRequestHandler(IMediator mediator, IAppCache appCache, IDiscordRestChannelAPI channelApi) : base(mediator, appCache)
    {
        _channelApi = channelApi;
    }

    public async Task<Result<ulong>> Handle(SendPollRequest request, CancellationToken cancellationToken)
    {
        var result = await _channelApi.CreateMessageAsync(new Snowflake(request.Poll.ChannelId),
            embeds: new[] { PollHelper.GetPollEmbed(request.Poll) },
            components: PollHelper.GetPollSelectMenu(request.Poll), ct: cancellationToken);
        
        return result.IsSuccess
            ? Results.Success(result.Entity.ID.Value)
            : Results.Fail<ulong>("Message not sent");
        
    }


    
}