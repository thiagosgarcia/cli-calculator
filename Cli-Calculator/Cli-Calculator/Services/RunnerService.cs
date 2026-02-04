using Domain.ExceptionHandling;
using Domain.Models;
using Domain.Services;
using Microsoft.Extensions.Options;

namespace Cli_Calculator.Services;

public class RunnerService(
    IOptionsSnapshot<ApplicationOptions> options,
    CancellationTokenSource cts,
    IOperationService operationService
) : IRunnerService, IDisposable
{
    public void Run()
    {
        while (!cts.IsCancellationRequested)
        {
            try
            {
                Console.WriteLine(
                    "Enter the operation to be performed (+ - * /) or press Enter to add numbers (Invalid entries will default to addition):");
                var opInput = Console.ReadLine();
                var operation = opInput switch
                {
                    "+" => Operation.Add,
                    "-" => Operation.Subtract,
                    "*" => Operation.Multiply,
                    "/" => Operation.Divide,
                    _ => Operation.Add
                };


                Console.WriteLine($"Enter the comma separated numbers to perform operation {operation}:");
                var str = Console.ReadLine();
                _ = operationService.Execute(str, operation);
                if (str is null)
                    break;
            }
            catch (MaximumNumbersExceededException ex)
            {
                Console.WriteLine(ex.Message);
                if (options.Value.ExitOnError)
                    break;
            }
            catch (NoNegativesAllowedException ex)
            {
                Console.WriteLine(ex.Message);
                if (options.Value.ExitOnError)
                    break;
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine(ex.Message);
                break;
            }
        }
    }

    public void Dispose()
    {
        cts.CancelAfter(TimeSpan.FromSeconds(1));
    }
}