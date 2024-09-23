// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.IItemThumbnailsCollectionRequest
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.OneDrive.Sdk
{
  public interface IItemThumbnailsCollectionRequest : IBaseRequest
  {
    Task<ThumbnailSet> AddAsync(ThumbnailSet thumbnailSet);

    Task<ThumbnailSet> AddAsync(ThumbnailSet thumbnailSet, CancellationToken cancellationToken);

    Task<IItemThumbnailsCollectionPage> GetAsync();

    Task<IItemThumbnailsCollectionPage> GetAsync(CancellationToken cancellationToken);

    IItemThumbnailsCollectionRequest Expand(string value);

    IItemThumbnailsCollectionRequest Select(string value);

    IItemThumbnailsCollectionRequest Top(int value);

    IItemThumbnailsCollectionRequest Filter(string value);

    IItemThumbnailsCollectionRequest Skip(int value);

    IItemThumbnailsCollectionRequest OrderBy(string value);
  }
}
