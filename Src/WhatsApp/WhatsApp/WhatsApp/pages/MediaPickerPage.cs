// Decompiled with JetBrains decompiler
// Type: WhatsApp.MediaPickerPage
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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;
using WhatsApp.WaCollections;
using WhatsApp.WaViewModels;
using WhatsAppNative;


namespace WhatsApp
{
  public class MediaPickerPage : PhoneApplicationPage
  {
    public static MediaPickerState.Item preChosenChild;
    private static MediaPickerState NextInstanceMediaPickerState;
    private static IObserver<MediaSharingArgs> NextInstanceObserver;
    private MediaPickerState pickerState_;
    private IObserver<MediaSharingArgs> pickerObserver_;
    private Microsoft.Phone.Shell.ApplicationBar submitAppBar_;
    private Microsoft.Phone.Shell.ApplicationBar selectionAppBar_;
    private ApplicationBarIconButton submitButton_;
    private bool isPageLoaded_;
    private bool isAlbumsShown_;
    private bool isVideoGridShown_;
    private bool isVideoTabVisited_;
    private IEnumerable<MediaPickerState.Item> videos_;
    private bool isPictureGridShown_;
    private bool isPictureTabVisited_;
    private IEnumerable<MediaPickerState.Item> pics_;
    private object loadingLock_ = new object();
    private List<MediaPickerPage.Album> albumsSource_;
    private List<KeyedList<string, MediaMultiSelector.Item>> pictureGridSource_;
    private List<KeyedList<string, MediaMultiSelector.Item>> videoGridSource_;
    private IDisposable pickerStateItemsChangedSub_;
    private DateTime? lastAppBarActionTime_;
    private DispatcherTimer warningTimer_;
    private bool isPageRemoved_;
    internal PivotHeaderConverter PivotHeaderConverter;
    internal Grid LayoutRoot;
    internal ProgressBar LoadingProgressBar;
    internal PageTitlePanel TitlePanel;
    internal Pivot Pivot;
    internal PivotItem AlbumsPivotItem;
    internal ListBox AlbumList;
    internal PivotItem VideosPivotItem;
    internal MediaMultiSelector VideoGrid;
    internal Grid WarningPanelVideo;
    internal TextBlock WarningTextBlockVideo;
    internal PivotItem PicturesPivotItem;
    internal MediaMultiSelector PictureGrid;
    internal Grid WarningPanelPicture;
    internal TextBlock WarningTextBlockPicture;
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

    private bool IsDataLoaded
    {
      get
      {
        return this.albumsSource_ != null && this.pictureGridSource_ != null && this.videoGridSource_ != null;
      }
    }

