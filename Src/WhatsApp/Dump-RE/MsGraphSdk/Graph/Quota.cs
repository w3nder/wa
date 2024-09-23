// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.Quota
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
  public class Quota
  {
    [DataMember(Name = "deleted", EmitDefaultValue = false, IsRequired = false)]
    public long? Deleted { get; set; }

    [DataMember(Name = "remaining", EmitDefaultValue = false, IsRequired = false)]
    public long? Remaining { get; set; }

    [DataMember(Name = "state", EmitDefaultValue = false, IsRequired = false)]
    public string State { get; set; }

    [DataMember(Name = "total", EmitDefaultValue = false, IsRequired = false)]
    public long? Total { get; set; }

    [DataMember(Name = "used", EmitDefaultValue = false, IsRequired = false)]
    public long? Used { get; set; }

    [JsonExtensionData(ReadData = true)]
    public IDictionary<string, object> AdditionalData { get; set; }
  }
}
