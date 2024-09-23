// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IUserCalendarGroupsCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IUserCalendarGroupsCollectionRequest : IBaseRequest
  {
    Task<CalendarGroup> AddAsync(CalendarGroup calendarGroup);

    Task<CalendarGroup> AddAsync(CalendarGroup calendarGroup, CancellationToken cancellationToken);

    Task<IUserCalendarGroupsCollectionPage> GetAsync();

    Task<IUserCalendarGroupsCollectionPage> GetAsync(CancellationToken cancellationToken);

    IUserCalendarGroupsCollectionRequest Expand(string value);

    IUserCalendarGroupsCollectionRequest Select(string value);

    IUserCalendarGroupsCollectionRequest Top(int value);

    IUserCalendarGroupsCollectionRequest Filter(string value);

    IUserCalendarGroupsCollectionRequest Skip(int value);

    IUserCalendarGroupsCollectionRequest OrderBy(string value);
  }
}
