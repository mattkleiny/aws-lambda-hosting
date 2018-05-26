using System.Threading.Tasks;

namespace Amazon.Lambda.Runtime.Example.Services
{
  public interface ITestService
  {
    Task<string> GetMessageAsync();
  }
}