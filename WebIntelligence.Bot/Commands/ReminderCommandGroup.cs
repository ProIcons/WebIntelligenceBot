using System.ComponentModel;
using System.Drawing;
using System.Text;
using Humanizer;
using MediatR;
using Remora.Commands.Attributes;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Conditions;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Core;
using WebIntelligence.Bot.CommandGroups;
using WebIntelligence.Bot.Extensions;
using WebIntelligence.Bot.Helpers;
using WebIntelligence.Bot.Models;
using WebIntelligence.Bot.Services;
using WebIntelligence.Common.Helpers;
using WebIntelligence.Common.Models;
using WebIntelligence.Common.Requests;

namespace WebIntelligence.Bot.Commands
{
    [Group("remind")]
    public class ReminderCommandGroup : WebIntelligenceCommandGroup
    {
        public ReminderCommandGroup(
            ICommandContext commandContext,
            IMediator mediator,
            IDiscordRestGuildAPI guildApi,
            UserFeedbackService userFeedbackService,
            DiscordAvatarHelper discordAvatarHelper
        ) : base(commandContext, mediator, guildApi, discordAvatarHelper, userFeedbackService)
        {
        }

        [Command("me"), Description("Add a reminder for the invoking user.")]
        public async Task<Result<IUserMessage>> AddReminder(TimeSpan timeSpan, string message)
        {
            var sanitizedMessage = message.DiscordSanitize();

            var response = await Mediator.Send(
                new AddReminderRequest(
                    CommandContext.User.ID.Value,
                    CommandContext.ChannelID.Value,
                    timeSpan,
                    sanitizedMessage),
                CancellationToken);


            return response.IsSuccess
                ? new StateMessage($"You will be reminded about it in {timeSpan.Humanize()}", Color.DarkGreen)
                : Result<IUserMessage>.FromError(new ServiceError(response.Error!.Message));
        }

        [Command("list"), Description("List the reminders of the invoking user.")]
        public async Task<Result<IUserMessage>> ListUserReminders(int page = 1)
        {
            var embed = await GetUserReminders(CommandContext.User.ID, page - 1);

            return new EmbedMessage(embed);
        }

        [RequireDiscordPermission(DiscordPermission.Administrator), Command("list-user"),
         Description("List the reminders of the specified user.")]
        public async Task<Result<IUserMessage>> ListUserReminders(IGuildMember member, int page = 1)
        {
            var embed = await GetUserReminders(member.User.Value!.ID, page - 1);
            return new EmbedMessage(embed);
        }


        [Command("delete"), Description("Deletes the reminder of the invoking user.")]
        public async Task<Result<IUserMessage>> DeleteReminder(int reminderId)
        {
            var response = await Mediator.Send(new DeleteReminderRequest(CommandContext.User.ID.Value, reminderId));

            return response.IsSuccess
                ? new InfoMessage("Your reminder has been Deleted")
                : new ErrorMessage(response.Error!.Message);
        }

        [Command("delete-user"), RequireDiscordPermission(DiscordPermission.Administrator),
         Description("Deletes a reminder of the specified user.")]
        public async Task<Result<IUserMessage>> DeleteReminder(IGuildMember guildMember, int reminderId)
        {
            var response = await Mediator.Send(new DeleteReminderRequest(guildMember.User.Value!.ID.Value, reminderId));

            return response.IsSuccess
                ? new InfoMessage($"{guildMember.User.Value.Username}'s reminder has been deleted.")
                : new ErrorMessage(response.Error!.Message);
        }

        [Command("delete-all"), Description("Deletes all the reminders of the invoking user.")]
        public async Task<Result<IUserMessage>> DeleteAllReminders()
        {
            var response = await Mediator.Send(new DeleteAllRemindersRequest(CommandContext.User.ID.Value));

            return response.IsSuccess
                ? new InfoMessage("Your reminders have been deleted.")
                : new ErrorMessage(response.Error!.Message);
        }

        [Command("delete-user-all"), RequireDiscordPermission(DiscordPermission.Administrator),
         Description("Deletes all the reminders of the specified user.")]
        public async Task<Result<IUserMessage>> DeleteAllReminders(IGuildMember guildMember)
        {
            var response = await Mediator.Send(new DeleteAllRemindersRequest(guildMember.User.Value!.ID.Value));

            return response.IsSuccess
                ? new InfoMessage($"{guildMember.User.Value.Username}'s reminders has been deleted.")
                : new ErrorMessage(response.Error!.Message);
        }

        private async Task<Embed> GetUserReminders(Snowflake id, int page = 0)
        {
            var userResponse = await Mediator.Send(new GetUserRequest(id.Value));

            if (!userResponse.IsSuccess)
            {
                return new Embed(Description: userResponse.Error!.Message);
            }

            var response = await Mediator.Send(new GetRemindersRequest(id.Value));
            if (!response.IsSuccess)
            {
                return new Embed(Description: response.Error!.Message);
            }

            var guildUserEntity = await GuildApi.GetGuildMemberAsync(CommandContext.GuildID.Value, id);

            if (!guildUserEntity.IsSuccess || !guildUserEntity.Entity.User.HasValue)
            {
                return new Embed(Description: "Couldn't find user in Guild");
            }

            var guildUser = guildUserEntity.Entity;
            var userDto = userResponse.Entity!.User;

            var avatarUrl = DiscordAvatarHelper.GetAvatarUrl(guildUser.User.Value);

            var userHandle = !string.IsNullOrWhiteSpace(userDto.UsernameWithDiscriminator)
                ? userDto.UsernameWithDiscriminator
                : DiscordHandleHelper.BuildHandle(guildUser.User.Value.Username, guildUser.User.Value.Discriminator);


            var totalReminders = response.Entity!.Count;

            var usedReminders = response.Entity!.OrderByDescending(x => x.CreatedAt).Skip(page * 5).Take(5);
            var sb = new StringBuilder();

            var userReminders = usedReminders as ReminderDto[] ?? usedReminders.ToArray();
            if (userReminders.Any())
            {
                foreach (var reminder in userReminders)
                {
                    sb.AppendLine("```css");
                    sb.AppendLine($"[{reminder.Id}] {reminder.Message.Truncate(10, "..."),-10} in {reminder.RemindAt.Humanize()}");
                    sb.Append("```");
                }
            }
            else
            {
                sb.AppendLine("User has no reminders");
            }

            int start = 1 + page * 5;
            int end = page * 5 + userReminders.Length;

            string title = "Reminders";
            string content = sb.ToString();

            if (start <= end)
            {
                title += $" ({(start != end ? $"{start}-{end}" : $"{start}")}/{totalReminders})";
            }
            else if (page > 0)
            {
                content = "User has no more reminders";
            }

            return new Embed(Author: new EmbedAuthor(userHandle, IconUrl: avatarUrl),
                Title: title, Description: content);
        }
    }
}