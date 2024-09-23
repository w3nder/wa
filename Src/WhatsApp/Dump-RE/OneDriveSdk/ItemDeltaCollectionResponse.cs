// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.ItemDeltaCollectionResponse
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.OneDrive.Sdk
{
  [DataContract]
  public class ItemDeltaCollectionResponse
  {
    [DataMember(Name = "value", EmitDefaultValue = false, IsRequired = false)]
    public IItemDeltaCollectionPage Value { get; set; }

    [DataMember(Name = "@delta.token", EmitDefaultValue = false, IsRequired = false)]
    public string Token { get; set; }

    [DataMember(Name = "@delta.deltaLink", EmitDefaultValue = false, IsRequired = false)]
    public string DeltaLink { get; set; }

    [JsonExtensionData(ReadData = true)]
    public IDictionary<string, object> AdditionalData { get; set; }
  }
}
