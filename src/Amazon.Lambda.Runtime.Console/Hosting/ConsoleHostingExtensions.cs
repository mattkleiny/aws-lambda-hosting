using System;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Amazon.Lambda.Hosting
{
  /// <summary>Extensions for hosting runtime-based applications in the CLI.</summary>
  public static class ConsoleHostingExtensions
  {
    /// <summary>Runs a the lambda host application with the given command line arguments.</summary>
    public static Task<int> RunLambdaConsoleAsync(this IHostBuilder builder, string[] args, CancellationToken cancellationToken = default)
    {
      Check.NotNull(args, nameof(args));

      var application = new CommandLineApplication();

      application.HelpOption();

      application.Command("switchboard", command =>
      {
        command.OnExecute(async () =>
        {
          builder.WithLambdaSwitchboard();

          await builder.RunConsoleAsync(cancellationToken);
        });
      });

      application.Command("execute", command =>
      {
        command.HelpOption();

        var functionOption = command.Argument<string>("function", "The name of the function to execute").IsRequired();
        var inputOption    = command.Option("-i|--input <INPUT>", "The input to pass to the function", CommandOptionType.SingleOrNoValue);

        command.OnExecute(async () =>
        {
          var function = functionOption.Value;
          var input    = inputOption.Value();

          // TODO: pre-process the input into the expected format

          var context = new LocalLambdaContext(function);
          var output  = await builder.RunLambdaAsync(input, context, cancellationToken);

          if (output != null)
          {
            Console.WriteLine(output);
          }
        });
      });

      return Task.FromResult(application.Execute(args));
    }

    /// <summary>Adds a service which displays a menu of all the attached lambda handlers and permits their execution.</summary>
    public static IHostBuilder WithLambdaSwitchboard(this IHostBuilder builder)
    {
      return builder.ConfigureServices(services => services.AddSingleton<IHostedService, LambdaSwitchboard>());
    }
  }
}