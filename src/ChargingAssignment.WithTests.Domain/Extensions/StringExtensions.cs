using System.Diagnostics.CodeAnalysis;

namespace CharginAssignment.WithTests.Domain.Extensions;

public static class StringExtensions
{
    /// <summary>
    ///Get String Between Two String In a String
    /// </summary>      
    public static string Between(this string? value, string str1, string str2)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "";

        var str1BeginIndex = value.IndexOf(str1, StringComparison.Ordinal);
        if (str1BeginIndex == -1)
            return "";
        var str2BeginIndex = value.IndexOf(str2, str1BeginIndex, StringComparison.Ordinal);
        if (str2BeginIndex == -1)
            return "";
        var str1EndIndex = str1BeginIndex + str1.Length;
        return str1EndIndex >= str2BeginIndex ? ""
            : value.Substring(str1EndIndex, str2BeginIndex - str1EndIndex);
    }

    public static string After(this string? value, string str)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "";

        var strBeginIndex = value.LastIndexOf(str, StringComparison.Ordinal);
        if (strBeginIndex == -1)
            return "";
        var strEndIndex = strBeginIndex + str.Length;
        return strBeginIndex >= value.Length ? "" : value.Substring(strEndIndex);
    }

    /// <summary>
    /// Check if a string has value or is null / empty
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsNotNullOrEmpty([NotNullWhen(true)] this string? value)
    {
        return string.IsNullOrEmpty(value) is false;
    }

    public static bool IsNullOrEmpty([NotNullWhen(false)] this string? value)
    {
        return string.IsNullOrEmpty(value);
    }

    /// <summary>
    /// = !string.IsNullOrWhitespace(string)
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsNotNullOrWhitespace([NotNullWhen(true)] this string? value)
    {
        return string.IsNullOrWhiteSpace(value) is false;
    }

    public static bool IsNullOrWhitespace([NotNullWhen(false)] this string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    public static int ToInt(this string value)
    {
        return Convert.ToInt32(value.Trim());
    }

    public static int? ToInt(this string? value, bool flagAllowNull)
    {
        return value.IsInt() ? Convert.ToInt32(value!.Trim()) : null;
    }

    public static long ToLong(this string value)
    {
        return Convert.ToInt64(value.Trim());
    }

    public static decimal ToDecimal(this string value)
    {
        return Convert.ToDecimal(value.Trim());
    }

    /// <summary>
    /// Set null if string is empty ("")
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string? SetNullIfEmpty(this string? str)
    {
        return str?.Length == 0 ? null : str;
    }

    /// <summary>
    /// Is string convertible to Int type
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool IsInt(this string? str)
    {
        if (string.IsNullOrWhiteSpace(str))
            return false;

        var cleanedStr = str.Trim();
        return int.TryParse(cleanedStr, out _);
    }

    /// <summary>
    /// Is string convertible to Long type
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool IsLong(this string? str)
    {
        if (string.IsNullOrWhiteSpace(str))
            return false;

        var cleanedStr = str.Trim();
        return long.TryParse(cleanedStr, out _);
    }

    /// <summary>
    /// Is string convertible to Decimal type
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>

    public static bool IsDecimal(this string? str)
    {
        if (string.IsNullOrWhiteSpace(str))
            return false;

        var cleanedStr = str.Trim();
        return decimal.TryParse(cleanedStr, out _);
    }

    /// <summary>
    /// Convert BigBoss to bigBoss (PascalCase to camelCase)
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string ToLowerFirstChar(this string? str)
    {
        if (string.IsNullOrWhiteSpace(str))
            return "";

        return char.ToLowerInvariant(str[0]) + str.Substring(1);
    }
}