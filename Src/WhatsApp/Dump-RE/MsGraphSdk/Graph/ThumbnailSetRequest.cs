// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.ThumbnailSetRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class ThumbnailSetRequest : BaseRequest, IThumbnailSetRequest, IBaseRequest
  {
    public ThumbnailSetRequest(string requestUrl, IBaseClient client, IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
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
