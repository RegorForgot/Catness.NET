using System.Text;
using Catness.Enums;
using Catness.Persistence.Models;
using Catness.Services.EntityFramework;
using Catness.Services.Timed;
using Catness.Utility;
using Discord;
using Discord.Interactions;

namespace Catness.Modules.Utility;

[Group("reminder", "Reminder actions")]
public class ReminderModule : InteractionModuleBase
{
    private readonly ReminderService _reminderService;
    private readonly ReminderDispatchService _reminderDispatchService;

    public ReminderModule(
        ReminderService reminderService,
        ReminderDispatchService reminderDispatchService)
    {
        _reminderService = reminderService;
        _reminderDispatchService = reminderDispatchService;
    }

    [SlashCommand("cancel", "Cancel a reminder")]
    public async Task CancelReminder(
        string reminderId,
        bool ephemeral = false)
    {
        if (!Guid.TryParse(reminderId, out Guid reminderGuid))
        {
            await RespondAsync("Bad reminder ID provided", ephemeral: ephemeral);
            return;
        }

        await DeferAsync(ephemeral: ephemeral);

        Reminder? dbReminder = await _reminderService.GetReminder(reminderGuid);

        if (dbReminder is null || dbReminder.UserId != Context.User.Id)
        {
            await FollowupAsync("You do not have a reminder with this ID.", ephemeral: ephemeral);
            return;
        }

        await _reminderDispatchService.StopReminder(dbReminder);
        await FollowupAsync($"Deleted reminder with id {reminderGuid}", ephemeral: ephemeral);
    }

    [SlashCommand("list", "List the user's reminders")]
    public async Task ListUserReminders(
        bool includePrivate = false)
    {
        await DeferAsync();
        List<Reminder> reminders = await _reminderService.GetUserReminders(Context.User.Id, includePrivate);

        string username;

        if (Context.Interaction.User is IGuildUser guildUser)
        {
            username = guildUser.Nickname ?? guildUser.DisplayName ?? guildUser.Username;
        }
        else
        {
            username = Context.User.Username;
        }

        if (reminders.Count == 0)
        {
            await FollowupAsync("You have no reminders!", ephemeral: true);
            return;
        }

        StringBuilder sb = new StringBuilder();

        foreach (Reminder reminder in reminders)
        {
            sb.AppendLine($"Id: {reminder.ReminderGuid}");
            sb.AppendLine($"Reminder on: {new TimestampTag(new DateTimeOffset(reminder.ReminderTime))}");
            sb.AppendLine($"Created on: {new TimestampTag(new DateTimeOffset(reminder.TimeCreated))}");
            sb.AppendLine($"For: {reminder.Body.Truncate(20)}");
            sb.AppendLine();
        }

        Embed embed = new EmbedBuilder()
            .WithTitle($"Reminders for {username}")
            .WithDescription(sb.ToString().TrimEnd())
            .WithCurrentTimestamp()
            .Build();

        await FollowupAsync(embed: embed, ephemeral: includePrivate);
    }

    [Group("add", "Add a reminder")]
    public class ReminderAddModule : InteractionModuleBase
    {
        private readonly ReminderService _reminderService;
        private readonly UserService _userService;

        public ReminderAddModule(
            ReminderService reminderService,
            UserService userService)
        {
            _reminderService = reminderService;
            _userService = userService;
        }

        [SlashCommand("on", "Add a reminder on a specific date and time")]
        public async Task AddReminderOn(
            string dateTimeString,
            bool privateReminder = false,
            string text = "")
        {
            if (!DateTime.TryParse(dateTimeString, out DateTime dateTime))
            {
                await RespondAsync("Please enter a valid date and/or time", ephemeral: privateReminder);
            }

            await DeferAsync();
            try
            {
                User user = await _userService.GetOrAddUser(Context.User.Id);

                ulong? channelId = null;

                if (Context.Channel is ITextChannel channel)
                {
                    channelId = channel.Id;
                }

                DateTime dateTimeWithTimeZone = dateTime.GetUtcDateTimeWithTimeZone(user.Locale ??= "Etc/UTC");

                if (dateTimeWithTimeZone.IsTimeBeforeNow())
                {
                    await FollowupAsync("The time you have chosen is before the present!", ephemeral: true);
                    return;
                }

                Reminder reminder = new Reminder
                {
                    ReminderGuid = Guid.NewGuid(),
                    UserId = Context.User.Id,
                    Body = text,
                    PrivateReminder = privateReminder,
                    ReminderTime = dateTimeWithTimeZone,
                    ChannelId = channelId,
                    TimeCreated = Context.Interaction.CreatedAt.UtcDateTime
                };

                bool success = await _reminderService.AddReminder(reminder);

                string response = success
                    ? $"You will be reminded {TimestampTag.FromDateTimeOffset(dateTimeWithTimeZone, TimestampTagStyles.Relative)}!"
                    : $"You have too many reminders (7 max)! Please delete one before adding another.";
                await FollowupAsync(response, ephemeral: success && privateReminder);
            }
            catch (Exception ex)
            {
                await FollowupAsync(ex.Message);
            }
        }

        [SlashCommand("from", "Add a reminder for a certain period from now")]
        public async Task AddReminderFrom(
            [ComplexParameter] TimespanType reminderTimeSpan,
            bool privateReminder = false,
            string text = "")
        {
            if (!reminderTimeSpan.IsValid())
            {
                await RespondAsync("Please select a non-zero time.", ephemeral: true);
            }

            await DeferAsync();
            try
            {
                _ = await _userService.GetOrAddUser(Context.User.Id);

                ulong? channelId = null;

                if (Context.Channel is ITextChannel channel)
                {
                    channelId = channel.Id;
                }

                TimeSpan timeSpan = reminderTimeSpan.GetTimeSpan();
                DateTime reminderTime = DateTime.UtcNow + timeSpan;

                Reminder reminder = new Reminder
                {
                    ReminderGuid = Guid.NewGuid(),
                    UserId = Context.User.Id,
                    Body = text,
                    PrivateReminder = privateReminder,
                    ReminderTime = reminderTime,
                    ChannelId = channelId,
                    TimeCreated = Context.Interaction.CreatedAt.UtcDateTime
                };

                bool success = await _reminderService.AddReminder(reminder);

                string response = success
                    ? $"You will be reminded {TimestampTag.FromDateTimeOffset(reminderTime, TimestampTagStyles.Relative)}!"
                    : $"You have too many reminders (7 max)! Please delete one before adding another.";
                await FollowupAsync(response, ephemeral: success && privateReminder);
            }
            catch (Exception ex)
            {
                await FollowupAsync(ex.InnerException?.Message);
            }
        }
    }
}