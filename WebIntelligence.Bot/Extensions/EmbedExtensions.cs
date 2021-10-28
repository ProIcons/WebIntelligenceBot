using Remora.Discord.API.Abstractions.Objects;

namespace WebIntelligence.Bot.Extensions;

public static class EmbedExtensions
{
    public static int CalculateEmbedLength(this IEmbed embed)
    {
        var length = 0;

        if (embed.Title.HasValue)
        {
            length += embed.Title.Value.Length;
        }

        if (embed.Description.HasValue)
        {
            length += embed.Description.Value.Length;
        }

        if (embed.Fields.HasValue)
        {
            foreach (var field in embed.Fields.Value)
            {
                length += field.Name.Length;
                length += field.Value.Length;
            }
        }

        if (embed.Author.HasValue)
        {
            length += embed.Author.Value.Name.Length;
        }

        if (embed.Footer.HasValue)
        {
            length += embed.Footer.Value.Text.Length;
        }

        return length;
    }
}