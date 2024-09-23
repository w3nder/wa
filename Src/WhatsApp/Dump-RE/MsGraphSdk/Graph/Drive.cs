// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.Drive
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.Graph
{
  [DataContract]
  public class Drive : Entity
  {
    [DataMember(Name = "driveType", EmitDefaultValue = false, IsRequired = false)]
    public string DriveType { get; set; }

    [DataMember(Name = "owner", EmitDefaultValue = false, IsRequired = false)]
    public IdentitySet Owner { get; set; }

    [DataMember(Name = "quota", EmitDefaultValue = false, IsRequired = false)]
    public Quota Quota { get; set; }

    [DataMember(Name = "items", EmitDefaultValue = false, IsRequired = false)]
    public IDriveItemsCollectionPage Items { get; set; }

    [DataMember(Name = "special", EmitDefaultValue = false, IsRequired = false)]
    public IDriveSpecialCollectionPage Special { get; set; }

    [DataMember(Name = "root", EmitDefaultValue = false, IsRequired = false)]
    public DriveItem Root { get; set; }
  }
}
