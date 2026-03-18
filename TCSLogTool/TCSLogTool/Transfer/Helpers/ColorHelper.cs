public static class ColorHelper
{
    private static Random _rand = new();

    public static string RandomColor()
    {
        return $"#{_rand.Next(0x1000000):X6}";
    }
}