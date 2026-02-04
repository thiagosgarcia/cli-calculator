using Cli_Calculator.Operations;

namespace CliCalculator;

public class MultiplicationOperationTests
{
    private readonly MultiplicationOperation _service = new();

    [Theory]
    [InlineData(new long[] { }, "= 0")]
    [InlineData(new long[] { 0 }, "0 = 0")]
    [InlineData(new long[] { 1, 2, 3 }, "1 * 2 * 3 = 6")]
    [InlineData(new long[] { 1, 0, 3 }, "1 * 0 * 3 = 0")]
    [InlineData(new long[] { 1, 0, -3 }, "1 * 0 * -3 = 0")]
    [InlineData(new long[] { 10, 5, 2 }, "10 * 5 * 2 = 100")]
    [InlineData(new long[] { 10, -2, 3 }, "10 * -2 * 3 = -60")]
    public void ShouldLogAndMultiply(long[] numbers, string? expected)
    {
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);
        _ = _service.LogAndAggregate(numbers);

        var output = stringWriter.ToString().Trim();
        Assert.Equal(expected, output);
    }
}