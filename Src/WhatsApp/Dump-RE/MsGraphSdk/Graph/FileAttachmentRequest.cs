// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.FileAttachmentRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class FileAttachmentRequest : BaseRequest, IFileAttachmentRequest, IBaseRequest
  {
    public FileAttachmentRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<FileAttachment> CreateAsync(FileAttachment fileAttachmentToCreate)
    {
      return this.CreateAsync(fileAttachmentToCreate, CancellationToken.None);
    }

    public async Task<FileAttachment> CreateAsync(
      FileAttachment fileAttachmentToCreate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PUT";
      FileAttachment fileAttachmentToInitialize = await this.SendAsync<FileAttachment>((object) fileAttachmentToCreate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(fileAttachmentToInitialize);
      return fileAttachmentToInitialize;
    }

    public Task DeleteAsync() => this.DeleteAsync(CancellationToken.None);

    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
      this.Method = "DELETE";
      FileAttachment fileAttachment = await this.SendAsync<FileAttachment>((object) null, cancellationToken).ConfigureAwait(false);
    }

    public Task<FileAttachment> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<FileAttachment> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      FileAttachment fileAttachmentToInitialize = await this.SendAsync<FileAttachment>((object) null, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(fileAttachmentToInitialize);
      return fileAttachmentToInitialize;
    }

    public Task<FileAttachment> UpdateAsync(FileAttachment fileAttachmentToUpdate)
    {
      return this.UpdateAsync(fileAttachmentToUpdate, CancellationToken.None);
    }

    public async Task<FileAttachment> UpdateAsync(
      FileAttachment fileAttachmentToUpdate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PATCH";
      FileAttachment fileAttachmentToInitialize = await this.SendAsync<FileAttachment>((object) fileAttachmentToUpdate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(fileAttachmentToInitialize);
      return fileAttachmentToInitialize;
    }

    public IFileAttachmentRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IFileAttachmentRequest) this;
    }

    public IFileAttachmentRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IFileAttachmentRequest) this;
    }

    private void InitializeCollectionProperties(FileAttachment fileAttachmentToInitialize)
    {
    }
  }
}
