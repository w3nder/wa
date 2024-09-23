// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.EventTentativelyAcceptRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class EventTentativelyAcceptRequestBuilder : 
    BasePostMethodRequestBuilder<IEventTentativelyAcceptRequest>,
    IEventTentativelyAcceptRequestBuilder
  {
    public EventTentativelyAcceptRequestBuilder(
      string requestUrl,
      IBaseClient client,
      string Comment,
      bool? SendResponse)
      : base(requestUrl, client)
    {
      this.SetParameter<string>("comment", Comment, true);
      this.SetParameter<bool?>("sendResponse", SendResponse, true);
    }

    protected override IEventTentativelyAcceptRequest CreateRequest(
      string functionUrl,
      IEnumerable<Option> options)
    {
      EventTentativelyAcceptRequest request = new EventTentativelyAcceptRequest(functionUrl, this.Client, options);
      if (this.HasParameter("comment"))
        request.RequestBody.Comment = this.GetParameter<string>("comment");
      if (this.HasParameter("sendResponse"))
        request.RequestBody.SendResponse = this.GetParameter<bool?>("sendResponse");
      return (IEventTentativelyAcceptRequest) request;
    }
  }
}
