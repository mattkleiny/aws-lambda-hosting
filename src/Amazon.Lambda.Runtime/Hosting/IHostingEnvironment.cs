namespace Amazon.Lambda.Hosting
{
  public interface IHostingEnvironment
  {
    string ApplicationName { get; }
    string EnvironmentName { get; }
  }
}