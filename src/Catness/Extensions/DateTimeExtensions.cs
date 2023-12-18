using NodaTime;

namespace Catness.Extensions;

public static class DateTimeExtensions
{
    public static DateTime GetUtcDateTimeWithTimeZone(this DateTime discordDateTime, string locality)
    {
        DateTimeZone timeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(locality) ?? DateTimeZone.Utc;

        return DateTime.SpecifyKind(discordDateTime, DateTimeKind.Utc) -
               timeZone.GetUtcOffset(Instant.FromDateTimeUtc(DateTime.UtcNow)).ToTimeSpan();
    }

    public static bool IsTimeBeforeNow(this DateTime discordDateTime)
    {
        return DateTime.SpecifyKind(discordDateTime, DateTimeKind.Utc) < DateTime.UtcNow;
    }
}