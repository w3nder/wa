// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.ICalendarEventsCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface ICalendarEventsCollectionRequest : IBaseRequest
  {
    Task<Event> AddAsync(Event eventsEvent);

    Task<Event> AddAsync(Event eventsEvent, CancellationToken cancellationToken);

    Task<ICalendarEventsCollectionPage> GetAsync();

    Task<ICalendarEventsCollectionPage> GetAsync(CancellationToken cancellationToken);

    ICalendarEventsCollectionRequest Expand(string value);

    ICalendarEventsCollectionRequest Select(string value);

    ICalendarEventsCollectionRequest Top(int value);

    ICalendarEventsCollectionRequest Filter(string value);

    ICalendarEventsCollectionRequest Skip(int value);

    ICalendarEventsCollectionRequest OrderBy(string value);
  }
}
