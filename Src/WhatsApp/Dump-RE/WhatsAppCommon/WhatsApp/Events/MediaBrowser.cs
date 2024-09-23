// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.MediaBrowser
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.Events
{
  public class MediaBrowser : WamEvent
  {
    public long? mediaBrowserPresentationT;

    public void Reset() => this.mediaBrowserPresentationT = new long?();

    public override uint GetCode() => 1532;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.mediaBrowserPresentationT);
    }
  }
}
