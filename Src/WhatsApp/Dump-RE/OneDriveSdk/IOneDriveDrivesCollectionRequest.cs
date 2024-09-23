// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.IOneDriveDrivesCollectionRequest
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.OneDrive.Sdk
{
  public interface IOneDriveDrivesCollectionRequest : IBaseRequest
  {
    Task<Drive> AddAsync(Drive drive);

    Task<Drive> AddAsync(Drive drive, CancellationToken cancellationToken);

    Task<IOneDriveDrivesCollectionPage> GetAsync();

    Task<IOneDriveDrivesCollectionPage> GetAsync(CancellationToken cancellationToken);

    IOneDriveDrivesCollectionRequest Expand(string value);

    IOneDriveDrivesCollectionRequest Select(string value);

    IOneDriveDrivesCollectionRequest Top(int value);

    IOneDriveDrivesCollectionRequest Filter(string value);

    IOneDriveDrivesCollectionRequest Skip(int value);

    IOneDriveDrivesCollectionRequest OrderBy(string value);
  }
}
