using Cli_Calculator.Operations;

namespace CliCalculator;

public class SubtractionOperationTests
{
    private readonly SubtractionOperation _service = new();

    [Theory]
    [InlineData(new long[] { }, "= 0")]
    [InlineData(new long[] { 0 }, "0 = 0")]
    [InlineData(new long[] { 1, 2, 3 }, "1 - 2 - 3 = -4")]
    [InlineData(new long[] { 1, 0, 3 }, "1 - 0 - 3 = -2")]
    [InlineData(new long[] { 1, 0, -3 }, "1 - 0 - -3 = 4")]
    [InlineData(new long[] { 10, 5, 2 }, "10 - 5 - 2 = 3")]
    [InlineData(new long[] { 10, 20, 30, 40 }, "10 - 20 - 30 - 40 = -80")]
    [InlineData(new long[] { 10, 20, 30, -40 }, "10 - 20 - 30 - -40 = 0")]
    public void ShouldLogAndSubtract(long[] numbers, string? expected)
    {
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);
        _ = _service.LogAndAggregate(numbers);

        var output = stringWriter.ToString().Trim();
        Assert.Equal(expected, output);
    }
}