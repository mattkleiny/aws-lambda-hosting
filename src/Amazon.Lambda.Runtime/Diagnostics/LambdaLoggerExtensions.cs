using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Amazon.Lambda.Diagnostics
{
  /// <summary>Extensions for configuring the <see cref="LambdaLoggerProvider"/>.</summary>
  public static class LambdaLoggerExtensions
  {
    public static ILoggingBuilder AddLambdaLogger(this ILoggingBuilder builder)
    {
      return builder.AddLambdaLogger(_ => { });
    }

    public static ILoggingBuilder AddLambdaLogger(this ILoggingBuilder builder, Action<LambdaLoggerOptions> configurer)
    {
      Check.NotNull(configurer, nameof(configurer));

      builder.Services.AddSingleton<ILoggerProvider, LambdaLoggerProvider>();
      builder.Services.Configure(configurer);

      return builder;
    }
  }
}