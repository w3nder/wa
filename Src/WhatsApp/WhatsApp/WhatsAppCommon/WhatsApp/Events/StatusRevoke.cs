// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.StatusRevoke
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class StatusRevoke : WamEvent
  {
    public long? statusSessionId;
    public wam_enum_media_type? mediaType;
    public long? statusLifeT;

    public void Reset()
    {
      this.statusSessionId = new long?();
      this.mediaType = new wam_enum_media_type?();
      this.statusLifeT = new long?();
    }

    public override uint GetCode() => 1250;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.statusSessionId);
      Wam.MaybeSerializeField(2, Wam.EnumToLong<wam_enum_media_type>(this.mediaType));
      Wam.MaybeSerializeField(3, this.statusLifeT);
    }
  }
}
