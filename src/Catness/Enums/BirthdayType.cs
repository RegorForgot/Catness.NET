using Discord.Interactions;

namespace Catness.Enums;

public class BirthdayType
{
    private int Day { get; set; }
    private Month Month { get; set; }
    private int Year { get; set; } = 1;

    public BirthdayType() { }

    [ComplexParameterCtor]
    public BirthdayType(
        [MinValue(1)] [MaxValue(31)] [Summary("Day", "Day of birth")]
        int day,
        [Summary("Month", "Month of birth")] Month month,
        [MinValue(1)] [MaxValue(9999)] [Summary("Year", "Year of birth (optional)")]
        int year = 1)
    {
        Day = day;
        Month = month;
        Year = year;
    }

    public DateOnly GetDate()
    {
        return new DateOnly(Year, (int)Month, Day);
    }
}

public enum Month
{
    January = 1,
    February = 2,
    March = 3,
    April = 4,
    May = 5,
    June = 6,
    July = 7,
    August = 8,
    September = 9,
    October = 10,
    November = 11,
    December = 12
}