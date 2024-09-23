// Decompiled with JetBrains decompiler
// Type: WhatsApp.WebSessionsPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Devices;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Shell;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Threading;
using WhatsApp.Events;
using WhatsApp.WaViewModels;

#nullable disable
namespace WhatsApp
{
  public class WebSessionsPage : PhoneApplicationPage
  {
    private static readonly string LogHdr = "WebSess";
    private bool qrRead;
    private DispatcherTimer qrLoginSuccessTimer;
    private DispatcherTimer qrLoginTimeoutTimer;
    private DispatcherTimer qrExpiredSessionTimer;
    private QrSession qrSession;
    private QrScannerUsage fsEvent;
    private long fsStartTimeTicks = -1;
    private IDisposable sessionsChangedSub;
    private bool backKeyToSession;
    internal Storyboard ShowScannerStoryboard;
    internal Storyboard ShowSessionsStoryboard;
    internal Grid LayoutRoot;
    internal PageTitlePanel PageTitle;
    internal WhatsApp.CompatibilityShims.LongListSelector SessionsList;
    internal CompositeTransform SessionsXForm;
    internal Image WebSessionLogo;
    internal Grid ScannerPanel;
    internal CompositeTransform ScannerXForm;
    internal QrScanner Scanner;
    internal ApplicationBarIconButton ScanQrButton;
    internal ApplicationBarIconButton LogoutButton;
    private bool _contentLoaded;

    private QrSession QrSession
    {
      get
      {
        if (this.qrSession == null && AppState.GetConnection() != null)
          this.qrSession = AppState.GetConnection().EventHandler.Qr.Session;
        return this.qrSession;
      }
    }

    public WebSessionsPage()
    {
      this.InitializeComponent();
      Localizable.LocalizeAppBar((PhoneApplicationPage) this);
      this.SessionsList.OverlapScrollBar = true;
      this.Scanner.QrScanned += new EventHandler<QrScanner.QrScannerEventArgs>(this.Scanner_QrScanned);
      this.BackKeyPress += new EventHandler<CancelEventArgs>(this.WebSessions_BackKeyPress);
      this.WebSessionLogo.Source = (System.Windows.Media.ImageSource) AssetStore.LoadAsset("active-connections.png");
    }

    private void WebSessionsPage_Loaded(object sender, RoutedEventArgs e)
    {
    }

    private void SetupThePurging()
    {
      DateTime? nullable = new DateTime?();
      if (this.QrSession != null)
      {
        this.QrSession.PurgeExpiredBrowsers();
        nullable = this.QrSession.GetFirstExpirationTime();
      }
      if (!nullable.HasValue)
        return;
      if (this.qrExpiredSessionTimer == null)
      {
        this.qrExpiredSessionTimer = new DispatcherTimer();
        this.qrExpiredSessionTimer.Tick += new EventHandler(this.QRExpiredSessionTimer_Tick);
      }
      this.qrExpiredSessionTimer.Interval = nullable.Value - DateTime.Now;
      this.qrExpiredSessionTimer.Start();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      if (this.QrSession != null && this.QrSession.HasConnections)
        this.ShowSessions(false);
      else
        this.ShowScanner(false);
      base.OnNavigatedTo(e);
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
      this.Scanner.QrScanned -= new EventHandler<QrScanner.QrScannerEventArgs>(this.Scanner_QrScanned);
      this.Scanner.Unload();
      if (this.fsEvent != null)
      {
        long? scanValidCount = this.fsEvent.scanValidCount;
        long num1 = 0;
        if ((scanValidCount.GetValueOrDefault() <= num1 ? (scanValidCount.HasValue ? 1 : 0) : 0) != 0)
        {
          long num2 = (DateTime.UtcNow.Ticks - this.fsStartTimeTicks) / 10000L;
          if (num2 > 0L)
            this.fsEvent.scanTimeT = new long?(num2);
        }
        this.fsEvent.SaveEvent();
        this.fsEvent = (QrScannerUsage) null;
      }
      base.OnNavigatedFrom(e);
    }

