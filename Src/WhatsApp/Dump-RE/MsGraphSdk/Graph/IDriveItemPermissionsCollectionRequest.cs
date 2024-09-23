// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IDriveItemPermissionsCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IDriveItemPermissionsCollectionRequest : IBaseRequest
  {
    Task<Permission> AddAsync(Permission permission);

    Task<Permission> AddAsync(Permission permission, CancellationToken cancellationToken);

    Task<IDriveItemPermissionsCollectionPage> GetAsync();

    Task<IDriveItemPermissionsCollectionPage> GetAsync(CancellationToken cancellationToken);

    IDriveItemPermissionsCollectionRequest Expand(string value);

    IDriveItemPermissionsCollectionRequest Select(string value);

    IDriveItemPermissionsCollectionRequest Top(int value);

    IDriveItemPermissionsCollectionRequest Filter(string value);

    IDriveItemPermissionsCollectionRequest Skip(int value);

    IDriveItemPermissionsCollectionRequest OrderBy(string value);
  }
}
