// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.ThumbnailSet
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
  public class ThumbnailSet
  {
    [DataMember(Name = "id", EmitDefaultValue = false, IsRequired = false)]
    public string Id { get; set; }

    [DataMember(Name = "large", EmitDefaultValue = false, IsRequired = false)]
    public Thumbnail Large { get; set; }

    [DataMember(Name = "medium", EmitDefaultValue = false, IsRequired = false)]
    public Thumbnail Medium { get; set; }

    [DataMember(Name = "small", EmitDefaultValue = false, IsRequired = false)]
    public Thumbnail Small { get; set; }

    [DataMember(Name = "source", EmitDefaultValue = false, IsRequired = false)]
    public Thumbnail Source { get; set; }

    [DataMember(Name = "@odata.type", EmitDefaultValue = false, IsRequired = false)]
    public string ODataType { get; set; }

    [JsonExtensionData(ReadData = true, WriteData = true)]
    public IDictionary<string, object> AdditionalData { get; set; }

    public Thumbnail this[string customThumbnailName]
    {
      get
      {
        object obj;
        return this.AdditionalData != null && this.AdditionalData.TryGetValue(customThumbnailName, out obj) ? obj as Thumbnail : (Thumbnail) null;
      }
    }
  }
}
