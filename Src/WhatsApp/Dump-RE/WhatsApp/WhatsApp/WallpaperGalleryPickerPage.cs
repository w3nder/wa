// Decompiled with JetBrains decompiler
// Type: WhatsApp.WallpaperGalleryPickerPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Shell;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

#nullable disable
namespace WhatsApp
{
  public class WallpaperGalleryPickerPage : PhoneApplicationPage
  {
    private GlobalProgressIndicator progressIndicator;
    private List<WallpaperGalleryPickerPage.Item> items;
    private bool inAnimation_;
    private bool acceptClicked_;
    private int selected;
    private WallpaperPreviewControl[] previews = new WallpaperPreviewControl[3];
    private bool previewUp_;
    private static IObserver<string> Observer;
    internal Grid LayoutRoot;
    internal PageTitlePanel TitlePanel;
    internal ListBox ItemsPanel;
    internal PivotWithPreview SliderView;
    private bool _contentLoaded;

    public WallpaperGalleryPickerPage()
    {
      this.InitializeComponent();
      SysTrayHelper.SetForegroundColor((DependencyObject) this, Constants.SysTrayOffWhite);
      this.progressIndicator = new GlobalProgressIndicator((DependencyObject) this);
      ApplicationBarIconButton applicationBarIconButton = new ApplicationBarIconButton();
      applicationBarIconButton.IconUri = new Uri("/Images/check.png", UriKind.RelativeOrAbsolute);
      applicationBarIconButton.Text = AppResources.Accept;
      applicationBarIconButton.Click += new EventHandler(this.OnAcceptClick);
      this.ApplicationBar.Buttons.Add((object) applicationBarIconButton);
      this.ApplicationBar.IsVisible = false;
      this.Loaded += new RoutedEventHandler(this.OnLoaded);
      this.InitList();
    }

    private void InitList()
    {
      double zoomMultiplier = ResolutionHelper.ZoomMultiplier;
      this.ItemsPanel.Margin = new Thickness(24.0 * zoomMultiplier, 24.0 * zoomMultiplier, 24.0 * zoomMultiplier, 0.0);
      string[] source = new string[58]
      {
        "3.jpg #dn",
        "4.jpg #ln",
        "5.jpg",
        "8.jpg #dn",
        "9.jpg #ln",
        "10.jpg",
        "12.jpg",
        "13.jpg",
        "20.jpg #ln",
        "21.jpg #dn",
        "22.jpg",
        "23.jpg #ln",
        "24.jpg",
        "34.png",
        "39.jpg #dn",
        "40.jpg #ln",
        "41.jpg",
        "#fff7e9a8 #d",
        "#ffd0deb1 #d",
        "#ffaed8c7 #d",
        "#ffa9dbd8 #d",
        "#ffc7e9eb #d",
        "#ffcbdaec #d",
        "#ffd7d3eb #d",
        "#ffd9dade #d",
        "#fff2dad5 #d",
        "#fff2d5e1 #d",
        "#ffffa7a8 #d",
        "#ffffd1a4 #d",
        "#ffff981a #d",
        "#ffeda200 #d",
        "#ffcca029 #d",
        "#ffe6e365 #d",
        "#ffb2cc3d #d",
        "#ff73c780 #d",
        "#ff68d5d9 #d",
        "#ffca5cdb #e",
        "#ffe55cb1 #e",
        "#fff25596 #e",
        "#ffff5978 #e",
        "#ffff737e #e",
        "#fffa6e64 #e",
        "#ffff824c #e",
        "#ffa3a600 #e",
        "#ff81b224 #e",
        "#ff37b851 #e",
        "#ff00c288 #e",
        "#ff00c4aa #d",
        "#ff00becc #e",
        "#ff243640 #l",
        "#ff55626f #l",
        "#ff74676a #l",
        "#ff48324d #l",
        "#ff773380 #l",
        "#ff7a77d9 #l",
        "#ff245ab2 #l",
        "#ff0094d9 #l",
        "#ff3179f5 #l"
      };
      int i = 0;
      this.items = ((IEnumerable<string>) source).Select<string, WallpaperGalleryPickerPage.Item>((Func<string, WallpaperGalleryPickerPage.Item>) (v => new WallpaperGalleryPickerPage.Item(v, i++))).ToList<WallpaperGalleryPickerPage.Item>();
      this.ItemsPanel.ItemsSource = (IEnumerable) this.items;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
      this.previews = new WallpaperPreviewControl[3]
      {
        new WallpaperPreviewControl(),
        new WallpaperPreviewControl(),
        new WallpaperPreviewControl()
      };
      this.SliderView.SelectionChanged += new EventHandler<SelectionChangedEventArgs>(this.SliderView_SelectionChanged);
      this.SliderView.VisualRequest += new EventHandler<VisualRequestEventArgs>(this.SliderView_VisualRequest);
      this.SliderView.Count = this.items.Count;
      this.SliderView.Index = this.items.Count > 2 ? this.items.Count - 2 : 0;
      this.TitlePanel.Margin = new Thickness(0.0, UIUtils.SystemTraySizePortrait, 0.0, 0.0);
      this.TitlePanel.SmallTitle = AppResources.WallpaperGallery;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e) => base.OnNavigatedTo(e);

