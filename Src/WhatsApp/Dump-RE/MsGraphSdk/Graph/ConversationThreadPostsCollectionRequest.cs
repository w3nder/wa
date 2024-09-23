// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.ConversationThreadPostsCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class ConversationThreadPostsCollectionRequest : 
    BaseRequest,
    IConversationThreadPostsCollectionRequest,
    IBaseRequest
  {
    public ConversationThreadPostsCollectionRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<Post> AddAsync(Post post) => this.AddAsync(post, CancellationToken.None);

    public Task<Post> AddAsync(Post post, CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "POST";
      return this.SendAsync<Post>((object) post, cancellationToken);
    }

    public Task<IConversationThreadPostsCollectionPage> GetAsync()
    {
      return this.GetAsync(CancellationToken.None);
    }

    public async Task<IConversationThreadPostsCollectionPage> GetAsync(
      CancellationToken cancellationToken)
    {
      this.Method = "GET";
      ConversationThreadPostsCollectionResponse collectionResponse = await this.SendAsync<ConversationThreadPostsCollectionResponse>((object) null, cancellationToken).ConfigureAwait(false);
      if (collectionResponse == null || collectionResponse.Value == null || collectionResponse.Value.CurrentPage == null)
        return (IConversationThreadPostsCollectionPage) null;
      if (collectionResponse.AdditionalData != null)
      {
        object obj;
        collectionResponse.AdditionalData.TryGetValue("@odata.nextLink", out obj);
        string nextPageLinkString = obj as string;
        if (!string.IsNullOrEmpty(nextPageLinkString))
          collectionResponse.Value.InitializeNextPageRequest(this.Client, nextPageLinkString);
        collectionResponse.Value.AdditionalData = collectionResponse.AdditionalData;
      }
      return collectionResponse.Value;
    }

    public IConversationThreadPostsCollectionRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IConversationThreadPostsCollectionRequest) this;
    }

    public IConversationThreadPostsCollectionRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IConversationThreadPostsCollectionRequest) this;
    }

    public IConversationThreadPostsCollectionRequest Top(int value)
    {
      this.QueryOptions.Add(new QueryOption("$top", value.ToString()));
      return (IConversationThreadPostsCollectionRequest) this;
    }

    public IConversationThreadPostsCollectionRequest Filter(string value)
    {
      this.QueryOptions.Add(new QueryOption("$filter", value));
      return (IConversationThreadPostsCollectionRequest) this;
    }

    public IConversationThreadPostsCollectionRequest Skip(int value)
    {
      this.QueryOptions.Add(new QueryOption("$skip", value.ToString()));
      return (IConversationThreadPostsCollectionRequest) this;
    }

    public IConversationThreadPostsCollectionRequest OrderBy(string value)
    {
      this.QueryOptions.Add(new QueryOption("$orderby", value));
      return (IConversationThreadPostsCollectionRequest) this;
    }
  }
}
