// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.UploadSession
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.OneDrive.Sdk
{
  [DataContract]
  [JsonConverter(typeof (DerivedTypeConverter))]
  public class UploadSession
  {
    [DataMember(Name = "uploadUrl", EmitDefaultValue = false, IsRequired = false)]
    public string UploadUrl { get; set; }

    [DataMember(Name = "expirationDateTime", EmitDefaultValue = false, IsRequired = false)]
    public DateTimeOffset? ExpirationDateTime { get; set; }

    [DataMember(Name = "nextExpectedRanges", EmitDefaultValue = false, IsRequired = false)]
    public IEnumerable<string> NextExpectedRanges { get; set; }

    [JsonExtensionData(ReadData = true)]
    public IDictionary<string, object> AdditionalData { get; set; }
  }
}
