// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.ItemPermissionsCollectionRequest
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
  public class ItemPermissionsCollectionRequest : 
    BaseRequest,
    IItemPermissionsCollectionRequest,
    IBaseRequest
  {
    public ItemPermissionsCollectionRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
      this.SdkVersionHeaderPrefix = "onedrive";
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

    public Task<IItemPermissionsCollectionPage> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<IItemPermissionsCollectionPage> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      ItemPermissionsCollectionResponse collectionResponse = await this.SendAsync<ItemPermissionsCollectionResponse>((object) null, cancellationToken).ConfigureAwait(false);
      if (collectionResponse == null || collectionResponse.Value == null || collectionResponse.Value.CurrentPage == null)
        return (IItemPermissionsCollectionPage) null;
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

    public IItemPermissionsCollectionRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IItemPermissionsCollectionRequest) this;
    }

    public IItemPermissionsCollectionRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IItemPermissionsCollectionRequest) this;
    }

    public IItemPermissionsCollectionRequest Top(int value)
    {
      this.QueryOptions.Add(new QueryOption("$top", value.ToString()));
      return (IItemPermissionsCollectionRequest) this;
    }

    public IItemPermissionsCollectionRequest Filter(string value)
    {
      this.QueryOptions.Add(new QueryOption("$filter", value));
      return (IItemPermissionsCollectionRequest) this;
    }

    public IItemPermissionsCollectionRequest Skip(int value)
    {
      this.QueryOptions.Add(new QueryOption("$skip", value.ToString()));
      return (IItemPermissionsCollectionRequest) this;
    }

    public IItemPermissionsCollectionRequest OrderBy(string value)
    {
      this.QueryOptions.Add(new QueryOption("$orderby", value));
      return (IItemPermissionsCollectionRequest) this;
    }
  }
}
