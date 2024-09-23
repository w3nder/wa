// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.MailFolder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.Graph
{
  [DataContract]
  public class MailFolder : Entity
  {
    [DataMember(Name = "displayName", EmitDefaultValue = false, IsRequired = false)]
    public string DisplayName { get; set; }

    [DataMember(Name = "parentFolderId", EmitDefaultValue = false, IsRequired = false)]
    public string ParentFolderId { get; set; }

    [DataMember(Name = "childFolderCount", EmitDefaultValue = false, IsRequired = false)]
    public int? ChildFolderCount { get; set; }

    [DataMember(Name = "unreadItemCount", EmitDefaultValue = false, IsRequired = false)]
    public int? UnreadItemCount { get; set; }

    [DataMember(Name = "totalItemCount", EmitDefaultValue = false, IsRequired = false)]
    public int? TotalItemCount { get; set; }

    [DataMember(Name = "messages", EmitDefaultValue = false, IsRequired = false)]
    public IMailFolderMessagesCollectionPage Messages { get; set; }

    [DataMember(Name = "childFolders", EmitDefaultValue = false, IsRequired = false)]
    public IMailFolderChildFoldersCollectionPage ChildFolders { get; set; }
  }
}
