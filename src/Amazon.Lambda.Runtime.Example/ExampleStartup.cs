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
    public static void Main(string[] args)
    {
      Execute(null, null);
    }
    
    [UsedImplicitly]
    public static Task<object> Execute(object input, ILambdaContext context) => new LambdaHostBuilder()
      .UseStartup<ExampleStartup>()
      .WithHandler<Handler1>()
      .WithHandler<Handler2>()
      .BuildLambdaHost()
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
            [WellKnownService.Dynamo] = new Uri("http://localhost:8000")
          });
        });
      }
    }

    [UsedImplicitly]
    public void Configure(IHostBuilder builder, IHostingEnvironment environment, IConfiguration configuration)
    {
    }
  }
}