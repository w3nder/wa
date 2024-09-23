// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.ShareRequest
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
  public class ShareRequest : BaseRequest, IShareRequest, IBaseRequest
  {
    public ShareRequest(string requestUrl, IBaseClient client, IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
      this.SdkVersionHeaderPrefix = "onedrive";
    }

    public Task<Share> CreateAsync(Share shareToCreate)
    {
      return this.CreateAsync(shareToCreate, CancellationToken.None);
    }

    public async Task<Share> CreateAsync(Share shareToCreate, CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PUT";
      Share shareToInitialize = await this.SendAsync<Share>((object) shareToCreate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(shareToInitialize);
      return shareToInitialize;
    }

    public Task DeleteAsync() => this.DeleteAsync(CancellationToken.None);

    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
      this.Method = "DELETE";
      Share share = await this.SendAsync<Share>((object) null, cancellationToken).ConfigureAwait(false);
    }

    public Task<Share> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<Share> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      Share shareToInitialize = await this.SendAsync<Share>((object) null, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(shareToInitialize);
      return shareToInitialize;
    }

    public Task<Share> UpdateAsync(Share shareToUpdate)
    {
      return this.UpdateAsync(shareToUpdate, CancellationToken.None);
    }

    public async Task<Share> UpdateAsync(Share shareToUpdate, CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PATCH";
      Share shareToInitialize = await this.SendAsync<Share>((object) shareToUpdate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(shareToInitialize);
      return shareToInitialize;
    }

    public IShareRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IShareRequest) this;
    }

    public IShareRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IShareRequest) this;
    }

    private void InitializeCollectionProperties(Share shareToInitialize)
    {
      if (shareToInitialize == null || shareToInitialize.AdditionalData == null || shareToInitialize.Items == null || shareToInitialize.Items.CurrentPage == null)
        return;
      shareToInitialize.Items.AdditionalData = shareToInitialize.AdditionalData;
      object obj;
      shareToInitialize.AdditionalData.TryGetValue("items@odata.nextLink", out obj);
      string nextPageLinkString = obj as string;
      if (string.IsNullOrEmpty(nextPageLinkString))
        return;
      shareToInitialize.Items.InitializeNextPageRequest(this.Client, nextPageLinkString);
    }
  }
}
