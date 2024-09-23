// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.ICalendarCalendarViewCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface ICalendarCalendarViewCollectionRequest : IBaseRequest
  {
    Task<Event> AddAsync(Event calendarViewEvent);

    Task<Event> AddAsync(Event calendarViewEvent, CancellationToken cancellationToken);

    Task<ICalendarCalendarViewCollectionPage> GetAsync();

    Task<ICalendarCalendarViewCollectionPage> GetAsync(CancellationToken cancellationToken);

    ICalendarCalendarViewCollectionRequest Expand(string value);

    ICalendarCalendarViewCollectionRequest Select(string value);

    ICalendarCalendarViewCollectionRequest Top(int value);

    ICalendarCalendarViewCollectionRequest Filter(string value);

    ICalendarCalendarViewCollectionRequest Skip(int value);

    ICalendarCalendarViewCollectionRequest OrderBy(string value);
  }
}
