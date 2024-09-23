// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.SubscriptionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class SubscriptionRequest : BaseRequest, ISubscriptionRequest, IBaseRequest
  {
    public SubscriptionRequest(string requestUrl, IBaseClient client, IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<Subscription> CreateAsync(Subscription subscriptionToCreate)
    {
      return this.CreateAsync(subscriptionToCreate, CancellationToken.None);
    }

    public async Task<Subscription> CreateAsync(
      Subscription subscriptionToCreate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PUT";
      Subscription subscriptionToInitialize = await this.SendAsync<Subscription>((object) subscriptionToCreate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(subscriptionToInitialize);
      return subscriptionToInitialize;
    }

    public Task DeleteAsync() => this.DeleteAsync(CancellationToken.None);

    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
      this.Method = "DELETE";
      Subscription subscription = await this.SendAsync<Subscription>((object) null, cancellationToken).ConfigureAwait(false);
    }

    public Task<Subscription> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<Subscription> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      Subscription subscriptionToInitialize = await this.SendAsync<Subscription>((object) null, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(subscriptionToInitialize);
      return subscriptionToInitialize;
    }

    public Task<Subscription> UpdateAsync(Subscription subscriptionToUpdate)
    {
      return this.UpdateAsync(subscriptionToUpdate, CancellationToken.None);
    }

    public async Task<Subscription> UpdateAsync(
      Subscription subscriptionToUpdate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PATCH";
      Subscription subscriptionToInitialize = await this.SendAsync<Subscription>((object) subscriptionToUpdate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(subscriptionToInitialize);
      return subscriptionToInitialize;
    }

    private void InitializeCollectionProperties(Subscription subscriptionToInitialize)
    {
    }
  }
}
