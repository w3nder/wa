// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.MessageCreateReplyAllRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class MessageCreateReplyAllRequestBuilder : 
    BaseGetMethodRequestBuilder<IMessageCreateReplyAllRequest>,
    IMessageCreateReplyAllRequestBuilder
  {
    public MessageCreateReplyAllRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    protected override IMessageCreateReplyAllRequest CreateRequest(
      string functionUrl,
      IEnumerable<Option> options)
    {
      return (IMessageCreateReplyAllRequest) new MessageCreateReplyAllRequest(functionUrl, this.Client, options);
    }
  }
}
