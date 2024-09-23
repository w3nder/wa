// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.ItemCopyRequestBody
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.OneDrive.Sdk
{
  [DataContract]
  public class ItemCopyRequestBody
  {
    [DataMember(Name = "name", EmitDefaultValue = false, IsRequired = false)]
    public string Name { get; set; }

    [DataMember(Name = "parentReference", EmitDefaultValue = false, IsRequired = false)]
    public ItemReference ParentReference { get; set; }
  }
}
