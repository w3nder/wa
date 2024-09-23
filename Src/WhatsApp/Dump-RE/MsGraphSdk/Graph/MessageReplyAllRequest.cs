﻿// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.MessageReplyAllRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class MessageReplyAllRequest : BaseRequest, IMessageReplyAllRequest, IBaseRequest
  {
    public MessageReplyAllRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
      this.Method = "POST";
      this.ContentType = "application/json";
      this.RequestBody = new MessageReplyAllRequestBody();
    }

    public MessageReplyAllRequestBody RequestBody { get; private set; }

    public Task PostAsync() => this.PostAsync(CancellationToken.None);

    public Task PostAsync(CancellationToken cancellationToken)
    {
      return this.SendAsync((object) this.RequestBody, cancellationToken);
    }

    public IMessageReplyAllRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IMessageReplyAllRequest) this;
    }

    public IMessageReplyAllRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IMessageReplyAllRequest) this;
    }
  }
}
