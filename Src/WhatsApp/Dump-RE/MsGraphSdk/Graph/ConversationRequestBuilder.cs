// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.ConversationRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class ConversationRequestBuilder : 
    EntityRequestBuilder,
    IConversationRequestBuilder,
    IEntityRequestBuilder,
    IBaseRequestBuilder
  {
    public ConversationRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IConversationRequest Request() => this.Request((IEnumerable<Option>) null);

    public IConversationRequest Request(IEnumerable<Option> options)
    {
      return (IConversationRequest) new ConversationRequest(this.RequestUrl, this.Client, options);
    }

    public IConversationThreadsCollectionRequestBuilder Threads
    {
      get
      {
        return (IConversationThreadsCollectionRequestBuilder) new ConversationThreadsCollectionRequestBuilder(this.AppendSegmentToRequestUrl("threads"), this.Client);
      }
    }
  }
}
