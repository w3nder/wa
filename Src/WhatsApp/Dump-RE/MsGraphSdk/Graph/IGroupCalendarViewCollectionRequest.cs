// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IGroupCalendarViewCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IGroupCalendarViewCollectionRequest : IBaseRequest
  {
    Task<Event> AddAsync(Event calendarViewEvent);

    Task<Event> AddAsync(Event calendarViewEvent, CancellationToken cancellationToken);

    Task<IGroupCalendarViewCollectionPage> GetAsync();

    Task<IGroupCalendarViewCollectionPage> GetAsync(CancellationToken cancellationToken);

    IGroupCalendarViewCollectionRequest Expand(string value);

    IGroupCalendarViewCollectionRequest Select(string value);

    IGroupCalendarViewCollectionRequest Top(int value);

    IGroupCalendarViewCollectionRequest Filter(string value);

    IGroupCalendarViewCollectionRequest Skip(int value);

    IGroupCalendarViewCollectionRequest OrderBy(string value);
  }
}
