// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.DirectoryRoleRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class DirectoryRoleRequest : BaseRequest, IDirectoryRoleRequest, IBaseRequest
  {
    public DirectoryRoleRequest(string requestUrl, IBaseClient client, IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<DirectoryRole> CreateAsync(DirectoryRole directoryRoleToCreate)
    {
      return this.CreateAsync(directoryRoleToCreate, CancellationToken.None);
    }

    public async Task<DirectoryRole> CreateAsync(
      DirectoryRole directoryRoleToCreate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PUT";
      DirectoryRole directoryRoleToInitialize = await this.SendAsync<DirectoryRole>((object) directoryRoleToCreate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(directoryRoleToInitialize);
      return directoryRoleToInitialize;
    }

    public Task DeleteAsync() => this.DeleteAsync(CancellationToken.None);

    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
      this.Method = "DELETE";
      DirectoryRole directoryRole = await this.SendAsync<DirectoryRole>((object) null, cancellationToken).ConfigureAwait(false);
    }

    public Task<DirectoryRole> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<DirectoryRole> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      DirectoryRole directoryRoleToInitialize = await this.SendAsync<DirectoryRole>((object) null, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(directoryRoleToInitialize);
      return directoryRoleToInitialize;
    }

    public Task<DirectoryRole> UpdateAsync(DirectoryRole directoryRoleToUpdate)
    {
      return this.UpdateAsync(directoryRoleToUpdate, CancellationToken.None);
    }

    public async Task<DirectoryRole> UpdateAsync(
      DirectoryRole directoryRoleToUpdate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PATCH";
      DirectoryRole directoryRoleToInitialize = await this.SendAsync<DirectoryRole>((object) directoryRoleToUpdate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(directoryRoleToInitialize);
      return directoryRoleToInitialize;
    }

    private void InitializeCollectionProperties(DirectoryRole directoryRoleToInitialize)
    {
      if (directoryRoleToInitialize == null || directoryRoleToInitialize.AdditionalData == null || directoryRoleToInitialize.Members == null || directoryRoleToInitialize.Members.CurrentPage == null)
        return;
      directoryRoleToInitialize.Members.AdditionalData = directoryRoleToInitialize.AdditionalData;
      object obj;
      directoryRoleToInitialize.AdditionalData.TryGetValue("members@odata.nextLink", out obj);
      string nextPageLinkString = obj as string;
      if (string.IsNullOrEmpty(nextPageLinkString))
        return;
      directoryRoleToInitialize.Members.InitializeNextPageRequest(this.Client, nextPageLinkString);
    }
  }
}
