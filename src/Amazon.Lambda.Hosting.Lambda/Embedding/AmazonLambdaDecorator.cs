using System.Threading;
using System.Threading.Tasks;
using Amazon.Lambda.Model;
using Amazon.Runtime;

namespace Amazon.Lambda.Hosting.Embedding
{
  /// <summary>Base class for any <see cref="Amazon.Lambda.IAmazonLambda"/> client decorators.</summary>
  internal abstract class AmazonLambdaDecorator : IAmazonLambda
  {
    protected AmazonLambdaDecorator(IAmazonLambda client)
    {
      Check.NotNull(client, nameof(client));

      Client = client;
    }

    public IAmazonLambda Client { get; }

    public virtual IClientConfig Config => Client.Config;

    public virtual Task<AddPermissionResponse> AddPermissionAsync(AddPermissionRequest request, CancellationToken cancellationToken = new CancellationToken())
    {
      return Client.AddPermissionAsync(request, cancellationToken);
    }

    public virtual Task<CreateAliasResponse> CreateAliasAsync(CreateAliasRequest request, CancellationToken cancellationToken = new CancellationToken())
    {
      return Client.CreateAliasAsync(request, cancellationToken);
    }

    public virtual Task<CreateEventSourceMappingResponse> CreateEventSourceMappingAsync(CreateEventSourceMappingRequest request, CancellationToken cancellationToken = new CancellationToken())
    {
      return Client.CreateEventSourceMappingAsync(request, cancellationToken);
    }

    public virtual Task<CreateFunctionResponse> CreateFunctionAsync(CreateFunctionRequest request, CancellationToken cancellationToken = new CancellationToken())
    {
      return Client.CreateFunctionAsync(request, cancellationToken);
    }

    public virtual Task<DeleteAliasResponse> DeleteAliasAsync(DeleteAliasRequest request, CancellationToken cancellationToken = new CancellationToken())
    {
      return Client.DeleteAliasAsync(request, cancellationToken);
    }

    public virtual Task<DeleteEventSourceMappingResponse> DeleteEventSourceMappingAsync(DeleteEventSourceMappingRequest request, CancellationToken cancellationToken = new CancellationToken())
    {
      return Client.DeleteEventSourceMappingAsync(request, cancellationToken);
    }

    public virtual Task<DeleteFunctionResponse> DeleteFunctionAsync(string functionName, CancellationToken cancellationToken = new CancellationToken())
    {
      return Client.DeleteFunctionAsync(functionName, cancellationToken);
    }

    public virtual Task<DeleteFunctionResponse> DeleteFunctionAsync(DeleteFunctionRequest request, CancellationToken cancellationToken = new CancellationToken())
    {
      return Client.DeleteFunctionAsync(request, cancellationToken);
    }

    public virtual Task<DeleteFunctionConcurrencyResponse> DeleteFunctionConcurrencyAsync(DeleteFunctionConcurrencyRequest request, CancellationToken cancellationToken = new CancellationToken())
    {
      return Client.DeleteFunctionConcurrencyAsync(request, cancellationToken);
    }

    public virtual Task<GetAccountSettingsResponse> GetAccountSettingsAsync(GetAccountSettingsRequest request, CancellationToken cancellationToken = new CancellationToken())
    {
      return Client.GetAccountSettingsAsync(request, cancellationToken);
    }

    public virtual Task<GetAliasResponse> GetAliasAsync(GetAliasRequest request, CancellationToken cancellationToken = new CancellationToken())
    {
      return Client.GetAliasAsync(request, cancellationToken);
    }

    public virtual Task<GetEventSourceMappingResponse> GetEventSourceMappingAsync(GetEventSourceMappingRequest request, CancellationToken cancellationToken = new CancellationToken())
    {
      return Client.GetEventSourceMappingAsync(request, cancellationToken);
    }

    public virtual Task<GetFunctionResponse> GetFunctionAsync(string functionName, CancellationToken cancellationToken = new CancellationToken())
    {
      return Client.GetFunctionAsync(functionName, cancellationToken);
    }

    public virtual Task<GetFunctionResponse> GetFunctionAsync(GetFunctionRequest request, CancellationToken cancellationToken = new CancellationToken())
    {
      return Client.GetFunctionAsync(request, cancellationToken);
    }

    public virtual Task<GetFunctionConfigurationResponse> GetFunctionConfigurationAsync(string functionName, CancellationToken cancellationToken = new CancellationToken())
    {
      return Client.GetFunctionConfigurationAsync(functionName, cancellationToken);
    }

    public virtual Task<GetFunctionConfigurationResponse> GetFunctionConfigurationAsync(GetFunctionConfigurationRequest request, CancellationToken cancellationToken = new CancellationToken())
    {
      return Client.GetFunctionConfigurationAsync(request, cancellationToken);
    }

