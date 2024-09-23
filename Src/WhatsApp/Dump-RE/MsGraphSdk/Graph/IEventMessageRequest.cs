// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IEventMessageRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IEventMessageRequest : IBaseRequest
  {
    Task<EventMessage> CreateAsync(EventMessage eventMessageToCreate);

    Task<EventMessage> CreateAsync(
      EventMessage eventMessageToCreate,
      CancellationToken cancellationToken);

    Task DeleteAsync();

    Task DeleteAsync(CancellationToken cancellationToken);

    Task<EventMessage> GetAsync();

    Task<EventMessage> GetAsync(CancellationToken cancellationToken);

    Task<EventMessage> UpdateAsync(EventMessage eventMessageToUpdate);

    Task<EventMessage> UpdateAsync(
      EventMessage eventMessageToUpdate,
      CancellationToken cancellationToken);

    IEventMessageRequest Expand(string value);

    IEventMessageRequest Select(string value);
  }
}
