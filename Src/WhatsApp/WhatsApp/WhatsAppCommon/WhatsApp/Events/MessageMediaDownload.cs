// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.MessageMediaDownload
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class MessageMediaDownload : WamEvent
  {
    public wam_enum_media_type? messageMediaType;
    public bool? stickerIsFirstParty;
    public double? mediaSize;
    public double? bytesTransferred;

    public void Reset()
    {
      this.messageMediaType = new wam_enum_media_type?();
      this.stickerIsFirstParty = new bool?();
      this.mediaSize = new double?();
      this.bytesTransferred = new double?();
    }

    public override uint GetCode() => 1734;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, Wam.EnumToLong<wam_enum_media_type>(this.messageMediaType));
      Wam.MaybeSerializeField(2, this.stickerIsFirstParty);
      Wam.MaybeSerializeField(3, this.mediaSize);
      Wam.MaybeSerializeField(4, this.bytesTransferred);
    }
  }
}
