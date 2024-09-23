// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.VerifiedDomain
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
  public class VerifiedDomain
  {
    [DataMember(Name = "capabilities", EmitDefaultValue = false, IsRequired = false)]
    public string Capabilities { get; set; }

    [DataMember(Name = "isDefault", EmitDefaultValue = false, IsRequired = false)]
    public bool? IsDefault { get; set; }

    [DataMember(Name = "isInitial", EmitDefaultValue = false, IsRequired = false)]
    public bool? IsInitial { get; set; }

    [DataMember(Name = "name", EmitDefaultValue = false, IsRequired = false)]
    public string Name { get; set; }

    [DataMember(Name = "type", EmitDefaultValue = false, IsRequired = false)]
    public string Type { get; set; }

    [JsonExtensionData(ReadData = true)]
    public IDictionary<string, object> AdditionalData { get; set; }
  }
}
