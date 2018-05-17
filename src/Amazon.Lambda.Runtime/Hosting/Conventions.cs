﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Amazon.Lambda.Core;
using Amazon.Lambda.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Amazon.Lambda.Hosting
{
  internal static class Conventions
  {
    public static ServiceProvider ConfigureServices(object startup, IHostingEnvironment environment, IConfigurationRoot configuration, ILambdaContext target)
    {
      var collection = new ServiceCollection();

      FindCandidateMethods(target)
        .Where(method => "ConfigureServices".Equals(method.Method.Name, StringComparison.OrdinalIgnoreCase))
        .Where(method => method.Parameters.Length == 1)
        .Where(method => method.Parameters.Any(parameter => parameter.ParameterType == typeof(IServiceCollection)))
        .ForEach(method => method.Invoke(collection));

      // TODO: add per-environment services, as well
      throw new NotImplementedException();
    }

    public static void ConfigureEnvironment(object target, IHostingEnvironment environment, IConfigurationRoot configuration, IServiceProvider provider)
    {
      void ApplyConfiguration(CandidateMethod method)
      {
        var parameters = method.Parameters.Select(parameter =>
        {
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