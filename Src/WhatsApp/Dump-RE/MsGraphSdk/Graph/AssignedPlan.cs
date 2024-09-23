// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.AssignedPlan
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
  public class AssignedPlan
  {
    [DataMember(Name = "assignedDateTime", EmitDefaultValue = false, IsRequired = false)]
    public DateTimeOffset? AssignedDateTime { get; set; }

    [DataMember(Name = "capabilityStatus", EmitDefaultValue = false, IsRequired = false)]
    public string CapabilityStatus { get; set; }

    [DataMember(Name = "service", EmitDefaultValue = false, IsRequired = false)]
    public string Service { get; set; }

    [DataMember(Name = "servicePlanId", EmitDefaultValue = false, IsRequired = false)]
    public Guid? ServicePlanId { get; set; }

    [JsonExtensionData(ReadData = true)]
    public IDictionary<string, object> AdditionalData { get; set; }
  }
}
