// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IDriveItemThumbnailsCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IDriveItemThumbnailsCollectionRequest : IBaseRequest
  {
    Task<ThumbnailSet> AddAsync(ThumbnailSet thumbnailSet);

    Task<ThumbnailSet> AddAsync(ThumbnailSet thumbnailSet, CancellationToken cancellationToken);

    Task<IDriveItemThumbnailsCollectionPage> GetAsync();

    Task<IDriveItemThumbnailsCollectionPage> GetAsync(CancellationToken cancellationToken);

    IDriveItemThumbnailsCollectionRequest Expand(string value);

    IDriveItemThumbnailsCollectionRequest Select(string value);

    IDriveItemThumbnailsCollectionRequest Top(int value);

    IDriveItemThumbnailsCollectionRequest Filter(string value);

    IDriveItemThumbnailsCollectionRequest Skip(int value);

    IDriveItemThumbnailsCollectionRequest OrderBy(string value);
  }
}
