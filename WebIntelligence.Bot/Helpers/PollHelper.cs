using System.Text;
using Humanizer;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.Core;
using WebIntelligence.Common.Models;

namespace WebIntelligence.Bot.Helpers;

public static class PollHelper
{
    public static List<IMessageComponent> GetPollSelectMenu(PollDto pollDto) =>
        new()
        {
            new ActionRowComponent(new IMessageComponent[]
            {
                new SelectMenuComponent(
                    pollDto.Id.ToString(),
                    pollDto.Options.Select(x =>
                            new SelectOption(
                                x.Value,
                                x.Id,
                                new Optional<string>(),
                                default,
                                false))
                        .ToArray(),
                    "Select an option",
                    default,
                    default,
                    false
                )
            })
        };

    public static Embed GetPollEmbed(PollDto pollDto, bool voteConcluded = false)
    {
        StringBuilder embedBody = new();

        embedBody.AppendLine("The **Question** is:");
        embedBody.AppendLine(pollDto.Question);

        List<EmbedField> embedFields = new();
        PollOptionDto? mostPopularOption = null;
        foreach (var option in pollDto.Options)
        {
            if (mostPopularOption == null || mostPopularOption.UserVotes.Count < option.UserVotes.Count)
            {
                mostPopularOption = option;
            }

            var activeBars = pollDto.UserVotes.Count > 0
                ? (int)(Math.Floor(option.UserVotes.Count * 100d / pollDto.UserVotes.Count) / 10)
                : 0;
            var inactiveBars = 10 - activeBars;
            embedFields.Add(new EmbedField(
                $"{option.Value} {(voteConcluded ? $"({option.UserVotes.Count} votes)" : "")}",
                $"{new string('⬜', activeBars)}{new string('⬛', inactiveBars)}"
            ));
        }

        if (voteConcluded)
        {
            embedBody.AppendLine("**Vote Concluded**");
            embedBody.AppendLine("Most Popular Answer: ");
            embedBody.AppendLine("```");
            embedBody.AppendLine(mostPopularOption?.Value);
            embedBody.AppendLine("```");
        }

        EmbedFooter embedFooter =
            new(voteConcluded
                ? "Finished"
                : $"Finishing in {(DateTimeOffset.UtcNow - pollDto.EndingTime).Humanize()}");

        return new Embed
        {
            Title = "Poll",
            Description = embedBody.ToString(),
            Fields = embedFields,
            Footer = embedFooter
        };
    }
}