// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.ICalendarGroupCalendarsCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface ICalendarGroupCalendarsCollectionRequest : IBaseRequest
  {
    Task<Calendar> AddAsync(Calendar calendar);

    Task<Calendar> AddAsync(Calendar calendar, CancellationToken cancellationToken);

    Task<ICalendarGroupCalendarsCollectionPage> GetAsync();

    Task<ICalendarGroupCalendarsCollectionPage> GetAsync(CancellationToken cancellationToken);

    ICalendarGroupCalendarsCollectionRequest Expand(string value);

    ICalendarGroupCalendarsCollectionRequest Select(string value);

    ICalendarGroupCalendarsCollectionRequest Top(int value);

    ICalendarGroupCalendarsCollectionRequest Filter(string value);

    ICalendarGroupCalendarsCollectionRequest Skip(int value);

    ICalendarGroupCalendarsCollectionRequest OrderBy(string value);
  }
}
