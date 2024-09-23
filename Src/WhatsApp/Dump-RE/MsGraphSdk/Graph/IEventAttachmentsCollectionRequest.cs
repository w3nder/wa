// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IEventAttachmentsCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IEventAttachmentsCollectionRequest : IBaseRequest
  {
    Task<Attachment> AddAsync(Attachment attachment);

    Task<Attachment> AddAsync(Attachment attachment, CancellationToken cancellationToken);

    Task<IEventAttachmentsCollectionPage> GetAsync();

    Task<IEventAttachmentsCollectionPage> GetAsync(CancellationToken cancellationToken);

    IEventAttachmentsCollectionRequest Expand(string value);

    IEventAttachmentsCollectionRequest Select(string value);

    IEventAttachmentsCollectionRequest Top(int value);

    IEventAttachmentsCollectionRequest Filter(string value);

    IEventAttachmentsCollectionRequest Skip(int value);

    IEventAttachmentsCollectionRequest OrderBy(string value);
  }
}
