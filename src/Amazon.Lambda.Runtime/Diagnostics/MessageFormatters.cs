using System;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Amazon.Lambda.Diagnostics
{
  /// <summary>A message formatter for the <see cref="LambdaLoggerProvider"/>.</summary>
  public delegate string MessageFormatter(LogLevel level, string categoryName, string message);

  /// <summary>Factory for some default <see cref="MessageFormatter"/>s.</summary>
  public static class MessageFormatters
  {
    /// <summary>Builds a default <see cref="MessageFormatter"/> with the given included metadata.</summary>
    public static MessageFormatter DefaultFormatter(bool includeTime = true, bool includeLevel = true, bool includeScope = true)
    {
      return (level, categoryName, message) =>
      {
        var builder = new StringBuilder();

        if (includeTime) builder.Append($"{DateTime.Now:s} - ");
        if (includeScope) builder.Append(categoryName);
        if (includeLevel) builder.Append($" [{level.ToString().ToUpper()}]: ");

        builder.Append(message);

        return builder.ToString();
      };
    }
  }
}