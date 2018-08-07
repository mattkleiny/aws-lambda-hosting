using System.Threading.Tasks;

namespace Amazon.Lambda.Hosting.Example.Services
{
  public interface ITestService
  {
    Task<string> GetMessageAsync();
  }
}