using System;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.Hosting;
using Amazon.Lambda.Model;
using Amazon.Lambda.Services;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServiceStack.Aws.DynamoDb;

namespace Amazon.Lambda
{
  public sealed class Startup
  {
    /// <summary>The <see cref="IHostBuilder"/> for this example.</summary>
    public static IHostBuilder HostBuilder => new HostBuilder()
      .UseStartup<Startup>()
      .UseDynamo()
      .WithFunctionalHandlers<Startup>();

    /// <summary>This is the entry point from the CLI.</summary>
    public static Task<int> Main(string[] args)
      => HostBuilder.RunLambdaConsoleAsync(args);

    /// <summary>This is the entry point from AWS.</summary>
    [UsedImplicitly]
    public static Task<object> ExecuteAsync(object input, ILambdaContext context)
      => HostBuilder.RunLambdaAsync(input, context);

    [LambdaFunction("handler-1")]
    public void Handler1(object input, IPocoDynamo dynamo)
    {
      var posts = dynamo.GetAll<BlogPost>().ToArray();

      foreach (var post in posts)
      {
        Console.WriteLine($"{post.Id} - {post.Title} - {post.Slug} - {post.Body}");
      }
    }

    [UsedImplicitly]
    public void ConfigureServices(IServiceCollection services, IHostingEnvironment environment)
    {
      services.AddScoped<IPocoDynamo, PocoDynamo>();

      if (environment.IsDevelopment())
      {
        services.ConfigureHostingOptions(options =>
        {
          options.MatchingStrategy = MatchingStrategies.MatchByNameSuffix(StringComparison.OrdinalIgnoreCase);

          options.AWS.AccessKey = "A1B2C3D4E5";
          options.AWS.SecretKey = "A1B2C3D4E5";

          options.RedirectTable[WellKnownService.Dynamo] = new Uri("http://localhost:8000");
        });
      }
    }

    [UsedImplicitly]
    public void Configure(IPocoDynamo dynamo, IHostingEnvironment environment)
    {
      dynamo.RegisterTable<BlogPost>();

      // seed some data in the development environment
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