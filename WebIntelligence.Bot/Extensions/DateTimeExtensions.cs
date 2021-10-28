namespace WebIntelligence.Bot.Extensions;

public static class DateTimeExtensions
{
    public static string ToDiscordDateMarkdown(this DateTimeOffset dateTime) => $"<t:{dateTime.ToUnixTimeSeconds()}:R>";
}