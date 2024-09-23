// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.Group
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
  public class Group : DirectoryObject
  {
    [DataMember(Name = "description", EmitDefaultValue = false, IsRequired = false)]
    public string Description { get; set; }

    [DataMember(Name = "displayName", EmitDefaultValue = false, IsRequired = false)]
    public string DisplayName { get; set; }

    [DataMember(Name = "groupTypes", EmitDefaultValue = false, IsRequired = false)]
    public IEnumerable<string> GroupTypes { get; set; }

    [DataMember(Name = "mail", EmitDefaultValue = false, IsRequired = false)]
    public string Mail { get; set; }

    [DataMember(Name = "mailEnabled", EmitDefaultValue = false, IsRequired = false)]
    public bool? MailEnabled { get; set; }

    [DataMember(Name = "mailNickname", EmitDefaultValue = false, IsRequired = false)]
    public string MailNickname { get; set; }

    [DataMember(Name = "onPremisesLastSyncDateTime", EmitDefaultValue = false, IsRequired = false)]
    public DateTimeOffset? OnPremisesLastSyncDateTime { get; set; }

    [DataMember(Name = "onPremisesSecurityIdentifier", EmitDefaultValue = false, IsRequired = false)]
    public string OnPremisesSecurityIdentifier { get; set; }

    [DataMember(Name = "onPremisesSyncEnabled", EmitDefaultValue = false, IsRequired = false)]
    public bool? OnPremisesSyncEnabled { get; set; }

    [DataMember(Name = "proxyAddresses", EmitDefaultValue = false, IsRequired = false)]
    public IEnumerable<string> ProxyAddresses { get; set; }

    [DataMember(Name = "securityEnabled", EmitDefaultValue = false, IsRequired = false)]
    public bool? SecurityEnabled { get; set; }

    [DataMember(Name = "visibility", EmitDefaultValue = false, IsRequired = false)]
    public string Visibility { get; set; }

    [DataMember(Name = "allowExternalSenders", EmitDefaultValue = false, IsRequired = false)]
    public bool? AllowExternalSenders { get; set; }

    [DataMember(Name = "autoSubscribeNewMembers", EmitDefaultValue = false, IsRequired = false)]
    public bool? AutoSubscribeNewMembers { get; set; }

    [DataMember(Name = "isSubscribedByMail", EmitDefaultValue = false, IsRequired = false)]
    public bool? IsSubscribedByMail { get; set; }

    [DataMember(Name = "unseenCount", EmitDefaultValue = false, IsRequired = false)]
    public int? UnseenCount { get; set; }

    [DataMember(Name = "members", EmitDefaultValue = false, IsRequired = false)]
    public IGroupMembersCollectionWithReferencesPage Members { get; set; }

    [DataMember(Name = "memberOf", EmitDefaultValue = false, IsRequired = false)]
    public IGroupMemberOfCollectionWithReferencesPage MemberOf { get; set; }

    [DataMember(Name = "createdOnBehalfOf", EmitDefaultValue = false, IsRequired = false)]
    public DirectoryObject CreatedOnBehalfOf { get; set; }

    [DataMember(Name = "owners", EmitDefaultValue = false, IsRequired = false)]
    public IGroupOwnersCollectionWithReferencesPage Owners { get; set; }

    [DataMember(Name = "threads", EmitDefaultValue = false, IsRequired = false)]
    public IGroupThreadsCollectionPage Threads { get; set; }

    [DataMember(Name = "calendar", EmitDefaultValue = false, IsRequired = false)]
    public Calendar Calendar { get; set; }

    [DataMember(Name = "calendarView", EmitDefaultValue = false, IsRequired = false)]
    public IGroupCalendarViewCollectionPage CalendarView { get; set; }

    [DataMember(Name = "events", EmitDefaultValue = false, IsRequired = false)]
    public IGroupEventsCollectionPage Events { get; set; }

    [DataMember(Name = "conversations", EmitDefaultValue = false, IsRequired = false)]
    public IGroupConversationsCollectionPage Conversations { get; set; }

    [DataMember(Name = "photo", EmitDefaultValue = false, IsRequired = false)]
    public ProfilePhoto Photo { get; set; }

    [DataMember(Name = "acceptedSenders", EmitDefaultValue = false, IsRequired = false)]
    public IGroupAcceptedSendersCollectionPage AcceptedSenders { get; set; }

    [DataMember(Name = "rejectedSenders", EmitDefaultValue = false, IsRequired = false)]
    public IGroupRejectedSendersCollectionPage RejectedSenders { get; set; }

    [DataMember(Name = "drive", EmitDefaultValue = false, IsRequired = false)]
    public Drive Drive { get; set; }
  }
}
