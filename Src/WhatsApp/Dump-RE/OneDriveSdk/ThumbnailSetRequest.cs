// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.ThumbnailSetRequest
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
  public class ThumbnailSetRequest : BaseRequest, IThumbnailSetRequest, IBaseRequest
  {
    public ThumbnailSetRequest(string requestUrl, IBaseClient client, IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
      this.SdkVersionHeaderPrefix = "onedrive";
    }

    public Task<ThumbnailSet> CreateAsync(ThumbnailSet thumbnailSetToCreate)
    {
      return this.CreateAsync(thumbnailSetToCreate, CancellationToken.None);
    }

    public async Task<ThumbnailSet> CreateAsync(
      ThumbnailSet thumbnailSetToCreate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PUT";
      ThumbnailSet thumbnailSetToInitialize = await this.SendAsync<ThumbnailSet>((object) thumbnailSetToCreate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(thumbnailSetToInitialize);
      return thumbnailSetToInitialize;
    }

    public Task DeleteAsync() => this.DeleteAsync(CancellationToken.None);

    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
      this.Method = "DELETE";
      ThumbnailSet thumbnailSet = await this.SendAsync<ThumbnailSet>((object) null, cancellationToken).ConfigureAwait(false);
    }

    public Task<ThumbnailSet> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<ThumbnailSet> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      ThumbnailSet thumbnailSetToInitialize = await this.SendAsync<ThumbnailSet>((object) null, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(thumbnailSetToInitialize);
      return thumbnailSetToInitialize;
    }

    public Task<ThumbnailSet> UpdateAsync(ThumbnailSet thumbnailSetToUpdate)
    {
      return this.UpdateAsync(thumbnailSetToUpdate, CancellationToken.None);
    }

    public async Task<ThumbnailSet> UpdateAsync(
      ThumbnailSet thumbnailSetToUpdate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PATCH";
      ThumbnailSet thumbnailSetToInitialize = await this.SendAsync<ThumbnailSet>((object) thumbnailSetToUpdate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(thumbnailSetToInitialize);
      return thumbnailSetToInitialize;
    }

    public IThumbnailSetRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IThumbnailSetRequest) this;
    }

    public IThumbnailSetRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IThumbnailSetRequest) this;
    }

    private void InitializeCollectionProperties(ThumbnailSet thumbnailSetToInitialize)
    {
    }
  }
}
