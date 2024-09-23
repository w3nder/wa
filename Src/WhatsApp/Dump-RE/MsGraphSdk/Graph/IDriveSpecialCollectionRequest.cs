// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IDriveSpecialCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IDriveSpecialCollectionRequest : IBaseRequest
  {
    Task<DriveItem> AddAsync(DriveItem driveItem);

    Task<DriveItem> AddAsync(DriveItem driveItem, CancellationToken cancellationToken);

    Task<IDriveSpecialCollectionPage> GetAsync();

    Task<IDriveSpecialCollectionPage> GetAsync(CancellationToken cancellationToken);

    IDriveSpecialCollectionRequest Expand(string value);

    IDriveSpecialCollectionRequest Select(string value);

    IDriveSpecialCollectionRequest Top(int value);

    IDriveSpecialCollectionRequest Filter(string value);

    IDriveSpecialCollectionRequest Skip(int value);

    IDriveSpecialCollectionRequest OrderBy(string value);
  }
}
