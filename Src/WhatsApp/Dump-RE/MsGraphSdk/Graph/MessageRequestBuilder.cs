// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.MessageRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class MessageRequestBuilder : 
    OutlookItemRequestBuilder,
    IMessageRequestBuilder,
    IOutlookItemRequestBuilder,
    IEntityRequestBuilder,
    IBaseRequestBuilder
  {
    public MessageRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IMessageRequest Request() => this.Request((IEnumerable<Option>) null);

    public IMessageRequest Request(IEnumerable<Option> options)
    {
      return (IMessageRequest) new MessageRequest(this.RequestUrl, this.Client, options);
    }

    public IMessageExtensionsCollectionRequestBuilder Extensions
    {
      get
      {
        return (IMessageExtensionsCollectionRequestBuilder) new MessageExtensionsCollectionRequestBuilder(this.AppendSegmentToRequestUrl("extensions"), this.Client);
      }
    }

    public IMessageAttachmentsCollectionRequestBuilder Attachments
    {
      get
      {
        return (IMessageAttachmentsCollectionRequestBuilder) new MessageAttachmentsCollectionRequestBuilder(this.AppendSegmentToRequestUrl("attachments"), this.Client);
      }
    }

    public IMessageCopyRequestBuilder Copy(string DestinationId = null)
    {
      return (IMessageCopyRequestBuilder) new MessageCopyRequestBuilder(this.AppendSegmentToRequestUrl("microsoft.graph.copy"), this.Client, DestinationId);
    }

    public IMessageMoveRequestBuilder Move(string DestinationId = null)
    {
      return (IMessageMoveRequestBuilder) new MessageMoveRequestBuilder(this.AppendSegmentToRequestUrl("microsoft.graph.move"), this.Client, DestinationId);
    }

    public IMessageCreateReplyRequestBuilder CreateReply()
    {
      return (IMessageCreateReplyRequestBuilder) new MessageCreateReplyRequestBuilder(this.AppendSegmentToRequestUrl("microsoft.graph.createReply"), this.Client);
    }

    public IMessageCreateReplyAllRequestBuilder CreateReplyAll()
    {
      return (IMessageCreateReplyAllRequestBuilder) new MessageCreateReplyAllRequestBuilder(this.AppendSegmentToRequestUrl("microsoft.graph.createReplyAll"), this.Client);
    }

    public IMessageCreateForwardRequestBuilder CreateForward()
    {
      return (IMessageCreateForwardRequestBuilder) new MessageCreateForwardRequestBuilder(this.AppendSegmentToRequestUrl("microsoft.graph.createForward"), this.Client);
    }

    public IMessageReplyRequestBuilder Reply(string Comment = null)
    {
      return (IMessageReplyRequestBuilder) new MessageReplyRequestBuilder(this.AppendSegmentToRequestUrl("microsoft.graph.reply"), this.Client, Comment);
    }

    public IMessageReplyAllRequestBuilder ReplyAll(string Comment = null)
    {
      return (IMessageReplyAllRequestBuilder) new MessageReplyAllRequestBuilder(this.AppendSegmentToRequestUrl("microsoft.graph.replyAll"), this.Client, Comment);
    }

    public IMessageForwardRequestBuilder Forward(
      string Comment = null,
      IEnumerable<Recipient> ToRecipients = null)
    {
      return (IMessageForwardRequestBuilder) new MessageForwardRequestBuilder(this.AppendSegmentToRequestUrl("microsoft.graph.forward"), this.Client, Comment, ToRecipients);
    }

    public IMessageSendRequestBuilder Send()
    {
      return (IMessageSendRequestBuilder) new MessageSendRequestBuilder(this.AppendSegmentToRequestUrl("microsoft.graph.send"), this.Client);
    }
  }
}