    private void SliderView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (!this.previewUp_)
        return;
      this.selected = (int) e.AddedItems[0];
      this.previews[this.selected % 3].Set(this.items.ElementAt<WallpaperGalleryPickerPage.Item>(this.selected).GetWallpaper());
      this.ItemsPanel.ScrollIntoView(this.ItemsPanel.Items.ElementAt<object>(this.selected));
    }

    private void SliderView_VisualRequest(object sender, VisualRequestEventArgs e)
    {
      if (this.previewUp_)
      {
        WallpaperGalleryPickerPage.Item obj = this.items.ElementAtOrDefault<WallpaperGalleryPickerPage.Item>(e.Index);
        if (obj != null)
          this.previews[e.Index % 3].Set(obj.GetWallpaper(), obj.ThumbnailSource);
      }
      e.Visual = (FrameworkElement) this.previews[e.Index % 3];
    }

    public void LoadOtherWallpapers()
    {
      int index1 = this.selected - 1;
      if (index1 >= 0)
      {
        WallpaperGalleryPickerPage.Item obj = this.items.ElementAtOrDefault<WallpaperGalleryPickerPage.Item>(index1);
        if (obj != null)
          this.previews[index1 % 3].Set(obj.GetWallpaper(), obj.ThumbnailSource);
      }
      int index2 = this.selected + 1;
      if (index2 >= this.items.Count)
        return;
      WallpaperGalleryPickerPage.Item obj1 = this.items.ElementAtOrDefault<WallpaperGalleryPickerPage.Item>(index2);
      if (obj1 == null)
        return;
      this.previews[index2 % 3].Set(obj1.GetWallpaper(), obj1.ThumbnailSource);
    }

    protected override void OnBackKeyPress(CancelEventArgs e)
    {
      if (this.previewUp_)
      {
        this.inAnimation_ = true;
        this.previewUp_ = false;
        this.TitlePanel.Opacity = 1.0;
        this.ItemsPanel.Opacity = 1.0;
        this.SliderView.IsHitTestVisible = false;
        this.selected = this.SliderView.Index;
        FrameworkElement frameworkElement = (FrameworkElement) (this.ItemsPanel.ItemContainerGenerator.ContainerFromIndex(this.selected) as ListBoxItem);
        this.previews[this.selected % 3].AnimateOut(frameworkElement.TransformToVisual((UIElement) this.LayoutRoot).Transform(new System.Windows.Point(0.0, 0.0)), frameworkElement.RenderSize, (Action) (() =>
        {
          this.inAnimation_ = false;
          this.SliderView.Opacity = 0.0;
        }));
        this.ApplicationBar.IsVisible = false;
        e.Cancel = true;
      }
      else
        base.OnBackKeyPress(e);
    }

