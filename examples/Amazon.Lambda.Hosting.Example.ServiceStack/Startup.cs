using System;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.Diagnostics;
using Amazon.Lambda.Hosting;
using Amazon.Lambda.Hosting.Example.ServiceStack.Handlers;
using Amazon.Lambda.Hosting.Example.ServiceStack.Model;
using Amazon.Lambda.Serialization.Json;
using Amazon.Lambda.Services;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServiceStack.Aws.DynamoDb;

[assembly: LambdaSerializer(typeof(JsonSerializer))]

namespace Amazon.Lambda.Hosting.Example.ServiceStack
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

        builder.AddParameterStore(basePath: "/lambda-runtime-example", optional: true, reloadAfter: TimeSpan.FromMinutes(1));

        builder.AddEnvironmentVariables();
      })
      .UseStartup<Startup>();

    /// <summary>The cached <see cref="IHost"/> for this application.</summary>
    public static IHost Host { get; } = HostBuilder.Build();

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

    [UsedImplicitly]
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddLogging(builder =>
      {
        builder.AddLambdaLogger();
        builder.SetMinimumLevel(Environment.IsDevelopment() ? LogLevel.Trace : LogLevel.Information);
      });

      services.AddDynamo();
      services.AddS3();

      services.AddFunctionalHandlers<ExampleHandler>();
      services.AddScoped<IPocoDynamo, PocoDynamo>();

      services.ConfigureHostingOptions(options =>
      {
        if (Environment.IsDevelopment())
        {
          options.AWS.AccessKey = "A1B2C3D4E5";
          options.AWS.SecretKey = "A1B2C3D4E5";

          options.RedirectTable[WellKnownService.Dynamo] = new Uri("http://localhost:8000");
          options.RedirectTable[WellKnownService.S3]     = new Uri("http://localhost:9000");
        }
      });
    }

    [UsedImplicitly]
    public void Configure(IPocoDynamo dynamo)
    {
      dynamo.RegisterTable<BlogPost>();

      if (Environment.IsDevelopment())
      {
        // asynchronously rebuild the development database
        Task.Factory.StartNew(() =>
        {
          dynamo.DeleteAllTables();
          dynamo.InitSchema();

          dynamo.PutItems(Enumerable.Range(0, 100).Select(i => new BlogPost
          {
            Title = $"Post {i}",
            Slug  = $"post-{i}",
            Body  = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Pellentesque ut elit nunc. Donec lacinia nisl non molestie gravida."
          }));
        });
      }
    }
  }
}