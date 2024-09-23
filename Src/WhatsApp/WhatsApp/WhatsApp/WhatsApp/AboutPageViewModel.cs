// Decompiled with JetBrains decompiler
// Type: WhatsApp.AboutPageViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media.Imaging;
using WhatsApp.WaViewModels;


namespace WhatsApp
{
  public class AboutPageViewModel : PageViewModelBase
  {
    public AboutPageViewModel(PageOrientation initialOrientation)
      : base(initialOrientation)
    {
    }

    public override string PageTitle => AppResources.AboutPageTitle;

    public Thickness TitlePanelMargin
    {
      get
      {
        switch (this.Orientation)
        {
          case PageOrientation.LandscapeLeft:
            return new Thickness(UIUtils.SystemTraySizeLandscape, 0.0, 0.0, 0.0);
          case PageOrientation.LandscapeRight:
            return new Thickness(0.0);
          default:
            return new Thickness(0.0, UIUtils.SystemTraySizePortrait, 0.0, 0.0);
        }
      }
    }

    public Thickness LogoMargin
    {
      get
      {
        return this.Orientation == PageOrientation.LandscapeLeft ? new Thickness(UIUtils.SystemTraySizeLandscape, 0.0, 0.0, 0.0) : new Thickness(0.0);
      }
    }

    public int ButtonPanelRow => !this.Orientation.IsLandscape() ? 2 : 1;

    public int ButtonPanelCol => !this.Orientation.IsLandscape() ? 0 : 1;

    public Thickness ButtonPanelMargin
    {
      get
      {
        double left = this.Orientation.IsLandscape() ? 0.0 : UIUtils.PageSideMargin;
        double num1 = this.Orientation == PageOrientation.LandscapeRight ? UIUtils.PageSideMargin + UIUtils.SystemTraySizeLandscape : 0.0;
        int num2 = this.Orientation.IsLandscape() ? 0 : 48;
        double right = num1;
        double bottom = (double) num2;
        return new Thickness(left, 0.0, right, bottom);
      }
    }

    public string VersionStr
    {
      get
      {
        return string.Format("{0} {1}", (object) AppResources.VersionHeader.ToLangFriendlyLower(), (object) AppState.GetAppVersion());
      }
    }

    public int VersionBlockRow => !this.Orientation.IsLandscape() ? 3 : 4;

    public Thickness VersionBlockMargin
    {
      get
      {
        return new Thickness(this.Orientation == PageOrientation.LandscapeLeft ? UIUtils.PageSideMargin + UIUtils.SystemTraySizeLandscape : UIUtils.PageSideMargin, 0.0, 0.0, (double) (this.Orientation.IsLandscape() ? 0 : 24));
      }
    }

    public string CopyRightStr
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "© 2010-{0} WhatsApp Inc.", (object) AppState.GetBuildTime().Year);
      }
    }

    public int CopyRightBlockCol => !this.Orientation.IsLandscape() ? 0 : 1;

    public int CopyRightBlockColSpan => !this.Orientation.IsLandscape() ? 2 : 1;

    public Thickness CopyRightBlockMargin
    {
      get
      {
        return new Thickness(this.Orientation.IsLandscape() ? 0.0 : UIUtils.PageSideMargin, 0.0, this.Orientation == PageOrientation.LandscapeRight ? UIUtils.PageSideMargin + UIUtils.SystemTraySizeLandscape : UIUtils.PageSideMargin, 0.0);
      }
    }

    public new double ZoomFactor => ResolutionHelper.ZoomFactor;

    protected override void OnOrientationChanged()
    {
      base.OnOrientationChanged();
      this.NotifyPropertyChanged("TitlePanelMargin");
      this.NotifyPropertyChanged("LogoMargin");
      this.NotifyPropertyChanged("ButtonPanelRow");
      this.NotifyPropertyChanged("ButtonPanelCol");
      this.NotifyPropertyChanged("ButtonPanelMargin");
      this.NotifyPropertyChanged("VersionBlockRow");
      this.NotifyPropertyChanged("VersionBlockMargin");
      this.NotifyPropertyChanged("CopyRightBlockCol");
      this.NotifyPropertyChanged("CopyRightBlockColSpan");
      this.NotifyPropertyChanged("CopyRightBlockMargin");
    }

    public class ButtonViewModel
    {
      public BitmapSource Icon { get; set; }

      public string Label { get; set; }

      public Action OnClickAction { get; set; }
    }
  }
}
