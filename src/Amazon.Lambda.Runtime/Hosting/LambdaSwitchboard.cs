using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Hosting;

namespace Amazon.Lambda.Hosting
{
  /// <summary>A service which displays a menu of all the attached <see cref="ILambdaHandler"/>s and permits their execution.</summary>
  internal sealed class LambdaSwitchboard : IHostedService
  {
    private readonly LambdaHandlerRegistration[] registrations;

    private readonly IHost    host;
    private readonly string[] cliArgs;

    private readonly Thread                 thread;
    private readonly CommandLineApplication app;

    public LambdaSwitchboard(IEnumerable<LambdaHandlerRegistration> registrations, IHost host, string[] cliArgs)
    {
      Check.NotNull(registrations, nameof(registrations));
      Check.NotNull(host,          nameof(host));
      Check.NotNull(cliArgs,       nameof(cliArgs));

      this.registrations = registrations.ToArray();
      this.host          = host;
      this.cliArgs       = cliArgs;

      app    = BuildApplication();
      thread = BuildThread();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
      // TODO: run the CLI app here, instead?
      
      thread.Start();

      return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
      // we're on a background thread which should terminate gracefully
      return Task.CompletedTask;
    }

    /// <summary>Builds the <see cref="CommandLineApplication"/> for executing tasks directly off the CLI commands.</summary>
    private static CommandLineApplication BuildApplication() => new CommandLineApplication()
      .Command("execute", command =>
      {
        command.Description = "Executes a handler directly";

        command.OnExecute(() => { return -1; });
      });

    /// <summary>Builds the main <see cref="Thread"/> for executing the switchboard.</summary>
    private Thread BuildThread() => new Thread(() =>
    {
      Thread.Sleep(100); // HACK: wait until the rest of the logs have completed

      Console.WriteLine("Welcome to the lambda switchboard.");
      Console.WriteLine("Please make your selection below:");

      for (var index = 0; index < registrations.Length; index++)
      {
        var registration = registrations[index];

        Console.WriteLine($"[{index}] - {registration.FunctionName} mapped to {registration.FriendlyName}");
      }

      while (true)
      {
        // read the id of the handler to execute
        Console.Write("> ");

        if (!int.TryParse(Console.ReadLine(), out var option)) continue;
        if (option < 0 || option >= registrations.Length) continue;

        var registration = registrations[option];

        Console.WriteLine($"Executing {registration.FriendlyName}");

        // TODO: support various types of input here
        var result = host.RunLambdaAsync(null, new LocalLambdaContext(registration.FunctionName)).Result;

        Console.WriteLine(result);
      }
    }) {IsBackground = true};
  }
}