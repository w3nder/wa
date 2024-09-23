// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.IItemDeltaRequest
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.OneDrive.Sdk
{
  public interface IItemDeltaRequest : IBaseRequest
  {
    Task<IItemDeltaCollectionPage> GetAsync();

    Task<IItemDeltaCollectionPage> GetAsync(CancellationToken cancellationToken);

    IItemDeltaRequest Expand(string value);

    IItemDeltaRequest Select(string value);

    IItemDeltaRequest Top(int value);

    IItemDeltaRequest Filter(string value);

    IItemDeltaRequest Skip(int value);

    IItemDeltaRequest OrderBy(string value);
  }
}
