// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.PhysicalAddress
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
  public class PhysicalAddress
  {
    [DataMember(Name = "street", EmitDefaultValue = false, IsRequired = false)]
    public string Street { get; set; }

    [DataMember(Name = "city", EmitDefaultValue = false, IsRequired = false)]
    public string City { get; set; }

    [DataMember(Name = "state", EmitDefaultValue = false, IsRequired = false)]
    public string State { get; set; }

    [DataMember(Name = "countryOrRegion", EmitDefaultValue = false, IsRequired = false)]
    public string CountryOrRegion { get; set; }

    [DataMember(Name = "postalCode", EmitDefaultValue = false, IsRequired = false)]
    public string PostalCode { get; set; }

    [JsonExtensionData(ReadData = true)]
    public IDictionary<string, object> AdditionalData { get; set; }
  }
}
