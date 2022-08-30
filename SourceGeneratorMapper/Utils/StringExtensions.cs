namespace SourceGeneratorMapper.Utils;

public static class StringExtensions
{
    public static string? AppendString(this string? prefix, string suffix)
    {
        return prefix == null ? null : prefix + suffix;
    }
}