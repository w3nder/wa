// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.ConversationThreadPostsCollectionRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class ConversationThreadPostsCollectionRequestBuilder : 
    BaseRequestBuilder,
    IConversationThreadPostsCollectionRequestBuilder
  {
    public ConversationThreadPostsCollectionRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IConversationThreadPostsCollectionRequest Request()
    {
      return this.Request((IEnumerable<Option>) null);
    }

    public IConversationThreadPostsCollectionRequest Request(IEnumerable<Option> options)
    {
      return (IConversationThreadPostsCollectionRequest) new ConversationThreadPostsCollectionRequest(this.RequestUrl, this.Client, options);
    }

    public IPostRequestBuilder this[string id]
    {
      get
      {
        return (IPostRequestBuilder) new PostRequestBuilder(this.AppendSegmentToRequestUrl(id), this.Client);
      }
    }
  }
}
