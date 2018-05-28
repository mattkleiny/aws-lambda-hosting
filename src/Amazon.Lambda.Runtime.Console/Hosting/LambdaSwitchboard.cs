using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Amazon.Lambda.Hosting
{
  /// <summary>A service which displays a menu of all the attached <see cref="ILambdaHandler"/>s and permits their execution.</summary>
  internal sealed class LambdaSwitchboard : BackgroundService
  {
    private readonly LambdaHandlerRegistration[] registrations;

    private readonly IHost host;

    public LambdaSwitchboard(IEnumerable<LambdaHandlerRegistration> registrations, IHost host)
    {
      Check.NotNull(registrations, nameof(registrations));
      Check.NotNull(host,          nameof(host));

      this.registrations = registrations.ToArray();
      this.host          = host;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
      return Task.Factory.StartNew(
        () => RunSwitchboardAsync(stoppingToken),
        stoppingToken,
        TaskCreationOptions.LongRunning,
        TaskScheduler.Default
      );
    }

    /// <summary>Builds the main <see cref="Thread"/> for executing the switchboard.</summary>
    private async Task RunSwitchboardAsync(CancellationToken cancellationToken = default)
    {
      Thread.Sleep(100); // HACK: wait until the rest of the logs have completed

      Console.WriteLine("Welcome to the lambda switchboard.");
      Console.WriteLine("Please make your selection below:");

      for (var index = 0; index < registrations.Length; index++)
      {
        var registration = registrations[index];

        Console.WriteLine($"[{index}] - {registration.FunctionName} mapped to {registration.FriendlyName}");
      }

      while (!cancellationToken.IsCancellationRequested)
      {
        // read the id of the handler to execute
        Console.Write("> ");

        if (!int.TryParse(Console.ReadLine(), out var option)) continue;
        if (option < 0 || option >= registrations.Length) continue;

        var registration = registrations[option];

        Console.WriteLine($"Executing {registration.FriendlyName}");

        // TODO: support various types of input here
        // TODO: support cancellation of long-running invocations here

        var context = new LocalLambdaContext(registration.FunctionName);
        var result  = await host.RunLambdaAsync(null, context, cancellationToken);

        Console.WriteLine(result);
      }
    }
  }
}