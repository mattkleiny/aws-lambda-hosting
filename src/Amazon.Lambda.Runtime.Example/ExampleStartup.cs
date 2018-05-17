﻿using System;
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
    public static async Task Main(string[] args)
    {
      await Execute(null, null);
    }

    [UsedImplicitly]
    public static Task<object> Execute(object input, ILambdaContext context)
    {
      return new LambdaHostBuilder()
        .UseStartup<ExampleStartup>()
        .WithHandler<Handler1>()
        .WithHandler<Handler2>()
        .Build()
        .ExecuteAsync(input, context);
    }

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
            [WellKnownService.DynamoDB] = new Uri("http://localhost:8000")
          });
        });
      }
    }

    [UsedImplicitly]
    public void Configure(IHostingEnvironment environment, IConfiguration configuration)
    {
    }
  }
}