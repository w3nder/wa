// Decompiled with JetBrains decompiler
// Type: WhatsApp.ListPickerPageViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System.Windows;
using System.Windows.Media;
using WhatsApp.WaViewModels;


namespace WhatsApp
{
  public class ListPickerPageViewModel : PageViewModelBase
  {
    private string title;
    private string subtitle;
    private Brush backgroundBrush = UIUtils.PhoneChromeBrush;

    public ListPickerPageViewModel(
      PageOrientation initialOrientation,
      string customTitle = null,
      string customSubtitle = null)
      : base(initialOrientation)
    {
      this.title = customTitle;
      this.subtitle = customSubtitle;
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

    public override string PageTitle => this.title ?? AppResources.ChooseAnOption;

    public string Subtitle => this.subtitle;

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
