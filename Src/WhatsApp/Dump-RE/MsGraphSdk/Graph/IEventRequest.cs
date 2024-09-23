// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IEventRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IEventRequest : IBaseRequest
  {
    Task<Event> CreateAsync(Event eventToCreate);

    Task<Event> CreateAsync(Event eventToCreate, CancellationToken cancellationToken);

    Task DeleteAsync();

    Task DeleteAsync(CancellationToken cancellationToken);

    Task<Event> GetAsync();

    Task<Event> GetAsync(CancellationToken cancellationToken);

    Task<Event> UpdateAsync(Event eventToUpdate);

    Task<Event> UpdateAsync(Event eventToUpdate, CancellationToken cancellationToken);

    IEventRequest Expand(string value);

    IEventRequest Select(string value);
  }
}