    private void Scanner_QrScanned(object sender, QrScanner.QrScannerEventArgs e)
    {
      Log.d(WebSessionsPage.LogHdr, nameof (Scanner_QrScanned));
      if (this.qrRead || this.QrSession == null)
        return;
      long? nullable;
      if (this.fsEvent != null)
      {
        QrScannerUsage fsEvent = this.fsEvent;
        nullable = fsEvent.scanReadCount;
        long num = 1;
        fsEvent.scanReadCount = nullable.HasValue ? new long?(nullable.GetValueOrDefault() + num) : new long?();
      }
      this.qrRead = true;
      VibrateController.Default.Start(TimeSpan.FromMilliseconds(200.0));
      if (this.QrSession.OnQrCodeScanned(e.Data, new Action(this.QrLoginSuccess), new Action(this.QrLoginFail)))
      {
        if (this.fsEvent != null)
        {
          QrScannerUsage fsEvent = this.fsEvent;
          nullable = fsEvent.scanValidCount;
          long num1 = 1;
          fsEvent.scanValidCount = nullable.HasValue ? new long?(nullable.GetValueOrDefault() + num1) : new long?();
          long num2 = (DateTime.UtcNow.Ticks - this.fsStartTimeTicks) / 10000L;
          if (num2 > 0L)
            this.fsEvent.scanTimeT = new long?(num2);
        }
        this.Scanner.Waiting = true;
        if (this.qrLoginTimeoutTimer == null)
        {
          this.qrLoginTimeoutTimer = new DispatcherTimer();
          this.qrLoginTimeoutTimer.Interval = TimeSpan.FromMilliseconds(30000.0);
          this.qrLoginTimeoutTimer.Tick += new EventHandler(this.QrLoginTimeoutTimer_Tick);
        }
        this.qrLoginTimeoutTimer.Start();
      }
      else
        this.QrScanFail();
    }

    private void QrLoginSuccess()
    {
      Log.d(WebSessionsPage.LogHdr, "QrLoginSuccess | {0}", (object) this.qrRead);
      if (!this.qrRead)
        return;
      this.qrLoginTimeoutTimer.Stop();
      this.qrRead = false;
      this.Scanner.Waiting = false;
      this.Scanner.Complete = true;
      this.Scanner.StopScan();
      if (this.qrLoginSuccessTimer == null)
      {
        this.qrLoginSuccessTimer = new DispatcherTimer();
        this.qrLoginSuccessTimer.Interval = TimeSpan.FromMilliseconds(750.0);
        this.qrLoginSuccessTimer.Tick += new EventHandler(this.QrLoginSuccessTimer_Tick);
      }
      this.qrLoginSuccessTimer.Start();
    }

    private void QrLoginFail()
    {
      Log.d(WebSessionsPage.LogHdr, "QrLoginFail | {0}", (object) this.qrRead);
      if (!this.qrRead)
        return;
      if (this.qrLoginTimeoutTimer != null)
        this.qrLoginTimeoutTimer.Stop();
      this.qrRead = false;
      this.Scanner.Waiting = false;
      int num = (int) MessageBox.Show(AppResources.WebTryAgain, AppResources.WebQrScanFail, MessageBoxButton.OK);
    }

    private void QrScanFail()
    {
      Log.d(WebSessionsPage.LogHdr, "QrScanFail | {0}", (object) this.qrRead);
      if (!this.qrRead)
        return;
      if (this.qrLoginTimeoutTimer != null)
        this.qrLoginTimeoutTimer.Stop();
      this.qrRead = false;
      this.Scanner.Waiting = false;
      int num = (int) MessageBox.Show(AppResources.WebBadQrCode, AppResources.WebBadQrCodeTitle, MessageBoxButton.OK);
    }

    private void QrLoginTimeoutTimer_Tick(object sender, EventArgs e)
    {
      if (!this.qrRead)
        return;
      if (this.qrLoginTimeoutTimer != null)
        this.qrLoginTimeoutTimer.Stop();
      this.qrRead = false;
      this.Scanner.Waiting = false;
      int num = (int) MessageBox.Show(AppResources.WebNetworkFailure, AppResources.WebQrScanFail, MessageBoxButton.OK);
    }

    private void QrLoginSuccessTimer_Tick(object sender, EventArgs e)
    {
      this.qrLoginSuccessTimer.Stop();
      this.ShowSessions(true);
    }

    private void QRExpiredSessionTimer_Tick(object sender, EventArgs e)
    {
      this.qrExpiredSessionTimer.Stop();
      this.SetupThePurging();
    }

    public void LogOutAction(object sender, EventArgs args)
    {
      UIUtils.MessageBox(AppResources.WebLogout, AppResources.WebLogoutAll, (IEnumerable<string>) new string[2]
      {
        AppResources.CancelButton,
        AppResources.WebLogout
      }, (Action<int>) (idx =>
      {
        if (idx != 1)
          return;
        this.QrSession.LogoutAll();
        this.NavigationService.GoBack();
      }));
    }

