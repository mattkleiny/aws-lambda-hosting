﻿using System.Threading;
using System.Threading.Tasks;
using Amazon.Lambda.Core;

namespace Amazon.Lambda
{
  /// <summary>A strongly-typed lambda handler for the application pipeline.</summary>
  public interface ILambdaHandler
  {
    /// <summary>Executes the handler asynchronously with the given input and context.</summary>
    Task<object?> ExecuteAsync(object input, ILambdaContext context, CancellationToken cancellationToken = default);
  }

  /// <summary>A strongly-typed <see cref="ILambdaHandler"/> that expects no input and provides no output.</summary>
  public abstract class LambdaHandler : ILambdaHandler
  {
    public abstract Task ExecuteAsync(ILambdaContext context, CancellationToken cancellationToken = default);

    async Task<object?> ILambdaHandler.ExecuteAsync(object input, ILambdaContext context, CancellationToken cancellationToken)
    {
      await ExecuteAsync(context, cancellationToken);

      return null;
    }
  }

  /// <summary>A strongly-typed <see cref="ILambdaHandler"/> that expects input of the given type, <see cref="T"/>.</summary>
  public abstract class LambdaHandler<T> : ILambdaHandler
  {
    public abstract Task<object> ExecuteAsync(T input, ILambdaContext context, CancellationToken cancellationToken = default);

    async Task<object?> ILambdaHandler.ExecuteAsync(object input, ILambdaContext context, CancellationToken cancellationToken)
    {
      return await ExecuteAsync((T)input, context, cancellationToken);
    }
  }

  /// <summary>A strongly-typed <see cref="ILambdaHandler"/> that expects input of the given type, <see cref="TInput"/> and produces output of type <see cref="TOutput"/>.</summary>
  public abstract class LambdaHandler<TInput, TOutput> : ILambdaHandler
  {
    public abstract Task<TOutput> ExecuteAsync(TInput input, ILambdaContext context, CancellationToken cancellationToken = default);

    async Task<object?> ILambdaHandler.ExecuteAsync(object input, ILambdaContext context, CancellationToken cancellationToken)
    {
      return await ExecuteAsync((TInput)input, context, cancellationToken);
    }
  }
}