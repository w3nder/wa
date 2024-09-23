// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.RemoteItem
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
  public class RemoteItem
  {
    [DataMember(Name = "file", EmitDefaultValue = false, IsRequired = false)]
    public File File { get; set; }

    [DataMember(Name = "fileSystemInfo", EmitDefaultValue = false, IsRequired = false)]
    public FileSystemInfo FileSystemInfo { get; set; }

    [DataMember(Name = "folder", EmitDefaultValue = false, IsRequired = false)]
    public Folder Folder { get; set; }

    [DataMember(Name = "id", EmitDefaultValue = false, IsRequired = false)]
    public string Id { get; set; }

    [DataMember(Name = "name", EmitDefaultValue = false, IsRequired = false)]
    public string Name { get; set; }

    [DataMember(Name = "parentReference", EmitDefaultValue = false, IsRequired = false)]
    public ItemReference ParentReference { get; set; }

    [DataMember(Name = "size", EmitDefaultValue = false, IsRequired = false)]
    public long? Size { get; set; }

    [JsonExtensionData(ReadData = true)]
    public IDictionary<string, object> AdditionalData { get; set; }
  }
}
