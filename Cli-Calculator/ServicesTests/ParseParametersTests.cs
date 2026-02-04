using Domain.Extensions;

namespace CliCalculator;

public class ParseParametersTests
{
    [Theory]
    [InlineData("--AdditionalDelimiter \b", "AdditionalDelimiter", "\b")]
    [InlineData("--AdditionalDelimiter ;", "AdditionalDelimiter", ";")]
    [InlineData("--AdditionalDelimiter ; --UpperOverride 100", "UpperOverride", "100")]
    [InlineData("--AdditionalDelimiter ; --AllowNegatives --UpperOverride 100", "AllowNegatives", "true")]
    [InlineData("--AdditionalDelimiter ; --AllowNegatives true --UpperOverride 100", "AllowNegatives", "true")]
    [InlineData("--AdditionalDelimiter ; --AllowNegatives false --UpperOverride 100", "AllowNegatives", "false")]
    [InlineData("--AdditionalDelimiter ; --AllowNegatives false --UpperOverride -1", "UpperOverride", "-1")]
    public void ShouldParseAdditionalDelimiter(string arg, string targetParam, string expected)
    {
        var pairs = arg.Split(' ').ParseParameters();
        Assert.Equal(expected.ToLower(), pairs.First(x => x.Key == targetParam).Value.ToString().ToLower());
    }
}