using System.Threading.Tasks;

namespace Amazon.Lambda.Services
{
  public interface ITestService
  {
    Task<string> GetMessageAsync();
  }
}