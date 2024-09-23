// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.ConversationThreadsCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class ConversationThreadsCollectionRequest : 
    BaseRequest,
    IConversationThreadsCollectionRequest,
    IBaseRequest
  {
    public ConversationThreadsCollectionRequest(
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

    public Task<IConversationThreadsCollectionPage> GetAsync()
    {
      return this.GetAsync(CancellationToken.None);
    }

    public async Task<IConversationThreadsCollectionPage> GetAsync(
      CancellationToken cancellationToken)
    {
      this.Method = "GET";
      ConversationThreadsCollectionResponse collectionResponse = await this.SendAsync<ConversationThreadsCollectionResponse>((object) null, cancellationToken).ConfigureAwait(false);
      if (collectionResponse == null || collectionResponse.Value == null || collectionResponse.Value.CurrentPage == null)
        return (IConversationThreadsCollectionPage) null;
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

    public IConversationThreadsCollectionRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IConversationThreadsCollectionRequest) this;
    }

    public IConversationThreadsCollectionRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IConversationThreadsCollectionRequest) this;
    }

    public IConversationThreadsCollectionRequest Top(int value)
    {
      this.QueryOptions.Add(new QueryOption("$top", value.ToString()));
      return (IConversationThreadsCollectionRequest) this;
    }

    public IConversationThreadsCollectionRequest Filter(string value)
    {
      this.QueryOptions.Add(new QueryOption("$filter", value));
      return (IConversationThreadsCollectionRequest) this;
    }

    public IConversationThreadsCollectionRequest Skip(int value)
    {
      this.QueryOptions.Add(new QueryOption("$skip", value.ToString()));
      return (IConversationThreadsCollectionRequest) this;
    }

    public IConversationThreadsCollectionRequest OrderBy(string value)
    {
      this.QueryOptions.Add(new QueryOption("$orderby", value));
      return (IConversationThreadsCollectionRequest) this;
    }
  }
}
