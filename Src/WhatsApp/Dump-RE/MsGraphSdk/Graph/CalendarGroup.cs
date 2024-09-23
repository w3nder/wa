// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.CalendarGroup
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System;
using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.Graph
{
  [DataContract]
  public class CalendarGroup : Entity
  {
    [DataMember(Name = "name", EmitDefaultValue = false, IsRequired = false)]
    public string Name { get; set; }

    [DataMember(Name = "classId", EmitDefaultValue = false, IsRequired = false)]
    public Guid? ClassId { get; set; }

    [DataMember(Name = "changeKey", EmitDefaultValue = false, IsRequired = false)]
    public string ChangeKey { get; set; }

    [DataMember(Name = "calendars", EmitDefaultValue = false, IsRequired = false)]
    public ICalendarGroupCalendarsCollectionPage Calendars { get; set; }
  }
}
