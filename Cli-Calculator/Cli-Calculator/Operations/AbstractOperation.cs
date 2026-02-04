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

            Console.Write($"{number} ");

            if (isFirst)
                total = number;
            else
                total = Aggregator(total, number);

            isFirst = false;
        }

        Console.WriteLine($"= {total}");
        return total;
    }
}