using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Amazon.Lambda.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Amazon.Lambda.Hosting
{
  /// <summary>A convention-based <see cref="IStartup"/> implementation that uses method reflection..</summary>
  internal sealed class ConventionBasedStartup : IStartup
  {
    private readonly object              startup;
    private readonly IHostingEnvironment environment;

    public ConventionBasedStartup(object startup, IHostingEnvironment environment)
    {
      this.startup     = startup;
      this.environment = environment;
    }

    public void Configure(IServiceProvider services)           => Conventions.ConfigureEnvironment(startup, services, environment);
    public void ConfigureServices(IServiceCollection services) => Conventions.ConfigureServices(startup, services, environment);

    /// <summary>Applies convention-based configuration of the host and pipeline.</summary>
    private static class Conventions
    {
      /// <summary>Applies the 'ConfigureServices' and 'Configure{Development/Staging/Production/etc}Services' methods with the given context.</summary>
      public static void ConfigureServices(object startup, IServiceCollection collection, IHostingEnvironment environment)
      {
        void ApplyConfiguration(CandidateMethod method)
        {
          var parameters = method.Parameters.Select<ParameterInfo, object>(parameter =>
          {
            if (parameter.ParameterType == typeof(IServiceCollection)) return collection;

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

      /// <summary>Applies the 'Configure' and 'Configure{Development/Staging/Production/etc}' methods with the given context.</summary>
      public static void ConfigureEnvironment(object startup, IServiceProvider provider, IHostingEnvironment environment)
      {
        void ApplyConfiguration(CandidateMethod method)
        {
          var parameters = method.Parameters.Select(parameter =>
          {
            if (parameter.ParameterType == typeof(IServiceProvider)) return provider;

            return provider.GetService(parameter.ParameterType);
          });

          method.Invoke(parameters.ToArray());
        }

        // root configuration
        FindCandidateMethods(startup)
          .Where(method => "Configure".Equals(method.Method.Name, StringComparison.OrdinalIgnoreCase))
          .Where(method => method.Parameters.All(parameter => parameter.ParameterType != typeof(IServiceCollection)))
          .ForEach(ApplyConfiguration);

        // per-environment configuration
        FindCandidateMethods(startup)
          .Where(method => $"Configure{environment.EnvironmentName}".Equals(method.Method.Name, StringComparison.OrdinalIgnoreCase))
          .Where(method => method.Parameters.All(parameter => parameter.ParameterType != typeof(IServiceCollection)))
          .ForEach(ApplyConfiguration);
      }

      /// <summary>Finds all of the <see cref="CandidateMethod"/>s on the given types that are both public and instance-bound.</summary>
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
}