    public MediaPickerPage()
    {
      this.InitializeComponent();
      this.submitAppBar_ = this.Resources[(object) "SubmitAppBar"] as Microsoft.Phone.Shell.ApplicationBar;
      Localizable.LocalizeAppBar(this.submitAppBar_);
      this.submitButton_ = this.submitAppBar_.Buttons[0] as ApplicationBarIconButton;
      this.selectionAppBar_ = this.Resources[(object) "SelectionAppBar"] as Microsoft.Phone.Shell.ApplicationBar;
      Localizable.LocalizeAppBar(this.selectionAppBar_);
      this.PictureGrid.SingleItemSelected += new MediaMultiSelector.ItemSelectionChangedHandler(this.MediaGrid_SingleItemSelected);
      this.PictureGrid.ItemSelectionToggled += new MediaMultiSelector.ItemSelectionChangedHandler(this.PictureGrid_ItemSelectionToggled);
      this.PictureGrid.ItemSelectionBlocked += new MediaMultiSelector.ItemSelectionChangedHandler(this.PictureGrid_ItemSelectionBlocked);
      this.PictureGrid.IsWhiteScrollBar = true;
      this.VideoGrid.SingleItemSelected += new MediaMultiSelector.ItemSelectionChangedHandler(this.MediaGrid_SingleItemSelected);
      this.VideoGrid.ItemSelectionToggled += new MediaMultiSelector.ItemSelectionChangedHandler(this.PictureGrid_ItemSelectionToggled);
      this.VideoGrid.ItemSelectionBlocked += new MediaMultiSelector.ItemSelectionChangedHandler(this.PictureGrid_ItemSelectionBlocked);
      this.VideoGrid.IsWhiteScrollBar = true;
      this.pickerState_ = MediaPickerPage.NextInstanceMediaPickerState;
      this.pickerObserver_ = MediaPickerPage.NextInstanceObserver;
      MediaPickerPage.NextInstanceMediaPickerState = (MediaPickerState) null;
      MediaPickerPage.NextInstanceObserver = (IObserver<MediaSharingArgs>) null;
      this.Pivot.Items.Remove((object) this.PicturesPivotItem);
      this.Pivot.Items.Remove((object) this.VideosPivotItem);
      if (this.pickerState_ != null && this.pickerObserver_ != null)
      {
        this.Pivot.SelectionChanged += new SelectionChangedEventHandler(this.Pivot_SelectionChanged);
        this.pickerStateItemsChangedSub_ = this.pickerState_.SubscribeToSelectedItemsChange((Action<MediaSharingState.SelectedItemsChangeCause>) (_ => this.Dispatcher.BeginInvokeIfNeeded((Action) (() => this.ProcessPickerStateSelectedItemsChanged()))));
        this.LoadAsync();
      }
      this.WarningTextBlockVideo.Text = AppState.IsLowMemoryDevice ? AppResources.ReachedMultiPickerLimit : AppResources.ReachedMultiPickerLimit30;
      this.WarningTextBlockPicture.Text = AppState.IsLowMemoryDevice ? AppResources.ReachedMultiPickerLimit : AppResources.ReachedMultiPickerLimit30;
    }

    public static IObservable<MediaSharingArgs> StartPhotoPicker(bool allowMultiSelection)
    {
      return MediaPickerPage.Start(new MediaPickerState(allowMultiSelection, MediaSharingState.SharingMode.ChoosePicture));
    }

    public static IObservable<MediaSharingArgs> StartVideoPicker()
    {
      return MediaPickerPage.Start(new MediaPickerState(false, MediaSharingState.SharingMode.ChooseVideo));
    }

    public static IObservable<MediaSharingArgs> StartMediaPicker(bool allowMultiSelection)
    {
      return MediaPickerPage.Start(new MediaPickerState(allowMultiSelection, MediaSharingState.SharingMode.ChooseMedia));
    }

    public static IObservable<MediaSharingArgs> Start(MediaPickerState pickerState)
    {
      return Observable.Create<MediaSharingArgs>((Func<IObserver<MediaSharingArgs>, Action>) (observer =>
      {
        MediaPickerPage.NextInstanceMediaPickerState = pickerState;
        MediaPickerPage.NextInstanceObserver = observer;
        NavUtils.NavigateToPage(nameof (MediaPickerPage));
        return (Action) (() => { });
      }));
    }

    private void NotifyObserver(MediaSharingArgs args = null)
    {
      MediaSharingArgs.SharingStatus status = MediaSharingArgs.SharingStatus.None;
      if (this.pickerState_.SelectedCount <= 0)
        status = MediaSharingArgs.SharingStatus.Canceled;
      if (args != null)
        args.Status = status;
      this.pickerObserver_.OnNext(args ?? new MediaSharingArgs((MediaSharingState) this.pickerState_, status, this.NavigationService));
    }

    private IEnumerable<MediaPickerState.Item> OrderByDate(IEnumerable<MediaPickerState.Item> source)
    {
      return source.Select(item =>
      {
        try
        {
          var data = new
          {
            Item = item,
            FileTime = item.MediaItem.GetFileTimeAttribute(MediaItemTimes.Date)
          };
          return data;
        }
        catch (Exception ex)
        {
          var data = null;
          return data;
        }
      }).Where(item => item != null).OrderBy(item => item.FileTime).Select(item => item.Item);
    }

