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

[assembly: LambdaSerializer(typeof(JsonSerializer))]

namespace Amazon.Lambda
{
  public sealed class ExampleStartup
  {
    [UsedImplicitly]
    public static Task<object> Execute(object input, ILambdaContext context) => new LambdaHostBuilder()
      .UseStartup<ExampleStartup>()
      .WithHandler<Handler1>()
      .WithHandler<Handler2>()
      .Build()
      .ExecuteAsync(input, context);

    [UsedImplicitly]
    public void ConfigureServices(IServiceCollection services, IHostingEnvironment environment)
    {
      services.AddScoped<ITestService, TestService>();

      if (environment.IsDevelopment())
      {
        services.ConfigureHostingOptions(options =>
        {
          options.AddRedirectTable(new RedirectTable
          {
            [WellKnownService.DynamoDB] = new Uri("http://localhost:8000"),
          });
        });
      }
    }

    [UsedImplicitly]
    public void Configure(ILambdaHostBuilder builder, IHostingEnvironment environment, IConfiguration configuration)
    {
    }
  }
}