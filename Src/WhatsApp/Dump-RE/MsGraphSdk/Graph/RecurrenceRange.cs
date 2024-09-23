// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.RecurrenceRange
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
  public class RecurrenceRange
  {
    [DataMember(Name = "type", EmitDefaultValue = false, IsRequired = false)]
    public RecurrenceRangeType? Type { get; set; }

    [DataMember(Name = "startDate", EmitDefaultValue = false, IsRequired = false)]
    public Date StartDate { get; set; }

    [DataMember(Name = "endDate", EmitDefaultValue = false, IsRequired = false)]
    public Date EndDate { get; set; }

    [DataMember(Name = "recurrenceTimeZone", EmitDefaultValue = false, IsRequired = false)]
    public string RecurrenceTimeZone { get; set; }

    [DataMember(Name = "numberOfOccurrences", EmitDefaultValue = false, IsRequired = false)]
    public int? NumberOfOccurrences { get; set; }

    [JsonExtensionData(ReadData = true)]
    public IDictionary<string, object> AdditionalData { get; set; }
  }
}
