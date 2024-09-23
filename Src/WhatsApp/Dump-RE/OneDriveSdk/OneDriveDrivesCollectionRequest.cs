// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.OneDriveDrivesCollectionRequest
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
  public class OneDriveDrivesCollectionRequest : 
    BaseRequest,
    IOneDriveDrivesCollectionRequest,
    IBaseRequest
  {
    public OneDriveDrivesCollectionRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
      this.SdkVersionHeaderPrefix = "onedrive";
    }

    public Task<Drive> AddAsync(Drive drive) => this.AddAsync(drive, CancellationToken.None);

    public Task<Drive> AddAsync(Drive drive, CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "POST";
      return this.SendAsync<Drive>((object) drive, cancellationToken);
    }

    public Task<IOneDriveDrivesCollectionPage> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<IOneDriveDrivesCollectionPage> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      OneDriveDrivesCollectionResponse collectionResponse = await this.SendAsync<OneDriveDrivesCollectionResponse>((object) null, cancellationToken).ConfigureAwait(false);
      if (collectionResponse == null || collectionResponse.Value == null || collectionResponse.Value.CurrentPage == null)
        return (IOneDriveDrivesCollectionPage) null;
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

    public IOneDriveDrivesCollectionRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IOneDriveDrivesCollectionRequest) this;
    }

    public IOneDriveDrivesCollectionRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IOneDriveDrivesCollectionRequest) this;
    }

    public IOneDriveDrivesCollectionRequest Top(int value)
    {
      this.QueryOptions.Add(new QueryOption("$top", value.ToString()));
      return (IOneDriveDrivesCollectionRequest) this;
    }

    public IOneDriveDrivesCollectionRequest Filter(string value)
    {
      this.QueryOptions.Add(new QueryOption("$filter", value));
      return (IOneDriveDrivesCollectionRequest) this;
    }

    public IOneDriveDrivesCollectionRequest Skip(int value)
    {
      this.QueryOptions.Add(new QueryOption("$skip", value.ToString()));
      return (IOneDriveDrivesCollectionRequest) this;
    }

    public IOneDriveDrivesCollectionRequest OrderBy(string value)
    {
      this.QueryOptions.Add(new QueryOption("$orderby", value));
      return (IOneDriveDrivesCollectionRequest) this;
    }
  }
}
