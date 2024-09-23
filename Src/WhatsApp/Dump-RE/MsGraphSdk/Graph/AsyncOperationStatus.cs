// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.AsyncOperationStatus
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
  public class AsyncOperationStatus
  {
    [DataMember(Name = "operation", EmitDefaultValue = false, IsRequired = false)]
    public string Operation { get; set; }

    [DataMember(Name = "percentageComplete", EmitDefaultValue = false, IsRequired = false)]
    public double? PercentageComplete { get; set; }

    [DataMember(Name = "status", EmitDefaultValue = false, IsRequired = false)]
    public string Status { get; set; }

    [JsonExtensionData(ReadData = true)]
    public IDictionary<string, object> AdditionalData { get; set; }
  }
}
