namespace Catness.Utility;

public static class StringExtensions
{
    public static string? Truncate(this string? value, int maxChars)
        =>
            string.IsNullOrEmpty(value) ? value :
            value.Length <= maxChars ? value :
            value[..maxChars] + "...";
}