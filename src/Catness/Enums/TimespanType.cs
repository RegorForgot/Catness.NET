using Discord.Interactions;

namespace Catness.Enums;

public class TimespanType
{
    private int Minute { get; set; }
    private int Hour { get; set; }
    private int Day { get; set; }

    public TimespanType() { }

    [ComplexParameterCtor]
    public TimespanType(
        [MinValue(0)] [MaxValue(60)] [Summary("Minutes")]
        int minute = 0,
        [MinValue(0)] [MaxValue(24)] [Summary("Hours")]
        int hour = 0,
        [MinValue(0)] [MaxValue(9999)] [Summary("Days")]
        int day = 0)
    {
        Hour = hour;
        Minute = minute;
        Day = day;
    }

    public bool IsValid() => Day != 0 || Hour != 0 || Minute != 0;

    public TimeSpan GetTimeSpan() => new TimeSpan(Day, Hour, Minute, 0);
}