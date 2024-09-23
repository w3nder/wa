// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.MessageCreateReplyRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class MessageCreateReplyRequestBuilder : 
    BaseGetMethodRequestBuilder<IMessageCreateReplyRequest>,
    IMessageCreateReplyRequestBuilder
  {
    public MessageCreateReplyRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    protected override IMessageCreateReplyRequest CreateRequest(
      string functionUrl,
      IEnumerable<Option> options)
    {
      return (IMessageCreateReplyRequest) new MessageCreateReplyRequest(functionUrl, this.Client, options);
    }
  }
}
