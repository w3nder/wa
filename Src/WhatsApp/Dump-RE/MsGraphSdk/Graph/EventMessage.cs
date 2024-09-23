// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.EventMessage
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.Graph
{
  [DataContract]
  public class EventMessage : Message
  {
    [DataMember(Name = "meetingMessageType", EmitDefaultValue = false, IsRequired = false)]
    public Microsoft.Graph.MeetingMessageType? MeetingMessageType { get; set; }

    [DataMember(Name = "event", EmitDefaultValue = false, IsRequired = false)]
    public Event Event { get; set; }
  }
}
