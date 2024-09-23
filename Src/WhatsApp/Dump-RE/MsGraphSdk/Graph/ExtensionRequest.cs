// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.ExtensionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class ExtensionRequest : BaseRequest, IExtensionRequest, IBaseRequest
  {
    public ExtensionRequest(string requestUrl, IBaseClient client, IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<Extension> CreateAsync(Extension extensionToCreate)
    {
      return this.CreateAsync(extensionToCreate, CancellationToken.None);
    }

    public async Task<Extension> CreateAsync(
      Extension extensionToCreate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PUT";
      Extension extensionToInitialize = await this.SendAsync<Extension>((object) extensionToCreate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(extensionToInitialize);
      return extensionToInitialize;
    }

    public Task DeleteAsync() => this.DeleteAsync(CancellationToken.None);

    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
      this.Method = "DELETE";
      Extension extension = await this.SendAsync<Extension>((object) null, cancellationToken).ConfigureAwait(false);
    }

    public Task<Extension> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<Extension> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      Extension extensionToInitialize = await this.SendAsync<Extension>((object) null, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(extensionToInitialize);
      return extensionToInitialize;
    }

    public Task<Extension> UpdateAsync(Extension extensionToUpdate)
    {
      return this.UpdateAsync(extensionToUpdate, CancellationToken.None);
    }

    public async Task<Extension> UpdateAsync(
      Extension extensionToUpdate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PATCH";
      Extension extensionToInitialize = await this.SendAsync<Extension>((object) extensionToUpdate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(extensionToInitialize);
      return extensionToInitialize;
    }

    public IExtensionRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IExtensionRequest) this;
    }

    public IExtensionRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IExtensionRequest) this;
    }

    private void InitializeCollectionProperties(Extension extensionToInitialize)
    {
    }
  }
}