    private void LoadAsync()
    {
      Log.l("media picker", "async loading starts");
      this.LoadingProgressBar.Visibility = Visibility.Visible;
      WAThreadPool.RunAfterDelay(TimeSpan.FromMilliseconds(100.0), (Action) (() =>
      {
        IMediaLibrary mediaLib = NativeInterfaces.MediaLib;
        bool abort = false;
        lock (this.loadingLock_)
        {
          if (this.isPageRemoved_)
            abort = true;
          else
            this.albumsSource_ = this.LoadMediaAlbumsSource(mediaLib);
        }
        if (!abort)
          this.Dispatcher.InvokeSynchronous((Action) (() => this.ShowAlbums()));
        lock (this.loadingLock_)
        {
          if (this.isPageRemoved_)
            abort = true;
          else if (this.albumsSource_ != null)
          {
            List<MediaPickerState.Item> objList = new List<MediaPickerState.Item>(this.albumsSource_.Sum<MediaPickerPage.Album>((Func<MediaPickerPage.Album, int>) (a => a.ItemsCount)));
            foreach (MediaPickerPage.Album album in this.albumsSource_)
            {
              if (this.isPageRemoved_)
              {
                abort = true;
                break;
              }
              objList.AddRange(album.GetItems());
            }
            switch (this.pickerState_.Mode)
            {
              case MediaSharingState.SharingMode.ChoosePicture:
                this.pics_ = (IEnumerable<MediaPickerState.Item>) objList;
                this.videos_ = (IEnumerable<MediaPickerState.Item>) null;
                break;
              case MediaSharingState.SharingMode.ChooseVideo:
                this.videos_ = (IEnumerable<MediaPickerState.Item>) objList;
                this.pics_ = (IEnumerable<MediaPickerState.Item>) null;
                break;
              default:
                LinkedList<MediaPickerState.Item> linkedList1 = new LinkedList<MediaPickerState.Item>();
                LinkedList<MediaPickerState.Item> linkedList2 = new LinkedList<MediaPickerState.Item>();
                string fullPathSafe = MediaPickerPage.preChosenChild != null ? MediaPickerPage.preChosenChild.GetFullPathSafe("prechosen") : (string) null;
                foreach (MediaPickerState.Item obj in objList)
                {
                  if (this.isPageRemoved_)
                  {
                    abort = true;
                    break;
                  }
                  if (fullPathSafe != null && fullPathSafe == obj.GetFullPathSafe("item"))
                  {
                    obj.IsSelected = true;
                    try
                    {
                      obj.VideoInfo = MediaPickerPage.preChosenChild.VideoInfo;
                      obj.MediaTimelineThumbnails = MediaPickerPage.preChosenChild.MediaTimelineThumbnails;
                      obj.RotatedTimes = MediaPickerPage.preChosenChild.RotatedTimes;
                      obj.RelativeCropPos = MediaPickerPage.preChosenChild.RelativeCropPos;
                      obj.RelativeCropSize = MediaPickerPage.preChosenChild.RelativeCropSize;
                      obj.Caption = MediaPickerPage.preChosenChild.Caption;
                      obj.DrawArgs = MediaPickerPage.preChosenChild.DrawArgs;
                      this.pickerState_.AddItem((MediaSharingState.IItem) obj);
                    }
                    catch (Exception ex)
                    {
                      Log.LogException(ex, "Failed to add the pre selected media item to the new picker");
                    }
                  }
                  if (obj.MediaType == FunXMPP.FMessage.Type.Image)
                    linkedList1.AddLast(obj);
                  else if (obj.MediaType == FunXMPP.FMessage.Type.Video)
                    linkedList2.AddLast(obj);
                }
                this.pics_ = (IEnumerable<MediaPickerState.Item>) linkedList1;
                this.videos_ = (IEnumerable<MediaPickerState.Item>) linkedList2;
                break;
            }
          }
        }
        this.Dispatcher.BeginInvoke((Action) (() =>
        {
          this.LoadingProgressBar.Visibility = Visibility.Collapsed;
          if (abort)
            return;
          if (this.pics_ != null && this.pics_.Count<MediaPickerState.Item>() > 0)
            this.Pivot.Items.Add((object) this.PicturesPivotItem);
          if (this.videos_ != null && this.videos_.Count<MediaPickerState.Item>() > 0)
            this.Pivot.Items.Add((object) this.VideosPivotItem);
          this.RefreshPage();
        }));
      }));
    }

