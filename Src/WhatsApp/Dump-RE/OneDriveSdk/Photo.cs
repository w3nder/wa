// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.Photo
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
  public class Photo
  {
    [DataMember(Name = "cameraMake", EmitDefaultValue = false, IsRequired = false)]
    public string CameraMake { get; set; }

    [DataMember(Name = "cameraModel", EmitDefaultValue = false, IsRequired = false)]
    public string CameraModel { get; set; }

    [DataMember(Name = "exposureDenominator", EmitDefaultValue = false, IsRequired = false)]
    public double? ExposureDenominator { get; set; }

    [DataMember(Name = "exposureNumerator", EmitDefaultValue = false, IsRequired = false)]
    public double? ExposureNumerator { get; set; }

    [DataMember(Name = "focalLength", EmitDefaultValue = false, IsRequired = false)]
    public double? FocalLength { get; set; }

    [DataMember(Name = "fNumber", EmitDefaultValue = false, IsRequired = false)]
    public double? FNumber { get; set; }

    [DataMember(Name = "takenDateTime", EmitDefaultValue = false, IsRequired = false)]
    public DateTimeOffset? TakenDateTime { get; set; }

    [DataMember(Name = "iso", EmitDefaultValue = false, IsRequired = false)]
    public int? Iso { get; set; }

    [JsonExtensionData(ReadData = true)]
    public IDictionary<string, object> AdditionalData { get; set; }
  }
}
