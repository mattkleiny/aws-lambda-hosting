using System;
using System.Reflection;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Amazon.Lambda.Hosting
{
  /// <summary>
  /// A <see cref="HostBuilder"/> specialization for AWS Lambda execution.
  /// <para/>
  /// Provides a configuration model very similar to a traditional ASP.NET Core WEb application but
  /// with conveniences to help with environment-specific Lambda configuration and testing.
  /// </summary>
  public sealed class LambdaHostBuilder : HostBuilder
  {
    /// <summary>
    /// Specifies the given <see cref="TStartup"/> as the bootstrap container for the environment.
    /// <para/>
    /// The startup implementation should mimic a traditional ASP.NET Core Web application Startup
    /// with ConfigureServices and Configure methods as appropriate.
    /// </summary>
    public LambdaHostBuilder UseStartup<TStartup>()
      where TStartup : class, new()
    {
      var startup = new TStartup();

      void ApplyServiceConventions(HostBuilderContext context, IServiceCollection collection)
      {
        Conventions.ConfigureServices(startup, this, context.HostingEnvironment, context.Configuration, collection);
      }
      
      void ApplyContainerConventions(HostBuilderContext context, IServiceCollection builder)
      {
        var services = builder.BuildServiceProvider();

        Conventions.ConfigureEnvironment(startup, this, context.HostingEnvironment, context.Configuration, services);
      }

      ConfigureServices(ApplyServiceConventions);
      ConfigureContainer<IServiceCollection>(ApplyContainerConventions);

      // this is supposed to be automatic, but it doesn't appear to work in the current release of the host builder
      this.UseEnvironment(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production");
      
      return this;
    }

    /// <summary>Maps the given <see cref="ILambdaHandler"/> with it's associate <see cref="LambdaFunctionAttribute.FunctionName"/> in the host.</summary>
    public LambdaHostBuilder WithHandler<THandler>()
      where THandler : ILambdaHandler
    {
      var attribute = typeof(THandler).GetCustomAttribute<LambdaFunctionAttribute>();

      if (attribute == null)
      {
        throw new ArgumentException($"The handler ${typeof(THandler)} does not have an associated ${nameof(LambdaFunctionAttribute)}.");
      }

      return WithHandler<THandler>(attribute.FunctionName);
    }

    /// <summary>Maps the given <see cref="ILambdaHandler"/> to the given <see cref="functionName"/> in the host.</summary>
    public LambdaHostBuilder WithHandler<THandler>(string functionName)
      where THandler : ILambdaHandler
    {
      return this;
    }

    /// <summary>Executes the appropriate <see cref="ILambdaHandler"/> for given input and <see cref="ILambdaContext"/>.</summary>
    public async Task<object> ExecuteAsync(object input, ILambdaContext context)
    {
      Check.NotNull(context, nameof(context));

      throw new NotImplementedException();
    }
  }
}