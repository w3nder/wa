// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.IDriveSharedCollectionRequest
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.OneDrive.Sdk
{
  public interface IDriveSharedCollectionRequest : IBaseRequest
  {
    Task<Item> AddAsync(Item item);

    Task<Item> AddAsync(Item item, CancellationToken cancellationToken);

    Task<IDriveSharedCollectionPage> GetAsync();

    Task<IDriveSharedCollectionPage> GetAsync(CancellationToken cancellationToken);

    IDriveSharedCollectionRequest Expand(string value);

    IDriveSharedCollectionRequest Select(string value);

    IDriveSharedCollectionRequest Top(int value);

    IDriveSharedCollectionRequest Filter(string value);

    IDriveSharedCollectionRequest Skip(int value);

    IDriveSharedCollectionRequest OrderBy(string value);
  }
}
