namespace Amazon.Lambda.Diagnostics
{
  /// <summary>Options for the <see cref="LambdaLoggerProvider"/>.</summary>
  public sealed class LambdaLoggerOptions
  {
    /// <summary>The <see cref="MessageFormatter"/> to use when writing messages.</summary>
    public MessageFormatter MessageFormatter { get; set; } = MessageFormatters.DefaultFormatter();
  }
}