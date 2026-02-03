using Domain.Models;

namespace Domain.Services;

public interface IOperationService
{
    void Execute(string? args);
    long PerformOperation(IEnumerable<long> numbers, Operation operation = Operation.Add);
    IEnumerable<long> ExtractNumbers(string? args, string separator = ",");
}