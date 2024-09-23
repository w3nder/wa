// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IMessageRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public interface IMessageRequestBuilder : 
    IOutlookItemRequestBuilder,
    IEntityRequestBuilder,
    IBaseRequestBuilder
  {
    IMessageRequest Request();

    IMessageRequest Request(IEnumerable<Option> options);

    IMessageExtensionsCollectionRequestBuilder Extensions { get; }

    IMessageAttachmentsCollectionRequestBuilder Attachments { get; }

    IMessageCopyRequestBuilder Copy(string DestinationId = null);

    IMessageMoveRequestBuilder Move(string DestinationId = null);

    IMessageCreateReplyRequestBuilder CreateReply();

    IMessageCreateReplyAllRequestBuilder CreateReplyAll();

    IMessageCreateForwardRequestBuilder CreateForward();

    IMessageReplyRequestBuilder Reply(string Comment = null);

    IMessageReplyAllRequestBuilder ReplyAll(string Comment = null);

    IMessageForwardRequestBuilder Forward(string Comment = null, IEnumerable<Recipient> ToRecipients = null);

    IMessageSendRequestBuilder Send();
  }
}
