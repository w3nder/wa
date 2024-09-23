// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.Permission
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.Graph
{
  [DataContract]
  public class Permission : Entity
  {
    [DataMember(Name = "grantedTo", EmitDefaultValue = false, IsRequired = false)]
    public IdentitySet GrantedTo { get; set; }

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
  }
}
