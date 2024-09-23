// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.IDriveSpecialCollectionRequest
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.OneDrive.Sdk
{
  public interface IDriveSpecialCollectionRequest : IBaseRequest
  {
    Task<Item> AddAsync(Item item);

    Task<Item> AddAsync(Item item, CancellationToken cancellationToken);

    Task<IDriveSpecialCollectionPage> GetAsync();

    Task<IDriveSpecialCollectionPage> GetAsync(CancellationToken cancellationToken);

    IDriveSpecialCollectionRequest Expand(string value);

    IDriveSpecialCollectionRequest Select(string value);

    IDriveSpecialCollectionRequest Top(int value);

    IDriveSpecialCollectionRequest Filter(string value);

    IDriveSpecialCollectionRequest Skip(int value);

    IDriveSpecialCollectionRequest OrderBy(string value);
  }
}
