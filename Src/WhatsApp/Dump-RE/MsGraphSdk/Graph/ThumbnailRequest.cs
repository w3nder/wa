﻿// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.ThumbnailRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class ThumbnailRequest : BaseRequest, IThumbnailRequest, IBaseRequest
  {
    public ThumbnailRequest(string requestUrl, IBaseClient client, IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<Thumbnail> CreateAsync(Thumbnail thumbnailToCreate)
    {
      return this.CreateAsync(thumbnailToCreate, CancellationToken.None);
    }

    public async Task<Thumbnail> CreateAsync(
      Thumbnail thumbnailToCreate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PUT";
      Thumbnail thumbnailToInitialize = await this.SendAsync<Thumbnail>((object) thumbnailToCreate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(thumbnailToInitialize);
      return thumbnailToInitialize;
    }

    public Task DeleteAsync() => this.DeleteAsync(CancellationToken.None);

    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
      this.Method = "DELETE";
      Thumbnail thumbnail = await this.SendAsync<Thumbnail>((object) null, cancellationToken).ConfigureAwait(false);
    }

    public Task<Thumbnail> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<Thumbnail> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      Thumbnail thumbnailToInitialize = await this.SendAsync<Thumbnail>((object) null, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(thumbnailToInitialize);
      return thumbnailToInitialize;
    }

    public Task<Thumbnail> UpdateAsync(Thumbnail thumbnailToUpdate)
    {
      return this.UpdateAsync(thumbnailToUpdate, CancellationToken.None);
    }

    public async Task<Thumbnail> UpdateAsync(
      Thumbnail thumbnailToUpdate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PATCH";
      Thumbnail thumbnailToInitialize = await this.SendAsync<Thumbnail>((object) thumbnailToUpdate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(thumbnailToInitialize);
      return thumbnailToInitialize;
    }

    public IThumbnailRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IThumbnailRequest) this;
    }

    public IThumbnailRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IThumbnailRequest) this;
    }

    private void InitializeCollectionProperties(Thumbnail thumbnailToInitialize)
    {
    }
  }
}