    private List<MediaPickerPage.Album> LoadMediaAlbumsSource(IMediaLibrary mediaLib)
    {
      MediaItemTypes? filter = new MediaItemTypes?();
      switch (this.pickerState_.Mode)
      {
        case MediaSharingState.SharingMode.ChoosePicture:
          filter = new MediaItemTypes?(MediaItemTypes.Picture);
          break;
        case MediaSharingState.SharingMode.ChooseVideo:
          filter = new MediaItemTypes?(MediaItemTypes.Video);
          break;
      }
      int albumId = 0;
      return mediaLib.GetAlbums(filter).Where<MediaListWithName>((Func<MediaListWithName, bool>) (a => a.Guid != "{9ae241c6-e6cc-4080-a2ba-245e0f7c47c6}")).Select<MediaListWithName, MediaPickerPage.Album>((Func<MediaListWithName, MediaPickerPage.Album>) (a => new MediaPickerPage.Album(albumId++, a))).ToList<MediaPickerPage.Album>();
    }

    private void ShowAlbums()
    {
      if (this.isAlbumsShown_ || !this.isPageLoaded_ || this.albumsSource_ == null || this.isPageRemoved_)
      {
        Log.l("media picker", "skipping show albums {0} {1} {2} {3}", (object) this.isAlbumsShown_, (object) this.isPageLoaded_, (object) (this.albumsSource_ == null), (object) this.isPageRemoved_);
      }
      else
      {
        bool flag = this.albumsSource_ == null || this.albumsSource_.Count == 0;
        this.AlbumList.ItemsSource = flag ? (IEnumerable) null : (IEnumerable) this.albumsSource_;
        int num = flag ? 1 : 0;
        this.isAlbumsShown_ = true;
        Log.l("media picker", "showing albums");
      }
    }

    private void ShowVideoGrid()
    {
      if (this.isVideoGridShown_ || !this.isVideoTabVisited_ || this.videoGridSource_ == null || this.pickerState_.Mode != MediaSharingState.SharingMode.ChooseVideo && this.pickerState_.Mode != MediaSharingState.SharingMode.ChooseMedia)
      {
        Log.l("media picker", "show video grid | skipped");
      }
      else
      {
        Log.l("media picker", "show video grid");
        lock (this.loadingLock_)
        {
          DateTime? start = PerformanceTimer.Start(PerformanceTimer.Mode.DebugAndBeta);
          if (this.videoGridSource_ == null || this.videoGridSource_.Count == 0)
          {
            this.VideoGrid.ItemsSource = (List<KeyedList<string, MediaMultiSelector.Item>>) null;
            this.VideoGrid.FooterTextBlock.Text = AppResources.NoVideosAvailable;
          }
          else
          {
            this.VideoGrid.ItemsSource = this.videoGridSource_;
            this.VideoGrid.ScrollToBottom();
          }
          this.isVideoGridShown_ = true;
          PerformanceTimer.End("media picker page: show video grid", start);
        }
      }
    }

    private void ShowPictureGrid()
    {
      if (this.isPictureGridShown_ || !this.isPictureTabVisited_ || this.pictureGridSource_ == null || this.pickerState_.Mode != MediaSharingState.SharingMode.ChoosePicture && this.pickerState_.Mode != MediaSharingState.SharingMode.ChooseMedia)
      {
        Log.l("media picker", "show pic grid | skipped");
      }
      else
      {
        Log.l("media picker", "show pic grid");
        lock (this.loadingLock_)
        {
          DateTime? start = PerformanceTimer.Start(PerformanceTimer.Mode.DebugAndBeta);
          if (this.pictureGridSource_ == null || this.pictureGridSource_.Count == 0)
          {
            this.PictureGrid.ItemsSource = (List<KeyedList<string, MediaMultiSelector.Item>>) null;
            this.PictureGrid.FooterTextBlock.Text = AppResources.NoPicturesAvailable;
          }
          else
          {
            this.PictureGrid.ItemsSource = this.pictureGridSource_;
            this.PictureGrid.ScrollToBottom();
          }
          this.isPictureGridShown_ = true;
          PerformanceTimer.End("media picker page: show pic grid", start);
        }
      }
    }

