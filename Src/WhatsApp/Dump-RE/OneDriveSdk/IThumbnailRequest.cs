// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.IThumbnailRequest
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.OneDrive.Sdk
{
  public interface IThumbnailRequest : IBaseRequest
  {
    Task<Thumbnail> CreateAsync(Thumbnail thumbnailToCreate);

    Task<Thumbnail> CreateAsync(Thumbnail thumbnailToCreate, CancellationToken cancellationToken);

    Task DeleteAsync();

    Task DeleteAsync(CancellationToken cancellationToken);

    Task<Thumbnail> GetAsync();

    Task<Thumbnail> GetAsync(CancellationToken cancellationToken);

    Task<Thumbnail> UpdateAsync(Thumbnail thumbnailToUpdate);

    Task<Thumbnail> UpdateAsync(Thumbnail thumbnailToUpdate, CancellationToken cancellationToken);

    IThumbnailRequest Expand(string value);

    IThumbnailRequest Select(string value);
  }
}
