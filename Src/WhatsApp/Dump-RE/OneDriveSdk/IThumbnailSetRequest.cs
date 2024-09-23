// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.IThumbnailSetRequest
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.OneDrive.Sdk
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
