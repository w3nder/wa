// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.GifPreviewFetched
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.Events
{
  public class GifPreviewFetched : WamEvent
  {
    public wam_enum_gif_search_provider? gifSearchProvider;
    public long? fileSize;
    public long? roundTripTime;

    public void Reset()
    {
      this.gifSearchProvider = new wam_enum_gif_search_provider?();
      this.fileSize = new long?();
      this.roundTripTime = new long?();
    }

    public override uint GetCode() => 1132;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, Wam.EnumToLong<wam_enum_gif_search_provider>(this.gifSearchProvider));
      Wam.MaybeSerializeField(2, this.fileSize);
      Wam.MaybeSerializeField(3, this.roundTripTime);
    }
  }
}
