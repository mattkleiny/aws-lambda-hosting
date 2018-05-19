using System.Threading.Tasks;

namespace Amazon.Lambda.Services
{
  public interface ITestService
  {
    Task<string> GetMessageAsync();
  }

  public sealed class TestService : ITestService
  {
    public Task<string> GetMessageAsync()
    {
      return Task.FromResult("Hello, World!");
    }
  }
}