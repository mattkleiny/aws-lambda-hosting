using System.Threading.Tasks;

namespace Amazon.Lambda.Hosting.Example.Services
{
  public sealed class TestService : ITestService
  {
    public Task<string> GetMessageAsync()
    {
      return Task.FromResult("Hello, World!");
    }
  }
}