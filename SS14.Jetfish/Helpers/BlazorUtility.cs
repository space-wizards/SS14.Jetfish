namespace SS14.Jetfish.Helpers;

public static class BlazorUtility
{
    public static IEnumerable<string> MaxCharacters(string ch, int max)
    {
        if (!string.IsNullOrEmpty(ch) && max < ch.Length)
            yield return $"Max {max} characters";
    }
}