// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IGroupEventsCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IGroupEventsCollectionRequest : IBaseRequest
  {
    Task<Event> AddAsync(Event eventsEvent);

    Task<Event> AddAsync(Event eventsEvent, CancellationToken cancellationToken);

    Task<IGroupEventsCollectionPage> GetAsync();

    Task<IGroupEventsCollectionPage> GetAsync(CancellationToken cancellationToken);

    IGroupEventsCollectionRequest Expand(string value);

    IGroupEventsCollectionRequest Select(string value);

    IGroupEventsCollectionRequest Top(int value);

    IGroupEventsCollectionRequest Filter(string value);

    IGroupEventsCollectionRequest Skip(int value);

    IGroupEventsCollectionRequest OrderBy(string value);
  }
}
