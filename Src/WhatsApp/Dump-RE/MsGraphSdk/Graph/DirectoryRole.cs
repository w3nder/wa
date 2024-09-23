// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.DirectoryRole
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.Graph
{
  [DataContract]
  public class DirectoryRole : DirectoryObject
  {
    [DataMember(Name = "description", EmitDefaultValue = false, IsRequired = false)]
    public string Description { get; set; }

    [DataMember(Name = "displayName", EmitDefaultValue = false, IsRequired = false)]
    public string DisplayName { get; set; }

    [DataMember(Name = "roleTemplateId", EmitDefaultValue = false, IsRequired = false)]
    public string RoleTemplateId { get; set; }

    [DataMember(Name = "members", EmitDefaultValue = false, IsRequired = false)]
    public IDirectoryRoleMembersCollectionWithReferencesPage Members { get; set; }
  }
}
