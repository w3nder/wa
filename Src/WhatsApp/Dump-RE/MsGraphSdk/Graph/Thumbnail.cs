// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.Thumbnail
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.Graph
{
  [DataContract]
  [JsonConverter(typeof (DerivedTypeConverter))]
  public class Thumbnail
  {
    [DataMember(Name = "content", EmitDefaultValue = false, IsRequired = false)]
    public Stream Content { get; set; }

    [DataMember(Name = "height", EmitDefaultValue = false, IsRequired = false)]
    public int? Height { get; set; }

    [DataMember(Name = "url", EmitDefaultValue = false, IsRequired = false)]
    public string Url { get; set; }

    [DataMember(Name = "width", EmitDefaultValue = false, IsRequired = false)]
    public int? Width { get; set; }

    [JsonExtensionData(ReadData = true)]
    public IDictionary<string, object> AdditionalData { get; set; }
  }
}
