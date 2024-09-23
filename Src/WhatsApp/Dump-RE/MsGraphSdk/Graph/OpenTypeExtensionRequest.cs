// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.OpenTypeExtensionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class OpenTypeExtensionRequest : BaseRequest, IOpenTypeExtensionRequest, IBaseRequest
  {
    public OpenTypeExtensionRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<OpenTypeExtension> CreateAsync(OpenTypeExtension openTypeExtensionToCreate)
    {
      return this.CreateAsync(openTypeExtensionToCreate, CancellationToken.None);
    }

    public async Task<OpenTypeExtension> CreateAsync(
      OpenTypeExtension openTypeExtensionToCreate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PUT";
      OpenTypeExtension openTypeExtensionToInitialize = await this.SendAsync<OpenTypeExtension>((object) openTypeExtensionToCreate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(openTypeExtensionToInitialize);
      return openTypeExtensionToInitialize;
    }

    public Task DeleteAsync() => this.DeleteAsync(CancellationToken.None);

    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
      this.Method = "DELETE";
      OpenTypeExtension openTypeExtension = await this.SendAsync<OpenTypeExtension>((object) null, cancellationToken).ConfigureAwait(false);
    }

    public Task<OpenTypeExtension> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<OpenTypeExtension> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      OpenTypeExtension openTypeExtensionToInitialize = await this.SendAsync<OpenTypeExtension>((object) null, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(openTypeExtensionToInitialize);
      return openTypeExtensionToInitialize;
    }

    public Task<OpenTypeExtension> UpdateAsync(OpenTypeExtension openTypeExtensionToUpdate)
    {
      return this.UpdateAsync(openTypeExtensionToUpdate, CancellationToken.None);
    }

    public async Task<OpenTypeExtension> UpdateAsync(
      OpenTypeExtension openTypeExtensionToUpdate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PATCH";
      OpenTypeExtension openTypeExtensionToInitialize = await this.SendAsync<OpenTypeExtension>((object) openTypeExtensionToUpdate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(openTypeExtensionToInitialize);
      return openTypeExtensionToInitialize;
    }

    public IOpenTypeExtensionRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IOpenTypeExtensionRequest) this;
    }

    public IOpenTypeExtensionRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IOpenTypeExtensionRequest) this;
    }

    private void InitializeCollectionProperties(OpenTypeExtension openTypeExtensionToInitialize)
    {
    }
  }
}
