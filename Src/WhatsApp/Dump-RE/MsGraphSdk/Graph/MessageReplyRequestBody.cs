// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.MessageReplyRequestBody
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.Graph
{
  [DataContract]
  public class MessageReplyRequestBody
  {
    [DataMember(Name = "Comment", EmitDefaultValue = false, IsRequired = false)]
    public string Comment { get; set; }
  }
}
