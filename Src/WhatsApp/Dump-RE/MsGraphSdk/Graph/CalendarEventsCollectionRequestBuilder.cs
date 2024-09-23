// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.CalendarEventsCollectionRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class CalendarEventsCollectionRequestBuilder : 
    BaseRequestBuilder,
    ICalendarEventsCollectionRequestBuilder
  {
    public CalendarEventsCollectionRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public ICalendarEventsCollectionRequest Request() => this.Request((IEnumerable<Option>) null);

    public ICalendarEventsCollectionRequest Request(IEnumerable<Option> options)
    {
      return (ICalendarEventsCollectionRequest) new CalendarEventsCollectionRequest(this.RequestUrl, this.Client, options);
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