    public static string GetPickerPageTitleText(MediaPickerState pickerState)
    {
      string pickerPageTitleText = AppResources.PageTitleChooseMedia;
      int selectedCount = pickerState.SelectedCount;
      if (selectedCount > 0)
      {
        switch (pickerState.Mode)
        {
          case MediaSharingState.SharingMode.ChoosePicture:
            pickerPageTitleText = Plurals.Instance.GetString(AppResources.SelectedPicturesPlural, selectedCount).ToUpper();
            break;
          case MediaSharingState.SharingMode.ChooseVideo:
            pickerPageTitleText = Plurals.Instance.GetString(AppResources.SelectedVideosPlural, selectedCount).ToUpper();
            break;
          default:
            pickerPageTitleText = Plurals.Instance.GetString(AppResources.SelectedMediaPlural, selectedCount).ToUpper();
            break;
        }
      }
      else
      {
        switch (pickerState.Mode)
        {
          case MediaSharingState.SharingMode.ChoosePicture:
            pickerPageTitleText = AppResources.PageTitleChoosePicture;
            break;
          case MediaSharingState.SharingMode.ChooseVideo:
            pickerPageTitleText = AppResources.PageTitleChooseVideo;
            break;
        }
      }
      return pickerPageTitleText;
    }

    private void UpdateAppBar()
    {
      if ((this.Pivot.SelectedItem == this.PicturesPivotItem || this.Pivot.SelectedItem == this.VideosPivotItem) && this.pickerState_.AllowMultiSelection)
      {
        if (this.PictureGrid.IsMultiSelectionEnabled)
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
      this.TitlePanel.SmallTitle = MediaPickerPage.GetPickerPageTitleText(this.pickerState_);
      int selectedCount = this.pickerState_.SelectedCount;
      bool flag = this.pickerState_.AllowMultiSelection && selectedCount > 0;
      this.PictureGrid.IsMultiSelectionEnabled = flag;
      this.VideoGrid.IsMultiSelectionEnabled = flag;
      this.UpdateAppBar();
      this.PictureGrid.IsSelectionAddBlocked = flag && selectedCount >= MediaSharingState.MaxSelectedItem;
      this.VideoGrid.IsSelectionAddBlocked = flag && selectedCount >= MediaSharingState.MaxSelectedItem;
      switch (this.pickerState_.Mode)
      {
        case MediaSharingState.SharingMode.ChoosePicture:
          this.Pivot.Items.Remove((object) this.VideosPivotItem);
          break;
        case MediaSharingState.SharingMode.ChooseVideo:
          this.Pivot.Items.Remove((object) this.PicturesPivotItem);
          break;
      }
    }

    public static bool TestAppBarActionBlocking(ref DateTime? lastActionTime)
    {
      DateTime now = DateTime.Now;
      if (lastActionTime.HasValue)
      {
        DateTime dateTime = now;
        DateTime? nullable1 = lastActionTime;
        TimeSpan? nullable2 = nullable1.HasValue ? new TimeSpan?(dateTime - nullable1.GetValueOrDefault()) : new TimeSpan?();
        TimeSpan timeSpan = TimeSpan.FromMilliseconds(500.0);
        if ((nullable2.HasValue ? (nullable2.GetValueOrDefault() < timeSpan ? 1 : 0) : 0) != 0)
          return true;
      }
      lastActionTime = new DateTime?(now);
      return false;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      if (this.pickerState_ == null || this.pickerObserver_ == null)
      {
        base.OnNavigatedTo(e);
        Log.l("media picker", "missing instance args");
        this.Dispatcher.BeginInvoke((Action) (() => NavUtils.GoBack(this.NavigationService)));
      }
      else
      {
        if (this.NavigationService.SearchBackStack("CameraPage") == 0)
          this.NavigationService.RemoveBackEntry();
        this.RefreshPage();
        base.OnNavigatedTo(e);
      }
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      this.isPageRemoved_ = true;
      lock (this.loadingLock_)
      {
        this.pickerStateItemsChangedSub_.SafeDispose();
        this.pickerStateItemsChangedSub_ = (IDisposable) null;
      }
      WAThreadPool.QueueUserWorkItem((Action) (() =>
      {
        lock (this.loadingLock_)
        {
          if (this.albumsSource_ == null)
            return;
          this.albumsSource_.ForEach((Action<MediaPickerPage.Album>) (a => a.SafeDispose()));
        }
      }));
      if (this.pickerObserver_ != null)
        this.pickerObserver_.OnCompleted();
      base.OnRemovedFromJournal(e);
    }

    protected override void OnBackKeyPress(CancelEventArgs e)
    {
      if (this.Pivot.IsLocked)
      {
        this.Pivot.IsLocked = false;
        e.Cancel = true;
      }
      if (this.NavigationService.SearchBackStack("PicturePreviewPage") == 0 && MediaPickerPage.preChosenChild == null)
        this.NavigationService.RemoveBackEntry();
      base.OnBackKeyPress(e);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
      this.isPageLoaded_ = true;
      this.ShowAlbums();
    }

    private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (this.Pivot.SelectedItem == this.VideosPivotItem)
      {
        this.isVideoTabVisited_ = true;
        if (!this.isVideoGridShown_)
        {
          this.Dispatcher.BeginInvoke((Action) (() => this.LoadingProgressBar.Visibility = Visibility.Visible));
          WAThreadPool.RunAfterDelay(TimeSpan.FromMilliseconds(100.0), (Action) (() =>
          {
            lock (this.loadingLock_)
            {
              if (this.videos_ != null)
              {
                if (this.videoGridSource_ == null)
                  this.videoGridSource_ = this.OrderByDate(this.videos_).Cast<MediaMultiSelector.Item>().GroupBy<MediaMultiSelector.Item, string>((Func<MediaMultiSelector.Item, string>) (item => item.GroupingKey)).Select<IGrouping<string, MediaMultiSelector.Item>, KeyedList<string, MediaMultiSelector.Item>>((Func<IGrouping<string, MediaMultiSelector.Item>, KeyedList<string, MediaMultiSelector.Item>>) (group => new KeyedList<string, MediaMultiSelector.Item>(group))).ToList<KeyedList<string, MediaMultiSelector.Item>>();
              }
            }
            this.Dispatcher.BeginInvoke((Action) (() =>
            {
              this.LoadingProgressBar.Visibility = Visibility.Collapsed;
              this.ShowVideoGrid();
            }));
          }));
        }
      }
      else if (this.Pivot.SelectedItem == this.PicturesPivotItem)
      {
        this.isPictureTabVisited_ = true;
        if (!this.isPictureGridShown_)
        {
          this.Dispatcher.BeginInvoke((Action) (() => this.LoadingProgressBar.Visibility = Visibility.Visible));
          WAThreadPool.RunAfterDelay(TimeSpan.FromMilliseconds(100.0), (Action) (() =>
          {
            lock (this.loadingLock_)
            {
              if (this.pics_ != null)
              {
                if (this.pictureGridSource_ == null)
                  this.pictureGridSource_ = this.OrderByDate(this.pics_).Cast<MediaMultiSelector.Item>().GroupBy<MediaMultiSelector.Item, string>((Func<MediaMultiSelector.Item, string>) (item => item.GroupingKey)).Select<IGrouping<string, MediaMultiSelector.Item>, KeyedList<string, MediaMultiSelector.Item>>((Func<IGrouping<string, MediaMultiSelector.Item>, KeyedList<string, MediaMultiSelector.Item>>) (group => new KeyedList<string, MediaMultiSelector.Item>(group))).ToList<KeyedList<string, MediaMultiSelector.Item>>();
              }
            }
            this.Dispatcher.BeginInvoke((Action) (() =>
            {
              this.LoadingProgressBar.Visibility = Visibility.Collapsed;
              this.ShowPictureGrid();
            }));
          }));
        }
      }
      this.UpdateAppBar();
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
      int selectedCount = this.pickerState_.SelectedCount;
      this.PictureGrid.IsMultiSelectionEnabled = true;
      this.VideoGrid.IsMultiSelectionEnabled = true;
      this.UpdateAppBar();
      this.PictureGrid.IsSelectionAddBlocked = selectedCount >= MediaSharingState.MaxSelectedItem;
      this.VideoGrid.IsSelectionAddBlocked = selectedCount >= MediaSharingState.MaxSelectedItem;
    }

