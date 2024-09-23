// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.RecurrencePattern
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
  public class RecurrencePattern
  {
    [DataMember(Name = "type", EmitDefaultValue = false, IsRequired = false)]
    public RecurrencePatternType? Type { get; set; }

    [DataMember(Name = "interval", EmitDefaultValue = false, IsRequired = false)]
    public int? Interval { get; set; }

    [DataMember(Name = "month", EmitDefaultValue = false, IsRequired = false)]
    public int? Month { get; set; }

    [DataMember(Name = "dayOfMonth", EmitDefaultValue = false, IsRequired = false)]
    public int? DayOfMonth { get; set; }

    [DataMember(Name = "daysOfWeek", EmitDefaultValue = false, IsRequired = false)]
    public IEnumerable<DayOfWeek> DaysOfWeek { get; set; }

    [DataMember(Name = "firstDayOfWeek", EmitDefaultValue = false, IsRequired = false)]
    public DayOfWeek? FirstDayOfWeek { get; set; }

    [DataMember(Name = "index", EmitDefaultValue = false, IsRequired = false)]
    public WeekIndex? Index { get; set; }

    [JsonExtensionData(ReadData = true)]
    public IDictionary<string, object> AdditionalData { get; set; }
  }
}
