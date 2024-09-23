// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.EventSnoozeReminderRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class EventSnoozeReminderRequestBuilder : 
    BasePostMethodRequestBuilder<IEventSnoozeReminderRequest>,
    IEventSnoozeReminderRequestBuilder
  {
    public EventSnoozeReminderRequestBuilder(
      string requestUrl,
      IBaseClient client,
      DateTimeTimeZone NewReminderTime)
      : base(requestUrl, client)
    {
      this.SetParameter<DateTimeTimeZone>("newReminderTime", NewReminderTime, false);
    }

    protected override IEventSnoozeReminderRequest CreateRequest(
      string functionUrl,
      IEnumerable<Option> options)
    {
      EventSnoozeReminderRequest request = new EventSnoozeReminderRequest(functionUrl, this.Client, options);
      if (this.HasParameter("newReminderTime"))
        request.RequestBody.NewReminderTime = this.GetParameter<DateTimeTimeZone>("newReminderTime");
      return (IEventSnoozeReminderRequest) request;
    }
  }
}
