// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.OpenWithApp
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
  public class OpenWithApp
  {
    [DataMember(Name = "app", EmitDefaultValue = false, IsRequired = false)]
    public Identity App { get; set; }

    [DataMember(Name = "viewUrl", EmitDefaultValue = false, IsRequired = false)]
    public string ViewUrl { get; set; }

    [DataMember(Name = "editUrl", EmitDefaultValue = false, IsRequired = false)]
    public string EditUrl { get; set; }

    [DataMember(Name = "viewPostParameters", EmitDefaultValue = false, IsRequired = false)]
    public string ViewPostParameters { get; set; }

    [DataMember(Name = "editPostParameters", EmitDefaultValue = false, IsRequired = false)]
    public string EditPostParameters { get; set; }

    [JsonExtensionData(ReadData = true)]
    public IDictionary<string, object> AdditionalData { get; set; }
  }
}
