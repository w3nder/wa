// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.SharingInvitation
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.Graph
{
  [DataContract]
  [JsonConverter(typeof (DerivedTypeConverter))]
  public class SharingInvitation
  {
    [DataMember(Name = "email", EmitDefaultValue = false, IsRequired = false)]
    public string Email { get; set; }

    [DataMember(Name = "invitedBy", EmitDefaultValue = false, IsRequired = false)]
    public IdentitySet InvitedBy { get; set; }

    [DataMember(Name = "redeemedBy", EmitDefaultValue = false, IsRequired = false)]
    public string RedeemedBy { get; set; }

    [DataMember(Name = "signInRequired", EmitDefaultValue = false, IsRequired = false)]
    public bool? SignInRequired { get; set; }

    [JsonExtensionData(ReadData = true)]
    public IDictionary<string, object> AdditionalData { get; set; }
  }
}
