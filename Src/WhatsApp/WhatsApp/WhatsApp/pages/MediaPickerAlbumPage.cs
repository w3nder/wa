// Decompiled with JetBrains decompiler
// Type: WhatsApp.MediaPickerAlbumPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;
using WhatsAppNative;


namespace WhatsApp
{
  public class MediaPickerAlbumPage : PhoneApplicationPage
  {
    private static MediaPickerState NextInstanceMediaPickerState;
    private static MediaPickerPage.Album NextInstanceAlbum;
    private static IObserver<MediaSharingArgs> NextInstanceObserver;
    private MediaPickerState pickerState_;
    private MediaPickerPage.Album album_;
    private IObserver<MediaSharingArgs> albumObserver_;
    private IDisposable scheduledAsyncLoading_;
    private IDisposable pickerStateItemsChangedSub_;
    private List<MediaMultiSelector.Item> mediaGridSource_;
    private Microsoft.Phone.Shell.ApplicationBar submitAppBar_;
    private Microsoft.Phone.Shell.ApplicationBar selectionAppBar_;
    private ApplicationBarIconButton submitButton_;
    private bool isPageLoaded_;
    private bool isMediaGridShown_;
    private DateTime? lastAppBarActionTime_;
    private DispatcherTimer warningTimer_;
    internal Grid LayoutRoot;
    internal PageTitlePanel TitlePanel;
    internal ProgressBar LoadingProgressBar;
    internal MediaMultiSelector MediaGrid;
    internal Grid WarningPanel;
    internal TextBlock WarningTextBlock;
    private bool _contentLoaded;

    private DispatcherTimer WarningTimer
    {
      get
      {
        if (this.warningTimer_ == null)
        {
          this.warningTimer_ = new DispatcherTimer()
          {
            Interval = TimeSpan.FromMilliseconds(2500.0)
          };
          this.warningTimer_.Tick += new EventHandler(this.WarningTimer_Tick);
        }
        return this.warningTimer_;
      }
    }

    public MediaPickerAlbumPage()
    {
      this.InitializeComponent();
      this.submitAppBar_ = this.Resources[(object) "SubmitAppBar"] as Microsoft.Phone.Shell.ApplicationBar;
      Localizable.LocalizeAppBar(this.submitAppBar_);
      this.submitButton_ = this.submitAppBar_.Buttons[0] as ApplicationBarIconButton;
      this.selectionAppBar_ = this.Resources[(object) "SelectionAppBar"] as Microsoft.Phone.Shell.ApplicationBar;
      Localizable.LocalizeAppBar(this.selectionAppBar_);
      this.MediaGrid.SingleItemSelected += new MediaMultiSelector.ItemSelectionChangedHandler(this.MediaGrid_SingleItemSelected);
      this.MediaGrid.ItemSelectionToggled += new MediaMultiSelector.ItemSelectionChangedHandler(this.MediaGrid_ItemSelectionToggled);
      this.MediaGrid.ItemSelectionBlocked += new MediaMultiSelector.ItemSelectionChangedHandler(this.MediaGrid_ItemSelectionBlocked);
      this.MediaGrid.IsWhiteScrollBar = true;
      this.album_ = MediaPickerAlbumPage.NextInstanceAlbum;
      this.pickerState_ = MediaPickerAlbumPage.NextInstanceMediaPickerState;
      this.albumObserver_ = MediaPickerAlbumPage.NextInstanceObserver;
      MediaPickerAlbumPage.NextInstanceAlbum = (MediaPickerPage.Album) null;
      MediaPickerAlbumPage.NextInstanceMediaPickerState = (MediaPickerState) null;
      MediaPickerAlbumPage.NextInstanceObserver = (IObserver<MediaSharingArgs>) null;
      if (this.pickerState_ != null && this.albumObserver_ != null && this.album_ != null)
      {
        this.TitlePanel.LargeTitle = this.album_.Header;
        this.pickerStateItemsChangedSub_ = this.pickerState_.SubscribeToSelectedItemsChange((Action<MediaSharingState.SelectedItemsChangeCause>) (_ => this.ProcessPickerStateSelectedItemsChanged()));
        this.LoadAlbumContentAsync();
      }
      this.WarningTextBlock.Text = AppState.IsLowMemoryDevice ? AppResources.ReachedMultiPickerLimit : AppResources.ReachedMultiPickerLimit30;
    }

