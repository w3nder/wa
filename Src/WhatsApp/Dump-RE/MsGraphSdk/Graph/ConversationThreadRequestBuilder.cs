// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.ConversationThreadRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class ConversationThreadRequestBuilder : 
    EntityRequestBuilder,
    IConversationThreadRequestBuilder,
    IEntityRequestBuilder,
    IBaseRequestBuilder
  {
    public ConversationThreadRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IConversationThreadRequest Request() => this.Request((IEnumerable<Option>) null);

    public IConversationThreadRequest Request(IEnumerable<Option> options)
    {
      return (IConversationThreadRequest) new ConversationThreadRequest(this.RequestUrl, this.Client, options);
    }

    public IConversationThreadPostsCollectionRequestBuilder Posts
    {
      get
      {
        return (IConversationThreadPostsCollectionRequestBuilder) new ConversationThreadPostsCollectionRequestBuilder(this.AppendSegmentToRequestUrl("posts"), this.Client);
      }
    }

    public IConversationThreadReplyRequestBuilder Reply(Post Post)
    {
      return (IConversationThreadReplyRequestBuilder) new ConversationThreadReplyRequestBuilder(this.AppendSegmentToRequestUrl("microsoft.graph.reply"), this.Client, Post);
    }
  }
}
