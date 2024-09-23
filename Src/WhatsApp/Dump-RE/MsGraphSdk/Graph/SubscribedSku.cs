// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.SubscribedSku
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.Graph
{
  [DataContract]
  public class SubscribedSku : Entity
  {
    [DataMember(Name = "capabilityStatus", EmitDefaultValue = false, IsRequired = false)]
    public string CapabilityStatus { get; set; }

    [DataMember(Name = "consumedUnits", EmitDefaultValue = false, IsRequired = false)]
    public int? ConsumedUnits { get; set; }

    [DataMember(Name = "prepaidUnits", EmitDefaultValue = false, IsRequired = false)]
    public LicenseUnitsDetail PrepaidUnits { get; set; }

    [DataMember(Name = "servicePlans", EmitDefaultValue = false, IsRequired = false)]
    public IEnumerable<ServicePlanInfo> ServicePlans { get; set; }

    [DataMember(Name = "skuId", EmitDefaultValue = false, IsRequired = false)]
    public Guid? SkuId { get; set; }

    [DataMember(Name = "skuPartNumber", EmitDefaultValue = false, IsRequired = false)]
    public string SkuPartNumber { get; set; }

    [DataMember(Name = "appliesTo", EmitDefaultValue = false, IsRequired = false)]
    public string AppliesTo { get; set; }
  }
}
