// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.CalendarGroupRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class CalendarGroupRequestBuilder : 
    EntityRequestBuilder,
    ICalendarGroupRequestBuilder,
    IEntityRequestBuilder,
    IBaseRequestBuilder
  {
    public CalendarGroupRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public ICalendarGroupRequest Request() => this.Request((IEnumerable<Option>) null);

    public ICalendarGroupRequest Request(IEnumerable<Option> options)
    {
      return (ICalendarGroupRequest) new CalendarGroupRequest(this.RequestUrl, this.Client, options);
    }

    public ICalendarGroupCalendarsCollectionRequestBuilder Calendars
    {
      get
      {
        return (ICalendarGroupCalendarsCollectionRequestBuilder) new CalendarGroupCalendarsCollectionRequestBuilder(this.AppendSegmentToRequestUrl("calendars"), this.Client);
      }
    }
  }
}
