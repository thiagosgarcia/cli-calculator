namespace Cli_Calculator.Operations;

public class DivisionOperation : AbstractOperation
{
    protected override string Operation => "/";
    protected override long Aggregator(long total, long number)
    {
        if(number == 0)
            throw new DivideByZeroException("Cannot divide by zero.");
        return total / number;
    }
}