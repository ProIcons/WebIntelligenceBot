using System.IO;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Extensions;
using Remora.Discord.Core;

namespace WebIntelligence.Bot.Responders;

public class TestResponder //: IResponder<IMessageCreate>
{
    private readonly IDiscordRestChannelAPI _channelApi;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public TestResponder(IOptions<JsonSerializerOptions> jsonSerializerOptions, IDiscordRestChannelAPI channelApi)
    {
        _channelApi = channelApi;
        _jsonSerializerOptions = jsonSerializerOptions.Value;
    }

    public async Task<Result> RespondAsync(IMessageCreate gatewayEvent, CancellationToken ct = new())
    {
        if (gatewayEvent.Author.IsBot.HasValue && gatewayEvent.Author.IsBot.Value) return Result.FromSuccess();


        var opts = _jsonSerializerOptions.Clone();
        opts.WriteIndented = true;
        var payload = JsonSerializer.Serialize(gatewayEvent, opts);

        var stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));

        var result = await _channelApi.CreateMessageAsync(gatewayEvent.ChannelID,
            file: new FileData($"{Guid.NewGuid()}.json", stream), ct: ct);
        return result.IsSuccess
            ? Result.FromSuccess()
            : Result.FromError(result);
    }
}