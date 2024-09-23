// Decompiled with JetBrains decompiler
// Type: WhatsApp.GifPicker
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WhatsApp.Events;
using WhatsApp.WaViewModels;

#nullable disable
namespace WhatsApp
{
  public class GifPicker : UserControl
  {
    private string LogHeader = "GifSearch";
    private IDisposable delaySub;
    private ObservableCollection<GifPicker.GifItem> CurrentGifs = new ObservableCollection<GifPicker.GifItem>();
    private GifSendingPage SendingPage;
    private int CurrentGifKey;
    private int CurrentAddedGifs;
    private long currentSearchStartTimeTicks;
    private string currentSearchTerm;
    private IDisposable currentSearchDisp;
    internal Grid LayoutRoot;
    internal Rectangle HorizontalBar;
    internal WhatsApp.CompatibilityShims.LongListSelector GifSearchResultsList;
    internal TextBlock ResultsTooltip;
    internal Grid LoadingPreviewPage;
    internal ProgressBar LoadingProgressBar;
    internal Grid LoadingGif;
    private bool _contentLoaded;

    public string CurrentSearchTerm => this.currentSearchTerm;

    public GifPicker(GifSendingPage sendingpage)
    {
      this.InitializeComponent();
      new GifSearchSessionStarted()
      {
        gifSearchProvider = new wam_enum_gif_search_provider?(GifProviders.Instance.GetProviderForFieldStats())
      }.SaveEventSampled(10U);
      this.ResultsTooltip.Text = AppResources.NoResults;
      this.GifSearchResultsList.OverlapScrollBar = true;
      this.SendingPage = sendingpage;
      this.CurrentGifKey = 0;
      this.GifSearchResultsList.ItemsSource = (IList) this.CurrentGifs;
      this.Search("");
      this.GifSearchResultsList.ItemRealized += new EventHandler<ItemRealizationEventArgs>(this.GifSearchResultsList_ItemRealized);
      if (!Settings.IsWaAdmin)
        return;
      this.HorizontalBar.Visibility = Visibility.Visible;
    }

    private void GifSearchResultsList_ItemRealized(object sender, ItemRealizationEventArgs e)
    {
      if (!(e.Container.Content is GifPicker.GifItem content) || this.CurrentGifs == null || this.CurrentAddedGifs - 1 != content.key)
        return;
      long checkSearchTimeTicks = this.currentSearchStartTimeTicks;
      this.Dispatcher.RunAfterDelay(TimeSpan.FromMilliseconds(500.0), (Action) (() =>
      {
        if (checkSearchTimeTicks != this.currentSearchStartTimeTicks)
          return;
        this.currentSearchDisp.SafeDispose();
        this.currentSearchDisp = GifProviders.Instance.LoadAdditionalGifs().Subscribe<GifSearchResult>((Action<GifSearchResult>) (gifresult =>
        {
          if (checkSearchTimeTicks != this.currentSearchStartTimeTicks)
            return;
          this.Dispatcher.BeginInvoke((Action) (() =>
          {
            if (checkSearchTimeTicks != this.currentSearchStartTimeTicks)
              return;
            this.CurrentGifs.Add(new GifPicker.GifItem(gifresult, this.CurrentGifKey++));
          }));
        }), (Action<Exception>) (ex => Log.l(nameof (GifPicker), "Exception procexsing additional gif request: {0}", (object) ex.ToString())));
        this.CurrentAddedGifs += 12;
      }));
    }

