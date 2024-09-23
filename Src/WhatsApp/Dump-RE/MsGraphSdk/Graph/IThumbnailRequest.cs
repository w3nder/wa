// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IThumbnailRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
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
