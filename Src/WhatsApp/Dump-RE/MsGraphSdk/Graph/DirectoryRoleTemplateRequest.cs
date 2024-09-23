// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.DirectoryRoleTemplateRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class DirectoryRoleTemplateRequest : 
    BaseRequest,
    IDirectoryRoleTemplateRequest,
    IBaseRequest
  {
    public DirectoryRoleTemplateRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<DirectoryRoleTemplate> CreateAsync(
      DirectoryRoleTemplate directoryRoleTemplateToCreate)
    {
      return this.CreateAsync(directoryRoleTemplateToCreate, CancellationToken.None);
    }

    public async Task<DirectoryRoleTemplate> CreateAsync(
      DirectoryRoleTemplate directoryRoleTemplateToCreate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PUT";
      DirectoryRoleTemplate directoryRoleTemplateToInitialize = await this.SendAsync<DirectoryRoleTemplate>((object) directoryRoleTemplateToCreate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(directoryRoleTemplateToInitialize);
      return directoryRoleTemplateToInitialize;
    }

    public Task DeleteAsync() => this.DeleteAsync(CancellationToken.None);

    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
      this.Method = "DELETE";
      DirectoryRoleTemplate directoryRoleTemplate = await this.SendAsync<DirectoryRoleTemplate>((object) null, cancellationToken).ConfigureAwait(false);
    }

    public Task<DirectoryRoleTemplate> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<DirectoryRoleTemplate> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      DirectoryRoleTemplate directoryRoleTemplateToInitialize = await this.SendAsync<DirectoryRoleTemplate>((object) null, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(directoryRoleTemplateToInitialize);
      return directoryRoleTemplateToInitialize;
    }

    public Task<DirectoryRoleTemplate> UpdateAsync(
      DirectoryRoleTemplate directoryRoleTemplateToUpdate)
    {
      return this.UpdateAsync(directoryRoleTemplateToUpdate, CancellationToken.None);
    }

    public async Task<DirectoryRoleTemplate> UpdateAsync(
      DirectoryRoleTemplate directoryRoleTemplateToUpdate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PATCH";
      DirectoryRoleTemplate directoryRoleTemplateToInitialize = await this.SendAsync<DirectoryRoleTemplate>((object) directoryRoleTemplateToUpdate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(directoryRoleTemplateToInitialize);
      return directoryRoleTemplateToInitialize;
    }

    private void InitializeCollectionProperties(
      DirectoryRoleTemplate directoryRoleTemplateToInitialize)
    {
    }
  }
}
