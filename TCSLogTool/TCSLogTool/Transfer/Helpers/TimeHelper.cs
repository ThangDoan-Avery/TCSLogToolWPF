public static class TimeHelper
{
    public static long ToUnixMs(DateTimeOffset dt)
        => dt.ToUnixTimeMilliseconds();
}