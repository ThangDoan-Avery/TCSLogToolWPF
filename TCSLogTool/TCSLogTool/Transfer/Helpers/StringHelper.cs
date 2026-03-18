using System.Text.RegularExpressions;

public static class StringHelper
{
    public static string ToLabel(string raw)
    {
        return Regex.Replace(raw, "(\\B[A-Z])", " $1");
    }
}