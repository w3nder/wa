// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.UserAssignLicenseRequestBody
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
  public class UserAssignLicenseRequestBody
  {
    [DataMember(Name = "addLicenses", EmitDefaultValue = false, IsRequired = false)]
    public IEnumerable<AssignedLicense> AddLicenses { get; set; }

    [DataMember(Name = "removeLicenses", EmitDefaultValue = false, IsRequired = false)]
    public IEnumerable<Guid> RemoveLicenses { get; set; }
  }
}
