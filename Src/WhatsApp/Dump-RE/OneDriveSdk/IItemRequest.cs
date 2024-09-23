// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.IItemRequest
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.OneDrive.Sdk
{
  public interface IItemRequest : IBaseRequest
  {
    Task<Item> CreateAsync(Item itemToCreate);

    Task<Item> CreateAsync(Item itemToCreate, CancellationToken cancellationToken);

    Task DeleteAsync();

    Task DeleteAsync(CancellationToken cancellationToken);

    Task<Item> GetAsync();

    Task<Item> GetAsync(CancellationToken cancellationToken);

    Task<Item> UpdateAsync(Item itemToUpdate);

    Task<Item> UpdateAsync(Item itemToUpdate, CancellationToken cancellationToken);

    IItemRequest Expand(string value);

    IItemRequest Select(string value);
  }
}
