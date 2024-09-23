// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.Contact
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
  public class Contact : OutlookItem
  {
    [DataMember(Name = "parentFolderId", EmitDefaultValue = false, IsRequired = false)]
    public string ParentFolderId { get; set; }

    [DataMember(Name = "birthday", EmitDefaultValue = false, IsRequired = false)]
    public DateTimeOffset? Birthday { get; set; }

    [DataMember(Name = "fileAs", EmitDefaultValue = false, IsRequired = false)]
    public string FileAs { get; set; }

    [DataMember(Name = "displayName", EmitDefaultValue = false, IsRequired = false)]
    public string DisplayName { get; set; }

    [DataMember(Name = "givenName", EmitDefaultValue = false, IsRequired = false)]
    public string GivenName { get; set; }

    [DataMember(Name = "initials", EmitDefaultValue = false, IsRequired = false)]
    public string Initials { get; set; }

    [DataMember(Name = "middleName", EmitDefaultValue = false, IsRequired = false)]
    public string MiddleName { get; set; }

    [DataMember(Name = "nickName", EmitDefaultValue = false, IsRequired = false)]
    public string NickName { get; set; }

    [DataMember(Name = "surname", EmitDefaultValue = false, IsRequired = false)]
    public string Surname { get; set; }

    [DataMember(Name = "title", EmitDefaultValue = false, IsRequired = false)]
    public string Title { get; set; }

    [DataMember(Name = "yomiGivenName", EmitDefaultValue = false, IsRequired = false)]
    public string YomiGivenName { get; set; }

    [DataMember(Name = "yomiSurname", EmitDefaultValue = false, IsRequired = false)]
    public string YomiSurname { get; set; }

    [DataMember(Name = "yomiCompanyName", EmitDefaultValue = false, IsRequired = false)]
    public string YomiCompanyName { get; set; }

    [DataMember(Name = "generation", EmitDefaultValue = false, IsRequired = false)]
    public string Generation { get; set; }

    [DataMember(Name = "emailAddresses", EmitDefaultValue = false, IsRequired = false)]
    public IEnumerable<EmailAddress> EmailAddresses { get; set; }

    [DataMember(Name = "imAddresses", EmitDefaultValue = false, IsRequired = false)]
    public IEnumerable<string> ImAddresses { get; set; }

    [DataMember(Name = "jobTitle", EmitDefaultValue = false, IsRequired = false)]
    public string JobTitle { get; set; }

    [DataMember(Name = "companyName", EmitDefaultValue = false, IsRequired = false)]
    public string CompanyName { get; set; }

    [DataMember(Name = "department", EmitDefaultValue = false, IsRequired = false)]
    public string Department { get; set; }

    [DataMember(Name = "officeLocation", EmitDefaultValue = false, IsRequired = false)]
    public string OfficeLocation { get; set; }

    [DataMember(Name = "profession", EmitDefaultValue = false, IsRequired = false)]
    public string Profession { get; set; }

    [DataMember(Name = "businessHomePage", EmitDefaultValue = false, IsRequired = false)]
    public string BusinessHomePage { get; set; }

    [DataMember(Name = "assistantName", EmitDefaultValue = false, IsRequired = false)]
    public string AssistantName { get; set; }

    [DataMember(Name = "manager", EmitDefaultValue = false, IsRequired = false)]
    public string Manager { get; set; }

    [DataMember(Name = "homePhones", EmitDefaultValue = false, IsRequired = false)]
    public IEnumerable<string> HomePhones { get; set; }

    [DataMember(Name = "mobilePhone", EmitDefaultValue = false, IsRequired = false)]
    public string MobilePhone { get; set; }

    [DataMember(Name = "businessPhones", EmitDefaultValue = false, IsRequired = false)]
    public IEnumerable<string> BusinessPhones { get; set; }

    [DataMember(Name = "homeAddress", EmitDefaultValue = false, IsRequired = false)]
    public PhysicalAddress HomeAddress { get; set; }

    [DataMember(Name = "businessAddress", EmitDefaultValue = false, IsRequired = false)]
    public PhysicalAddress BusinessAddress { get; set; }

    [DataMember(Name = "otherAddress", EmitDefaultValue = false, IsRequired = false)]
    public PhysicalAddress OtherAddress { get; set; }

    [DataMember(Name = "spouseName", EmitDefaultValue = false, IsRequired = false)]
    public string SpouseName { get; set; }

    [DataMember(Name = "personalNotes", EmitDefaultValue = false, IsRequired = false)]
    public string PersonalNotes { get; set; }

    [DataMember(Name = "children", EmitDefaultValue = false, IsRequired = false)]
    public IEnumerable<string> Children { get; set; }

    [DataMember(Name = "extensions", EmitDefaultValue = false, IsRequired = false)]
    public IContactExtensionsCollectionPage Extensions { get; set; }

    [DataMember(Name = "photo", EmitDefaultValue = false, IsRequired = false)]
    public ProfilePhoto Photo { get; set; }
  }
}
