// See https://aka.ms/new-console-template for more information

using Cli_Calculator.Operations;
using Cli_Calculator.Services;
using Domain.Extensions;
using Domain.Models;
using Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var globalCts = new CancellationTokenSource();

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

var parameters = new ConfigurationBuilder()
    .AddInMemoryCollection(args.ParseParameters()!)
    .Build();

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
        {
            services.AddSingleton(globalCts);
            services.AddTransient<SumOperation>();
            services.AddTransient<IRunnerService, RunnerService>();
            services.AddTransient<IOperationService, OperationService>();
            services.Configure<ApplicationOptions>(configuration);
            services.Configure<ApplicationParameters>(parameters);
        }
    ).Build();

Console.CancelKeyPress += (_, eventArgs) =>
{
    eventArgs.Cancel = true;
    globalCts.Cancel();
};

Console.WriteLine("Welcome to CLI-Calculator!\n");

var runner = host.Services.GetRequiredService<IRunnerService>();
runner.Run();

if (globalCts.IsCancellationRequested)
    Console.Write("Cancellation signal received. ");

Console.WriteLine("Exiting application...");
globalCts.Cancel();

return;