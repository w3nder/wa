// Decompiled with JetBrains decompiler
// Type: WhatsApp.EmojiKeyboardViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Windows;
using System.Windows.Media;
using WhatsApp.WaViewModels;

#nullable disable
namespace WhatsApp
{
  public class EmojiKeyboardViewModel : WaViewModelBase
  {
    private EmojiKeyboardViewModel.Type selectedType;
    private Subject<Unit> popupSizeChangedSubject = new Subject<Unit>();
    private bool isTextBoxFocused_;
    private Brush inactiveTextBoxBrush_ = (Brush) new SolidColorBrush(Colors.LightGray);
    private Brush activeTextBoxBrush_ = (Brush) new SolidColorBrush(Colors.White);
    private Brush activeTextBoxBorderBrush_;
    private PageOrientation orientation;
    private double portraitSIPHeight;
    private double landscapeSIPHeight;
    private bool accentHighlight_ = true;
    private bool isSearchOpen;

    public EmojiKeyboardViewModel()
    {
      this.SelectedType = EmojiKeyboardViewModel.Type.Emoji;
      this.IsSearchSupported = false;
    }

    public bool IsSearchSupported { get; set; }

    public EmojiKeyboardViewModel.Type SelectedType
    {
      get => this.selectedType;
      set
      {
        if (this.selectedType != value)
          this.selectedType = value;
        this.NotifyPropertyChanged("BackspaceKeyVisibility");
      }
    }

    public IObservable<Unit> PopupSizeChangedObservable()
    {
      return (IObservable<Unit>) this.popupSizeChangedSubject;
    }

    public Visibility BackspaceKeyVisibility
    {
      get => (this.selectedType == EmojiKeyboardViewModel.Type.Emoji).ToVisibility();
    }

    public Visibility GifPickerVisibility
    {
      get => (this.selectedType == EmojiKeyboardViewModel.Type.Gif).ToVisibility();
    }

    public Visibility EmojiPickerVisibility
    {
      get => (this.selectedType == EmojiKeyboardViewModel.Type.Emoji).ToVisibility();
    }

    public Visibility StickerPickerVisibility
    {
      get => (this.selectedType == EmojiKeyboardViewModel.Type.Sticker).ToVisibility();
    }

    public Brush EmojiButtonForegroundBrush
    {
      get
      {
        return this.selectedType != EmojiKeyboardViewModel.Type.Emoji ? (Brush) UIUtils.ForegroundBrush : (Brush) UIUtils.AccentBrush;
      }
    }

    public Brush GifButtonForegroundBrush
    {
      get
      {
        return this.selectedType != EmojiKeyboardViewModel.Type.Gif ? (Brush) UIUtils.ForegroundBrush : (Brush) UIUtils.AccentBrush;
      }
    }

    public Brush StickerButtonForegroundBrush
    {
      get
      {
        return this.selectedType != EmojiKeyboardViewModel.Type.Sticker ? (Brush) UIUtils.ForegroundBrush : (Brush) UIUtils.AccentBrush;
      }
    }

    public Visibility SearchButtonVisibility
    {
      get
      {
        switch (this.selectedType)
        {
          case EmojiKeyboardViewModel.Type.Emoji:
            return this.IsSearchSupported.ToVisibility();
          case EmojiKeyboardViewModel.Type.Gif:
            return Visibility.Visible;
          default:
            return Visibility.Collapsed;
        }
      }
    }

    public Visibility AddButtonVisibility => Visibility.Collapsed;

    public Visibility SearchBarVisibility => this.IsSearchOpen.ToVisibility();

    public Visibility BottomBarVisibility
    {
      get => !this.IsSearchOpen ? Visibility.Visible : Visibility.Collapsed;
    }

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
      }
    }

    public double PickerHeightVisible
    {
      get
      {
        switch (this.selectedType)
        {
          case EmojiKeyboardViewModel.Type.Emoji:
            return ResolutionHelper.ZoomMultiplier * 140.0;
          case EmojiKeyboardViewModel.Type.Gif:
            return ResolutionHelper.ZoomMultiplier * 200.0;
          default:
            return ResolutionHelper.ZoomMultiplier * 200.0;
        }
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
      get => !this.IsTextBoxFocused ? this.inactiveTextBoxBrush_ : this.activeTextBoxBrush_;
    }

    public Brush TextBoxBorderBrush
    {
      get
      {
        return !this.IsTextBoxFocused ? this.InactiveTextBoxBorderBrush : this.ActiveTextBoxBorderBrush;
      }
    }

    public Visibility KeyboardPlaceHolderVisibility => this.IsTextBoxFocused.ToVisibility();

    public PageOrientation Orientation
    {
      get => this.orientation;
      set
      {
        if (this.orientation == value)
          return;
        this.orientation = value;
      }
    }

    public double KeyboardPlaceHolderHeight
    {
      get
      {
        if (this.Orientation.IsPortrait())
          return this.portraitSIPHeight;
        return this.Orientation.IsLandscape() ? this.landscapeSIPHeight : 0.0;
      }
      set
      {
        if (this.Orientation.IsPortrait())
        {
          this.portraitSIPHeight = value;
        }
        else
        {
          if (!this.Orientation.IsLandscape())
            return;
          this.landscapeSIPHeight = value;
        }
      }
    }

    public double PopupHeight
    {
      get
      {
        return !this.IsSearchOpen ? UIUtils.SIPHeightPortrait : UIUtils.SIPHeightPortrait + this.PickerHeightVisible + UIUtils.SIPHeightAdjustmentForWP10;
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

    public bool IsSearchOpen
    {
      get => this.isSearchOpen;
      set
      {
        if (this.isSearchOpen == value)
          return;
        Log.d("EmojiKeyboard", "Search opened set | {0}", (object) value);
        this.isSearchOpen = value;
        this.popupSizeChangedSubject.OnNext(new Unit());
      }
    }

    public System.Windows.Media.ImageSource SearchImage
    {
      get => (System.Windows.Media.ImageSource) AssetStore.KeypadSearchButtonIcon;
    }

    public System.Windows.Media.ImageSource CancelSearchImage
    {
      get => (System.Windows.Media.ImageSource) AssetStore.EmojiCancelSearchButtonIcon;
    }

    public System.Windows.Media.ImageSource DeleteIconImage => AssetStore.KeypadCancelIcon;

    public System.Windows.Media.ImageSource BackspaceImage
    {
      get => (System.Windows.Media.ImageSource) AssetStore.KeypadBackSpaceIcon;
    }

    public System.Windows.Media.ImageSource GifImage => AssetStore.KeypadGifIcon;

    public System.Windows.Media.ImageSource EmojiImage => (System.Windows.Media.ImageSource) AssetStore.KeypadEmojiIcon;

    public System.Windows.Media.ImageSource StickerImage
    {
      get => (System.Windows.Media.ImageSource) AssetStore.KeypadStickerIcon;
    }

    public System.Windows.Media.ImageSource AddButtonImage
    {
      get => (System.Windows.Media.ImageSource) AssetStore.KeypadPlusIcon;
    }

    public enum Type
    {
      Emoji,
      Gif,
      Sticker,
    }
  }
}
