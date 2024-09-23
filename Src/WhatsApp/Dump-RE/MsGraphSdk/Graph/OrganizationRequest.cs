// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.OrganizationRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class OrganizationRequest : BaseRequest, IOrganizationRequest, IBaseRequest
  {
    public OrganizationRequest(string requestUrl, IBaseClient client, IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<Organization> CreateAsync(Organization organizationToCreate)
    {
      return this.CreateAsync(organizationToCreate, CancellationToken.None);
    }

    public async Task<Organization> CreateAsync(
      Organization organizationToCreate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PUT";
      Organization organizationToInitialize = await this.SendAsync<Organization>((object) organizationToCreate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(organizationToInitialize);
      return organizationToInitialize;
    }

    public Task DeleteAsync() => this.DeleteAsync(CancellationToken.None);

    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
      this.Method = "DELETE";
      Organization organization = await this.SendAsync<Organization>((object) null, cancellationToken).ConfigureAwait(false);
    }

    public Task<Organization> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<Organization> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      Organization organizationToInitialize = await this.SendAsync<Organization>((object) null, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(organizationToInitialize);
      return organizationToInitialize;
    }

    public Task<Organization> UpdateAsync(Organization organizationToUpdate)
    {
      return this.UpdateAsync(organizationToUpdate, CancellationToken.None);
    }

    public async Task<Organization> UpdateAsync(
      Organization organizationToUpdate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PATCH";
      Organization organizationToInitialize = await this.SendAsync<Organization>((object) organizationToUpdate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(organizationToInitialize);
      return organizationToInitialize;
    }

    private void InitializeCollectionProperties(Organization organizationToInitialize)
    {
    }
  }
}
