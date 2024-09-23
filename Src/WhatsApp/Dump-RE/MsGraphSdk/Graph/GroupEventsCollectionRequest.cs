// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.GroupEventsCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class GroupEventsCollectionRequest : 
    BaseRequest,
    IGroupEventsCollectionRequest,
    IBaseRequest
  {
    public GroupEventsCollectionRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<Event> AddAsync(Event eventsEvent)
    {
      return this.AddAsync(eventsEvent, CancellationToken.None);
    }

    public Task<Event> AddAsync(Event eventsEvent, CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "POST";
      return this.SendAsync<Event>((object) eventsEvent, cancellationToken);
    }

    public Task<IGroupEventsCollectionPage> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<IGroupEventsCollectionPage> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      GroupEventsCollectionResponse collectionResponse = await this.SendAsync<GroupEventsCollectionResponse>((object) null, cancellationToken).ConfigureAwait(false);
      if (collectionResponse == null || collectionResponse.Value == null || collectionResponse.Value.CurrentPage == null)
        return (IGroupEventsCollectionPage) null;
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

    public IGroupEventsCollectionRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IGroupEventsCollectionRequest) this;
    }

    public IGroupEventsCollectionRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IGroupEventsCollectionRequest) this;
    }

    public IGroupEventsCollectionRequest Top(int value)
    {
      this.QueryOptions.Add(new QueryOption("$top", value.ToString()));
      return (IGroupEventsCollectionRequest) this;
    }

    public IGroupEventsCollectionRequest Filter(string value)
    {
      this.QueryOptions.Add(new QueryOption("$filter", value));
      return (IGroupEventsCollectionRequest) this;
    }

    public IGroupEventsCollectionRequest Skip(int value)
    {
      this.QueryOptions.Add(new QueryOption("$skip", value.ToString()));
      return (IGroupEventsCollectionRequest) this;
    }

    public IGroupEventsCollectionRequest OrderBy(string value)
    {
      this.QueryOptions.Add(new QueryOption("$orderby", value));
      return (IGroupEventsCollectionRequest) this;
    }
  }
}
