// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.GroupConversationsCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class GroupConversationsCollectionRequest : 
    BaseRequest,
    IGroupConversationsCollectionRequest,
    IBaseRequest
  {
    public GroupConversationsCollectionRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<Conversation> AddAsync(Conversation conversation)
    {
      return this.AddAsync(conversation, CancellationToken.None);
    }

    public Task<Conversation> AddAsync(
      Conversation conversation,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "POST";
      return this.SendAsync<Conversation>((object) conversation, cancellationToken);
    }

    public Task<IGroupConversationsCollectionPage> GetAsync()
    {
      return this.GetAsync(CancellationToken.None);
    }

    public async Task<IGroupConversationsCollectionPage> GetAsync(
      CancellationToken cancellationToken)
    {
      this.Method = "GET";
      GroupConversationsCollectionResponse collectionResponse = await this.SendAsync<GroupConversationsCollectionResponse>((object) null, cancellationToken).ConfigureAwait(false);
      if (collectionResponse == null || collectionResponse.Value == null || collectionResponse.Value.CurrentPage == null)
        return (IGroupConversationsCollectionPage) null;
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

    public IGroupConversationsCollectionRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IGroupConversationsCollectionRequest) this;
    }

    public IGroupConversationsCollectionRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IGroupConversationsCollectionRequest) this;
    }

    public IGroupConversationsCollectionRequest Top(int value)
    {
      this.QueryOptions.Add(new QueryOption("$top", value.ToString()));
      return (IGroupConversationsCollectionRequest) this;
    }

    public IGroupConversationsCollectionRequest Filter(string value)
    {
      this.QueryOptions.Add(new QueryOption("$filter", value));
      return (IGroupConversationsCollectionRequest) this;
    }

    public IGroupConversationsCollectionRequest Skip(int value)
    {
      this.QueryOptions.Add(new QueryOption("$skip", value.ToString()));
      return (IGroupConversationsCollectionRequest) this;
    }

    public IGroupConversationsCollectionRequest OrderBy(string value)
    {
      this.QueryOptions.Add(new QueryOption("$orderby", value));
      return (IGroupConversationsCollectionRequest) this;
    }
  }
}
