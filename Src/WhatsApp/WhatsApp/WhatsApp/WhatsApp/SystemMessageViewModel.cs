// Decompiled with JetBrains decompiler
// Type: WhatsApp.SystemMessageViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System.Windows;
using System.Windows.Media;


namespace WhatsApp
{
  public class SystemMessageViewModel : MessageViewModel
  {
    private static SolidColorBrush sysMsgBgBrushOverWallpaper;
    private static SolidColorBrush sysMsgBgBrush;
    private static SolidColorBrush sysMsgFgBrushOverWallpaper;
    private static SolidColorBrush sysMsgFgBrush;

    private static SolidColorBrush SystemMessageBackgroundBrushOverWallpaper
    {
      get
      {
        return SystemMessageViewModel.sysMsgBgBrushOverWallpaper ?? (SystemMessageViewModel.sysMsgBgBrushOverWallpaper = new SolidColorBrush(Color.FromArgb(byte.MaxValue, (byte) 206, (byte) 206, (byte) 206)));
      }
    }

    private static SolidColorBrush SystemMessageBackgroundBrush
    {
      get
      {
        SolidColorBrush sysMsgBgBrush = SystemMessageViewModel.sysMsgBgBrush;
        if (sysMsgBgBrush != null)
          return sysMsgBgBrush;
        Color color = ImageStore.IsDarkTheme() ? Color.FromArgb(byte.MaxValue, (byte) 38, (byte) 38, (byte) 38) : Color.FromArgb(byte.MaxValue, (byte) 222, (byte) 222, (byte) 222);
        return SystemMessageViewModel.sysMsgBgBrush = new SolidColorBrush(color);
      }
    }

    private static SolidColorBrush SystemMessageForegroundBrushOverWallpaper
    {
      get
      {
        return SystemMessageViewModel.sysMsgFgBrushOverWallpaper ?? (SystemMessageViewModel.sysMsgFgBrushOverWallpaper = new SolidColorBrush(Color.FromArgb(byte.MaxValue, (byte) 26, (byte) 26, (byte) 26)));
      }
    }

    private static SolidColorBrush SystemMessageForegroundBrush
    {
      get
      {
        SolidColorBrush sysMsgFgBrush = SystemMessageViewModel.sysMsgFgBrush;
        if (sysMsgFgBrush != null)
          return sysMsgFgBrush;
        Color color = ImageStore.IsDarkTheme() ? Color.FromArgb(byte.MaxValue, (byte) 204, (byte) 204, (byte) 204) : Color.FromArgb(byte.MaxValue, (byte) 26, (byte) 26, (byte) 26);
        return SystemMessageViewModel.sysMsgFgBrush = new SolidColorBrush(color);
      }
    }

    public override SolidColorBrush BackgroundBrush
    {
      get
      {
        return ChatPage.Current != null && ChatPage.Current.IsWallpaperSet ? SystemMessageViewModel.SystemMessageBackgroundBrushOverWallpaper : SystemMessageViewModel.SystemMessageBackgroundBrush;
      }
    }

    public override bool ShouldShowTail => false;

    public override Brush ForegroundBrush
    {
      get
      {
        return ChatPage.Current != null && ChatPage.Current.IsWallpaperSet ? (Brush) SystemMessageViewModel.SystemMessageForegroundBrushOverWallpaper : (Brush) SystemMessageViewModel.SystemMessageForegroundBrush;
      }
    }

    public override bool IsSelectable => false;

    public override string SenderJid => (string) null;

    public override HorizontalAlignment HorizontalAlignment => HorizontalAlignment.Center;

    public override double MaxBubbleWidth => double.PositiveInfinity;

    public override Thickness BubbleMargin => new Thickness(12.0 * this.zoomMultiplier);

    public override Thickness ViewPanelMargin
    {
      get
      {
        return new Thickness(12.0 * this.zoomMultiplier, 6.0 * this.zoomMultiplier, 12.0 * this.zoomMultiplier, 8.0 * this.zoomMultiplier);
      }
    }

    public override bool ShouldShowHeader => false;

    public override bool ShouldShowFooter => false;

    public SystemMessageViewModel(Message m)
      : base(m)
    {
    }

    protected override string GetTextStr()
    {
      return this.Message.GetSystemMessage(this.SenderJid, actionable: true);
    }

    public override void RefreshOnChatBackgroundChanged() => this.Notify("ChatBackgroundChanged");

    public override void RefreshOnContactChanged(string targetJid)
    {
      this.TextStrCache = (string) null;
      this.Notify("TextChanged");
    }

    public virtual bool ShowSystemMessageIcon
    {
      get
      {
        return this.Message.GetSystemMessageType() == SystemMessageWrapper.MessageTypes.ConversationEncrypted;
      }
    }

    public System.Windows.Media.ImageSource SystemMessageIcon
    {
      get
      {
        return this.ShowSystemMessageIcon ? (System.Windows.Media.ImageSource) AssetStore.LoadAsset("encrypted.png") : (System.Windows.Media.ImageSource) null;
      }
    }

    public override bool EnableContextMenu
    {
      get
      {
        bool enableContextMenu = true;
        switch (this.Message.GetSystemMessageType())
        {
          case SystemMessageWrapper.MessageTypes.ParticipantChange:
          case SystemMessageWrapper.MessageTypes.SubjectChange:
          case SystemMessageWrapper.MessageTypes.GroupPhotoChange:
          case SystemMessageWrapper.MessageTypes.BroadcastListCreated:
          case SystemMessageWrapper.MessageTypes.GainedAdmin:
          case SystemMessageWrapper.MessageTypes.GroupDeleted:
          case SystemMessageWrapper.MessageTypes.GroupCreated:
          case SystemMessageWrapper.MessageTypes.IdentityChanged:
          case SystemMessageWrapper.MessageTypes.ConversationEncrypted:
          case SystemMessageWrapper.MessageTypes.ConvBizIsVerified:
          case SystemMessageWrapper.MessageTypes.ConvBizIsUnVerified:
          case SystemMessageWrapper.MessageTypes.ConvBizNowStandard:
          case SystemMessageWrapper.MessageTypes.ConvBizNowUnverified:
          case SystemMessageWrapper.MessageTypes.ConvBizNowVerified:
          case SystemMessageWrapper.MessageTypes.GroupDescriptionChanged:
          case SystemMessageWrapper.MessageTypes.GroupRestrictionLocked:
          case SystemMessageWrapper.MessageTypes.GroupRestrictionUnlocked:
          case SystemMessageWrapper.MessageTypes.GroupMadeAnnouncementOnly:
          case SystemMessageWrapper.MessageTypes.GroupMadeNotAnnouncementOnly:
          case SystemMessageWrapper.MessageTypes.LostAdmin:
          case SystemMessageWrapper.MessageTypes.GroupDescriptionDeleted:
            enableContextMenu = false;
            break;
        }
        return enableContextMenu;
      }
    }

    public virtual bool IsTapAllowed => true;
  }
}
