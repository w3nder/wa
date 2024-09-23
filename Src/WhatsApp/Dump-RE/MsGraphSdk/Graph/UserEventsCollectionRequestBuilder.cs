// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.UserEventsCollectionRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class UserEventsCollectionRequestBuilder : 
    BaseRequestBuilder,
    IUserEventsCollectionRequestBuilder
  {
    public UserEventsCollectionRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IUserEventsCollectionRequest Request() => this.Request((IEnumerable<Option>) null);

    public IUserEventsCollectionRequest Request(IEnumerable<Option> options)
    {
      return (IUserEventsCollectionRequest) new UserEventsCollectionRequest(this.RequestUrl, this.Client, options);
    }

    public IEventRequestBuilder this[string id]
    {
      get
      {
        return (IEventRequestBuilder) new EventRequestBuilder(this.AppendSegmentToRequestUrl(id), this.Client);
      }
    }
  }
}
