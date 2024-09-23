// Decompiled with JetBrains decompiler
// Type: WhatsApp.UnreadDividerViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Windows;
using System.Windows.Media;

#nullable disable
namespace WhatsApp
{
  public class UnreadDividerViewModel : MessageViewModel
  {
    private static SolidColorBrush unreadDividerBgBrushOverWallpaper_;
    private static SolidColorBrush unreadDividerFgBrushOverWallpaper_;
    private static SolidColorBrush unreadDividerBgBrush_;
    private static Brush unreadDividerFgBrush_;

    public static SolidColorBrush UnreadDividerBackgroundBrushOverWallpaper
    {
      get
      {
        return UnreadDividerViewModel.unreadDividerBgBrushOverWallpaper_ ?? (UnreadDividerViewModel.unreadDividerBgBrushOverWallpaper_ = new SolidColorBrush(Color.FromArgb((byte) 178, byte.MaxValue, byte.MaxValue, byte.MaxValue)));
      }
    }

    public static SolidColorBrush UnreadDividerForegroundBrushOverWallpaper
    {
      get
      {
        return UnreadDividerViewModel.unreadDividerFgBrushOverWallpaper_ ?? (UnreadDividerViewModel.unreadDividerFgBrushOverWallpaper_ = new SolidColorBrush(Color.FromArgb(byte.MaxValue, (byte) 51, (byte) 51, (byte) 51)));
      }
    }

    public static SolidColorBrush UnreadDividerBackgroundBrush
    {
      get
      {
        SolidColorBrush unreadDividerBgBrush = UnreadDividerViewModel.unreadDividerBgBrush_;
        if (unreadDividerBgBrush != null)
          return unreadDividerBgBrush;
        Color color = ImageStore.IsDarkTheme() ? Color.FromArgb((byte) 52, byte.MaxValue, byte.MaxValue, byte.MaxValue) : Color.FromArgb((byte) 18, (byte) 0, (byte) 0, (byte) 0);
        return UnreadDividerViewModel.unreadDividerBgBrush_ = new SolidColorBrush(color);
      }
    }

    private static Brush UnreadDividerForegroundBrush
    {
      get
      {
        Brush unreadDividerFgBrush = UnreadDividerViewModel.unreadDividerFgBrush_;
        if (unreadDividerFgBrush != null)
          return unreadDividerFgBrush;
        Color color = ImageStore.IsDarkTheme() ? Color.FromArgb(byte.MaxValue, (byte) 153, (byte) 153, (byte) 153) : Color.FromArgb(byte.MaxValue, (byte) 77, (byte) 77, (byte) 77);
        return UnreadDividerViewModel.unreadDividerFgBrush_ = (Brush) new SolidColorBrush(color);
      }
    }

    public override Brush ForegroundBrush
    {
      get
      {
        return MessageViewModel.IsOverWallpaper ? (Brush) UnreadDividerViewModel.UnreadDividerForegroundBrushOverWallpaper : UnreadDividerViewModel.UnreadDividerForegroundBrush;
      }
    }

    public override SolidColorBrush BackgroundBrush
    {
      get
      {
        return MessageViewModel.IsOverWallpaper ? UnreadDividerViewModel.UnreadDividerBackgroundBrushOverWallpaper : UnreadDividerViewModel.UnreadDividerBackgroundBrush;
      }
    }

    public static Brush GetForegroundBrush()
    {
      return MessageViewModel.IsOverWallpaper ? (Brush) UnreadDividerViewModel.UnreadDividerForegroundBrushOverWallpaper : UnreadDividerViewModel.UnreadDividerForegroundBrush;
    }

    public static SolidColorBrush GetBackgroundBrush()
    {
      return MessageViewModel.IsOverWallpaper ? UnreadDividerViewModel.UnreadDividerBackgroundBrushOverWallpaper : UnreadDividerViewModel.UnreadDividerBackgroundBrush;
    }

    public override string SenderJid => (string) null;

    public override HorizontalAlignment HorizontalAlignment => HorizontalAlignment.Stretch;

    public override double MaxBubbleWidth => double.PositiveInfinity;

    public override Thickness BubbleMargin
    {
      get => new Thickness(0.0, 12.0 * this.zoomMultiplier, 0.0, 12.0 * this.zoomMultiplier);
    }

    public override bool ShouldShowHeader => false;

    public override bool ShouldShowFooter => false;

    public override bool ShouldShowTail => false;

    public FontFamily UnreadDividerForegroundFontFamily
    {
      get
      {
        return !MessageViewModel.IsOverWallpaper && ImageStore.IsDarkTheme() ? Application.Current.Resources[(object) "PhoneFontFamilyNormal"] as FontFamily : Application.Current.Resources[(object) "PhoneFontFamilySemiBold"] as FontFamily;
      }
    }

    public System.Windows.Media.ImageSource UnreadDividerChevronIcon
    {
      get
      {
        return !MessageViewModel.IsOverWallpaper ? (System.Windows.Media.ImageSource) AssetStore.ChevronIcon : (System.Windows.Media.ImageSource) AssetStore.ChevronIconBlack;
      }
    }

    public override bool IsSelectable => false;

    public override Thickness ViewPanelMargin
    {
      get
      {
        return new Thickness(24.0 * this.zoomMultiplier, 12.0 * this.zoomMultiplier, 24.0 * this.zoomMultiplier, 12.0 * this.zoomMultiplier);
      }
    }

    public UnreadDividerViewModel(Message m)
      : base(m)
    {
    }

    public override void RefreshOnChatBackgroundChanged() => this.Notify("ChatBackgroundChanged");

    protected override string GetTextStr()
    {
      int result = 0;
      return int.TryParse(this.Message.Data, out result) && result != 0 ? Plurals.Instance.GetString(result > 0 ? AppResources.UnreadDividerPlural : AppResources.UnheardDividerPlural, Math.Abs(result)).ToUpper() : "";
    }

    public void IncreaseUnreadCount(int n)
    {
      if (n < 1)
        return;
      int result = 0;
      if (!int.TryParse(this.Message.Data, out result))
        return;
      this.Message.Data = (Math.Abs(result) + n).ToString();
      this.TextStrCache = (string) null;
      this.Notify("TextStr");
    }

    public override bool EnableContextMenu => false;

    public override int MessageID => -101;
  }
}
