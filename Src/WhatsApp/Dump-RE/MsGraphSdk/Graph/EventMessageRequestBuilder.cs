// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.EventMessageRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class EventMessageRequestBuilder : 
    MessageRequestBuilder,
    IEventMessageRequestBuilder,
    IMessageRequestBuilder,
    IOutlookItemRequestBuilder,
    IEntityRequestBuilder,
    IBaseRequestBuilder
  {
    public EventMessageRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IEventMessageRequest Request() => this.Request((IEnumerable<Option>) null);

    public IEventMessageRequest Request(IEnumerable<Option> options)
    {
      return (IEventMessageRequest) new EventMessageRequest(this.RequestUrl, this.Client, options);
    }

    public IEventRequestBuilder Event
    {
      get
      {
        return (IEventRequestBuilder) new EventRequestBuilder(this.AppendSegmentToRequestUrl("event"), this.Client);
      }
    }
  }
}
