// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IPostAttachmentsCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IPostAttachmentsCollectionRequest : IBaseRequest
  {
    Task<Attachment> AddAsync(Attachment attachment);

    Task<Attachment> AddAsync(Attachment attachment, CancellationToken cancellationToken);

    Task<IPostAttachmentsCollectionPage> GetAsync();

    Task<IPostAttachmentsCollectionPage> GetAsync(CancellationToken cancellationToken);

    IPostAttachmentsCollectionRequest Expand(string value);

    IPostAttachmentsCollectionRequest Select(string value);

    IPostAttachmentsCollectionRequest Top(int value);

    IPostAttachmentsCollectionRequest Filter(string value);

    IPostAttachmentsCollectionRequest Skip(int value);

    IPostAttachmentsCollectionRequest OrderBy(string value);
  }
}
