// Decompiled with JetBrains decompiler
// Type: WhatsApp.Mms4ServerPropHelper
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp
{
  public class Mms4ServerPropHelper
  {
    public static bool ForceMms4;

    public static bool IsMms4Enabled()
    {
      return Mms4ServerPropHelper.ForceMms4 || Settings.Mms4ServerAudio || Settings.Mms4ServerImage || Settings.Mms4ServerPtt || Settings.MmsServerDoc || Settings.MmsServerGif || Settings.MmsServerVideo;
    }

    public static bool IsMms4EnabledForType(FunXMPP.FMessage.FunMediaType mediaType, bool upload)
    {
      if (Mms4ServerPropHelper.ForceMms4 || mediaType == FunXMPP.FMessage.FunMediaType.Sticker || mediaType == FunXMPP.FMessage.FunMediaType.Audio && Settings.Mms4ServerAudio || mediaType == FunXMPP.FMessage.FunMediaType.Image && Settings.Mms4ServerImage || mediaType == FunXMPP.FMessage.FunMediaType.Ptt && Settings.Mms4ServerPtt || mediaType == FunXMPP.FMessage.FunMediaType.Document && Settings.MmsServerDoc || mediaType == FunXMPP.FMessage.FunMediaType.Gif && Settings.MmsServerGif)
        return true;
      return mediaType == FunXMPP.FMessage.FunMediaType.Video && Settings.MmsServerVideo;
    }
  }
}
