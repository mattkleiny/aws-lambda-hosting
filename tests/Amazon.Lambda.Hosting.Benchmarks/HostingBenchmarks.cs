using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.Diagnostics;
using Amazon.Lambda.Hosting;
using Amazon.Lambda.Services;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;
using BenchmarkDotNet.Running;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Amazon.Lambda.Hosting.Benchmarks
{
  /// <summary>Benchmarks for hosting overhead caused by this model of lambda design.</summary>
  [CoreJob]
  public class HostingBenchmarks
  {
    public static void Main(string[] args)
    {
      BenchmarkRunner.Run<HostingBenchmarks>();
    }

    [Benchmark]
    public async Task WithTransientHostAsync()
    {
      var context = new LocalLambdaContext("handler-1");

      await Startup.HostBuilder.RunLambdaAsync("hello, world!", context);
    }

    [Benchmark]
    public async Task WithCachedHostAsync()
    {
      var context = new LocalLambdaContext("handler-1");

      await Startup.Host.RunLambdaAsync("hello, world!", context);
    }

    [Benchmark]
    public async Task WithoutHostAsync()
    {
      var context = new LocalLambdaContext("handler-1");
      var startup = new Startup();

      await startup.Handler1("hello, world!", context);
    }

    private sealed class Startup
    {
      public static IHostBuilder HostBuilder => new HostBuilder().UseStartup<Startup>();
      public static IHost        Host        { get; } = HostBuilder.Build();

      [LambdaFunction("handler-1")]
      public Task Handler1(string input, ILambdaContext context)
      {
        return Task.FromResult(input.ToUpper());
      }

      [UsedImplicitly]
      public void ConfigureServices(IServiceCollection services, IHostingEnvironment environment)
      {
        services.AddLogging(builder =>
        {
          builder.AddLambdaLogger();
          builder.SetMinimumLevel(environment.IsDevelopment() ? LogLevel.Trace : LogLevel.Information);
        });

        services.AddFunctionalHandlers<Startup>();

        services.ConfigureHostingOptions(options =>
        {
          if (environment.IsDevelopment())
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
}