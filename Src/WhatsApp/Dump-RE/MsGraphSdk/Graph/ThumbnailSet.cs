// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.ThumbnailSet
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.Graph
{
  [DataContract]
  public class ThumbnailSet : Entity
  {
    [DataMember(Name = "large", EmitDefaultValue = false, IsRequired = false)]
    public Thumbnail Large { get; set; }

    [DataMember(Name = "medium", EmitDefaultValue = false, IsRequired = false)]
    public Thumbnail Medium { get; set; }

    [DataMember(Name = "small", EmitDefaultValue = false, IsRequired = false)]
    public Thumbnail Small { get; set; }

    [DataMember(Name = "source", EmitDefaultValue = false, IsRequired = false)]
    public Thumbnail Source { get; set; }

    public Thumbnail this[string customThumbnailName]
    {
      get
      {
        object obj;
        return this.AdditionalData != null && this.AdditionalData.TryGetValue(customThumbnailName, out obj) ? obj as Thumbnail : (Thumbnail) null;
      }
    }
  }
}
