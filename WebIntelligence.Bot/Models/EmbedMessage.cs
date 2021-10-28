using System.Collections.Generic;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using WebIntelligence.Bot.Models;

namespace WebIntelligence.Bot.Models;

public record EmbedMessage(
    IEmbed Embed,
    List<IMessageComponent>? MessageComponents = default,
    AllowedMentions? AllowedMentions = default
) : BaseMessage<EmbedMessage>(MessageComponents, AllowedMentions)
{
    public static explicit operator EmbedMessage(Embed message) => new(message);
}