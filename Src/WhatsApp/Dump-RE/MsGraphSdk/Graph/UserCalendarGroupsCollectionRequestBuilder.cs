// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.UserCalendarGroupsCollectionRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class UserCalendarGroupsCollectionRequestBuilder : 
    BaseRequestBuilder,
    IUserCalendarGroupsCollectionRequestBuilder
  {
    public UserCalendarGroupsCollectionRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IUserCalendarGroupsCollectionRequest Request()
    {
      return this.Request((IEnumerable<Option>) null);
    }

    public IUserCalendarGroupsCollectionRequest Request(IEnumerable<Option> options)
    {
      return (IUserCalendarGroupsCollectionRequest) new UserCalendarGroupsCollectionRequest(this.RequestUrl, this.Client, options);
    }

    public ICalendarGroupRequestBuilder this[string id]
    {
      get
      {
        return (ICalendarGroupRequestBuilder) new CalendarGroupRequestBuilder(this.AppendSegmentToRequestUrl(id), this.Client);
      }
    }
  }
}
