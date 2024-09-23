// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.EventRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class EventRequest : BaseRequest, IEventRequest, IBaseRequest
  {
    public EventRequest(string requestUrl, IBaseClient client, IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<Event> CreateAsync(Event eventToCreate)
    {
      return this.CreateAsync(eventToCreate, CancellationToken.None);
    }

    public async Task<Event> CreateAsync(Event eventToCreate, CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PUT";
      Event eventToInitialize = await this.SendAsync<Event>((object) eventToCreate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(eventToInitialize);
      return eventToInitialize;
    }

    public Task DeleteAsync() => this.DeleteAsync(CancellationToken.None);

    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
      this.Method = "DELETE";
      Event @event = await this.SendAsync<Event>((object) null, cancellationToken).ConfigureAwait(false);
    }

    public Task<Event> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<Event> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      Event eventToInitialize = await this.SendAsync<Event>((object) null, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(eventToInitialize);
      return eventToInitialize;
    }

    public Task<Event> UpdateAsync(Event eventToUpdate)
    {
      return this.UpdateAsync(eventToUpdate, CancellationToken.None);
    }

    public async Task<Event> UpdateAsync(Event eventToUpdate, CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PATCH";
      Event eventToInitialize = await this.SendAsync<Event>((object) eventToUpdate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(eventToInitialize);
      return eventToInitialize;
    }

    public IEventRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IEventRequest) this;
    }

    public IEventRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IEventRequest) this;
    }

    private void InitializeCollectionProperties(Event eventToInitialize)
    {
      if (eventToInitialize == null || eventToInitialize.AdditionalData == null)
        return;
      if (eventToInitialize.Instances != null && eventToInitialize.Instances.CurrentPage != null)
      {
        eventToInitialize.Instances.AdditionalData = eventToInitialize.AdditionalData;
        object obj;
        eventToInitialize.AdditionalData.TryGetValue("instances@odata.nextLink", out obj);
        string nextPageLinkString = obj as string;
        if (!string.IsNullOrEmpty(nextPageLinkString))
          eventToInitialize.Instances.InitializeNextPageRequest(this.Client, nextPageLinkString);
      }
      if (eventToInitialize.Extensions != null && eventToInitialize.Extensions.CurrentPage != null)
      {
        eventToInitialize.Extensions.AdditionalData = eventToInitialize.AdditionalData;
        object obj;
        eventToInitialize.AdditionalData.TryGetValue("extensions@odata.nextLink", out obj);
        string nextPageLinkString = obj as string;
        if (!string.IsNullOrEmpty(nextPageLinkString))
          eventToInitialize.Extensions.InitializeNextPageRequest(this.Client, nextPageLinkString);
      }
      if (eventToInitialize.Attachments == null || eventToInitialize.Attachments.CurrentPage == null)
        return;
      eventToInitialize.Attachments.AdditionalData = eventToInitialize.AdditionalData;
      object obj1;
      eventToInitialize.AdditionalData.TryGetValue("attachments@odata.nextLink", out obj1);
      string nextPageLinkString1 = obj1 as string;
      if (string.IsNullOrEmpty(nextPageLinkString1))
        return;
      eventToInitialize.Attachments.InitializeNextPageRequest(this.Client, nextPageLinkString1);
    }
  }
}
