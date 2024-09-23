// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.CalendarRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class CalendarRequestBuilder : 
    EntityRequestBuilder,
    ICalendarRequestBuilder,
    IEntityRequestBuilder,
    IBaseRequestBuilder
  {
    public CalendarRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public ICalendarRequest Request() => this.Request((IEnumerable<Option>) null);

    public ICalendarRequest Request(IEnumerable<Option> options)
    {
      return (ICalendarRequest) new CalendarRequest(this.RequestUrl, this.Client, options);
    }

    public ICalendarEventsCollectionRequestBuilder Events
    {
      get
      {
        return (ICalendarEventsCollectionRequestBuilder) new CalendarEventsCollectionRequestBuilder(this.AppendSegmentToRequestUrl("events"), this.Client);
      }
    }

    public ICalendarCalendarViewCollectionRequestBuilder CalendarView
    {
      get
      {
        return (ICalendarCalendarViewCollectionRequestBuilder) new CalendarCalendarViewCollectionRequestBuilder(this.AppendSegmentToRequestUrl("calendarView"), this.Client);
      }
    }
  }
}
