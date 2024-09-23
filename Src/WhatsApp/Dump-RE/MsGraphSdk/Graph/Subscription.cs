// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.Subscription
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System;
using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.Graph
{
  [DataContract]
  public class Subscription : Entity
  {
    [DataMember(Name = "resource", EmitDefaultValue = false, IsRequired = false)]
    public string Resource { get; set; }

    [DataMember(Name = "changeType", EmitDefaultValue = false, IsRequired = false)]
    public string ChangeType { get; set; }

    [DataMember(Name = "clientState", EmitDefaultValue = false, IsRequired = false)]
    public string ClientState { get; set; }

    [DataMember(Name = "notificationUrl", EmitDefaultValue = false, IsRequired = false)]
    public string NotificationUrl { get; set; }

    [DataMember(Name = "expirationDateTime", EmitDefaultValue = false, IsRequired = false)]
    public DateTimeOffset? ExpirationDateTime { get; set; }
  }
}
