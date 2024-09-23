// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.Permission
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
  public class Permission
  {
    [DataMember(Name = "grantedTo", EmitDefaultValue = false, IsRequired = false)]
    public IdentitySet GrantedTo { get; set; }

    [DataMember(Name = "id", EmitDefaultValue = false, IsRequired = false)]
    public string Id { get; set; }

    [DataMember(Name = "invitation", EmitDefaultValue = false, IsRequired = false)]
    public SharingInvitation Invitation { get; set; }

    [DataMember(Name = "inheritedFrom", EmitDefaultValue = false, IsRequired = false)]
    public ItemReference InheritedFrom { get; set; }

    [DataMember(Name = "link", EmitDefaultValue = false, IsRequired = false)]
    public SharingLink Link { get; set; }

    [DataMember(Name = "roles", EmitDefaultValue = false, IsRequired = false)]
    public IEnumerable<string> Roles { get; set; }

    [DataMember(Name = "shareId", EmitDefaultValue = false, IsRequired = false)]
    public string ShareId { get; set; }

    [DataMember(Name = "@odata.type", EmitDefaultValue = false, IsRequired = false)]
    public string ODataType { get; set; }

    [JsonExtensionData(ReadData = true, WriteData = true)]
    public IDictionary<string, object> AdditionalData { get; set; }
  }
}
