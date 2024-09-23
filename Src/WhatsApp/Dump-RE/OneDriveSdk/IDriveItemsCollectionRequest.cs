// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.IDriveItemsCollectionRequest
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.OneDrive.Sdk
{
  public interface IDriveItemsCollectionRequest : IBaseRequest
  {
    Task<Item> AddAsync(Item item);

    Task<Item> AddAsync(Item item, CancellationToken cancellationToken);

    Task<IDriveItemsCollectionPage> GetAsync();

    Task<IDriveItemsCollectionPage> GetAsync(CancellationToken cancellationToken);

    IDriveItemsCollectionRequest Expand(string value);

    IDriveItemsCollectionRequest Select(string value);

    IDriveItemsCollectionRequest Top(int value);

    IDriveItemsCollectionRequest Filter(string value);

    IDriveItemsCollectionRequest Skip(int value);

    IDriveItemsCollectionRequest OrderBy(string value);
  }
}
