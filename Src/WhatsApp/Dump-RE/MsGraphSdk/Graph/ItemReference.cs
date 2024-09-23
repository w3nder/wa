// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.ItemReference
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.Graph
{
  [DataContract]
  [JsonConverter(typeof (DerivedTypeConverter))]
  public class ItemReference
  {
    [DataMember(Name = "driveId", EmitDefaultValue = false, IsRequired = false)]
    public string DriveId { get; set; }

    [DataMember(Name = "id", EmitDefaultValue = false, IsRequired = false)]
    public string Id { get; set; }

    [DataMember(Name = "path", EmitDefaultValue = false, IsRequired = false)]
    public string Path { get; set; }

    [JsonExtensionData(ReadData = true)]
    public IDictionary<string, object> AdditionalData { get; set; }
  }
}
