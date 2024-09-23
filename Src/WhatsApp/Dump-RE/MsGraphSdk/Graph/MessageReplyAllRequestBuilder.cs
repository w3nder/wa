// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.MessageReplyAllRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class MessageReplyAllRequestBuilder : 
    BasePostMethodRequestBuilder<IMessageReplyAllRequest>,
    IMessageReplyAllRequestBuilder
  {
    public MessageReplyAllRequestBuilder(string requestUrl, IBaseClient client, string Comment)
      : base(requestUrl, client)
    {
      this.SetParameter<string>("comment", Comment, true);
    }

    protected override IMessageReplyAllRequest CreateRequest(
      string functionUrl,
      IEnumerable<Option> options)
    {
      MessageReplyAllRequest request = new MessageReplyAllRequest(functionUrl, this.Client, options);
      if (this.HasParameter("comment"))
        request.RequestBody.Comment = this.GetParameter<string>("comment");
      return (IMessageReplyAllRequest) request;
    }
  }
}
