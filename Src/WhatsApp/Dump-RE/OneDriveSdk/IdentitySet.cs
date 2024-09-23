// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.IdentitySet
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
  public class IdentitySet
  {
    [DataMember(Name = "application", EmitDefaultValue = false, IsRequired = false)]
    public Identity Application { get; set; }

    [DataMember(Name = "device", EmitDefaultValue = false, IsRequired = false)]
    public Identity Device { get; set; }

    [DataMember(Name = "user", EmitDefaultValue = false, IsRequired = false)]
    public Identity User { get; set; }

    [JsonExtensionData(ReadData = true)]
    public IDictionary<string, object> AdditionalData { get; set; }
  }
}
