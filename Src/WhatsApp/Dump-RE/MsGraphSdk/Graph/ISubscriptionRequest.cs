// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.ISubscriptionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface ISubscriptionRequest : IBaseRequest
  {
    Task<Subscription> CreateAsync(Subscription subscriptionToCreate);

    Task<Subscription> CreateAsync(
      Subscription subscriptionToCreate,
      CancellationToken cancellationToken);

    Task DeleteAsync();

    Task DeleteAsync(CancellationToken cancellationToken);

    Task<Subscription> GetAsync();

    Task<Subscription> GetAsync(CancellationToken cancellationToken);

    Task<Subscription> UpdateAsync(Subscription subscriptionToUpdate);

    Task<Subscription> UpdateAsync(
      Subscription subscriptionToUpdate,
      CancellationToken cancellationToken);
  }
}
