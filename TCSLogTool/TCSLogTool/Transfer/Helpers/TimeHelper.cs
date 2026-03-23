public static class TimeHelper
{
    public static long ToUnixMs(DateTimeOffset dt)
        => dt.ToUnixTimeMilliseconds();

    public static int GetOffsetMinutes(DateTimeOffset dt)
    => (int)dt.Offset.TotalMinutes;
}