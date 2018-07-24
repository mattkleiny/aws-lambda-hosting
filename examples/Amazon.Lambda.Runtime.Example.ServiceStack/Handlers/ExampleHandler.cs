using System.Threading;
using System.Threading.Tasks;
using Amazon.Lambda.Hosting;
using Amazon.Lambda.Runtime.Example.ServiceStack.Model;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using ServiceStack.Aws.DynamoDb;

namespace Amazon.Lambda.Runtime.Example.ServiceStack.Handlers
{
  public sealed class ExampleHandler
  {
    private readonly ILogger<ExampleHandler> logger;

    public ExampleHandler(ILogger<ExampleHandler> logger)
    {
      this.logger = logger;
    }

    [LambdaFunction("handler-1")]
    public void Handler1(IPocoDynamo dynamo)
    {
      var posts = dynamo.GetAll<BlogPost>().ToArray();

      foreach (var post in posts)
      {
        logger.LogInformation($"{post.Id} - {post.Title} - {post.Slug} - {post.Body}");
      }
    }

    [LambdaFunction("handler-2")]
    public async Task Handler2(IAmazonS3 s3, CancellationToken cancellationToken)
    {
      var results = await s3.ListBucketsAsync(new ListBucketsRequest(), cancellationToken);

      foreach (var bucket in results.Buckets)
      {
        logger.LogInformation($"Found bucket {bucket.BucketName}");
      }
    }
  }
}