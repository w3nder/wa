// Decompiled with JetBrains decompiler
// Type: WhatsApp.MediaGalleryPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

#nullable disable
namespace WhatsApp
{
  public class MediaGalleryPage : PhoneApplicationPage
  {
    private static string nextInstanceTitle;
    private static string[] nextInstanceJids;
    private string[] jids;
    internal Grid LayoutRoot;
    internal PageTitlePanel PageTitle;
    internal Pivot Pivot;
    internal PivotItem MediaTab;
    internal MediaTabView MediaView;
    internal PivotItem LinkTab;
    internal LinksTabView LinksView;
    internal PivotItem DocsTab;
    internal DocsTabView DocsView;
    private bool _contentLoaded;

    public MediaGalleryPage()
    {
      this.InitializeComponent();
      this.jids = MediaGalleryPage.nextInstanceJids;
      MediaGalleryPage.nextInstanceJids = (string[]) null;
      string nextInstanceTitle = MediaGalleryPage.nextInstanceTitle;
      MediaGalleryPage.nextInstanceTitle = (string) null;
      this.PageTitle.SmallTitle = nextInstanceTitle;
      PivotHeaderConverter pivotHeaderConverter = new PivotHeaderConverter();
      this.MediaTab.Header = (object) pivotHeaderConverter.Convert(AppResources.Media);
      this.LinkTab.Header = (object) pivotHeaderConverter.Convert(AppResources.Links);
      this.DocsTab.Header = (object) pivotHeaderConverter.Convert(AppResources.Docs);
      PivotItem mediaTab = this.MediaTab;
      PivotItem linkTab = this.LinkTab;
      PivotItem docsTab = this.DocsTab;
      Thickness thickness1 = new Thickness(24.0 * ResolutionHelper.ZoomMultiplier, 0.0, 0.0, 0.0);
      Thickness thickness2 = thickness1;
      docsTab.Margin = thickness2;
      Thickness thickness3;
      Thickness thickness4 = thickness3 = thickness1;
      linkTab.Margin = thickness3;
      Thickness thickness5 = thickness4;
      mediaTab.Margin = thickness5;
      if (this.jids != null && ((IEnumerable<string>) this.jids).Any<string>())
      {
        this.MediaView.Load(this.jids);
        this.Dispatcher.RunAfterDelay(TimeSpan.FromMilliseconds(1500.0), (Action) (() =>
        {
          this.LinksView.Load(this.jids);
          this.DocsView.Load(this.jids);
        }));
      }
      this.Loaded += new RoutedEventHandler(this.OnLoaded);
    }

    public static void Start(string title, string[] jids)
    {
      MediaGalleryPage.nextInstanceJids = jids;
      MediaGalleryPage.nextInstanceTitle = title;
      NavUtils.NavigateToPage(nameof (MediaGalleryPage));
    }

    protected override void OnBackKeyPress(CancelEventArgs e)
    {
      e.Cancel = true;
      base.OnBackKeyPress(e);
      Storyboarder.Perform(WaAnimations.PageTransition(PageTransitionAnimation.SlideDownFadeOut), (DependencyObject) this.LayoutRoot, false, (Action) (() => NavUtils.GoBack(this.NavigationService)));
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);
      if (this.jids == null || !((IEnumerable<string>) this.jids).Any<string>())
        this.Dispatcher.BeginInvoke((Action) (() => NavUtils.GoBack()));
      else
        Storyboarder.Perform(WaAnimations.PageTransition(PageTransitionAnimation.SlideUpFadeIn), (DependencyObject) this.LayoutRoot);
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      base.OnRemovedFromJournal(e);
      this.MediaView.Dispose();
      this.LinksView.Dispose();
    }

    private void OnLoaded(object sender, EventArgs e)
    {
      this.MediaView.Show();
      this.DocsView.Show();
    }

    private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (!e.AddedItems.Contains((object) this.LinkTab))
        return;
      this.LinksView.Show();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/MediaGalleryPage.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.PageTitle = (PageTitlePanel) this.FindName("PageTitle");
      this.Pivot = (Pivot) this.FindName("Pivot");
      this.MediaTab = (PivotItem) this.FindName("MediaTab");
      this.MediaView = (MediaTabView) this.FindName("MediaView");
      this.LinkTab = (PivotItem) this.FindName("LinkTab");
      this.LinksView = (LinksTabView) this.FindName("LinksView");
      this.DocsTab = (PivotItem) this.FindName("DocsTab");
      this.DocsView = (DocsTabView) this.FindName("DocsView");
    }
  }
}
