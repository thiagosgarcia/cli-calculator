using Domain.Models;

namespace Domain.Services;

public interface IOperationService
{
    long Execute(string? args);
    public (string? cleanArgs, string[] delimiter) ExtractCustomSeparator(string? args);
    long PerformOperation(IEnumerable<long> numbers, Operation operation = Operation.Add);
    IEnumerable<long> ExtractNumbers(string? args, string separator = ",", string[] customSeparator = null);
}