// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.ItemVersionsCollectionRequest
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
  public class ItemVersionsCollectionRequest : 
    BaseRequest,
    IItemVersionsCollectionRequest,
    IBaseRequest
  {
    public ItemVersionsCollectionRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
      this.SdkVersionHeaderPrefix = "onedrive";
    }

    public Task<Item> AddAsync(Item item) => this.AddAsync(item, CancellationToken.None);

    public Task<Item> AddAsync(Item item, CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "POST";
      return this.SendAsync<Item>((object) item, cancellationToken);
    }

    public Task<IItemVersionsCollectionPage> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<IItemVersionsCollectionPage> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      ItemVersionsCollectionResponse collectionResponse = await this.SendAsync<ItemVersionsCollectionResponse>((object) null, cancellationToken).ConfigureAwait(false);
      if (collectionResponse == null || collectionResponse.Value == null || collectionResponse.Value.CurrentPage == null)
        return (IItemVersionsCollectionPage) null;
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

    public IItemVersionsCollectionRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IItemVersionsCollectionRequest) this;
    }

    public IItemVersionsCollectionRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IItemVersionsCollectionRequest) this;
    }

    public IItemVersionsCollectionRequest Top(int value)
    {
      this.QueryOptions.Add(new QueryOption("$top", value.ToString()));
      return (IItemVersionsCollectionRequest) this;
    }

    public IItemVersionsCollectionRequest Filter(string value)
    {
      this.QueryOptions.Add(new QueryOption("$filter", value));
      return (IItemVersionsCollectionRequest) this;
    }

    public IItemVersionsCollectionRequest Skip(int value)
    {
      this.QueryOptions.Add(new QueryOption("$skip", value.ToString()));
      return (IItemVersionsCollectionRequest) this;
    }

    public IItemVersionsCollectionRequest OrderBy(string value)
    {
      this.QueryOptions.Add(new QueryOption("$orderby", value));
      return (IItemVersionsCollectionRequest) this;
    }
  }
}
