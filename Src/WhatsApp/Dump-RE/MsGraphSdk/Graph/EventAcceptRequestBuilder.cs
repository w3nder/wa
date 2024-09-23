// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.EventAcceptRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class EventAcceptRequestBuilder : 
    BasePostMethodRequestBuilder<IEventAcceptRequest>,
    IEventAcceptRequestBuilder
  {
    public EventAcceptRequestBuilder(
      string requestUrl,
      IBaseClient client,
      string Comment,
      bool? SendResponse)
      : base(requestUrl, client)
    {
      this.SetParameter<string>("comment", Comment, true);
      this.SetParameter<bool?>("sendResponse", SendResponse, true);
    }

    protected override IEventAcceptRequest CreateRequest(
      string functionUrl,
      IEnumerable<Option> options)
    {
      EventAcceptRequest request = new EventAcceptRequest(functionUrl, this.Client, options);
      if (this.HasParameter("comment"))
        request.RequestBody.Comment = this.GetParameter<string>("comment");
      if (this.HasParameter("sendResponse"))
        request.RequestBody.SendResponse = this.GetParameter<bool?>("sendResponse");
      return (IEventAcceptRequest) request;
    }
  }
}
