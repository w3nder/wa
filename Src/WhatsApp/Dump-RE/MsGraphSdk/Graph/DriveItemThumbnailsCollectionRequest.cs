// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.DriveItemThumbnailsCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class DriveItemThumbnailsCollectionRequest : 
    BaseRequest,
    IDriveItemThumbnailsCollectionRequest,
    IBaseRequest
  {
    public DriveItemThumbnailsCollectionRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
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

    public Task<IDriveItemThumbnailsCollectionPage> GetAsync()
    {
      return this.GetAsync(CancellationToken.None);
    }

    public async Task<IDriveItemThumbnailsCollectionPage> GetAsync(
      CancellationToken cancellationToken)
    {
      this.Method = "GET";
      DriveItemThumbnailsCollectionResponse collectionResponse = await this.SendAsync<DriveItemThumbnailsCollectionResponse>((object) null, cancellationToken).ConfigureAwait(false);
      if (collectionResponse == null || collectionResponse.Value == null || collectionResponse.Value.CurrentPage == null)
        return (IDriveItemThumbnailsCollectionPage) null;
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

    public IDriveItemThumbnailsCollectionRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IDriveItemThumbnailsCollectionRequest) this;
    }

    public IDriveItemThumbnailsCollectionRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IDriveItemThumbnailsCollectionRequest) this;
    }

    public IDriveItemThumbnailsCollectionRequest Top(int value)
    {
      this.QueryOptions.Add(new QueryOption("$top", value.ToString()));
      return (IDriveItemThumbnailsCollectionRequest) this;
    }

    public IDriveItemThumbnailsCollectionRequest Filter(string value)
    {
      this.QueryOptions.Add(new QueryOption("$filter", value));
      return (IDriveItemThumbnailsCollectionRequest) this;
    }

    public IDriveItemThumbnailsCollectionRequest Skip(int value)
    {
      this.QueryOptions.Add(new QueryOption("$skip", value.ToString()));
      return (IDriveItemThumbnailsCollectionRequest) this;
    }

    public IDriveItemThumbnailsCollectionRequest OrderBy(string value)
    {
      this.QueryOptions.Add(new QueryOption("$orderby", value));
      return (IDriveItemThumbnailsCollectionRequest) this;
    }
  }
}
