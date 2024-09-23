// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.Attachment
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System;
using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.Graph
{
  [DataContract]
  public class Attachment : Entity
  {
    protected internal Attachment()
    {
    }

    [DataMember(Name = "lastModifiedDateTime", EmitDefaultValue = false, IsRequired = false)]
    public DateTimeOffset? LastModifiedDateTime { get; set; }

    [DataMember(Name = "name", EmitDefaultValue = false, IsRequired = false)]
    public string Name { get; set; }

    [DataMember(Name = "contentType", EmitDefaultValue = false, IsRequired = false)]
    public string ContentType { get; set; }

    [DataMember(Name = "size", EmitDefaultValue = false, IsRequired = false)]
    public int? Size { get; set; }

    [DataMember(Name = "isInline", EmitDefaultValue = false, IsRequired = false)]
    public bool? IsInline { get; set; }
  }
}
