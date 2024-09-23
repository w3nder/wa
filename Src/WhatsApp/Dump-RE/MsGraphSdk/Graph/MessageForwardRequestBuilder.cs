// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.MessageForwardRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class MessageForwardRequestBuilder : 
    BasePostMethodRequestBuilder<IMessageForwardRequest>,
    IMessageForwardRequestBuilder
  {
    public MessageForwardRequestBuilder(
      string requestUrl,
      IBaseClient client,
      string Comment,
      IEnumerable<Recipient> ToRecipients)
      : base(requestUrl, client)
    {
      this.SetParameter<string>("comment", Comment, true);
      this.SetParameter<IEnumerable<Recipient>>("toRecipients", ToRecipients, true);
    }

    protected override IMessageForwardRequest CreateRequest(
      string functionUrl,
      IEnumerable<Option> options)
    {
      MessageForwardRequest request = new MessageForwardRequest(functionUrl, this.Client, options);
      if (this.HasParameter("comment"))
        request.RequestBody.Comment = this.GetParameter<string>("comment");
      if (this.HasParameter("toRecipients"))
        request.RequestBody.ToRecipients = this.GetParameter<IEnumerable<Recipient>>("toRecipients");
      return (IMessageForwardRequest) request;
    }
  }
}
