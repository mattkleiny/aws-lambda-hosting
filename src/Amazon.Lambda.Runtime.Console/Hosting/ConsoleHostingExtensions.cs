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
    /// <summary>Adds a service which displays a menu of all the attached lambda handlers and permits their execution.</summary>
    public static IHostBuilder WithLambdaSwitchboard(this IHostBuilder builder)
    {
      return builder.ConfigureServices(services => services.AddSingleton<IHostedService, LambdaSwitchboard>());
    }

    /// <summary>Runs a the lambda host application with the given command line arguments.</summary>
    public static Task<int> RunLambdaConsoleAsync(this IHostBuilder builder, string[] args, CancellationToken token = default)
    {
      Check.NotNull(args, nameof(args));

      var application = new CommandLineApplication();
      application.HelpOption();

      var functionOption = application.Option<string>("-f|--function <FUNCTION>", "The name of the function to execute", CommandOptionType.SingleValue);
      var inputOption    = application.Option("-i|--input <INPUT>", "The input to pass to the function", CommandOptionType.SingleOrNoValue);

      application.OnExecute(async () =>
      {
        var function = functionOption.Value();
        var input    = inputOption.Value();

        await builder.RunLambdaAsync(input, new LocalLambdaContext(function), token);
      });

      return Task.FromResult(application.Execute(args));
    }
  }
}