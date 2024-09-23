// Decompiled with JetBrains decompiler
// Type: WhatsApp.ContactsPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using WhatsApp.CommonOps;
using WhatsAppNative;


namespace WhatsApp
{
  public class ContactsPage : PhoneApplicationPage
  {
    private const string LogHeader = "mainpage";
    private GlobalProgressIndicator progressIndicator;
    private IDisposable refreshFavsSub;
    private IDisposable clearTileTimerSub;
    private ContactsPage.Tab currTab;
    private Microsoft.Phone.Shell.ApplicationBar mainAppBar;
    private Microsoft.Phone.Shell.ApplicationBar selectedChatsAppBar;
    private AppBarWrapper selectedChatsAppBarWrapper;
    private Microsoft.Phone.Shell.ApplicationBar callsAppBar;
    private Microsoft.Phone.Shell.ApplicationBar statusAppBar;
    private Microsoft.Phone.Shell.ApplicationBar selectedCallsAppBar;
    private Microsoft.Phone.Shell.ApplicationBar favsAppBar;
    private List<string> multiSelectedChats = new List<string>();
    private PivotItem statusPivotItem;
    private WaStatusList statusList;
    private PivotItem callsPivotItem;
    private CallHistoryList callList;
    private PivotItem chatsPivotItem;
    private Grid chatsPivotGrid;
    private IChatListControl chatList;
    private PivotItem favsPivotItem;
    private FavoriteList favList;
    private ContactsPagePivotHeader TabHeader;
    private CloudRestoreProgressControl restoreControl;
    private List<IDisposable> disposables = new List<IDisposable>();
    private bool isPageRemoved;
    private ApplicationBarMenuItem sendLogsMenuItem;
    private bool firstNavTo = true;
    private bool shouldScrollToTopOnLoaded;
    public static bool PushNagTestHook;
    internal ZoomBox LayoutRootZoomBox;
    internal Grid LayoutRoot;
    internal Pivot Pivot;
    private bool _contentLoaded;

    public static bool NaggedOnExit { get; set; }

    public bool IsOnChatList => this.currTab == ContactsPage.Tab.Chats;

    private Microsoft.Phone.Shell.ApplicationBar MainAppBar
    {
      get
      {
        return this.mainAppBar ?? (this.mainAppBar = this.Resources[(object) "MainMenu"] as Microsoft.Phone.Shell.ApplicationBar);
      }
    }

    private Microsoft.Phone.Shell.ApplicationBar SelectedChatsAppBar
    {
      get => this.GetSelectedChatsAppBar();
    }

    private Microsoft.Phone.Shell.ApplicationBar FavsAppBar
    {
      get
      {
        return this.favsAppBar ?? (this.favsAppBar = this.Resources[(object) "FavsMenu"] as Microsoft.Phone.Shell.ApplicationBar);
      }
    }

    private bool UseLLSChatList => InternalSettings.UseLLSChatList;

    public ContactsPage()
    {
      this.InitializeComponent();
      this.Init();
    }

