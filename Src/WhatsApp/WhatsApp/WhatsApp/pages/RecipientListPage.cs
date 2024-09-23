// Decompiled with JetBrains decompiler
// Type: WhatsApp.RecipientListPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using WhatsApp.WaCollections;
using WhatsApp.WaViewModels;


namespace WhatsApp
{
  public class RecipientListPage : PhoneApplicationPage
  {
    private const string LogHeader = "recipient picker";
    private const int FrequentCount = 3;
    private int initialShowBatch = 10;
    private int partialShowBatch = 20;
    private static RecipientListPage.InitState nextInstanceIniteState = (RecipientListPage.InitState) null;
    private bool isPageLoaded;
    private bool isPageRemoved;
    private bool isShown;
    private bool keepPageOnSubmit;
    private RecipientListPage.Mode pageMode;
    private string pageTitle;
    private string selectedCountFormat;
    private SelfStatusThreadViewModel statusViewModel;
    private ObservableCollection<KeyedObservableCollection<RecipientListPage.RecipientGrouping, RecipientItemViewModel>> mainListSrc;
    private RecipientListPage.RecipientItemsCache vmsCache = new RecipientListPage.RecipientItemsCache();
    private ObservableCollection<JidItemViewModel> selectedListSrc = new ObservableCollection<JidItemViewModel>();
    private HashSet<string> selectedJids = new HashSet<string>();
    private HashSet<string> readOnlyJids = new HashSet<string>();
    private string subtitleOverride;
    private FunXMPP.FMessage.Type sendingType;
    private string[] excludedJids;
    private IObserver<RecipientListPage.RecipientListResults> resultsObserver;
    private Storyboard slideDownSb;
    private KeyedObservableCollection<RecipientListPage.RecipientGrouping, RecipientItemViewModel> visibleRecents;
    private KeyedObservableCollection<RecipientListPage.RecipientGrouping, RecipientItemViewModel> visibleGroups;
    private List<ConversationItem> hiddenRecents;
    private List<RecipientItemViewModel> hiddenGroups;
    private List<KeyedObservableCollection<RecipientListPage.RecipientGrouping, RecipientItemViewModel>> phoneContacts;
    private List<RecipientItemViewModel> allGroups;
    private ObservableCollection<KeyedObservableCollection<RecipientListPage.RecipientGrouping, RecipientItemViewModel>> resultListSrc = new ObservableCollection<KeyedObservableCollection<RecipientListPage.RecipientGrouping, RecipientItemViewModel>>();
    private KeyedObservableCollection<RecipientListPage.RecipientGrouping, RecipientItemViewModel> chatResults;
    private KeyedObservableCollection<RecipientListPage.RecipientGrouping, RecipientItemViewModel> contactResults;
    private DateTime lastTextChangedAt = DateTime.UtcNow;
    private IDisposable delaySub;
    private List<IDisposable> disposables = new List<IDisposable>();
    private object currSearchLock = new object();
    private string currQuery;
    private string currFtsQuery;
    private IDisposable currSearchSub;
    private GlobalProgressIndicator progressIndicator;
    private bool isDataFullyLoaded;
    private bool isDataFullySuppressedSearch;
    private HashSet<RecipientListPage.RecipientItemType> filteredRecipientTypes;
    private IObservable<bool> statusVisibilityObservable;
    private int maxDurationOfVideo;
    private bool isRestrictedForForward;
    private bool isRestrictedForShareContact;
    private Brush selectionBackground;
    private System.Windows.Media.ImageSource selectionIcon;
    private Func<IEnumerable<string>, IObservable<bool>> confirmSelectionFunc;
    private const int searchStallTimeMs = 500;
    private static readonly TimeSpan searchStallTimeSpan = TimeSpan.FromMilliseconds(500.0);
    internal ZoomBox LayoutRootZoomBox;
    internal Grid LayoutRoot;
    internal PageTitlePanel PageTitle;
    internal TextBlock SelectionSummaryBlock;
    internal EmojiTextBox SearchField;
    internal ListBox SelectedList;
    internal WhatsApp.CompatibilityShims.LongListSelector MainList;
    internal WaSelfStatusItemControl StatusControl;
    internal Grid PrivacyButton;
    internal Grid StatusTooltipPanel;
    internal TextBlock StatusTooltipBlock;
    internal ProgressBar LoadingProgressBar;
    internal Grid SearchResultsPanel;
    internal WhatsApp.CompatibilityShims.LongListSelector ResultList;
    internal TextBlock ResultListFooterBlock;
    internal Grid BottomBar;
    internal RoundButton SubmitButton;
    internal Grid RecipientsPreview;
    internal ScrollViewer SelectedScrollViewer;
    internal RichTextBlock SelectedTextBlock;
    internal Button RightButton;
    private bool _contentLoaded;

    public RecipientListPage()
    {
      this.InitializeComponent();
      this.progressIndicator = new GlobalProgressIndicator((DependencyObject) this);
      this.LayoutRootZoomBox.ZoomFactor = ResolutionHelper.ZoomFactor;
      this.MainList.OverlapScrollBar = true;
      this.ResultList.OverlapScrollBar = true;
      RecipientListPage.InitState initState = RecipientListPage.nextInstanceIniteState ?? new RecipientListPage.InitState();
      RecipientListPage.nextInstanceIniteState = (RecipientListPage.InitState) null;
      this.resultsObserver = initState.Observer;
      this.sendingType = initState.SendingType;
      this.pageTitle = initState.Title;
      if (this.pageTitle != null)
        this.PageTitle.SmallTitle = this.pageTitle;
      else
        this.PageTitle.Visibility = Visibility.Collapsed;
      this.isRestrictedForForward = AppResources.Forward == this.pageTitle;
      this.isRestrictedForShareContact = AppResources.ShareContact == this.pageTitle;
      this.excludedJids = ((IEnumerable<string>) new string[2]
      {
        "0@s.whatsapp.net",
        "status@broadcast"
      }).Concat<string>((IEnumerable<string>) (initState.ExcludedJids ?? new string[0])).ToArray<string>();
      this.filteredRecipientTypes = initState.FilteredRecipientTypes;
      if (initState.PreSelections != null)
      {
        foreach (string preSelection in initState.PreSelections)
        {
          if (!this.selectedJids.Contains(preSelection))
          {
            this.selectedJids.Add(preSelection);
            IEnumerable<string> readOnlyJid = initState.ReadOnlyJid;
            if ((readOnlyJid != null ? (!readOnlyJid.Contains<string>(preSelection) ? 1 : 0) : 1) != 0)
              this.selectedListSrc.Add(new JidItemViewModel(preSelection));
          }
        }
      }
      if (initState.ReadOnlyJid != null)
      {
        foreach (string str in initState.ReadOnlyJid)
        {
          if (!this.readOnlyJids.Contains(str))
            this.readOnlyJids.Add(str);
        }
      }
      this.pageMode = initState.Mode;
      this.statusVisibilityObservable = initState.StatusVisibilityObservable;
      this.selectionBackground = initState.SelectionBackground;
      this.selectionIcon = initState.SelectionIcon;
      this.selectedCountFormat = initState.SelectedCountFormat;
      this.keepPageOnSubmit = initState.KeepPageOnSubmit;
      this.maxDurationOfVideo = initState.MaxDurationOfVideo;
      this.confirmSelectionFunc = initState.ConfirmSelectionFunc;
      this.subtitleOverride = initState.SubtitleOverride;
      this.SearchField.EnableActionButton = false;
      switch (this.pageMode)
      {
        case RecipientListPage.Mode.MultiPicker:
          this.SubmitButton.Visibility = Visibility.Visible;
          this.SelectedList.Visibility = Visibility.Visible;
          this.SelectedList.ItemsSource = (IEnumerable) this.selectedListSrc;
          this.BottomBar.Visibility = this.selectedListSrc.Any<JidItemViewModel>().ToVisibility();
          if (this.selectedCountFormat != null)
          {
            this.SelectionSummaryBlock.Text = Plurals.Instance.GetString(this.selectedCountFormat, this.selectedListSrc.Count);
            break;
          }
          break;
        default:
          this.RightButton.Content = (object) AppResources.Send;
          this.RecipientsPreview.Visibility = this.RightButton.Visibility = Visibility.Visible;
          this.SelectedList.Visibility = Visibility.Collapsed;
          this.BottomBar.Visibility = this.selectedJids.Any<string>().ToVisibility();
          break;
      }
      if (this.resultsObserver == null)
        return;
      this.disposables.Add(this.SearchField.GetTextChangedAsync().ObserveOnDispatcher<TextChangedEventArgs>().Subscribe<TextChangedEventArgs>(new Action<TextChangedEventArgs>(this.SearchField_TextChanged)));
      this.Loaded += new RoutedEventHandler(this.OnLoaded);
      this.LoadOnPageInit();
    }

