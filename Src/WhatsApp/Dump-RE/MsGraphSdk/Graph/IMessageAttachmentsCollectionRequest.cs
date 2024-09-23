// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IMessageAttachmentsCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IMessageAttachmentsCollectionRequest : IBaseRequest
  {
    Task<Attachment> AddAsync(Attachment attachment);

    Task<Attachment> AddAsync(Attachment attachment, CancellationToken cancellationToken);

    Task<IMessageAttachmentsCollectionPage> GetAsync();

    Task<IMessageAttachmentsCollectionPage> GetAsync(CancellationToken cancellationToken);

    IMessageAttachmentsCollectionRequest Expand(string value);

    IMessageAttachmentsCollectionRequest Select(string value);

    IMessageAttachmentsCollectionRequest Top(int value);

    IMessageAttachmentsCollectionRequest Filter(string value);

    IMessageAttachmentsCollectionRequest Skip(int value);

    IMessageAttachmentsCollectionRequest OrderBy(string value);
  }
}
