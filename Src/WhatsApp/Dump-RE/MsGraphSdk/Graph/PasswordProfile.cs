// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.PasswordProfile
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
  public class PasswordProfile
  {
    [DataMember(Name = "password", EmitDefaultValue = false, IsRequired = false)]
    public string Password { get; set; }

    [DataMember(Name = "forceChangePasswordNextSignIn", EmitDefaultValue = false, IsRequired = false)]
    public bool? ForceChangePasswordNextSignIn { get; set; }

    [JsonExtensionData(ReadData = true)]
    public IDictionary<string, object> AdditionalData { get; set; }
  }
}
