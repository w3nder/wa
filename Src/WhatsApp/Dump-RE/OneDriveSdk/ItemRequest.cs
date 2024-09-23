// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.ItemRequest
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.OneDrive.Sdk
{
  public class ItemRequest : BaseRequest, IItemRequest, IBaseRequest
  {
    public ItemRequest(string requestUrl, IBaseClient client, IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
      this.SdkVersionHeaderPrefix = "onedrive";
    }

    public Task<Item> CreateAsync(Item itemToCreate)
    {
      return this.CreateAsync(itemToCreate, CancellationToken.None);
    }

    public async Task<Item> CreateAsync(Item itemToCreate, CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PUT";
      Item itemToInitialize = await this.SendAsync<Item>((object) itemToCreate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(itemToInitialize);
      return itemToInitialize;
    }

    public Task DeleteAsync() => this.DeleteAsync(CancellationToken.None);

    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
      this.Method = "DELETE";
      Item obj = await this.SendAsync<Item>((object) null, cancellationToken).ConfigureAwait(false);
    }

    public Task<Item> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<Item> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      Item itemToInitialize = await this.SendAsync<Item>((object) null, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(itemToInitialize);
      return itemToInitialize;
    }

    public Task<Item> UpdateAsync(Item itemToUpdate)
    {
      return this.UpdateAsync(itemToUpdate, CancellationToken.None);
    }

    public async Task<Item> UpdateAsync(Item itemToUpdate, CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PATCH";
      Item itemToInitialize = await this.SendAsync<Item>((object) itemToUpdate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(itemToInitialize);
      return itemToInitialize;
    }

    public IItemRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IItemRequest) this;
    }

    public IItemRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IItemRequest) this;
    }

    private void InitializeCollectionProperties(Item itemToInitialize)
    {
      if (itemToInitialize == null || itemToInitialize.AdditionalData == null)
        return;
      if (itemToInitialize.Permissions != null && itemToInitialize.Permissions.CurrentPage != null)
      {
        itemToInitialize.Permissions.AdditionalData = itemToInitialize.AdditionalData;
        object obj;
        itemToInitialize.AdditionalData.TryGetValue("permissions@odata.nextLink", out obj);
        string nextPageLinkString = obj as string;
        if (!string.IsNullOrEmpty(nextPageLinkString))
          itemToInitialize.Permissions.InitializeNextPageRequest(this.Client, nextPageLinkString);
      }
      if (itemToInitialize.Versions != null && itemToInitialize.Versions.CurrentPage != null)
      {
        itemToInitialize.Versions.AdditionalData = itemToInitialize.AdditionalData;
        object obj;
        itemToInitialize.AdditionalData.TryGetValue("versions@odata.nextLink", out obj);
        string nextPageLinkString = obj as string;
        if (!string.IsNullOrEmpty(nextPageLinkString))
          itemToInitialize.Versions.InitializeNextPageRequest(this.Client, nextPageLinkString);
      }
      if (itemToInitialize.Children != null && itemToInitialize.Children.CurrentPage != null)
      {
        itemToInitialize.Children.AdditionalData = itemToInitialize.AdditionalData;
        object obj;
        itemToInitialize.AdditionalData.TryGetValue("children@odata.nextLink", out obj);
        string nextPageLinkString = obj as string;
        if (!string.IsNullOrEmpty(nextPageLinkString))
          itemToInitialize.Children.InitializeNextPageRequest(this.Client, nextPageLinkString);
      }
      if (itemToInitialize.Thumbnails == null || itemToInitialize.Thumbnails.CurrentPage == null)
        return;
      itemToInitialize.Thumbnails.AdditionalData = itemToInitialize.AdditionalData;
      object obj1;
      itemToInitialize.AdditionalData.TryGetValue("thumbnails@odata.nextLink", out obj1);
      string nextPageLinkString1 = obj1 as string;
      if (string.IsNullOrEmpty(nextPageLinkString1))
        return;
      itemToInitialize.Thumbnails.InitializeNextPageRequest(this.Client, nextPageLinkString1);
    }
  }
}
