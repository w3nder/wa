// Decompiled with JetBrains decompiler
// Type: WhatsApp.LiveIdUserInfo
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System.Runtime.Serialization;


namespace WhatsApp
{
  [DataContract]
  public class LiveIdUserInfo
  {
    [DataMember(Name = "id")]
    public string Id { get; set; }

    [DataMember(Name = "name")]
    public string Name { get; set; }

    [DataMember(Name = "first_name")]
    public string FirstName { get; set; }

    [DataMember(Name = "last_name")]
    public string LastName { get; set; }

    [DataMember(Name = "gender")]
    public string Gender { get; set; }

    [DataMember(Name = "emails")]
    public LiveIdUserEmails Emails { get; set; }

    [DataMember(Name = "locale")]
    public string Locale { get; set; }
  }
}
