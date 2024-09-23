// Decompiled with JetBrains decompiler
// Type: WhatsApp.MessageDetailPageViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System.Windows;
using WhatsApp.WaViewModels;

#nullable disable
namespace WhatsApp
{
  public class MessageDetailPageViewModel : PageViewModelBase
  {
    public Thickness TitleMargin
    {
      get
      {
        switch (this.Orientation)
        {
          case PageOrientation.LandscapeLeft:
            return new Thickness(UIUtils.SystemTraySizeLandscape, 0.0, 0.0, 0.0);
          case PageOrientation.LandscapeRight:
            return new Thickness(0.0, 0.0, UIUtils.SystemTraySizeLandscape, 0.0);
          default:
            return new Thickness(0.0, UIUtils.SystemTraySizePortrait, 0.0, 0.0);
        }
      }
    }

    public Thickness HeaderMargin
    {
      get
      {
        switch (this.Orientation)
        {
          case PageOrientation.LandscapeLeft:
            return new Thickness(UIUtils.SystemTraySizeLandscape, 0.0, 0.0, 12.0);
          case PageOrientation.LandscapeRight:
            return new Thickness(0.0, 0.0, UIUtils.SystemTraySizeLandscape, 12.0);
          default:
            return new Thickness(0.0, 0.0, 0.0, 12.0);
        }
      }
    }

    public MessageDetailPageViewModel(PageOrientation initialOrientation)
      : base(initialOrientation)
    {
    }

    protected override void OnOrientationChanged()
    {
      base.OnOrientationChanged();
      this.NotifyPropertyChanged("TitleMargin");
    }
  }
}
