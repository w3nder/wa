// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.EventMessageRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class EventMessageRequest : BaseRequest, IEventMessageRequest, IBaseRequest
  {
    public EventMessageRequest(string requestUrl, IBaseClient client, IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<EventMessage> CreateAsync(EventMessage eventMessageToCreate)
    {
      return this.CreateAsync(eventMessageToCreate, CancellationToken.None);
    }

    public async Task<EventMessage> CreateAsync(
      EventMessage eventMessageToCreate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PUT";
      EventMessage eventMessageToInitialize = await this.SendAsync<EventMessage>((object) eventMessageToCreate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(eventMessageToInitialize);
      return eventMessageToInitialize;
    }

    public Task DeleteAsync() => this.DeleteAsync(CancellationToken.None);

    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
      this.Method = "DELETE";
      EventMessage eventMessage = await this.SendAsync<EventMessage>((object) null, cancellationToken).ConfigureAwait(false);
    }

    public Task<EventMessage> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<EventMessage> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      EventMessage eventMessageToInitialize = await this.SendAsync<EventMessage>((object) null, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(eventMessageToInitialize);
      return eventMessageToInitialize;
    }

    public Task<EventMessage> UpdateAsync(EventMessage eventMessageToUpdate)
    {
      return this.UpdateAsync(eventMessageToUpdate, CancellationToken.None);
    }

    public async Task<EventMessage> UpdateAsync(
      EventMessage eventMessageToUpdate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PATCH";
      EventMessage eventMessageToInitialize = await this.SendAsync<EventMessage>((object) eventMessageToUpdate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(eventMessageToInitialize);
      return eventMessageToInitialize;
    }

    public IEventMessageRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IEventMessageRequest) this;
    }

    public IEventMessageRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IEventMessageRequest) this;
    }

    private void InitializeCollectionProperties(EventMessage eventMessageToInitialize)
    {
    }
  }
}
