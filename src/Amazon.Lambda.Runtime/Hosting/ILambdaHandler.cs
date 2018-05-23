using System.Threading.Tasks;
using Amazon.Lambda.Core;

namespace Amazon.Lambda.Hosting
{
  /// <summary>A strongly-typed lambda handler for the application pipeline.</summary>
  public interface ILambdaHandler
  {
    /// <summary>Executes the handler asynchronously with the given input and context.</summary>
    Task<object> ExecuteAsync(object input, ILambdaContext context);
  }
}