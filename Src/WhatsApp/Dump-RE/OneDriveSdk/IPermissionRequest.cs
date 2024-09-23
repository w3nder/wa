// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.IPermissionRequest
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.OneDrive.Sdk
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
