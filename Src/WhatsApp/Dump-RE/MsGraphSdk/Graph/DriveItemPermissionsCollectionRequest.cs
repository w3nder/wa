// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.DriveItemPermissionsCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class DriveItemPermissionsCollectionRequest : 
    BaseRequest,
    IDriveItemPermissionsCollectionRequest,
    IBaseRequest
  {
    public DriveItemPermissionsCollectionRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<Permission> AddAsync(Permission permission)
    {
      return this.AddAsync(permission, CancellationToken.None);
    }

    public Task<Permission> AddAsync(Permission permission, CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "POST";
      return this.SendAsync<Permission>((object) permission, cancellationToken);
    }

    public Task<IDriveItemPermissionsCollectionPage> GetAsync()
    {
      return this.GetAsync(CancellationToken.None);
    }

    public async Task<IDriveItemPermissionsCollectionPage> GetAsync(
      CancellationToken cancellationToken)
    {
      this.Method = "GET";
      DriveItemPermissionsCollectionResponse collectionResponse = await this.SendAsync<DriveItemPermissionsCollectionResponse>((object) null, cancellationToken).ConfigureAwait(false);
      if (collectionResponse == null || collectionResponse.Value == null || collectionResponse.Value.CurrentPage == null)
        return (IDriveItemPermissionsCollectionPage) null;
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

    public IDriveItemPermissionsCollectionRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IDriveItemPermissionsCollectionRequest) this;
    }

    public IDriveItemPermissionsCollectionRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IDriveItemPermissionsCollectionRequest) this;
    }

    public IDriveItemPermissionsCollectionRequest Top(int value)
    {
      this.QueryOptions.Add(new QueryOption("$top", value.ToString()));
      return (IDriveItemPermissionsCollectionRequest) this;
    }

    public IDriveItemPermissionsCollectionRequest Filter(string value)
    {
      this.QueryOptions.Add(new QueryOption("$filter", value));
      return (IDriveItemPermissionsCollectionRequest) this;
    }

    public IDriveItemPermissionsCollectionRequest Skip(int value)
    {
      this.QueryOptions.Add(new QueryOption("$skip", value.ToString()));
      return (IDriveItemPermissionsCollectionRequest) this;
    }

    public IDriveItemPermissionsCollectionRequest OrderBy(string value)
    {
      this.QueryOptions.Add(new QueryOption("$orderby", value));
      return (IDriveItemPermissionsCollectionRequest) this;
    }
  }
}
