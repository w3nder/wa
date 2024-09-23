// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.IDriveRequest
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.OneDrive.Sdk
{
  public interface IDriveRequest : IBaseRequest
  {
    Task<Drive> CreateAsync(Drive driveToCreate);

    Task<Drive> CreateAsync(Drive driveToCreate, CancellationToken cancellationToken);

    Task DeleteAsync();

    Task DeleteAsync(CancellationToken cancellationToken);

    Task<Drive> GetAsync();

    Task<Drive> GetAsync(CancellationToken cancellationToken);

    Task<Drive> UpdateAsync(Drive driveToUpdate);

    Task<Drive> UpdateAsync(Drive driveToUpdate, CancellationToken cancellationToken);

    IDriveRequest Expand(string value);

    IDriveRequest Select(string value);
  }
}
