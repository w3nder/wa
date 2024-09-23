// Decompiled with JetBrains decompiler
// Type: WhatsApp.ChatInputBarViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System.Windows;
using System.Windows.Media;
using WhatsApp.WaViewModels;


namespace WhatsApp
{
  public class ChatInputBarViewModel : WaViewModelBase
  {
    private bool isTextEmptyOrWhiteSpace = true;
    private bool hasAnyText;
    private bool isTextBoxFocused;
    private bool isEmojiKeyboardOpen;
    private bool enableRecordAction = true;
    private Brush redBrush = (Brush) new SolidColorBrush(Colors.Red);
    private bool isRecordingInProgress;

    public PageOrientation OwnerPageOrientation { get; set; }

    public Thickness ContentMargin
    {
      get
      {
        Thickness? nullable = new Thickness?();
        switch (this.OwnerPageOrientation)
        {
          case PageOrientation.Landscape:
          case PageOrientation.LandscapeLeft:
            nullable = new Thickness?(new Thickness(UIUtils.SystemTraySizeLandscape, 0.0, 0.0, 0.0));
            break;
          case PageOrientation.LandscapeRight:
            nullable = new Thickness?(new Thickness(0.0, 0.0, UIUtils.SystemTraySizeLandscape, 0.0));
            break;
        }
        return nullable ?? new Thickness(0.0);
      }
    }

    public Thickness MainPanelMargin => this.ContentMargin;

    public double MainPanelMinHeight => 72.0 * this.zoomMultiplier;

    public double MainPanelMaxHeight => 200.0 * this.zoomMultiplier;

    public double ActionButtonSize => 72.0 * this.zoomMultiplier;

    public double ActionButtonIconSize => 36.0 * this.zoomMultiplier;

    public double EmojiButtonIconSize => 32.0 * this.zoomMultiplier;

    public System.Windows.Media.ImageSource AttachButtonIconSource
    {
      get => (System.Windows.Media.ImageSource) AssetStore.InputBarAttachIcon;
    }

    public System.Windows.Media.ImageSource EmojiButtonIconSource
    {
      get
      {
        return !this.IsEmojiKeyboardOpen ? (System.Windows.Media.ImageSource) AssetStore.InputBarEmojiIcon : (System.Windows.Media.ImageSource) AssetStore.InputBarKeyboardIcon;
      }
    }

    public System.Windows.Media.ImageSource RightButtonIconSource
    {
      get
      {
        return this.RightButtonType != ChatInputBar.Actions.Record ? (ImageStore.IsDarkTheme() || !this.IsDisabledButtonIcon ? (System.Windows.Media.ImageSource) AssetStore.InputBarSendIcon : (System.Windows.Media.ImageSource) AssetStore.InputBarSendIconLight) : (System.Windows.Media.ImageSource) AssetStore.InputBarMicIcon;
      }
    }

    public Brush RightButtonBackgroundBrush
    {
      get
      {
        return this.isTextEmptyOrWhiteSpace || this.RightButtonType != ChatInputBar.Actions.SendText ? (Brush) UIUtils.TransparentBrush : (Brush) UIUtils.AccentBrush;
      }
    }

    private bool IsDisabledButtonIcon
    {
      get
      {
        bool disabledButtonIcon = false;
        switch (this.RightButtonType)
        {
          case ChatInputBar.Actions.SendText:
            disabledButtonIcon = this.isTextEmptyOrWhiteSpace;
            break;
          case ChatInputBar.Actions.Record:
            disabledButtonIcon = !this.enableRecordAction;
            break;
        }
        return disabledButtonIcon;
      }
    }

    public double RightButtonOpacity => !this.IsDisabledButtonIcon ? 1.0 : 0.35;

    public double TextBoxFontSize => Settings.SystemFontSize * this.zoomMultiplier;

    public bool IsTextEmptyOrWhiteSpace
    {
      get => this.isTextEmptyOrWhiteSpace;
      set
      {
        if (this.isTextEmptyOrWhiteSpace == value)
          return;
        this.isTextEmptyOrWhiteSpace = value;
        if (!ImageStore.IsDarkTheme())
          this.NotifyPropertyChanged("RightButtonIconSource");
        this.NotifyPropertyChanged("RightButtonBackgroundBrush");
        this.NotifyPropertyChanged("RightButtonOpacity");
      }
    }

    public bool HasAnyText
    {
      set
      {
        if (this.hasAnyText == value)
          return;
        this.hasAnyText = value;
        this.NotifyPropertyChanged("TextInputTooltipVisibility");
      }
    }

    public bool IsTextBoxFocused
    {
      get => this.isTextBoxFocused;
      set
      {
        this.isTextBoxFocused = value;
        this.NotifyPropertyChanged("RightButtonIconSource");
        this.NotifyPropertyChanged("RightButtonBackgroundBrush");
        this.NotifyPropertyChanged("RightButtonOpacity");
      }
    }

    public bool IsEmojiKeyboardOpen
    {
      get => this.isEmojiKeyboardOpen;
      set
      {
        this.isEmojiKeyboardOpen = value;
        this.NotifyPropertyChanged("RightButtonIconSource");
        this.NotifyPropertyChanged("RightButtonBackgroundBrush");
        this.NotifyPropertyChanged("RightButtonOpacity");
      }
    }

    public ChatInputBar.Actions RightButtonType
    {
      get
      {
        return !this.EnableRecordAction || !this.isTextEmptyOrWhiteSpace ? ChatInputBar.Actions.SendText : ChatInputBar.Actions.Record;
      }
    }

    public bool EnableRecordAction
    {
      get => this.enableRecordAction && !this.IsTextBoxFocused && !this.IsEmojiKeyboardOpen;
      set
      {
        this.enableRecordAction = value;
        this.NotifyPropertyChanged("RightButtonIconSource");
        this.NotifyPropertyChanged("RightButtonBackgroundBrush");
        this.NotifyPropertyChanged("RightButtonOpacity");
      }
    }

    public System.Windows.Media.ImageSource RecordingIndicatorIconSource
    {
      get => (System.Windows.Media.ImageSource) AssetStore.InputBarMicIconRed;
    }

    public Brush RecordingIndicatorBrush
    {
      get => !this.isRecordingInProgress ? UIUtils.SubtleBrush : this.redBrush;
    }

    public double DurationBlockFontSize => 36.0 * this.zoomMultiplier;

    public Thickness DurationBlockMargin => new Thickness(0.0, 2.0 * this.zoomMultiplier, 0.0, 0.0);

    public Thickness SlideToCancelPanelMargin
    {
      get => new Thickness(0.0, 0.0, 2.0 * this.ActionButtonSize, 0.0);
    }

    public string TextInputTooltipStr => AppResources.TextInputTooltip;

    public Visibility TextInputTooltipVisibility => (!this.hasAnyText).ToVisibility();

    public Thickness TextInputTooltipMargin
    {
      get => new Thickness(6.0 * this.zoomMultiplier, -4.0 * this.zoomMultiplier, 0.0, 0.0);
    }

    public bool IsRecordingInProgress
    {
      set
      {
        if (this.isRecordingInProgress == value)
          return;
        this.isRecordingInProgress = value;
        this.NotifyPropertyChanged("RecordingIndicatorBrush");
      }
    }
  }
}