    public void ScanAction(object sender, EventArgs args)
    {
      this.backKeyToSession = true;
      this.ShowScanner(true);
    }

    private void ShowScanner(bool animation)
    {
      this.SupportedOrientations = SupportedPageOrientation.Portrait;
      this.ApplicationBar.IsVisible = false;
      this.PageTitle.Subtitle = (string) null;
      if (this.fsEvent == null)
      {
        this.fsEvent = new QrScannerUsage();
        this.fsEvent.scanType = new wam_enum_scan_type?(wam_enum_scan_type.WEB_CLIENT);
        this.fsEvent.scanReadCount = new long?(0L);
        this.fsEvent.scanValidCount = new long?(0L);
        this.fsStartTimeTicks = DateTime.UtcNow.Ticks;
      }
      try
      {
        this.Scanner.ScanAsync();
      }
      catch (Exception ex)
      {
        if (this.QrSession.HasConnections)
          this.ShowSessions(animation);
        else
          this.NavigationService.GoBack();
      }
      this.ScannerPanel.Visibility = Visibility.Visible;
      if (animation)
      {
        this.ScannerXForm.TranslateY = 800.0;
        this.SessionsXForm.TranslateY = 0.0;
        this.ShowSessionsStoryboard.Stop();
        this.Dispatcher.BeginInvoke((Action) (() => Storyboarder.Perform(this.ShowScannerStoryboard, onComplete: (Action) (() =>
        {
          this.SessionsXForm.TranslateY = 0.0;
          this.ScannerXForm.TranslateY = 0.0;
          this.SessionsList.Visibility = Visibility.Collapsed;
        }))));
      }
      else
        this.SessionsList.Visibility = Visibility.Collapsed;
    }

    private void ShowSessions(bool animation)
    {
      this.backKeyToSession = false;
      this.SupportedOrientations = SupportedPageOrientation.PortraitOrLandscape;
      this.SetupThePurging();
      this.Scanner.StopScan();
      ObservableCollection<QrSessionInfo> sessions = AppState.GetConnection().EventHandler.Qr.Session.SessionsInfo;
      this.sessionsChangedSub.SafeDispose();
      this.sessionsChangedSub = Observable.Return<Unit>(new Unit()).Concat<Unit>(sessions.GetCollectionChangedAsnyc().Select<NotifyCollectionChangedEventArgs, Unit>((Func<NotifyCollectionChangedEventArgs, Unit>) (_ => new Unit()))).ObserveOnDispatcher<Unit>().Subscribe<Unit>((Action<Unit>) (_ =>
      {
        this.SessionsList.ItemsSource = (IList) sessions.Where<QrSessionInfo>((Func<QrSessionInfo, bool>) (si => si != null)).Select<QrSessionInfo, WebSessionsPage.SessionInfoViewModel>((Func<QrSessionInfo, WebSessionsPage.SessionInfoViewModel>) (si => new WebSessionsPage.SessionInfoViewModel(si))).ToList<WebSessionsPage.SessionInfoViewModel>();
        this.SessionsList.Visibility = Visibility.Visible;
      }));
      if (animation)
      {
        this.ScannerXForm.TranslateY = 0.0;
        this.SessionsXForm.TranslateY = 800.0;
        this.ShowScannerStoryboard.Stop();
        this.Dispatcher.BeginInvoke((Action) (() => Storyboarder.Perform(this.ShowSessionsStoryboard, onComplete: (Action) (() =>
        {
          this.PageTitle.Subtitle = AppResources.WebLoggedInComputer;
          this.ApplicationBar.IsVisible = true;
          this.SessionsXForm.TranslateY = 0.0;
          this.ScannerXForm.TranslateY = 0.0;
          this.ScannerPanel.Visibility = Visibility.Collapsed;
        }))));
      }
      else
      {
        this.PageTitle.KeepOriginalSubtitleCase = true;
        this.PageTitle.Subtitle = AppResources.WebLoggedInComputer;
        this.ApplicationBar.IsVisible = true;
        this.ScannerPanel.Visibility = Visibility.Collapsed;
      }
    }

