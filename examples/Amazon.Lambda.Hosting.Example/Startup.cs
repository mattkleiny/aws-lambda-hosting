using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.Json;
using Amazon.Lambda.Services;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

[assembly: LambdaSerializer(typeof(JsonSerializer))]

namespace Amazon.Lambda.Hosting.Example
{
  public sealed class Startup
  {
    public static IHostBuilder HostBuilder { get; } = new HostBuilder()
      .ConfigureAppConfiguration((context, builder) =>
      {
        var environment = context.HostingEnvironment.EnvironmentName;

        builder.AddJsonFile("appsettings.json",               optional: true, reloadOnChange: true);
        builder.AddJsonFile($"appsettings{environment}.json", optional: true, reloadOnChange: true);

        builder.AddEnvironmentVariables();
      })
      .UseStartup<Startup>();

    [UsedImplicitly]
    public static Task<object?> ExecuteAsync(object input, ILambdaContext context)
    {
      return HostBuilder.ExecuteLambdaAsync(input, context);
    }

    public Startup(IHostEnvironment environment, IConfiguration configuration)
    {
      Environment   = environment;
      Configuration = configuration;
    }

    public IHostEnvironment Environment   { get; }
    public IConfiguration   Configuration { get; }

    [LambdaFunction("handler-4")]
    public Task<object> Handler4(object input, ILambdaContext context)
    {
      return Task.FromResult<object>("Hello from Handler 4");
    }

    [LambdaFunction("handler-5")]
    public static Task<string> Handler5(ILambdaContext context)
    {
      return Task.FromResult($"Hello, from {context.FunctionName}!");
    }

    [UsedImplicitly]
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddLogging(builder =>
      {
        var logLevel = Environment.IsDevelopment()
          ? LogLevel.Trace
          : LogLevel.Information;

        builder.SetMinimumLevel(logLevel);
      });

      services.AddFunctionalHandlers<Startup>();

      services.ConfigureHostingOptions(options =>
      {
        if (Environment.IsDevelopment())
        {
          options.AWS.AccessKey = "A1B2C3D4E5";
          options.AWS.SecretKey = "A1B2C3D4E5";

          options.RedirectTable[WellKnownService.S3]     = new Uri("http://localhost:9000");
          options.RedirectTable[WellKnownService.Dynamo] = new Uri("http://localhost:8000");
        }
      });
    }
  }
}