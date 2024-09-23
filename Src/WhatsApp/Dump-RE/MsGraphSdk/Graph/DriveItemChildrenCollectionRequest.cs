// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.DriveItemChildrenCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class DriveItemChildrenCollectionRequest : 
    BaseRequest,
    IDriveItemChildrenCollectionRequest,
    IBaseRequest
  {
    public DriveItemChildrenCollectionRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<DriveItem> AddAsync(DriveItem driveItem)
    {
      return this.AddAsync(driveItem, CancellationToken.None);
    }

    public Task<DriveItem> AddAsync(DriveItem driveItem, CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "POST";
      return this.SendAsync<DriveItem>((object) driveItem, cancellationToken);
    }

    public Task<IDriveItemChildrenCollectionPage> GetAsync()
    {
      return this.GetAsync(CancellationToken.None);
    }

    public async Task<IDriveItemChildrenCollectionPage> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      DriveItemChildrenCollectionResponse collectionResponse = await this.SendAsync<DriveItemChildrenCollectionResponse>((object) null, cancellationToken).ConfigureAwait(false);
      if (collectionResponse == null || collectionResponse.Value == null || collectionResponse.Value.CurrentPage == null)
        return (IDriveItemChildrenCollectionPage) null;
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

    public IDriveItemChildrenCollectionRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IDriveItemChildrenCollectionRequest) this;
    }

    public IDriveItemChildrenCollectionRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IDriveItemChildrenCollectionRequest) this;
    }

    public IDriveItemChildrenCollectionRequest Top(int value)
    {
      this.QueryOptions.Add(new QueryOption("$top", value.ToString()));
      return (IDriveItemChildrenCollectionRequest) this;
    }

    public IDriveItemChildrenCollectionRequest Filter(string value)
    {
      this.QueryOptions.Add(new QueryOption("$filter", value));
      return (IDriveItemChildrenCollectionRequest) this;
    }

    public IDriveItemChildrenCollectionRequest Skip(int value)
    {
      this.QueryOptions.Add(new QueryOption("$skip", value.ToString()));
      return (IDriveItemChildrenCollectionRequest) this;
    }

    public IDriveItemChildrenCollectionRequest OrderBy(string value)
    {
      this.QueryOptions.Add(new QueryOption("$orderby", value));
      return (IDriveItemChildrenCollectionRequest) this;
    }
  }
}