    private void Init()
    {
      Log.d("mainpage", "init");
      FieldStats.SetContactsOpenStartTime();
      this.LayoutRootZoomBox.ZoomFactor = ResolutionHelper.ZoomFactor;
      this.progressIndicator = new GlobalProgressIndicator((DependencyObject) this);
      PivotHeaderConverter pivotHeaderConverter = new PivotHeaderConverter();
      ItemCollection items1 = this.Pivot.Items;
      PivotItem pivotItem1 = new PivotItem();
      pivotItem1.Header = (object) pivotHeaderConverter.Convert(AppResources.ChatsHeader);
      PivotItem pivotItem2 = pivotItem1;
      this.chatsPivotItem = pivotItem1;
      PivotItem pivotItem3 = pivotItem2;
      items1.Insert(0, (object) pivotItem3);
      Grid grid = new Grid();
      grid.Margin = new Thickness(0.0, 0.0, 0.0, 0.0);
      this.chatsPivotGrid = grid;
      this.chatsPivotGrid.RowDefinitions.Add(new RowDefinition()
      {
        Height = GridLength.Auto
      });
      this.chatsPivotGrid.RowDefinitions.Add(new RowDefinition()
      {
        Height = new GridLength(1.0, GridUnitType.Star)
      });
      if (this.UseLLSChatList)
      {
        ChatListControl element = new ChatListControl();
        this.chatList = (IChatListControl) element;
        this.chatsPivotItem.Margin = new Thickness(0.0);
        Grid.SetRow((FrameworkElement) element, 1);
        this.chatsPivotGrid.Children.Add((UIElement) element);
      }
      else
      {
        ChatListControlLB element = new ChatListControlLB();
        this.chatList = (IChatListControl) element;
        this.chatList.IsMultiSelectionAllowed = true;
        this.chatsPivotItem.Margin = new Thickness(0.0);
        Grid.SetRow((FrameworkElement) element, 1);
        this.chatsPivotGrid.Children.Add((UIElement) element);
      }
      this.chatsPivotItem.Content = (object) this.chatsPivotGrid;
      this.UpdateForStatusV3();
      this.UpdateForVoipCapability();
      ApplicationBarMenuItem applicationBarMenuItem1 = new ApplicationBarMenuItem()
      {
        Text = "StarredMessagesLower"
      };
      applicationBarMenuItem1.Click += new EventHandler(this.StarredMessagesMenuItem_Click);
      this.MainAppBar.MenuItems.Insert(2, (object) applicationBarMenuItem1);
      ApplicationBarMenuItem applicationBarMenuItem2 = new ApplicationBarMenuItem()
      {
        Text = "WhatsAppWebLower"
      };
      applicationBarMenuItem2.Click += new EventHandler(this.QrButton_Click);
      this.MainAppBar.MenuItems.Insert(4, (object) applicationBarMenuItem2);
      ApplicationBarIconButton applicationBarIconButton = new ApplicationBarIconButton()
      {
        Text = "AttachButtonCamera",
        IconUri = new Uri("/Images/camera-icon.png", UriKind.Relative)
      };
      applicationBarIconButton.Click += (EventHandler) ((sender, e) => this.LaunchCamera(false));
      this.MainAppBar.Buttons.Insert(0, (object) applicationBarIconButton);
      Localizable.LocalizeAppBar(this.MainAppBar);
      Localizable.LocalizeAppBar(this.FavsAppBar);
      this.chatList.SetSourceAsync(Observable.Create<List<ConversationItem>>((Func<IObserver<List<ConversationItem>>, Action>) (observer =>
      {
        DateTime? start = PerformanceTimer.Start(PerformanceTimer.Mode.DebugAndBeta);
        List<ConversationItem> items = (List<ConversationItem>) null;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db => items = db.GetConversationItems(new JidHelper.JidTypes[3]
        {
          JidHelper.JidTypes.User,
          JidHelper.JidTypes.Group,
          JidHelper.JidTypes.Psa
        }, false, SqliteMessagesContext.ConversationSortTypes.SortKeyAndTimestamp)));
        PerformanceTimer.End("MainPage > initial chats loading", start);
        ContactsContext.Instance((Action<ContactsContext>) (db =>
        {
          string[] array = items.Take<ConversationItem>(10).Select<ConversationItem, string>((Func<ConversationItem, string>) (ci =>
          {
            string str = (string) null;
            switch (ci.Conversation.JidType)
            {
              case JidHelper.JidTypes.User:
                str = ci.Jid;
                break;
              case JidHelper.JidTypes.Group:
                if (ci.Message != null && !ci.Message.KeyFromMe && ci.Message.MediaWaType != FunXMPP.FMessage.Type.System)
                {
                  str = ci.Message.GetSenderJid();
                  break;
                }
                break;
            }
            return str;
          })).Where<string>((Func<string, bool>) (jid => jid != null)).ToArray<string>();
          UserCache.Add((IEnumerable<UserStatus>) db.GetUserStatuses((IEnumerable<string>) array, false, false));
        }));
        observer.OnNext(items);
        observer.OnCompleted();
        return (Action) (() => { });
      })), (Func<Conversation, bool>) (c => !c.IsBroadcast()), (Action<ChatCollection>) (chats =>
      {
        chats.GetAllFromDb = (Func<MessagesContext, IEnumerable<Conversation>>) (db => (IEnumerable<Conversation>) db.GetConversations(new JidHelper.JidTypes[3]
        {
          JidHelper.JidTypes.User,
          JidHelper.JidTypes.Group,
          JidHelper.JidTypes.Psa
        }, false));
        chats.CollectionChanged += (EventHandler) ((sender, e) => this.UpdateChatListHelpText(chats));
        chats.CollectionChanged += (EventHandler) ((sender, e) => this.chatList.OnChatCollectionChanged());
        this.Dispatcher.BeginInvoke((Action) (() => this.UpdateChatListHelpText(chats)));
      }), "chat list");
      IDisposable disposable1 = this.chatList.ChatRequestedObservable().ObserveOnDispatcher<Conversation>().Subscribe<Conversation>(new Action<Conversation>(this.ChatList_ChatRequested));
      IDisposable disposable2 = this.chatList.MultiSelectionChangedObservable().ObserveOnDispatcher<Unit>().Subscribe<Unit>((Action<Unit>) (_ => this.ChatList_MultiSelectionsChanged()));
      IDisposable disposable3 = this.chatList.PendingSelectionObservable().ObserveOnDispatcher<bool>().Subscribe<bool>(new Action<bool>(this.ChatList_PendingSelection));
      this.disposables.Add(disposable1);
      this.disposables.Add(disposable2);
      this.disposables.Add(disposable3);
      this.disposables.Add(Settings.GetSettingsChangedObservable(new Settings.Key[1]
      {
        Settings.Key.IsWaAdmin
      }).ObserveOnDispatcher<Settings.Key>().Subscribe<Settings.Key>(new Action<Settings.Key>(this.OnSettingsChanged)));
      this.UpdateForWaAdminCapability();
      this.InitTabHeaders();
      Log.d("mainpage", "init complete");
    }

    private void UpdateChatListHelpText(ChatCollection chats)
    {
      if (chats.Count > 0)
      {
        this.chatList.SetTooltip((string) null);
      }
      else
      {
        bool hasArchivedChats = false;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db => hasArchivedChats = db.GetArchivedConversationItems().Any<ConversationItem>()));
        string str = string.Format(AppResources.StartNewChatHelpText, (object) "➕");
        this.chatList.SetTooltip(hasArchivedChats ? string.Format("{0}\n\n{1}", (object) AppResources.AllChatsAreArchived, (object) str) : str);
      }
    }

    private void EnsureStatusTabContent()
    {
      if (this.statusList != null)
      {
        Log.d("mainpage", "ensure status tab content | skip | already created");
      }
      else
      {
        WaStatusList waStatusList = new WaStatusList();
        waStatusList.Margin = new Thickness(0.0, 0.0, 0.0, 0.0);
        this.statusList = waStatusList;
        this.disposables.Add(this.statusList.AddNewStatusObservable().Throttle<Unit>(TimeSpan.FromSeconds(1.0)).ObserveOnDispatcher<Unit>().Subscribe<Unit>((Action<Unit>) (_ => this.LaunchCamera(true))));
        object obj;
        if (Settings.ShowStatusTabTooltipBanner)
        {
          Settings.ShowStatusTabTooltipBanner = false;
          Grid grid1 = new Grid();
          grid1.RowDefinitions.Add(new RowDefinition()
          {
            Height = new GridLength(1.0, GridUnitType.Auto)
          });
          grid1.RowDefinitions.Add(new RowDefinition()
          {
            Height = new GridLength(1.0, GridUnitType.Star)
          });
          Grid grid2 = new Grid();
          grid2.Background = (Brush) UIUtils.AccentBrush;
          grid2.Margin = new Thickness(0.0);
          Grid banner = grid2;
          banner.ColumnDefinitions.Add(new ColumnDefinition()
          {
            Width = new GridLength(1.0, GridUnitType.Star)
          });
          banner.ColumnDefinitions.Add(new ColumnDefinition()
          {
            Width = new GridLength(1.0, GridUnitType.Auto)
          });
          StackPanel stackPanel = new StackPanel();
          stackPanel.Margin = new Thickness(24.0, 24.0, 0.0, 24.0);
          StackPanel element1 = stackPanel;
          Grid.SetColumn((FrameworkElement) element1, 0);
          banner.Children.Add((UIElement) element1);
          TextBlock textBlock1 = new TextBlock();
          textBlock1.Text = AppResources.StatusOnboardTooltip;
          textBlock1.TextWrapping = TextWrapping.Wrap;
          textBlock1.Margin = new Thickness(0.0);
          textBlock1.Foreground = (Brush) UIUtils.WhiteBrush;
          textBlock1.VerticalAlignment = VerticalAlignment.Center;
          textBlock1.FontSize = 24.0;
          TextBlock textBlock2 = textBlock1;
          element1.Children.Add((UIElement) textBlock2);
          Button button1 = new Button();
          button1.Margin = new Thickness(-12.0, 24.0, 0.0, 0.0);
          button1.Padding = new Thickness(12.0, 6.0, 12.0, 12.0);
          button1.VerticalAlignment = VerticalAlignment.Center;
          button1.HorizontalAlignment = HorizontalAlignment.Left;
          button1.VerticalContentAlignment = VerticalAlignment.Center;
          button1.HorizontalContentAlignment = HorizontalAlignment.Center;
          button1.Content = (object) AppResources.PrivacySettings;
          button1.Foreground = (Brush) UIUtils.WhiteBrush;
          button1.BorderBrush = (Brush) UIUtils.WhiteBrush;
          Button button2 = button1;
          button2.Click += new RoutedEventHandler(this.PrivacySettingsMenuItem_Click);
          element1.Children.Add((UIElement) button2);
          Grid grid3 = new Grid();
          grid3.Background = (Brush) UIUtils.TransparentBrush;
          Grid element2 = grid3;
          element2.Tap += (EventHandler<System.Windows.Input.GestureEventArgs>) ((sender, e) => banner.Visibility = Visibility.Collapsed);
          Grid.SetColumn((FrameworkElement) element2, 1);
          banner.Children.Add((UIElement) element2);
          Image image1 = new Image();
          image1.Source = (System.Windows.Media.ImageSource) AssetStore.DismissIconWhite;
          image1.VerticalAlignment = VerticalAlignment.Center;
          image1.HorizontalAlignment = HorizontalAlignment.Center;
          image1.Margin = new Thickness(12.0, 0.0, 24.0, 0.0);
          image1.Height = 48.0;
          image1.Width = 48.0;
          image1.Stretch = Stretch.UniformToFill;
          Image image2 = image1;
          element2.Children.Add((UIElement) image2);
          Grid.SetRow((FrameworkElement) banner, 0);
          Grid.SetRow((FrameworkElement) this.statusList, 1);
          grid1.Children.Add((UIElement) banner);
          grid1.Children.Add((UIElement) this.statusList);
          obj = (object) grid1;
        }
        else
          obj = (object) this.statusList;
        this.statusPivotItem.Content = obj;
        Log.d("mainpage", "created status tab content");
      }
    }

    private void UpdateForStatusV3()
    {
      this.statusList.SafeDispose();
      this.statusList = (WaStatusList) null;
      PivotHeaderConverter pivotHeaderConverter = new PivotHeaderConverter();
      if (this.favsPivotItem != null)
        this.Pivot.Items.Remove((object) this.favsPivotItem);
      if (this.statusPivotItem != null)
        return;
      ItemCollection items = this.Pivot.Items;
      PivotItem pivotItem1 = new PivotItem();
      pivotItem1.Margin = new Thickness(0.0);
      pivotItem1.Header = (object) pivotHeaderConverter.Convert(AppResources.StatusV3Title);
      PivotItem pivotItem2 = pivotItem1;
      this.statusPivotItem = pivotItem1;
      PivotItem pivotItem3 = pivotItem2;
      items.Add((object) pivotItem3);
      Log.d("mainpage", "created status tab");
    }

    private void UpdateForVoipCapability()
    {
      if (this.callsPivotItem != null)
        this.Pivot.Items.Remove((object) this.callsPivotItem);
      this.callList.SafeDispose();
      this.callList = (CallHistoryList) null;
      PivotHeaderConverter pivotHeaderConverter = new PivotHeaderConverter();
      ItemCollection items = this.Pivot.Items;
      PivotItem pivotItem1 = new PivotItem();
      pivotItem1.Margin = new Thickness(0.0);
      pivotItem1.Header = (object) pivotHeaderConverter.Convert(AppResources.CallsHeader);
      CallHistoryList callHistoryList1 = new CallHistoryList();
      callHistoryList1.Margin = new Thickness(0.0, 0.0, 0.0, 0.0);
      CallHistoryList callHistoryList2 = callHistoryList1;
      this.callList = callHistoryList1;
      pivotItem1.Content = (object) callHistoryList2;
      PivotItem pivotItem2 = pivotItem1;
      this.callsPivotItem = pivotItem1;
      PivotItem pivotItem3 = pivotItem2;
      items.Insert(1, (object) pivotItem3);
    }

    private void UpdateForWaAdminCapability()
    {
      if (this.sendLogsMenuItem != null)
      {
        this.MainAppBar.MenuItems.Remove((object) this.sendLogsMenuItem);
        this.sendLogsMenuItem = (ApplicationBarMenuItem) null;
      }
      if (!Settings.IsWaAdmin)
        return;
      this.sendLogsMenuItem = new ApplicationBarMenuItem()
      {
        Text = "send log"
      };
      this.sendLogsMenuItem.Click += (EventHandler) ((sender, e) =>
      {
        WaUriParams uriParams = new WaUriParams();
        uriParams.AddBool("quicky", true);
        NavUtils.NavigateToPage(this.NavigationService, "ContactSupportPage", uriParams);
      });
      this.MainAppBar.MenuItems.Add((object) this.sendLogsMenuItem);
    }

    private void UpdateForCloudMediaRestore()
    {
      if (OneDriveRestoreManager.IsRestoreIncomplete)
        this.AddCloudRestoreControl();
      else
        this.RemoveCloudRestoreControl();
    }

    private void AddCloudRestoreControl()
    {
      if (this.restoreControl != null)
        return;
      OneDriveRestoreManager.Instance.RestoreStopped += new EventHandler<BkupRestStoppedEventArgs>(this.OneDrive_RestoreStopped);
      CloudRestoreProgressControl restoreProgressControl = new CloudRestoreProgressControl();
      restoreProgressControl.Margin = new Thickness(24.0, 12.0, 24.0, 12.0);
      this.restoreControl = restoreProgressControl;
      this.restoreControl.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.RestoreProgress_Tap);
      Grid.SetRow((FrameworkElement) this.restoreControl, 0);
      this.chatsPivotGrid.Children.Insert(0, (UIElement) this.restoreControl);
    }

    private void RemoveCloudRestoreControl()
    {
      if (this.restoreControl == null)
        return;
      OneDriveRestoreManager.Instance.RestoreStopped -= new EventHandler<BkupRestStoppedEventArgs>(this.OneDrive_RestoreStopped);
      this.restoreControl.Tap -= new EventHandler<System.Windows.Input.GestureEventArgs>(this.RestoreProgress_Tap);
      this.chatsPivotGrid.Children.Remove((UIElement) this.restoreControl);
      this.restoreControl = (CloudRestoreProgressControl) null;
    }

    private void InitTabHeaders()
    {
      ContactsPagePivotHeader contactsPagePivotHeader = new ContactsPagePivotHeader(this.Pivot);
      contactsPagePivotHeader.Margin = new Thickness(24.0, 0.0, 24.0, 0.0);
      contactsPagePivotHeader.HorizontalAlignment = HorizontalAlignment.Stretch;
      this.TabHeader = contactsPagePivotHeader;
      this.TabHeader.EnableUpdate(true);
      this.TabHeader.UpdateAsync(true, true, true);
      Grid.SetRow((FrameworkElement) this.TabHeader, 0);
      this.LayoutRoot.Children.Add((UIElement) this.TabHeader);
      this.disposables.Add(this.TabHeader.TappedChatsPivotObservable().ObserveOnDispatcher<Unit>().Subscribe<Unit>((Action<Unit>) (_ => this.TabHeader_TappedChatsPivot())));
    }

    private void SearchFavorites(string title)
    {
      ListTabData[] tabs = new ListTabData[1];
      WaContactsListTabData contactsListTabData = new WaContactsListTabData();
      contactsListTabData.Header = (string) null;
      tabs[0] = (ListTabData) contactsListTabData;
      JidItemPickerPage.Start(tabs, title).ObserveOnDispatcher<List<string>>().Subscribe<List<string>>((Action<List<string>>) (selJids => NavUtils.NavigateToChat(selJids.FirstOrDefault<string>(), false, nameof (ContactsPage))));
    }

    private void SearchChats()
    {
      this.PerformPageTransition((Action) (() => NavUtils.NavigateToPage(this.NavigationService, "ChatSearchPage")), PageTransitionAnimation.FadeOut);
    }

    private double CalcAvailableShortTextWidth(bool? isLandscape = null)
    {
      if (!isLandscape.HasValue)
        isLandscape = new bool?((this.Orientation & PageOrientation.Portrait) == PageOrientation.None);
      return isLandscape.Value ? Math.Max(Application.Current.Host.Content.ActualWidth, Application.Current.Host.Content.ActualHeight) - 350.0 : Math.Min(Application.Current.Host.Content.ActualWidth, Application.Current.Host.Content.ActualHeight) - 200.0;
    }

    private void PerformPageTransition(Action forwardOut, Action onForwardOutNavigated = null)
    {
      if (forwardOut == null)
        return;
      forwardOut();
      if (onForwardOutNavigated == null)
        return;
      App.CurrentApp.ScheduleRootFrameNavigatedWork(onForwardOutNavigated);
    }

    private void PerformPageTransition(
      Action forwardOut,
      PageTransitionAnimation transition,
      Action onForwardOutNavigated = null)
    {
      Storyboard pendingSb = Storyboarder.Perform(WaAnimations.PageTransition(transition), (DependencyObject) this.LayoutRoot, false, forwardOut);
      if (pendingSb != null)
        App.CurrentApp.ScheduleRootFrameNavigatedWork((Action) (() => pendingSb.Stop()));
      if (onForwardOutNavigated == null)
        return;
      App.CurrentApp.ScheduleRootFrameNavigatedWork(onForwardOutNavigated);
    }

    private void NagPush()
    {
      string message = AppResources.NoPushSummary;
      if (TimeSpan.FromMilliseconds((double) NativeInterfaces.Misc.GetTickCount()) > TimeSpan.FromMinutes(10.0))
        message = message + "\n\n" + AppResources.PleaseReboot;
      UIUtils.MessageBox(AppResources.NoPushTitle, message, (IEnumerable<string>) new string[2]
      {
        AppResources.DismissButton,
        AppResources.LearnMoreButton
      }, (Action<int>) (idx =>
      {
        if (idx != 1)
          return;
        new WebBrowserTask()
        {
          Uri = new Uri(WaWebUrls.FaqlUrlPushDisconnected)
        }.Show();
      }));
    }

    private void NagBgAgent()
    {
      UIUtils.MessageBox(AppResources.GenericWarning, AppResources.BgAgentDisabled, (IEnumerable<string>) new string[2]
      {
        AppResources.DismissButton,
        AppResources.Settings
      }, (Action<int>) (idx =>
      {
        if (idx != 1)
          return;
        NavUtils.NavigateExternal("app://25BB0297-A723-479c-BDC8-9274FC6C6470/_default");
      }));
    }

    private void NagMPNSTiles()
    {
      Settings.MPNSChatTileExists = false;
      UIUtils.MessageBox(AppResources.GenericWarning, AppResources.TilesDisappearingWarning, (IEnumerable<string>) new string[1]
      {
        AppResources.DismissButton
      }, (Action<int>) (idx => { }));
    }

    public static void NagDeprecation()
    {
      UIUtils.MessageBox(AppResources.WP7UnsupportedPhone, string.Format(AppResources.WP7DeprecationNagDescription, (object) AppState.GetDeprecationDateString(), (object) AppResources.LearnMoreButton), (IEnumerable<string>) new string[2]
      {
        AppResources.LearnMoreButton,
        AppResources.RemindMeLater
      }, (Action<int>) (idx =>
      {
        if (idx == 1)
          AppState.SetDeprecationNagDisplayed();
        if (idx != 0)
          return;
        new WebBrowserTask()
        {
          Uri = new Uri(Constants.SwitchPhonesUrl)
        }.Show();
      }), true);
    }

    private Microsoft.Phone.Shell.ApplicationBar GetStatusAppBar()
    {
      if (this.statusAppBar != null)
        return this.statusAppBar;
      Microsoft.Phone.Shell.ApplicationBar bar = new Microsoft.Phone.Shell.ApplicationBar();
      ApplicationBarIconButton applicationBarIconButton1 = new ApplicationBarIconButton()
      {
        IconUri = new Uri("/Images/camera-icon.png", UriKind.Relative),
        Text = "AddNewStatus"
      };
      applicationBarIconButton1.Click += (EventHandler) ((sender, e) => this.LaunchCamera(true));
      bar.Buttons.Add((object) applicationBarIconButton1);
      ApplicationBarIconButton applicationBarIconButton2 = new ApplicationBarIconButton()
      {
        IconUri = new Uri("/Images/edit.png", UriKind.Relative),
        Text = "AddNewTextStatus"
      };
      applicationBarIconButton2.Click += (EventHandler) ((sender, e) => this.ComposeTextStatus());
      bar.Buttons.Add((object) applicationBarIconButton2);
      ApplicationBarMenuItem applicationBarMenuItem = new ApplicationBarMenuItem()
      {
        Text = "PrivacySettings"
      };
      applicationBarMenuItem.Click += new EventHandler(this.PrivacySettingsMenuItem_Click);
      bar.MenuItems.Add((object) applicationBarMenuItem);
      Localizable.LocalizeAppBar(bar);
      return this.statusAppBar = bar;
    }

    private Microsoft.Phone.Shell.ApplicationBar GetCallsAppBar()
    {
      if (this.callsAppBar != null)
        return this.callsAppBar;
      Microsoft.Phone.Shell.ApplicationBar bar = new Microsoft.Phone.Shell.ApplicationBar();
      ApplicationBarIconButton applicationBarIconButton1 = new ApplicationBarIconButton()
      {
        IconUri = new Uri("/Images/assets/dark/phone-solid.png", UriKind.Relative),
        Text = "InitiateVoiceCall"
      };
      applicationBarIconButton1.Click += new EventHandler(this.NewCallButton_Click);
      bar.Buttons.Add((object) applicationBarIconButton1);
      ApplicationBarIconButton applicationBarIconButton2 = new ApplicationBarIconButton()
      {
        IconUri = new Uri("/Images/assets/dark/video-solid.png", UriKind.Relative),
        Text = "InitiateVideoCall"
      };
      applicationBarIconButton2.Click += new EventHandler(this.NewVideoCallButton_Click);
      bar.Buttons.Add((object) applicationBarIconButton2);
      ApplicationBarMenuItem applicationBarMenuItem1 = new ApplicationBarMenuItem()
      {
        Text = "SettingsTitle"
      };
      applicationBarMenuItem1.Click += new EventHandler(this.SettingsMenuItem_Click);
      bar.MenuItems.Add((object) applicationBarMenuItem1);
      ApplicationBarMenuItem applicationBarMenuItem2 = new ApplicationBarMenuItem()
      {
        Text = "CallLogMenuDeleteCallHistory"
      };
      applicationBarMenuItem2.Click += new EventHandler(this.DeleteCallHistory_Click);
      bar.MenuItems.Add((object) applicationBarMenuItem2);
      Localizable.LocalizeAppBar(bar);
      return this.callsAppBar = bar;
    }

    private Microsoft.Phone.Shell.ApplicationBar GetSelectedCallsAppBar()
    {
      if (this.selectedCallsAppBar != null)
        return this.selectedCallsAppBar;
      Microsoft.Phone.Shell.ApplicationBar resource = this.Resources[(object) "SelectedCallsMenu"] as Microsoft.Phone.Shell.ApplicationBar;
      Localizable.LocalizeAppBar(resource);
      return this.selectedCallsAppBar = resource;
    }

    private Microsoft.Phone.Shell.ApplicationBar GetSelectedChatsAppBar()
    {
      if (this.selectedChatsAppBar != null)
        return this.selectedChatsAppBar;
      Microsoft.Phone.Shell.ApplicationBar resource = this.Resources[(object) "SelectedChatsMenu"] as Microsoft.Phone.Shell.ApplicationBar;
      AppBarWrapper appBarWrapper = new AppBarWrapper((IApplicationBar) resource);
      ApplicationBarIconButton button = resource.Buttons[0] as ApplicationBarIconButton;
      appBarWrapper.AddButtonUpdateAction(0, (Action<object, ApplicationBarIconButton>) ((argObj, argButton) =>
      {
        bool flag = this.multiSelectedChats.Any<string>();
        argButton.IsEnabled = flag;
      }));
      button = resource.Buttons[1] as ApplicationBarIconButton;
      appBarWrapper.AddButtonUpdateAction(1, (Action<object, ApplicationBarIconButton>) ((argObj, argButton) =>
      {
        bool flag1 = this.multiSelectedChats.Any<string>();
        bool flag2 = this.multiSelectedChats.Any<string>((Func<string, bool>) (jid => JidHelper.IsGroupJid(jid)));
        argButton.IsEnabled = flag1 && !flag2;
      }));
      Uri muteIconUri = new Uri("/Images/assets/dark/mute.png", UriKind.Relative);
      Uri unmuteIconUri = new Uri("/Images/assets/dark/unmute.png", UriKind.Relative);
      bool mute = true;
      ApplicationBarIconButton applicationBarIconButton1 = new ApplicationBarIconButton(muteIconUri);
      applicationBarIconButton1.Text = "MuteTitle";
      string unmuteText = Localizable.StringForKey("Unmute");
      string muteText = Localizable.StringForKey(applicationBarIconButton1.Text);
      applicationBarIconButton1.Click += (EventHandler) ((sender, e) => this.MuteChats_Click(sender, e, mute));
      resource.Buttons.Add((object) applicationBarIconButton1);
      appBarWrapper.AddButtonUpdateAction(2, (Action<object, ApplicationBarIconButton>) ((argObj, argButton) =>
      {
        int num1 = this.multiSelectedChats.Count<string>();
        int num2 = this.multiSelectedChats.Count<string>((Func<string, bool>) (jid =>
        {
          Conversation convo = (Conversation) null;
          MessagesContext.Run((MessagesContext.MessagesCallback) (db => convo = db.GetConversation(jid, CreateOptions.None)));
          return convo.IsMuted();
        }));
        if (num1 > 0)
        {
          if (num2 == num1)
          {
            argButton.IsEnabled = true;
            argButton.Text = unmuteText;
            argButton.IconUri = unmuteIconUri;
            mute = false;
          }
          else if (num2 == 0)
          {
            argButton.IsEnabled = true;
            argButton.Text = muteText;
            argButton.IconUri = muteIconUri;
            mute = true;
          }
          else
            argButton.IsEnabled = false;
        }
        else
          argButton.IsEnabled = false;
      }));
      Uri pinIconUri = new Uri("/Images/assets/dark/pin.png", UriKind.Relative);
      Uri unpinIconUri = new Uri("/Images/assets/dark/unpin.png", UriKind.Relative);
      bool pin = true;
      ApplicationBarIconButton applicationBarIconButton2 = new ApplicationBarIconButton(pinIconUri);
      applicationBarIconButton2.Text = "PinTitle";
      string unpinText = Localizable.StringForKey("UnpinTitle");
      string pinText = Localizable.StringForKey(applicationBarIconButton2.Text);
      applicationBarIconButton2.Click += (EventHandler) ((sender, e) => this.PinChats_Click(sender, e, pin));
      resource.Buttons.Add((object) applicationBarIconButton2);
      appBarWrapper.AddButtonUpdateAction(3, (Action<object, ApplicationBarIconButton>) ((argObj, argButton) =>
      {
        int num3 = this.multiSelectedChats.Count<string>();
        int num4 = this.multiSelectedChats.Count<string>((Func<string, bool>) (jid =>
        {
          Conversation convo = (Conversation) null;
          MessagesContext.Run((MessagesContext.MessagesCallback) (db => convo = db.GetConversation(jid, CreateOptions.None)));
          return convo.IsPinned();
        }));
        int numberOfAlreadyPinned = 0;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db => numberOfAlreadyPinned = ((IEnumerable<Conversation>) db.GetPinnedConversations()).Count<Conversation>()));
        bool flag = numberOfAlreadyPinned + num3 <= 3;
        if (num3 > 0)
        {
          if (num4 == num3)
          {
            argButton.IsEnabled = true;
            argButton.Text = unpinText;
            argButton.IconUri = unpinIconUri;
            pin = false;
          }
          else if (num4 == 0 & flag)
          {
            argButton.IsEnabled = true;
            argButton.Text = pinText;
            argButton.IconUri = pinIconUri;
            pin = true;
          }
          else
            argButton.IsEnabled = false;
        }
        else
          argButton.IsEnabled = false;
      }));
      ApplicationBarMenuItem applicationBarMenuItem = new ApplicationBarMenuItem()
      {
        Text = "MarkAsRead"
      };
      string markAsUnreadText = Localizable.StringForKey("MarkAsUnread");
      string markAsReadText = Localizable.StringForKey(applicationBarMenuItem.Text);
      bool markAsRead = true;
      applicationBarMenuItem.Click += (EventHandler) ((sender, e) => this.MarkAsReadMenuItem_Click(sender, e, markAsRead));
      resource.MenuItems.Add((object) applicationBarMenuItem);
      resource.IsMenuEnabled = true;
      appBarWrapper.AddMenuItemUpdateAction(0, (Action<object, ApplicationBarMenuItem>) ((argObj, argMenuItem) =>
      {
        int num5 = this.multiSelectedChats.Count<string>();
        int num6 = this.multiSelectedChats.Count<string>((Func<string, bool>) (jid =>
        {
          Conversation convo = (Conversation) null;
          MessagesContext.Run((MessagesContext.MessagesCallback) (db => convo = db.GetConversation(jid, CreateOptions.None)));
          return !convo.IsRead();
        }));
        if (num5 > 0)
        {
          if (num6 == num5)
          {
            argMenuItem.Text = markAsReadText;
            markAsRead = true;
            argMenuItem.IsEnabled = true;
          }
          else if (num6 == 0)
          {
            argMenuItem.Text = markAsUnreadText;
            markAsRead = false;
            argMenuItem.IsEnabled = true;
          }
          else
            argMenuItem.IsEnabled = false;
        }
        else
          argMenuItem.IsEnabled = false;
      }));
      Localizable.LocalizeAppBar(resource);
      this.selectedChatsAppBarWrapper = appBarWrapper;
      return this.selectedChatsAppBar = resource;
    }

    private void OneDrive_RestoreStopped(object sender, BkupRestStoppedEventArgs e)
    {
      bool flag;
      switch (e.Reason)
      {
        case OneDriveBkupRestStopReason.Completed:
        case OneDriveBkupRestStopReason.Abort:
        case OneDriveBkupRestStopReason.AbortError:
          flag = true;
          break;
        default:
          flag = false;
          break;
      }
      if (!flag)
        return;
      Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() => this.RemoveCloudRestoreControl()));
    }

    private void RestoreProgress_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      NavUtils.NavigateToPage(this.NavigationService, "ChatBackupPage", folderName: "Pages/Settings");
    }

    private void ChatList_PendingSelection(bool pending)
    {
      if (pending && !this.Pivot.IsLocked)
      {
        Log.d("mainpage", "locking pivot pending selection");
        this.Pivot.IsLocked = true;
      }
      else
      {
        if (this.chatList.IsMultiSelectionEnabled || !this.Pivot.IsLocked)
          return;
        Log.d("mainpage", "unlocking pivot pending selection");
        this.Pivot.IsLocked = false;
      }
    }

    private void ChatList_MultiSelectionsChanged()
    {
      this.multiSelectedChats = this.chatList.GetMultiSelectedChats();
      Microsoft.Phone.Shell.ApplicationBar applicationBar;
      if (this.multiSelectedChats.Any<string>() || this.chatList.IsMultiSelectionEnabled)
      {
        this.Pivot.IsLocked = true;
        applicationBar = this.SelectedChatsAppBar;
        this.selectedChatsAppBarWrapper.UpdateAppBar();
        this.TabHeader.ShowSelected(this.multiSelectedChats.Count<string>());
      }
      else
      {
        this.Pivot.IsLocked = false;
        applicationBar = this.MainAppBar;
        this.TabHeader.ShowSelected(0);
      }
      if (this.ApplicationBar == applicationBar)
        return;
      this.ApplicationBar = (IApplicationBar) applicationBar;
    }

    private void TabHeader_TappedChatsPivot() => this.chatList.EnableMultiSelection(false);

    private void ChatList_ChatRequested(Conversation c)
    {
      if (c == null)
        return;
      Storyboarder.Perform(WaAnimations.PageTransition(PageTransitionAnimation.ContinuumForwardOut), (DependencyObject) this.LayoutRoot);
      this.Dispatcher.BeginInvokeIfNeeded((Action) (() => NavUtils.NavigateToChat(this.NavigationService, c.Jid, false, nameof (ContactsPage))));
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      Log.d("mainpage", nameof (OnNavigatedTo));
      IDictionary<string, string> queryString = this.NavigationContext.QueryString;
      string str = (string) null;
      if (queryString.TryGetValue("type", out str) && str == "invite")
      {
        string code = (string) null;
        if (queryString.TryGetValue("code", out code) && !string.IsNullOrEmpty(code))
        {
          queryString.Remove("type");
          queryString.Remove("code");
          GroupInviteLinkPage.JoinGroupWithInviteCode(code);
        }
      }
      this.shouldScrollToTopOnLoaded = false;
      if (e.IsNavigationInitiator)
      {
        if (e.NavigationMode == NavigationMode.Back && App.CurrentApp.RecentNavigatedFromUri != null && App.CurrentApp.RecentNavigatedFromUri.StartsWith(UriUtils.CreatePageUriStr("ChatPage.xaml")))
        {
          this.LayoutRoot.Opacity = 0.0;
          EventHandler updated = (EventHandler) null;
          updated = (EventHandler) ((_, __) =>
          {
            this.LayoutUpdated -= updated;
            if (this.isPageRemoved)
              return;
            this.Dispatcher.BeginInvoke((Action) (() => Storyboarder.Perform(WaAnimations.PageTransition(PageTransitionAnimation.ContinuumBackwardIn), (DependencyObject) this.LayoutRoot, onComplete: (Action) (() => this.LayoutRoot.Opacity = 1.0))));
            this.chatList.EnsurePreviousSelectedChatVisible();
          });
          this.LayoutUpdated += updated;
        }
      }
      else
      {
        if (BackgroundDataDisabledPage.Applicable)
          this.Dispatcher.BeginInvoke((Action) (() => NavUtils.NavigateToPage("BackgroundDataDisabledPage", "ClearStack=true")));
        else if (AppState.BatterySaverEnabled)
          this.Dispatcher.BeginInvoke((Action) (() => Nag.NagBatterySaver()));
        else if (BackgroundAgentHelper.BackgroundAgentDisabled)
          this.Dispatcher.BeginInvoke((Action) (() => this.NagBgAgent()));
        else if (AppState.UseWindowsNotificationService && Settings.MPNSChatTileExists)
          this.Dispatcher.BeginInvoke((Action) (() => this.NagMPNSTiles()));
        else if (AppState.IsTimeForDeprecationNag())
          this.Dispatcher.BeginInvoke((Action) (() => ContactsPage.NagDeprecation()));
        if (!this.firstNavTo)
        {
          this.shouldScrollToTopOnLoaded = true;
          this.GetLayoutUpdatedAsync().Take<Unit>(1).Subscribe<Unit>((Action<Unit>) (_ =>
          {
            if (!this.shouldScrollToTopOnLoaded)
              return;
            if (this.chatList.IsMultiSelectionEnabled)
              this.chatList.EnableMultiSelection(false);
            this.chatList.ScrollToTop();
            this.shouldScrollToTopOnLoaded = false;
          }));
        }
      }
      this.UpdateForCloudMediaRestore();
      base.OnNavigatedTo(e);
      this.firstNavTo = false;
      this.clearTileTimerSub.SafeDispose();
      this.clearTileTimerSub = (e.NavigationMode != NavigationMode.Reset ? Observable.Return<Unit>(new Unit()) : Observable.Timer(TimeSpan.FromMilliseconds(1000.0)).Take<long>(1).Select<long, Unit>((Func<long, Unit>) (_ => new Unit()))).ObserveOn<Unit>(WAThreadPool.Scheduler).Subscribe<Unit>((Action<Unit>) (_ =>
      {
        this.clearTileTimerSub.SafeDispose();
        this.clearTileTimerSub = (IDisposable) null;
        TileHelper.ClearMainTile();
      }));
      try
      {
        NavUtils.ClearBackStack();
      }
      catch (Exception ex)
      {
        Log.SendCrashLog(ex, "clearing back stack on navigated to main page");
      }
      DateTime? eulaAcceptedUtc = Settings.EULAAcceptedUtc;
      DateTime? registrationCompleteUtc = Settings.RegistrationCompleteUtc;
      if (eulaAcceptedUtc.HasValue && (!registrationCompleteUtc.HasValue || registrationCompleteUtc.Value < eulaAcceptedUtc.Value))
      {
        DateTime utcNow = DateTime.UtcNow;
        if (utcNow <= eulaAcceptedUtc.Value)
        {
          Log.l("mainpage", "Unexpected times found for registration event {0}, {1}, {2}", (object) eulaAcceptedUtc.Value, (object) registrationCompleteUtc, (object) utcNow);
          Log.SendCrashLog((Exception) new ArgumentException("Unexpected registration time"), "Contacts before eula accepted");
          Settings.RegistrationCompleteUtc = eulaAcceptedUtc;
        }
        else
        {
          long ticks = (utcNow - eulaAcceptedUtc.Value).Ticks;
          Settings.RegistrationCompleteUtc = new DateTime?(utcNow);
          if (ticks <= TimeSpan.FromDays(1.0).Ticks || registrationCompleteUtc.HasValue)
            FieldStats.ReportRegistrationComplete(ticks / 10000L);
        }
      }
      Log.d("mainpage", "OnNavigatedTo complete");
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
      this.clearTileTimerSub.SafeDispose();
      this.clearTileTimerSub = (IDisposable) null;
      base.OnNavigatedFrom(e);
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      this.isPageRemoved = true;
      this.statusList.SafeDispose();
      this.statusList = (WaStatusList) null;
      this.chatList.Clear();
      this.callList.SafeDispose();
      this.refreshFavsSub.SafeDispose();
      this.refreshFavsSub = (IDisposable) null;
      List<IDisposable> list = this.disposables.ToList<IDisposable>();
      this.disposables.Clear();
      list.ForEach((Action<IDisposable>) (d => d.SafeDispose()));
      base.OnRemovedFromJournal(e);
    }

    protected override void OnBackKeyPress(CancelEventArgs e)
    {
      if (this.currTab != ContactsPage.Tab.Chats)
      {
        e.Cancel = true;
        this.Pivot.SelectedItem = (object) this.chatsPivotItem;
      }
      else if (this.chatList.IsMultiSelectionEnabled)
      {
        e.Cancel = true;
        this.chatList.EnableMultiSelection(false);
      }
      else
      {
        if (!ContactsPage.NaggedOnExit)
        {
          if (AppState.BatterySaverEnabled)
          {
            e.Cancel = ContactsPage.NaggedOnExit = true;
            Nag.NagBatterySaver();
            return;
          }
          if (BackgroundAgentHelper.BackgroundAgentDisabled)
          {
            e.Cancel = ContactsPage.NaggedOnExit = true;
            this.NagBgAgent();
            return;
          }
          bool flag = false;
          try
          {
            flag = NativeInterfaces.Misc.GetCellInfo(CellInfoFlags.NetworkInfo).Roaming;
          }
          catch (Exception ex)
          {
          }
          if (!flag && App.LastLoginTime.HasValue && !PushSystem.ForegroundInstance.IsHealthy || ContactsPage.PushNagTestHook)
          {
            e.Cancel = ContactsPage.NaggedOnExit = true;
            this.NagPush();
            return;
          }
        }
        base.OnBackKeyPress(e);
      }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
      Log.d("mainpage", nameof (OnLoaded));
      if (!Settings.FirstRegistrationUIShown)
      {
        Settings.FirstRegistrationUIShown = true;
        if (this.favList != null)
        {
          int num;
          this.favList.GetDataReadyObservable().Take<int>(1).ObserveOnDispatcher<int>().Subscribe<int>((Action<int>) (n => num = (int) MessageBox.Show(n > 0 ? AppResources.FoundContacts : AppResources.NoFriends)));
        }
      }
      AppState.CheckDevicePermissions();
      FieldStats.ReportContactsOpen();
    }

    private void MainPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (e == null)
      {
        Log.SendCrashLog((Exception) new NullReferenceException(), "main page pivot event args", logOnlyForRelease: true);
      }
      else
      {
        if (this.chatList != null && this.chatList.IsMultiSelectionEnabled)
          this.chatList.EnableMultiSelection(false);
        bool enable = true;
        Microsoft.Phone.Shell.ApplicationBar appbar = (Microsoft.Phone.Shell.ApplicationBar) null;
        ContactsPage.Tab currTab = this.currTab;
        if (this.chatsPivotItem != null && e.AddedItems.Contains((object) this.chatsPivotItem))
        {
          this.currTab = ContactsPage.Tab.Chats;
          appbar = this.MainAppBar;
        }
        else if (this.statusPivotItem != null && e.AddedItems.Contains((object) this.statusPivotItem))
        {
          this.EnsureStatusTabContent();
          this.statusList.TryReload();
          Settings.LastSeenStatusListTimeUtc = new DateTime?(FunRunner.CurrentServerTimeUtc);
          this.TabHeader.ClearNewStatusIndicator();
          this.currTab = ContactsPage.Tab.Status;
          appbar = this.GetStatusAppBar();
        }
        else if (this.favsPivotItem != null && e.AddedItems.Contains((object) this.favsPivotItem))
        {
          this.favList.Show();
          this.currTab = ContactsPage.Tab.Favorites;
          appbar = this.FavsAppBar;
          FieldStats.ReportUiUsage(wam_enum_ui_usage_type.FAVORITES_VIEWS);
        }
        else if (this.callsPivotItem != null && this.callList != null && e.AddedItems.Contains((object) this.callsPivotItem))
        {
          Settings.LastSeenMissedCallTimeUtc = new DateTime?(FunRunner.CurrentServerTimeUtc);
          this.Dispatcher.BeginInvoke((Action) (() =>
          {
            this.callList.Show();
            this.TabHeader.ClearMissedCalls();
          }));
          this.currTab = ContactsPage.Tab.Calls;
          appbar = this.GetCallsAppBar();
        }
        if (this.callsPivotItem != null && e.RemovedItems.Contains((object) this.callsPivotItem))
          this.Dispatcher.BeginInvoke((Action) (() => this.TabHeader.ClearMissedCalls()));
        else if (this.statusPivotItem != null && e.RemovedItems.Contains((object) this.statusPivotItem))
          this.Dispatcher.BeginInvoke((Action) (() => this.TabHeader.ClearNewStatusIndicator()));
        if (this.chatsPivotItem != null)
          this.chatsPivotItem.Visibility = (this.currTab == ContactsPage.Tab.Chats).ToVisibility();
        if (this.favsPivotItem != null)
          this.favsPivotItem.Visibility = (this.currTab == ContactsPage.Tab.Favorites).ToVisibility();
        if (this.callsPivotItem != null)
          this.callsPivotItem.Visibility = (this.currTab == ContactsPage.Tab.Calls).ToVisibility();
        if (this.statusPivotItem != null)
          this.statusPivotItem.Visibility = (this.currTab == ContactsPage.Tab.Status).ToVisibility();
        UIUtils.EnableAppBar(appbar, enable);
        this.ApplicationBar = (IApplicationBar) appbar;
        Log.d("mainpage", "pivot {0} -> {1}", (object) currTab, (object) this.currTab);
      }
    }

    private void OnSettingsChanged(Settings.Key changedItem)
    {
      if (changedItem == Settings.Key.IsWaAdmin)
        this.UpdateForWaAdminCapability();
      this.TabHeader.Refresh();
    }

    private void QrButton_Click(object sender, EventArgs args)
    {
      this.PerformPageTransition((Action) (() => NavUtils.NavigateToPage(this.NavigationService, "WebSessionsPage")), PageTransitionAnimation.SlideDownFadeOut);
    }

    private void SearchButton_Click(object sender, EventArgs e)
    {
      switch (this.currTab)
      {
        case ContactsPage.Tab.Chats:
          this.SearchChats();
          break;
        case ContactsPage.Tab.Favorites:
          this.SearchFavorites(AppResources.SearchUpper);
          break;
      }
    }

    private void NewCallButton_Click(object sender, EventArgs e)
    {
      try
      {
        CallInfoStruct? callInfo = Voip.Instance.GetCallInfo();
        if (callInfo.HasValue)
        {
          if (callInfo.Value.CallState == CallState.CallActive)
            return;
        }
      }
      catch (Exception ex)
      {
      }
      ListTabData[] tabs = new ListTabData[1];
      WaCallableContactsListTabData contactsListTabData = new WaCallableContactsListTabData();
      contactsListTabData.Header = (string) null;
      tabs[0] = (ListTabData) contactsListTabData;
      JidItemPickerPage.Start(tabs, AppResources.InitiateVoiceCall).ObserveOnDispatcher<List<string>>().Subscribe<List<string>>((Action<List<string>>) (selJids => CallContact.Call(selJids.FirstOrDefault<string>(), true, context: "from main page new call")));
    }

    private void NewVideoCallButton_Click(object sender, EventArgs e)
    {
      try
      {
        CallInfoStruct? callInfo = Voip.Instance.GetCallInfo();
        if (callInfo.HasValue)
        {
          if (callInfo.Value.CallState == CallState.CallActive)
            return;
        }
      }
      catch (Exception ex)
      {
      }
      ListTabData[] tabs = new ListTabData[1];
      WaCallableContactsListTabData contactsListTabData = new WaCallableContactsListTabData();
      contactsListTabData.Header = (string) null;
      tabs[0] = (ListTabData) contactsListTabData;
      JidItemPickerPage.Start(tabs, AppResources.InitiateVideoCall).ObserveOnDispatcher<List<string>>().Subscribe<List<string>>((Action<List<string>>) (selJids => CallContact.VideoCall(selJids.FirstOrDefault<string>(), true)));
    }

    private void NewConvoButton_Click(object sender, EventArgs e)
    {
      this.SearchFavorites(AppResources.NewConvoTitle);
    }

    private void NewGroupButton_Click(object sender, EventArgs e)
    {
      FieldStats.ReportUiUsage(wam_enum_ui_usage_type.NEW_GROUP);
      RecipientListPage.StartContactPicker(AppResources.GroupAddParticipants, (IEnumerable<string>) null, false, (Brush) UIUtils.SelectionBrush, keepPageOnSubmit: true).ObserveOnDispatcher<RecipientListPage.RecipientListResults>().Subscribe<RecipientListPage.RecipientListResults>((Action<RecipientListPage.RecipientListResults>) (recipientListResults =>
      {
        List<string> selectedJids = recipientListResults?.SelectedJids;
        if (selectedJids == null || !selectedJids.Any<string>())
          return;
        GroupCreationPage.Start(selectedJids, true);
      }));
    }

    private void PinChats_Click(object sender, EventArgs e, bool pin)
    {
      List<string> jids = this.chatList.GetMultiSelectedChats();
      this.chatList.EnableMultiSelection(false);
      if (pin)
      {
        Log.l("mainpage", "Pinning multiple chats");
        AppState.Worker.Enqueue((Action) (() => PinChat.Pin(jids.ToArray(), true)));
      }
      else
      {
        Log.l("mainpage", "Unpinning multiple chats");
        AppState.Worker.Enqueue((Action) (() => PinChat.Unpin(jids.ToArray(), true)));
      }
    }

    private void MuteChats_Click(object sender, EventArgs e, bool mute)
    {
      List<string> multiSelectedChats = this.chatList.GetMultiSelectedChats();
      this.chatList.EnableMultiSelection(false);
      if (mute)
      {
        Log.l("mainpage", "Muting multiple chats");
        MuteChatPicker.Launch(multiSelectedChats.ToArray()).Subscribe<Unit>();
      }
      else
      {
        Log.l("mainpage", "Unmuting multiple chats");
        if (multiSelectedChats.Count<string>() == 1)
          MuteChatPicker.Launch(multiSelectedChats.ToArray()).Subscribe<Unit>();
        else
          MuteChat.Mute(multiSelectedChats.ToArray(), new TimeSpan?(), true).SubscribeOn<Unit>((IScheduler) AppState.Worker).Subscribe<Unit>();
      }
    }

    private void MarkAsReadMenuItem_Click(object sender, EventArgs e, bool markAsRead)
    {
      List<string> jids = this.chatList.GetMultiSelectedChats();
      this.chatList.EnableMultiSelection(false);
      if (markAsRead)
      {
        Log.l("mainpage", "Marking multiple chats as read");
        AppState.Worker.Enqueue((Action) (() => MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          foreach (string jid in jids)
            MarkChatRead.MarkRead(db, jid, true, true);
        }))));
      }
      else
      {
        Log.l("mainpage", "Marking multiple chats as unread");
        AppState.Worker.Enqueue((Action) (() => MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          foreach (string jid in jids)
            MarkChatRead.MarkUnread(db, jid, true);
        }))));
      }
    }

    private void ArchiveChats_Click(object sender, EventArgs e)
    {
      List<string> jids = this.chatList.GetMultiSelectedChats();
      this.chatList.EnableMultiSelection(false);
      AppState.Worker.Enqueue((Action) (() =>
      {
        ArchiveChat.Archive(jids.ToArray(), true);
        FieldStats.ReportUiUsage(wam_enum_ui_usage_type.CHAT_ARCHIVE);
      }));
    }

    private void DeleteCallHistory_Click(object sender, EventArgs e)
    {
      if (this.callList == null)
        return;
      this.callList.DeleteAll();
    }

    private void DeleteChats_Click(object sender, EventArgs e)
    {
      List<string> jids = this.chatList.GetMultiSelectedChats().Where<string>((Func<string, bool>) (j => !JidHelper.IsGroupJid(j))).ToList<string>();
      if (!jids.Any<string>())
      {
        this.chatList.EnableMultiSelection(false);
      }
      else
      {
        IObservable<bool> source = Observable.Return<bool>(true);
        if (jids.Count == 1)
        {
          Conversation convo = (Conversation) null;
          MessagesContext.Run((MessagesContext.MessagesCallback) (db => convo = db.GetConversation(jids[0], CreateOptions.None)));
          if (convo != null)
            source = DeleteChat.PromptForDelete(source, convo);
        }
        else
        {
          string message = Plurals.Instance.GetString(AppResources.DeleteMultipleChatsConfirmPlural, jids.Count);
          source = source.Decision(message, AppResources.Delete, AppResources.Cancel);
        }
        source.Take<bool>(1).Subscribe<bool>((Action<bool>) (confirmed =>
        {
          if (!confirmed)
            return;
          this.chatList.EnableMultiSelection(false);
          DeleteChat.Delete(jids.ToArray());
          for (int index = 0; index < jids.Count; ++index)
            FieldStats.ReportUiUsage(wam_enum_ui_usage_type.CHAT_DELETE);
        }));
      }
    }

    private void PrivacySettingsMenuItem_Click(object sender, EventArgs e)
    {
      this.PerformPageTransition((Action) (() => NavUtils.NavigateToPage(this.NavigationService, "StatusPrivacySettingsPage", folderName: "Pages/Settings")), PageTransitionAnimation.TurnstileForwardOut);
    }

    private void SettingsMenuItem_Click(object sender, EventArgs e)
    {
      this.PerformPageTransition((Action) (() => NavUtils.NavigateToPage(this.NavigationService, "SettingsPage")), PageTransitionAnimation.TurnstileForwardOut);
    }

    private void BroadcastListMenuItem_Click(object sender, EventArgs e)
    {
      this.PerformPageTransition((Action) (() => NavUtils.NavigateToPage(this.NavigationService, "BroadcastListMainPage")), PageTransitionAnimation.SlideDownFadeOut);
    }

    private void ArchivedChatsMenuItem_Click(object sender, EventArgs e)
    {
      this.PerformPageTransition((Action) (() => NavUtils.NavigateToPage(this.NavigationService, "ArchivedChatsPage")), PageTransitionAnimation.SlideDownFadeOut);
    }

    private void StarredMessagesMenuItem_Click(object sender, EventArgs e)
    {
      this.PerformPageTransition((Action) (() => StarredMessagesPage.Start((string[]) null)), PageTransitionAnimation.SlideDownFadeOut);
    }

    private void SelectChatsMenuItem_Click(object sender, EventArgs e)
    {
      this.chatList.EnableMultiSelection(true);
    }

    private void RefreshMenuItem_Click(object sender, EventArgs e)
    {
      if (this.refreshFavsSub != null)
        return;
      Action release = (Action) (() =>
      {
        release = (Action) (() => { });
        this.refreshFavsSub.SafeDispose();
        this.refreshFavsSub = (IDisposable) null;
        this.progressIndicator.Release();
      });
      ContactsContext.Instance((Action<ContactsContext>) (db => this.refreshFavsSub = db.StatusUpdateSubject.SubscribeOn<Unit>((IScheduler) AppState.Worker).ObserveOnDispatcher<Unit>().Subscribe<Unit>((Action<Unit>) (count => release()), (Action<Exception>) (ex =>
      {
        int num = (int) MessageBox.Show(AppResources.UpdateFavoritesFail);
        Log.LogException(ex, "settings: update fav list fail");
        release();
      }), release)));
      this.progressIndicator.Acquire();
      ContactStore.SyncInteractive();
    }

    private void ComposeTextStatus() => NavUtils.NavigateToPage("TextStatusComposePage");

    private void LaunchCamera(bool forStatusOnly)
    {
      int imageMaxEdge = Settings.ImageMaxEdge;
      TakePicture.Launch(forStatusOnly ? TakePicture.Mode.StatusOnly : TakePicture.Mode.Regular, true, imageMaxEdge, imageMaxEdge).Take<TakePicture.CapturedPictureArgs>(1).ObserveOnDispatcher<TakePicture.CapturedPictureArgs>().Subscribe<TakePicture.CapturedPictureArgs>((Action<TakePicture.CapturedPictureArgs>) (args =>
      {
        if (args == null)
        {
          MediaPickerPage.preChosenChild = (MediaPickerState.Item) null;
          this.LaunchAlbumsPicker(MediaSharingState.SharingMode.ChooseMedia, (MediaPickerState) null, forStatusOnly);
        }
        else if (args.CameraRollItem != null)
        {
          MediaPickerState sharingState = new MediaPickerState(true, MediaSharingState.SharingMode.ChooseMedia);
          if (forStatusOnly)
            sharingState.RecipientJids = new string[1]
            {
              "status@broadcast"
            };
          if (args.CameraRollItem.GetMediaType() == FunXMPP.FMessage.Type.Video && args.VideoArgs == null)
          {
            MediaPickerState.Item cameraRollItem = args.CameraRollItem;
            string fullPath = cameraRollItem.GetFullPath();
            int num = fullPath.LastIndexOf('.');
            string str = (string) null;
            if (num > 0 && num < fullPath.Length - 1)
              str = fullPath.Substring(num + 1).Trim();
            Stream stream = (Stream) null;
            try
            {
              using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
                stream = nativeMediaStorage.OpenFile(fullPath, FileMode.Open, FileAccess.Read);
            }
            catch (Exception ex)
            {
              Log.LogException(ex, "get video stream for preview");
              stream = (Stream) null;
            }
            if (stream == null)
              return;
            BitmapSource thumbnail = cameraRollItem.GetThumbnail();
            WriteableBitmap writeableBitmap2 = thumbnail == null ? (WriteableBitmap) null : (thumbnail is WriteableBitmap writeableBitmap3 ? writeableBitmap3 : new WriteableBitmap(thumbnail));
            WaVideoArgs waVideoArgs = new WaVideoArgs()
            {
              Stream = stream,
              Thumbnail = writeableBitmap2,
              FullPath = fullPath,
              FileExtension = str ?? "mp4"
            };
            try
            {
              waVideoArgs.Duration = cameraRollItem.GetDuration();
              waVideoArgs.OrientationAngle = -1;
            }
            catch (Exception ex)
            {
              Log.LogException(ex, "creating video args for camera roll video");
            }
            args.CameraRollItem.VideoInfo = waVideoArgs;
          }
          sharingState.AddItem((MediaSharingState.IItem) args.CameraRollItem);
          this.LaunchPicturePreview((MediaSharingState) sharingState, forStatusOnly);
        }
        else
        {
          CameraSharingState sharingState = new CameraSharingState();
          if (forStatusOnly)
            sharingState.RecipientJids = new string[1]
            {
              "status@broadcast"
            };
          if (args.Bitmap != null)
          {
            sharingState.AddItem((MediaSharingState.IItem) new CameraSharingState.Item(args.TempFileStream, args.Bitmap, args.ZoomScale, wam_enum_media_picker_origin_type.MENU_CAMERA_CAPTURE));
            this.LaunchPicturePreview((MediaSharingState) sharingState, forStatusOnly);
          }
          else
          {
            if (args.VideoArgs == null)
              return;
            sharingState.AddItem((MediaSharingState.IItem) new CameraSharingState.Item(args.VideoArgs, args.ZoomScale, wam_enum_media_picker_origin_type.MENU_CAMERA_CAPTURE));
            this.LaunchPicturePreview((MediaSharingState) sharingState, forStatusOnly);
          }
        }
      }));
    }

    private void LaunchPicturePreview(MediaSharingState sharingState, bool forStatusOnly = false)
    {
      PicturePreviewPage.Start(sharingState, true).ObserveOnDispatcher<MediaSharingArgs>().Subscribe<MediaSharingArgs>((Action<MediaSharingArgs>) (args =>
      {
        if (args.Status == MediaSharingArgs.SharingStatus.Canceled)
        {
          this.LaunchCamera(forStatusOnly);
          args.SharingState.SafeDispose();
        }
        else
        {
          if (args.Status != MediaSharingArgs.SharingStatus.Submitted)
            return;
          OptimisticUploadManager optUploadManager = (OptimisticUploadManager) null;
          CameraPage.TakePictureOnly = false;
          int imageMaxEdge = Settings.ImageMaxEdge;
          List<MediaSharingState.PicInfo> picInfos = new List<MediaSharingState.PicInfo>();
          List<WaVideoArgs> videoInfo = new List<WaVideoArgs>();
          foreach (MediaSharingState.IItem selectedItem in args.SharingState.SelectedItems)
          {
            if (selectedItem.GetMediaType() == FunXMPP.FMessage.Type.Image)
            {
              if (MediaUpload.OptimisticUploadAllowed && optUploadManager == null)
                optUploadManager = new OptimisticUploadManager(wam_enum_opt_upload_context_type.CHAT_LIST_CAMERA);
              MediaSharingState.PicInfo picInfo = selectedItem.ToPicInfo(new Size((double) imageMaxEdge, (double) imageMaxEdge));
              picInfos.Add(picInfo);
              if (!picInfo.isChangedByUser() && optUploadManager != null)
                optUploadManager.AddToOptimisticUploadQueue(picInfo, selectedItem.GetFullPath());
            }
            else
            {
              if (selectedItem.GetMediaType() == FunXMPP.FMessage.Type.Gif && selectedItem.GifInfo != null)
              {
                Stream stream = (Stream) null;
                using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
                  stream = nativeMediaStorage.OpenFile(selectedItem.GetFullPath(), FileMode.Open, FileAccess.Read);
                WriteableBitmap bitmap1 = selectedItem.GetBitmap(new Size((double) imageMaxEdge, (double) imageMaxEdge));
                WaVideoArgs waVideoArgs = new WaVideoArgs()
                {
                  FileExtension = ".gif",
                  ContentType = "image/gif",
                  FullPath = selectedItem.GetFullPath(),
                  Stream = stream,
                  CodecInfo = CodecInfo.NeedsTranscode,
                  TranscodeReason = TranscodeReason.BadCodec,
                  LargeThumbnail = bitmap1,
                  Thumbnail = bitmap1,
                  OrientationAngle = selectedItem.RotatedTimes * -90,
                  LoopingPlayback = true,
                  TimeCrop = selectedItem.GifInfo.TimeCrop
                };
                if (selectedItem.RelativeCropPos.HasValue)
                {
                  WriteableBitmap bitmap2 = selectedItem.GetBitmap(new Size((double) imageMaxEdge, (double) imageMaxEdge), false, false);
                  CropRectangle cropRectangle = new CropRectangle()
                  {
                    Height = (int) ((double) bitmap2.PixelHeight * selectedItem.RelativeCropSize.Value.Height),
                    Width = (int) ((double) bitmap2.PixelWidth * selectedItem.RelativeCropSize.Value.Width),
                    XOffset = (int) (selectedItem.RelativeCropPos.Value.X * (double) bitmap2.PixelWidth),
                    YOffset = (int) (selectedItem.RelativeCropPos.Value.Y * (double) bitmap2.PixelHeight)
                  };
                  waVideoArgs.CropRectangle = new CropRectangle?(cropRectangle);
                }
                selectedItem.VideoInfo = waVideoArgs;
              }
              videoInfo.Add(selectedItem.VideoInfo);
            }
          }
          Action<RecipientListPage.RecipientListResults> onNext = (Action<RecipientListPage.RecipientListResults>) (recipientListResults =>
          {
            List<string> selJids = recipientListResults?.SelectedJids;
            Log.l("mainpage", "Recipients selected: {0}", (object) (selJids == null ? -1 : selJids.Count));
            if (selJids == null || selJids.Count < 1)
            {
              optUploadManager.SafeDispose();
              optUploadManager = (OptimisticUploadManager) null;
              MediaSharingState newState = args.SharingState.recreateForRestart();
              if (newState != null)
              {
                Log.l("mainpage", "Restarting Picture Preview");
                this.Dispatcher.BeginInvoke((Action) (() => this.LaunchPicturePreview(newState, forStatusOnly)));
              }
              else
                Log.l("mainpage", "No recipients selected and restart not supported");
            }
            else
            {
              int thumbnailWidth = MessageViewModel.LargeThumbPixelWidth;
              Dictionary<string, OptimisticUpload> optimisticUploads = optUploadManager?.RetrieveOptimisticUploads();
              WAThreadPool.QueueUserWorkItem((Action) (() =>
              {
                bool flag = selJids.Contains("status@broadcast");
                int count3 = picInfos.Count;
                for (int index = 0; index < count3; ++index)
                {
                  Log.l("mainpage", "processing outgoing image {0}/{1}", (object) (index + 1), (object) count3);
                  MediaSharingState.PicInfo pi = picInfos[index];
                  Stream stream = (Stream) null;
                  WriteableBitmap picture = (WriteableBitmap) null;
                  if (pi.isChangedByUser())
                  {
                    stream = (Stream) new NativeMediaStorage().GetTempFile();
                    this.Dispatcher.InvokeSynchronous((Action) (() =>
                    {
                      picture = pi.DrawingBitmapCache ?? pi.GetBitmap(withDrawing: true);
                      picture.SaveJpeg(stream, picture.PixelWidth, picture.PixelHeight, 0, Settings.JpegQuality);
                    }));
                    stream.Position = 0L;
                  }
                  using (Stream jpegStream = stream ?? pi.GetImageStream())
                  {
                    if (jpegStream != null)
                    {
                      if (args.SharingState.Mode == MediaSharingState.SharingMode.TakePicture)
                      {
                        MediaDownload.SaveMediaToCameraRoll(pi.PathForDb, FunXMPP.FMessage.Type.Image, saveAlbum: "Camera Roll");
                        pi.PathForDb = (string) null;
                      }
                      OptimisticUpload optimisticUpload = (OptimisticUpload) null;
                      if (optimisticUploads != null && optimisticUploads.TryGetValue(pi.GetOptimisticUniqueId(), out optimisticUpload) && !optimisticUpload.SetUseFileOnServer())
                        optimisticUpload = (OptimisticUpload) null;
                      Action<Message> messageModifier = (Action<Message>) (msg =>
                      {
                        if (optimisticUpload == null || optimisticUpload.OptimisticMessage == null)
                          return;
                        msg.UploadContext = optimisticUpload.OptimisticMessage.UploadContext;
                        msg.MediaKey = optimisticUpload.OptimisticMessage.MediaKey;
                        msg.MediaUploadUrl = optimisticUpload.OptimisticMessage.MediaUploadUrl;
                      });
                      MediaUpload.SendPicture(selJids, jpegStream, thumbnailWidth, picture, pi.PathForDb, pi.Caption, messageModifier);
                      if (flag)
                        FieldStats.ReportFsStatusPostEvent(FunXMPP.FMessage.Type.Image, forStatusOnly ? wam_enum_status_post_origin.STATUS_TAB : wam_enum_status_post_origin.CAMERA_TAB, wam_enum_status_post_result.OK);
                    }
                  }
                  picInfos[index] = pi = (MediaSharingState.PicInfo) null;
                  Log.l(string.Format("contacts page: processed outgoing image {0}/{1}", (object) (index + 1), (object) count3), (object) false);
                }
                int count4 = videoInfo.Count;
                for (int index = 0; index < count4; ++index)
                {
                  Log.l(string.Format("contacts page: processing outgoing video {0}/{1}", (object) (index + 1), (object) count4), (object) true);
                  string filename = !videoInfo[index].IsCameraVideo || videoInfo[index].FullPath == null ? videoInfo[index].PreviewPlayPath : NativeMediaStorage.MakeUri(videoInfo[index].FullPath);
                  try
                  {
                    Mp4Atom.OrientationMatrix orientationMatrix = VideoFrameGrabber.MatrixForAngle(videoInfo[index].OrientationAngle);
                    if (orientationMatrix != null)
                    {
                      if (!videoInfo[index].ShouldTranscode)
                        VideoFrameGrabber.WriteRotationMatrix(filename, orientationMatrix.Matrix);
                    }
                  }
                  catch (Exception ex)
                  {
                    Log.LogException(ex, "Failed to rotate video");
                  }
                  MediaUpload.SendVideo(selJids, videoInfo[index]);
                  if (flag)
                    FieldStats.ReportFsStatusPostEvent(FunXMPP.FMessage.Type.Video, forStatusOnly ? wam_enum_status_post_origin.STATUS_TAB : wam_enum_status_post_origin.CAMERA_TAB, wam_enum_status_post_result.OK);
                }
                args.SharingState.SafeDispose();
                if (optimisticUploads != null)
                {
                  foreach (IDisposable d in optimisticUploads.Values)
                    d.SafeDispose();
                }
                optUploadManager.SafeDispose();
                optUploadManager = (OptimisticUploadManager) null;
              }));
            }
          });
          Action<Exception> onError = (Action<Exception>) (ex =>
          {
            Log.LogException(ex, "Exception selecting mutlicast targets");
            optUploadManager.SafeDispose();
            optUploadManager = (OptimisticUploadManager) null;
          });
          Action onCompleted = (Action) (() =>
          {
            optUploadManager.SafeDispose();
            optUploadManager = (OptimisticUploadManager) null;
          });
          if (forStatusOnly)
          {
            this.NavigationService.JumpBackTo(nameof (ContactsPage), fallbackToHome: true);
            onNext(new RecipientListPage.RecipientListResults(new List<string>()
            {
              "status@broadcast"
            }));
            onCompleted();
          }
          else
          {
            IEnumerable<int> source = args.SharingState.SelectedItems.Select<MediaSharingState.IItem, int>((Func<MediaSharingState.IItem, int>) (item => item.GetMediaType() != FunXMPP.FMessage.Type.Video ? 0 : item.GetDuration()));
            int maxDuration = source.Any<int>() ? source.Max() : 0;
            RecipientListPage.StartRecipientPicker(AppResources.Send, args.SharingState.SelectedItems.FirstOrDefault<MediaSharingState.IItem>()?.GetMediaType(), clearStack: true, maxDuration: maxDuration).ObserveOnDispatcher<RecipientListPage.RecipientListResults>().Subscribe<RecipientListPage.RecipientListResults>(onNext, onError, onCompleted);
          }
        }
      }));
    }

    private void LaunchAlbumsPicker(
      MediaSharingState.SharingMode mode,
      MediaPickerState state,
      bool forStatusOnly)
    {
      if (mode == MediaSharingState.SharingMode.TakePicture)
        return;
      IObservable<MediaSharingArgs> source = (IObservable<MediaSharingArgs>) null;
      if (state != null)
      {
        source = MediaPickerPage.Start(state);
      }
      else
      {
        switch (mode)
        {
          case MediaSharingState.SharingMode.ChooseMedia:
            source = MediaPickerPage.Start(new MediaPickerState(true, MediaSharingState.SharingMode.ChooseMedia));
            break;
          case MediaSharingState.SharingMode.ChoosePicture:
            source = MediaPickerPage.Start(new MediaPickerState(true, MediaSharingState.SharingMode.ChoosePicture));
            break;
          case MediaSharingState.SharingMode.ChooseVideo:
            source = MediaPickerPage.Start(new MediaPickerState(true, MediaSharingState.SharingMode.ChooseVideo));
            break;
        }
      }
      if (source == null)
        return;
      source.Subscribe<MediaSharingArgs>((Action<MediaSharingArgs>) (args =>
      {
        int num1 = args.Status == MediaSharingArgs.SharingStatus.Canceled ? 1 : 0;
        MediaPickerState.Item obj = args.SharingState.SelectedItems.FirstOrDefault<MediaSharingState.IItem>() as MediaPickerState.Item;
        if (num1 != 0 || obj == null)
        {
          NavUtils.NavigateBackToChat(args.NavService);
        }
        else
        {
          foreach (MediaSharingState.IItem selectedItem in args.SharingState.SelectedItems)
          {
            if (selectedItem.GetMediaType() == FunXMPP.FMessage.Type.Video && selectedItem.VideoInfo == null)
            {
              string fullPath = selectedItem.GetFullPath();
              int num2 = fullPath.LastIndexOf('.');
              string str = (string) null;
              if (num2 > 0 && num2 < fullPath.Length - 1)
                str = fullPath.Substring(num2 + 1).Trim();
              Stream stream = (Stream) null;
              try
              {
                using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
                  stream = nativeMediaStorage.OpenFile(fullPath, FileMode.Open, FileAccess.Read);
              }
              catch (Exception ex)
              {
                Log.LogException(ex, "get video stream for preview");
                stream = (Stream) null;
              }
              if (stream == null)
                return;
              BitmapSource thumbnail = selectedItem.GetThumbnail();
              WriteableBitmap writeableBitmap2 = thumbnail == null ? (WriteableBitmap) null : (thumbnail is WriteableBitmap writeableBitmap3 ? writeableBitmap3 : new WriteableBitmap(thumbnail));
              WaVideoArgs waVideoArgs = new WaVideoArgs()
              {
                Stream = stream,
                Thumbnail = writeableBitmap2,
                FullPath = fullPath,
                FileExtension = str ?? "mp4"
              };
              try
              {
                waVideoArgs.Duration = selectedItem.GetDuration();
                waVideoArgs.OrientationAngle = -1;
              }
              catch (Exception ex)
              {
                Log.LogException(ex, "creating video args for album video");
              }
              selectedItem.VideoInfo = waVideoArgs;
            }
          }
          MediaPickerPage.preChosenChild = (MediaPickerState.Item) null;
          if (forStatusOnly)
            args.SharingState.RecipientJids = new string[1]
            {
              "status@broadcast"
            };
          this.LaunchPicturePreview(args.SharingState, forStatusOnly);
        }
      }));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/ContactsPage.xaml", UriKind.Relative));
      this.LayoutRootZoomBox = (ZoomBox) this.FindName("LayoutRootZoomBox");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.Pivot = (Pivot) this.FindName("Pivot");
    }

    public enum Tab
    {
      Chats,
      Calls,
      Favorites,
      Status,
    }
  }
}
