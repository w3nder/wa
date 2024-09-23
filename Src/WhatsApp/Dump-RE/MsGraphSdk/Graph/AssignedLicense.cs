// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.AssignedLicense
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.Graph
{
  [DataContract]
  [JsonConverter(typeof (DerivedTypeConverter))]
  public class AssignedLicense
  {
    [DataMember(Name = "disabledPlans", EmitDefaultValue = false, IsRequired = false)]
    public IEnumerable<Guid> DisabledPlans { get; set; }

    [DataMember(Name = "skuId", EmitDefaultValue = false, IsRequired = false)]
    public Guid? SkuId { get; set; }

    [JsonExtensionData(ReadData = true)]
    public IDictionary<string, object> AdditionalData { get; set; }
  }
}
