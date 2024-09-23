// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IUserCalendarViewCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IUserCalendarViewCollectionRequest : IBaseRequest
  {
    Task<Event> AddAsync(Event calendarViewEvent);

    Task<Event> AddAsync(Event calendarViewEvent, CancellationToken cancellationToken);

    Task<IUserCalendarViewCollectionPage> GetAsync();

    Task<IUserCalendarViewCollectionPage> GetAsync(CancellationToken cancellationToken);

    IUserCalendarViewCollectionRequest Expand(string value);

    IUserCalendarViewCollectionRequest Select(string value);

    IUserCalendarViewCollectionRequest Top(int value);

    IUserCalendarViewCollectionRequest Filter(string value);

    IUserCalendarViewCollectionRequest Skip(int value);

    IUserCalendarViewCollectionRequest OrderBy(string value);
  }
}