    private void MediaGrid_SingleItemSelected(MediaMultiSelector.Item item)
    {
      if (this.pickerState_.SelectedCount > 0)
        this.pickerState_.DeleteItems((Func<MediaSharingState.IItem, bool>) null);
      if (!(item is MediaPickerState.Item obj))
        return;
      obj.IsSelected = true;
      this.pickerState_.ProcessItemSelectionChange(obj);
      this.NotifyObserver();
    }

    private void PictureGrid_ItemSelectionToggled(MediaMultiSelector.Item item)
    {
      this.pickerState_.ProcessItemSelectionChange(item as MediaPickerState.Item);
    }

    private void PictureGrid_ItemSelectionBlocked(MediaMultiSelector.Item item)
    {
      if (this.WarningPanelVideo.Visibility == Visibility.Collapsed)
        Storyboarder.Perform(this.Resources, "WarningVideoInSB");
      this.WarningPanelVideo.Visibility = Visibility.Visible;
      if (this.WarningPanelPicture.Visibility == Visibility.Collapsed)
        Storyboarder.Perform(this.Resources, "WarningPictureInSB");
      this.WarningPanelPicture.Visibility = Visibility.Visible;
      if (this.WarningTimer.IsEnabled)
        this.WarningTimer.Stop();
      this.WarningTimer.Start();
    }

