﻿// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.ConversationThreadReplyRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class ConversationThreadReplyRequestBuilder : 
    BasePostMethodRequestBuilder<IConversationThreadReplyRequest>,
    IConversationThreadReplyRequestBuilder
  {
    public ConversationThreadReplyRequestBuilder(string requestUrl, IBaseClient client, Post Post)
      : base(requestUrl, client)
    {
      this.SetParameter<Post>("post", Post, false);
    }

    protected override IConversationThreadReplyRequest CreateRequest(
      string functionUrl,
      IEnumerable<Option> options)
    {
      ConversationThreadReplyRequest request = new ConversationThreadReplyRequest(functionUrl, this.Client, options);
      if (this.HasParameter("post"))
        request.RequestBody.Post = this.GetParameter<Post>("post");
      return (IConversationThreadReplyRequest) request;
    }
  }
}
