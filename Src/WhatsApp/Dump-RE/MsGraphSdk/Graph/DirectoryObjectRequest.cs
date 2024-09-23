// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.DirectoryObjectRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class DirectoryObjectRequest : BaseRequest, IDirectoryObjectRequest, IBaseRequest
  {
    public DirectoryObjectRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<DirectoryObject> CreateAsync(DirectoryObject directoryObjectToCreate)
    {
      return this.CreateAsync(directoryObjectToCreate, CancellationToken.None);
    }

    public async Task<DirectoryObject> CreateAsync(
      DirectoryObject directoryObjectToCreate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PUT";
      DirectoryObject directoryObjectToInitialize = await this.SendAsync<DirectoryObject>((object) directoryObjectToCreate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(directoryObjectToInitialize);
      return directoryObjectToInitialize;
    }

    public Task DeleteAsync() => this.DeleteAsync(CancellationToken.None);

    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
      this.Method = "DELETE";
      DirectoryObject directoryObject = await this.SendAsync<DirectoryObject>((object) null, cancellationToken).ConfigureAwait(false);
    }

    public Task<DirectoryObject> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<DirectoryObject> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      DirectoryObject directoryObjectToInitialize = await this.SendAsync<DirectoryObject>((object) null, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(directoryObjectToInitialize);
      return directoryObjectToInitialize;
    }

    public Task<DirectoryObject> UpdateAsync(DirectoryObject directoryObjectToUpdate)
    {
      return this.UpdateAsync(directoryObjectToUpdate, CancellationToken.None);
    }

    public async Task<DirectoryObject> UpdateAsync(
      DirectoryObject directoryObjectToUpdate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PATCH";
      DirectoryObject directoryObjectToInitialize = await this.SendAsync<DirectoryObject>((object) directoryObjectToUpdate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(directoryObjectToInitialize);
      return directoryObjectToInitialize;
    }

    private void InitializeCollectionProperties(DirectoryObject directoryObjectToInitialize)
    {
    }
  }
}
