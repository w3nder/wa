// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.EventRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class EventRequestBuilder : 
    OutlookItemRequestBuilder,
    IEventRequestBuilder,
    IOutlookItemRequestBuilder,
    IEntityRequestBuilder,
    IBaseRequestBuilder
  {
    public EventRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IEventRequest Request() => this.Request((IEnumerable<Option>) null);

    public IEventRequest Request(IEnumerable<Option> options)
    {
      return (IEventRequest) new EventRequest(this.RequestUrl, this.Client, options);
    }

    public ICalendarRequestBuilder Calendar
    {
      get
      {
        return (ICalendarRequestBuilder) new CalendarRequestBuilder(this.AppendSegmentToRequestUrl("calendar"), this.Client);
      }
    }

    public IEventInstancesCollectionRequestBuilder Instances
    {
      get
      {
        return (IEventInstancesCollectionRequestBuilder) new EventInstancesCollectionRequestBuilder(this.AppendSegmentToRequestUrl("instances"), this.Client);
      }
    }

    public IEventExtensionsCollectionRequestBuilder Extensions
    {
      get
      {
        return (IEventExtensionsCollectionRequestBuilder) new EventExtensionsCollectionRequestBuilder(this.AppendSegmentToRequestUrl("extensions"), this.Client);
      }
    }

    public IEventAttachmentsCollectionRequestBuilder Attachments
    {
      get
      {
        return (IEventAttachmentsCollectionRequestBuilder) new EventAttachmentsCollectionRequestBuilder(this.AppendSegmentToRequestUrl("attachments"), this.Client);
      }
    }

    public IEventAcceptRequestBuilder Accept(string Comment = null, bool? SendResponse = null)
    {
      return (IEventAcceptRequestBuilder) new EventAcceptRequestBuilder(this.AppendSegmentToRequestUrl("microsoft.graph.accept"), this.Client, Comment, SendResponse);
    }

    public IEventDeclineRequestBuilder Decline(string Comment = null, bool? SendResponse = null)
    {
      return (IEventDeclineRequestBuilder) new EventDeclineRequestBuilder(this.AppendSegmentToRequestUrl("microsoft.graph.decline"), this.Client, Comment, SendResponse);
    }

    public IEventTentativelyAcceptRequestBuilder TentativelyAccept(
      string Comment = null,
      bool? SendResponse = null)
    {
      return (IEventTentativelyAcceptRequestBuilder) new EventTentativelyAcceptRequestBuilder(this.AppendSegmentToRequestUrl("microsoft.graph.tentativelyAccept"), this.Client, Comment, SendResponse);
    }

    public IEventSnoozeReminderRequestBuilder SnoozeReminder(DateTimeTimeZone NewReminderTime)
    {
      return (IEventSnoozeReminderRequestBuilder) new EventSnoozeReminderRequestBuilder(this.AppendSegmentToRequestUrl("microsoft.graph.snoozeReminder"), this.Client, NewReminderTime);
    }

    public IEventDismissReminderRequestBuilder DismissReminder()
    {
      return (IEventDismissReminderRequestBuilder) new EventDismissReminderRequestBuilder(this.AppendSegmentToRequestUrl("microsoft.graph.dismissReminder"), this.Client);
    }
  }
}
