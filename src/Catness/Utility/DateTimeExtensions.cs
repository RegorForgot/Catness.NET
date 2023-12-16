using NodaTime;

namespace Catness.Utility;

public static class DateTimeExtensions
{
    public static DateTime GetUtcDateTimeWithTimeZone(this DateTime discordDateTime, string locality)
    {
        DateTimeZone timeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(locality) ?? DateTimeZone.Utc;

        return Instant
            .FromDateTimeUtc(DateTime.SpecifyKind(discordDateTime, DateTimeKind.Utc))
            .InZone(timeZone)
            .ToDateTimeUtc();
    }

    public static bool IsTimeBeforeNow(this DateTime discordDateTime)
    {
        return DateTime.SpecifyKind(discordDateTime, DateTimeKind.Utc) < DateTime.UtcNow;
    }
}