    public static IObservable<MediaSharingArgs> Start(
      MediaPickerPage.Album album,
      MediaPickerState state)
    {
      return album != null ? Observable.Create<MediaSharingArgs>((Func<IObserver<MediaSharingArgs>, Action>) (observer =>
      {
        MediaPickerAlbumPage.NextInstanceAlbum = album;
        MediaPickerAlbumPage.NextInstanceMediaPickerState = state;
        MediaPickerAlbumPage.NextInstanceObserver = observer;
        NavUtils.NavigateToPage(nameof (MediaPickerAlbumPage));
        return (Action) (() => { });
      })) : (IObservable<MediaSharingArgs>) null;
    }

    private void LoadAlbumContentAsync()
    {
      this.LoadingProgressBar.Visibility = Visibility.Visible;
      WAThreadPool.QueueUserWorkItem((Action) (() =>
      {
        Log.l("album picker", "load album: {0}", (object) this.album_.Header);
        this.mediaGridSource_ = this.album_.GetItems().Cast<MediaMultiSelector.Item>().ToList<MediaMultiSelector.Item>();
        this.Dispatcher.BeginInvoke(new Action(this.ShowMediaGrid));
      }));
    }

    private void ShowMediaGrid()
    {
      if (this.isMediaGridShown_ || !this.isPageLoaded_ || this.mediaGridSource_ == null)
      {
        Log.l("album picker", "show media grid | skipped | already shown: {0}, page loaded: {1}, source null: {2}", (object) this.isMediaGridShown_, (object) this.isPageLoaded_, (object) (this.mediaGridSource_ == null));
      }
      else
      {
        Log.l("album picker", "show media grid | count: {0}", (object) this.mediaGridSource_.Count);
        if (this.mediaGridSource_.Count == 0)
        {
          this.MediaGrid.FlatItemsSource = (List<MediaMultiSelector.Item>) null;
          this.MediaGrid.FooterTextBlock.Text = this.pickerState_.Mode == MediaSharingState.SharingMode.ChooseVideo ? AppResources.NoVideosAvailable : AppResources.NoPicturesAvailable;
          this.MediaGrid.LoadingProgressBar.Visibility = Visibility.Collapsed;
        }
        else
        {
          this.MediaGrid.FlatItemsSource = this.mediaGridSource_;
          if (this.album_.MostRecentAsFirst)
            this.MediaGrid.ScrollToBottom();
        }
        this.isMediaGridShown_ = true;
        this.LoadingProgressBar.Visibility = Visibility.Collapsed;
      }
    }

    private void NotifyObserver()
    {
      this.albumObserver_.OnNext(new MediaSharingArgs((MediaSharingState) this.pickerState_, MediaSharingArgs.SharingStatus.None, this.NavigationService));
    }

    private void UpdateAppBar()
    {
      if (this.pickerState_.AllowMultiSelection)
      {
        if (this.MediaGrid.IsMultiSelectionEnabled)
        {
          this.ApplicationBar = (IApplicationBar) this.submitAppBar_;
          this.submitButton_.IsEnabled = this.pickerState_.SelectedCount > 0;
        }
        else
          this.ApplicationBar = (IApplicationBar) this.selectionAppBar_;
      }
      else
        this.ApplicationBar = (IApplicationBar) null;
    }

    private void RefreshPage()
    {
      int selectedCount = this.pickerState_.SelectedCount;
      bool flag = this.pickerState_.AllowMultiSelection && selectedCount > 0;
      this.TitlePanel.SmallTitle = MediaPickerPage.GetPickerPageTitleText(this.pickerState_);
      this.MediaGrid.IsMultiSelectionEnabled = flag;
      this.UpdateAppBar();
      this.MediaGrid.IsSelectionAddBlocked = flag && selectedCount >= MediaSharingState.MaxSelectedItem;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      if (this.pickerState_ == null || this.albumObserver_ == null || this.album_ == null)
      {
        Log.l("album picker", "missing instance args");
        this.Dispatcher.BeginInvoke((Action) (() => NavUtils.GoBack(this.NavigationService)));
      }
      else if (e.NavigationMode == NavigationMode.Reset)
      {
        Log.l("album picker", "fast resumed to album picker | abort");
        this.Dispatcher.BeginInvoke((Action) (() => NavUtils.NavigateBackToChat(this.NavigationService)));
      }
      else
      {
        this.RefreshPage();
        base.OnNavigatedTo(e);
      }
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      this.pickerStateItemsChangedSub_.SafeDispose();
      this.pickerStateItemsChangedSub_ = (IDisposable) null;
      this.scheduledAsyncLoading_.SafeDispose();
      if (this.albumObserver_ != null)
        this.albumObserver_.OnCompleted();
      base.OnRemovedFromJournal(e);
    }

