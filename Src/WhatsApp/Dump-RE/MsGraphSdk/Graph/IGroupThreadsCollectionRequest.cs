// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IGroupThreadsCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IGroupThreadsCollectionRequest : IBaseRequest
  {
    Task<ConversationThread> AddAsync(ConversationThread conversationThread);

    Task<ConversationThread> AddAsync(
      ConversationThread conversationThread,
      CancellationToken cancellationToken);

    Task<IGroupThreadsCollectionPage> GetAsync();

    Task<IGroupThreadsCollectionPage> GetAsync(CancellationToken cancellationToken);

    IGroupThreadsCollectionRequest Expand(string value);

    IGroupThreadsCollectionRequest Select(string value);

    IGroupThreadsCollectionRequest Top(int value);

    IGroupThreadsCollectionRequest Filter(string value);

    IGroupThreadsCollectionRequest Skip(int value);

    IGroupThreadsCollectionRequest OrderBy(string value);
  }
}
