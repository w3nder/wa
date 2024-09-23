// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.Post
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
  public class Post : OutlookItem
  {
    [DataMember(Name = "body", EmitDefaultValue = false, IsRequired = false)]
    public ItemBody Body { get; set; }

    [DataMember(Name = "receivedDateTime", EmitDefaultValue = false, IsRequired = false)]
    public DateTimeOffset? ReceivedDateTime { get; set; }

    [DataMember(Name = "hasAttachments", EmitDefaultValue = false, IsRequired = false)]
    public bool? HasAttachments { get; set; }

    [DataMember(Name = "from", EmitDefaultValue = false, IsRequired = false)]
    public Recipient From { get; set; }

    [DataMember(Name = "sender", EmitDefaultValue = false, IsRequired = false)]
    public Recipient Sender { get; set; }

    [DataMember(Name = "conversationThreadId", EmitDefaultValue = false, IsRequired = false)]
    public string ConversationThreadId { get; set; }

    [DataMember(Name = "newParticipants", EmitDefaultValue = false, IsRequired = false)]
    public IEnumerable<Recipient> NewParticipants { get; set; }

    [DataMember(Name = "conversationId", EmitDefaultValue = false, IsRequired = false)]
    public string ConversationId { get; set; }

    [DataMember(Name = "extensions", EmitDefaultValue = false, IsRequired = false)]
    public IPostExtensionsCollectionPage Extensions { get; set; }

    [DataMember(Name = "inReplyTo", EmitDefaultValue = false, IsRequired = false)]
    public Post InReplyTo { get; set; }

    [DataMember(Name = "attachments", EmitDefaultValue = false, IsRequired = false)]
    public IPostAttachmentsCollectionPage Attachments { get; set; }
  }
}
