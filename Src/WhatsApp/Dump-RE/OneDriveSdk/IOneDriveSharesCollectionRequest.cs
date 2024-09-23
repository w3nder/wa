// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.IOneDriveSharesCollectionRequest
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.OneDrive.Sdk
{
  public interface IOneDriveSharesCollectionRequest : IBaseRequest
  {
    Task<Share> AddAsync(Share share);

    Task<Share> AddAsync(Share share, CancellationToken cancellationToken);

    Task<IOneDriveSharesCollectionPage> GetAsync();

    Task<IOneDriveSharesCollectionPage> GetAsync(CancellationToken cancellationToken);

    IOneDriveSharesCollectionRequest Expand(string value);

    IOneDriveSharesCollectionRequest Select(string value);

    IOneDriveSharesCollectionRequest Top(int value);

    IOneDriveSharesCollectionRequest Filter(string value);

    IOneDriveSharesCollectionRequest Skip(int value);

    IOneDriveSharesCollectionRequest OrderBy(string value);
  }
}