    private void WarningTimer_Tick(object sender, EventArgs e)
    {
      Storyboarder.Perform(this.Resources, "WarningVideoOutSB", onComplete: (Action) (() => this.WarningPanelVideo.Visibility = Visibility.Collapsed));
      Storyboarder.Perform(this.Resources, "WarningPictureOutSB", onComplete: (Action) (() => this.WarningPanelPicture.Visibility = Visibility.Collapsed));
      if (!this.WarningTimer.IsEnabled)
        return;
      this.WarningTimer.Stop();
    }

    private void ProcessPickerStateSelectedItemsChanged()
    {
      this.TitlePanel.SmallTitle = MediaPickerPage.GetPickerPageTitleText(this.pickerState_);
      int selectedCount = this.pickerState_.SelectedCount;
      this.submitButton_.IsEnabled = selectedCount > 0;
      if (selectedCount < MediaSharingState.MaxSelectedItem)
      {
        this.WarningPanelVideo.Visibility = Visibility.Collapsed;
        this.WarningPanelPicture.Visibility = Visibility.Collapsed;
        this.PictureGrid.IsSelectionAddBlocked = false;
        this.VideoGrid.IsSelectionAddBlocked = false;
      }
      else
      {
        this.PictureGrid.IsSelectionAddBlocked = true;
        this.VideoGrid.IsSelectionAddBlocked = true;
      }
    }

    private void AlbumList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      MediaPickerPage.Album selectedItem = this.AlbumList.SelectedItem as MediaPickerPage.Album;
      this.AlbumList.SelectedItem = (object) null;
      if (selectedItem == null)
        return;
      IObservable<MediaSharingArgs> source = MediaPickerAlbumPage.Start(selectedItem, this.pickerState_);
      if (source == null)
        return;
      Log.l("media picker", "open album \"{0}\"", (object) selectedItem.Header);
      source.Subscribe<MediaSharingArgs>((Action<MediaSharingArgs>) (args => this.NotifyObserver(args)));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/MediaPickerPage.xaml", UriKind.Relative));
      this.PivotHeaderConverter = (PivotHeaderConverter) this.FindName("PivotHeaderConverter");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.LoadingProgressBar = (ProgressBar) this.FindName("LoadingProgressBar");
      this.TitlePanel = (PageTitlePanel) this.FindName("TitlePanel");
      this.Pivot = (Pivot) this.FindName("Pivot");
      this.AlbumsPivotItem = (PivotItem) this.FindName("AlbumsPivotItem");
      this.AlbumList = (ListBox) this.FindName("AlbumList");
      this.VideosPivotItem = (PivotItem) this.FindName("VideosPivotItem");
      this.VideoGrid = (MediaMultiSelector) this.FindName("VideoGrid");
      this.WarningPanelVideo = (Grid) this.FindName("WarningPanelVideo");
      this.WarningTextBlockVideo = (TextBlock) this.FindName("WarningTextBlockVideo");
      this.PicturesPivotItem = (PivotItem) this.FindName("PicturesPivotItem");
      this.PictureGrid = (MediaMultiSelector) this.FindName("PictureGrid");
      this.WarningPanelPicture = (Grid) this.FindName("WarningPanelPicture");
      this.WarningTextBlockPicture = (TextBlock) this.FindName("WarningTextBlockPicture");
    }

