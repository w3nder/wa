// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.MessageReplyRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class MessageReplyRequestBuilder : 
    BasePostMethodRequestBuilder<IMessageReplyRequest>,
    IMessageReplyRequestBuilder
  {
    public MessageReplyRequestBuilder(string requestUrl, IBaseClient client, string Comment)
      : base(requestUrl, client)
    {
      this.SetParameter<string>("comment", Comment, true);
    }

    protected override IMessageReplyRequest CreateRequest(
      string functionUrl,
      IEnumerable<Option> options)
    {
      MessageReplyRequest request = new MessageReplyRequest(functionUrl, this.Client, options);
      if (this.HasParameter("comment"))
        request.RequestBody.Comment = this.GetParameter<string>("comment");
      return (IMessageReplyRequest) request;
    }
  }
}
