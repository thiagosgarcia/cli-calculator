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
        (args, var customSeparator) = ExtractCustomSeparator(args);
        var numbers = ExtractNumbers(args, customSeparator: customSeparator).ToList();

        ValidateNegatives(numbers);
        ValidateMaximumNumberCount(numbers);

        _ = PerformOperation(numbers);
    }

    public (string? cleanArgs, string? delimiter) ExtractCustomSeparator(string? args)
    {
        if (string.IsNullOrWhiteSpace(args))
            return (args, null);

        const string delimiterPrefix = "//";
        if (args.StartsWith(delimiterPrefix))
        {
            var customPrefix = args.Substring(delimiterPrefix.Length, 1);
            var cleanArgs = args.Substring(delimiterPrefix.Length + 2);
            return (cleanArgs, customPrefix);
        }

        return (args, null);
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

    public IEnumerable<long> ExtractNumbers(string? args, string separator = ",", string? customSeparator = null)
    {
        if (string.IsNullOrWhiteSpace(args))
        {
            yield return 0;
            yield break;
        }

        var parts = args.Split(separator);
        foreach (var part in parts)
        {
            var customSeparators = new[] {"\n"};
            
            if(customSeparator is not null)
                customSeparators = customSeparators.Append(customSeparator).ToArray();

            var partIsProcessed = false;
            foreach (var cs in customSeparators)
            {
                if (part.Contains(cs))
                {
                    //The definition says 1 char-long separator, so with this recursion we can support any pattern
                    var inner = ExtractNumbers(part, cs, customSeparator);
                    foreach (var innerNumber in inner)
                        yield return innerNumber;
                    partIsProcessed = true;
                    break;
                }
            }
            if(partIsProcessed)
                continue;
            
            if (long.TryParse(part, out var number))
                yield return ValidateBounds(number);
            else
                yield return 0;
        }
    }

    private long ValidateBounds(long param) => param > 1000 ? 0 : param;
}