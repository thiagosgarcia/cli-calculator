using Cli_Calculator.Operations;
using Cli_Calculator.Services;
using Domain.Models;
using Microsoft.Extensions.Options;
using Moq;

namespace CliCalculator;

public class IntegrationTests
{
    private readonly OperationService operationService;
    private readonly Mock<IOptionsSnapshot<ApplicationOptions>> optionsMock;

    public IntegrationTests()
    {
        
        var defaultOptions = new ApplicationOptions
        {
            ExitOnError = false,
            MaxNumbers = null
        };
        optionsMock = new Mock<IOptionsSnapshot<ApplicationOptions>>();
        optionsMock.Setup(o => o.Value).Returns(defaultOptions);
        operationService = new OperationService(optionsMock.Object, new SumOperation());
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
}