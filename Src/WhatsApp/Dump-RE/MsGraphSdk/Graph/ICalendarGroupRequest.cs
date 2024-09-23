// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.ICalendarGroupRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface ICalendarGroupRequest : IBaseRequest
  {
    Task<CalendarGroup> CreateAsync(CalendarGroup calendarGroupToCreate);

    Task<CalendarGroup> CreateAsync(
      CalendarGroup calendarGroupToCreate,
      CancellationToken cancellationToken);

    Task DeleteAsync();

    Task DeleteAsync(CancellationToken cancellationToken);

    Task<CalendarGroup> GetAsync();

    Task<CalendarGroup> GetAsync(CancellationToken cancellationToken);

    Task<CalendarGroup> UpdateAsync(CalendarGroup calendarGroupToUpdate);

    Task<CalendarGroup> UpdateAsync(
      CalendarGroup calendarGroupToUpdate,
      CancellationToken cancellationToken);

    ICalendarGroupRequest Expand(string value);

    ICalendarGroupRequest Select(string value);
  }
}
