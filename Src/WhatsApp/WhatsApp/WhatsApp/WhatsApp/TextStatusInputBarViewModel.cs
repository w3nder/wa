// Decompiled with JetBrains decompiler
// Type: WhatsApp.TextStatusInputBarViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System.Windows;
using System.Windows.Media;
using WhatsApp.WaViewModels;


namespace WhatsApp
{
  public class TextStatusInputBarViewModel : WaViewModelBase
  {
    private Brush textBg;
    private WhatsApp.ProtoBuf.Message.ExtendedTextMessage.FontType font;
    private bool isTextEmptyOrWhiteSpace = true;
    private bool hasAnyText;
    private bool isTextBoxFocused;
    private bool isEmojiKeyboardOpen;

    public TextStatusInputBarViewModel(bool zoomed)
    {
      this.zoomMultiplier = zoomed ? ResolutionHelper.ZoomMultiplier : 1.0;
    }

    public Brush TextBackground
    {
      get => this.textBg ?? (Brush) new SolidColorBrush(Colors.Transparent);
      set
      {
        this.textBg = value;
        this.NotifyPropertyChanged(nameof (TextBackground));
      }
    }

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

    public double ActionButtonSize => 72.0 * this.zoomMultiplier;

    public double ActionButtonIconSize => 36.0 * this.zoomMultiplier;

    public System.Windows.Media.ImageSource EmojiButtonIconSource
    {
      get
      {
        return !this.IsEmojiKeyboardOpen ? (System.Windows.Media.ImageSource) AssetStore.InputBarStatusEmojiIcon : (System.Windows.Media.ImageSource) AssetStore.InputBarKeyboardIconWhite;
      }
    }

    public WhatsApp.ProtoBuf.Message.ExtendedTextMessage.FontType Font
    {
      get => this.font;
      set
      {
        this.font = value;
        this.NotifyPropertyChanged("FontButtonIconSource");
      }
    }

    public System.Windows.Media.ImageSource FontButtonIconSource
    {
      get
      {
        System.Windows.Media.ImageSource buttonIconSource;
        switch (this.font)
        {
          case WhatsApp.ProtoBuf.Message.ExtendedTextMessage.FontType.SERIF:
            buttonIconSource = (System.Windows.Media.ImageSource) AssetStore.InputBarFontIconSegue;
            break;
          case WhatsApp.ProtoBuf.Message.ExtendedTextMessage.FontType.NORICAN_REGULAR:
            buttonIconSource = (System.Windows.Media.ImageSource) AssetStore.InputBarFontIconNorican;
            break;
          case WhatsApp.ProtoBuf.Message.ExtendedTextMessage.FontType.BRYNDAN_WRITE:
            buttonIconSource = (System.Windows.Media.ImageSource) AssetStore.InputBarFontIconBryndan;
            break;
          case WhatsApp.ProtoBuf.Message.ExtendedTextMessage.FontType.OSWALD_HEAVY:
            buttonIconSource = (System.Windows.Media.ImageSource) AssetStore.InputBarFontIconOswald;
            break;
          default:
            buttonIconSource = (System.Windows.Media.ImageSource) AssetStore.InputBarFontIconSegue;
            break;
        }
        return buttonIconSource;
      }
    }

    public System.Windows.Media.ImageSource BackgroundButtonIconSource
    {
      get => (System.Windows.Media.ImageSource) AssetStore.InputBarColorIcon;
    }

    public System.Windows.Media.ImageSource SendButtonIconSource
    {
      get => (System.Windows.Media.ImageSource) AssetStore.InputBarStatusSendIcon;
    }

    public double SendButtonOpacity => this.IsTextEmptyOrWhiteSpace ? 0.0 : 1.0;

    public bool OuterActionButtonHitTestVisible => !this.IsTextBoxFocused;

    public double OuterActionButtonOpacity => this.IsTextBoxFocused ? 0.0 : 1.0;

    public double InnerActionButtonOpacity => this.IsTextBoxFocused ? 1.0 : 0.0;

    public Visibility OuterActionButtonVisibility => (!this.IsTextBoxFocused).ToVisibility();

    public Visibility InnerActionButtonVisibility => this.IsTextBoxFocused.ToVisibility();

    public double TextBoxFontSize => Settings.SystemFontSize * this.zoomMultiplier;

    public bool IsTextEmptyOrWhiteSpace
    {
      get => this.isTextEmptyOrWhiteSpace;
      set
      {
        if (this.isTextEmptyOrWhiteSpace == value)
          return;
        this.isTextEmptyOrWhiteSpace = value;
        this.NotifyPropertyChanged("SendButtonOpacity");
      }
    }

    public bool HasAnyText
    {
      set
      {
        if (this.hasAnyText == value)
          return;
        this.hasAnyText = value;
      }
    }

    public bool IsTextBoxFocused
    {
      get => this.isTextBoxFocused;
      set
      {
        this.isTextBoxFocused = value;
        this.NotifyPropertyChanged("SendButtonOpacity");
        this.NotifyPropertyChanged("OuterActionButtonVisibility");
        this.NotifyPropertyChanged("InnerActionButtonVisibility");
        this.NotifyPropertyChanged("OuterActionButtonOpacity");
        this.NotifyPropertyChanged("InnerActionButtonOpacity");
        this.NotifyPropertyChanged("OuterActionButtonHitTestVisible");
      }
    }

    public bool IsEmojiKeyboardOpen
    {
      get => this.isEmojiKeyboardOpen;
      set
      {
        this.isEmojiKeyboardOpen = value;
        this.NotifyPropertyChanged("SendButtonOpacity");
      }
    }

    public FlowDirection AppFlowDirection => App.CurrentApp.RootFrame.FlowDirection;
  }
}
