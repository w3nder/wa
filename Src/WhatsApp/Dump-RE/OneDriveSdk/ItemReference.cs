// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.ItemReference
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
  public class ItemReference
  {
    [DataMember(Name = "driveId", EmitDefaultValue = false, IsRequired = false)]
    public string DriveId { get; set; }

    [DataMember(Name = "id", EmitDefaultValue = false, IsRequired = false)]
    public string Id { get; set; }

    [DataMember(Name = "path", EmitDefaultValue = false, IsRequired = false)]
    public string Path { get; set; }

    [JsonExtensionData(ReadData = true)]
    public IDictionary<string, object> AdditionalData { get; set; }
  }
}
