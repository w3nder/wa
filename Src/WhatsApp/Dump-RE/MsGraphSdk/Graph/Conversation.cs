// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.Conversation
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
  public class Conversation : Entity
  {
    [DataMember(Name = "topic", EmitDefaultValue = false, IsRequired = false)]
    public string Topic { get; set; }

    [DataMember(Name = "hasAttachments", EmitDefaultValue = false, IsRequired = false)]
    public bool? HasAttachments { get; set; }

    [DataMember(Name = "lastDeliveredDateTime", EmitDefaultValue = false, IsRequired = false)]
    public DateTimeOffset? LastDeliveredDateTime { get; set; }

    [DataMember(Name = "uniqueSenders", EmitDefaultValue = false, IsRequired = false)]
    public IEnumerable<string> UniqueSenders { get; set; }

    [DataMember(Name = "preview", EmitDefaultValue = false, IsRequired = false)]
    public string Preview { get; set; }

    [DataMember(Name = "threads", EmitDefaultValue = false, IsRequired = false)]
    public IConversationThreadsCollectionPage Threads { get; set; }
  }
}
