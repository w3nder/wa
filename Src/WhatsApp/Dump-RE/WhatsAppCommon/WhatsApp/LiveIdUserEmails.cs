// Decompiled with JetBrains decompiler
// Type: WhatsApp.LiveIdUserEmails
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System.Runtime.Serialization;

#nullable disable
namespace WhatsApp
{
  [DataContract]
  public class LiveIdUserEmails
  {
    [DataMember(Name = "preferred")]
    public string Preferred { get; set; }

    [DataMember(Name = "account")]
    public string Account { get; set; }

    [DataMember(Name = "business")]
    public string Business { get; set; }

    [DataMember(Name = "personal")]
    public string Personal { get; set; }
  }
}
