// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.SpecialFolder
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
  public class SpecialFolder
  {
    [DataMember(Name = "name", EmitDefaultValue = false, IsRequired = false)]
    public string Name { get; set; }

    [JsonExtensionData(ReadData = true)]
    public IDictionary<string, object> AdditionalData { get; set; }
  }
}
