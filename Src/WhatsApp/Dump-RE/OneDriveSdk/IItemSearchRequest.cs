// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.IItemSearchRequest
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.OneDrive.Sdk
{
  public interface IItemSearchRequest : IBaseRequest
  {
    Task<IItemSearchCollectionPage> GetAsync();

    Task<IItemSearchCollectionPage> GetAsync(CancellationToken cancellationToken);

    IItemSearchRequest Expand(string value);

    IItemSearchRequest Select(string value);

    IItemSearchRequest Top(int value);

    IItemSearchRequest Filter(string value);

    IItemSearchRequest Skip(int value);

    IItemSearchRequest OrderBy(string value);
  }
}
