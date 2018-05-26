using System;
using ServiceStack.DataAnnotations;

namespace Amazon.Lambda.Runtime.Example.ServiceStack.Model
{
  [Alias("BlogPosts")]
  public sealed class BlogPost
  {
    [PrimaryKey]
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Title { get; set; }
    public string Slug  { get; set; }
    public string Body  { get; set; }
  }
}