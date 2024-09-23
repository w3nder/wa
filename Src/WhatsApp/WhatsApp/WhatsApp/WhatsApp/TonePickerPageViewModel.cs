// Decompiled with JetBrains decompiler
// Type: WhatsApp.TonePickerPageViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System.Windows;
using System.Windows.Media;
using WhatsApp.WaViewModels;


namespace WhatsApp
{
  public class TonePickerPageViewModel : PageViewModelBase
  {
    private bool showResetToDefault;
    private Brush backgroundBrush = UIUtils.PhoneChromeBrush;

    public TonePickerPageViewModel(PageOrientation initialOrientation)
      : base(initialOrientation)
    {
    }

    public override Thickness PageMargin
    {
      get
      {
        switch (this.Orientation)
        {
          case PageOrientation.LandscapeLeft:
            return new Thickness(72.0, 0.0, 0.0, 0.0);
          case PageOrientation.LandscapeRight:
            return new Thickness(0.0, 0.0, 72.0, 0.0);
          default:
            return new Thickness(0.0, 32.0, 0.0, 0.0);
        }
      }
    }

    public override string PageTitle => AppResources.ChooseAnOption;

    public bool ShowResetToDefault
    {
      set
      {
        this.showResetToDefault = value;
        this.NotifyPropertyChanged("ResetToDefaultVisibility");
      }
    }

    public Visibility ResetToDefaultVisibility => this.showResetToDefault.ToVisibility();

    public string ResetToDefaultStr => AppResources.ResetToDefault;

    public Brush BackgroundBrush
    {
      get => this.backgroundBrush;
      set
      {
        this.backgroundBrush = value;
        this.NotifyPropertyChanged(nameof (BackgroundBrush));
      }
    }
  }
}
