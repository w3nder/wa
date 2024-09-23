// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IDriveItemsCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IDriveItemsCollectionRequest : IBaseRequest
  {
    Task<DriveItem> AddAsync(DriveItem driveItem);

    Task<DriveItem> AddAsync(DriveItem driveItem, CancellationToken cancellationToken);

    Task<IDriveItemsCollectionPage> GetAsync();

    Task<IDriveItemsCollectionPage> GetAsync(CancellationToken cancellationToken);

    IDriveItemsCollectionRequest Expand(string value);

    IDriveItemsCollectionRequest Select(string value);

    IDriveItemsCollectionRequest Top(int value);

    IDriveItemsCollectionRequest Filter(string value);

    IDriveItemsCollectionRequest Skip(int value);

    IDriveItemsCollectionRequest OrderBy(string value);
  }
}
