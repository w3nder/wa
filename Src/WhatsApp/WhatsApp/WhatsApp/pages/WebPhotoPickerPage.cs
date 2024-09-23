// Decompiled with JetBrains decompiler
// Type: WhatsApp.WebPhotoPickerPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;


namespace WhatsApp
{
  public class WebPhotoPickerPage : PhoneApplicationPage
  {
    private static Dictionary<int, WebPhotoPickerPage.InstanceArgs> state = new Dictionary<int, WebPhotoPickerPage.InstanceArgs>();
    private static TicketCounter count = new TicketCounter();
    private static object stateLock = new object();
    private IDisposable subscription;
    private List<IDisposable> subSubscriptions = new List<IDisposable>();
    private IObserver<WebPhotoPickerPage.WebPhotoPickerArgs> observer;
    private GlobalProgressIndicator progress;
    private string size;
    private string aspect;
    private int minimumSize;
    internal Grid LayoutRoot;
    internal PageTitlePanel TitlePanel;
    internal TextBox SearchTextBox;
    internal ScrollViewer ResultsScrollViewer;
    internal WrapPanel ResultsPanel;
    internal Image BingLogo;
    private bool _contentLoaded;

    public WebPhotoPickerPage()
    {
      this.InitializeComponent();
      this.progress = new GlobalProgressIndicator((DependencyObject) this);
      this.BingLogo.Source = (System.Windows.Media.ImageSource) AssetStore.PoweredByBingIcon;
      this.TitlePanel.SmallTitle = AppResources.SearchTermHeader;
      this.GetLoadedAsync().Take<Unit>(1).Subscribe<Unit>((Action<Unit>) (_ => this.SearchTextBox.Focus()));
    }

    private void Search()
    {
      string query = (this.SearchTextBox.Text ?? "").Trim();
      if (query.Length == 0)
      {
        int num1 = (int) MessageBox.Show(AppResources.EnterSearchTerm);
      }
      else
      {
        this.DisposeAll();
        this.ResultsPanel.Children.Clear();
        this.progress.Acquire();
        this.subscription = WebServices.Instance.ImageSearch(query, this.size, this.aspect, this.minimumSize).ObserveOnDispatcher<ImageSearchResult>().Subscribe<ImageSearchResult>((Action<ImageSearchResult>) (result =>
        {
          this.subSubscriptions.Add(result.Thumbnail.ObserveOnDispatcher<WriteableBitmap>().Subscribe<WriteableBitmap>((Action<WriteableBitmap>) (thumb =>
          {
            Image image = new Image()
            {
              Source = (System.Windows.Media.ImageSource) thumb,
              Tag = (object) result
            };
            image.Height = image.Width = 138.0;
            image.Stretch = Stretch.UniformToFill;
            image.Margin = new Thickness(4.0);
            image.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.img_Tap);
            this.ResultsPanel.Children.Add((UIElement) image);
          })));
          this.BingLogo.Visibility = Visibility.Visible;
          this.ResultsScrollViewer.Focus();
        }), (Action<Exception>) (ex =>
        {
          Log.l(ex, "Exception from web search");
          this.progress.Release();
          int num2 = (int) MessageBox.Show(AppResources.WebSearchFailure);
        }), (Action) (() => this.progress.Release()));
      }
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      bool flag = false;
      if (this.observer == null)
      {
        string s = (string) null;
        int result;
        if (!this.NavigationContext.QueryString.TryGetValue("id", out s) || !int.TryParse(s, out result))
          throw new InvalidOperationException("Could not determine ID");
        WebPhotoPickerPage.InstanceArgs instanceArgs = (WebPhotoPickerPage.InstanceArgs) null;
        lock (WebPhotoPickerPage.stateLock)
        {
          if (WebPhotoPickerPage.state.TryGetValue(result, out instanceArgs))
            WebPhotoPickerPage.state.Remove(result);
        }
        if (instanceArgs == null)
        {
          flag = true;
        }
        else
        {
          this.observer = instanceArgs.Observer;
          TextBox searchTextBox = this.SearchTextBox;
          string rawInput = instanceArgs.InitialSearchTerm ?? "";
          string textOnly;
          string str = textOnly = Emoji.ConvertToTextOnly(rawInput, (byte[]) null);
          searchTextBox.Text = textOnly;
          if (!string.IsNullOrEmpty(str))
            this.SearchTextBox.Select(this.SearchTextBox.Text.Length, 0);
          this.size = instanceArgs.Size;
          this.aspect = instanceArgs.Aspect;
          this.minimumSize = instanceArgs.MinimumSize;
        }
      }
      base.OnNavigatedTo(e);
      if (!flag)
        return;
      this.Dispatcher.BeginInvoke((Action) (() => NavUtils.GoBack(this.NavigationService)));
    }

