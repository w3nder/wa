// Decompiled with JetBrains decompiler
// Type: WhatsApp.EmojiTextBoxViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Windows;
using System.Windows.Media;

#nullable disable
namespace WhatsApp
{
  public class EmojiTextBoxViewModel : PropChangedBase
  {
    private bool isTextBoxFocused_;
    private Brush inactiveTextBoxBrush_ = (Brush) new SolidColorBrush(Colors.LightGray);
    private Brush activeTextBoxBrush_ = (Brush) new SolidColorBrush(Colors.White);
    private Brush activeTextBoxBorderBrush_;
    private Brush redBrush_ = (Brush) new SolidColorBrush(Colors.Red);
    private int lengthAlertRange = 10;
    private int maxLength;
    private int currentTextLength;
    private bool accentHighlight_ = true;
    private bool isEmojiKeyboardOpen_;

    public bool IsTextBoxFocused
    {
      get => this.isTextBoxFocused_;
      set
      {
        if (this.isTextBoxFocused_ == value)
          return;
        this.isTextBoxFocused_ = value;
        this.NotifyPropertyChanged("TextBoxBrush");
        this.NotifyPropertyChanged("TextBoxBorderBrush");
        this.NotifyPropertyChanged("LengthHintForeground");
      }
    }

    private Brush ActiveTextBoxBorderBrush
    {
      get
      {
        if (this.activeTextBoxBorderBrush_ == null)
          this.activeTextBoxBorderBrush_ = this.AccentHighlight ? (Brush) UIUtils.AccentBrush : (!ImageStore.IsDarkTheme() || this.ForceBorderHighlight ? (Brush) new SolidColorBrush(Colors.Black) : this.activeTextBoxBrush_);
        return this.activeTextBoxBorderBrush_;
      }
    }

    private Brush InactiveTextBoxBorderBrush => this.inactiveTextBoxBrush_;

    public Brush TextBoxBrush
    {
      get
      {
        return !this.IsTextBoxFocused && !this.isEmojiKeyboardOpen_ ? this.inactiveTextBoxBrush_ : this.activeTextBoxBrush_;
      }
    }

    public Brush TextBoxBorderBrush
    {
      get
      {
        return !this.IsTextBoxFocused && !this.isEmojiKeyboardOpen_ ? this.InactiveTextBoxBorderBrush : this.ActiveTextBoxBorderBrush;
      }
    }

    public bool ShowLengthHint => this.maxLength > 0;

    public Visibility LengthHintPanelVisibility
    {
      get => this.maxLength <= 0 ? Visibility.Collapsed : Visibility.Visible;
    }

    public string MaxLengthText
    {
      get => this.maxLength <= 0 ? "" : string.Format("/{0}", (object) this.maxLength);
    }

    public string CurrentTextLengthStr
    {
      get => this.maxLength <= 0 ? "" : this.currentTextLength.ToString();
    }

    public Brush CounterBrush { get; set; }

    public Brush LengthHintForeground
    {
      get
      {
        return this.isTextBoxFocused_ && this.maxLength > 0 && this.ShouldAlert(this.currentTextLength) ? this.redBrush_ : this.CounterBrush ?? UIUtils.SubtleBrush;
      }
    }

    public bool HasRoomForEmoji => this.maxLength <= 0 || this.currentTextLength < this.maxLength;

    public double ActionButtonIconOpacity
    {
      get => !this.isEmojiKeyboardOpen_ && !this.HasRoomForEmoji ? 0.3 : 1.0;
    }

    public int MaxLength
    {
      set
      {
        this.maxLength = value;
        this.lengthAlertRange = (int) Math.Max(3.0, (double) this.maxLength * 0.1);
        this.NotifyPropertyChanged("LengthHintPanelVisibility");
        this.NotifyPropertyChanged("MaxLengthText");
        this.NotifyPropertyChanged("CurrentTextLengthStr");
        this.NotifyPropertyChanged("LengthHintForeground");
        this.NotifyPropertyChanged("ActionButtonIconOpacity");
      }
    }

    private bool ShouldAlert(int length) => this.maxLength - length <= this.lengthAlertRange;

    public int CurrentTextLength
    {
      set
      {
        int currentTextLength = this.currentTextLength;
        this.currentTextLength = value;
        this.NotifyPropertyChanged("CurrentTextLengthStr");
        this.NotifyPropertyChanged("ActionButtonIconOpacity");
        if (this.ShouldAlert(currentTextLength) == this.ShouldAlert(this.currentTextLength))
          return;
        this.NotifyPropertyChanged("LengthHintForeground");
      }
    }

    public bool AccentHighlight
    {
      get => this.accentHighlight_;
      set
      {
        this.accentHighlight_ = value;
        this.activeTextBoxBorderBrush_ = (Brush) null;
        this.NotifyPropertyChanged("TextBoxBorderBrush");
      }
    }

    public bool ForceBorderHighlight { get; set; }

    public bool IsEmojiKeyboardOpen
    {
      get => this.isEmojiKeyboardOpen_;
      set
      {
        if (this.isEmojiKeyboardOpen_ == value)
          return;
        this.isEmojiKeyboardOpen_ = value;
        this.NotifyPropertyChanged("ActionButtonIcon");
        this.NotifyPropertyChanged("ActionButtonIconOpacity");
        this.NotifyPropertyChanged("TextBoxBrush");
        this.NotifyPropertyChanged("TextBoxBorderBrush");
      }
    }

    public System.Windows.Media.ImageSource ActionButtonIcon
    {
      get
      {
        return !this.isEmojiKeyboardOpen_ ? (System.Windows.Media.ImageSource) AssetStore.TextboxEmojiIcon : (System.Windows.Media.ImageSource) AssetStore.TextboxKeyboardIcon;
      }
    }
  }
}
