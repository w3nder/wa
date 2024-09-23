// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IDriveItemChildrenCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IDriveItemChildrenCollectionRequest : IBaseRequest
  {
    Task<DriveItem> AddAsync(DriveItem driveItem);

    Task<DriveItem> AddAsync(DriveItem driveItem, CancellationToken cancellationToken);

    Task<IDriveItemChildrenCollectionPage> GetAsync();

    Task<IDriveItemChildrenCollectionPage> GetAsync(CancellationToken cancellationToken);

    IDriveItemChildrenCollectionRequest Expand(string value);

    IDriveItemChildrenCollectionRequest Select(string value);

    IDriveItemChildrenCollectionRequest Top(int value);

    IDriveItemChildrenCollectionRequest Filter(string value);

    IDriveItemChildrenCollectionRequest Skip(int value);

    IDriveItemChildrenCollectionRequest OrderBy(string value);
  }
}
