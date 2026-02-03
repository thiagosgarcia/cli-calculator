using Cli_Calculator.Operations;
using Cli_Calculator.Services;
using Domain.ExceptionHandling;
using Domain.Models;
using Microsoft.Extensions.Options;
using Moq;

namespace CliCalculator;

public class OperationServiceTests
{
    private OperationService operationService;
    private Mock<IOptionsSnapshot<ApplicationOptions>> optionsMock;
    private readonly Mock<SumOperation> sumMock;

    public OperationServiceTests()
    {
        sumMock = new Mock<SumOperation>();
        MockOptions();
    }

    private void MockOptions(int? max = 2)
    {
        var defaultOptions = new ApplicationOptions
        {
            ExitOnError = false,
            MaxNumbers = max
        };
        optionsMock = new Mock<IOptionsSnapshot<ApplicationOptions>>();
        optionsMock.Setup(o => o.Value).Returns(defaultOptions);

        operationService = new OperationService(optionsMock.Object, sumMock.Object);
    }

    [Theory]
    [InlineData("1,2,3", new long[] { 1, 2, 3 })]
    [InlineData("10,20,30,40 , 1000, 1001", new long[] { 10, 20, 30, 40, 1000, 0 })]
    [InlineData("100", new long[] { 100 })]
    [InlineData("", new long[] { 0 })]
    [InlineData(null, new long[] { 0 })]
    [InlineData("1, 2, abc, 34j, 0 , 2 , ^%^2, &^ , , ",
        new long[] { 1, 2, 0, 0, 0, 2, 0, 0, 0, 0 })] //Empty entries should be considered zero
    public void ShouldExtractNumber(string? input, long[] expected)
    {
        var result = operationService.ExtractNumbers(input).ToArray();
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("1\n2,3", new long[] { 1, 2, 3 })]
    [InlineData("1\n2\n 3", new long[] { 1, 2, 3 })]
    [InlineData("10,20,30\n40", new long[] { 10, 20, 30, 40 })]
    [InlineData("10,20,30\n40, 1000, 1001", new long[] { 10, 20, 30, 40, 1000, 0 })]
    [InlineData("1, 2, abc\n 34j, 0 , 2 , ^%^2, &^ \n , ",
        new long[] { 1, 2, 0, 0, 0, 2, 0, 0, 0, 0 })] //Empty entries should be considered zero
    public void ShouldExtractNumberWithAlternativeDelimiter(string? input, long[] expected)
    {
        var result = operationService.ExtractNumbers(input).ToArray();
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("p", "1p2,3", new long[] { 1, 2, 3 })]
    [InlineData("#", "10,20#30,40 , 1000, 1001", new long[] { 10, 20, 30, 40, 1000, 0 })]
    [InlineData("***", "10,20***30,40 , 1000, 1001", new long[] { 10, 20, 30, 40, 1000, 0 })]
    [InlineData("#","100", new long[] { 100 })]
    [InlineData("#","", new long[] { 0 })]
    [InlineData("#",null, new long[] { 0 })]
    [InlineData("$","1, 2$ abc$ 34j, 0 , 2 , ^%^2, &^ , , ",
        new long[] { 1, 2, 0, 0, 0, 2, 0, 0, 0, 0 })] //Empty entries should be considered zero
    public void ShouldExtractNumberWithCustomDelimiter(string? custom, string? input, long[] expected)
    {
        var result = operationService.ExtractNumbers(input, customSeparator:custom).ToArray();
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("a","1a2\n3", new long[] { 1, 2, 3 })]
    [InlineData("abc","1abc2\n3", new long[] { 1, 2, 3 })]
    [InlineData("b","1\n2b 3", new long[] { 1, 2, 3 })]
    [InlineData("#","10,20,30\n40", new long[] { 10, 20, 30, 40 })]
    [InlineData("#*#","10,20#*#30\n40", new long[] { 10, 20, 30, 40 })]
    [InlineData("#","10,20,30\n40, 1000, 1001", new long[] { 10, 20, 30, 40, 1000, 0 })]
    [InlineData("#","1, 2, abc\n 34j, 0 , 2 , ^%^2, &^ \n , ",
        new long[] { 1, 2, 0, 0, 0, 2, 0, 0, 0, 0 })] //Empty entries should be considered zero
    public void ShouldExtractNumberWithAlternativeAndCustomDelimiter(string? custom, string? input, long[] expected)
    {
        var result = operationService.ExtractNumbers(input, customSeparator: custom).ToArray();
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(Operation.Add)]
    public void ShouldCallCorrectOperation(Operation operation)
    {
        //*Extracted operations to abstract class
        _ = operationService.PerformOperation([], operation);
        sumMock.Verify(x => x.LogAndAggregate(new long[] { }), Times.Once);
        sumMock.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData("1,2,3")]
    [InlineData("10,20,30,40")]
    [InlineData("1, 2, abc, 34j, 0 , 2 , ^%^2, &^ , , ")]
    [InlineData("//#,1#2,3")]
    [InlineData("/[***]\n1***2,3")]
    [InlineData("//[***]***1***2***3")]
    public void ShouldLimitNumbersByDefaultMax(string args)
    {
        Assert.Throws<MaximumNumbersExceededException>(() => operationService.Execute(args));
        optionsMock.Verify(x => x.Value, Times.Once);
        optionsMock.VerifyNoOtherCalls();
        sumMock.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData("1,2,-3", "-3")]
    [InlineData("-10,-20,-30,-40", "-10, -20, -30, -40")]
    [InlineData("1, -2, abc, 34j, 0 , 2 , ^%^2, &^ , , ", "-2")]
    [InlineData("//#,-1#2,3", "-1")]
    [InlineData("//#\n-1#2,3", "-1")]
    [InlineData("//[***],-1***2,3", "-1")]
    [InlineData("//[***]\n-1***2,3", "-1")]
    public void ShouldDenyNegatives(string args, string negatives)
    {
        var msg = string.Empty;
        Assert.Throws<NoNegativesAllowedException>(() =>
        {
            try
            {
                operationService.Execute(args);
            }
            catch (NoNegativesAllowedException ex)
            {
                msg = ex.Message;
                throw;
            }
        });
        Assert.Contains(negatives, msg);
        optionsMock.VerifyNoOtherCalls();
        sumMock.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData("1,2,3")]
    [InlineData("10,20,30,40")]
    [InlineData("1, 2, abc, 34j, 0 , 2 , ^%^2, &^ , , ")]
    public void ShouldNotLimitNumbers(string args)
    {
        MockOptions(null);
        operationService.Execute(args);
        optionsMock.Verify(x => x.Value, Times.Once);
        optionsMock.VerifyNoOtherCalls();
        sumMock.Verify(x => x.LogAndAggregate(It.IsAny<IEnumerable<long>>()), Times.Once);
        sumMock.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData("1,2")]
    [InlineData("10,20")]
    [InlineData("1")]
    [InlineData("")]
    [InlineData(null)]
    public void ShouldPerformOperation(string args)
    {
        operationService.Execute(args);
        optionsMock.Verify(x => x.Value, Times.Once);
        optionsMock.VerifyNoOtherCalls();
        sumMock.Verify(x => x.LogAndAggregate(It.IsAny<IEnumerable<long>>()), Times.Once);
        sumMock.VerifyNoOtherCalls();
    }
    
    [Theory]
    [InlineData("//#,-1#2,3", "#")]
    [InlineData("//#\n-1#2,3", "#")]
    [InlineData("//a,1a2\n3", "a")]
    [InlineData("//b\n1\n2b 3", "b")]
    [InlineData("//#,10,20,30\n40", "#")]
    [InlineData("//##10,20,30\n40", "#")]
    [InlineData("//[###]###10###20,30\n40", "###")]
    public void ExtractCustomSeparator(string args, string separator)
    {
        var (cleanArgs, delimiter) = operationService.ExtractCustomSeparator(args);
        Assert.NotNull(delimiter);
        Assert.NotEqual(args, cleanArgs);
        var prefix = separator.Length > 1 ? "[" : "";
        Assert.Equal(args.Replace($"//{prefix}{separator}", "").Substring(1), cleanArgs);
        Assert.Equal(delimiter, separator);
    }
}