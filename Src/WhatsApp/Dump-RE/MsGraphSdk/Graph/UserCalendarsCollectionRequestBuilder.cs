// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.UserCalendarsCollectionRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class UserCalendarsCollectionRequestBuilder : 
    BaseRequestBuilder,
    IUserCalendarsCollectionRequestBuilder
  {
    public UserCalendarsCollectionRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IUserCalendarsCollectionRequest Request() => this.Request((IEnumerable<Option>) null);

    public IUserCalendarsCollectionRequest Request(IEnumerable<Option> options)
    {
      return (IUserCalendarsCollectionRequest) new UserCalendarsCollectionRequest(this.RequestUrl, this.Client, options);
    }

    public ICalendarRequestBuilder this[string id]
    {
      get
      {
        return (ICalendarRequestBuilder) new CalendarRequestBuilder(this.AppendSegmentToRequestUrl(id), this.Client);
      }
    }
  }
}
