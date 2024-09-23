// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IGroupConversationsCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IGroupConversationsCollectionRequest : IBaseRequest
  {
    Task<Conversation> AddAsync(Conversation conversation);

    Task<Conversation> AddAsync(Conversation conversation, CancellationToken cancellationToken);

    Task<IGroupConversationsCollectionPage> GetAsync();

    Task<IGroupConversationsCollectionPage> GetAsync(CancellationToken cancellationToken);

    IGroupConversationsCollectionRequest Expand(string value);

    IGroupConversationsCollectionRequest Select(string value);

    IGroupConversationsCollectionRequest Top(int value);

    IGroupConversationsCollectionRequest Filter(string value);

    IGroupConversationsCollectionRequest Skip(int value);

    IGroupConversationsCollectionRequest OrderBy(string value);
  }
}