    private void WebSessions_BackKeyPress(object sender, CancelEventArgs e)
    {
      if (!this.backKeyToSession)
        return;
      this.ShowSessions(true);
      e.Cancel = true;
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/WebSessionsPage.xaml", UriKind.Relative));
      this.ShowScannerStoryboard = (Storyboard) this.FindName("ShowScannerStoryboard");
      this.ShowSessionsStoryboard = (Storyboard) this.FindName("ShowSessionsStoryboard");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.PageTitle = (PageTitlePanel) this.FindName("PageTitle");
      this.SessionsList = (WhatsApp.CompatibilityShims.LongListSelector) this.FindName("SessionsList");
      this.SessionsXForm = (CompositeTransform) this.FindName("SessionsXForm");
      this.WebSessionLogo = (Image) this.FindName("WebSessionLogo");
      this.ScannerPanel = (Grid) this.FindName("ScannerPanel");
      this.ScannerXForm = (CompositeTransform) this.FindName("ScannerXForm");
      this.Scanner = (QrScanner) this.FindName("Scanner");
      this.ScanQrButton = (ApplicationBarIconButton) this.FindName("ScanQrButton");
      this.LogoutButton = (ApplicationBarIconButton) this.FindName("LogoutButton");
    }

    public class SessionInfoViewModel : WaViewModelBase
    {
      private QrSessionInfo sessionInfo;

      public SessionInfoViewModel(QrSessionInfo session)
      {
        this.sessionInfo = session;
        if (this.sessionInfo == null)
          return;
        this.sessionInfo.PropertyChanged += new PropertyChangedEventHandler(this.sessionInfo_PropertyChanged);
      }

      private void sessionInfo_PropertyChanged(object sender, PropertyChangedEventArgs e)
      {
        if (e.PropertyName == "LastConnected")
        {
          this.NotifyPropertyChanged("TitleStr");
          this.NotifyPropertyChanged("TitleBrush");
        }
        else
        {
          if (!(e.PropertyName == "Location"))
            return;
          this.NotifyPropertyChanged("LocationStr");
          this.NotifyPropertyChanged("LocationVisibility");
        }
      }

      public System.Windows.Media.ImageSource BrowserIcon
      {
        get
        {
          System.Windows.Media.ImageSource browserIcon;
          switch (this.sessionInfo == null ? (string) null : this.sessionInfo.Browser)
          {
            case "Chrome":
              browserIcon = (System.Windows.Media.ImageSource) AssetStore.LoadAsset("active-connections-chrome.png", AssetStore.ThemeSetting.Dark);
              break;
            case "Firefox":
              browserIcon = (System.Windows.Media.ImageSource) AssetStore.LoadAsset("active-connections-firefox.png", AssetStore.ThemeSetting.Dark);
              break;
            case "IE":
              browserIcon = (System.Windows.Media.ImageSource) AssetStore.LoadAsset("active-connections-ie.png", AssetStore.ThemeSetting.Dark);
              break;
            case "Opera":
              browserIcon = (System.Windows.Media.ImageSource) AssetStore.LoadAsset("active-connections-opera.png", AssetStore.ThemeSetting.Dark);
              break;
            case "Safari":
              browserIcon = (System.Windows.Media.ImageSource) AssetStore.LoadAsset("active-connections-safari.png", AssetStore.ThemeSetting.Dark);
              break;
            case "Edge":
              browserIcon = (System.Windows.Media.ImageSource) AssetStore.LoadAsset("active-connections-edge.png", AssetStore.ThemeSetting.Dark);
              break;
            default:
              browserIcon = (System.Windows.Media.ImageSource) AssetStore.LoadAsset("active-connections-unknown.png", AssetStore.ThemeSetting.Dark);
              break;
          }
          return browserIcon;
        }
      }

      public string TitleStr
      {
        get
        {
          if (this.sessionInfo == null)
            return (string) null;
          DateTime? lastConnected = this.sessionInfo.LastConnected;
          if (!lastConnected.HasValue)
            return AppResources.WebCurrentlyActive;
          string webLastActive = AppResources.WebLastActive;
          lastConnected = this.sessionInfo.LastConnected;
          string str = DateTimeUtils.FormatLastSeen(lastConnected.Value);
          return string.Format(webLastActive, (object) str);
        }
      }

      public Brush TitleBrush
      {
        get
        {
          return this.sessionInfo == null || this.sessionInfo.LastConnected.HasValue ? UIUtils.SubtleBrush : (Brush) UIUtils.AccentBrush;
        }
      }

      public string LocationStr
      {
        get
        {
          return this.sessionInfo == null || this.sessionInfo.Location == null ? (string) null : string.Format(AppResources.WebStartedIn, (object) this.sessionInfo.Location);
        }
      }

      public Visibility LocationVisibility
      {
        get => (this.sessionInfo != null && this.sessionInfo.Location != null).ToVisibility();
      }

      public string OperatingSystemStr
      {
        get => this.sessionInfo != null ? this.sessionInfo.OperatingSystem : "";
      }
    }
  }
}
