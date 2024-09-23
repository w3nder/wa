// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.Message
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.Graph
{
  [DataContract]
  public class Message : OutlookItem
  {
    [DataMember(Name = "receivedDateTime", EmitDefaultValue = false, IsRequired = false)]
    public DateTimeOffset? ReceivedDateTime { get; set; }

    [DataMember(Name = "sentDateTime", EmitDefaultValue = false, IsRequired = false)]
    public DateTimeOffset? SentDateTime { get; set; }

    [DataMember(Name = "hasAttachments", EmitDefaultValue = false, IsRequired = false)]
    public bool? HasAttachments { get; set; }

    [DataMember(Name = "internetMessageId", EmitDefaultValue = false, IsRequired = false)]
    public string InternetMessageId { get; set; }

    [DataMember(Name = "subject", EmitDefaultValue = false, IsRequired = false)]
    public string Subject { get; set; }

    [DataMember(Name = "body", EmitDefaultValue = false, IsRequired = false)]
    public ItemBody Body { get; set; }

    [DataMember(Name = "bodyPreview", EmitDefaultValue = false, IsRequired = false)]
    public string BodyPreview { get; set; }

    [DataMember(Name = "importance", EmitDefaultValue = false, IsRequired = false)]
    public Microsoft.Graph.Importance? Importance { get; set; }

    [DataMember(Name = "parentFolderId", EmitDefaultValue = false, IsRequired = false)]
    public string ParentFolderId { get; set; }

    [DataMember(Name = "sender", EmitDefaultValue = false, IsRequired = false)]
    public Recipient Sender { get; set; }

    [DataMember(Name = "from", EmitDefaultValue = false, IsRequired = false)]
    public Recipient From { get; set; }

    [DataMember(Name = "toRecipients", EmitDefaultValue = false, IsRequired = false)]
    public IEnumerable<Recipient> ToRecipients { get; set; }

    [DataMember(Name = "ccRecipients", EmitDefaultValue = false, IsRequired = false)]
    public IEnumerable<Recipient> CcRecipients { get; set; }

    [DataMember(Name = "bccRecipients", EmitDefaultValue = false, IsRequired = false)]
    public IEnumerable<Recipient> BccRecipients { get; set; }

    [DataMember(Name = "replyTo", EmitDefaultValue = false, IsRequired = false)]
    public IEnumerable<Recipient> ReplyTo { get; set; }

    [DataMember(Name = "conversationId", EmitDefaultValue = false, IsRequired = false)]
    public string ConversationId { get; set; }

    [DataMember(Name = "uniqueBody", EmitDefaultValue = false, IsRequired = false)]
    public ItemBody UniqueBody { get; set; }

    [DataMember(Name = "isDeliveryReceiptRequested", EmitDefaultValue = false, IsRequired = false)]
    public bool? IsDeliveryReceiptRequested { get; set; }

    [DataMember(Name = "isReadReceiptRequested", EmitDefaultValue = false, IsRequired = false)]
    public bool? IsReadReceiptRequested { get; set; }

    [DataMember(Name = "isRead", EmitDefaultValue = false, IsRequired = false)]
    public bool? IsRead { get; set; }

    [DataMember(Name = "isDraft", EmitDefaultValue = false, IsRequired = false)]
    public bool? IsDraft { get; set; }

    [DataMember(Name = "webLink", EmitDefaultValue = false, IsRequired = false)]
    public string WebLink { get; set; }

    [DataMember(Name = "inferenceClassification", EmitDefaultValue = false, IsRequired = false)]
    public InferenceClassificationType? InferenceClassification { get; set; }

    [DataMember(Name = "extensions", EmitDefaultValue = false, IsRequired = false)]
    public IMessageExtensionsCollectionPage Extensions { get; set; }

    [DataMember(Name = "attachments", EmitDefaultValue = false, IsRequired = false)]
    public IMessageAttachmentsCollectionPage Attachments { get; set; }
  }
}
