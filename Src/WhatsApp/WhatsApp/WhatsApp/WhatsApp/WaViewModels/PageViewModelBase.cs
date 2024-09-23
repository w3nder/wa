// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaViewModels.PageViewModelBase
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System.Windows;


namespace WhatsApp.WaViewModels
{
  public abstract class PageViewModelBase : WaViewModelBase
  {
    private PageOrientation orientation;

    public PageOrientation Orientation
    {
      get => this.orientation;
      set
      {
        this.orientation = value;
        this.OnOrientationChanged();
      }
    }

    public virtual Thickness PageMargin => new Thickness(0.0);

    public virtual string PageTitle => (string) null;

    public virtual string PageSubtitle => (string) null;

    public double ZoomFactor => ResolutionHelper.ZoomFactor;

    public PageViewModelBase(PageOrientation initialOrientation)
    {
      this.orientation = initialOrientation;
    }

    protected virtual void OnOrientationChanged() => this.NotifyPropertyChanged("PageMargin");
  }
}
