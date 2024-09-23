// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.PermissionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class PermissionRequest : BaseRequest, IPermissionRequest, IBaseRequest
  {
    public PermissionRequest(string requestUrl, IBaseClient client, IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<Permission> CreateAsync(Permission permissionToCreate)
    {
      return this.CreateAsync(permissionToCreate, CancellationToken.None);
    }

    public async Task<Permission> CreateAsync(
      Permission permissionToCreate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PUT";
      Permission permissionToInitialize = await this.SendAsync<Permission>((object) permissionToCreate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(permissionToInitialize);
      return permissionToInitialize;
    }

    public Task DeleteAsync() => this.DeleteAsync(CancellationToken.None);

    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
      this.Method = "DELETE";
      Permission permission = await this.SendAsync<Permission>((object) null, cancellationToken).ConfigureAwait(false);
    }

    public Task<Permission> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<Permission> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      Permission permissionToInitialize = await this.SendAsync<Permission>((object) null, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(permissionToInitialize);
      return permissionToInitialize;
    }

    public Task<Permission> UpdateAsync(Permission permissionToUpdate)
    {
      return this.UpdateAsync(permissionToUpdate, CancellationToken.None);
    }

    public async Task<Permission> UpdateAsync(
      Permission permissionToUpdate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PATCH";
      Permission permissionToInitialize = await this.SendAsync<Permission>((object) permissionToUpdate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(permissionToInitialize);
      return permissionToInitialize;
    }

    public IPermissionRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IPermissionRequest) this;
    }

    public IPermissionRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IPermissionRequest) this;
    }

    private void InitializeCollectionProperties(Permission permissionToInitialize)
    {
    }
  }
}
