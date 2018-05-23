using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Amazon.Lambda.Hosting
{
  /// <summary>
  /// Configures a <see cref="IDisposable"/> for AWS Lambda handler execution.
  /// <para/>
  /// The hosting configuration mimics the ASP.NET Core initialization mechanism,
  /// for it's flexibility and familiarity, and permits execution of handlers
  /// either via a testing harness, in-situ or from the AWS production runtime.
  /// </summary>
  public sealed class LambdaHostBuilder : HostBuilder
  {
    public LambdaHostBuilder UseStartup<TStartup>()
      where TStartup : class, new()
    {
      var startup = new TStartup();

      ConfigureServices((context, collection) =>
      {
        Conventions.ConfigureServices(startup, this, context.HostingEnvironment, context.Configuration, collection);
      });
      
      ConfigureContainer<IServiceCollection>((context, builder) =>
      {
        var services = builder.BuildServiceProvider();
        
        Conventions.ConfigureEnvironment(startup, this, context.HostingEnvironment, context.Configuration, services);
      });
      
      return this;
    }

    public LambdaHostBuilder WithHandler<THandler>()
      where THandler : ILambdaHandler
    {
      return this;
    }

    public LambdaHostBuilder WithHandler<THandler>(string functionName)
      where THandler : ILambdaHandler
    {
      return this;
    }

    public LambdaHost BuildLambdaHost()
    {
      return new LambdaHost(Build());
    }
  }
}