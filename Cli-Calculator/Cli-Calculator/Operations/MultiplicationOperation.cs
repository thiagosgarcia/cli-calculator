namespace Cli_Calculator.Operations;

public class MultiplicationOperation : AbstractOperation
{
    protected override string Operation => "*";
    protected override long Aggregator(long total, long number) => total * number;
}