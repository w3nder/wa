// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaViewModels.ChatPageViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WhatsApp.WaCollections;

#nullable disable
namespace WhatsApp.WaViewModels
{
  public class ChatPageViewModel : PageViewModelBase
  {
    private IDisposable propertyChangedSub_;
    private Conversation convo_;
    private ChatPage.InputMode inputMode_ = ChatPage.InputMode.None;
    private bool isAnnouncementOnly;
    private bool isReadOnly;
    private bool shouldShowGroupDescription = true;
    private System.Windows.Media.ImageSource dismissButtonIcon;
    private bool isGroupParticipant = true;
    private bool wasConversationSeenOnEntry = true;
    private bool isMuted;

    public Conversation Conversation
    {
      get => this.convo_;
      set
      {
        if (this.convo_ == value)
          return;
        this.convo_ = value;
        this.IsConversationStatusChanged = false;
        this.ResetPropertyChangedSubscription();
      }
    }

    public bool IsConversationStatusChanged { get; set; }

    public ChatPage.InputMode InputMode
    {
      get => this.inputMode_;
      set
      {
        if (this.inputMode_ == value)
          return;
        this.inputMode_ = value;
      }
    }

    public bool IsSIPUp
    {
      get
      {
        return this.inputMode_ == ChatPage.InputMode.Keyboard || this.inputMode_ == ChatPage.InputMode.Emoji;
      }
    }

    public WallpaperStore.WallpaperState Wallpaper { get; set; }

    public SolidColorBrush TitleForeground
    {
      get
      {
        return this.Wallpaper != null ? this.Wallpaper.PreferredForegroundBrush : UIUtils.ForegroundBrush;
      }
    }

    public Color SysTrayForegroundColor
    {
      get
      {
        return !(this.Wallpaper == null || this.Wallpaper.ForegroundPreference == WallpaperStore.WallpaperState.ForegroundPreferences.Either ? ImageStore.IsDarkTheme() : this.Wallpaper.ForegroundPreference == WallpaperStore.WallpaperState.ForegroundPreferences.LightOnly) ? Colors.Black : Constants.SysTrayOffWhite;
      }
    }

    public Visibility TitleInfoPanelVisibility
    {
      get
      {
        return (this.shouldShowGroupDescription && !this.WasConversationSeenOnEntry && !string.IsNullOrEmpty(this.convo_.GroupDescription)).ToVisibility();
      }
    }

    public Visibility ReadOnlyPanelVisibility => this.isReadOnly.ToVisibility();

    public Visibility InputPanelVisibility => (!this.isReadOnly).ToVisibility();

    public string ReadOnlyHelpTextString
    {
      get
      {
        if (!this.isGroupParticipant)
          return AppResources.ReadOnlyGroupHelpText;
        return this.isAnnouncementOnly ? AppResources.AnnouncementGroupHelpText : "";
      }
    }

    public bool IsAnnouncementOnly
    {
      get => this.isAnnouncementOnly;
      set
      {
        if (this.isAnnouncementOnly == value)
          return;
        Log.l("chatpage", "announcement only: {0} -> {1}", (object) this.isAnnouncementOnly, (object) value);
        this.isAnnouncementOnly = value;
        if (!this.isAnnouncementOnly || this.IsReadOnly)
          return;
        this.IsReadOnly = true;
      }
    }

    public bool IsReadOnly
    {
      get => this.isReadOnly;
      set
      {
        if (this.isReadOnly == value)
          return;
        Log.l("chatpage", "read only: {0} -> {1}", (object) this.IsReadOnly, (object) value);
        this.isReadOnly = value;
        this.NotifyPropertyChanged("ReadOnlyPanelVisibility");
      }
    }

    public bool ShouldShowGroupDescription
    {
      get => this.shouldShowGroupDescription;
      set
      {
        if (this.shouldShowGroupDescription == value)
          return;
        Log.l("chatpage", "should show group description: {0} -> {1}", (object) this.ShouldShowGroupDescription, (object) value);
        this.shouldShowGroupDescription = value;
      }
    }

    public System.Windows.Media.ImageSource DimissButtonIcon
    {
      get
      {
        if (this.dismissButtonIcon == null)
        {
          BitmapSource dismissIconWhite = AssetStore.DismissIconWhite;
          if (dismissIconWhite == null)
            AppState.Worker.RunAfterDelay(TimeSpan.FromMilliseconds(500.0), (Action) (() => this.NotifyPropertyChanged(nameof (DimissButtonIcon))));
          else
            this.dismissButtonIcon = (System.Windows.Media.ImageSource) IconUtils.CreateColorIcon(dismissIconWhite, (Brush) UIUtils.ForegroundBrush, new double?((double) dismissIconWhite.PixelWidth));
        }
        return this.dismissButtonIcon;
      }
    }

    public bool IsGroupParticipant
    {
      get => this.isGroupParticipant;
      set
      {
        if (this.isGroupParticipant == value)
          return;
        Log.l("chatpage", "set is group participant: {0} -> {1}", (object) this.IsGroupParticipant, (object) value);
        this.isGroupParticipant = value;
        if (this.isGroupParticipant || this.IsReadOnly)
          return;
        this.IsReadOnly = true;
      }
    }

    public ChatPageViewModel(PageOrientation initialOrientation)
      : base(initialOrientation)
    {
    }

    public bool WasConversationSeenOnEntry
    {
      get => this.wasConversationSeenOnEntry;
      set
      {
        if (this.wasConversationSeenOnEntry == value)
          return;
        Log.l("chatpage", "wasConversationSeenOnEntry: {0} -> {1}", (object) this.wasConversationSeenOnEntry, (object) value);
        this.wasConversationSeenOnEntry = value;
        this.NotifyPropertyChanged(nameof (WasConversationSeenOnEntry));
      }
    }

    public bool IsMuted
    {
      get => this.isMuted;
      set
      {
        if (this.isMuted == value)
          return;
        Log.l("chatpage", "isMuted: {0} -> {1}", (object) this.isMuted, (object) value);
        this.isMuted = value;
        this.NotifyPropertyChanged("ToggleMuteButtonTitle");
      }
    }

    public bool AllowLinks
    {
      get
      {
        bool allowLinks = true;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db => allowLinks = SuspiciousJid.ShouldAllowLinksForJid(db, this.convo_.Jid)));
        return allowLinks;
      }
    }

    protected override void DisposeManagedResources()
    {
      this.propertyChangedSub_.SafeDispose();
      this.propertyChangedSub_ = (IDisposable) null;
      base.DisposeManagedResources();
    }

    private void ResetPropertyChangedSubscription()
    {
      this.propertyChangedSub_.SafeDispose();
      this.propertyChangedSub_ = (IDisposable) null;
      Conversation conversation = this.Conversation;
      if (conversation == null)
        return;
      Set<string> trackedConvoProperties = new Set<string>((IEnumerable<string>) new string[1]
      {
        "Status"
      });
      this.propertyChangedSub_ = conversation.GetPropertyChangedAsync().Where<PropertyChangedEventArgs>((Func<PropertyChangedEventArgs, bool>) (a => trackedConvoProperties.Contains(a.PropertyName))).ObserveOnDispatcher<PropertyChangedEventArgs>().Subscribe<PropertyChangedEventArgs>((Action<PropertyChangedEventArgs>) (a =>
      {
        if (!(a.PropertyName == "Status"))
          return;
        this.IsConversationStatusChanged = true;
      }));
    }
  }
}
