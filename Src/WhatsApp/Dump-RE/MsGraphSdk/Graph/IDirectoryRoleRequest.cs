// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IDirectoryRoleRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IDirectoryRoleRequest : IBaseRequest
  {
    Task<DirectoryRole> CreateAsync(DirectoryRole directoryRoleToCreate);

    Task<DirectoryRole> CreateAsync(
      DirectoryRole directoryRoleToCreate,
      CancellationToken cancellationToken);

    Task DeleteAsync();

    Task DeleteAsync(CancellationToken cancellationToken);

    Task<DirectoryRole> GetAsync();

    Task<DirectoryRole> GetAsync(CancellationToken cancellationToken);

    Task<DirectoryRole> UpdateAsync(DirectoryRole directoryRoleToUpdate);

    Task<DirectoryRole> UpdateAsync(
      DirectoryRole directoryRoleToUpdate,
      CancellationToken cancellationToken);
  }
}
