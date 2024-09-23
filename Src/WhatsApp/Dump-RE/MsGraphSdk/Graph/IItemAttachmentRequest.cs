// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IItemAttachmentRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IItemAttachmentRequest : IBaseRequest
  {
    Task<ItemAttachment> CreateAsync(ItemAttachment itemAttachmentToCreate);

    Task<ItemAttachment> CreateAsync(
      ItemAttachment itemAttachmentToCreate,
      CancellationToken cancellationToken);

    Task DeleteAsync();

    Task DeleteAsync(CancellationToken cancellationToken);

    Task<ItemAttachment> GetAsync();

    Task<ItemAttachment> GetAsync(CancellationToken cancellationToken);

    Task<ItemAttachment> UpdateAsync(ItemAttachment itemAttachmentToUpdate);

    Task<ItemAttachment> UpdateAsync(
      ItemAttachment itemAttachmentToUpdate,
      CancellationToken cancellationToken);

    IItemAttachmentRequest Expand(string value);

    IItemAttachmentRequest Select(string value);
  }
}
