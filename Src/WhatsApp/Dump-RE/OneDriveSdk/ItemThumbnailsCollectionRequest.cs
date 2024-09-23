// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.ItemThumbnailsCollectionRequest
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
  public class ItemThumbnailsCollectionRequest : 
    BaseRequest,
    IItemThumbnailsCollectionRequest,
    IBaseRequest
  {
    public ItemThumbnailsCollectionRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
      this.SdkVersionHeaderPrefix = "onedrive";
    }

    public Task<ThumbnailSet> AddAsync(ThumbnailSet thumbnailSet)
    {
      return this.AddAsync(thumbnailSet, CancellationToken.None);
    }

    public Task<ThumbnailSet> AddAsync(
      ThumbnailSet thumbnailSet,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "POST";
      return this.SendAsync<ThumbnailSet>((object) thumbnailSet, cancellationToken);
    }

    public Task<IItemThumbnailsCollectionPage> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<IItemThumbnailsCollectionPage> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      ItemThumbnailsCollectionResponse collectionResponse = await this.SendAsync<ItemThumbnailsCollectionResponse>((object) null, cancellationToken).ConfigureAwait(false);
      if (collectionResponse == null || collectionResponse.Value == null || collectionResponse.Value.CurrentPage == null)
        return (IItemThumbnailsCollectionPage) null;
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

    public IItemThumbnailsCollectionRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IItemThumbnailsCollectionRequest) this;
    }

    public IItemThumbnailsCollectionRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IItemThumbnailsCollectionRequest) this;
    }

    public IItemThumbnailsCollectionRequest Top(int value)
    {
      this.QueryOptions.Add(new QueryOption("$top", value.ToString()));
      return (IItemThumbnailsCollectionRequest) this;
    }

    public IItemThumbnailsCollectionRequest Filter(string value)
    {
      this.QueryOptions.Add(new QueryOption("$filter", value));
      return (IItemThumbnailsCollectionRequest) this;
    }

    public IItemThumbnailsCollectionRequest Skip(int value)
    {
      this.QueryOptions.Add(new QueryOption("$skip", value.ToString()));
      return (IItemThumbnailsCollectionRequest) this;
    }

    public IItemThumbnailsCollectionRequest OrderBy(string value)
    {
      this.QueryOptions.Add(new QueryOption("$orderby", value));
      return (IItemThumbnailsCollectionRequest) this;
    }
  }
}
