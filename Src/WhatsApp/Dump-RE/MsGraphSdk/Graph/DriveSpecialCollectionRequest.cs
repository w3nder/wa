// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.DriveSpecialCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class DriveSpecialCollectionRequest : 
    BaseRequest,
    IDriveSpecialCollectionRequest,
    IBaseRequest
  {
    public DriveSpecialCollectionRequest(
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

    public Task<IDriveSpecialCollectionPage> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<IDriveSpecialCollectionPage> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      DriveSpecialCollectionResponse collectionResponse = await this.SendAsync<DriveSpecialCollectionResponse>((object) null, cancellationToken).ConfigureAwait(false);
      if (collectionResponse == null || collectionResponse.Value == null || collectionResponse.Value.CurrentPage == null)
        return (IDriveSpecialCollectionPage) null;
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

    public IDriveSpecialCollectionRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IDriveSpecialCollectionRequest) this;
    }

    public IDriveSpecialCollectionRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IDriveSpecialCollectionRequest) this;
    }

    public IDriveSpecialCollectionRequest Top(int value)
    {
      this.QueryOptions.Add(new QueryOption("$top", value.ToString()));
      return (IDriveSpecialCollectionRequest) this;
    }

    public IDriveSpecialCollectionRequest Filter(string value)
    {
      this.QueryOptions.Add(new QueryOption("$filter", value));
      return (IDriveSpecialCollectionRequest) this;
    }

    public IDriveSpecialCollectionRequest Skip(int value)
    {
      this.QueryOptions.Add(new QueryOption("$skip", value.ToString()));
      return (IDriveSpecialCollectionRequest) this;
    }

    public IDriveSpecialCollectionRequest OrderBy(string value)
    {
      this.QueryOptions.Add(new QueryOption("$orderby", value));
      return (IDriveSpecialCollectionRequest) this;
    }
  }
}
