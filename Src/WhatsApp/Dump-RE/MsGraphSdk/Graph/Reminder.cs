// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.Reminder
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
  public class Reminder
  {
    [DataMember(Name = "eventId", EmitDefaultValue = false, IsRequired = false)]
    public string EventId { get; set; }

    [DataMember(Name = "eventStartTime", EmitDefaultValue = false, IsRequired = false)]
    public DateTimeTimeZone EventStartTime { get; set; }

    [DataMember(Name = "eventEndTime", EmitDefaultValue = false, IsRequired = false)]
    public DateTimeTimeZone EventEndTime { get; set; }

    [DataMember(Name = "changeKey", EmitDefaultValue = false, IsRequired = false)]
    public string ChangeKey { get; set; }

    [DataMember(Name = "eventSubject", EmitDefaultValue = false, IsRequired = false)]
    public string EventSubject { get; set; }

    [DataMember(Name = "eventLocation", EmitDefaultValue = false, IsRequired = false)]
    public Location EventLocation { get; set; }

    [DataMember(Name = "eventWebLink", EmitDefaultValue = false, IsRequired = false)]
    public string EventWebLink { get; set; }

    [DataMember(Name = "reminderFireTime", EmitDefaultValue = false, IsRequired = false)]
    public DateTimeTimeZone ReminderFireTime { get; set; }

    [JsonExtensionData(ReadData = true)]
    public IDictionary<string, object> AdditionalData { get; set; }
  }
}
