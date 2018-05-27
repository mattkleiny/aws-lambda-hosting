using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions.Internal;
using Microsoft.Extensions.Options;

namespace Amazon.Lambda.Diagnostics
{
  /// <summary>A <see cref="ILoggerProvider"/> which writes to the <see cref="LambdaLogger"/>.</summary>
  public sealed class LambdaLoggerProvider : ILoggerProvider
  {
    private readonly ConcurrentDictionary<string, ILogger> loggers = new ConcurrentDictionary<string, ILogger>();

    private readonly LambdaLoggerOptions          options;
    private readonly MessageQueue                 messageQueue;
    private readonly Func<string, LogLevel, bool> levelFilter;

    public LambdaLoggerProvider(
      IOptions<LambdaLoggerOptions> options,
      IOptions<LoggerFilterOptions> filterOptions)
    {
      Check.NotNull(options,       nameof(options));
      Check.NotNull(filterOptions, nameof(filterOptions));

      this.options = options.Value;

      messageQueue = new MessageQueue();

      levelFilter = (category, level) => level >= filterOptions.Value.MinLevel;
    }

    public ILogger CreateLogger(string categoryName)
    {
      Check.NotNullOrEmpty(categoryName, nameof(categoryName));

      return loggers.GetOrAdd(categoryName, _ => new Logger(categoryName, messageQueue, options, levelFilter));
    }

    public void Dispose()
    {
      messageQueue.Dispose();
    }

    /// <summary>A message for writing to the <see cref="ILambdaLogger"/>.</summary>
    private struct LogMessage
    {
      public EventId EventId { get; set; }
      public string  Message { get; set; }
    }

    /// <summary>An <see cref="ILogger"/> which delegates to the <see cref="ILambdaLogger"/>.</summary>
    private sealed class Logger : ILogger
    {
      private readonly string                       categoryName;
      private readonly MessageQueue                 messageQueue;
      private readonly LambdaLoggerOptions          options;
      private readonly Func<string, LogLevel, bool> levelFilter;

      public Logger(string categoryName, MessageQueue messageQueue, LambdaLoggerOptions options, Func<string, LogLevel, bool> levelFilter)
      {
        this.categoryName = categoryName;
        this.messageQueue = messageQueue;
        this.options      = options;
        this.levelFilter  = levelFilter;
      }

      public IDisposable BeginScope<TState>(TState state)
      {
        return NullScope.Instance;
      }

      public bool IsEnabled(LogLevel logLevel)
      {
        return levelFilter(categoryName, logLevel);
      }

      public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
      {
        if (!IsEnabled(logLevel)) return;

        var message = formatter(state, exception);

        messageQueue.EnqueueMessage(new LogMessage
        {
          EventId = eventId,
          Message = FormatMessage(logLevel, message)
        });
      }

      private string FormatMessage(LogLevel logLevel, string message)
      {
        // if a null message formatter is provided, just format an empty string
        return options?.MessageFormatter(logLevel, categoryName, message) ?? string.Empty;
      }
    }

    /// <summary>A queue of messages to be written to the <see cref="LambdaLogger"/> .</summary>
    private sealed class MessageQueue : IDisposable
    {
      private readonly BlockingCollection<LogMessage> messages = new BlockingCollection<LogMessage>(1024);

      private readonly Task processTask;

      public MessageQueue()
      {
        processTask = Task.Factory.StartNew(ProcessQueue, this, TaskCreationOptions.LongRunning);
      }

      public void EnqueueMessage(LogMessage message)
      {
        if (!messages.IsAddingCompleted)
        {
          try
          {
            messages.Add(message);
            return;
          }
          catch (InvalidOperationException)
          {
            // no-op
          }
        }

        WriteMessage(message);
      }

      public void Dispose()
      {
        messages.CompleteAdding();

        try
        {
          processTask.Wait(TimeSpan.FromSeconds(1));
        }
        catch (TaskCanceledException)
        {
          // no-op
        }
      }

      private void ProcessQueue(object state)
      {
        foreach (var message in messages.GetConsumingEnumerable())
        {
          WriteMessage(message);
        }
      }

      private static void WriteMessage(LogMessage message) => LambdaLogger.Log(message.Message);
    }
  }
}