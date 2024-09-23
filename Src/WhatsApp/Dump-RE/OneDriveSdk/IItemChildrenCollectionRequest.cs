// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.IItemChildrenCollectionRequest
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.OneDrive.Sdk
{
  public interface IItemChildrenCollectionRequest : IBaseRequest
  {
    Task<Item> AddAsync(Item item);

    Task<Item> AddAsync(Item item, CancellationToken cancellationToken);

    Task<IItemChildrenCollectionPage> GetAsync();

    Task<IItemChildrenCollectionPage> GetAsync(CancellationToken cancellationToken);

    IItemChildrenCollectionRequest Expand(string value);

    IItemChildrenCollectionRequest Select(string value);

    IItemChildrenCollectionRequest Top(int value);

    IItemChildrenCollectionRequest Filter(string value);

    IItemChildrenCollectionRequest Skip(int value);

    IItemChildrenCollectionRequest OrderBy(string value);
  }
}
