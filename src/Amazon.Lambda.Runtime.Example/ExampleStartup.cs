using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.Handlers;
using Amazon.Lambda.Hosting;
using Amazon.Lambda.Serialization.Json;
using Amazon.Lambda.Services;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

[assembly: LambdaSerializer(typeof(JsonSerializer))]

namespace Amazon.Lambda
{
  public sealed class ExampleStartup
  {
    /// <summary>The <see cref="IHostBuilder"/> for this example.</summary>
    public static IHostBuilder HostBuilder => new HostBuilder()
      .UseStartup<ExampleStartup>()
      .UseS3()
      .UseDynamo()
      .WithHandler<Handler1>()
      .WithHandler<Handler2>();

    /// <summary>This is the entry point from the CLI.</summary>
    public static Task Main(string[] args)
      => HostBuilder.WithLambdaSwitchboard().RunConsoleAsync();

    /// <summary>This is the entry point from AWS.</summary>
    [UsedImplicitly]
    public static Task<object> ExecuteAsync(object input, ILambdaContext context)
      => HostBuilder.RunLambdaAsync(input, context);

    [UsedImplicitly]
    public void ConfigureServices(IServiceCollection services, IHostingEnvironment environment)
    {
      services.AddScoped<ITestService, TestService>();

      if (environment.IsDevelopment())
      {
        services.ConfigureHostingOptions(options =>
        {
          options.AWS.AccessKey = "A1B2C3D4E5";
          options.AWS.SecretKey = "A1B2C3D4E5";
          
          options.RedirectTable[WellKnownService.S3]     = new Uri("http://localhost:9000");
          options.RedirectTable[WellKnownService.Dynamo] = new Uri("http://localhost:8000");
        });
      }
    }
  }
}