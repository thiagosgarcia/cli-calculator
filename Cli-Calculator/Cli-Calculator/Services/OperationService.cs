using Cli_Calculator.Operations;
using Domain.ExceptionHandling;
using Domain.Models;
using Domain.Services;
using Microsoft.Extensions.Options;

namespace Cli_Calculator.Services;

public class OperationService(
        IOptionsSnapshot<ApplicationOptions> options,
        SumOperation sumOperation
    ) :IOperationService
{
    public void Execute(string? args)
    {
        var numbers = ExtractNumbers(args).ToList();

        var maxNumbersValue = options.Value.MaxNumbers;
        if(maxNumbersValue is not null) //null removes the limit
        {
            var maxNumbers = Math.Max(2, Math.Min(int.MaxValue, (int)maxNumbersValue));
            if (numbers.Count > maxNumbers)
                throw new MaximumNumbersExceededException(maxNumbers, numbers.Count);
        }
        
        _ = PerformOperation(numbers);
    }

    public long PerformOperation(IEnumerable<long> numbers, Operation operation = Operation.Add)
    {
        return operation switch
        {
            Operation.Add => sumOperation.LogAndAggregate(numbers),
            _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
        };
    }
    
    public IEnumerable<long> ExtractNumbers(string? args, string separator = ",")
    {
        if (string.IsNullOrWhiteSpace(args))
        {
            yield return 0;
            yield break;
        }
        
        var parts = args.Split(separator, StringSplitOptions.TrimEntries);
        foreach (var part in parts)
            if (long.TryParse(part, out var number))
                yield return number;
            else
                yield return 0;
    }
}