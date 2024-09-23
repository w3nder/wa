// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.IShareItemsCollectionRequest
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.OneDrive.Sdk
{
  public interface IShareItemsCollectionRequest : IBaseRequest
  {
    Task<Item> AddAsync(Item item);

    Task<Item> AddAsync(Item item, CancellationToken cancellationToken);

    Task<IShareItemsCollectionPage> GetAsync();

    Task<IShareItemsCollectionPage> GetAsync(CancellationToken cancellationToken);

    IShareItemsCollectionRequest Expand(string value);

    IShareItemsCollectionRequest Select(string value);

    IShareItemsCollectionRequest Top(int value);

    IShareItemsCollectionRequest Filter(string value);

    IShareItemsCollectionRequest Skip(int value);

    IShareItemsCollectionRequest OrderBy(string value);
  }
}
