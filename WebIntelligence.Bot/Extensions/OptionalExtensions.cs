using Remora.Discord.Core;

namespace WebIntelligence.Bot.Extensions;

public static class OptionalExtensions
{
    public static string ToYesNo(this Optional<bool> input)
    {
        return input.HasValue ? "Yes" : "No";
    }
}