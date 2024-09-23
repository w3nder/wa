// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.Hashes
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
  public class Hashes
  {
    [DataMember(Name = "crc32Hash", EmitDefaultValue = false, IsRequired = false)]
    public string Crc32Hash { get; set; }

    [DataMember(Name = "sha1Hash", EmitDefaultValue = false, IsRequired = false)]
    public string Sha1Hash { get; set; }

    [JsonExtensionData(ReadData = true)]
    public IDictionary<string, object> AdditionalData { get; set; }
  }
}
