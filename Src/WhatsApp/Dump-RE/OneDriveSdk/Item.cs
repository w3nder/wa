// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.Item
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.OneDrive.Sdk
{
  [DataContract]
  [JsonConverter(typeof (DerivedTypeConverter))]
  public class Item
  {
    [DataMember(Name = "content", EmitDefaultValue = false, IsRequired = false)]
    public Stream Content { get; set; }

    [DataMember(Name = "createdBy", EmitDefaultValue = false, IsRequired = false)]
    public IdentitySet CreatedBy { get; set; }

    [DataMember(Name = "createdDateTime", EmitDefaultValue = false, IsRequired = false)]
    public DateTimeOffset? CreatedDateTime { get; set; }

    [DataMember(Name = "cTag", EmitDefaultValue = false, IsRequired = false)]
    public string CTag { get; set; }

    [DataMember(Name = "description", EmitDefaultValue = false, IsRequired = false)]
    public string Description { get; set; }

    [DataMember(Name = "eTag", EmitDefaultValue = false, IsRequired = false)]
    public string ETag { get; set; }

    [DataMember(Name = "id", EmitDefaultValue = false, IsRequired = false)]
    public string Id { get; set; }

    [DataMember(Name = "lastModifiedBy", EmitDefaultValue = false, IsRequired = false)]
    public IdentitySet LastModifiedBy { get; set; }

    [DataMember(Name = "lastModifiedDateTime", EmitDefaultValue = false, IsRequired = false)]
    public DateTimeOffset? LastModifiedDateTime { get; set; }

    [DataMember(Name = "name", EmitDefaultValue = false, IsRequired = false)]
    public string Name { get; set; }

    [DataMember(Name = "parentReference", EmitDefaultValue = false, IsRequired = false)]
    public ItemReference ParentReference { get; set; }

    [DataMember(Name = "size", EmitDefaultValue = false, IsRequired = false)]
    public long? Size { get; set; }

    [DataMember(Name = "webUrl", EmitDefaultValue = false, IsRequired = false)]
    public string WebUrl { get; set; }

    [DataMember(Name = "audio", EmitDefaultValue = false, IsRequired = false)]
    public Audio Audio { get; set; }

    [DataMember(Name = "deleted", EmitDefaultValue = false, IsRequired = false)]
    public Deleted Deleted { get; set; }

    [DataMember(Name = "file", EmitDefaultValue = false, IsRequired = false)]
    public File File { get; set; }

    [DataMember(Name = "fileSystemInfo", EmitDefaultValue = false, IsRequired = false)]
    public FileSystemInfo FileSystemInfo { get; set; }

    [DataMember(Name = "folder", EmitDefaultValue = false, IsRequired = false)]
    public Folder Folder { get; set; }

    [DataMember(Name = "image", EmitDefaultValue = false, IsRequired = false)]
    public Image Image { get; set; }

    [DataMember(Name = "location", EmitDefaultValue = false, IsRequired = false)]
    public Location Location { get; set; }

    [DataMember(Name = "openWith", EmitDefaultValue = false, IsRequired = false)]
    public OpenWithSet OpenWith { get; set; }

    [DataMember(Name = "photo", EmitDefaultValue = false, IsRequired = false)]
    public Photo Photo { get; set; }

    [DataMember(Name = "remoteItem", EmitDefaultValue = false, IsRequired = false)]
    public Item RemoteItem { get; set; }

    [DataMember(Name = "searchResult", EmitDefaultValue = false, IsRequired = false)]
    public SearchResult SearchResult { get; set; }

    [DataMember(Name = "shared", EmitDefaultValue = false, IsRequired = false)]
    public Shared Shared { get; set; }

    [DataMember(Name = "specialFolder", EmitDefaultValue = false, IsRequired = false)]
    public SpecialFolder SpecialFolder { get; set; }

    [DataMember(Name = "video", EmitDefaultValue = false, IsRequired = false)]
    public Video Video { get; set; }

    [DataMember(Name = "permissions", EmitDefaultValue = false, IsRequired = false)]
    public IItemPermissionsCollectionPage Permissions { get; set; }

    [DataMember(Name = "versions", EmitDefaultValue = false, IsRequired = false)]
    public IItemVersionsCollectionPage Versions { get; set; }

    [DataMember(Name = "children", EmitDefaultValue = false, IsRequired = false)]
    public IItemChildrenCollectionPage Children { get; set; }

    [DataMember(Name = "thumbnails", EmitDefaultValue = false, IsRequired = false)]
    public IItemThumbnailsCollectionPage Thumbnails { get; set; }

    [DataMember(Name = "@odata.type", EmitDefaultValue = false, IsRequired = false)]
    public string ODataType { get; set; }

    [JsonExtensionData(ReadData = true, WriteData = true)]
    public IDictionary<string, object> AdditionalData { get; set; }
  }
}
