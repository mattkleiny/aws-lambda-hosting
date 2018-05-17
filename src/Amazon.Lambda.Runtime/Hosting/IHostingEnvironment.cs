using System;

namespace Amazon.Lambda.Hosting
{
  public interface IHostingEnvironment
  {
    string ApplicationName { get; }
    string EnvironmentName { get; }
  }

  public static class HostingEnvironmentExtensions
  {
    public static bool IsDevelopment(this IHostingEnvironment environment)
      => "Development".Equals(environment.EnvironmentName, StringComparison.OrdinalIgnoreCase);

    public static bool IsStaging(this IHostingEnvironment environment)
      => "Staging".Equals(environment.EnvironmentName, StringComparison.OrdinalIgnoreCase);

    public static bool IsProduction(this IHostingEnvironment environment)
      => "Production".Equals(environment.EnvironmentName, StringComparison.OrdinalIgnoreCase);
  }
}