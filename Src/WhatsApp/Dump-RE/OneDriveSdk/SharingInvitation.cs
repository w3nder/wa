// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.SharingInvitation
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
  public class SharingInvitation
  {
    [DataMember(Name = "email", EmitDefaultValue = false, IsRequired = false)]
    public string Email { get; set; }

    [DataMember(Name = "invitedBy", EmitDefaultValue = false, IsRequired = false)]
    public IdentitySet InvitedBy { get; set; }

    [DataMember(Name = "signInRequired", EmitDefaultValue = false, IsRequired = false)]
    public bool? SignInRequired { get; set; }

    [DataMember(Name = "sendInvitationStatus", EmitDefaultValue = false, IsRequired = false)]
    public string SendInvitationStatus { get; set; }

    [DataMember(Name = "inviteErrorResolveUrl", EmitDefaultValue = false, IsRequired = false)]
    public string InviteErrorResolveUrl { get; set; }

    [JsonExtensionData(ReadData = true)]
    public IDictionary<string, object> AdditionalData { get; set; }
  }
}
