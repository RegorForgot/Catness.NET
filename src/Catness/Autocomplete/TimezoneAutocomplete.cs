// Thanks to Frikandel, the developer of .fmbot, for the below autocomplete handler

using Catness.Utility;
using Discord;
using Discord.Interactions;
using TimeZoneNames;

namespace Catness.Autocomplete;

public class TimezoneAutocomplete : AutocompleteHandler
{
    private readonly IDictionary<string, string> _allTimeZones = TZNames.GetDisplayNames("en-US", true);

    public async override Task<AutocompletionResult> GenerateSuggestionsAsync(
        IInteractionContext context,
        IAutocompleteInteraction autocompleteInteraction,
        IParameterInfo parameter,
        IServiceProvider services)
    {
        var results = new Dictionary<string, string>();

        if (autocompleteInteraction.Data?.Current?.Value == null ||
            string.IsNullOrWhiteSpace(autocompleteInteraction.Data?.Current?.Value.ToString()))
        {
            results.ReplaceOrAddToDictionary(new Dictionary<string, string>
            {
                { "null", "Start typing to search for more timezones" }
            });
        }
        else
        {
            string? searchValue = autocompleteInteraction.Data.Current.Value.ToString();

            results.ReplaceOrAddToDictionary(_allTimeZones
                .Where(w => w.Value.StartsWith(searchValue, StringComparison.OrdinalIgnoreCase))
                .Take(5)
                .ToDictionary(d => d.Key, d => d.Value));

            results.ReplaceOrAddToDictionary(_allTimeZones
                .Where(w => w.Value.Contains(searchValue, StringComparison.OrdinalIgnoreCase))
                .Take(5)
                .ToDictionary(d => d.Key, d => d.Value));
        }

        return await Task.FromResult(
            AutocompletionResult.FromSuccess(results.Select(s => new AutocompleteResult(s.Value, s.Key))));
    }
}