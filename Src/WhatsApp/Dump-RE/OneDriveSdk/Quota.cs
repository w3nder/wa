// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.Quota
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.OneDrive.Sdk
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
