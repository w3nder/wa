// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IFileAttachmentRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IFileAttachmentRequest : IBaseRequest
  {
    Task<FileAttachment> CreateAsync(FileAttachment fileAttachmentToCreate);

    Task<FileAttachment> CreateAsync(
      FileAttachment fileAttachmentToCreate,
      CancellationToken cancellationToken);

    Task DeleteAsync();

    Task DeleteAsync(CancellationToken cancellationToken);

    Task<FileAttachment> GetAsync();

    Task<FileAttachment> GetAsync(CancellationToken cancellationToken);

    Task<FileAttachment> UpdateAsync(FileAttachment fileAttachmentToUpdate);

    Task<FileAttachment> UpdateAsync(
      FileAttachment fileAttachmentToUpdate,
      CancellationToken cancellationToken);

    IFileAttachmentRequest Expand(string value);

    IFileAttachmentRequest Select(string value);
  }
}
