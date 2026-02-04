using Cli_Calculator.Operations;

namespace CliCalculator;

public class DivisionOperationTests
{
    private readonly DivisionOperation _service = new();

    [Theory]
    [InlineData(new long[] { }, "= 0")]
    [InlineData(new long[] { 1 }, "1 = 1")]
    [InlineData(new long[] { 8, 2, 2 }, "8 / 2 / 2 = 2")]
    [InlineData(new long[] { 10, 5, 2 }, "10 / 5 / 2 = 1")]
    [InlineData(new long[] { -20, 2, -2 }, "-20 / 2 / -2 = 5")]
    public void ShouldLogAndDivide(long[] numbers, string? expected)
    {
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);
        _ = _service.LogAndAggregate(numbers);

        var output = stringWriter.ToString().Trim();
        Assert.Equal(expected, output);
    }

    [Theory]
    [InlineData(new long[] { 0, 0 })]
    [InlineData(new long[] { 1, 0 })]
    public void ShouldNotDivideByZero(long[] numbers)
    {
        Assert.Throws<DivideByZeroException>(() => _service.LogAndAggregate(numbers));
    }
}