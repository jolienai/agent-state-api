namespace AgentState.Application.Shared;

internal static class DatetimeExtensions
{
    public static bool IsLessThanOneHour(this DateTime timestampUtc)
    {
        return DateTime.UtcNow - timestampUtc > TimeSpan.FromHours(1);
    }

    public static bool IsLunchTime(this DateTime timestampUtc)
    {
        var time = timestampUtc.TimeOfDay;

        var lunchStart = TimeSpan.FromHours(11);
        var lunchEnd = TimeSpan.FromHours(13);

        return time >= lunchStart && time <= lunchEnd;
    }
}