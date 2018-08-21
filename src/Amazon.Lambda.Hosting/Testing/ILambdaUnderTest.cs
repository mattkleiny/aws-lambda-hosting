﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Lambda.Hosting;

namespace Amazon.Lambda.Testing
{
  /// <summary>
  /// Denotes a <see cref="ILambdaHandler"/> that is bootstrapped for testing.
  /// <para/>
  /// Provides a convenience interface for execution of the Lambda itself as well as a common extension point for configuration.
  /// </summary>
  public interface ILambdaUnderTest<out THandler>
    where THandler : class, ILambdaHandler
  {
    LambdaContext    Context  { get; }
    THandler         Handler  { get; }
    IServiceProvider Services { get; }

    Task<object> ExecuteAsync(object input, CancellationToken cancellationToken = default);
  }
}