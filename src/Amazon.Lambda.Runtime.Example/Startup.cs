using System;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.Lambda.Core;
using Amazon.Lambda.Hosting;
using Amazon.Lambda.Runtime.Example.Handlers;
using Amazon.Lambda.Runtime.Example.Services;
using Amazon.Lambda.Serialization.Json;
using Amazon.Lambda.Services;
using Amazon.S3;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

[assembly: LambdaSerializer(typeof(JsonSerializer))]

namespace Amazon.Lambda.Runtime.Example
{
  public sealed class Startup
  {
    /// <summary>The <see cref="IHostBuilder"/> for this example.</summary>
    public static IHostBuilder HostBuilder => new HostBuilder()
      .UseStartup<Startup>()
      .UseS3()
      .UseDynamo()
      .WithHandler<Handler1>()
      .WithHandler<Handler2>()
      .WithFunctionalHandlers<Startup>();

    /// <summary>This is the entry point from the CLI.</summary>
    public static Task<int> Main(string[] args)
      => HostBuilder.RunLambdaConsoleAsync(args);

    /// <summary>This is the entry point from AWS.</summary>
    [UsedImplicitly]
    public static Task<object> ExecuteAsync(object input, ILambdaContext context)
      => HostBuilder.RunLambdaAsync(input, context);

    [LambdaFunction("handler-3")]
    public async Task<object> Handler3(object input, ITestService testService)
    {
      return await testService.GetMessageAsync();
    }

    [LambdaFunction("handler-4")]
    public Task<object> Handler4(object input, IAmazonS3 s3, IAmazonDynamoDB dynamo, ITestService testService, ILambdaContext context)
    {
      return Task.FromResult<object>("Hello from Handler 4");
    }

    [UsedImplicitly]
    public void ConfigureServices(IServiceCollection services, IHostingEnvironment environment)
    {
      services.AddScoped<ITestService, TestService>();

      if (environment.IsDevelopment())
      {
        services.ConfigureHostingOptions(options =>
        {
          options.MatchingStrategy = MatchingStrategies.MatchByNameSuffix(StringComparison.OrdinalIgnoreCase);

          options.AWS.AccessKey = "A1B2C3D4E5";
          options.AWS.SecretKey = "A1B2C3D4E5";

          options.RedirectTable[WellKnownService.S3]     = new Uri("http://localhost:9000");
          options.RedirectTable[WellKnownService.Dynamo] = new Uri("http://localhost:8000");
        });
      }
    }
  }
}