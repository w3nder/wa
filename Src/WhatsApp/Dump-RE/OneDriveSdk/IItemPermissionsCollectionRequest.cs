// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.IItemPermissionsCollectionRequest
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.OneDrive.Sdk
{
  public interface IItemPermissionsCollectionRequest : IBaseRequest
  {
    Task<Permission> AddAsync(Permission permission);

    Task<Permission> AddAsync(Permission permission, CancellationToken cancellationToken);

    Task<IItemPermissionsCollectionPage> GetAsync();

    Task<IItemPermissionsCollectionPage> GetAsync(CancellationToken cancellationToken);

    IItemPermissionsCollectionRequest Expand(string value);

    IItemPermissionsCollectionRequest Select(string value);

    IItemPermissionsCollectionRequest Top(int value);

    IItemPermissionsCollectionRequest Filter(string value);

    IItemPermissionsCollectionRequest Skip(int value);

    IItemPermissionsCollectionRequest OrderBy(string value);
  }
}
