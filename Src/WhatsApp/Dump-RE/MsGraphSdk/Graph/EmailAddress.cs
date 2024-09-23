// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.EmailAddress
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
  public class EmailAddress
  {
    [DataMember(Name = "name", EmitDefaultValue = false, IsRequired = false)]
    public string Name { get; set; }

    [DataMember(Name = "address", EmitDefaultValue = false, IsRequired = false)]
    public string Address { get; set; }

    [JsonExtensionData(ReadData = true)]
    public IDictionary<string, object> AdditionalData { get; set; }
  }
}
