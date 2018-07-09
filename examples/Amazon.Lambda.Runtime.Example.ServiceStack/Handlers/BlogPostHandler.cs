using Amazon.Lambda.Hosting;
using Amazon.Lambda.Runtime.Example.ServiceStack.Model;
using Microsoft.Extensions.Logging;
using ServiceStack.Aws.DynamoDb;

namespace Amazon.Lambda.Runtime.Example.ServiceStack.Handlers
{
  public sealed class BlogPostHandler
  {
    private readonly ILogger<BlogPostHandler> logger;

    public BlogPostHandler(ILogger<BlogPostHandler> logger)
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
  }
}