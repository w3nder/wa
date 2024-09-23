// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IReferenceAttachmentRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IReferenceAttachmentRequest : IBaseRequest
  {
    Task<ReferenceAttachment> CreateAsync(ReferenceAttachment referenceAttachmentToCreate);

    Task<ReferenceAttachment> CreateAsync(
      ReferenceAttachment referenceAttachmentToCreate,
      CancellationToken cancellationToken);

    Task DeleteAsync();

    Task DeleteAsync(CancellationToken cancellationToken);

    Task<ReferenceAttachment> GetAsync();

    Task<ReferenceAttachment> GetAsync(CancellationToken cancellationToken);

    Task<ReferenceAttachment> UpdateAsync(ReferenceAttachment referenceAttachmentToUpdate);

    Task<ReferenceAttachment> UpdateAsync(
      ReferenceAttachment referenceAttachmentToUpdate,
      CancellationToken cancellationToken);

    IReferenceAttachmentRequest Expand(string value);

    IReferenceAttachmentRequest Select(string value);
  }
}
