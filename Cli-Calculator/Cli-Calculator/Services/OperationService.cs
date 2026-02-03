using Cli_Calculator.Operations;
using Domain.ExceptionHandling;
using Domain.Models;
using Domain.Services;
using Microsoft.Extensions.Options;

namespace Cli_Calculator.Services;

public class OperationService(
    IOptionsSnapshot<ApplicationOptions> options,
    SumOperation sumOperation
) : IOperationService
{
    public void Execute(string? args)
    {
        var numbers = ExtractNumbers(args).ToList();

        ValidateNegatives(numbers);
        ValidateMaximumNumberCount(numbers);

        _ = PerformOperation(numbers);
    }

    private void ValidateNegatives(List<long> numbers)
    {
        var negatives = numbers.Where(n => n < 0).ToArray();
        if (negatives.Length != 0)
            throw new NoNegativesAllowedException(negatives);
    }

    private void ValidateMaximumNumberCount(List<long> numbers)
    {
        var maxNumbersValue = options.Value.MaxNumbers;
        if (maxNumbersValue is not null) //null removes the limit
        {
            var maxNumbers = Math.Max(2, Math.Min(int.MaxValue, (int)maxNumbersValue));
            if (numbers.Count > maxNumbers)
                throw new MaximumNumbersExceededException(maxNumbers, numbers.Count);
        }
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

        var parts = args.Split(separator);
        foreach (var part in parts)
        {
            var newLine = "\n";
            if (part.Contains(newLine))
            {
                var inner = ExtractNumbers(part, newLine);
                foreach (var innerNumber in inner)
                    yield return innerNumber;
                continue;
            }
            
            if (long.TryParse(part, out var number))
                yield return ValidateBounds(number);
            else
                yield return 0;
        }
    }

    private long ValidateBounds(long param) => param > 1000 ? 0 : param;
}