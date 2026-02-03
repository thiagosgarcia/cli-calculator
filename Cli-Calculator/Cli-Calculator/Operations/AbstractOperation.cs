namespace Cli_Calculator.Operations;

public abstract class AbstractOperation
{
    protected abstract string Operation { get; }
    protected abstract long Aggregator(long total, long number);
    public virtual long LogAndAggregate(IEnumerable<long> numbers)
    {
        var total = 0L;
        var isFirst = true;
        foreach (var number in numbers)
        {
            if (!isFirst)
                Console.Write($"{Operation} ");
            else
                isFirst = false;
            Console.Write($"{number} ");
            total = Aggregator(total, number);
        }

        Console.WriteLine($"= {total}");
        return total;
    }

}