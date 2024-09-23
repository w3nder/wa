// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.ReferenceAttachmentRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class ReferenceAttachmentRequest : BaseRequest, IReferenceAttachmentRequest, IBaseRequest
  {
    public ReferenceAttachmentRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<ReferenceAttachment> CreateAsync(ReferenceAttachment referenceAttachmentToCreate)
    {
      return this.CreateAsync(referenceAttachmentToCreate, CancellationToken.None);
    }

    public async Task<ReferenceAttachment> CreateAsync(
      ReferenceAttachment referenceAttachmentToCreate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PUT";
      ReferenceAttachment referenceAttachmentToInitialize = await this.SendAsync<ReferenceAttachment>((object) referenceAttachmentToCreate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(referenceAttachmentToInitialize);
      return referenceAttachmentToInitialize;
    }

    public Task DeleteAsync() => this.DeleteAsync(CancellationToken.None);

    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
      this.Method = "DELETE";
      ReferenceAttachment referenceAttachment = await this.SendAsync<ReferenceAttachment>((object) null, cancellationToken).ConfigureAwait(false);
    }

    public Task<ReferenceAttachment> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<ReferenceAttachment> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      ReferenceAttachment referenceAttachmentToInitialize = await this.SendAsync<ReferenceAttachment>((object) null, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(referenceAttachmentToInitialize);
      return referenceAttachmentToInitialize;
    }

    public Task<ReferenceAttachment> UpdateAsync(ReferenceAttachment referenceAttachmentToUpdate)
    {
      return this.UpdateAsync(referenceAttachmentToUpdate, CancellationToken.None);
    }

    public async Task<ReferenceAttachment> UpdateAsync(
      ReferenceAttachment referenceAttachmentToUpdate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PATCH";
      ReferenceAttachment referenceAttachmentToInitialize = await this.SendAsync<ReferenceAttachment>((object) referenceAttachmentToUpdate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(referenceAttachmentToInitialize);
      return referenceAttachmentToInitialize;
    }

    public IReferenceAttachmentRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IReferenceAttachmentRequest) this;
    }

    public IReferenceAttachmentRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IReferenceAttachmentRequest) this;
    }

    private void InitializeCollectionProperties(
      ReferenceAttachment referenceAttachmentToInitialize)
    {
    }
  }
}