    public void Search(string searchTerm)
    {
      if (this.currentSearchTerm == searchTerm)
        return;
      this.ResultsTooltip.Visibility = Visibility.Collapsed;
      this.LoadingGif.Visibility = Visibility.Visible;
      this.currentSearchTerm = searchTerm;
      this.currentSearchStartTimeTicks = DateTime.Now.Ticks;
      this.currentSearchDisp.SafeDispose();
      IObservable<GifSearchResult> source;
      if (string.IsNullOrEmpty(this.currentSearchTerm))
      {
        source = GifProviders.Instance.TrendingGifs();
        new GifTrendingViewed()
        {
          gifSearchProvider = new wam_enum_gif_search_provider?(GifProviders.Instance.GetProviderForFieldStats())
        }.SaveEventSampled(10U);
      }
      else
      {
        source = GifProviders.Instance.GifSearch(this.currentSearchTerm);
        new GifSearchPerformed()
        {
          gifSearchProvider = new wam_enum_gif_search_provider?(GifProviders.Instance.GetProviderForFieldStats())
        }.SaveEventSampled(10U);
      }
      this.CurrentGifs.Clear();
      this.CurrentAddedGifs = 12;
      this.CurrentGifKey = 0;
      this.currentSearchDisp = source.Subscribe<GifSearchResult>((Action<GifSearchResult>) (gifresult => this.Dispatcher.BeginInvoke((Action) (() =>
      {
        this.LoadingGif.Visibility = Visibility.Collapsed;
        this.CurrentGifs.Add(new GifPicker.GifItem(gifresult, this.CurrentGifKey++));
        new GifThumbnailFetched()
        {
          gifSearchProvider = new wam_enum_gif_search_provider?(GifProviders.Instance.GetProviderForFieldStats())
        }.SaveEventSampled(10U);
      }))), (Action<Exception>) (ex =>
      {
        Log.l(nameof (GifPicker), "Exception procexsing gif request: {0}", (object) ex.ToString());
        this.Dispatcher.BeginInvoke((Action) (() =>
        {
          this.LoadingGif.Visibility = Visibility.Collapsed;
          this.ResultsTooltip.Visibility = this.CurrentGifs.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
          if (this.CurrentGifs.Count != 0)
            return;
          new GifSearchNoResults()
          {
            gifSearchProvider = new wam_enum_gif_search_provider?(GifProviders.Instance.GetProviderForFieldStats())
          }.SaveEvent();
        }));
      }), (Action) (() => this.Dispatcher.BeginInvoke((Action) (() =>
      {
        this.LoadingGif.Visibility = Visibility.Collapsed;
        this.ResultsTooltip.Visibility = this.CurrentGifs.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        if (this.CurrentGifs.Count != 0)
          return;
        new GifSearchNoResults()
        {
          gifSearchProvider = new wam_enum_gif_search_provider?(GifProviders.Instance.GetProviderForFieldStats())
        }.SaveEvent();
      }))));
    }

    private void GifItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      GifPicker.GifItem gifItem = (GifPicker.GifItem) ((FrameworkElement) sender).Tag;
      this.LayoutRoot.IsHitTestVisible = false;
      this.LoadingPreviewPage.Visibility = Visibility.Visible;
      new GifSearchResultTapped()
      {
        gifSearchProvider = new wam_enum_gif_search_provider?(GifProviders.Instance.GetProviderForFieldStats())
      }.SaveEventSampled(10U);
      try
      {
        new Uri(gifItem.Item.Mp4Path, UriKind.Absolute).ToGetRequest().GetResponseBytesAync().Subscribe<byte[]>((Action<byte[]>) (bytes =>
        {
          if (bytes.Length != 0)
          {
            this.SendingPage.LaunchGifPreview(gifItem.Item, this, bytes);
          }
          else
          {
            Log.l(this.LogHeader, "No data downloading Gif {0}", (object) gifItem.Item.Mp4Path);
            this.ErrorDownloadingGif();
          }
        }), (Action<Exception>) (ex =>
        {
          Log.LogException(ex, "Exception downloading gif");
          this.ErrorDownloadingGif();
        }), (Action) (() => this.Dispatcher.BeginInvoke((Action) (() =>
        {
          this.LayoutRoot.IsHitTestVisible = true;
          this.LoadingPreviewPage.Visibility = Visibility.Collapsed;
        }))));
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "Exception creating gif download");
        this.ErrorDownloadingGif();
      }
    }

    private void ErrorDownloadingGif()
    {
      this.Dispatcher.BeginInvokeIfNeeded((Action) (() =>
      {
        this.LayoutRoot.IsHitTestVisible = true;
        this.LoadingPreviewPage.Visibility = Visibility.Collapsed;
        int num = (int) MessageBox.Show(AppResources.NetworkErrorTryAgain);
      }));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/GifPicker.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.HorizontalBar = (Rectangle) this.FindName("HorizontalBar");
      this.GifSearchResultsList = (WhatsApp.CompatibilityShims.LongListSelector) this.FindName("GifSearchResultsList");
      this.ResultsTooltip = (TextBlock) this.FindName("ResultsTooltip");
      this.LoadingPreviewPage = (Grid) this.FindName("LoadingPreviewPage");
      this.LoadingProgressBar = (ProgressBar) this.FindName("LoadingProgressBar");
      this.LoadingGif = (Grid) this.FindName("LoadingGif");
    }

    public class GifItem : WaViewModelBase
    {
      public int key;

      public BitmapSource Thumbnail
      {
        get
        {
          if (this.Item.bitmap == null)
            this.Item.bitmap = new BitmapImage(new Uri(this.Item.GifPreviewPath, UriKind.Absolute));
          return (BitmapSource) this.Item.bitmap;
        }
      }

      public GifSearchResult Item { get; set; }

      public GifItem(GifSearchResult item, int key)
      {
        this.Item = item;
        this.key = key;
      }
    }
  }
}
