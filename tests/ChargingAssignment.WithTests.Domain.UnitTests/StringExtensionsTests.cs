using CharginAssignment.WithTests.Domain.Extensions;

namespace CharginAssignment.WithTests.Domain.UnitTests;

public class StringExtensionsTests
{
    [Theory]
    [InlineData("abc def ghi", "abc", "ghi", " def ")]
    [InlineData("abc def ghi", "abc", "xyz", "")]
    [InlineData("", "abc", "ghi", "")]
    [InlineData(null, "abc", "ghi", "")]
    public void Between_ReturnsStringBetweenTwoStrings(string? input, string str1, string str2, string expectedResult)
    {
        var result = input.Between(str1, str2);

        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("abc", "bc", "")]
    [InlineData("abcdfg", "bc", "dfg")]
    [InlineData("abc", "xy", "")]
    [InlineData("", "abc", "")]
    [InlineData(null, "abc", "")]
    public void After_ReturnsStringAfterSpecifiedString(string? input, string str, string expectedResult)
    {
        var result = input.After(str);

        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("abc", true)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsNotNullOrEmpty_ReturnsTrueIfStringIsNotNullOrEmpty(string? input, bool expectedResult)
    {
        var result = input.IsNotNullOrEmpty();

        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("abc", false)]
    [InlineData("", true)]
    [InlineData(null, true)]
    public void IsNullOrEmpty_ReturnsTrueIfStringIsNullOrEmpty(string? input, bool expectedResult)
    {
        var result = input.IsNullOrEmpty();

        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("abc", true)]
    [InlineData("  ", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsNotNullOrWhitespace_ReturnsTrueIfStringIsNotNullOrWhitespace(string? input, bool expectedResult)
    {
        var result = input.IsNotNullOrWhitespace();

        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("abc", false)]
    [InlineData("  ", true)]
    [InlineData("", true)]
    [InlineData(null, true)]
    public void IsNullOrWhitespace_ReturnsTrueIfStringIsNullOrWhitespace(string? input, bool expectedResult)
    {
        var result = input.IsNullOrWhitespace();

        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("123", 123)]
    [InlineData("-456", -456)]
    public void ToInt_ConvertsStringToInt(string? input, int expectedResult)
    {
        var result = input?.ToInt();

        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("123", false, 123)]
    [InlineData("", true, null)]
    [InlineData("abc", true, null)]
    public void ToInt_ConvertsStringToIntWithNullOption(string? input, bool flagAllowNull, int? expectedResult)
    {
        var result = input.ToInt(flagAllowNull);

        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("123", 123L)]
    [InlineData("-456", -456L)]
    public void ToLong_ConvertsStringToLong(string input, long expectedResult)
    {
        var result = input.ToLong();

        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("123.45", 123.45)]
    [InlineData("-456.78", -456.78)]
    public void ToDecimal_ConvertsStringToDecimal(string input, decimal expectedResult)
    {
        var result = input.ToDecimal();

        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("", null)]
    [InlineData(null, null)]
    [InlineData("abc", "abc")]
    public void SetNullIfEmpty_ReturnsNullIfStringIsEmpty(string? input, string? expectedResult)
    {
        var result = input.SetNullIfEmpty();

        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("123", true)]
    [InlineData("-456", true)]
    [InlineData("abc", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsInt_ReturnsTrueIfStringIsConvertibleToInt(string? input, bool expectedResult)
    {
        var result = input.IsInt();

        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("123", true)]
    [InlineData("-456", true)]
    [InlineData("abc", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsLong_ReturnsTrueIfStringIsConvertibleToLong(string? input, bool expectedResult)
    {
        var result = input.IsLong();

        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("123.45", true)]
    [InlineData("-456.78", true)]
    [InlineData("abc", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsDecimal_ReturnsTrueIfStringIsConvertibleToDecimal(string? input, bool expectedResult)
    {
        var result = input.IsDecimal();

        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("BigBoss", "bigBoss")]
    [InlineData("SmallBoss", "smallBoss")]
    [InlineData("", "")]
    [InlineData(null, "")]
    public void ToLowerFirstChar_ConvertsPascalCaseToCamelCase(string? input, string expectedResult)
    {
        var result = input.ToLowerFirstChar();

        result.Should().Be(expectedResult);
    }
}