// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.OutlookItem
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
  public class OutlookItem : Entity
  {
    protected internal OutlookItem()
    {
    }

    [DataMember(Name = "createdDateTime", EmitDefaultValue = false, IsRequired = false)]
    public DateTimeOffset? CreatedDateTime { get; set; }

    [DataMember(Name = "lastModifiedDateTime", EmitDefaultValue = false, IsRequired = false)]
    public DateTimeOffset? LastModifiedDateTime { get; set; }

    [DataMember(Name = "changeKey", EmitDefaultValue = false, IsRequired = false)]
    public string ChangeKey { get; set; }

    [DataMember(Name = "categories", EmitDefaultValue = false, IsRequired = false)]
    public IEnumerable<string> Categories { get; set; }
  }
}