    private static IObservable<RecipientListPage.RecipientListResults> StartImpl(
      string title,
      RecipientListPage.Mode mode,
      FunXMPP.FMessage.Type? type,
      HashSet<RecipientListPage.RecipientItemType> filteredRecipientTypes,
      string[] excludedJids,
      IEnumerable<string> preSelected,
      NavUtils.BackStackOps backStackOp,
      bool keepPageOnSubmit,
      Brush selectionBackground,
      System.Windows.Media.ImageSource selectionIcon,
      string selectionCountFormat,
      IObservable<bool> statusVisibilityObservable,
      int maxDurationOfVideo,
      IEnumerable<string> readOnlySelected,
      Func<IEnumerable<string>, IObservable<bool>> confirmSelectionFunc,
      string subtitleOverride)
    {
      return Observable.Create<RecipientListPage.RecipientListResults>((Func<IObserver<RecipientListPage.RecipientListResults>, Action>) (observer =>
      {
        RecipientListPage.nextInstanceIniteState = new RecipientListPage.InitState()
        {
          Observer = observer,
          Title = title,
          Mode = mode,
          SendingType = (FunXMPP.FMessage.Type) ((int) type ?? 0),
          FilteredRecipientTypes = filteredRecipientTypes,
          ExcludedJids = excludedJids,
          PreSelections = preSelected,
          StatusVisibilityObservable = statusVisibilityObservable,
          SelectionBackground = selectionBackground,
          SelectionIcon = selectionIcon,
          SelectedCountFormat = selectionCountFormat,
          KeepPageOnSubmit = keepPageOnSubmit,
          MaxDurationOfVideo = maxDurationOfVideo,
          ReadOnlyJid = readOnlySelected,
          ConfirmSelectionFunc = confirmSelectionFunc,
          SubtitleOverride = subtitleOverride
        };
        WaUriParams uriParams = new WaUriParams();
        switch (backStackOp)
        {
          case NavUtils.BackStackOps.ReplaceOne:
            uriParams.AddBool("PageReplace", true);
            break;
          case NavUtils.BackStackOps.ClearAll:
            uriParams.AddBool("ClearStack", true);
            break;
        }
        NavUtils.NavigateToPage(nameof (RecipientListPage), uriParams);
        return (Action) (() => { });
      }));
    }

    public static IObservable<RecipientListPage.RecipientListResults> StartContactPicker(
      string title,
      IEnumerable<string> preSelected,
      bool replacePage,
      Brush selectionBackground = null,
      System.Windows.Media.ImageSource selectionIcon = null,
      string selectionCountFormat = null,
      bool keepPageOnSubmit = false,
      IEnumerable<string> readOnlySelected = null,
      Func<IEnumerable<string>, IObservable<bool>> confirmSelectionFunc = null,
      string subtitleOverride = null)
    {
      string title1 = title;
      FunXMPP.FMessage.Type? type = new FunXMPP.FMessage.Type?();
      HashSet<RecipientListPage.RecipientItemType> filteredRecipientTypes = new HashSet<RecipientListPage.RecipientItemType>();
      filteredRecipientTypes.Add(RecipientListPage.RecipientItemType.WaContact);
      IEnumerable<string> preSelected1 = preSelected;
      int backStackOp = replacePage ? 1 : 0;
      int num = keepPageOnSubmit ? 1 : 0;
      Brush selectionBackground1 = selectionBackground;
      System.Windows.Media.ImageSource selectionIcon1 = selectionIcon;
      string selectionCountFormat1 = selectionCountFormat;
      IEnumerable<string> readOnlySelected1 = readOnlySelected;
      Func<IEnumerable<string>, IObservable<bool>> confirmSelectionFunc1 = confirmSelectionFunc;
      string subtitleOverride1 = subtitleOverride;
      return RecipientListPage.StartImpl(title1, RecipientListPage.Mode.MultiPicker, type, filteredRecipientTypes, (string[]) null, preSelected1, (NavUtils.BackStackOps) backStackOp, num != 0, selectionBackground1, selectionIcon1, selectionCountFormat1, (IObservable<bool>) null, 0, readOnlySelected1, confirmSelectionFunc1, subtitleOverride1);
    }

    public static IObservable<RecipientListPage.RecipientListResults> StartPhoneContactPicker(
      string title,
      Brush selectionBackground,
      string selectionCountFormat)
    {
      return Observable.Create<RecipientListPage.RecipientListResults>((Func<IObserver<RecipientListPage.RecipientListResults>, Action>) (observer =>
      {
        RecipientListPage.nextInstanceIniteState = new RecipientListPage.InitState()
        {
          Observer = observer,
          Title = title,
          Mode = RecipientListPage.Mode.MultiPicker,
          SendingType = FunXMPP.FMessage.Type.Undefined,
          FilteredRecipientTypes = new HashSet<RecipientListPage.RecipientItemType>()
          {
            RecipientListPage.RecipientItemType.Contact
          },
          ExcludedJids = (string[]) null,
          PreSelections = (IEnumerable<string>) null,
          SelectionBackground = selectionBackground,
          SelectionIcon = (System.Windows.Media.ImageSource) null,
          SelectedCountFormat = selectionCountFormat,
          StatusVisibilityObservable = (IObservable<bool>) null
        };
        NavUtils.NavigateToPage(nameof (RecipientListPage), new WaUriParams());
        return (Action) (() => { });
      }));
    }

    public static IObservable<RecipientListPage.RecipientListResults> StartRecipientPicker(
      string title,
      FunXMPP.FMessage.Type? type,
      string sourceJid = null,
      bool clearStack = false,
      IObservable<bool> statusVisibilityObservable = null,
      int maxDuration = 0)
    {
      return RecipientListPage.StartImpl(title, RecipientListPage.Mode.MessageSending, type, (HashSet<RecipientListPage.RecipientItemType>) null, new string[1]
      {
        sourceJid
      }, (IEnumerable<string>) null, (NavUtils.BackStackOps) (clearStack ? 2 : 0), false, (Brush) null, (System.Windows.Media.ImageSource) null, (string) null, statusVisibilityObservable, maxDuration, (IEnumerable<string>) null, (Func<IEnumerable<string>, IObservable<bool>>) null, (string) null);
    }

    private static HashSet<RecipientListPage.RecipientItemType> GetAllRecipientTypes()
    {
      return new HashSet<RecipientListPage.RecipientItemType>()
      {
        RecipientListPage.RecipientItemType.Status,
        RecipientListPage.RecipientItemType.Frequent,
        RecipientListPage.RecipientItemType.Recent,
        RecipientListPage.RecipientItemType.Group,
        RecipientListPage.RecipientItemType.WaContact
      };
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);
      if (this.resultsObserver == null)
      {
        Log.l("recipient picker", "Exiting - observer is null!");
        this.Dispatcher.BeginInvoke((Action) (() => NavUtils.GoBack(this.NavigationService)));
      }
      else
      {
        if (this.statusViewModel == null)
          return;
        this.Dispatcher.BeginInvoke((Action) (() => this.statusViewModel.Refresh()));
      }
    }

    private void SlideDownAndBackOut(Action exitAction)
    {
      if (this.slideDownSb == null)
        this.slideDownSb = WaAnimations.PageTransition(PageTransitionAnimation.SlideDownFadeOut);
      Storyboarder.Perform(this.slideDownSb, (DependencyObject) this.LayoutRoot, false, (Action) (() =>
      {
        Log.l("recipient picker", "Leaving");
        NavUtils.GoBack(this.NavigationService);
        exitAction();
      }));
    }

    private bool ShouldEnableRecipientType(RecipientListPage.RecipientItemType type)
    {
      return type == RecipientListPage.RecipientItemType.Contact ? this.filteredRecipientTypes != null && this.filteredRecipientTypes.Contains(type) : this.filteredRecipientTypes == null || !this.filteredRecipientTypes.Any<RecipientListPage.RecipientItemType>() || this.filteredRecipientTypes.Contains(type);
    }

