// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.ServicePlanInfo
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
  public class ServicePlanInfo
  {
    [DataMember(Name = "servicePlanId", EmitDefaultValue = false, IsRequired = false)]
    public Guid? ServicePlanId { get; set; }

    [DataMember(Name = "servicePlanName", EmitDefaultValue = false, IsRequired = false)]
    public string ServicePlanName { get; set; }

    [DataMember(Name = "provisioningStatus", EmitDefaultValue = false, IsRequired = false)]
    public string ProvisioningStatus { get; set; }

    [DataMember(Name = "appliesTo", EmitDefaultValue = false, IsRequired = false)]
    public string AppliesTo { get; set; }

    [JsonExtensionData(ReadData = true)]
    public IDictionary<string, object> AdditionalData { get; set; }
  }
}
