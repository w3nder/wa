// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IUserEventsCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IUserEventsCollectionRequest : IBaseRequest
  {
    Task<Event> AddAsync(Event eventsEvent);

    Task<Event> AddAsync(Event eventsEvent, CancellationToken cancellationToken);

    Task<IUserEventsCollectionPage> GetAsync();

    Task<IUserEventsCollectionPage> GetAsync(CancellationToken cancellationToken);

    IUserEventsCollectionRequest Expand(string value);

    IUserEventsCollectionRequest Select(string value);

    IUserEventsCollectionRequest Top(int value);

    IUserEventsCollectionRequest Filter(string value);

    IUserEventsCollectionRequest Skip(int value);

    IUserEventsCollectionRequest OrderBy(string value);
  }
}
