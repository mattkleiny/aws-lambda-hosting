using System.Threading.Tasks;

namespace Amazon.Lambda.Runtime.Example.Services
{
  public sealed class TestService : ITestService
  {
    public Task<string> GetMessageAsync()
    {
      return Task.FromResult("Hello, World!");
    }
  }
}