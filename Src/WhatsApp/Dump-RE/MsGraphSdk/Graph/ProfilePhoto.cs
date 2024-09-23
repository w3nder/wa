// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.ProfilePhoto
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.Graph
{
  [DataContract]
  public class ProfilePhoto : Entity
  {
    [DataMember(Name = "height", EmitDefaultValue = false, IsRequired = false)]
    public int? Height { get; set; }

    [DataMember(Name = "width", EmitDefaultValue = false, IsRequired = false)]
    public int? Width { get; set; }
  }
}
