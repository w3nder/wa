// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IConversationThreadPostsCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IConversationThreadPostsCollectionRequest : IBaseRequest
  {
    Task<Post> AddAsync(Post post);

    Task<Post> AddAsync(Post post, CancellationToken cancellationToken);

    Task<IConversationThreadPostsCollectionPage> GetAsync();

    Task<IConversationThreadPostsCollectionPage> GetAsync(CancellationToken cancellationToken);

    IConversationThreadPostsCollectionRequest Expand(string value);

    IConversationThreadPostsCollectionRequest Select(string value);

    IConversationThreadPostsCollectionRequest Top(int value);

    IConversationThreadPostsCollectionRequest Filter(string value);

    IConversationThreadPostsCollectionRequest Skip(int value);

    IConversationThreadPostsCollectionRequest OrderBy(string value);
  }
}
