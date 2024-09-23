// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IEventRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public interface IEventRequestBuilder : 
    IOutlookItemRequestBuilder,
    IEntityRequestBuilder,
    IBaseRequestBuilder
  {
    IEventRequest Request();

    IEventRequest Request(IEnumerable<Option> options);

    ICalendarRequestBuilder Calendar { get; }

    IEventInstancesCollectionRequestBuilder Instances { get; }

    IEventExtensionsCollectionRequestBuilder Extensions { get; }

    IEventAttachmentsCollectionRequestBuilder Attachments { get; }

    IEventAcceptRequestBuilder Accept(string Comment = null, bool? SendResponse = null);

    IEventDeclineRequestBuilder Decline(string Comment = null, bool? SendResponse = null);

    IEventTentativelyAcceptRequestBuilder TentativelyAccept(string Comment = null, bool? SendResponse = null);

    IEventSnoozeReminderRequestBuilder SnoozeReminder(DateTimeTimeZone NewReminderTime);

    IEventDismissReminderRequestBuilder DismissReminder();
  }
}
