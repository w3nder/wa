// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.Device
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
  public class Device : DirectoryObject
  {
    [DataMember(Name = "accountEnabled", EmitDefaultValue = false, IsRequired = false)]
    public bool? AccountEnabled { get; set; }

    [DataMember(Name = "alternativeSecurityIds", EmitDefaultValue = false, IsRequired = false)]
    public IEnumerable<AlternativeSecurityId> AlternativeSecurityIds { get; set; }

    [DataMember(Name = "approximateLastSignInDateTime", EmitDefaultValue = false, IsRequired = false)]
    public DateTimeOffset? ApproximateLastSignInDateTime { get; set; }

    [DataMember(Name = "deviceId", EmitDefaultValue = false, IsRequired = false)]
    public string DeviceId { get; set; }

    [DataMember(Name = "deviceMetadata", EmitDefaultValue = false, IsRequired = false)]
    public string DeviceMetadata { get; set; }

    [DataMember(Name = "deviceVersion", EmitDefaultValue = false, IsRequired = false)]
    public int? DeviceVersion { get; set; }

    [DataMember(Name = "displayName", EmitDefaultValue = false, IsRequired = false)]
    public string DisplayName { get; set; }

    [DataMember(Name = "isCompliant", EmitDefaultValue = false, IsRequired = false)]
    public bool? IsCompliant { get; set; }

    [DataMember(Name = "isManaged", EmitDefaultValue = false, IsRequired = false)]
    public bool? IsManaged { get; set; }

    [DataMember(Name = "onPremisesLastSyncDateTime", EmitDefaultValue = false, IsRequired = false)]
    public DateTimeOffset? OnPremisesLastSyncDateTime { get; set; }

    [DataMember(Name = "onPremisesSyncEnabled", EmitDefaultValue = false, IsRequired = false)]
    public bool? OnPremisesSyncEnabled { get; set; }

    [DataMember(Name = "operatingSystem", EmitDefaultValue = false, IsRequired = false)]
    public string OperatingSystem { get; set; }

    [DataMember(Name = "operatingSystemVersion", EmitDefaultValue = false, IsRequired = false)]
    public string OperatingSystemVersion { get; set; }

    [DataMember(Name = "physicalIds", EmitDefaultValue = false, IsRequired = false)]
    public IEnumerable<string> PhysicalIds { get; set; }

    [DataMember(Name = "trustType", EmitDefaultValue = false, IsRequired = false)]
    public string TrustType { get; set; }

    [DataMember(Name = "registeredOwners", EmitDefaultValue = false, IsRequired = false)]
    public IDeviceRegisteredOwnersCollectionWithReferencesPage RegisteredOwners { get; set; }

    [DataMember(Name = "registeredUsers", EmitDefaultValue = false, IsRequired = false)]
    public IDeviceRegisteredUsersCollectionWithReferencesPage RegisteredUsers { get; set; }
  }
}
