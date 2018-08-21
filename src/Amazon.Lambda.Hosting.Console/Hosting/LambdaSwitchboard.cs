using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Amazon.Lambda.Hosting
{
  /// <summary>A service which displays a menu of all the attached <see cref="ILambdaHandler"/>s and permits their execution.</summary>
  internal sealed class LambdaSwitchboard : BackgroundService
  {
    private readonly LambdaHandlerMetadata[] metadatas;
    private readonly IHost                   host;

    public LambdaSwitchboard(IEnumerable<LambdaHandlerMetadata> metadatas, IHost host)
    {
      Check.NotNull(metadatas, nameof(metadatas));
      Check.NotNull(host, nameof(host));

      this.metadatas = metadatas.ToArray();
      this.host      = host;
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
      await Task.Delay(100, cancellationToken); // HACK: wait until the rest of the logs have completed

      Console.WriteLine("Welcome to the lambda switchboard.");
      Console.WriteLine("Please make your selection below:");

      for (var index = 0; index < metadatas.Length; index++)
      {
        var metadata = metadatas[index];

        Console.WriteLine($"[{index}] - {metadata.FunctionName} mapped to {metadata.FriendlyName}");
      }

      while (!cancellationToken.IsCancellationRequested)
      {
        // read the id of the handler to execute
        Console.Write("> ");

        if (!int.TryParse(Console.ReadLine(), out var option)) continue;
        if (option < 0 || option >= metadatas.Length) continue;

        var metadata = metadatas[option];

        Console.WriteLine($"Executing {metadata.FriendlyName}");

        // TODO: support various types of input here
        // TODO: support cancellation of long-running invocations here

        var context = LambdaContext.ForFunction(metadata.FunctionName);
        var result  = await host.RunLambdaAsync(null, context, cancellationToken);

        Console.WriteLine(result);
      }
    }
  }
}