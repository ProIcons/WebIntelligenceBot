using LazyCache;
using MediatR;
using Microsoft.VisualBasic.CompilerServices;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Gateway.Responders;
using WebIntelligence.Common.Models;
using WebIntelligence.Common.Requests;

namespace WebIntelligence.Bot.Responders;

public class PollResponder : IResponder<IInteractionCreate>
{
    private readonly IDiscordRestInteractionAPI _interactionApi;
    private readonly IMediator _mediator;

    public PollResponder(IDiscordRestInteractionAPI interactionApi, IMediator mediator)
    {
        _interactionApi = interactionApi;
        _mediator = mediator;
    }

    public async Task<Result> RespondAsync(IInteractionCreate gatewayEvent, CancellationToken ct = new CancellationToken())
    {
        if (gatewayEvent.Type is not InteractionType.MessageComponent
            || !gatewayEvent.Message.HasValue
            || !gatewayEvent.Member.HasValue
            || gatewayEvent.User.HasValue
            && gatewayEvent.User.Value.IsBot.HasValue
            && !gatewayEvent.User.Value.IsBot.Value
            || !gatewayEvent.Member.Value.User.HasValue
            || !gatewayEvent.ChannelID.HasValue
            || !gatewayEvent.Data.HasValue
            || !gatewayEvent.Data.Value.CustomID.HasValue)
        {
            return Result.FromSuccess();
        }

        var user = gatewayEvent.Member.Value.User.Value;
        var lgtmData = gatewayEvent.Data.Value;
        var lgtmType = lgtmData.ComponentType.Value;
        var lgtmRespondDeferred = await _interactionApi.CreateInteractionResponseAsync
        (
            gatewayEvent.ID,
            gatewayEvent.Token,
            new InteractionResponse(InteractionCallbackType.DeferredUpdateMessage),
            ct
        );

        if (!lgtmRespondDeferred.IsSuccess)
        {
            return lgtmRespondDeferred;
        }

        var lgtmChannelId = gatewayEvent.ChannelID.Value.Value;

        if (lgtmType == ComponentType.SelectMenu)
        {
            var exists = await _mediator.Send(new DoesPollExistRequest(Guid.Parse(lgtmData.CustomID.Value)), ct);
            if (exists)
            {
                await _mediator.Send(new PollProcessVoteRequest(lgtmChannelId, user.ID.Value, lgtmData.CustomID.Value, lgtmData.Values.Value[0]), ct);
            }
        }

        return Result.FromSuccess();
    }
}