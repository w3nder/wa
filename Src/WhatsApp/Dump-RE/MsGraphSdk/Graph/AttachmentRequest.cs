// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.AttachmentRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class AttachmentRequest : BaseRequest, IAttachmentRequest, IBaseRequest
  {
    public AttachmentRequest(string requestUrl, IBaseClient client, IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<Attachment> CreateAsync(Attachment attachmentToCreate)
    {
      return this.CreateAsync(attachmentToCreate, CancellationToken.None);
    }

    public async Task<Attachment> CreateAsync(
      Attachment attachmentToCreate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PUT";
      Attachment attachmentToInitialize = await this.SendAsync<Attachment>((object) attachmentToCreate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(attachmentToInitialize);
      return attachmentToInitialize;
    }

    public Task DeleteAsync() => this.DeleteAsync(CancellationToken.None);

    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
      this.Method = "DELETE";
      Attachment attachment = await this.SendAsync<Attachment>((object) null, cancellationToken).ConfigureAwait(false);
    }

    public Task<Attachment> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<Attachment> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      Attachment attachmentToInitialize = await this.SendAsync<Attachment>((object) null, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(attachmentToInitialize);
      return attachmentToInitialize;
    }

    public Task<Attachment> UpdateAsync(Attachment attachmentToUpdate)
    {
      return this.UpdateAsync(attachmentToUpdate, CancellationToken.None);
    }

    public async Task<Attachment> UpdateAsync(
      Attachment attachmentToUpdate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PATCH";
      Attachment attachmentToInitialize = await this.SendAsync<Attachment>((object) attachmentToUpdate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(attachmentToInitialize);
      return attachmentToInitialize;
    }

    public IAttachmentRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IAttachmentRequest) this;
    }

    public IAttachmentRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IAttachmentRequest) this;
    }

    private void InitializeCollectionProperties(Attachment attachmentToInitialize)
    {
    }
  }
}
