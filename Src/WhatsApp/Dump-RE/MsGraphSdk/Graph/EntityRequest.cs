// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.EntityRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class EntityRequest : BaseRequest, IEntityRequest, IBaseRequest
  {
    public EntityRequest(string requestUrl, IBaseClient client, IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<Entity> CreateAsync(Entity entityToCreate)
    {
      return this.CreateAsync(entityToCreate, CancellationToken.None);
    }

    public async Task<Entity> CreateAsync(
      Entity entityToCreate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PUT";
      Entity entityToInitialize = await this.SendAsync<Entity>((object) entityToCreate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(entityToInitialize);
      return entityToInitialize;
    }

    public Task DeleteAsync() => this.DeleteAsync(CancellationToken.None);

    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
      this.Method = "DELETE";
      Entity entity = await this.SendAsync<Entity>((object) null, cancellationToken).ConfigureAwait(false);
    }

    public Task<Entity> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<Entity> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      Entity entityToInitialize = await this.SendAsync<Entity>((object) null, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(entityToInitialize);
      return entityToInitialize;
    }

    public Task<Entity> UpdateAsync(Entity entityToUpdate)
    {
      return this.UpdateAsync(entityToUpdate, CancellationToken.None);
    }

    public async Task<Entity> UpdateAsync(
      Entity entityToUpdate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PATCH";
      Entity entityToInitialize = await this.SendAsync<Entity>((object) entityToUpdate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(entityToInitialize);
      return entityToInitialize;
    }

    public IEntityRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IEntityRequest) this;
    }

    public IEntityRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IEntityRequest) this;
    }

    private void InitializeCollectionProperties(Entity entityToInitialize)
    {
    }
  }
}
