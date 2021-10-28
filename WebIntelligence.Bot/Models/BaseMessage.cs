using System.Collections.Generic;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Results;

namespace WebIntelligence.Bot.Models;

public abstract record BaseMessage<T>(
    List<IMessageComponent>? MessageComponents = default,
    AllowedMentions? AllowedMentions = default
) : IUserMessage where T : BaseMessage<T>
{
    public static implicit operator Result<T>(BaseMessage<T> baseMessage) => Result<T>.FromSuccess((T)baseMessage);

    public static implicit operator Task<Result<T>>(BaseMessage<T> baseMessage) =>
        Task.FromResult(Result<T>.FromSuccess((T)baseMessage));

    public static implicit operator Task<Result<IUserMessage>>(BaseMessage<T> baseMessage) =>
        Task.FromResult(Result<IUserMessage>.FromSuccess((T)baseMessage));
}