using System;
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
        if (typeof(IStartup).IsAssignableFrom(typeof(TStartup)))
        {
          collection.AddSingleton(typeof(IStartup), typeof(TStartup));
        }
        else
        {
          collection.AddSingleton<TStartup>();
          collection.AddSingleton<IStartup>(provider => new ConventionBasedStartup(
            startup: provider.GetRequiredService<TStartup>(),
            environment: context.HostingEnvironment
          ));
        }
      }

      void ApplyContainerConventions(HostBuilderContext context, IServiceCollection collection)
      {
        var baseServices = collection.BuildServiceProvider();
        var startup      = baseServices.GetRequiredService<IStartup>();

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

    /// <summary>Executes the appropriate <see cref="ILambdaHandler"/> for given input and <see cref="ILambdaContext"/>.</summary>
    public static async Task<object> RunLambdaAsync(this IHostBuilder builder, object input, ILambdaContext context, CancellationToken cancellationToken = default)
    {
      Check.NotNull(context, nameof(context));

      using (var host = builder.Build())
      {
        return await host.RunLambdaAsync(input, context, cancellationToken);
      }
    }

    /// <summary>Executes the appropriate <see cref="ILambdaHandler"/> for given input and <see cref="ILambdaContext"/>.</summary>
    public static async Task<object> RunLambdaAsync(this IHost host, object input, ILambdaContext context, CancellationToken cancellationToken = default)
    {
      Check.NotNull(context, nameof(context));

      using (var scope = host.Services.CreateScope())
      {
        var handler = scope.ServiceProvider.ResolveLambdaHandler(input, context);

        return await handler.ExecuteAsync(input, context, cancellationToken);
      }
    }

    /// <summary>Resolves the appropriate <see cref="ILambdaHandler"/> for the given context.</summary>
    public static ILambdaHandler ResolveLambdaHandler(this IServiceProvider services, object input, ILambdaContext context)
    {
      Check.NotNull(context, nameof(context));

      if (!services.TryResolveLambdaHandler(input, context, out var handler))
      {
        throw new UnresolvedHandlerException($"Unable to locate an appropriate handler for the function {context.FunctionName}");
      }

      return handler;
    }

    /// <summary>Attempts to resolve the <see cref="ILambdaHandler"/> for the given context.</summary>
    public static bool TryResolveLambdaHandler(this IServiceProvider services, object input, ILambdaContext context, out ILambdaHandler handler)
    {
      Check.NotNull(context, nameof(context));

      var registrations = services.GetServices<LambdaHandlerRegistration>();
      var isMatch       = services.GetRequiredService<IOptions<HostingOptions>>().Value.MatchingStrategy;

      foreach (var registration in registrations)
      {
        if (isMatch(registration, input, context))
        {
          handler = (ILambdaHandler) services.GetService(registration.HandlerType);
          return true;
        }
      }

      handler = null;
      return false;
    }

    /// <summary>Maps the given <see cref="ILambdaHandler"/> with it's associate <see cref="LambdaFunctionAttribute.FunctionName"/> in the host.</summary>
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

    /// <summary>Maps the given <see cref="ILambdaHandler"/> to the given <see cref="functionName"/> in the host.</summary>
    public static IServiceCollection AddHandler<THandler>(this IServiceCollection services, string functionName)
      where THandler : class, ILambdaHandler
    {
      Check.NotNullOrEmpty(functionName, nameof(functionName));

      services.AddScoped<THandler>();
      services.AddSingleton(new LambdaHandlerRegistration(
        functionName: functionName,
        handlerType: typeof(THandler),
        friendlyName: typeof(THandler).Name
      ));

      return services;
    }

    /// <summary>Maps the functional handlers from the given classes publically accessible <see cref="LambdaFunctionAttribute"/> annotated methods.</summary>
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
        services.AddSingleton(new LambdaHandlerRegistration(
          function.FunctionName,
          handlerType: typeof(FunctionalDispatcher<THandler>),
          friendlyName: function.FriendlyName
        ));
      }

      return services;
    }

    /// <summary>Configures the <see cref="HostingOptions"/> for the application.</summary>
    public static IServiceCollection ConfigureHostingOptions(this IServiceCollection services, Action<HostingOptions> configurer)
    {
      Check.NotNull(configurer, nameof(configurer));

      services.Configure(configurer);

      return services;
    }
  }
}