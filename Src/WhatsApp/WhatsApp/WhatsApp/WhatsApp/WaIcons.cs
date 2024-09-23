// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaIcons
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System.Windows.Media.Imaging;


namespace WhatsApp
{
  public class WaIcons
  {
    private static WriteableBitmap accentBackgroundAudioIcon_;
    private static WriteableBitmap accentBackgroundImageIcon_;
    private static WriteableBitmap accentBackgroundVideoIcon_;

    public static WriteableBitmap AccentBackgroundAudioIcon
    {
      get
      {
        WriteableBitmap backgroundAudioIcon = WaIcons.accentBackgroundAudioIcon_;
        if (backgroundAudioIcon != null)
          return backgroundAudioIcon;
        BitmapSource audioButtonIcon = AssetStore.AudioButtonIcon;
        double? size = new double?();
        return WaIcons.accentBackgroundAudioIcon_ = IconUtils.CreateBackgroundThemeIcon(audioButtonIcon, size);
      }
    }

    public static WriteableBitmap AccentBackgroundImageIcon
    {
      get
      {
        WriteableBitmap backgroundImageIcon = WaIcons.accentBackgroundImageIcon_;
        if (backgroundImageIcon != null)
          return backgroundImageIcon;
        BitmapSource pictureButtonIcon = AssetStore.PictureButtonIcon;
        double? size = new double?();
        return WaIcons.accentBackgroundImageIcon_ = IconUtils.CreateBackgroundThemeIcon(pictureButtonIcon, size);
      }
    }

    public static WriteableBitmap AccentBackgroundVideoIcon
    {
      get
      {
        WriteableBitmap backgroundVideoIcon = WaIcons.accentBackgroundVideoIcon_;
        if (backgroundVideoIcon != null)
          return backgroundVideoIcon;
        BitmapSource videoButtonIcon = AssetStore.VideoButtonIcon;
        double? size = new double?();
        return WaIcons.accentBackgroundVideoIcon_ = IconUtils.CreateBackgroundThemeIcon(videoButtonIcon, size);
      }
    }
  }
}
