// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.SubscribedSkuRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class SubscribedSkuRequest : BaseRequest, ISubscribedSkuRequest, IBaseRequest
  {
    public SubscribedSkuRequest(string requestUrl, IBaseClient client, IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<SubscribedSku> CreateAsync(SubscribedSku subscribedSkuToCreate)
    {
      return this.CreateAsync(subscribedSkuToCreate, CancellationToken.None);
    }

    public async Task<SubscribedSku> CreateAsync(
      SubscribedSku subscribedSkuToCreate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PUT";
      SubscribedSku subscribedSkuToInitialize = await this.SendAsync<SubscribedSku>((object) subscribedSkuToCreate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(subscribedSkuToInitialize);
      return subscribedSkuToInitialize;
    }

    public Task DeleteAsync() => this.DeleteAsync(CancellationToken.None);

    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
      this.Method = "DELETE";
      SubscribedSku subscribedSku = await this.SendAsync<SubscribedSku>((object) null, cancellationToken).ConfigureAwait(false);
    }

    public Task<SubscribedSku> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<SubscribedSku> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      SubscribedSku subscribedSkuToInitialize = await this.SendAsync<SubscribedSku>((object) null, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(subscribedSkuToInitialize);
      return subscribedSkuToInitialize;
    }

    public Task<SubscribedSku> UpdateAsync(SubscribedSku subscribedSkuToUpdate)
    {
      return this.UpdateAsync(subscribedSkuToUpdate, CancellationToken.None);
    }

    public async Task<SubscribedSku> UpdateAsync(
      SubscribedSku subscribedSkuToUpdate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PATCH";
      SubscribedSku subscribedSkuToInitialize = await this.SendAsync<SubscribedSku>((object) subscribedSkuToUpdate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(subscribedSkuToInitialize);
      return subscribedSkuToInitialize;
    }

    private void InitializeCollectionProperties(SubscribedSku subscribedSkuToInitialize)
    {
    }
  }
}