    protected override void OnBackKeyPress(CancelEventArgs e) => base.OnBackKeyPress(e);

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
      this.isPageLoaded_ = true;
      this.ShowMediaGrid();
    }

    private void Done_Click(object sender, EventArgs e)
    {
      if (MediaPickerPage.TestAppBarActionBlocking(ref this.lastAppBarActionTime_))
        return;
      this.NotifyObserver();
    }

    private void Select_Click(object sender, EventArgs e)
    {
      if (MediaPickerPage.TestAppBarActionBlocking(ref this.lastAppBarActionTime_))
        return;
      this.ApplicationBar = (IApplicationBar) this.submitAppBar_;
      this.MediaGrid.IsMultiSelectionEnabled = true;
      this.UpdateAppBar();
      this.MediaGrid.IsSelectionAddBlocked = this.pickerState_.SelectedCount >= MediaSharingState.MaxSelectedItem;
    }

    private void ProcessPickerStateSelectedItemsChanged()
    {
      this.TitlePanel.SmallTitle = MediaPickerPage.GetPickerPageTitleText(this.pickerState_);
      int selectedCount = this.pickerState_.SelectedCount;
      this.submitButton_.IsEnabled = selectedCount > 0;
      if (selectedCount < MediaSharingState.MaxSelectedItem)
      {
        this.WarningPanel.Visibility = Visibility.Collapsed;
        this.MediaGrid.IsSelectionAddBlocked = false;
      }
      else
        this.MediaGrid.IsSelectionAddBlocked = true;
    }

    private void MediaGrid_SingleItemSelected(MediaMultiSelector.Item item)
    {
      if (this.pickerState_.SelectedCount > 0)
        this.pickerState_.DeleteItems((Func<MediaSharingState.IItem, bool>) null);
      if (!(item is MediaPickerState.Item obj))
      {
        Log.l("album picker", "null selected item");
      }
      else
      {
        try
        {
          using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
          {
            using (Stream stream = nativeMediaStorage.OpenFile(obj.GetFullPath(), FileMode.Open, FileAccess.Read))
            {
              if (stream.Length == 0L)
                throw new Exception("Length was 0");
            }
          }
        }
        catch (Exception ex1)
        {
          MediaItemTypes mediaItemTypes = MediaItemTypes.Unknown;
          try
          {
            mediaItemTypes = obj.MediaItem.GetType();
          }
          catch (Exception ex2)
          {
          }
          Log.l(ex1, string.Format("open media from album | type:{0}", (object) mediaItemTypes));
          string messageBoxText;
          switch (mediaItemTypes)
          {
            case MediaItemTypes.Video:
              messageBoxText = AppResources.CouldNotOpenVideo;
              break;
            case MediaItemTypes.Picture:
              messageBoxText = AppResources.CouldNotOpenPhoto;
              break;
            default:
              messageBoxText = AppResources.CouldNotOpenMediaFile;
              break;
          }
          int num = (int) MessageBox.Show(messageBoxText);
          return;
        }
        this.pickerState_.AddItem((MediaSharingState.IItem) obj);
        this.NotifyObserver();
      }
    }

    private void MediaGrid_ItemSelectionToggled(MediaMultiSelector.Item item)
    {
      this.pickerState_.ProcessItemSelectionChange(item as MediaPickerState.Item);
    }

    private void MediaGrid_ItemSelectionBlocked(MediaMultiSelector.Item item)
    {
      if (this.WarningPanel.Visibility == Visibility.Collapsed)
        Storyboarder.Perform(this.Resources, "WarningInSB");
      this.WarningPanel.Visibility = Visibility.Visible;
      if (this.WarningTimer.IsEnabled)
        this.WarningTimer.Stop();
      this.WarningTimer.Start();
    }

    private void WarningTimer_Tick(object sender, EventArgs e)
    {
      Storyboarder.Perform(this.Resources, "WarningOutSB", onComplete: (Action) (() => this.WarningPanel.Visibility = Visibility.Collapsed));
      if (!this.WarningTimer.IsEnabled)
        return;
      this.WarningTimer.Stop();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/MediaPickerAlbumPage.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.TitlePanel = (PageTitlePanel) this.FindName("TitlePanel");
      this.LoadingProgressBar = (ProgressBar) this.FindName("LoadingProgressBar");
      this.MediaGrid = (MediaMultiSelector) this.FindName("MediaGrid");
      this.WarningPanel = (Grid) this.FindName("WarningPanel");
      this.WarningTextBlock = (TextBlock) this.FindName("WarningTextBlock");
    }
  }
}
