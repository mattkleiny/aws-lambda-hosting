using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Amazon.Lambda.Hosting
{
  /// <summary>A convention-based startup implementation that uses method reflection.</summary>
  internal sealed class ConventionBasedStartup
  {
    private readonly object           startup;
    private readonly IHostEnvironment environment;

    public ConventionBasedStartup(object startup, IHostEnvironment environment)
    {
      this.startup     = startup;
      this.environment = environment;
    }

    public void Configure(IServiceProvider services)
    {
      void ApplyConfiguration(CandidateMethod method)
      {
        var parameters = method.Parameters.Select(parameter =>
        {
          if (parameter.ParameterType == typeof(IServiceProvider)) return services;

          return services.GetService(parameter.ParameterType);
        });

        method.Invoke(parameters.ToArray());
      }

      // root configuration
      var rootCandidates = FindCandidateMethods(startup)
        .Where(method => "Configure".Equals(method.Method.Name, StringComparison.OrdinalIgnoreCase))
        .Where(method => method.Parameters.All(parameter => parameter.ParameterType != typeof(IServiceCollection)));

      foreach (var candidate in rootCandidates)
      {
        ApplyConfiguration(candidate);
      }

      // per-environment configuration
      var perEnvironmentCandidates = FindCandidateMethods(startup)
        .Where(method => $"Configure{environment.EnvironmentName}".Equals(method.Method.Name, StringComparison.OrdinalIgnoreCase))
        .Where(method => method.Parameters.All(parameter => parameter.ParameterType != typeof(IServiceCollection)));

      foreach (var candidate in perEnvironmentCandidates)
      {
        ApplyConfiguration(candidate);
      }
    }

    public void ConfigureServices(IServiceCollection services)
    {
      void ApplyConfiguration(CandidateMethod method)
      {
        var parameters = method.Parameters.Select<ParameterInfo, object>(parameter =>
        {
          if (parameter.ParameterType == typeof(IServiceCollection)) return services;

          throw new ArgumentException($"An unrecognized argument type was requested: {parameter.ParameterType}");
        });

        method.Invoke(parameters.ToArray());
      }

      // root services
      var rootCandidates = FindCandidateMethods(startup)
        .Where(method => "ConfigureServices".Equals(method.Method.Name, StringComparison.OrdinalIgnoreCase))
        .Where(method => method.Parameters.Any(parameter => parameter.ParameterType == typeof(IServiceCollection)));

      foreach (var candidate in rootCandidates)
      {
        ApplyConfiguration(candidate);
      }

      // per-environment services
      var perEnvironmentCandidates = FindCandidateMethods(startup)
        .Where(method => $"Configure{environment.EnvironmentName}Services".Equals(method.Method.Name, StringComparison.OrdinalIgnoreCase))
        .Where(method => method.Parameters.Any(parameter => parameter.ParameterType == typeof(IServiceCollection)));

      foreach (var candidate in perEnvironmentCandidates)
      {
        ApplyConfiguration(candidate);
      }
    }

    private static IEnumerable<CandidateMethod> FindCandidateMethods(object target)
    {
      return target.GetType()
        .GetMethods(BindingFlags.Public | BindingFlags.Instance)
        .Select(method => new CandidateMethod(target, method));
    }

    private readonly record struct CandidateMethod(object Target, MethodInfo Method)
    {
      public ParameterInfo[] Parameters { get; } = Method.GetParameters();

      public void Invoke(params object?[] parameters)
      {
        Method.Invoke(Target, parameters);
      }
    }
  }
}