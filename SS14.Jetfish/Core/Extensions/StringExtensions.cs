namespace SS14.Jetfish.Core.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Truncates a given input string.
    /// </summary>
    public static string Truncate(this string s, int maxLength, string postfix = "…")
    {
        if (string.IsNullOrEmpty(s) || maxLength <= 0 || s.Length <= maxLength)
            return s;

        var truncatedLength = maxLength - postfix.Length;

        return string.Concat(s.AsSpan(0, truncatedLength), postfix);
    }
}
