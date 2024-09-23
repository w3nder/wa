// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.Event
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.Graph
{
  [DataContract]
  public class Event : OutlookItem
  {
    [DataMember(Name = "originalStartTimeZone", EmitDefaultValue = false, IsRequired = false)]
    public string OriginalStartTimeZone { get; set; }

    [DataMember(Name = "originalEndTimeZone", EmitDefaultValue = false, IsRequired = false)]
    public string OriginalEndTimeZone { get; set; }

    [DataMember(Name = "responseStatus", EmitDefaultValue = false, IsRequired = false)]
    public ResponseStatus ResponseStatus { get; set; }

    [DataMember(Name = "iCalUId", EmitDefaultValue = false, IsRequired = false)]
    public string ICalUId { get; set; }

    [DataMember(Name = "reminderMinutesBeforeStart", EmitDefaultValue = false, IsRequired = false)]
    public int? ReminderMinutesBeforeStart { get; set; }

    [DataMember(Name = "isReminderOn", EmitDefaultValue = false, IsRequired = false)]
    public bool? IsReminderOn { get; set; }

    [DataMember(Name = "hasAttachments", EmitDefaultValue = false, IsRequired = false)]
    public bool? HasAttachments { get; set; }

    [DataMember(Name = "subject", EmitDefaultValue = false, IsRequired = false)]
    public string Subject { get; set; }

    [DataMember(Name = "body", EmitDefaultValue = false, IsRequired = false)]
    public ItemBody Body { get; set; }

    [DataMember(Name = "bodyPreview", EmitDefaultValue = false, IsRequired = false)]
    public string BodyPreview { get; set; }

    [DataMember(Name = "importance", EmitDefaultValue = false, IsRequired = false)]
    public Microsoft.Graph.Importance? Importance { get; set; }

    [DataMember(Name = "sensitivity", EmitDefaultValue = false, IsRequired = false)]
    public Microsoft.Graph.Sensitivity? Sensitivity { get; set; }

    [DataMember(Name = "start", EmitDefaultValue = false, IsRequired = false)]
    public DateTimeTimeZone Start { get; set; }

    [DataMember(Name = "originalStart", EmitDefaultValue = false, IsRequired = false)]
    public DateTimeOffset? OriginalStart { get; set; }

    [DataMember(Name = "end", EmitDefaultValue = false, IsRequired = false)]
    public DateTimeTimeZone End { get; set; }

    [DataMember(Name = "location", EmitDefaultValue = false, IsRequired = false)]
    public Location Location { get; set; }

    [DataMember(Name = "isAllDay", EmitDefaultValue = false, IsRequired = false)]
    public bool? IsAllDay { get; set; }

    [DataMember(Name = "isCancelled", EmitDefaultValue = false, IsRequired = false)]
    public bool? IsCancelled { get; set; }

    [DataMember(Name = "isOrganizer", EmitDefaultValue = false, IsRequired = false)]
    public bool? IsOrganizer { get; set; }

    [DataMember(Name = "recurrence", EmitDefaultValue = false, IsRequired = false)]
    public PatternedRecurrence Recurrence { get; set; }

    [DataMember(Name = "responseRequested", EmitDefaultValue = false, IsRequired = false)]
    public bool? ResponseRequested { get; set; }

    [DataMember(Name = "seriesMasterId", EmitDefaultValue = false, IsRequired = false)]
    public string SeriesMasterId { get; set; }

    [DataMember(Name = "showAs", EmitDefaultValue = false, IsRequired = false)]
    public FreeBusyStatus? ShowAs { get; set; }

    [DataMember(Name = "type", EmitDefaultValue = false, IsRequired = false)]
    public EventType? Type { get; set; }

    [DataMember(Name = "attendees", EmitDefaultValue = false, IsRequired = false)]
    public IEnumerable<Attendee> Attendees { get; set; }

    [DataMember(Name = "organizer", EmitDefaultValue = false, IsRequired = false)]
    public Recipient Organizer { get; set; }

    [DataMember(Name = "webLink", EmitDefaultValue = false, IsRequired = false)]
    public string WebLink { get; set; }

    [DataMember(Name = "calendar", EmitDefaultValue = false, IsRequired = false)]
    public Calendar Calendar { get; set; }

    [DataMember(Name = "instances", EmitDefaultValue = false, IsRequired = false)]
    public IEventInstancesCollectionPage Instances { get; set; }

    [DataMember(Name = "extensions", EmitDefaultValue = false, IsRequired = false)]
    public IEventExtensionsCollectionPage Extensions { get; set; }

    [DataMember(Name = "attachments", EmitDefaultValue = false, IsRequired = false)]
    public IEventAttachmentsCollectionPage Attachments { get; set; }
  }
}
