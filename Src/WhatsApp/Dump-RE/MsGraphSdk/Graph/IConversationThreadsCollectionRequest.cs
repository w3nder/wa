// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IConversationThreadsCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IConversationThreadsCollectionRequest : IBaseRequest
  {
    Task<ConversationThread> AddAsync(ConversationThread conversationThread);

    Task<ConversationThread> AddAsync(
      ConversationThread conversationThread,
      CancellationToken cancellationToken);

    Task<IConversationThreadsCollectionPage> GetAsync();

    Task<IConversationThreadsCollectionPage> GetAsync(CancellationToken cancellationToken);

    IConversationThreadsCollectionRequest Expand(string value);

    IConversationThreadsCollectionRequest Select(string value);

    IConversationThreadsCollectionRequest Top(int value);

    IConversationThreadsCollectionRequest Filter(string value);

    IConversationThreadsCollectionRequest Skip(int value);

    IConversationThreadsCollectionRequest OrderBy(string value);
  }
}
