using System.Collections.Generic;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using WebIntelligence.Bot.Models;

namespace WebIntelligence.Bot.Models;

public record TextMessage(
    string Content,
    List<IMessageComponent>? MessageComponents = default,
    AllowedMentions? AllowedMentions = default
) : BaseMessage<TextMessage>(MessageComponents, AllowedMentions)
{
    public static explicit operator TextMessage(string message) => new(message);
}