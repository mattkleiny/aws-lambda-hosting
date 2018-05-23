using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.Handlers;
using Amazon.Lambda.Hosting;
using Amazon.Lambda.Serialization.Json;
using Amazon.Lambda.Services;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

[assembly: LambdaSerializer(typeof(JsonSerializer))]

namespace Amazon.Lambda
{
  public sealed class ExampleStartup
  {
    /// <summary>This is the entry point from the CLI.</summary>
    public static async Task Main(string[] args)
    {
      var execute = await ExecuteAsync(null, new LocalLambdaContext("lambda-runtime-example-handler-1"));
      
      Console.WriteLine(execute);
    }
    
    /// <summary>This is the entry point from AWS.</summary>
    [UsedImplicitly]
    public static Task<object> ExecuteAsync(object input, ILambdaContext context) => new LambdaHostBuilder()
      .UseStartup<ExampleStartup>()
      .WithHandler<Handler1>()
      .WithHandler<Handler2>()
      .ExecuteAsync(input, context);

    [UsedImplicitly]
    public void ConfigureServices(IServiceCollection services, IHostingEnvironment environment)
    {
      services.AddScoped<ITestService, TestService>();

      if (environment.IsDevelopment())
      {
        services.ConfigureHostingOptions(options =>
        {
          options.RedirectTable[WellKnownService.Dynamo] = new Uri("http://localhost:8000");
        });
      }
    }

    [UsedImplicitly]
    public void Configure(IHostBuilder builder, IHostingEnvironment environment, IConfiguration configuration)
    {
    }
  }
}