using ServiceStack.DataAnnotations;

namespace Amazon.Lambda.Model
{
  [Alias("BlogPosts")]
  public sealed class BlogPost
  {
    [AutoIncrement]
    public long Id { get; set; }

    public string Title { get; set; }
    public string Slug  { get; set; }
    public string Body  { get; set; }
  }
}