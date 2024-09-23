// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IEntityRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IEntityRequest : IBaseRequest
  {
    Task<Entity> CreateAsync(Entity entityToCreate);

    Task<Entity> CreateAsync(Entity entityToCreate, CancellationToken cancellationToken);

    Task DeleteAsync();

    Task DeleteAsync(CancellationToken cancellationToken);

    Task<Entity> GetAsync();

    Task<Entity> GetAsync(CancellationToken cancellationToken);

    Task<Entity> UpdateAsync(Entity entityToUpdate);

    Task<Entity> UpdateAsync(Entity entityToUpdate, CancellationToken cancellationToken);

    IEntityRequest Expand(string value);

    IEntityRequest Select(string value);
  }
}
