// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.GroupThreadsCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class GroupThreadsCollectionRequest : 
    BaseRequest,
    IGroupThreadsCollectionRequest,
    IBaseRequest
  {
    public GroupThreadsCollectionRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<ConversationThread> AddAsync(ConversationThread conversationThread)
    {
      return this.AddAsync(conversationThread, CancellationToken.None);
    }

    public Task<ConversationThread> AddAsync(
      ConversationThread conversationThread,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "POST";
      return this.SendAsync<ConversationThread>((object) conversationThread, cancellationToken);
    }

    public Task<IGroupThreadsCollectionPage> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<IGroupThreadsCollectionPage> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      GroupThreadsCollectionResponse collectionResponse = await this.SendAsync<GroupThreadsCollectionResponse>((object) null, cancellationToken).ConfigureAwait(false);
      if (collectionResponse == null || collectionResponse.Value == null || collectionResponse.Value.CurrentPage == null)
        return (IGroupThreadsCollectionPage) null;
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

    public IGroupThreadsCollectionRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IGroupThreadsCollectionRequest) this;
    }

    public IGroupThreadsCollectionRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IGroupThreadsCollectionRequest) this;
    }

    public IGroupThreadsCollectionRequest Top(int value)
    {
      this.QueryOptions.Add(new QueryOption("$top", value.ToString()));
      return (IGroupThreadsCollectionRequest) this;
    }

    public IGroupThreadsCollectionRequest Filter(string value)
    {
      this.QueryOptions.Add(new QueryOption("$filter", value));
      return (IGroupThreadsCollectionRequest) this;
    }

    public IGroupThreadsCollectionRequest Skip(int value)
    {
      this.QueryOptions.Add(new QueryOption("$skip", value.ToString()));
      return (IGroupThreadsCollectionRequest) this;
    }

    public IGroupThreadsCollectionRequest OrderBy(string value)
    {
      this.QueryOptions.Add(new QueryOption("$orderby", value));
      return (IGroupThreadsCollectionRequest) this;
    }
  }
}
