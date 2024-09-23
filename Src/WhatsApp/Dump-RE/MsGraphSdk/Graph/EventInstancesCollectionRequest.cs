// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.EventInstancesCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class EventInstancesCollectionRequest : 
    BaseRequest,
    IEventInstancesCollectionRequest,
    IBaseRequest
  {
    public EventInstancesCollectionRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<Event> AddAsync(Event instancesEvent)
    {
      return this.AddAsync(instancesEvent, CancellationToken.None);
    }

    public Task<Event> AddAsync(Event instancesEvent, CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "POST";
      return this.SendAsync<Event>((object) instancesEvent, cancellationToken);
    }

    public Task<IEventInstancesCollectionPage> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<IEventInstancesCollectionPage> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      EventInstancesCollectionResponse collectionResponse = await this.SendAsync<EventInstancesCollectionResponse>((object) null, cancellationToken).ConfigureAwait(false);
      if (collectionResponse == null || collectionResponse.Value == null || collectionResponse.Value.CurrentPage == null)
        return (IEventInstancesCollectionPage) null;
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

    public IEventInstancesCollectionRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IEventInstancesCollectionRequest) this;
    }

    public IEventInstancesCollectionRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IEventInstancesCollectionRequest) this;
    }

    public IEventInstancesCollectionRequest Top(int value)
    {
      this.QueryOptions.Add(new QueryOption("$top", value.ToString()));
      return (IEventInstancesCollectionRequest) this;
    }

    public IEventInstancesCollectionRequest Filter(string value)
    {
      this.QueryOptions.Add(new QueryOption("$filter", value));
      return (IEventInstancesCollectionRequest) this;
    }

    public IEventInstancesCollectionRequest Skip(int value)
    {
      this.QueryOptions.Add(new QueryOption("$skip", value.ToString()));
      return (IEventInstancesCollectionRequest) this;
    }

    public IEventInstancesCollectionRequest OrderBy(string value)
    {
      this.QueryOptions.Add(new QueryOption("$orderby", value));
      return (IEventInstancesCollectionRequest) this;
    }
  }
}
