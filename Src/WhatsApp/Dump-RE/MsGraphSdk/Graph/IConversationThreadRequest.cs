// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IConversationThreadRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IConversationThreadRequest : IBaseRequest
  {
    Task<ConversationThread> CreateAsync(ConversationThread conversationThreadToCreate);

    Task<ConversationThread> CreateAsync(
      ConversationThread conversationThreadToCreate,
      CancellationToken cancellationToken);

    Task DeleteAsync();

    Task DeleteAsync(CancellationToken cancellationToken);

    Task<ConversationThread> GetAsync();

    Task<ConversationThread> GetAsync(CancellationToken cancellationToken);

    Task<ConversationThread> UpdateAsync(ConversationThread conversationThreadToUpdate);

    Task<ConversationThread> UpdateAsync(
      ConversationThread conversationThreadToUpdate,
      CancellationToken cancellationToken);

    IConversationThreadRequest Expand(string value);

    IConversationThreadRequest Select(string value);
  }
}
