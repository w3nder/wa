// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.GroupThreadsCollectionRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class GroupThreadsCollectionRequestBuilder : 
    BaseRequestBuilder,
    IGroupThreadsCollectionRequestBuilder
  {
    public GroupThreadsCollectionRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IGroupThreadsCollectionRequest Request() => this.Request((IEnumerable<Option>) null);

    public IGroupThreadsCollectionRequest Request(IEnumerable<Option> options)
    {
      return (IGroupThreadsCollectionRequest) new GroupThreadsCollectionRequest(this.RequestUrl, this.Client, options);
    }

    public IConversationThreadRequestBuilder this[string id]
    {
      get
      {
        return (IConversationThreadRequestBuilder) new ConversationThreadRequestBuilder(this.AppendSegmentToRequestUrl(id), this.Client);
      }
    }
  }
}