    private void LoadOnPageInit()
    {
      if (!this.ShouldEnableRecipientType(RecipientListPage.RecipientItemType.Frequent) && !this.ShouldEnableRecipientType(RecipientListPage.RecipientItemType.Recent))
        this.mainListSrc = new ObservableCollection<KeyedObservableCollection<RecipientListPage.RecipientGrouping, RecipientItemViewModel>>();
      else
        WAThreadPool.Scheduler.Schedule((Action) (() =>
        {
          Log.d("recipient picker", "LoadOnPageInit recent/frequent");
          List<Conversation> frequentChats = (List<Conversation>) null;
          List<ConversationItem> recentChats = (List<ConversationItem>) null;
          MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
          {
            if (this.ShouldEnableRecipientType(RecipientListPage.RecipientItemType.Frequent))
            {
              string[] frequentChats1 = db.GetFrequentChats(this.sendingType, new int?(5), this.excludedJids);
              if (((IEnumerable<string>) frequentChats1).Any<string>())
                frequentChats = ((IEnumerable<string>) frequentChats1).Select<string, Conversation>((Func<string, Conversation>) (jid =>
                {
                  Conversation conversation = (Conversation) null;
                  switch (JidHelper.GetJidType(jid))
                  {
                    case JidHelper.JidTypes.User:
                    case JidHelper.JidTypes.Group:
                      conversation = db.GetConversation(jid, CreateOptions.None);
                      break;
                  }
                  return conversation;
                })).Where<Conversation>((Func<Conversation, bool>) (c => c != null)).Take<Conversation>(3).ToList<Conversation>();
            }
            if (!this.ShouldEnableRecipientType(RecipientListPage.RecipientItemType.Recent))
              return;
            recentChats = db.GetConversationItems(new JidHelper.JidTypes[2]
            {
              JidHelper.JidTypes.Group,
              JidHelper.JidTypes.User
            }, true, SqliteMessagesContext.ConversationSortTypes.TimestampOnly, new int?(this.initialShowBatch));
          }));
          Log.d("recipient picker", "LoadOnPageInit frequent vms");
          KeyedObservableCollection<RecipientListPage.RecipientGrouping, RecipientItemViewModel> frequentVms = (KeyedObservableCollection<RecipientListPage.RecipientGrouping, RecipientItemViewModel>) null;
          if (frequentChats != null && frequentChats.Any<Conversation>())
            frequentVms = new KeyedObservableCollection<RecipientListPage.RecipientGrouping, RecipientItemViewModel>(new RecipientListPage.RecipientGrouping(AppResources.FrequentlyContacted, RecipientListPage.RecipientItemType.Frequent), frequentChats.Select<Conversation, RecipientItemViewModel>((Func<Conversation, RecipientItemViewModel>) (c => this.vmsCache.Get(c.Jid, RecipientListPage.RecipientItemType.Frequent, true, (Func<RecipientItemViewModel>) (() => this.CreateItemViewModel(c, (UserStatus) null, (Message) null, (Contact) null, RecipientListPage.RecipientItemType.Frequent))))));
          Log.d("recipient picker", "LoadOnPageInit recent chats");
          KeyedObservableCollection<RecipientListPage.RecipientGrouping, RecipientItemViewModel> recentVms = (KeyedObservableCollection<RecipientListPage.RecipientGrouping, RecipientItemViewModel>) null;
          if (recentChats != null && recentChats.Any<ConversationItem>())
            recentVms = new KeyedObservableCollection<RecipientListPage.RecipientGrouping, RecipientItemViewModel>(new RecipientListPage.RecipientGrouping(AppResources.RecentHeader, RecipientListPage.RecipientItemType.Recent), recentChats.Select<ConversationItem, RecipientItemViewModel>((Func<ConversationItem, RecipientItemViewModel>) (ci => this.vmsCache.Get(ci.Jid, RecipientListPage.RecipientItemType.Recent, true, (Func<RecipientItemViewModel>) (() => this.CreateItemViewModel(ci.Conversation, (UserStatus) null, ci.Message, (Contact) null, RecipientListPage.RecipientItemType.Recent))))));
          this.Dispatcher.BeginInvoke((Action) (() =>
          {
            if (this.isPageRemoved)
              return;
            Log.d("recipient picker", "LoadOnPageInit main Src");
            this.mainListSrc = new ObservableCollection<KeyedObservableCollection<RecipientListPage.RecipientGrouping, RecipientItemViewModel>>();
            if (frequentVms != null && frequentVms.Any<RecipientItemViewModel>())
              this.mainListSrc.Add(frequentVms);
            if (recentVms != null && recentVms.Any<RecipientItemViewModel>())
            {
              this.visibleRecents = recentVms;
              this.mainListSrc.Add(this.visibleRecents);
            }
            this.TryShow();
          }));
        }));
    }

    private void LoadOnPageLoaded()
    {
      if (this.isPageRemoved)
        return;
      WAThreadPool.Scheduler.Schedule((Action) (() =>
      {
        if (this.isPageRemoved)
          return;
        if (this.ShouldEnableRecipientType(RecipientListPage.RecipientItemType.Group))
        {
          Log.d("recipient picker", "LoadOnPageLoaded groups");
          this.LoadInitialGroups();
        }
        if (this.ShouldEnableRecipientType(RecipientListPage.RecipientItemType.WaContact))
        {
          Log.d("recipient picker", "LoadOnPageLoaded user Statuses");
          this.LoadUserStatuses();
        }
        if (this.ShouldEnableRecipientType(RecipientListPage.RecipientItemType.Contact))
        {
          Log.d("recipient picker", "LoadOnPageLoaded phone contacts");
          this.LoadPhoneContacts();
        }
        this.Dispatcher.BeginInvoke((Action) (() =>
        {
          if (this.isPageRemoved)
            return;
          this.LoadingProgressBar.Visibility = Visibility.Collapsed;
          this.LoadRemainings();
        }));
      }));
    }

    private void LoadRemainings()
    {
      if (this.isPageRemoved)
        return;
      bool loadMoreRecents = this.ShouldEnableRecipientType(RecipientListPage.RecipientItemType.Recent) && this.visibleRecents != null && this.visibleRecents.Count >= this.initialShowBatch;
      bool loadMoreGroups = this.ShouldEnableRecipientType(RecipientListPage.RecipientItemType.Group) && this.visibleGroups != null;
      List<RecipientItemViewModel> groupsSnap = loadMoreGroups ? this.visibleGroups.ToList<RecipientItemViewModel>() : new List<RecipientItemViewModel>();
      WAThreadPool.Scheduler.Schedule((Action) (() =>
      {
        Log.d("recipient picker", "LoadRemainings recentChats");
        List<ConversationItem> recentChats = (List<ConversationItem>) null;
        List<Conversation> groups = (List<Conversation>) null;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          if (loadMoreRecents)
            recentChats = db.GetConversationItems(new JidHelper.JidTypes[2]
            {
              JidHelper.JidTypes.Group,
              JidHelper.JidTypes.User
            }, true, SqliteMessagesContext.ConversationSortTypes.SortKeyAndTimestamp);
          if (!loadMoreGroups)
            return;
          groups = db.GetGroups(true, true);
        }));
        Log.d("recipient picker", "LoadRemainings moreRecents");
        List<ConversationItem> moreRecents = new List<ConversationItem>();
        if (loadMoreRecents)
        {
          Set<int> set = new Set<int>();
          foreach (RecipientItemViewModel visibleRecent in (Collection<RecipientItemViewModel>) this.visibleRecents)
            set.Add(visibleRecent.Conversation.ConversationID);
          foreach (ConversationItem conversationItem in recentChats)
          {
            if (!set.Contains(conversationItem.Conversation.ConversationID))
              moreRecents.Add(conversationItem);
          }
        }
        Log.d("recipient picker", "LoadRemainings  group view models");
        List<RecipientItemViewModel> moreGroups = new List<RecipientItemViewModel>();
        if (loadMoreGroups)
        {
          Dictionary<int, RecipientItemViewModel> dictionary = new Dictionary<int, RecipientItemViewModel>();
          foreach (RecipientItemViewModel recipientItemViewModel in groupsSnap)
            dictionary[recipientItemViewModel.Conversation.ConversationID] = recipientItemViewModel;
          RecipientItemViewModel recipientItemViewModel1 = (RecipientItemViewModel) null;
          foreach (Conversation conversation in groups)
          {
            Conversation c = conversation;
            string s = NativeInterfaces.Misc.NormalizeUnicodeString(c.GroupSubject ?? "", false);
            if (!dictionary.TryGetValue(c.ConversationID, out recipientItemViewModel1))
            {
              recipientItemViewModel1 = this.vmsCache.Get(c.Jid, RecipientListPage.RecipientItemType.Group, true, (Func<RecipientItemViewModel>) (() => this.CreateItemViewModel(c, (UserStatus) null, (Message) null, (Contact) null, RecipientListPage.RecipientItemType.Group)));
              if (recipientItemViewModel1 != null)
                moreGroups.Add(recipientItemViewModel1);
            }
            recipientItemViewModel1?.SetTitleStr(s, false);
          }
          groupsSnap.AddRange((IEnumerable<RecipientItemViewModel>) moreGroups);
        }
        this.Dispatcher.BeginInvoke((Action) (() =>
        {
          if (this.isPageRemoved)
            return;
          Log.d("recipient picker", "LoadRemainings moreRecents");
          this.hiddenRecents = moreRecents;
          if (this.visibleRecents != null)
            this.visibleRecents.Key.ShowFooter = moreRecents.Any<ConversationItem>();
          this.allGroups = groupsSnap;
          this.hiddenGroups = moreGroups;
          if (this.visibleGroups != null)
            this.visibleGroups.Key.ShowFooter = moreGroups.Any<RecipientItemViewModel>();
          this.isDataFullyLoaded = true;
          if (!this.isDataFullySuppressedSearch)
            return;
          this.isDataFullySuppressedSearch = false;
          this.SearchField_TextChanged((TextChangedEventArgs) null);
        }));
      }));
    }

    private void LoadInitialGroups()
    {
      List<Conversation> groups = (List<Conversation>) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db => groups = db.GetGroups(true, true, new int?(this.initialShowBatch))));
      KeyedObservableCollection<RecipientListPage.RecipientGrouping, RecipientItemViewModel> groupsVms = new KeyedObservableCollection<RecipientListPage.RecipientGrouping, RecipientItemViewModel>(new RecipientListPage.RecipientGrouping(AppResources.GroupsHeader, RecipientListPage.RecipientItemType.Group), groups.Select<Conversation, RecipientItemViewModel>((Func<Conversation, RecipientItemViewModel>) (c => this.vmsCache.Get(c.Jid, RecipientListPage.RecipientItemType.Group, true, (Func<RecipientItemViewModel>) (() => this.CreateItemViewModel(c, (UserStatus) null, (Message) null, (Contact) null, RecipientListPage.RecipientItemType.Group))))));
      this.Dispatcher.BeginInvoke((Action) (() =>
      {
        if (this.isPageRemoved || !groupsVms.Any<RecipientItemViewModel>())
          return;
        this.visibleGroups = groupsVms;
        this.mainListSrc.Add(this.visibleGroups);
      }));
    }

    private void LoadUserStatuses()
    {
      UserStatus[] users = (UserStatus[]) null;
      ContactsContext.Instance((Action<ContactsContext>) (db => users = db.GetWaContacts(false)));
      IEnumerable<KeyedObservableCollection<RecipientListPage.RecipientGrouping, RecipientItemViewModel>> groupedContactVms = ((IEnumerable<UserStatus>) users).Select<UserStatus, RecipientItemViewModel>((Func<UserStatus, RecipientItemViewModel>) (u => this.vmsCache.Get(u.Jid, RecipientListPage.RecipientItemType.WaContact, true, (Func<RecipientItemViewModel>) (() => this.CreateItemViewModel((Conversation) null, u, (Message) null, (Contact) null, RecipientListPage.RecipientItemType.WaContact))))).OrderBy<RecipientItemViewModel, string>((Func<RecipientItemViewModel, string>) (vm => vm.TitleStr)).GroupBy<RecipientItemViewModel, string>((Func<RecipientItemViewModel, string>) (vm => vm.TitleStr.ToGroupChar())).Select<IGrouping<string, RecipientItemViewModel>, KeyedObservableCollection<RecipientListPage.RecipientGrouping, RecipientItemViewModel>>((Func<IGrouping<string, RecipientItemViewModel>, KeyedObservableCollection<RecipientListPage.RecipientGrouping, RecipientItemViewModel>>) (g => new KeyedObservableCollection<RecipientListPage.RecipientGrouping, RecipientItemViewModel>(new RecipientListPage.RecipientGrouping(g.Key, RecipientListPage.RecipientItemType.WaContact), (IEnumerable<RecipientItemViewModel>) g)));
      this.Dispatcher.BeginInvoke((Action) (() =>
      {
        if (this.isPageRemoved)
          return;
        foreach (KeyedObservableCollection<RecipientListPage.RecipientGrouping, RecipientItemViewModel> observableCollection in groupedContactVms)
          this.mainListSrc.Add(observableCollection);
      }));
    }

    private void LoadPhoneContacts()
    {
      bool cancelled = false;
      bool contactsLoaded = false;
      Log.d("recipient picker", "LoadPhoneContacts started");
      IDisposable sub = ContactStore.GetAllContacts().Catch<Contact[], Exception>((Func<Exception, IObservable<Contact[]>>) (exn => Observable.Empty<Contact[]>())).Subscribe<Contact[]>((Action<Contact[]>) (contacts =>
      {
        List<RecipientItemViewModel> source = new List<RecipientItemViewModel>();
        if (cancelled)
          return;
        List<string> stringList = new List<string>();
        foreach (Contact contact in contacts)
        {
          Contact c = contact;
          source.Add(this.vmsCache.Get(c.GetIdentifier(), RecipientListPage.RecipientItemType.Contact, true, (Func<RecipientItemViewModel>) (() => this.CreateItemViewModel((Conversation) null, (UserStatus) null, (Message) null, c, RecipientListPage.RecipientItemType.Contact))));
        }
        if (cancelled)
          return;
        IEnumerable<KeyedObservableCollection<RecipientListPage.RecipientGrouping, RecipientItemViewModel>> phoneContactsToDisplay = source.OrderBy<RecipientItemViewModel, string>((Func<RecipientItemViewModel, string>) (c => c.GetTitle())).GroupBy<RecipientItemViewModel, string>((Func<RecipientItemViewModel, string>) (c => c.GetTitle().ToGroupChar())).OrderBy<IGrouping<string, RecipientItemViewModel>, string>((Func<IGrouping<string, RecipientItemViewModel>, string>) (g => g.Key)).Select<IGrouping<string, RecipientItemViewModel>, KeyedObservableCollection<RecipientListPage.RecipientGrouping, RecipientItemViewModel>>((Func<IGrouping<string, RecipientItemViewModel>, KeyedObservableCollection<RecipientListPage.RecipientGrouping, RecipientItemViewModel>>) (g => new KeyedObservableCollection<RecipientListPage.RecipientGrouping, RecipientItemViewModel>(new RecipientListPage.RecipientGrouping(g.Key, RecipientListPage.RecipientItemType.Contact), (IEnumerable<RecipientItemViewModel>) g)));
        if (cancelled)
          return;
        this.Dispatcher.BeginInvoke((Action) (() =>
        {
          if (this.isPageRemoved | cancelled)
            return;
          this.phoneContacts = phoneContactsToDisplay.ToList<KeyedObservableCollection<RecipientListPage.RecipientGrouping, RecipientItemViewModel>>();
          foreach (KeyedObservableCollection<RecipientListPage.RecipientGrouping, RecipientItemViewModel> phoneContact in this.phoneContacts)
            this.mainListSrc.Add(phoneContact);
          Log.d("recipient picker", "LoadPhoneContacts completed");
          contactsLoaded = true;
        }));
      }));
      this.disposables.Add(Disposable.Create((Action) (() =>
      {
        cancelled = true;
        if (!contactsLoaded)
          Log.d("recipient picker", "LoadPhoneContacts cancelled");
        sub.SafeDispose();
      })));
    }

    private RecipientItemViewModel CreateItemViewModel(
      Conversation c,
      UserStatus u,
      Message m,
      Contact pc,
      RecipientListPage.RecipientItemType type)
    {
      RecipientItemViewModel itemViewModel = (RecipientItemViewModel) null;
      switch (type)
      {
        case RecipientListPage.RecipientItemType.Frequent:
          if (c != null)
          {
            RecipientItemViewModel recipientItemViewModel = new RecipientItemViewModel(c, RecipientListPage.RecipientItemType.Frequent);
            recipientItemViewModel.EnableChatPreview = false;
            recipientItemViewModel.EnableContextMenu = false;
            recipientItemViewModel.EnableRecipientCheck = true;
            itemViewModel = recipientItemViewModel;
            break;
          }
          break;
        case RecipientListPage.RecipientItemType.Recent:
          if (c != null)
          {
            RecipientItemViewModel recipientItemViewModel = new RecipientItemViewModel(c, m, RecipientListPage.RecipientItemType.Recent);
            recipientItemViewModel.EnableChatPreview = true;
            recipientItemViewModel.EnableContextMenu = false;
            recipientItemViewModel.EnableRecipientCheck = true;
            itemViewModel = recipientItemViewModel;
            break;
          }
          break;
        case RecipientListPage.RecipientItemType.Group:
          if (c != null)
          {
            RecipientItemViewModel recipientItemViewModel = new RecipientItemViewModel(c, RecipientListPage.RecipientItemType.Group);
            recipientItemViewModel.EnableContextMenu = false;
            recipientItemViewModel.EnableRecipientCheck = true;
            itemViewModel = recipientItemViewModel;
            break;
          }
          break;
        case RecipientListPage.RecipientItemType.WaContact:
          if (u != null)
          {
            RecipientItemViewModel recipientItemViewModel = new RecipientItemViewModel(u);
            recipientItemViewModel.EnableContextMenu = false;
            itemViewModel = recipientItemViewModel;
            if (this.readOnlyJids.Contains(u.Jid) && !string.IsNullOrEmpty(this.subtitleOverride))
            {
              itemViewModel.SubtitleOverride = this.subtitleOverride;
              break;
            }
            break;
          }
          break;
        case RecipientListPage.RecipientItemType.Contact:
          if (pc != null)
          {
            PhoneContactItemViewModel contactItemViewModel = new PhoneContactItemViewModel(pc);
            contactItemViewModel.EnableContextMenu = false;
            itemViewModel = (RecipientItemViewModel) contactItemViewModel;
            break;
          }
          break;
      }
      if (itemViewModel != null)
      {
        itemViewModel.IsSelected = this.selectedJids.Contains(itemViewModel.Jid);
        if (this.readOnlyJids.Contains(itemViewModel.Jid))
        {
          itemViewModel.SelectionBackground = UIUtils.PhoneInactiveBrush;
          itemViewModel.IsReadOnly = true;
        }
        else
        {
          itemViewModel.SelectionBackground = this.selectionBackground;
          itemViewModel.IsReadOnly = false;
        }
        itemViewModel.SelectionIconSource = this.selectionIcon;
      }
      return itemViewModel;
    }

    private void TryShow()
    {
      Log.d("recipient picker", nameof (TryShow));
      if (!this.isPageLoaded || this.isShown || this.mainListSrc == null)
        return;
      this.isShown = true;
      Log.d("recipient picker", "TryShow Status");
      if (this.ShouldEnableRecipientType(RecipientListPage.RecipientItemType.Status))
      {
        Action enableStatus = (Action) (() =>
        {
          this.StatusControl.Visibility = Visibility.Visible;
          this.statusViewModel = new SelfStatusThreadViewModel((WaStatusThread) null, true);
          this.StatusControl.ViewModel = (JidItemViewModel) this.statusViewModel;
          this.disposables.Add(this.statusViewModel.GetObservable().Where<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (kv => kv.Key == "IsSelected")).ObserveOnDispatcher<KeyValuePair<string, object>>().Subscribe<KeyValuePair<string, object>>((Action<KeyValuePair<string, object>>) (kv => this.SetSelection("status@broadcast", (RecipientItemViewModel) null, this.statusViewModel.IsSelected))));
        });
        bool flag = false;
        if (this.statusVisibilityObservable != null)
          this.disposables.Add(this.statusVisibilityObservable.ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (visible =>
          {
            if (!visible)
              return;
            enableStatus();
          })));
        else
          flag = this.sendingType.IsSupportedStatusType() || this.sendingType == FunXMPP.FMessage.Type.Undefined;
        if (flag)
          enableStatus();
      }
      this.MainList.ItemsSource = (IList) this.mainListSrc;
      this.LoadOnPageLoaded();
    }

    private string GetSelectedNamesList()
    {
      string selectedNamesList = string.Join(", ", this.selectedListSrc.Select<JidItemViewModel, string>((Func<JidItemViewModel, string>) (vm => this.vmsCache.GetOne(vm.Jid)?.TitleStr ?? "")));
      if (this.selectedJids.Contains("status@broadcast"))
        selectedNamesList = !this.selectedListSrc.Any<JidItemViewModel>() ? AppResources.MyStatusV3Title : string.Format("{0}, {1}", (object) AppResources.MyStatusV3Title, (object) selectedNamesList);
      return selectedNamesList;
    }

    private void ViewMoreGroups()
    {
      if (this.visibleGroups == null || !this.visibleGroups.Any<RecipientItemViewModel>() || this.hiddenGroups == null)
        return;
      if (!this.hiddenGroups.Any<RecipientItemViewModel>())
      {
        this.visibleGroups.Key.ShowFooter = false;
      }
      else
      {
        List<RecipientItemViewModel> list = this.hiddenGroups.Take<RecipientItemViewModel>(this.partialShowBatch).ToList<RecipientItemViewModel>();
        this.hiddenGroups = this.hiddenGroups.Skip<RecipientItemViewModel>(this.partialShowBatch).ToList<RecipientItemViewModel>();
        Action<RecipientItemViewModel> action = (Action<RecipientItemViewModel>) (vm => this.visibleGroups.Add(vm));
        list.ForEach(action);
        if (this.hiddenGroups.Any<RecipientItemViewModel>())
          return;
        this.visibleGroups.Key.ShowFooter = false;
      }
    }

    private void ViewMoreRecents()
    {
      if (this.visibleRecents == null || !this.visibleRecents.Any<RecipientItemViewModel>() || this.hiddenRecents == null)
        return;
      if (!this.hiddenRecents.Any<ConversationItem>())
      {
        this.visibleRecents.Key.ShowFooter = false;
      }
      else
      {
        List<RecipientItemViewModel> list = this.hiddenRecents.Take<ConversationItem>(this.partialShowBatch).Select<ConversationItem, RecipientItemViewModel>((Func<ConversationItem, RecipientItemViewModel>) (ci => this.vmsCache.Get(ci.Jid, RecipientListPage.RecipientItemType.Recent, true, (Func<RecipientItemViewModel>) (() => this.CreateItemViewModel(ci.Conversation, (UserStatus) null, ci.Message, (Contact) null, RecipientListPage.RecipientItemType.Recent))))).ToList<RecipientItemViewModel>();
        this.hiddenRecents = this.hiddenRecents.Skip<ConversationItem>(this.partialShowBatch).ToList<ConversationItem>();
        Action<RecipientItemViewModel> action = (Action<RecipientItemViewModel>) (vm => this.visibleRecents.Add(vm));
        list.ForEach(action);
        if (this.hiddenRecents.Any<ConversationItem>())
          return;
        this.visibleRecents.Key.ShowFooter = false;
      }
    }

    private void SetSelection(string jid, RecipientItemViewModel vm, bool selected)
    {
      if (jid == null || this.readOnlyJids.Contains(jid))
        return;
      if (JidHelper.IsStatusJid(jid))
      {
        if (selected)
        {
          this.selectedJids.Add(jid);
          this.PrivacyButton.Visibility = Visibility.Visible;
          List<string> stringList = new List<string>();
          if (Settings.ShowStatusRecipientPickerTooltip)
          {
            Settings.ShowStatusRecipientPickerTooltip = false;
            stringList.Add(AppResources.StatusFirstTimeNotice);
          }
          if (this.sendingType == FunXMPP.FMessage.Type.Video && this.maxDurationOfVideo > Settings.StatusVideoMaxDuration)
          {
            string str = Plurals.Instance.GetString(AppResources.StatusVideoTrimTooltipPlural, Settings.StatusVideoMaxDuration);
            stringList.Add(str);
          }
          if (stringList.Any<string>())
          {
            this.StatusTooltipBlock.Text = string.Join("\n\n", (IEnumerable<string>) stringList);
            this.StatusTooltipPanel.Visibility = Visibility.Visible;
          }
        }
        else
        {
          this.selectedJids.Remove(jid);
          this.PrivacyButton.Visibility = this.StatusTooltipPanel.Visibility = Visibility.Collapsed;
        }
      }
      else
      {
        if (this.selectedJids.Contains(jid) == selected)
          return;
        if (selected)
        {
          if (this.isRestrictedForForward)
          {
            int recipientsForCurrCountry = CountryInfo.GetMaxMessageRecipientsForCurrCountry();
            if (this.selectedJids.Count >= recipientsForCurrCountry)
            {
              UIUtils.ShowMessageBox("", Plurals.Instance.GetString(AppResources.MulticastLimitExceededPlural, recipientsForCurrCountry)).Subscribe<Unit>();
              return;
            }
          }
          else if (this.isRestrictedForShareContact)
          {
            int groupParticipants = Settings.MaxGroupParticipants;
            if (this.selectedJids.Count >= groupParticipants)
            {
              UIUtils.ShowMessageBox("", Plurals.Instance.GetString(AppResources.ContactSharingLimitExceededPlural, groupParticipants)).Subscribe<Unit>();
              return;
            }
          }
          this.selectedJids.Add(jid);
          JidItemViewModel jidItemViewModel = !(vm is PhoneContactItemViewModel contactItemViewModel) ? new JidItemViewModel(jid) : (JidItemViewModel) new PhoneContactJidItemViewModel(contactItemViewModel.PhoneContact);
          this.selectedListSrc.Add(jidItemViewModel);
          if (this.pageMode == RecipientListPage.Mode.MultiPicker)
            this.SelectedList.ScrollIntoView((object) jidItemViewModel);
        }
        else
        {
          this.selectedJids.Remove(jid);
          this.selectedListSrc.RemoveFirst<JidItemViewModel>((Func<JidItemViewModel, bool>) (jvm => jvm.Jid == jid));
        }
        this.vmsCache.Apply(jid, (Action<RecipientItemViewModel>) (jvm => jvm.IsSelected = selected));
      }
      if (this.pageMode == RecipientListPage.Mode.MessageSending)
      {
        this.SelectedTextBlock.Text = new RichTextBlock.TextSet()
        {
          Text = this.GetSelectedNamesList()
        };
        this.SelectedScrollViewer.UpdateLayout();
        this.SelectedScrollViewer.ScrollToHorizontalOffset(this.SelectedScrollViewer.ScrollableWidth);
        this.BottomBar.Visibility = this.selectedJids.Any<string>().ToVisibility();
      }
      else
      {
        if (this.pageMode != RecipientListPage.Mode.MultiPicker)
          return;
        if (this.selectedCountFormat != null)
          this.SelectionSummaryBlock.Text = Plurals.Instance.GetString(this.selectedCountFormat, this.selectedListSrc.Count);
        this.BottomBar.Visibility = this.selectedListSrc.Any<JidItemViewModel>().ToVisibility();
      }
    }

    private IObservable<RecipientListPage.SearchResult> GetSearchObservable(
      string normalizedTerm,
      string ftsQuery)
    {
      return Observable.Create<RecipientListPage.SearchResult>((Func<IObserver<RecipientListPage.SearchResult>, Action>) (observer =>
      {
        bool disposed = false;
        AppState.Worker.Enqueue((Action) (() =>
        {
          lock (this.currSearchLock)
          {
            if (this.currFtsQuery != ftsQuery)
            {
              observer.OnCompleted();
              return;
            }
          }
          List<RecipientItemViewModel> recipientItemViewModelList3 = new List<RecipientItemViewModel>();
          Set<string> set = new Set<string>();
          if (!disposed && this.ShouldEnableRecipientType(RecipientListPage.RecipientItemType.WaContact))
          {
            List<string> matchedContacts = (List<string>) null;
            ContactsContext.Instance((Action<ContactsContext>) (db => matchedContacts = db.LookupJidsByName(ftsQuery)));
            foreach (string str in matchedContacts)
            {
              set.Add(str);
              RecipientItemViewModel recipientItemViewModel = this.vmsCache.Get(str, RecipientListPage.RecipientItemType.WaContact, false, (Func<RecipientItemViewModel>) null);
              if (recipientItemViewModel != null)
                recipientItemViewModelList3.Add(recipientItemViewModel);
            }
          }
          Dictionary<string, RecipientItemViewModel> dictionary = (Dictionary<string, RecipientItemViewModel>) null;
          if (!disposed)
          {
            if (this.ShouldEnableRecipientType(RecipientListPage.RecipientItemType.Group))
            {
              try
              {
                dictionary = this.allGroups.Where<RecipientItemViewModel>((Func<RecipientItemViewModel, bool>) (vm => vm.TitleStrLower.IndexOf(normalizedTerm) >= 0)).ToDictionary<RecipientItemViewModel, string>((Func<RecipientItemViewModel, string>) (vm => vm.Jid));
              }
              catch (Exception ex)
              {
                dictionary = new Dictionary<string, RecipientItemViewModel>();
                Log.LogException(ex, "recipient search");
              }
            }
          }
          List<RecipientItemViewModel> recipientItemViewModelList4 = new List<RecipientItemViewModel>();
          if (!disposed && this.ShouldEnableRecipientType(RecipientListPage.RecipientItemType.Recent))
          {
            if (this.visibleRecents != null)
            {
              foreach (RecipientItemViewModel visibleRecent in (Collection<RecipientItemViewModel>) this.visibleRecents)
              {
                RecipientItemViewModel recipientItemViewModel = (RecipientItemViewModel) null;
                if (visibleRecent.Conversation.IsGroup())
                  dictionary.TryGetValue(visibleRecent.Jid, out recipientItemViewModel);
                else if (set.Contains(visibleRecent.Jid))
                  recipientItemViewModel = this.vmsCache.Get(visibleRecent.Conversation.Jid, RecipientListPage.RecipientItemType.WaContact, false, (Func<RecipientItemViewModel>) null);
                if (recipientItemViewModel != null)
                  recipientItemViewModelList4.Add(recipientItemViewModel);
              }
            }
            if (this.hiddenRecents != null)
            {
              foreach (ConversationItem hiddenRecent in this.hiddenRecents)
              {
                RecipientItemViewModel recipientItemViewModel = (RecipientItemViewModel) null;
                if (hiddenRecent.Conversation.IsGroup())
                  dictionary.TryGetValue(hiddenRecent.Jid, out recipientItemViewModel);
                else if (set.Contains(hiddenRecent.Jid))
                  recipientItemViewModel = this.vmsCache.Get(hiddenRecent.Conversation.Jid, RecipientListPage.RecipientItemType.WaContact, false, (Func<RecipientItemViewModel>) null);
                if (recipientItemViewModel != null)
                  recipientItemViewModelList4.Add(recipientItemViewModel);
              }
            }
          }
          if (!disposed && this.ShouldEnableRecipientType(RecipientListPage.RecipientItemType.Contact))
          {
            if (this.phoneContacts != null)
            {
              foreach (Collection<RecipientItemViewModel> phoneContact in this.phoneContacts)
              {
                foreach (RecipientItemViewModel recipientItemViewModel in phoneContact)
                {
                  if (recipientItemViewModel.TitleStr.ToLower().IndexOf(normalizedTerm) >= 0)
                    recipientItemViewModelList3.Add(recipientItemViewModel);
                }
              }
            }
            else
              Log.l("recipient picker", "contact selection not available");
          }
          if (disposed)
          {
            Log.d("recipient picker", "GetSearchObservable disposed {0}", (object) ftsQuery.Length);
            observer.OnNext((RecipientListPage.SearchResult) null);
          }
          else
          {
            Log.d("recipient picker", "GetSearchObservable finished {0}", (object) ftsQuery.Length);
            observer.OnNext(new RecipientListPage.SearchResult(ftsQuery)
            {
              ChatResults = recipientItemViewModelList4,
              ContactResults = recipientItemViewModelList3
            });
          }
          observer.OnCompleted();
        }));
        return (Action) (() =>
        {
          Log.d("recipient picker", "GetSearchObservable disposing {0}", (object) ftsQuery.Length);
          disposed = true;
        });
      }));
    }

    private void ShowSearchResults(bool show)
    {
      this.SearchResultsPanel.Visibility = show.ToVisibility();
    }

    private void Search()
    {
      string query = this.currQuery;
      string ftsQuery = this.currFtsQuery;
      this.currSearchSub.SafeDispose();
      this.currSearchSub = (IDisposable) null;
      if (ftsQuery == null)
      {
        this.ShowSearchResults(false);
        this.resultListSrc.Clear();
        this.chatResults = this.contactResults = (KeyedObservableCollection<RecipientListPage.RecipientGrouping, RecipientItemViewModel>) null;
        this.progressIndicator.ReleaseAll();
      }
      else
      {
        string lower = NativeInterfaces.Misc.NormalizeUnicodeString(query, false).ToLower();
        this.progressIndicator.Acquire();
        this.currSearchSub = this.GetSearchObservable(lower, ftsQuery).ObserveOnDispatcher<RecipientListPage.SearchResult>().Subscribe<RecipientListPage.SearchResult>(new Action<RecipientListPage.SearchResult>(this.OnSearchResults), (Action) (() =>
        {
          this.currSearchSub.SafeDispose();
          this.currSearchSub = (IDisposable) null;
          lock (this.currSearchLock)
          {
            if (this.currFtsQuery == ftsQuery)
              this.progressIndicator.ReleaseAll();
          }
          Log.d("recipient picker", "search complete | q:{0}", (object) query);
        }));
        Log.d("recipient picker", "scheduled search | q:{0}", (object) query);
      }
    }

    private void OnSearchResults(RecipientListPage.SearchResult res)
    {
      if (res == null)
        return;
      lock (this.currSearchLock)
      {
        if (res.FtsQuery != this.currFtsQuery)
        {
          Log.d("recipient picker", "ignored search result | {0} vs. {1}", (object) res.FtsQuery, (object) this.currFtsQuery);
          return;
        }
      }
      Log.d("recipient picker", "process search result | q:{0}", (object) res.FtsQuery);
      bool flag1 = this.resultListSrc.Any<KeyedObservableCollection<RecipientListPage.RecipientGrouping, RecipientItemViewModel>>();
      bool flag2 = false;
      Log.d("recipient picker", "render {0}", (object) res.FtsQuery.Length);
      if (res.ChatResults != null && res.ChatResults.Any<RecipientItemViewModel>())
      {
        flag2 = true;
        if (this.chatResults != null && this.chatResults.Any<RecipientItemViewModel>())
          Utils.UpdateInPlace<RecipientItemViewModel>((IList<RecipientItemViewModel>) this.chatResults, (IList<RecipientItemViewModel>) res.ChatResults, (Func<RecipientItemViewModel, string>) (vm => vm.Jid), (Action<RecipientItemViewModel>) null);
        else
          this.chatResults = new KeyedObservableCollection<RecipientListPage.RecipientGrouping, RecipientItemViewModel>(new RecipientListPage.RecipientGrouping(AppResources.ChatsHeader, RecipientListPage.RecipientItemType.Recent), (IEnumerable<RecipientItemViewModel>) res.ChatResults);
        if (!this.resultListSrc.Contains(this.chatResults))
          this.resultListSrc.Insert(0, this.chatResults);
      }
      else if (this.chatResults != null)
        this.resultListSrc.Remove(this.chatResults);
      if (res.ContactResults != null && res.ContactResults.Any<RecipientItemViewModel>())
      {
        flag2 = true;
        if (this.contactResults != null && this.contactResults.Any<RecipientItemViewModel>())
          Utils.UpdateInPlace<RecipientItemViewModel>((IList<RecipientItemViewModel>) this.contactResults, (IList<RecipientItemViewModel>) res.ContactResults, (Func<RecipientItemViewModel, string>) (vm => vm.Jid), (Action<RecipientItemViewModel>) null);
        else
          this.contactResults = new KeyedObservableCollection<RecipientListPage.RecipientGrouping, RecipientItemViewModel>(new RecipientListPage.RecipientGrouping(AppResources.ContactsHeader, RecipientListPage.RecipientItemType.WaContact), (IEnumerable<RecipientItemViewModel>) res.ContactResults);
        if (!this.resultListSrc.Contains(this.contactResults))
          this.resultListSrc.Add(this.contactResults);
      }
      else if (this.contactResults != null)
        this.resultListSrc.Remove(this.contactResults);
      if (flag1)
      {
        if (!flag2)
          this.ResultList.ItemsSource = (IList) new object[0];
      }
      else if (flag2)
        this.ResultList.ItemsSource = (IList) this.resultListSrc;
      Log.d("recipient picker", "show {0}", (object) res.FtsQuery.Length);
      this.ShowSearchResults(true);
      Log.d("recipient picker", "visibility {0}", (object) res.FtsQuery.Length);
      this.ResultListFooterBlock.Visibility = (!flag2).ToVisibility();
    }

    private void SearchField_TextChanged(TextChangedEventArgs e)
    {
      string rawTerm = this.SearchField.Text ?? "";
      if (!this.isDataFullyLoaded)
      {
        Log.l("recipient picker", "Ignoring search text input, not loaded yet");
        this.isDataFullySuppressedSearch = true;
      }
      else
      {
        string query = (string) null;
        string ftsQuery = (string) null;
        ChatSearchPage.ProcessSearchTerm(rawTerm, out query, out ftsQuery);
        lock (this.currSearchLock)
        {
          if (this.currFtsQuery == ftsQuery)
          {
            Log.d("recipient picker", "search skipped | query not changed: {0}", (object) query);
            return;
          }
          this.currFtsQuery = ftsQuery;
          this.currQuery = query;
        }
        DateTime utcNow = DateTime.UtcNow;
        IDisposable delaySub = this.delaySub;
        double totalMilliseconds = (utcNow - this.lastTextChangedAt).TotalMilliseconds;
        this.lastTextChangedAt = utcNow;
        int num = rawTerm.Length <= 3 ? 0 : (totalMilliseconds <= 500.0 || this.delaySub != null ? (totalMilliseconds <= 1000.0 ? 0 : (this.delaySub != null ? 1 : 0)) : 1);
        this.delaySub.SafeDispose();
        this.delaySub = (IDisposable) null;
        if (num != 0)
          this.Search();
        else
          this.delaySub = Observable.Timer(RecipientListPage.searchStallTimeSpan).ObserveOnDispatcher<long>().Subscribe<long>((Action<long>) (_ =>
          {
            this.Search();
            if (!(utcNow == this.lastTextChangedAt))
              return;
            this.delaySub.SafeDispose();
            this.delaySub = (IDisposable) null;
          }));
      }
    }

    private void OnLoaded(object sender, EventArgs e)
    {
      if (this.isPageLoaded)
        return;
      this.isPageLoaded = true;
      this.TryShow();
      this.ResultListFooterBlock.Text = AppResources.NoResults;
      this.ResultList.ItemsSource = (IList) this.resultListSrc;
      this.MainList.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(this.List_ManipulationStarted);
      this.ResultList.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(this.List_ManipulationStarted);
    }

    private void List_ManipulationStarted(object sender, EventArgs e)
    {
      this.Focus();
      this.SearchField.CloseEmojiKeyboard();
    }

    private void Item_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (!((sender is RecipientItemControl recipientItemControl ? recipientItemControl.ViewModel : (JidItemViewModel) null) is RecipientItemViewModel viewModel) || viewModel.ShouldDisable())
        return;
      string jid = viewModel?.Jid;
      this.SetSelection(jid, viewModel, !this.selectedJids.Contains(jid));
    }

    private void Submit_Click(object sender, RoutedEventArgs e)
    {
      Action notify = (Action) (() =>
      {
        RecipientListPage.RecipientListResults recipientListResults;
        if (this.filteredRecipientTypes != null && this.filteredRecipientTypes.Count == 1 && this.filteredRecipientTypes.Contains(RecipientListPage.RecipientItemType.Contact))
        {
          List<Contact> selectedContacts = new List<Contact>();
          foreach (Collection<RecipientItemViewModel> collection in (Collection<KeyedObservableCollection<RecipientListPage.RecipientGrouping, RecipientItemViewModel>>) this.mainListSrc)
          {
            foreach (PhoneContactItemViewModel contactItemViewModel in collection)
            {
              if (this.selectedJids.Contains(contactItemViewModel.Jid))
                selectedContacts.Add(contactItemViewModel.PhoneContact);
            }
          }
          recipientListResults = new RecipientListPage.RecipientListResults(selectedContacts);
        }
        else
        {
          this.selectedJids.ToArray<string>();
          foreach (string selectedJid in this.selectedJids)
          {
            if (!JidChecker.CheckJidProtocolString(selectedJid))
            {
              JidChecker.MaybeSendJidErrorClb(nameof (RecipientListPage), selectedJid);
              this.selectedJids.Remove(selectedJid);
            }
          }
          recipientListResults = new RecipientListPage.RecipientListResults(this.selectedJids.ToList<string>());
        }
        this.resultsObserver.OnNext(recipientListResults);
        this.resultsObserver.OnCompleted();
      });
      IObservable<bool> source = this.confirmSelectionFunc == null ? Observable.Return<bool>(true) : this.confirmSelectionFunc((IEnumerable<string>) this.selectedJids);
      IDisposable sub = (IDisposable) null;
      sub = source.Take<bool>(1).ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (confirmed =>
      {
        if (confirmed)
        {
          if (this.keepPageOnSubmit)
            notify();
          else
            this.SlideDownAndBackOut(notify);
        }
        sub.SafeDispose();
        sub = (IDisposable) null;
      }));
    }

    private void ViewMore_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (!((sender is Grid grid ? grid.DataContext : (object) null) is RecipientListPage.RecipientGrouping dataContext))
        return;
      switch (dataContext.Type)
      {
        case RecipientListPage.RecipientItemType.Recent:
          this.ViewMoreRecents();
          break;
        case RecipientListPage.RecipientItemType.Group:
          this.ViewMoreGroups();
          break;
      }
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      this.isPageRemoved = true;
      IDisposable[] array = this.disposables.ToArray();
      this.disposables.Clear();
      foreach (IDisposable d in array)
        d.SafeDispose();
      base.OnRemovedFromJournal(e);
    }

    protected override void OnBackKeyPress(CancelEventArgs e)
    {
      if (this.currFtsQuery != null)
      {
        this.currSearchSub.SafeDispose();
        this.currSearchSub = (IDisposable) null;
        this.SearchField.Text = "";
      }
      else if (this.resultsObserver != null)
        this.SlideDownAndBackOut((Action) (() =>
        {
          this.resultsObserver.OnNext((RecipientListPage.RecipientListResults) null);
          this.resultsObserver.OnCompleted();
        }));
      e.Cancel = true;
      base.OnBackKeyPress(e);
    }

    private void OnSelectedItemRemoved(object sender, EventArgs e)
    {
      this.SetSelection((sender is FrameworkElement frameworkElement ? frameworkElement.DataContext : (object) null) is JidItemViewModel dataContext ? dataContext.Jid : (string) null, (RecipientItemViewModel) null, false);
    }

    private void SelectedList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      this.SelectedList.SelectedItem = (object) null;
    }

    private void TooltipDismissButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      this.StatusTooltipPanel.Visibility = Visibility.Collapsed;
    }

    private void PrivacyButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      NavUtils.NavigateToPage(this.NavigationService, "StatusPrivacySettingsPage", folderName: "Pages/Settings");
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/RecipientListPage.xaml", UriKind.Relative));
      this.LayoutRootZoomBox = (ZoomBox) this.FindName("LayoutRootZoomBox");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.PageTitle = (PageTitlePanel) this.FindName("PageTitle");
      this.SelectionSummaryBlock = (TextBlock) this.FindName("SelectionSummaryBlock");
      this.SearchField = (EmojiTextBox) this.FindName("SearchField");
      this.SelectedList = (ListBox) this.FindName("SelectedList");
      this.MainList = (WhatsApp.CompatibilityShims.LongListSelector) this.FindName("MainList");
      this.StatusControl = (WaSelfStatusItemControl) this.FindName("StatusControl");
      this.PrivacyButton = (Grid) this.FindName("PrivacyButton");
      this.StatusTooltipPanel = (Grid) this.FindName("StatusTooltipPanel");
      this.StatusTooltipBlock = (TextBlock) this.FindName("StatusTooltipBlock");
      this.LoadingProgressBar = (ProgressBar) this.FindName("LoadingProgressBar");
      this.SearchResultsPanel = (Grid) this.FindName("SearchResultsPanel");
      this.ResultList = (WhatsApp.CompatibilityShims.LongListSelector) this.FindName("ResultList");
      this.ResultListFooterBlock = (TextBlock) this.FindName("ResultListFooterBlock");
      this.BottomBar = (Grid) this.FindName("BottomBar");
      this.SubmitButton = (RoundButton) this.FindName("SubmitButton");
      this.RecipientsPreview = (Grid) this.FindName("RecipientsPreview");
      this.SelectedScrollViewer = (ScrollViewer) this.FindName("SelectedScrollViewer");
      this.SelectedTextBlock = (RichTextBlock) this.FindName("SelectedTextBlock");
      this.RightButton = (Button) this.FindName("RightButton");
    }

    public enum RecipientItemType
    {
      Undefined,
      Frequent,
      Recent,
      Group,
      WaContact,
      Contact,
      Status,
    }

    private enum Mode
    {
      MessageSending,
      MultiPicker,
    }

    public class RecipientListResults
    {
      public List<string> SelectedJids { get; private set; }

      public List<Contact> SelectedContacts { get; private set; }

      public RecipientListResults(List<Contact> selectedContacts)
      {
        this.SelectedJids = (List<string>) null;
        this.SelectedContacts = selectedContacts;
      }

      public RecipientListResults(List<string> selectedRecipients)
      {
        this.SelectedJids = selectedRecipients;
        this.SelectedContacts = (List<Contact>) null;
      }
    }

    private class RecipientGrouping : WaViewModelBase
    {
      private bool showFooter;

      public RecipientListPage.RecipientItemType Type { get; private set; }

      public string Name { get; private set; }

      public bool ShowFooter
      {
        set
        {
          this.showFooter = value;
          this.NotifyPropertyChanged("FooterVisibility");
        }
      }

      public string JumpListHeader
      {
        get
        {
          string jumpListHeader;
          switch (this.Type)
          {
            case RecipientListPage.RecipientItemType.Frequent:
              jumpListHeader = "❤";
              break;
            case RecipientListPage.RecipientItemType.Recent:
              jumpListHeader = "\uD83D\uDD52";
              break;
            case RecipientListPage.RecipientItemType.Group:
              jumpListHeader = "\uD83D\uDC65";
              break;
            default:
              jumpListHeader = this.Name;
              break;
          }
          return jumpListHeader;
        }
      }

      public RecipientGrouping(string groupingName, RecipientListPage.RecipientItemType itemType)
      {
        this.Name = groupingName;
        this.Type = itemType;
      }

      public Visibility FooterVisibility => this.showFooter.ToVisibility();

      public string ViewMoreText => AppResources.ViewMore;
    }

    private class SearchResult
    {
      protected List<RecipientItemViewModel> chatResults;
      protected List<RecipientItemViewModel> contactResults;

      public string FtsQuery { get; set; }

      public List<RecipientItemViewModel> ChatResults
      {
        get => this.chatResults ?? new List<RecipientItemViewModel>();
        set => this.chatResults = value;
      }

      public List<RecipientItemViewModel> ContactResults
      {
        get => this.contactResults ?? new List<RecipientItemViewModel>();
        set => this.contactResults = value;
      }

      public bool HasNoResults
      {
        get
        {
          if (this.FtsQuery == null || this.chatResults != null && this.chatResults.Any<RecipientItemViewModel>())
            return false;
          return this.contactResults == null || !this.contactResults.Any<RecipientItemViewModel>();
        }
      }

      public SearchResult(string ftsQuery) => this.FtsQuery = ftsQuery;
    }

    private class RecipientItemsCache
    {
      private const int ValueArrayCapacity = 5;
      private Dictionary<string, RecipientItemViewModel[]> cache = new Dictionary<string, RecipientItemViewModel[]>();

      public void Set(string jid, RecipientItemViewModel vm)
      {
        if (jid == null || vm == null)
          return;
        RecipientItemViewModel[] recipientItemViewModelArray = (RecipientItemViewModel[]) null;
        if (!this.cache.TryGetValue(jid, out recipientItemViewModelArray) || recipientItemViewModelArray == null)
        {
          recipientItemViewModelArray = new RecipientItemViewModel[5];
          this.cache[jid] = recipientItemViewModelArray;
        }
        recipientItemViewModelArray[(int) (vm.Type - 1)] = vm;
      }

      public RecipientItemViewModel GetOne(string jid)
      {
        RecipientItemViewModel[] source = (RecipientItemViewModel[]) null;
        RecipientItemViewModel one = (RecipientItemViewModel) null;
        if (this.cache.TryGetValue(jid, out source) && source != null)
          one = ((IEnumerable<RecipientItemViewModel>) source).FirstOrDefault<RecipientItemViewModel>((Func<RecipientItemViewModel, bool>) (item => item != null));
        return one;
      }

      public RecipientItemViewModel Get(
        string jid,
        RecipientListPage.RecipientItemType type,
        bool createIfNotFound,
        Func<RecipientItemViewModel> createFunc)
      {
        RecipientItemViewModel vm = (RecipientItemViewModel) null;
        if (((!this.TryGet(jid, type, out vm) ? 1 : (vm == null ? 1 : 0)) & (createIfNotFound ? 1 : 0)) != 0 && createFunc != null)
        {
          vm = createFunc();
          if (vm != null)
            this.Set(jid, vm);
        }
        return vm;
      }

      public bool TryGet(
        string jid,
        RecipientListPage.RecipientItemType type,
        out RecipientItemViewModel vm)
      {
        RecipientItemViewModel[] recipientItemViewModelArray = (RecipientItemViewModel[]) null;
        vm = (RecipientItemViewModel) null;
        if (this.cache.TryGetValue(jid, out recipientItemViewModelArray) && recipientItemViewModelArray != null)
          vm = recipientItemViewModelArray[(int) (type - 1)];
        return vm != null;
      }

      public void Apply(string jid, Action<RecipientItemViewModel> a)
      {
        if (jid == null || a == null)
          return;
        RecipientItemViewModel[] recipientItemViewModelArray = (RecipientItemViewModel[]) null;
        if (!this.cache.TryGetValue(jid, out recipientItemViewModelArray) || recipientItemViewModelArray == null)
          return;
        foreach (RecipientItemViewModel recipientItemViewModel in recipientItemViewModelArray)
        {
          if (recipientItemViewModel != null)
            a(recipientItemViewModel);
        }
      }
    }

    private class InitState
    {
      public IObserver<RecipientListPage.RecipientListResults> Observer;
      public string Title;
      public FunXMPP.FMessage.Type SendingType;
      public string[] ExcludedJids;
      public HashSet<RecipientListPage.RecipientItemType> FilteredRecipientTypes;
      public IEnumerable<string> PreSelections;
      public RecipientListPage.Mode Mode;
      public IObservable<bool> StatusVisibilityObservable;
      public Brush SelectionBackground;
      public System.Windows.Media.ImageSource SelectionIcon;
      public string SelectedCountFormat;
      public bool KeepPageOnSubmit;
      public int MaxDurationOfVideo;
      public IEnumerable<string> ReadOnlyJid;
      public Func<IEnumerable<string>, IObservable<bool>> ConfirmSelectionFunc;
      public string SubtitleOverride;
    }
  }
}
