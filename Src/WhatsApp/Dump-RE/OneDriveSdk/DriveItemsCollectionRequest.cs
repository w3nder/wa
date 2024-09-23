// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.DriveItemsCollectionRequest
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
  public class DriveItemsCollectionRequest : BaseRequest, IDriveItemsCollectionRequest, IBaseRequest
  {
    public DriveItemsCollectionRequest(
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

    public Task<IDriveItemsCollectionPage> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<IDriveItemsCollectionPage> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      DriveItemsCollectionResponse collectionResponse = await this.SendAsync<DriveItemsCollectionResponse>((object) null, cancellationToken).ConfigureAwait(false);
      if (collectionResponse == null || collectionResponse.Value == null || collectionResponse.Value.CurrentPage == null)
        return (IDriveItemsCollectionPage) null;
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

    public IDriveItemsCollectionRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IDriveItemsCollectionRequest) this;
    }

    public IDriveItemsCollectionRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IDriveItemsCollectionRequest) this;
    }

    public IDriveItemsCollectionRequest Top(int value)
    {
      this.QueryOptions.Add(new QueryOption("$top", value.ToString()));
      return (IDriveItemsCollectionRequest) this;
    }

    public IDriveItemsCollectionRequest Filter(string value)
    {
      this.QueryOptions.Add(new QueryOption("$filter", value));
      return (IDriveItemsCollectionRequest) this;
    }

    public IDriveItemsCollectionRequest Skip(int value)
    {
      this.QueryOptions.Add(new QueryOption("$skip", value.ToString()));
      return (IDriveItemsCollectionRequest) this;
    }

    public IDriveItemsCollectionRequest OrderBy(string value)
    {
      this.QueryOptions.Add(new QueryOption("$orderby", value));
      return (IDriveItemsCollectionRequest) this;
    }
  }
}
