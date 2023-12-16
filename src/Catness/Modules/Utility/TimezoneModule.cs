using Catness.Autocomplete;
using Catness.Persistence.Models;
using Catness.Services.EFServices;
using Discord.Interactions;
using NodaTime;

namespace Catness.Modules.Utility;

[Group("timezone", "Timezone tools")]
public class TimezoneModule : InteractionModuleBase
{
    private readonly UserService _userService;

    public TimezoneModule(
        UserService userService)
    {
        _userService = userService;
    }

    [SlashCommand("set", "Set your timezone")]
    public async Task SetLocale(
        [Summary(description: "Your locale")] [Autocomplete(typeof(TimezoneAutocomplete))]
        string? locale = null)
    {
        await DeferAsync(ephemeral: true);

        User user = await _userService.GetOrAddUser(Context.User.Id);

        if (locale is null)
        {
            user.Locale = DateTimeZone.Utc.Id;
            await _userService.UpdateUser(user);

            await FollowupAsync("Reset timezone to UTC", ephemeral: true);
            return;
        }

        DateTimeZone? zone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(locale);

        if (zone is null)
        {
            await FollowupAsync("Invalid locale provided", ephemeral: true);
            return;
        }

        user.Locale = zone.Id;
        await _userService.UpdateUser(user);

        await FollowupAsync($"Set locality to {locale}",
            ephemeral: true);
    }
}