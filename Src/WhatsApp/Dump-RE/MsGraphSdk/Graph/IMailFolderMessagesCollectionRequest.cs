// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IMailFolderMessagesCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IMailFolderMessagesCollectionRequest : IBaseRequest
  {
    Task<Message> AddAsync(Message message);

    Task<Message> AddAsync(Message message, CancellationToken cancellationToken);

    Task<IMailFolderMessagesCollectionPage> GetAsync();

    Task<IMailFolderMessagesCollectionPage> GetAsync(CancellationToken cancellationToken);

    IMailFolderMessagesCollectionRequest Expand(string value);

    IMailFolderMessagesCollectionRequest Select(string value);

    IMailFolderMessagesCollectionRequest Top(int value);

    IMailFolderMessagesCollectionRequest Filter(string value);

    IMailFolderMessagesCollectionRequest Skip(int value);

    IMailFolderMessagesCollectionRequest OrderBy(string value);
  }
}
