// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.PostRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class PostRequest : BaseRequest, IPostRequest, IBaseRequest
  {
    public PostRequest(string requestUrl, IBaseClient client, IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<Post> CreateAsync(Post postToCreate)
    {
      return this.CreateAsync(postToCreate, CancellationToken.None);
    }

    public async Task<Post> CreateAsync(Post postToCreate, CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PUT";
      Post postToInitialize = await this.SendAsync<Post>((object) postToCreate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(postToInitialize);
      return postToInitialize;
    }

    public Task DeleteAsync() => this.DeleteAsync(CancellationToken.None);

    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
      this.Method = "DELETE";
      Post post = await this.SendAsync<Post>((object) null, cancellationToken).ConfigureAwait(false);
    }

    public Task<Post> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<Post> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      Post postToInitialize = await this.SendAsync<Post>((object) null, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(postToInitialize);
      return postToInitialize;
    }

    public Task<Post> UpdateAsync(Post postToUpdate)
    {
      return this.UpdateAsync(postToUpdate, CancellationToken.None);
    }

    public async Task<Post> UpdateAsync(Post postToUpdate, CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PATCH";
      Post postToInitialize = await this.SendAsync<Post>((object) postToUpdate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(postToInitialize);
      return postToInitialize;
    }

    public IPostRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IPostRequest) this;
    }

    public IPostRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IPostRequest) this;
    }

    private void InitializeCollectionProperties(Post postToInitialize)
    {
      if (postToInitialize == null || postToInitialize.AdditionalData == null)
        return;
      if (postToInitialize.Extensions != null && postToInitialize.Extensions.CurrentPage != null)
      {
        postToInitialize.Extensions.AdditionalData = postToInitialize.AdditionalData;
        object obj;
        postToInitialize.AdditionalData.TryGetValue("extensions@odata.nextLink", out obj);
        string nextPageLinkString = obj as string;
        if (!string.IsNullOrEmpty(nextPageLinkString))
          postToInitialize.Extensions.InitializeNextPageRequest(this.Client, nextPageLinkString);
      }
      if (postToInitialize.Attachments == null || postToInitialize.Attachments.CurrentPage == null)
        return;
      postToInitialize.Attachments.AdditionalData = postToInitialize.AdditionalData;
      object obj1;
      postToInitialize.AdditionalData.TryGetValue("attachments@odata.nextLink", out obj1);
      string nextPageLinkString1 = obj1 as string;
      if (string.IsNullOrEmpty(nextPageLinkString1))
        return;
      postToInitialize.Attachments.InitializeNextPageRequest(this.Client, nextPageLinkString1);
    }
  }
}
