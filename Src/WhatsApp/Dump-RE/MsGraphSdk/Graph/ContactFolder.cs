// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.ContactFolder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.Graph
{
  [DataContract]
  public class ContactFolder : Entity
  {
    [DataMember(Name = "parentFolderId", EmitDefaultValue = false, IsRequired = false)]
    public string ParentFolderId { get; set; }

    [DataMember(Name = "displayName", EmitDefaultValue = false, IsRequired = false)]
    public string DisplayName { get; set; }

    [DataMember(Name = "contacts", EmitDefaultValue = false, IsRequired = false)]
    public IContactFolderContactsCollectionPage Contacts { get; set; }

    [DataMember(Name = "childFolders", EmitDefaultValue = false, IsRequired = false)]
    public IContactFolderChildFoldersCollectionPage ChildFolders { get; set; }
  }
}
