using Cli_Calculator.Operations;
using Cli_Calculator.Services;

namespace CliCalculator;

public class SumOperationTests
{
    private readonly SumOperation _service = new();

    [Theory]
    [InlineData(new long[] { }, "= 0")]
    [InlineData(new long[] { 0 }, "0 = 0")]
    [InlineData(new long[] { 1, 2, 3 }, "1 + 2 + 3 = 6")]
    [InlineData(new long[] { 1, 0, 3 }, "1 + 0 + 3 = 4")]
    [InlineData(new long[] { 1, 0, -3 }, "1 + 0 + -3 = -2")]
    [InlineData(new long[] { 10, 20, 30, 40 }, "10 + 20 + 30 + 40 = 100")]
    [InlineData(new long[] { 10, 20, 30, -40 }, "10 + 20 + 30 + -40 = 20")]
    public void ShouldLogAndSum(long[] numbers, string? expected)
    {
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);
        _ = _service.LogAndAggregate(numbers);

        var output = stringWriter.ToString().Trim();
        Assert.Equal(expected, output);
    }
}