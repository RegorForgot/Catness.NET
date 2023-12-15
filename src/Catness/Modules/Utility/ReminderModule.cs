using Catness.Enums;
using Catness.Persistence.Models;
using Catness.Services.EFServices;
using Discord;
using Discord.Interactions;
using NodaTime;

namespace Catness.Modules.Utility;

[Group("reminder", "Reminder actions")]
public class ReminderModule : InteractionModuleBase
{
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
            DateTime dateTime,
            bool privateReminder = false,
            string text = "")
        {
            User user = await _userService.GetOrAddUser(Context.User.Id);

            ulong? channelId = null;

            if (Context.Channel is ITextChannel channel)
            {
                channelId = channel.Id;
            }

            user.Locale ??= "Etc/UTC";
            DateTimeZone timeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(user.Locale) ?? DateTimeZone.Utc;

            DateTimeOffset dateTimeWithTimeZone = Instant
                .FromDateTimeOffset(new DateTimeOffset(dateTime, TimeSpan.Zero))
                .InZone(timeZone)
                .ToDateTimeUtc();

            Reminder reminder = new Reminder
            {
                ReminderGuid = Guid.NewGuid(),
                User = user,
                Body = text,
                PrivateReminder = privateReminder,
                ReminderTime = dateTimeWithTimeZone,
                ChannelId = channelId,
                TimeCreated = Context.Interaction.CreatedAt
            };

            await _reminderService.AddReminder(reminder);
            await RespondAsync($"Added reminder on {TimestampTag.FromDateTimeOffset(dateTimeWithTimeZone)}", ephemeral: privateReminder);
        }

        [SlashCommand("from", "Add a reminder for a certain period from now")]
        public async Task AddReminderFrom(
            [ComplexParameter] TimespanType reminderTimeSpan,
            bool privateReminder = false,
            string text = "")
        {
            try
            {
                User user = await _userService.GetOrAddUser(Context.User.Id);

                ulong? channelId = null;

                if (Context.Channel is ITextChannel channel)
                {
                    channelId = channel.Id;
                }

                TimeSpan timeSpan = reminderTimeSpan.GetTimeSpan();
                DateTimeOffset reminderTime = DateTimeOffset.UtcNow + timeSpan;

                Reminder reminder = new Reminder
                {
                    ReminderGuid = Guid.NewGuid(),
                    UserId = Context.User.Id,
                    Body = text,
                    PrivateReminder = privateReminder,
                    ReminderTime = reminderTime,
                    ChannelId = channelId,
                    TimeCreated = Context.Interaction.CreatedAt
                };

                await _reminderService.AddReminder(reminder);
                await RespondAsync($"Added reminder on {TimestampTag.FromDateTimeOffset(reminderTime)}", ephemeral: privateReminder);
            }
            catch (Exception ex)
            {
                await RespondAsync(ex.InnerException?.Message);
            }
        }
    }
}