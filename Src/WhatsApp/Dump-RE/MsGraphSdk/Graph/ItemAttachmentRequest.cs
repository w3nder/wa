// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.ItemAttachmentRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class ItemAttachmentRequest : BaseRequest, IItemAttachmentRequest, IBaseRequest
  {
    public ItemAttachmentRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<ItemAttachment> CreateAsync(ItemAttachment itemAttachmentToCreate)
    {
      return this.CreateAsync(itemAttachmentToCreate, CancellationToken.None);
    }

    public async Task<ItemAttachment> CreateAsync(
      ItemAttachment itemAttachmentToCreate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PUT";
      ItemAttachment itemAttachmentToInitialize = await this.SendAsync<ItemAttachment>((object) itemAttachmentToCreate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(itemAttachmentToInitialize);
      return itemAttachmentToInitialize;
    }

    public Task DeleteAsync() => this.DeleteAsync(CancellationToken.None);

    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
      this.Method = "DELETE";
      ItemAttachment itemAttachment = await this.SendAsync<ItemAttachment>((object) null, cancellationToken).ConfigureAwait(false);
    }

    public Task<ItemAttachment> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<ItemAttachment> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      ItemAttachment itemAttachmentToInitialize = await this.SendAsync<ItemAttachment>((object) null, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(itemAttachmentToInitialize);
      return itemAttachmentToInitialize;
    }

    public Task<ItemAttachment> UpdateAsync(ItemAttachment itemAttachmentToUpdate)
    {
      return this.UpdateAsync(itemAttachmentToUpdate, CancellationToken.None);
    }

    public async Task<ItemAttachment> UpdateAsync(
      ItemAttachment itemAttachmentToUpdate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PATCH";
      ItemAttachment itemAttachmentToInitialize = await this.SendAsync<ItemAttachment>((object) itemAttachmentToUpdate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(itemAttachmentToInitialize);
      return itemAttachmentToInitialize;
    }

    public IItemAttachmentRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IItemAttachmentRequest) this;
    }

    public IItemAttachmentRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IItemAttachmentRequest) this;
    }

    private void InitializeCollectionProperties(ItemAttachment itemAttachmentToInitialize)
    {
    }
  }
}
