// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.ProvisionedPlan
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
  public class ProvisionedPlan
  {
    [DataMember(Name = "capabilityStatus", EmitDefaultValue = false, IsRequired = false)]
    public string CapabilityStatus { get; set; }

    [DataMember(Name = "provisioningStatus", EmitDefaultValue = false, IsRequired = false)]
    public string ProvisioningStatus { get; set; }

    [DataMember(Name = "service", EmitDefaultValue = false, IsRequired = false)]
    public string Service { get; set; }

    [JsonExtensionData(ReadData = true)]
    public IDictionary<string, object> AdditionalData { get; set; }
  }
}
