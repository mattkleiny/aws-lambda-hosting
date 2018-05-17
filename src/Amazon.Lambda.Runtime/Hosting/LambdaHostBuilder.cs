using System;

namespace Amazon.Lambda.Hosting
{
  /// <summary>
  /// Configures a <see cref="IDisposable"/> for AWS Lambda handler execution.
  /// <para/>
  /// The hosting configuration mimics the ASP.NET Core initialization mechanism,
  /// for it's flexibility and familiarity, and permits execution of handlers
  /// either via a testing harness, in-situ or from the AWS production runtime.
  /// </summary>
  public sealed class LambdaHostBuilder
  {
    internal object Startup { get; private set; }

    public LambdaHostBuilder UseStartup<TStartup>()
      where TStartup : class, new()
    {
      Startup = new TStartup();
      return this;
    }

    public LambdaHostBuilder WithHandler<THandler>()
      where THandler : ILambdaHandler
    {
      return this;
    }

    public LambdaHostBuilder WithHandler<THandler>(string functionName)
      where THandler : ILambdaHandler
    {
      return this;
    }

    public LambdaHost Build()
    {
      Check.NotNull(Startup, nameof(Startup));

      return new LambdaHost(Startup);
    }
  }
}