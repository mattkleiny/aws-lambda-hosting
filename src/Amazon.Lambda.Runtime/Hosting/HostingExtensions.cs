﻿using System;
using System.Reflection;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Amazon.Lambda.Hosting
{
  /// <summary>
  /// Extensions for the <see cref="IHostBuilder"/> for use in AWS Lambda execution.
  /// <para/>
  /// Provides a configuration model very similar to a traditional ASP.NET Core WEb application but
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
      where TStartup : class, new()
    {
      var startup = new TStartup();

      void ApplyServiceConventions(HostBuilderContext context, IServiceCollection collection)
      {
        Conventions.ConfigureServices(startup, builder, context.HostingEnvironment, context.Configuration, collection);
      }

      void ApplyContainerConventions(HostBuilderContext context, IServiceCollection collection)
      {
        var services = collection.BuildServiceProvider();

        Conventions.ConfigureEnvironment(startup, builder, context.HostingEnvironment, context.Configuration, services);
      }

      builder.ConfigureServices(ApplyServiceConventions);
      builder.ConfigureContainer<IServiceCollection>(ApplyContainerConventions);

      // this is supposed to be automatic, but it doesn't appear to work in the current release of the host builder
      builder.UseEnvironment(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production");

      return builder;
    }

    /// <summary>Maps the given <see cref="ILambdaHandler"/> with it's associate <see cref="LambdaFunctionAttribute.FunctionName"/> in the host.</summary>
    public static IHostBuilder WithHandler<THandler>(this IHostBuilder builder)
      where THandler : class, ILambdaHandler
    {
      var attribute = typeof(THandler).GetCustomAttribute<LambdaFunctionAttribute>();

      if (attribute == null)
      {
        throw new ArgumentException($"The handler ${typeof(THandler)} does not have an associated ${nameof(LambdaFunctionAttribute)}.");
      }

      return builder.WithHandler<THandler>(attribute.FunctionName);
    }

    /// <summary>Maps the given <see cref="ILambdaHandler"/> to the given <see cref="functionName"/> in the host.</summary>
    public static IHostBuilder WithHandler<THandler>(this IHostBuilder builder, string functionName)
      where THandler : class, ILambdaHandler
    {
      Check.NotNullOrEmpty(functionName, nameof(functionName));

      builder.ConfigureServices(services =>
      {
        services.AddScoped<THandler>();
        services.AddSingleton(_ => new LambdaHandlerRegistration(functionName, typeof(THandler)));
      });

      return builder;
    }

    /// <summary>Executes the appropriate <see cref="ILambdaHandler"/> for given input and <see cref="ILambdaContext"/>.</summary>
    public static async Task<object> RunLambdaAsync(this IHostBuilder builder, object input, ILambdaContext context)
    {
      Check.NotNull(context, nameof(context));

      using (var host = builder.Build())
      {
        return await host.RunLambdaAsync(input, context);
      }
    }

    /// <summary>Executes the appropriate <see cref="ILambdaHandler"/> for given input and <see cref="ILambdaContext"/>.</summary>
    public static async Task<object> RunLambdaAsync(this IHost host, object input, ILambdaContext context)
    {
      Check.NotNull(context, nameof(context));

      var handler = host.Services.ResolveLambdaHandler(context);

      return await handler.ExecuteAsync(input, context);
    }

    /// <summary>Adds a service which displays a menu of all the attached lambda handlers and permits their execution.</summary>
    public static IHostBuilder WithLambdaSwitchboard(this IHostBuilder builder)
    {
      builder.ConfigureServices(services => services.AddSingleton<IHostedService, LambdaSwitchboard>());
      
      return builder;
    }

    /// <summary>Resolves the appropraite <see cref="ILambdaHandler"/> for the given <see cref="ILambdaContext"/>.</summary>
    public static ILambdaHandler ResolveLambdaHandler(this IServiceProvider services, ILambdaContext context)
    {
      Check.NotNull(context, nameof(context));

      return services.ResolveLambdaHandler(context.FunctionName);
    }

    /// <summary>Resolves the appropraite <see cref="ILambdaHandler"/> for the given <see cref="functionName"/>.</summary>
    public static ILambdaHandler ResolveLambdaHandler(this IServiceProvider services, string functionName)
    {
      Check.NotNullOrEmpty(functionName, nameof(functionName));

      var registrations = services.GetServices<LambdaHandlerRegistration>();

      foreach (var registration in registrations)
      {
        if (registration.FunctionName.Equals(functionName, StringComparison.OrdinalIgnoreCase))
        {
          return (ILambdaHandler) services.GetService(registration.HandlerType);
        }
      }

      throw new UnresolvedHandlerException($"Unable to locate an appropriate handler for the function {functionName}");
    }
  }
}