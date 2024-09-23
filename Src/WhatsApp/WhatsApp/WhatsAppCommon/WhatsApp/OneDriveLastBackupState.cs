// Decompiled with JetBrains decompiler
// Type: WhatsApp.OneDriveLastBackupState
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System.Runtime.Serialization;


namespace WhatsApp
{
  [DataContract]
  public class OneDriveLastBackupState
  {
    [DataMember(Name = "time")]
    public long TimestampTicks { get; set; }

    [DataMember(Name = "id")]
    public int Id { get; set; }

    [DataMember(Name = "df")]
    public int DisplayFlag { get; set; }

    [DataMember(Name = "p1")]
    public string Parm1 { get; set; }

    [DataMember(Name = "p2")]
    public string Parm2 { get; set; }
  }
}
