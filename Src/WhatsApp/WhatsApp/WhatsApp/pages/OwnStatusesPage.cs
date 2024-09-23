// Decompiled with JetBrains decompiler
// Type: WhatsApp.OwnStatusesPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;


namespace WhatsApp
{
  public class OwnStatusesPage : PhoneApplicationPage
  {
    private static object nextInstanceIntendedStart;
    private bool isPageLoaded;
    private bool intendedStart;
    internal ZoomBox LayoutRootZoomBox;
    internal Grid LayoutRoot;
    internal PageTitlePanel PageTitle;
    internal OwnStatusList StatusList;
    private bool _contentLoaded;

    public OwnStatusesPage()
    {
      this.InitializeComponent();
      this.LayoutRootZoomBox.ZoomFactor = ResolutionHelper.ZoomFactor;
      this.PageTitle.SmallTitle = AppResources.MyStatusUpper;
      this.Loaded += new RoutedEventHandler(this.OnLoaded);
      this.StatusList.Emptied += (EventHandler) ((sender, e) => NavUtils.GoBack());
      if (OwnStatusesPage.nextInstanceIntendedStart == null)
        return;
      this.StatusList.Load(true);
      OwnStatusesPage.nextInstanceIntendedStart = (object) null;
      this.intendedStart = true;
    }

    public static void Start()
    {
      OwnStatusesPage.nextInstanceIntendedStart = new object();
      NavUtils.NavigateToPage(nameof (OwnStatusesPage));
    }

    private void OnLoaded(object sender, EventArgs e)
    {
      if (this.isPageLoaded)
        return;
      this.isPageLoaded = true;
      this.StatusList.Show(new int?());
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);
      if (this.intendedStart)
        return;
      this.Dispatcher.BeginInvoke((Action) (() => NavUtils.GoBack(this.NavigationService)));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/OwnStatusesPage.xaml", UriKind.Relative));
      this.LayoutRootZoomBox = (ZoomBox) this.FindName("LayoutRootZoomBox");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.PageTitle = (PageTitlePanel) this.FindName("PageTitle");
      this.StatusList = (OwnStatusList) this.FindName("StatusList");
    }
  }
}
