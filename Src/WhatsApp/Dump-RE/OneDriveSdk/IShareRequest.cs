// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.IShareRequest
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.OneDrive.Sdk
{
  public interface IShareRequest : IBaseRequest
  {
    Task<Share> CreateAsync(Share shareToCreate);

    Task<Share> CreateAsync(Share shareToCreate, CancellationToken cancellationToken);

    Task DeleteAsync();

    Task DeleteAsync(CancellationToken cancellationToken);

    Task<Share> GetAsync();

    Task<Share> GetAsync(CancellationToken cancellationToken);

    Task<Share> UpdateAsync(Share shareToUpdate);

    Task<Share> UpdateAsync(Share shareToUpdate, CancellationToken cancellationToken);

    IShareRequest Expand(string value);

    IShareRequest Select(string value);
  }
}
