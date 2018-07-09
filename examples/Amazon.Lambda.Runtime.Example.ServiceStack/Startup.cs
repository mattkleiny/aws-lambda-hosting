using System;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.Diagnostics;
using Amazon.Lambda.Hosting;
using Amazon.Lambda.Runtime.Example.ServiceStack.Handlers;
using Amazon.Lambda.Runtime.Example.ServiceStack.Model;
using Amazon.Lambda.Serialization.Json;
using Amazon.Lambda.Services;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServiceStack.Aws.DynamoDb;

[assembly: LambdaSerializer(typeof(JsonSerializer))]

namespace Amazon.Lambda.Runtime.Example.ServiceStack
{
  public sealed class Startup
  {
    /// <summary>The <see cref="IHostBuilder"/> for this example.</summary>
    public static IHostBuilder HostBuilder => new HostBuilder()
      .UseStartup<Startup>()
      .UseDynamo()
      .WithFunctionalHandlers<BlogPostHandler>();

    /// <summary>This is the entry point from the CLI.</summary>
    public static Task<int> Main(string[] args)
      => HostBuilder.RunLambdaConsoleAsync(args);

    /// <summary>This is the entry point from AWS.</summary>
    [UsedImplicitly]
    public static Task<object> ExecuteAsync(object input, ILambdaContext context)
      => HostBuilder.RunLambdaAsync(input, context);

    [UsedImplicitly]
    public void ConfigureServices(IServiceCollection services, IHostingEnvironment environment)
    {
      services.AddLogging(builder =>
      {
        builder.AddLambdaLogger();
        builder.SetMinimumLevel(environment.IsDevelopment() ? LogLevel.Trace : LogLevel.Information);
      });

      services.AddScoped<IPocoDynamo, PocoDynamo>();

      services.ConfigureHostingOptions(options =>
      {
        if (environment.IsDevelopment())
        {
          options.AWS.AccessKey = "A1B2C3D4E5";
          options.AWS.SecretKey = "A1B2C3D4E5";

          options.RedirectTable[WellKnownService.Dynamo] = new Uri("http://localhost:8000");
        }
      });
    }

    [UsedImplicitly]
    public void Configure(IPocoDynamo dynamo, IHostingEnvironment environment)
    {
      dynamo.RegisterTable<BlogPost>();

      if (environment.IsDevelopment())
      {
        dynamo.DeleteAllTables();
        dynamo.InitSchema();

        dynamo.PutItems(Enumerable.Range(0, 100).Select(i => new BlogPost
        {
          Title = $"Post {i}",
          Slug  = $"post-{i}",
          Body  = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Pellentesque ut elit nunc. Donec lacinia nisl non molestie gravida."
        }));
      }
    }
  }
}