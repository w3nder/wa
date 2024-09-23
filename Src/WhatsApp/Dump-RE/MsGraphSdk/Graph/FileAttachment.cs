// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.FileAttachment
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.Graph
{
  [DataContract]
  public class FileAttachment : Attachment
  {
    [DataMember(Name = "contentId", EmitDefaultValue = false, IsRequired = false)]
    public string ContentId { get; set; }

    [DataMember(Name = "contentLocation", EmitDefaultValue = false, IsRequired = false)]
    public string ContentLocation { get; set; }

    [DataMember(Name = "contentBytes", EmitDefaultValue = false, IsRequired = false)]
    public byte[] ContentBytes { get; set; }
  }
}
