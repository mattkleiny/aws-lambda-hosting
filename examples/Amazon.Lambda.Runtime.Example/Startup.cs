using System;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.Lambda.Core;
using Amazon.Lambda.Diagnostics;
using Amazon.Lambda.Hosting;
using Amazon.Lambda.Runtime.Example.Handlers;
using Amazon.Lambda.Runtime.Example.Services;
using Amazon.Lambda.Serialization.Json;
using Amazon.Lambda.Services;
using Amazon.S3;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

[assembly: LambdaSerializer(typeof(JsonSerializer))]

namespace Amazon.Lambda.Runtime.Example
{
  public sealed class Startup
  {
    /// <summary>The <see cref="IHostBuilder"/> for this example.</summary>
    public static IHostBuilder HostBuilder => new HostBuilder()
      .ConfigureAppConfiguration((context, builder) =>
      {
        var environment = context.HostingEnvironment.EnvironmentName;

        builder.AddJsonFile("appsettings.json",               optional: true, reloadOnChange: true);
        builder.AddJsonFile($"appsettings{environment}.json", optional: true, reloadOnChange: true);

        builder.AddEnvironmentVariables();
      })
      .UseStartup<Startup>();

    /// <summary>This is the entry point from the CLI.</summary>
    public static Task<int> Main(string[] args)
      => HostBuilder.RunLambdaConsoleAsync(args);

    /// <summary>This is the entry point from AWS.</summary>
    [UsedImplicitly]
    public static Task<object> ExecuteAsync(object input, ILambdaContext context)
      => HostBuilder.RunLambdaAsync(input, context);

    public Startup(IHostingEnvironment environment, IConfiguration configuration)
    {
      Environment   = environment;
      Configuration = configuration;
    }

    public IHostingEnvironment Environment   { get; }
    public IConfiguration      Configuration { get; }

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
        builder.AddLambdaLogger();
        builder.SetMinimumLevel(Environment.IsDevelopment() ? LogLevel.Trace : LogLevel.Information);
      });

      services.AddDynamo();
      services.AddElastiCache();
      services.AddLambda();
      services.AddS3();
      services.AddSNS();
      services.AddSQS();
      services.AddSSM();
      services.AddStepFunctions();

      services.AddHandler<Handler1>();
      services.AddHandler<Handler2>();
      services.AddFunctionalHandlers<Startup>();

      services.AddScoped<ITestService, TestService>();

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