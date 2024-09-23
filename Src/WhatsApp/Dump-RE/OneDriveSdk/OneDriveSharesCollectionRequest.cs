// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.OneDriveSharesCollectionRequest
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
  public class OneDriveSharesCollectionRequest : 
    BaseRequest,
    IOneDriveSharesCollectionRequest,
    IBaseRequest
  {
    public OneDriveSharesCollectionRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
      this.SdkVersionHeaderPrefix = "onedrive";
    }

    public Task<Share> AddAsync(Share share) => this.AddAsync(share, CancellationToken.None);

    public Task<Share> AddAsync(Share share, CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "POST";
      return this.SendAsync<Share>((object) share, cancellationToken);
    }

    public Task<IOneDriveSharesCollectionPage> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<IOneDriveSharesCollectionPage> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      OneDriveSharesCollectionResponse collectionResponse = await this.SendAsync<OneDriveSharesCollectionResponse>((object) null, cancellationToken).ConfigureAwait(false);
      if (collectionResponse == null || collectionResponse.Value == null || collectionResponse.Value.CurrentPage == null)
        return (IOneDriveSharesCollectionPage) null;
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

    public IOneDriveSharesCollectionRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IOneDriveSharesCollectionRequest) this;
    }

    public IOneDriveSharesCollectionRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IOneDriveSharesCollectionRequest) this;
    }

    public IOneDriveSharesCollectionRequest Top(int value)
    {
      this.QueryOptions.Add(new QueryOption("$top", value.ToString()));
      return (IOneDriveSharesCollectionRequest) this;
    }

    public IOneDriveSharesCollectionRequest Filter(string value)
    {
      this.QueryOptions.Add(new QueryOption("$filter", value));
      return (IOneDriveSharesCollectionRequest) this;
    }

    public IOneDriveSharesCollectionRequest Skip(int value)
    {
      this.QueryOptions.Add(new QueryOption("$skip", value.ToString()));
      return (IOneDriveSharesCollectionRequest) this;
    }

    public IOneDriveSharesCollectionRequest OrderBy(string value)
    {
      this.QueryOptions.Add(new QueryOption("$orderby", value));
      return (IOneDriveSharesCollectionRequest) this;
    }
  }
}
