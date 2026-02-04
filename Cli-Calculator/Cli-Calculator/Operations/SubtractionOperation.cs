namespace Cli_Calculator.Operations;

public class SubtractionOperation : AbstractOperation
{
    protected override string Operation => "-";
    protected override long Aggregator(long total, long number) => total - number;
}