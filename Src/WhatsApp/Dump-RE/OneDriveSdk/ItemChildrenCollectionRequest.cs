// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.ItemChildrenCollectionRequest
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
  public class ItemChildrenCollectionRequest : 
    BaseRequest,
    IItemChildrenCollectionRequest,
    IBaseRequest
  {
    public ItemChildrenCollectionRequest(
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

    public Task<IItemChildrenCollectionPage> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<IItemChildrenCollectionPage> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      ItemChildrenCollectionResponse collectionResponse = await this.SendAsync<ItemChildrenCollectionResponse>((object) null, cancellationToken).ConfigureAwait(false);
      if (collectionResponse == null || collectionResponse.Value == null || collectionResponse.Value.CurrentPage == null)
        return (IItemChildrenCollectionPage) null;
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

    public IItemChildrenCollectionRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IItemChildrenCollectionRequest) this;
    }

    public IItemChildrenCollectionRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IItemChildrenCollectionRequest) this;
    }

    public IItemChildrenCollectionRequest Top(int value)
    {
      this.QueryOptions.Add(new QueryOption("$top", value.ToString()));
      return (IItemChildrenCollectionRequest) this;
    }

    public IItemChildrenCollectionRequest Filter(string value)
    {
      this.QueryOptions.Add(new QueryOption("$filter", value));
      return (IItemChildrenCollectionRequest) this;
    }

    public IItemChildrenCollectionRequest Skip(int value)
    {
      this.QueryOptions.Add(new QueryOption("$skip", value.ToString()));
      return (IItemChildrenCollectionRequest) this;
    }

    public IItemChildrenCollectionRequest OrderBy(string value)
    {
      this.QueryOptions.Add(new QueryOption("$orderby", value));
      return (IItemChildrenCollectionRequest) this;
    }
  }
}
