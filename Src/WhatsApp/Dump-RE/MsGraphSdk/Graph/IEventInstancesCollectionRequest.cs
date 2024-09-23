// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IEventInstancesCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IEventInstancesCollectionRequest : IBaseRequest
  {
    Task<Event> AddAsync(Event instancesEvent);

    Task<Event> AddAsync(Event instancesEvent, CancellationToken cancellationToken);

    Task<IEventInstancesCollectionPage> GetAsync();

    Task<IEventInstancesCollectionPage> GetAsync(CancellationToken cancellationToken);

    IEventInstancesCollectionRequest Expand(string value);

    IEventInstancesCollectionRequest Select(string value);

    IEventInstancesCollectionRequest Top(int value);

    IEventInstancesCollectionRequest Filter(string value);

    IEventInstancesCollectionRequest Skip(int value);

    IEventInstancesCollectionRequest OrderBy(string value);
  }
}
