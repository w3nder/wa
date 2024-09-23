// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.User
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
  public class User : DirectoryObject
  {
    [DataMember(Name = "accountEnabled", EmitDefaultValue = false, IsRequired = false)]
    public bool? AccountEnabled { get; set; }

    [DataMember(Name = "assignedLicenses", EmitDefaultValue = false, IsRequired = false)]
    public IEnumerable<AssignedLicense> AssignedLicenses { get; set; }

    [DataMember(Name = "assignedPlans", EmitDefaultValue = false, IsRequired = false)]
    public IEnumerable<AssignedPlan> AssignedPlans { get; set; }

    [DataMember(Name = "businessPhones", EmitDefaultValue = false, IsRequired = false)]
    public IEnumerable<string> BusinessPhones { get; set; }

    [DataMember(Name = "city", EmitDefaultValue = false, IsRequired = false)]
    public string City { get; set; }

    [DataMember(Name = "companyName", EmitDefaultValue = false, IsRequired = false)]
    public string CompanyName { get; set; }

    [DataMember(Name = "country", EmitDefaultValue = false, IsRequired = false)]
    public string Country { get; set; }

    [DataMember(Name = "department", EmitDefaultValue = false, IsRequired = false)]
    public string Department { get; set; }

    [DataMember(Name = "displayName", EmitDefaultValue = false, IsRequired = false)]
    public string DisplayName { get; set; }

    [DataMember(Name = "givenName", EmitDefaultValue = false, IsRequired = false)]
    public string GivenName { get; set; }

    [DataMember(Name = "jobTitle", EmitDefaultValue = false, IsRequired = false)]
    public string JobTitle { get; set; }

    [DataMember(Name = "mail", EmitDefaultValue = false, IsRequired = false)]
    public string Mail { get; set; }

    [DataMember(Name = "mailNickname", EmitDefaultValue = false, IsRequired = false)]
    public string MailNickname { get; set; }

    [DataMember(Name = "mobilePhone", EmitDefaultValue = false, IsRequired = false)]
    public string MobilePhone { get; set; }

    [DataMember(Name = "onPremisesImmutableId", EmitDefaultValue = false, IsRequired = false)]
    public string OnPremisesImmutableId { get; set; }

    [DataMember(Name = "onPremisesLastSyncDateTime", EmitDefaultValue = false, IsRequired = false)]
    public DateTimeOffset? OnPremisesLastSyncDateTime { get; set; }

    [DataMember(Name = "onPremisesSecurityIdentifier", EmitDefaultValue = false, IsRequired = false)]
    public string OnPremisesSecurityIdentifier { get; set; }

    [DataMember(Name = "onPremisesSyncEnabled", EmitDefaultValue = false, IsRequired = false)]
    public bool? OnPremisesSyncEnabled { get; set; }

    [DataMember(Name = "passwordPolicies", EmitDefaultValue = false, IsRequired = false)]
    public string PasswordPolicies { get; set; }

    [DataMember(Name = "passwordProfile", EmitDefaultValue = false, IsRequired = false)]
    public PasswordProfile PasswordProfile { get; set; }

    [DataMember(Name = "officeLocation", EmitDefaultValue = false, IsRequired = false)]
    public string OfficeLocation { get; set; }

    [DataMember(Name = "postalCode", EmitDefaultValue = false, IsRequired = false)]
    public string PostalCode { get; set; }

    [DataMember(Name = "preferredLanguage", EmitDefaultValue = false, IsRequired = false)]
    public string PreferredLanguage { get; set; }

    [DataMember(Name = "provisionedPlans", EmitDefaultValue = false, IsRequired = false)]
    public IEnumerable<ProvisionedPlan> ProvisionedPlans { get; set; }

    [DataMember(Name = "proxyAddresses", EmitDefaultValue = false, IsRequired = false)]
    public IEnumerable<string> ProxyAddresses { get; set; }

    [DataMember(Name = "state", EmitDefaultValue = false, IsRequired = false)]
    public string State { get; set; }

    [DataMember(Name = "streetAddress", EmitDefaultValue = false, IsRequired = false)]
    public string StreetAddress { get; set; }

    [DataMember(Name = "surname", EmitDefaultValue = false, IsRequired = false)]
    public string Surname { get; set; }

    [DataMember(Name = "usageLocation", EmitDefaultValue = false, IsRequired = false)]
    public string UsageLocation { get; set; }

    [DataMember(Name = "userPrincipalName", EmitDefaultValue = false, IsRequired = false)]
    public string UserPrincipalName { get; set; }

    [DataMember(Name = "userType", EmitDefaultValue = false, IsRequired = false)]
    public string UserType { get; set; }

    [DataMember(Name = "aboutMe", EmitDefaultValue = false, IsRequired = false)]
    public string AboutMe { get; set; }

    [DataMember(Name = "birthday", EmitDefaultValue = false, IsRequired = false)]
    public DateTimeOffset? Birthday { get; set; }

    [DataMember(Name = "hireDate", EmitDefaultValue = false, IsRequired = false)]
    public DateTimeOffset? HireDate { get; set; }

    [DataMember(Name = "interests", EmitDefaultValue = false, IsRequired = false)]
    public IEnumerable<string> Interests { get; set; }

    [DataMember(Name = "mySite", EmitDefaultValue = false, IsRequired = false)]
    public string MySite { get; set; }

    [DataMember(Name = "pastProjects", EmitDefaultValue = false, IsRequired = false)]
    public IEnumerable<string> PastProjects { get; set; }

    [DataMember(Name = "preferredName", EmitDefaultValue = false, IsRequired = false)]
    public string PreferredName { get; set; }

    [DataMember(Name = "responsibilities", EmitDefaultValue = false, IsRequired = false)]
    public IEnumerable<string> Responsibilities { get; set; }

    [DataMember(Name = "schools", EmitDefaultValue = false, IsRequired = false)]
    public IEnumerable<string> Schools { get; set; }

    [DataMember(Name = "skills", EmitDefaultValue = false, IsRequired = false)]
    public IEnumerable<string> Skills { get; set; }

    [DataMember(Name = "ownedDevices", EmitDefaultValue = false, IsRequired = false)]
    public IUserOwnedDevicesCollectionWithReferencesPage OwnedDevices { get; set; }

    [DataMember(Name = "registeredDevices", EmitDefaultValue = false, IsRequired = false)]
    public IUserRegisteredDevicesCollectionWithReferencesPage RegisteredDevices { get; set; }

    [DataMember(Name = "manager", EmitDefaultValue = false, IsRequired = false)]
    public DirectoryObject Manager { get; set; }

    [DataMember(Name = "directReports", EmitDefaultValue = false, IsRequired = false)]
    public IUserDirectReportsCollectionWithReferencesPage DirectReports { get; set; }

    [DataMember(Name = "memberOf", EmitDefaultValue = false, IsRequired = false)]
    public IUserMemberOfCollectionWithReferencesPage MemberOf { get; set; }

    [DataMember(Name = "createdObjects", EmitDefaultValue = false, IsRequired = false)]
    public IUserCreatedObjectsCollectionWithReferencesPage CreatedObjects { get; set; }

    [DataMember(Name = "ownedObjects", EmitDefaultValue = false, IsRequired = false)]
    public IUserOwnedObjectsCollectionWithReferencesPage OwnedObjects { get; set; }

    [DataMember(Name = "messages", EmitDefaultValue = false, IsRequired = false)]
    public IUserMessagesCollectionPage Messages { get; set; }

    [DataMember(Name = "mailFolders", EmitDefaultValue = false, IsRequired = false)]
    public IUserMailFoldersCollectionPage MailFolders { get; set; }

    [DataMember(Name = "calendar", EmitDefaultValue = false, IsRequired = false)]
    public Calendar Calendar { get; set; }

    [DataMember(Name = "calendars", EmitDefaultValue = false, IsRequired = false)]
    public IUserCalendarsCollectionPage Calendars { get; set; }

    [DataMember(Name = "calendarGroups", EmitDefaultValue = false, IsRequired = false)]
    public IUserCalendarGroupsCollectionPage CalendarGroups { get; set; }

    [DataMember(Name = "calendarView", EmitDefaultValue = false, IsRequired = false)]
    public IUserCalendarViewCollectionPage CalendarView { get; set; }

    [DataMember(Name = "events", EmitDefaultValue = false, IsRequired = false)]
    public IUserEventsCollectionPage Events { get; set; }

    [DataMember(Name = "contacts", EmitDefaultValue = false, IsRequired = false)]
    public IUserContactsCollectionPage Contacts { get; set; }

    [DataMember(Name = "contactFolders", EmitDefaultValue = false, IsRequired = false)]
    public IUserContactFoldersCollectionPage ContactFolders { get; set; }

    [DataMember(Name = "inferenceClassification", EmitDefaultValue = false, IsRequired = false)]
    public InferenceClassification InferenceClassification { get; set; }

    [DataMember(Name = "photo", EmitDefaultValue = false, IsRequired = false)]
    public ProfilePhoto Photo { get; set; }

    [DataMember(Name = "drive", EmitDefaultValue = false, IsRequired = false)]
    public Drive Drive { get; set; }
  }
}
