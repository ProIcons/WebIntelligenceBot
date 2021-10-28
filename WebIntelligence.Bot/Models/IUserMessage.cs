using System.Collections.Generic;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;

namespace WebIntelligence.Bot.Models;

public interface IUserMessage
{
    List<IMessageComponent>? MessageComponents { get; }
    AllowedMentions? AllowedMentions { get; }
}