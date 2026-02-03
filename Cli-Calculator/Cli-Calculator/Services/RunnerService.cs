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
                Console.WriteLine("Enter the comma separated numbers to be added:");
                var str = Console.ReadLine();
                operationService.Execute(str);
                if (str is null)
                    break;
            }
            catch (MaximumNumbersExceededException ex)
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