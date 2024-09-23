// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.GifSearchPerformed
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class GifSearchPerformed : WamEvent
  {
    public wam_enum_gif_search_provider? gifSearchProvider;
    public long? roundTripTime;
    public string inputLanguageCode;
    public string languageCode;

    public void Reset()
    {
      this.gifSearchProvider = new wam_enum_gif_search_provider?();
      this.roundTripTime = new long?();
      this.inputLanguageCode = (string) null;
      this.languageCode = (string) null;
    }

    public override uint GetCode() => 1118;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, Wam.EnumToLong<wam_enum_gif_search_provider>(this.gifSearchProvider));
      Wam.MaybeSerializeField(2, this.roundTripTime);
      Wam.MaybeSerializeField(4, this.inputLanguageCode);
      Wam.MaybeSerializeField(3, this.languageCode);
    }
  }
}
