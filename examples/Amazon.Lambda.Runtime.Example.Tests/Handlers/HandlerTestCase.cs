using System;
using Amazon.Lambda.Hosting;
using Amazon.Lambda.Services;
using Amazon.Lambda.Testing;
using Microsoft.Extensions.Hosting;

namespace Amazon.Lambda.Runtime.Example.Tests.Handlers
{
  public abstract class HandlerTestCase
  {
    /// <summary>A configured <see cref="LambdaTestFixture"/> for the application.</summary>
    public LambdaTestFixture Fixture { get; } = new LambdaTestFixture(
      Startup.HostBuilder.ConfigureServices(services =>
      {
        // test-specific hosting options
        services.ConfigureHostingOptions(options =>
        {
          options.AWS.AccessKey       = "A1B2C3D4E5";
          options.AWS.SecretKey       = "A1B2C3D4E5";
          options.AWS.DefaultEndpoint = RegionEndpoint.APSoutheast2;

          options.RedirectTable[WellKnownService.S3] = new Uri("http://localhost:5000/minio");
        });
      })
    );
  }
}