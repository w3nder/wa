// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.IItemVersionsCollectionRequest
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.OneDrive.Sdk
{
  public interface IItemVersionsCollectionRequest : IBaseRequest
  {
    Task<Item> AddAsync(Item item);

    Task<Item> AddAsync(Item item, CancellationToken cancellationToken);

    Task<IItemVersionsCollectionPage> GetAsync();

    Task<IItemVersionsCollectionPage> GetAsync(CancellationToken cancellationToken);

    IItemVersionsCollectionRequest Expand(string value);

    IItemVersionsCollectionRequest Select(string value);

    IItemVersionsCollectionRequest Top(int value);

    IItemVersionsCollectionRequest Filter(string value);

    IItemVersionsCollectionRequest Skip(int value);

    IItemVersionsCollectionRequest OrderBy(string value);
  }
}
