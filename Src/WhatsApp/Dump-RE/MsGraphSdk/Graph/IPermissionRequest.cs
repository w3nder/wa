// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IPermissionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IPermissionRequest : IBaseRequest
  {
    Task<Permission> CreateAsync(Permission permissionToCreate);

    Task<Permission> CreateAsync(Permission permissionToCreate, CancellationToken cancellationToken);

    Task DeleteAsync();

    Task DeleteAsync(CancellationToken cancellationToken);

    Task<Permission> GetAsync();

    Task<Permission> GetAsync(CancellationToken cancellationToken);

    Task<Permission> UpdateAsync(Permission permissionToUpdate);

    Task<Permission> UpdateAsync(Permission permissionToUpdate, CancellationToken cancellationToken);

    IPermissionRequest Expand(string value);

    IPermissionRequest Select(string value);
  }
}