    public class Album : WaViewModelBase
    {
      private const int MaxCapacity = 1000000;
      private IEnumerable<MediaPickerState.Item> children_;
      private System.Windows.Media.ImageSource thumbnail_;
      private MediaListWithName listWithName_;
      private object albumLoadingLock_ = new object();

      public string Header
      {
        get => this.listWithName_ != null ? this.listWithName_.HeaderName : (string) null;
      }

      public string TileLabel
      {
        get => this.listWithName_ != null ? this.listWithName_.TileName : (string) null;
      }

      public int ItemsCount
      {
        get => this.listWithName_ != null ? this.listWithName_.List.GetItemCount() : 0;
      }

      public bool MostRecentAsFirst
      {
        get => this.listWithName_ != null && this.listWithName_.MostRecentAsFirst;
      }

      public System.Windows.Media.ImageSource Thumbnail
      {
        get
        {
          if (this.listWithName_ == null)
            return (System.Windows.Media.ImageSource) null;
          if (this.thumbnail_ == null)
          {
            byte[] thumbnail = this.listWithName_.GetThumbnail();
            if (thumbnail != null)
              this.thumbnail_ = (System.Windows.Media.ImageSource) BitmapUtils.CreateBitmap(thumbnail, 256, 256);
          }
          return this.thumbnail_;
        }
      }

      public int AlbumId { get; private set; }

      public Album(int albumId, MediaListWithName listWithName)
      {
        this.AlbumId = albumId;
        this.listWithName_ = listWithName;
      }

      private void LoadItems()
      {
        if (this.listWithName_ == null || this.children_ != null)
          return;
        lock (this.albumLoadingLock_)
        {
          if (this.children_ != null)
            return;
          Log.l("media picker", "load album \"{0}\" items", (object) this.Header);
          DateTime? start = PerformanceTimer.Start(PerformanceTimer.Mode.All);
          LinkedList<MediaPickerState.Item> linkedList = new LinkedList<MediaPickerState.Item>();
          int itemCount = this.listWithName_.List.GetItemCount();
          for (int Index = 0; Index < itemCount; ++Index)
            linkedList.AddLast(new MediaPickerState.Item(this.AlbumId * 1000000 + Index, wam_enum_media_picker_origin_type.CHAT_BUTTON_CAMERA_PHOTO_LIBRARY, this.listWithName_.List.GetItem(Index)));
          PerformanceTimer.End(string.Format("media picker: load album \"{0}\" items = {1}", (object) this.Header, (object) itemCount), start);
          this.children_ = (IEnumerable<MediaPickerState.Item>) linkedList;
        }
      }

      public IEnumerable<MediaPickerState.Item> GetItems(bool fetchIfNull = true)
      {
        if (this.children_ == null & fetchIfNull)
          this.LoadItems();
        return this.children_ ?? (IEnumerable<MediaPickerState.Item>) new MediaPickerState.Item[0];
      }

      protected override void DisposeManagedResources()
      {
        if (this.children_ != null)
        {
          lock (this.albumLoadingLock_)
          {
            if (this.children_ != null)
            {
              foreach (WaDisposable child in this.children_)
                child.Dispose();
              this.children_ = (IEnumerable<MediaPickerState.Item>) null;
            }
          }
        }
        if (this.listWithName_ != null)
          this.listWithName_.Dispose();
        base.DisposeManagedResources();
      }
    }
  }
}
