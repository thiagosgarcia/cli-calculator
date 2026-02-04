using Cli_Calculator.Operations;
using Cli_Calculator.Services;
using Domain.ExceptionHandling;
using Domain.Models;
using Microsoft.Extensions.Options;
using Moq;

namespace CliCalculator;

public class IntegrationTests
{
    private OperationService operationService;
    private Mock<IOptionsSnapshot<ApplicationOptions>> optionsMock;
    private Mock<IOptionsSnapshot<ApplicationParameters>> paramsMock;

    public IntegrationTests()
    {
        MockOptions();
    }

    private void MockOptions(int? maxCount = null, string? extraSeparator = null, bool? allowNegatives = false, int? maxValue = null)
    {
        var defaultOptions = new ApplicationOptions
        {
            ExitOnError = false,
            MaxNumbers = maxCount
        };
        optionsMock = new Mock<IOptionsSnapshot<ApplicationOptions>>();
        optionsMock.Setup(o => o.Value).Returns(defaultOptions);

        var defaultParameters = new ApplicationParameters()
        {
            AdditionalDelimiter = extraSeparator,
            AllowNegatives = allowNegatives,
            UpperOverride = maxValue
        };
        paramsMock = new Mock<IOptionsSnapshot<ApplicationParameters>>();
        paramsMock.Setup(o => o.Value).Returns(defaultParameters);

        operationService = new OperationService(optionsMock.Object, paramsMock.Object, new SumOperation());
    }

    [Theory]
    [InlineData("5,tytyt", 5)]
    [InlineData("1,2,3,4,5,6,7,8,9,10,11,12", 78)]
    [InlineData("1\n2,3", 6)]
    [InlineData("//#\n2#5", 7)]
    [InlineData("//[***]\n11***22***33", 66)]
    [InlineData("//[*][!!][r9r]\n11r9r22*hh*33!!44", 110)]
    [InlineData("2,,4,rrrr,1001,6", 12)]
    public void ProvidedSamples(string sample, long expected)
    {
        Assert.Equal(expected, operationService.Execute(sample));
    }

    [Theory]
    [InlineData("5\btytyt\b", 5)]
    [InlineData("1,2,3,4,5,6,7,8,9,10,11\b12", 78)]
    [InlineData("1\n2,3", 6)]
    [InlineData("//#\n2#5", 7)]
    [InlineData("//[***]\n11\b22***33", 66)]
    [InlineData("//[*][!!][r9r]\n11r9r22*hh\b33!!44", 110)]
    [InlineData("2,,4,rrrr,1001\b6", 12)]
    public void AlternateDelimiter(string sample, long expected)
    {
        MockOptions(extraSeparator: "\b");
        Assert.Equal(expected, operationService.Execute(sample));
    }

    [Theory]
    [InlineData("5,tytyt,-1", 4)]
    [InlineData("1,2,3,4,5,6,7,8,9,10,11,-12", 54)]
    [InlineData("1\n2,-3", 0)]
    [InlineData("//#\n-2#5", 3)]
    [InlineData("//[***]\n-11***22***33", 44)]
    [InlineData("//[*][!!][r9r]\n11r9r22*hh*33!!44,-1", 109)]
    [InlineData("-2,,4,rrrr,1001,6", 8)]
    public void ShouldAllowNegatives(string sample, long expected)
    {
        MockOptions(allowNegatives: true);
        Assert.Equal(expected, operationService.Execute(sample));
    }

    [Theory]
    [InlineData("5,tytyt,-1")]
    [InlineData("1,2,3,4,5,6,7,8,9,10,11,-12")]
    [InlineData("1\n2,-3")]
    [InlineData("//#\n-2#5")]
    [InlineData("//[***]\n-11***22***33")]
    [InlineData("//[*][!!][r9r]\n11r9r22*hh*33!!44,-1")]
    [InlineData("-2,,4,rrrr,1001,6")]
    public void ShouldNotAllowNegatives(string sample)
    {
        MockOptions(allowNegatives: false);
        Assert.Throws<NoNegativesAllowedException>(() => operationService.Execute(sample));
    }

    [Theory]
    [InlineData("2,,4,rrrr,1001,6", 12)]
    public void ShouldIgnoreBiggerThanMaximum(string sample, long expected)
    {
        Assert.Equal(expected, operationService.Execute(sample));
    }

    [Theory]
    [InlineData("2,,4,rrrr,1001,6", 1013)]
    [InlineData("-2,,4,rrrr,1001,6", 1009)]
    public void ShouldAcceptAllNumbers(string sample, long expected)
    {
        MockOptions(allowNegatives:true, maxValue: 2000);
        Assert.Equal(expected, operationService.Execute(sample));
    }
}