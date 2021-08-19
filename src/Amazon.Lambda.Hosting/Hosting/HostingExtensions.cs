using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Amazon.Lambda.Hosting
{
  /// <summary>
  /// Extensions for the <see cref="IHostBuilder"/> for use in AWS Lambda execution.
  /// <para/>
  /// Provides a configuration model very similar to a traditional ASP.NET Core Web application but
  /// with conveniences to help with environment-specific Lambda configuration and testing.
  /// </summary>
  public static class HostingExtensions
  {
    /// <summary>
    /// Specifies the given <see cref="TStartup"/> as the bootstrap container for the environment.
    /// <para/>
    /// The startup implementation should mimic a traditional ASP.NET Core Web application Startup
    /// with ConfigureServices and Configure methods as appropriate.
    /// </summary>
    public static IHostBuilder UseStartup<TStartup>(this IHostBuilder builder)
      where TStartup : class
    {
      void ConfigureStartup(HostBuilderContext context, IServiceCollection collection)
      {
        collection.AddSingleton<TStartup>();
        collection.AddSingleton(provider => new ConventionBasedStartup(
          startup: provider.GetRequiredService<TStartup>(),
          environment: context.HostingEnvironment
        ));
      }

      void ApplyContainerConventions(HostBuilderContext context, IServiceCollection collection)
      {
        var baseServices = collection.BuildServiceProvider();
        var startup      = baseServices.GetRequiredService<ConventionBasedStartup>();

        startup.ConfigureServices(collection);

        var allServices = collection.BuildServiceProvider();

        startup.Configure(allServices);
      }

      builder.ConfigureServices(ConfigureStartup);
      builder.ConfigureContainer<IServiceCollection>(ApplyContainerConventions);

      // this is supposed to be automatic, but it doesn't appear to work in the current release of the host builder
      builder.UseEnvironment(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production");

      return builder;
    }

    public static async Task<object?> ExecuteLambdaAsync(this IHostBuilder builder, object input, ILambdaContext context, CancellationToken cancellationToken = default)
    {
      using var host = builder.Build();

      return await host.ExecuteLambdaAsync(input, context, cancellationToken);
    }

    public static async Task<object?> ExecuteLambdaAsync(this IHost host, object input, ILambdaContext context, CancellationToken cancellationToken = default)
    {
      using var scope   = host.Services.CreateScope();
      var       handler = scope.ServiceProvider.ResolveLambdaHandler(context);

      return await handler.ExecuteAsync(input, context, cancellationToken);
    }

    public static LambdaHandlerMetadata ResolveLambdaHandlerMetadata(this IServiceProvider services, ILambdaContext context)
    {
      if (!services.TryResolveLambdaHandlerMetadata(context, out var metadata))
      {
        throw new UnresolvedHandlerException($"Unable to locate an appropriate handler for the function {context.FunctionName}");
      }

      return metadata;
    }

    public static bool TryResolveLambdaHandlerMetadata(this IServiceProvider services, ILambdaContext context, out LambdaHandlerMetadata result)
    {
      var metadatas = services.GetServices<LambdaHandlerMetadata>();
      var isMatch   = services.GetRequiredService<IOptions<HostingOptions>>().Value.MatchingStrategy;

      foreach (var metadata in metadatas)
      {
        if (isMatch(metadata, context))
        {
          result = metadata;
          return true;
        }
      }

      result = null!;
      return false;
    }

    public static ILambdaHandler ResolveLambdaHandler(this IServiceProvider services, ILambdaContext context)
    {
      if (!services.TryResolveLambdaHandler(context, out var handler))
      {
        throw new UnresolvedHandlerException($"Unable to locate an appropriate handler for the function {context.FunctionName}");
      }

      return handler;
    }

    public static bool TryResolveLambdaHandler(this IServiceProvider services, ILambdaContext context, out ILambdaHandler handler)
    {
      if (services.TryResolveLambdaHandlerMetadata(context, out var metadata))
      {
        handler = (ILambdaHandler)services.GetRequiredService(metadata.HandlerType);
        return true;
      }

      handler = null!;
      return false;
    }

    public static (ILambdaHandler, LambdaHandlerMetadata) ResolveLambdaHandlerWithMetadata(this IServiceProvider services, ILambdaContext context)
    {
      var metadata = services.ResolveLambdaHandlerMetadata(context);
      var handler  = (ILambdaHandler)services.GetRequiredService(metadata.HandlerType);

      return (handler, metadata);
    }

    public static IServiceCollection AddHandler<THandler>(this IServiceCollection services)
      where THandler : class, ILambdaHandler
    {
      var attribute = typeof(THandler).GetCustomAttribute<LambdaFunctionAttribute>();

      if (attribute == null)
      {
        throw new ArgumentException($"The handler ${typeof(THandler)} does not have an associated ${nameof(LambdaFunctionAttribute)}.");
      }

      return services.AddHandler<THandler>(attribute.FunctionName);
    }

    public static IServiceCollection AddHandler<THandler>(this IServiceCollection services, string functionName)
      where THandler : class, ILambdaHandler
    {
      Debug.Assert(!string.IsNullOrEmpty(functionName), "!string.IsNullOrEmpty(functionName)");

      services.AddScoped<THandler>();
      services.AddSingleton(LambdaHandlerMetadata.ForHandler<THandler>(functionName));

      return services;
    }

    public static IServiceCollection AddFunctionalHandlers<THandler>(this IServiceCollection services)
      where THandler : class
    {
      var functions = FunctionalDispatcher<THandler>.DiscoverFunctions();

      if (!functions.Any())
      {
        throw new InvalidOperationException($"The handler ${typeof(THandler)} does not have any valid functional handlers");
      }

      services.AddScoped<THandler>();
      services.AddScoped(provider => new FunctionalDispatcher<THandler>(
        host: provider.GetRequiredService<IHost>(),
        hostingOptions: provider.GetRequiredService<IOptions<HostingOptions>>(),
        functions: functions
      ));

      foreach (var function in functions)
      {
        services.AddSingleton(function.Metadata);
      }

      return services;
    }

    public static IServiceCollection ConfigureHostingOptions(this IServiceCollection services, Action<HostingOptions> configurer)
    {
      services.Configure(configurer);

      return services;
    }
  }
}