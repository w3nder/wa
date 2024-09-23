// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.Drive
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
  public class Drive
  {
    [DataMember(Name = "id", EmitDefaultValue = false, IsRequired = false)]
    public string Id { get; set; }

    [DataMember(Name = "driveType", EmitDefaultValue = false, IsRequired = false)]
    public string DriveType { get; set; }

    [DataMember(Name = "owner", EmitDefaultValue = false, IsRequired = false)]
    public IdentitySet Owner { get; set; }

    [DataMember(Name = "quota", EmitDefaultValue = false, IsRequired = false)]
    public Quota Quota { get; set; }

    [DataMember(Name = "items", EmitDefaultValue = false, IsRequired = false)]
    public IDriveItemsCollectionPage Items { get; set; }

    [DataMember(Name = "shared", EmitDefaultValue = false, IsRequired = false)]
    public IDriveSharedCollectionPage Shared { get; set; }

    [DataMember(Name = "special", EmitDefaultValue = false, IsRequired = false)]
    public IDriveSpecialCollectionPage Special { get; set; }

    [DataMember(Name = "@odata.type", EmitDefaultValue = false, IsRequired = false)]
    public string ODataType { get; set; }

    [JsonExtensionData(ReadData = true, WriteData = true)]
    public IDictionary<string, object> AdditionalData { get; set; }
  }
}
