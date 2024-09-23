// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IDriveItemRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IDriveItemRequest : IBaseRequest
  {
    Task<DriveItem> CreateAsync(DriveItem driveItemToCreate);

    Task<DriveItem> CreateAsync(DriveItem driveItemToCreate, CancellationToken cancellationToken);

    Task DeleteAsync();

    Task DeleteAsync(CancellationToken cancellationToken);

    Task<DriveItem> GetAsync();

    Task<DriveItem> GetAsync(CancellationToken cancellationToken);

    Task<DriveItem> UpdateAsync(DriveItem driveItemToUpdate);

    Task<DriveItem> UpdateAsync(DriveItem driveItemToUpdate, CancellationToken cancellationToken);

    IDriveItemRequest Expand(string value);

    IDriveItemRequest Select(string value);
  }
}
