// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IThumbnailSetRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IThumbnailSetRequest : IBaseRequest
  {
    Task<ThumbnailSet> CreateAsync(ThumbnailSet thumbnailSetToCreate);

    Task<ThumbnailSet> CreateAsync(
      ThumbnailSet thumbnailSetToCreate,
      CancellationToken cancellationToken);

    Task DeleteAsync();

    Task DeleteAsync(CancellationToken cancellationToken);

    Task<ThumbnailSet> GetAsync();

    Task<ThumbnailSet> GetAsync(CancellationToken cancellationToken);

    Task<ThumbnailSet> UpdateAsync(ThumbnailSet thumbnailSetToUpdate);

    Task<ThumbnailSet> UpdateAsync(
      ThumbnailSet thumbnailSetToUpdate,
      CancellationToken cancellationToken);

    IThumbnailSetRequest Expand(string value);

    IThumbnailSetRequest Select(string value);
  }
}
