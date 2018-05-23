using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Amazon.Lambda.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Amazon.Lambda.Hosting
{
  /// <summary>Applies convention-based configuration of the host and pipeline.</summary>
  internal static class Conventions
  {
    public static void ConfigureServices(object startup, IHostBuilder builder, IHostingEnvironment environment, IConfiguration configuration, IServiceCollection collection)
    {
      void ApplyConfiguration(CandidateMethod method)
      {
        var parameters = method.Parameters.Select<ParameterInfo, object>(parameter =>
        {
          if (parameter.ParameterType == typeof(IHostBuilder)) return builder;
          if (parameter.ParameterType == typeof(IHostingEnvironment)) return environment;
          if (parameter.ParameterType == typeof(IServiceCollection)) return collection;
          if (parameter.ParameterType == typeof(IConfiguration)) return configuration;

          throw new ArgumentException($"An unrecognized argument type was requested: {parameter.ParameterType}");
        });

        method.Invoke(parameters.ToArray());
      }

      // root services
      FindCandidateMethods(startup)
        .Where(method => "ConfigureServices".Equals(method.Method.Name, StringComparison.OrdinalIgnoreCase))
        .Where(method => method.Parameters.Any(parameter => parameter.ParameterType == typeof(IServiceCollection)))
        .ForEach(ApplyConfiguration);

      // per-environment services
      FindCandidateMethods(startup)
        .Where(method => $"Configure{environment.EnvironmentName}Services".Equals(method.Method.Name, StringComparison.OrdinalIgnoreCase))
        .Where(method => method.Parameters.Any(parameter => parameter.ParameterType == typeof(IServiceCollection)))
        .ForEach(ApplyConfiguration);
    }

    public static void ConfigureEnvironment(object target, IHostBuilder builder, IHostingEnvironment environment, IConfiguration configuration, IServiceProvider provider)
    {
      void ApplyConfiguration(CandidateMethod method)
      {
        var parameters = method.Parameters.Select(parameter =>
        {
          if (parameter.ParameterType == typeof(IHostBuilder)) return builder;
          if (parameter.ParameterType == typeof(IHostingEnvironment)) return environment;
          if (parameter.ParameterType == typeof(IServiceProvider)) return provider;
          if (parameter.ParameterType == typeof(IConfiguration)) return configuration;

          return provider.GetService(parameter.ParameterType);
        });

        method.Invoke(parameters.ToArray());
      }

      FindCandidateMethods(target)
        .Where(method => "Configure".Equals(method.Method.Name, StringComparison.OrdinalIgnoreCase))
        .Where(method => method.Parameters.All(parameter => parameter.ParameterType != typeof(IServiceCollection)))
        .ForEach(ApplyConfiguration);

      FindCandidateMethods(target)
        .Where(method => $"Configure{environment.EnvironmentName}".Equals(method.Method.Name, StringComparison.OrdinalIgnoreCase))
        .Where(method => method.Parameters.All(parameter => parameter.ParameterType != typeof(IServiceCollection)))
        .ForEach(ApplyConfiguration);
    }

    private static IEnumerable<CandidateMethod> FindCandidateMethods(object target) => target.GetType()
      .GetMethods(BindingFlags.Public | BindingFlags.Instance)
      .Select(method => new CandidateMethod(target, method));

    private sealed class CandidateMethod
    {
      private readonly object target;

      public CandidateMethod(object target, MethodInfo method)
      {
        this.target = target;

        Method     = method;
        Parameters = method.GetParameters();
      }

      public MethodInfo      Method     { get; }
      public ParameterInfo[] Parameters { get; }

      public void Invoke(params object[] parameters)
      {
        Method.Invoke(target, parameters);
      }
    }
  }
}