    public virtual Task<GetPolicyResponse> GetPolicyAsync(GetPolicyRequest request, CancellationToken cancellationToken = new CancellationToken())
    {
      return Client.GetPolicyAsync(request, cancellationToken);
    }

    public virtual Task<InvokeResponse> InvokeAsync(InvokeRequest request, CancellationToken cancellationToken = new CancellationToken())
    {
      return Client.InvokeAsync(request, cancellationToken);
    }

    public virtual Task<InvokeAsyncResponse> InvokeAsyncAsync(InvokeAsyncRequest request, CancellationToken cancellationToken = new CancellationToken())
    {
#pragma warning disable 618
      return Client.InvokeAsyncAsync(request, cancellationToken);
#pragma warning restore 618
    }

    public virtual Task<ListAliasesResponse> ListAliasesAsync(ListAliasesRequest request, CancellationToken cancellationToken = new CancellationToken())
    {
      return Client.ListAliasesAsync(request, cancellationToken);
    }

    public virtual Task<ListEventSourceMappingsResponse> ListEventSourceMappingsAsync(ListEventSourceMappingsRequest request, CancellationToken cancellationToken = new CancellationToken())
    {
      return Client.ListEventSourceMappingsAsync(request, cancellationToken);
    }

    public virtual Task<ListFunctionsResponse> ListFunctionsAsync(CancellationToken cancellationToken = new CancellationToken())
    {
      return Client.ListFunctionsAsync(cancellationToken);
    }

    public virtual Task<ListFunctionsResponse> ListFunctionsAsync(ListFunctionsRequest request, CancellationToken cancellationToken = new CancellationToken())
    {
      return Client.ListFunctionsAsync(request, cancellationToken);
    }

    public virtual Task<ListTagsResponse> ListTagsAsync(ListTagsRequest request, CancellationToken cancellationToken = new CancellationToken())
    {
      return Client.ListTagsAsync(request, cancellationToken);
    }

    public virtual Task<ListVersionsByFunctionResponse> ListVersionsByFunctionAsync(ListVersionsByFunctionRequest request, CancellationToken cancellationToken = new CancellationToken())
    {
      return Client.ListVersionsByFunctionAsync(request, cancellationToken);
    }

    public virtual Task<PublishVersionResponse> PublishVersionAsync(PublishVersionRequest request, CancellationToken cancellationToken = new CancellationToken())
    {
      return Client.PublishVersionAsync(request, cancellationToken);
    }

    public virtual Task<PutFunctionConcurrencyResponse> PutFunctionConcurrencyAsync(PutFunctionConcurrencyRequest request, CancellationToken cancellationToken = new CancellationToken())
    {
      return Client.PutFunctionConcurrencyAsync(request, cancellationToken);
    }

    public virtual Task<RemovePermissionResponse> RemovePermissionAsync(RemovePermissionRequest request, CancellationToken cancellationToken = new CancellationToken())
    {
      return Client.RemovePermissionAsync(request, cancellationToken);
    }

    public virtual Task<TagResourceResponse> TagResourceAsync(TagResourceRequest request, CancellationToken cancellationToken = new CancellationToken())
    {
      return Client.TagResourceAsync(request, cancellationToken);
    }

    public virtual Task<UntagResourceResponse> UntagResourceAsync(UntagResourceRequest request, CancellationToken cancellationToken = new CancellationToken())
    {
      return Client.UntagResourceAsync(request, cancellationToken);
    }

    public virtual Task<UpdateAliasResponse> UpdateAliasAsync(UpdateAliasRequest request, CancellationToken cancellationToken = new CancellationToken())
    {
      return Client.UpdateAliasAsync(request, cancellationToken);
    }

    public virtual Task<UpdateEventSourceMappingResponse> UpdateEventSourceMappingAsync(UpdateEventSourceMappingRequest request, CancellationToken cancellationToken = new CancellationToken())
    {
      return Client.UpdateEventSourceMappingAsync(request, cancellationToken);
    }

    public virtual Task<UpdateFunctionCodeResponse> UpdateFunctionCodeAsync(UpdateFunctionCodeRequest request, CancellationToken cancellationToken = new CancellationToken())
    {
      return Client.UpdateFunctionCodeAsync(request, cancellationToken);
    }

    public virtual Task<UpdateFunctionConfigurationResponse> UpdateFunctionConfigurationAsync(UpdateFunctionConfigurationRequest request, CancellationToken cancellationToken = new CancellationToken())
    {
      return Client.UpdateFunctionConfigurationAsync(request, cancellationToken);
    }

    public virtual void Dispose()
    {
      Client.Dispose();
    }
  }
}