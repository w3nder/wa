// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.Organization
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
  public class Organization : DirectoryObject
  {
    [DataMember(Name = "assignedPlans", EmitDefaultValue = false, IsRequired = false)]
    public IEnumerable<AssignedPlan> AssignedPlans { get; set; }

    [DataMember(Name = "businessPhones", EmitDefaultValue = false, IsRequired = false)]
    public IEnumerable<string> BusinessPhones { get; set; }

    [DataMember(Name = "city", EmitDefaultValue = false, IsRequired = false)]
    public string City { get; set; }

    [DataMember(Name = "country", EmitDefaultValue = false, IsRequired = false)]
    public string Country { get; set; }

    [DataMember(Name = "countryLetterCode", EmitDefaultValue = false, IsRequired = false)]
    public string CountryLetterCode { get; set; }

    [DataMember(Name = "displayName", EmitDefaultValue = false, IsRequired = false)]
    public string DisplayName { get; set; }

    [DataMember(Name = "marketingNotificationEmails", EmitDefaultValue = false, IsRequired = false)]
    public IEnumerable<string> MarketingNotificationEmails { get; set; }

    [DataMember(Name = "onPremisesLastSyncDateTime", EmitDefaultValue = false, IsRequired = false)]
    public DateTimeOffset? OnPremisesLastSyncDateTime { get; set; }

    [DataMember(Name = "onPremisesSyncEnabled", EmitDefaultValue = false, IsRequired = false)]
    public bool? OnPremisesSyncEnabled { get; set; }

    [DataMember(Name = "postalCode", EmitDefaultValue = false, IsRequired = false)]
    public string PostalCode { get; set; }

    [DataMember(Name = "preferredLanguage", EmitDefaultValue = false, IsRequired = false)]
    public string PreferredLanguage { get; set; }

    [DataMember(Name = "provisionedPlans", EmitDefaultValue = false, IsRequired = false)]
    public IEnumerable<ProvisionedPlan> ProvisionedPlans { get; set; }

    [DataMember(Name = "securityComplianceNotificationMails", EmitDefaultValue = false, IsRequired = false)]
    public IEnumerable<string> SecurityComplianceNotificationMails { get; set; }

    [DataMember(Name = "securityComplianceNotificationPhones", EmitDefaultValue = false, IsRequired = false)]
    public IEnumerable<string> SecurityComplianceNotificationPhones { get; set; }

    [DataMember(Name = "state", EmitDefaultValue = false, IsRequired = false)]
    public string State { get; set; }

    [DataMember(Name = "street", EmitDefaultValue = false, IsRequired = false)]
    public string Street { get; set; }

    [DataMember(Name = "technicalNotificationMails", EmitDefaultValue = false, IsRequired = false)]
    public IEnumerable<string> TechnicalNotificationMails { get; set; }

    [DataMember(Name = "verifiedDomains", EmitDefaultValue = false, IsRequired = false)]
    public IEnumerable<VerifiedDomain> VerifiedDomains { get; set; }
  }
}