    private void Item_Tap(object sender, EventArgs e)
    {
      FrameworkElement frameworkElement = sender as FrameworkElement;
      if (frameworkElement == null || !(frameworkElement.Tag is WallpaperGalleryPickerPage.Item tag) || this.inAnimation_)
        return;
      this.selected = tag.Index;
      this.SliderView.Opacity = 1.0;
      this.SliderView.Index = this.selected;
      this.ApplicationBar.IsVisible = true;
      this.inAnimation_ = true;
      this.previewUp_ = true;
      WallpaperStore.WallpaperState wallpaper = tag.GetWallpaper();
      this.previews[this.selected % 3].Set(wallpaper, tag.ThumbnailSource);
      this.previews[this.selected % 3].AnimateIn(frameworkElement.TransformToVisual((UIElement) this.LayoutRoot).Transform(new System.Windows.Point(0.0, 0.0)), frameworkElement.RenderSize, (Action) (() =>
      {
        if (!this.previewUp_)
          return;
        this.SliderView.IsHitTestVisible = true;
        this.inAnimation_ = false;
        this.TitlePanel.Opacity = 0.0;
        this.ItemsPanel.Opacity = 0.0;
        this.LoadOtherWallpapers();
      }));
      this.Dispatcher.BeginInvoke((Action) (() => this.previews[this.selected % 3].Set(wallpaper)));
    }

    public static IObservable<string> Start()
    {
      return Observable.Create<string>((Func<IObserver<string>, Action>) (observer =>
      {
        WallpaperGalleryPickerPage.Observer = observer;
        NavUtils.NavigateToPage(nameof (WallpaperGalleryPickerPage));
        return (Action) (() => { });
      }));
    }

    private void OnAcceptClick(object sender, EventArgs e)
    {
      if (this.acceptClicked_)
        return;
      this.acceptClicked_ = true;
      this.selected = this.SliderView.Index;
      string str = this.items.ElementAt<WallpaperGalleryPickerPage.Item>(this.selected).GetWallpaper().Get();
      WallpaperGalleryPickerPage.Observer.OnNext(str);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/WallpaperGalleryPickerPage.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.TitlePanel = (PageTitlePanel) this.FindName("TitlePanel");
      this.ItemsPanel = (ListBox) this.FindName("ItemsPanel");
      this.SliderView = (PivotWithPreview) this.FindName("SliderView");
    }

    public class Item
    {
      private string val;
      private System.Windows.Media.ImageSource thumb;
      private int index = -1;

      public Brush BackgroundBrush
      {
        get
        {
          Brush brush = (Brush) null;
          Color? color = WallpaperStore.TryParseColor(this.val);
          if (color.HasValue)
            brush = (Brush) new SolidColorBrush(color.Value);
          return brush ?? (Brush) UIUtils.TransparentBrush;
        }
      }

      public System.Windows.Media.ImageSource ThumbnailSource
      {
        get
        {
          if (this.thumb != null)
            return this.thumb;
          if (WallpaperStore.TryParseColor(this.val).HasValue)
            return (System.Windows.Media.ImageSource) null;
          WhatsApp.RegularExpressions.Match match = WallpaperStore.WallpaperWithFgPrefRegex.Match(this.val);
          using (Stream streamSource = App.OpenFromXAP("Images\\Wallpapers\\Thumbnails\\" + (match.Success ? match.Groups[1].Value : this.val)))
          {
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.SetSource(streamSource);
            return this.thumb = (System.Windows.Media.ImageSource) bitmapImage;
          }
        }
      }

      public int Index => this.index;

      public Item(string itemVal, int i)
      {
        this.val = itemVal;
        this.index = i;
      }

      public WallpaperStore.WallpaperState GetWallpaper()
      {
        return WallpaperStore.TryParseColor(this.val).HasValue ? new WallpaperStore.WallpaperState(this.val) : new WallpaperStore.WallpaperState(string.Format("{0}/{1}", (object) "Images/Wallpapers", (object) this.val));
      }
    }
  }
}