    private void DisposeAll()
    {
      if (this.subscription != null)
        this.subscription.Dispose();
      foreach (IDisposable subSubscription in this.subSubscriptions)
        subSubscription.Dispose();
      this.subSubscriptions.Clear();
    }

    private void SearchTextBox_GotFocus(object sender, EventArgs e)
    {
      this.SearchTextBox.SelectAll();
    }

    private void img_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (!(sender is Image image))
        return;
      ImageSearchResult res = image.Tag as ImageSearchResult;
      if (res == null)
        return;
      Action release = (Action) (() => { });
      Action onComplete = (Action) (() => this.Dispatcher.BeginInvokeIfNeeded(release));
      this.progress.Acquire();
      this.IsEnabled = false;
      release = (Action) (() =>
      {
        this.NavigationService.RemoveBackEntry();
        this.IsEnabled = true;
        this.progress.Release();
      });
      res.Contents.Subscribe<WriteableBitmap>((Action<WriteableBitmap>) (bitmap =>
      {
        WebPhotoPickerPage.WebPhotoPickerArgs webPhotoPickerArgs = new WebPhotoPickerPage.WebPhotoPickerArgs();
        webPhotoPickerArgs.Bitmap = bitmap;
        webPhotoPickerArgs.SourcePageUrl = res.SourcePageUrl;
        webPhotoPickerArgs.OnComplete = onComplete;
        try
        {
          this.observer.OnNext(webPhotoPickerArgs);
        }
        catch (Exception ex)
        {
          try
          {
            this.observer.OnError(ex);
          }
          finally
          {
            onComplete();
          }
        }
        this.progress.Release();
        this.IsEnabled = true;
      }), (Action<Exception>) (ex =>
      {
        this.progress.Release();
        int num = (int) MessageBox.Show(AppResources.SetPictureFromWebDownloadFailure);
        this.IsEnabled = true;
      }));
    }

    public static IObservable<WebPhotoPickerPage.WebPhotoPickerArgs> Start(
      NavigationService nav,
      string initialSearchTerm,
      string size,
      string aspect,
      int minimumSize)
    {
      return Observable.Create<WebPhotoPickerPage.WebPhotoPickerArgs>((Func<IObserver<WebPhotoPickerPage.WebPhotoPickerArgs>, Action>) (observer =>
      {
        int? nullable = new int?();
        try
        {
          nullable = new int?(WebPhotoPickerPage.count.NextTicket());
          lock (WebPhotoPickerPage.stateLock)
            WebPhotoPickerPage.state[nullable.Value] = new WebPhotoPickerPage.InstanceArgs()
            {
              Observer = observer,
              InitialSearchTerm = initialSearchTerm,
              Size = size,
              Aspect = aspect,
              MinimumSize = minimumSize
            };
          Uri uri = UriUtils.CreatePageUri(nameof (WebPhotoPickerPage), string.Format("id={0}", (object) nullable.Value));
          Deployment.Current.Dispatcher.BeginInvoke((Action) (() => nav.Navigate(uri)));
        }
        catch (Exception ex)
        {
          if (nullable.HasValue)
          {
            lock (WebPhotoPickerPage.stateLock)
              WebPhotoPickerPage.state.Remove(nullable.Value);
          }
          observer.OnError(ex);
        }
        return (Action) (() => { });
      }));
    }

    private void SearchTextBox_KeyUp(object sender, KeyEventArgs e)
    {
      if (e == null || e.Key != System.Windows.Input.Key.Enter)
        return;
      this.Search();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/WebPhotoPickerPage.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.TitlePanel = (PageTitlePanel) this.FindName("TitlePanel");
      this.SearchTextBox = (TextBox) this.FindName("SearchTextBox");
      this.ResultsScrollViewer = (ScrollViewer) this.FindName("ResultsScrollViewer");
      this.ResultsPanel = (WrapPanel) this.FindName("ResultsPanel");
      this.BingLogo = (Image) this.FindName("BingLogo");
    }

    private class InstanceArgs
    {
      public string InitialSearchTerm;
      public string Size;
      public string Aspect;
      public int MinimumSize;
      public IObserver<WebPhotoPickerPage.WebPhotoPickerArgs> Observer;
    }

    public struct WebPhotoPickerArgs
    {
      public WriteableBitmap Bitmap;
      public Action OnComplete;
      public string SourcePageUrl;
    }
  }
}
