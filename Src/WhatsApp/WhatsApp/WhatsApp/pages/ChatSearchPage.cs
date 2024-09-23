// Decompiled with JetBrains decompiler
// Type: WhatsApp.ChatSearchPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using WhatsApp.CommonOps;
using WhatsApp.WaCollections;
using WhatsApp.WaViewModels;


namespace WhatsApp
{
  public class ChatSearchPage : PhoneApplicationPage
  {
    private const string LogHeader = "searchpage";
    private List<IDisposable> disposables = new List<IDisposable>();
    private ChatSearchPage.Tabs currTab = ChatSearchPage.Tabs.Chats;
    private object currSearchLock = new object();
    private string currSearchQuery;
    private string currSearchFtsQuery;
    private IDisposable currSearchSub;
    private Dictionary<ChatSearchPage.Tabs, ChatSearchPage.SearchResult> unrenderedResults = new Dictionary<ChatSearchPage.Tabs, ChatSearchPage.SearchResult>();
    private Dictionary<ChatSearchPage.Tabs, ChatSearchPage.SearchResult> renderedResults = new Dictionary<ChatSearchPage.Tabs, ChatSearchPage.SearchResult>();
    private Dictionary<string, Conversation> cachedChats = new Dictionary<string, Conversation>();
    private object lastMsgVm;
    private IDisposable loadMoreMsgSub;
    private DateTime lastTextChangedAt = DateTime.UtcNow;
    private IDisposable delaySub;
    private PivotHeaderConverter pivotHeaderConverter = new PivotHeaderConverter();
    private GlobalProgressIndicator progressIndicator;
    private bool enableAutoLoadMoreMsgs = true;
    private bool messageIndexComplete = true;
    private const int MessageSearchInitialLimit = 50;
    private TabHeaderControl tabHeaders;
    private List<ConversationItem> cachedAllChats;
    private const int searchStallTimeMs = 500;
    private static readonly TimeSpan searchStallTimeSpan = TimeSpan.FromMilliseconds(500.0);
    internal ZoomBox LayoutRootZoomBox;
    internal Grid LayoutRoot;
    internal EmojiTextBox SearchField;
    internal Pivot Pivot;
    internal PivotItem ChatsPivotItem;
    internal WhatsApp.CompatibilityShims.LongListSelector ChatResultsList;
    internal TextBlock ChatsFooterTooltip;
    internal PivotItem MessagesPivotItem;
    internal WhatsApp.CompatibilityShims.LongListSelector MessageResultsList;
    internal TextBlock MessagesFooterTooltip;
    internal PivotItem ContactsPivotItem;
    internal WhatsApp.CompatibilityShims.LongListSelector ContactResultsList;
    internal TextBlock ContactsFooterTooltip;
    private bool _contentLoaded;

    public ChatSearchPage()
    {
      this.InitializeComponent();
      this.InitUI();
      this.MessageResultsList.JumpListStyle = (Style) null;
    }

    private void InitUI()
    {
      this.LayoutRootZoomBox.ZoomFactor = ResolutionHelper.ZoomFactor;
      this.progressIndicator = new GlobalProgressIndicator((DependencyObject) this);
      this.ClearSearchResults();
      this.ChatResultsList.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(this.ResultsList_ManipulationStarted);
      this.MessageResultsList.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(this.ResultsList_ManipulationStarted);
      this.ContactResultsList.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(this.ResultsList_ManipulationStarted);
      this.MessageResultsList.ItemRealized += new EventHandler<ItemRealizationEventArgs>(this.MessageResultsList_ItemRealized);
      this.disposables.Add(this.ChatResultsList.GetSelectionChangedAsync().Where<object>((Func<object, bool>) (o => o != null)).ObserveOnDispatcher<object>().Do<object>((Action<object>) (u => this.ChatResultsList.SelectedItem = (object) null)).Select<object, ChatItemViewModel>((Func<object, ChatItemViewModel>) (o => o as ChatItemViewModel)).Where<ChatItemViewModel>((Func<ChatItemViewModel, bool>) (vm => vm != null && vm.Conversation.Jid != null)).Subscribe<ChatItemViewModel>((Action<ChatItemViewModel>) (vm => NavUtils.NavigateToChat(vm.Conversation.Jid, false))));
      this.disposables.Add(this.MessageResultsList.GetSelectionChangedAsync().Where<object>((Func<object, bool>) (o => o != null)).ObserveOnDispatcher<object>().Do<object>((Action<object>) (u => this.MessageResultsList.SelectedItem = (object) null)).Select<object, MessageResultViewModel>((Func<object, MessageResultViewModel>) (o => o as MessageResultViewModel)).Where<MessageResultViewModel>((Func<MessageResultViewModel, bool>) (vm => vm != null)).Subscribe<MessageResultViewModel>((Action<MessageResultViewModel>) (vm =>
      {
        MessageSearchResult searchResult = vm.SearchResult;
        Message targetMsg = searchResult.Message;
        if (targetMsg == null)
          return;
        Log.l("searchpage", "open chat to msg | jid:{0},msgid:{1}", (object) targetMsg.KeyRemoteJid, (object) targetMsg.MessageID);
        ChatPage.NextInstanceInitState = new ChatPage.InitState()
        {
          MessageLoader = MessageLoader.Get(targetMsg.KeyRemoteJid, new int?(), 0, targetInitialLandingMsgId: new int?(targetMsg.MessageID)),
          SearchResult = searchResult
        };
        this.Dispatcher.BeginInvoke((Action) (() =>
        {
          Storyboarder.Perform(WaAnimations.PageTransition(PageTransitionAnimation.ContinuumBackwardOut), (DependencyObject) this.LayoutRoot);
          NavUtils.NavigateToChat(targetMsg.KeyRemoteJid, false, nameof (ChatSearchPage));
        }));
      })));
      this.disposables.Add(this.ContactResultsList.GetSelectionChangedAsync().Where<object>((Func<object, bool>) (o => o != null)).ObserveOnDispatcher<object>().Do<object>((Action<object>) (u => this.ContactResultsList.SelectedItem = (object) null)).Select<object, UserViewModel>((Func<object, UserViewModel>) (o => o as UserViewModel)).Where<UserViewModel>((Func<UserViewModel, bool>) (vm => vm != null && vm.User.Jid != null)).Subscribe<UserViewModel>((Action<UserViewModel>) (vm => NavUtils.NavigateToChat(vm.User.Jid, false))));
      this.disposables.Add(this.SearchField.GetTextChangedAsync().ObserveOnDispatcher<TextChangedEventArgs>().Subscribe<TextChangedEventArgs>(new Action<TextChangedEventArgs>(this.SearchField_TextChanged)));
      this.messageIndexComplete = !IndexMessages.IndexingInProgress;
      if (!this.messageIndexComplete)
        this.Pivot.Items.Remove((object) this.MessagesPivotItem);
      TabHeaderControl tabHeaderControl = new TabHeaderControl(this.Pivot);
      tabHeaderControl.Margin = new Thickness(24.0, 0.0, 24.0, 0.0);
      tabHeaderControl.HorizontalAlignment = HorizontalAlignment.Stretch;
      this.tabHeaders = tabHeaderControl;
      Grid.SetRow((FrameworkElement) this.tabHeaders, 1);
      this.LayoutRoot.Children.Add((UIElement) this.tabHeaders);
    }

    private void ResetTooltip(string s = null)
    {
      this.ChatsFooterTooltip.Text = this.MessagesFooterTooltip.Text = this.ContactsFooterTooltip.Text = s;
    }

    private void ResetTabHeaders()
    {
      this.ChatsPivotItem.Header = (object) this.pivotHeaderConverter.Convert(AppResources.ChatsHeader);
      this.MessagesPivotItem.Header = (object) this.pivotHeaderConverter.Convert(AppResources.MessagesHeader);
      this.ContactsPivotItem.Header = (object) this.pivotHeaderConverter.Convert(AppResources.ContactsHeader);
      if (this.tabHeaders == null)
        return;
      for (int index = 0; index < this.Pivot.Items.Count; ++index)
        this.tabHeaders.SetCount(index, 0);
    }

    private void ClearSearchResults()
    {
      this.renderedResults.Clear();
      this.ChatResultsList.ItemsSource = this.MessageResultsList.ItemsSource = this.ContactResultsList.ItemsSource = (IList) new object[0];
      this.ResetTooltip(AppResources.SearchTooltip);
      this.ResetTabHeaders();
    }

    private void RenderSearchResult(ChatSearchPage.SearchResult res)
    {
      this.renderedResults[res.TargetTab] = res;
      switch (res.TargetTab)
      {
        case ChatSearchPage.Tabs.Chats:
          if (!(this.ChatResultsList.ItemsSource is ObservableCollection<JidItemViewModel> itemsSource1) || !itemsSource1.Any<JidItemViewModel>())
          {
            ObservableCollection<JidItemViewModel> resultViewModels;
            this.ChatResultsList.ItemsSource = (IList) (resultViewModels = res.ResultViewModels);
          }
          else
            this.MergeItemSource<ChatItemViewModel>(itemsSource1, res.ResultViewModels, (Action<ChatItemViewModel, ChatItemViewModel>) ((old, @new) => old.CopySearchItemsFrom(@new)));
          this.ChatsFooterTooltip.Text = res.HasNoResults ? AppResources.NoResults : (string) null;
          break;
        case ChatSearchPage.Tabs.Messages:
          if (res is ChatSearchPage.MessageTabSearchResult messageTabSearchResult)
          {
            bool isFlatList = this.MessageResultsList.IsFlatList;
            IList itemsSource2 = this.MessageResultsList.ItemsSource;
            KeyedObservableCollection<string, JidItemViewModel> resultViewModels1 = messageTabSearchResult.StarredMessageResultViewModels;
            KeyedObservableCollection<string, JidItemViewModel> resultViewModels2 = messageTabSearchResult.MessageResultViewModels;
            KeyedObservableCollection<string, JidItemViewModel> observableCollection1;
            KeyedObservableCollection<string, JidItemViewModel> observableCollection2;
            if (messageTabSearchResult.ShouldRenderAsFlatList)
            {
              KeyedObservableCollection<string, JidItemViewModel> newSource = resultViewModels2 ?? resultViewModels1;
              bool flag = false;
              if (isFlatList)
              {
                if (!(itemsSource2 is KeyedObservableCollection<string, JidItemViewModel> observableCollection3) || !observableCollection3.Any<JidItemViewModel>())
                {
                  flag = true;
                  observableCollection3 = newSource;
                }
                else
                  this.MergeItemSource<MessageResultViewModel>((ObservableCollection<JidItemViewModel>) observableCollection3, (ObservableCollection<JidItemViewModel>) newSource, (Action<MessageResultViewModel, MessageResultViewModel>) ((old, @new) => old.SearchResult = @new.SearchResult));
              }
              else
              {
                flag = true;
                observableCollection3 = newSource;
              }
              if (flag)
              {
                this.MessageResultsList.ItemsSource = (IList) null;
                this.MessageResultsList.IsFlatList = true;
                this.MessageResultsList.GroupHeaderTemplate = (DataTemplate) null;
                this.MessageResultsList.ItemsSource = (IList) observableCollection3;
              }
              if (observableCollection3.Any<JidItemViewModel>((Func<JidItemViewModel, bool>) (vm => vm is MessageResultViewModel messageResultViewModel && messageResultViewModel.SearchResult != null && messageResultViewModel.SearchResult.Message != null && !messageResultViewModel.SearchResult.Message.IsStarred)))
              {
                observableCollection1 = (KeyedObservableCollection<string, JidItemViewModel>) null;
                observableCollection2 = observableCollection3;
              }
              else
              {
                observableCollection2 = (KeyedObservableCollection<string, JidItemViewModel>) null;
                observableCollection1 = observableCollection3;
              }
            }
            else
            {
              bool flag = false;
              if (isFlatList)
              {
                observableCollection1 = resultViewModels1;
                observableCollection2 = resultViewModels2;
                flag = true;
              }
              else if (!(itemsSource2 is List<KeyedObservableCollection<string, JidItemViewModel>> observableCollectionList1))
              {
                observableCollection1 = resultViewModels1;
                observableCollection2 = resultViewModels2;
                flag = true;
              }
              else
              {
                observableCollection1 = observableCollectionList1[0];
                observableCollection2 = observableCollectionList1[1];
                if (observableCollection1 == null || !observableCollection1.Any<JidItemViewModel>())
                {
                  observableCollection1 = resultViewModels1;
                  flag = true;
                }
                else
                  this.MergeItemSource<MessageResultViewModel>((ObservableCollection<JidItemViewModel>) observableCollection1, (ObservableCollection<JidItemViewModel>) resultViewModels1, (Action<MessageResultViewModel, MessageResultViewModel>) ((old, @new) => old.SearchResult = @new.SearchResult));
                if (observableCollection2 == null || !observableCollection2.Any<JidItemViewModel>())
                {
                  observableCollection2 = resultViewModels2;
                  flag = true;
                }
                else
                  this.MergeItemSource<MessageResultViewModel>((ObservableCollection<JidItemViewModel>) observableCollection2, (ObservableCollection<JidItemViewModel>) resultViewModels2, (Action<MessageResultViewModel, MessageResultViewModel>) ((old, @new) => old.SearchResult = @new.SearchResult));
              }
              if (flag)
              {
                List<KeyedObservableCollection<string, JidItemViewModel>> observableCollectionList2 = new List<KeyedObservableCollection<string, JidItemViewModel>>()
                {
                  observableCollection1,
                  observableCollection2
                };
                this.MessageResultsList.ItemsSource = (IList) null;
                this.MessageResultsList.IsFlatList = false;
                this.MessageResultsList.GroupHeaderTemplate = this.Resources[(object) "MessageResultListHeaderTemplate"] as DataTemplate;
                this.MessageResultsList.ItemsSource = (IList) observableCollectionList2;
              }
            }
            messageTabSearchResult.StarredMessageResultViewModels = observableCollection1;
            messageTabSearchResult.MessageResultViewModels = observableCollection2;
            this.lastMsgVm = observableCollection2 == null ? (object) (JidItemViewModel) null : (object) observableCollection2.LastOrDefault<JidItemViewModel>();
          }
          this.MessagesFooterTooltip.Text = res.HasNoResults ? AppResources.NoResults : (string) null;
          break;
        case ChatSearchPage.Tabs.Contacts:
          if (!(this.ContactResultsList.ItemsSource is ObservableCollection<JidItemViewModel> itemsSource3) || !itemsSource3.Any<JidItemViewModel>())
          {
            ObservableCollection<JidItemViewModel> resultViewModels;
            this.ContactResultsList.ItemsSource = (IList) (resultViewModels = res.ResultViewModels);
          }
          else
            this.MergeItemSource<UserViewModel>(itemsSource3, res.ResultViewModels, (Action<UserViewModel, UserViewModel>) ((old, @new) => old.CopySearchItemsFrom(@new)));
          this.ContactsFooterTooltip.Text = res.HasNoResults ? AppResources.NoResults : (string) null;
          break;
      }
    }

    private ObservableCollection<JidItemViewModel> MergeItemSource<VMType>(
      ObservableCollection<JidItemViewModel> currSource,
      ObservableCollection<JidItemViewModel> newSource,
      Action<VMType, VMType> onUpdate)
      where VMType : JidItemViewModel
    {
      if (currSource == null)
        return (ObservableCollection<JidItemViewModel>) null;
      Dictionary<string, JidItemViewModel> newDict = newSource.ToDictionary<JidItemViewModel, string>((Func<JidItemViewModel, string>) (vm => vm.Key));
      Utils.UpdateInPlace<JidItemViewModel>((IList<JidItemViewModel>) currSource, (IList<JidItemViewModel>) newSource, (Func<JidItemViewModel, string>) (vm => vm.Key), (Action<JidItemViewModel>) (vm =>
      {
        JidItemViewModel jidItemViewModel = (JidItemViewModel) null;
        if (!newDict.TryGetValue(vm.Key, out jidItemViewModel))
          return;
        onUpdate((VMType) vm, (VMType) jidItemViewModel);
      }));
      return currSource;
    }

    private void SetTabHeader(ChatSearchPage.Tabs tab, int count = 0)
    {
      PivotItem pivotItem = (PivotItem) null;
      switch (tab)
      {
        case ChatSearchPage.Tabs.Chats:
          pivotItem = this.ChatsPivotItem;
          break;
        case ChatSearchPage.Tabs.Messages:
          pivotItem = this.MessagesPivotItem;
          break;
        case ChatSearchPage.Tabs.Contacts:
          pivotItem = this.ContactsPivotItem;
          break;
      }
      if (pivotItem == null)
        return;
      this.tabHeaders.SetCount(this.Pivot.Items.IndexOf((object) pivotItem), count);
    }

    public static void ProcessSearchTerm(string rawTerm, out string query, out string ftsQuery)
    {
      string[] array1 = ((IEnumerable<string>) rawTerm.Split((char[]) null)).Select<string, string>((Func<string, string>) (s => s?.Trim())).Where<string>((Func<string, bool>) (s => !string.IsNullOrWhiteSpace(s))).ToArray<string>();
      query = ((IEnumerable<string>) array1).Any<string>() ? string.Join(" ", array1) : (string) null;
      string[] array2 = ((IEnumerable<string>) new string(rawTerm.Where<char>((Func<char, bool>) (c => !char.IsPunctuation(c))).ToArray<char>()).Split((char[]) null)).Select<string, string>((Func<string, string>) (s => s?.Trim())).Where<string>((Func<string, bool>) (s => !string.IsNullOrWhiteSpace(s))).ToArray<string>();
      ftsQuery = ((IEnumerable<string>) array2).Any<string>() ? string.Join(" ", ((IEnumerable<string>) array2).Select<string, string>((Func<string, string>) (s => s + "*"))) : (string) null;
    }

    private bool SetNewSearchTerms(string rawTerm)
    {
      lock (this.currSearchLock)
      {
        string query = (string) null;
        string ftsQuery = (string) null;
        ChatSearchPage.ProcessSearchTerm(rawTerm, out query, out ftsQuery);
        if (query == this.currSearchQuery)
        {
          Log.d("searchpage", "search skipped | query not changed: {0}", (object) query);
          return false;
        }
        this.currSearchQuery = query;
        this.currSearchFtsQuery = ftsQuery;
      }
      return true;
    }

    private void Search(ChatSearchPage.Tabs primaryTarget)
    {
      string query = this.currSearchQuery;
      string currSearchFtsQuery = this.currSearchFtsQuery;
      this.currSearchSub.SafeDispose();
      this.loadMoreMsgSub.SafeDispose();
      this.currSearchSub = this.loadMoreMsgSub = (IDisposable) null;
      this.lastMsgVm = (object) null;
      if (query == null)
      {
        this.ClearSearchResults();
        Log.d("searchpage", "cleared all tabs");
        this.progressIndicator.ReleaseAll();
      }
      else
      {
        this.ResetTooltip();
        this.progressIndicator.Acquire();
        this.currSearchSub = this.GetSearchObservable(primaryTarget, query, currSearchFtsQuery).SubscribeOn<ChatSearchPage.SearchResult>((IScheduler) AppState.Worker).ObserveOnDispatcher<ChatSearchPage.SearchResult>().Subscribe<ChatSearchPage.SearchResult>(new Action<ChatSearchPage.SearchResult>(this.OnSearchResult), (Action) (() =>
        {
          this.currSearchSub.SafeDispose();
          this.currSearchSub = (IDisposable) null;
          bool flag = false;
          lock (this.currSearchLock)
            flag = this.currSearchQuery == query;
          if (flag)
            this.progressIndicator.ReleaseAll();
          Log.d("searchpage", "search complete | q:{0}", (object) query);
        }));
        Log.d("searchpage", "scheduled search | q:{0}", (object) query);
      }
    }

    private void LoadMoreMessages()
    {
      if (this.loadMoreMsgSub != null)
      {
        Log.d("searchpage", "load more msgs scheduled already");
      }
      else
      {
        string query = (string) null;
        string ftsQuery = (string) null;
        lock (this.currSearchLock)
        {
          query = this.currSearchQuery;
          ftsQuery = this.currSearchFtsQuery;
        }
        if (query == null)
          return;
        ChatSearchPage.SearchResult searchResult = (ChatSearchPage.SearchResult) null;
        if (!this.renderedResults.TryGetValue(ChatSearchPage.Tabs.Messages, out searchResult) || searchResult == null || !(searchResult.SearchQuery == query) || searchResult.Count <= 0 || !(searchResult is ChatSearchPage.MessageTabSearchResult messageTabSearchResult))
          return;
        int count = messageTabSearchResult.MessageResultViewModels.Count;
        this.loadMoreMsgSub = this.GetLoadMoreMsgsObservable(query, ftsQuery, count).SubscribeOn<ChatSearchPage.SearchResult>((IScheduler) AppState.Worker).ObserveOnDispatcher<ChatSearchPage.SearchResult>().Subscribe<ChatSearchPage.SearchResult>((Action<ChatSearchPage.SearchResult>) (res => this.OnMoreMessageSearchResult(res as ChatSearchPage.MessageTabSearchResult)), (Action) (() =>
        {
          this.loadMoreMsgSub.SafeDispose();
          this.loadMoreMsgSub = (IDisposable) null;
        }));
      }
    }

    private IObservable<ChatSearchPage.SearchResult> GetLoadMoreMsgsObservable(
      string query,
      string ftsQuery,
      int offset)
    {
      return Observable.Create<ChatSearchPage.SearchResult>((Func<IObserver<ChatSearchPage.SearchResult>, Action>) (observer =>
      {
        Log.d("searchpage", "search more msgs | q:{0},offset:{1}", (object) query, (object) offset);
        bool flag = false;
        lock (this.currSearchLock)
          flag = this.currSearchQuery != query;
        if (flag)
        {
          Log.d("searchpage", "search more msgs aborted | q:{0}", (object) query);
        }
        else
        {
          ChatSearchPage.SearchResult searchResult = this.SearchMessages(query, ftsQuery, new int?(offset), new int?(100), false);
          lock (this.currSearchLock)
            flag = this.currSearchQuery != query;
          if (flag)
            Log.d("searchpage", "notify more msgs skipped | q:{0}", (object) query);
          else
            observer.OnNext(searchResult);
        }
        observer.OnCompleted();
        return (Action) (() => Log.d("searchpage", "search more msgs disposed | q:{0}", (object) query));
      }));
    }

    private IObservable<ChatSearchPage.SearchResult> GetSearchObservable(
      ChatSearchPage.Tabs primaryTarget,
      string query,
      string ftsQuery)
    {
      return Observable.Create<ChatSearchPage.SearchResult>((Func<IObserver<ChatSearchPage.SearchResult>, Action>) (observer =>
      {
        ChatSearchPage.Tabs[] source;
        switch (primaryTarget)
        {
          case ChatSearchPage.Tabs.Messages:
            ChatSearchPage.Tabs[] tabsArray1;
            if (!this.messageIndexComplete)
              tabsArray1 = new ChatSearchPage.Tabs[2]
              {
                ChatSearchPage.Tabs.Chats,
                ChatSearchPage.Tabs.Contacts
              };
            else
              tabsArray1 = new ChatSearchPage.Tabs[3]
              {
                ChatSearchPage.Tabs.Messages,
                ChatSearchPage.Tabs.Chats,
                ChatSearchPage.Tabs.Contacts
              };
            source = tabsArray1;
            break;
          case ChatSearchPage.Tabs.Contacts:
            ChatSearchPage.Tabs[] tabsArray2;
            if (!this.messageIndexComplete)
              tabsArray2 = new ChatSearchPage.Tabs[2]
              {
                ChatSearchPage.Tabs.Contacts,
                ChatSearchPage.Tabs.Chats
              };
            else
              tabsArray2 = new ChatSearchPage.Tabs[3]
              {
                ChatSearchPage.Tabs.Contacts,
                ChatSearchPage.Tabs.Chats,
                ChatSearchPage.Tabs.Messages
              };
            source = tabsArray2;
            break;
          default:
            ChatSearchPage.Tabs[] tabsArray3;
            if (!this.messageIndexComplete)
              tabsArray3 = new ChatSearchPage.Tabs[2]
              {
                ChatSearchPage.Tabs.Chats,
                ChatSearchPage.Tabs.Contacts
              };
            else
              tabsArray3 = new ChatSearchPage.Tabs[3]
              {
                ChatSearchPage.Tabs.Chats,
                ChatSearchPage.Tabs.Messages,
                ChatSearchPage.Tabs.Contacts
              };
            source = tabsArray3;
            break;
        }
        bool localCancel = false;
        Action[] searchActions = ((IEnumerable<ChatSearchPage.Tabs>) source).Select<ChatSearchPage.Tabs, Action>((Func<ChatSearchPage.Tabs, Action>) (target => (Action) (() =>
        {
          bool flag = false;
          lock (this.currSearchLock)
            flag = localCancel || this.currSearchQuery != query;
          if (flag)
          {
            Log.d("searchpage", "search aborted | q:{0}", (object) query);
            localCancel = true;
          }
          else
          {
            ChatSearchPage.SearchResult searchResult = this.SearchByTarget(target, query, ftsQuery);
            lock (this.currSearchLock)
              flag = localCancel || this.currSearchQuery != query;
            if (flag)
            {
              Log.d("searchpage", "search aborted | q:{0}", (object) query);
              localCancel = true;
            }
            else
              observer.OnNext(searchResult);
          }
        }))).ToArray<Action>();
        int idx = 0;
        Action attempt = (Action) (() => { });
        attempt = (Action) (() =>
        {
          int num = idx * 1000;
          Action nextAction = idx < searchActions.Length ? searchActions[idx++] : (Action) null;
          Action a = (Action) (() =>
          {
            nextAction();
            attempt();
          });
          if (nextAction == null)
            observer.OnCompleted();
          else if (num == 0)
            a();
          else
            AppState.Worker.RunAfterDelay(TimeSpan.FromMilliseconds((double) num), a);
        });
        attempt();
        return (Action) (() =>
        {
          Log.d("searchpage", "search disposed | q:{0}", (object) query);
          localCancel = true;
        });
      }));
    }

    private ChatSearchPage.SearchResult SearchByTarget(
      ChatSearchPage.Tabs target,
      string query,
      string ftsQuery)
    {
      ChatSearchPage.SearchResult searchResult = (ChatSearchPage.SearchResult) null;
      DateTime now = DateTime.Now;
      switch (target)
      {
        case ChatSearchPage.Tabs.Chats:
          searchResult = this.SearchChats(query, ftsQuery);
          break;
        case ChatSearchPage.Tabs.Messages:
          int num = this.enableAutoLoadMoreMsgs ? 50 : 200;
          searchResult = this.SearchMessages(query, ftsQuery, new int?(), new int?(num), true);
          break;
        case ChatSearchPage.Tabs.Contacts:
          searchResult = this.SearchWaContacts(query, ftsQuery);
          break;
      }
      Log.d("searchpage", "Search time {0} for {1}, length {2}", (object) (DateTime.Now - now).TotalMilliseconds, (object) target, (object) (query != null ? query.Length : -1));
      return searchResult;
    }

    private ChatSearchPage.SearchResult SearchChats(string query, string ftsQuery)
    {
      ChatSearchPage.SearchResult searchResult = new ChatSearchPage.SearchResult(query)
      {
        TargetTab = ChatSearchPage.Tabs.Chats
      };
      string caselessTerm = query.ToLowerInvariant();
      if (this.cachedAllChats == null)
        MessagesContext.Run((MessagesContext.MessagesCallback) (db => this.cachedAllChats = db.GetConversationItems(new JidHelper.JidTypes[3]
        {
          JidHelper.JidTypes.Group,
          JidHelper.JidTypes.User,
          JidHelper.JidTypes.Psa
        }, true)));
      UserStatusSearchResult[] favResults = (UserStatusSearchResult[]) null;
      ContactsContext.Instance((Action<ContactsContext>) (db => favResults = db.QueryUserStatusesFtsTableWithOffsets(ftsQuery)));
      Dictionary<string, UserStatus> dictionary = new Dictionary<string, UserStatus>();
      Dictionary<string, Pair<int, int>[]> favOffsetMap = new Dictionary<string, Pair<int, int>[]>();
      foreach (UserStatusSearchResult statusSearchResult in favResults)
      {
        if (statusSearchResult.JidMatched || statusSearchResult.ContactNameOffsets != null)
        {
          dictionary.Add(statusSearchResult.UserStatus.Jid, statusSearchResult.UserStatus);
          if (statusSearchResult.ContactNameOffsets != null)
            favOffsetMap.Add(statusSearchResult.UserStatus.Jid, statusSearchResult.ContactNameOffsets);
        }
      }
      List<ConversationItem> source = new List<ConversationItem>();
      string query1 = (string) null;
      string ftsQuery1 = (string) null;
      foreach (ConversationItem cachedAllChat in this.cachedAllChats)
      {
        if (JidHelper.IsUserJid(cachedAllChat.Jid))
        {
          if (dictionary.ContainsKey(cachedAllChat.Jid))
          {
            source.Add(cachedAllChat);
          }
          else
          {
            UserStatus user = UserCache.Get(cachedAllChat.Jid, false);
            if (user != null && user.IsTopTier())
            {
              ChatSearchPage.ProcessSearchTerm(user.GetVerifiedNameForDisplay().ToLowerInvariant(), out query1, out ftsQuery1);
              if (query1 != null && query1.Contains(caselessTerm))
                source.Add(cachedAllChat);
            }
          }
        }
        else
        {
          ChatSearchPage.ProcessSearchTerm(cachedAllChat.Conversation.GetName().ToLowerInvariant(), out query1, out ftsQuery1);
          if (query1 != null && query1.Contains(caselessTerm))
            source.Add(cachedAllChat);
        }
      }
      source.Sort(new Comparison<ConversationItem>(ConversationItem.CompareByTimestamp));
      ChatItemViewModel[] array = source.Select<ConversationItem, ChatItemViewModel>((Func<ConversationItem, ChatItemViewModel>) (item =>
      {
        ChatItemViewModel chatItemViewModel = new ChatItemViewModel(item.Conversation, item.Message)
        {
          EnableChatPreview = false,
          EnableContextMenu = true
        };
        Pair<int, int>[] offsets = (Pair<int, int>[]) null;
        if (favOffsetMap.TryGetValue(chatItemViewModel.Conversation.Jid, out offsets))
          chatItemViewModel.SetSearchOffsets(offsets);
        else
          chatItemViewModel.SetSearchText(caselessTerm);
        return chatItemViewModel;
      })).ToArray<ChatItemViewModel>();
      searchResult.AddResultViewModels((IEnumerable<JidItemViewModel>) array);
      return searchResult;
    }

    private ChatSearchPage.SearchResult SearchMessages(
      string query,
      string ftsQuery,
      int? offset,
      int? limit,
      bool includeStarred)
    {
      ChatSearchPage.MessageTabSearchResult messageTabSearchResult = new ChatSearchPage.MessageTabSearchResult(query);
      messageTabSearchResult.TargetTab = ChatSearchPage.Tabs.Messages;
      ChatSearchPage.MessageTabSearchResult r = messageTabSearchResult;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        MessagesContext messagesContext1 = db;
        string query1 = ftsQuery;
        int? offset1 = offset;
        int? nullable = limit;
        int? limit1 = new int?(nullable ?? 50);
        MessageSearchResult[] msgResults1 = messagesContext1.QueryFtsTableWithOffsets(query1, offset1, limit1, includeStarred: false);
        r.AddMessageResultViewModels((IEnumerable<JidItemViewModel>) this.MessageResultsToViewModels(db, msgResults1));
        if (!includeStarred)
          return;
        MessagesContext messagesContext2 = db;
        string query2 = ftsQuery;
        nullable = new int?();
        int? offset2 = nullable;
        nullable = new int?();
        int? limit2 = nullable;
        MessageSearchResult[] msgResults2 = messagesContext2.QueryFtsTableWithOffsets(query2, offset2, limit2, includeNonStarred: false);
        r.AddStarredMessageResultViewModels((IEnumerable<JidItemViewModel>) this.MessageResultsToViewModels(db, msgResults2));
      }));
      return (ChatSearchPage.SearchResult) r;
    }

    private JidItemViewModel[] MessageResultsToViewModels(
      MessagesContext db,
      MessageSearchResult[] msgResults)
    {
      return (JidItemViewModel[]) ((IEnumerable<MessageSearchResult>) msgResults).Select<MessageSearchResult, MessageResultViewModel>((Func<MessageSearchResult, MessageResultViewModel>) (qRes =>
      {
        Conversation convo = (Conversation) null;
        if (!this.cachedChats.TryGetValue(qRes.Message.KeyRemoteJid, out convo))
        {
          convo = db.GetConversation(qRes.Message.KeyRemoteJid, CreateOptions.None);
          this.cachedChats[qRes.Message.KeyRemoteJid] = convo;
        }
        if (convo == null)
          return (MessageResultViewModel) null;
        return new MessageResultViewModel(convo, qRes)
        {
          EnableContextMenu = false
        };
      })).Where<MessageResultViewModel>((Func<MessageResultViewModel, bool>) (vm => vm != null)).ToArray<MessageResultViewModel>();
    }

    private ChatSearchPage.SearchResult SearchWaContacts(string query, string ftsQuery)
    {
      ChatSearchPage.SearchResult searchResult = new ChatSearchPage.SearchResult(query);
      searchResult.TargetTab = ChatSearchPage.Tabs.Contacts;
      UserStatusSearchResult[] queryResults = (UserStatusSearchResult[]) null;
      ContactsContext.Instance((Action<ContactsContext>) (db => queryResults = db.QueryUserStatusesFtsTableWithOffsets(ftsQuery)));
      searchResult.AddResultViewModels((IEnumerable<JidItemViewModel>) ((IEnumerable<UserStatusSearchResult>) queryResults).Select<UserStatusSearchResult, UserViewModel>((Func<UserStatusSearchResult, UserViewModel>) (qRes =>
      {
        UserViewModel userViewModel = new UserViewModel(qRes.UserStatus);
        userViewModel.EnableContextMenu = false;
        userViewModel.SetSearchResult(qRes);
        return userViewModel;
      })).ToArray<UserViewModel>());
      return searchResult;
    }

    private void DisposeAll()
    {
      this.currSearchSub.SafeDispose();
      this.loadMoreMsgSub.SafeDispose();
      this.delaySub.SafeDispose();
      this.currSearchSub = this.loadMoreMsgSub = this.delaySub = (IDisposable) null;
      IDisposable[] array = this.disposables.ToArray();
      this.disposables.Clear();
      foreach (IDisposable d in array)
        d.SafeDispose();
    }

    private void CloseKeyboard()
    {
      this.Focus();
      this.SearchField.CloseEmojiKeyboard();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e) => base.OnNavigatedTo(e);

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      base.OnRemovedFromJournal(e);
      this.DisposeAll();
    }

    private void OnLoaded(object sender, RoutedEventArgs e) => this.SearchField.Focus();

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
    }

    private void MessageResultsList_ItemRealized(object sender, ItemRealizationEventArgs e)
    {
      if (!this.enableAutoLoadMoreMsgs)
        return;
      JidItemViewModel content = e.Container.Content as JidItemViewModel;
      JidItemViewModel lastMsgVm = this.lastMsgVm as JidItemViewModel;
      if (content == null || lastMsgVm == null || !(content.Key == lastMsgVm.Key))
        return;
      this.LoadMoreMessages();
    }

    private void ResultsList_ManipulationStarted(object sender, EventArgs e)
    {
      this.CloseKeyboard();
    }

    private void SearchField_TextChanged(TextChangedEventArgs e)
    {
      string rawTerm = this.SearchField.Text ?? "";
      ChatSearchPage.Tabs targetTab = this.currTab;
      if (!this.SetNewSearchTerms(rawTerm))
        return;
      DateTime utcNow = DateTime.UtcNow;
      IDisposable delaySub = this.delaySub;
      double totalMilliseconds = (utcNow - this.lastTextChangedAt).TotalMilliseconds;
      this.lastTextChangedAt = utcNow;
      int num = rawTerm.Length <= 3 ? 0 : (totalMilliseconds <= 500.0 || this.delaySub != null ? (totalMilliseconds <= 1000.0 ? 0 : (this.delaySub != null ? 1 : 0)) : 1);
      this.delaySub.SafeDispose();
      this.delaySub = (IDisposable) null;
      if (num != 0)
      {
        Log.d("searchpage", "start search | search term:[{0}]", (object) this.currSearchQuery);
        this.Search(targetTab);
      }
      else
      {
        this.delaySub = Observable.Timer(TimeSpan.FromMilliseconds(500.0)).ObserveOnDispatcher<long>().Subscribe<long>((Action<long>) (_ =>
        {
          Log.d("searchpage", "start search | delayed | search term:[{0}]", (object) this.currSearchQuery);
          this.Search(targetTab);
          this.delaySub.SafeDispose();
          this.delaySub = (IDisposable) null;
        }));
        Log.d("searchpage", "delayed search | search term:[{0}]", (object) this.currSearchQuery);
      }
    }

    private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      object selectedItem = this.Pivot.SelectedItem;
      if (selectedItem == null)
        return;
      if (selectedItem == this.ChatsPivotItem)
        this.currTab = ChatSearchPage.Tabs.Chats;
      else if (selectedItem == this.MessagesPivotItem)
        this.currTab = ChatSearchPage.Tabs.Messages;
      else if (selectedItem == this.ContactsPivotItem)
        this.currTab = ChatSearchPage.Tabs.Contacts;
      ChatSearchPage.SearchResult res = (ChatSearchPage.SearchResult) null;
      if (this.unrenderedResults.TryGetValue(this.currTab, out res) && res != null)
      {
        this.unrenderedResults[this.currTab] = (ChatSearchPage.SearchResult) null;
        this.RenderSearchResult(res);
      }
      else
      {
        if (this.currSearchQuery != null)
          return;
        this.Dispatcher.BeginInvoke((Action) (() => this.SearchField.Focus()));
      }
    }

    private void OnSearchResult(ChatSearchPage.SearchResult res)
    {
      if (res == null)
        return;
      lock (this.currSearchLock)
      {
        if (res.SearchQuery != this.currSearchQuery)
        {
          Log.d("searchpage", "ignored search result | {0} vs. {1}", (object) res.SearchQuery, (object) this.currSearchQuery);
          return;
        }
      }
      Log.d("searchpage", "process search result | q:{0},target:{1}", (object) res.SearchQuery, (object) res.TargetTab);
      if (res.TargetTab == this.currTab)
        this.RenderSearchResult(res);
      else
        this.unrenderedResults[res.TargetTab] = res;
      this.SetTabHeader(res.TargetTab, res.Count);
    }

    private void OnMoreMessageSearchResult(ChatSearchPage.MessageTabSearchResult moreMsgRes)
    {
      if (moreMsgRes == null)
        return;
      bool flag = false;
      lock (this.currSearchLock)
        flag = this.currSearchQuery == moreMsgRes.SearchQuery;
      ChatSearchPage.SearchResult searchResult = (ChatSearchPage.SearchResult) null;
      if (!flag || !this.renderedResults.TryGetValue(ChatSearchPage.Tabs.Messages, out searchResult) || searchResult == null || !(searchResult.SearchQuery == moreMsgRes.SearchQuery) || searchResult.Count <= 0 || moreMsgRes.Count <= 0)
        return;
      if (searchResult is ChatSearchPage.MessageTabSearchResult messageTabSearchResult)
        messageTabSearchResult.AddMessageResultViewModels((IEnumerable<JidItemViewModel>) moreMsgRes.MessageResultViewModels);
      this.lastMsgVm = (object) messageTabSearchResult.MessageResultViewModels.LastOrDefault<JidItemViewModel>();
      this.SetTabHeader(messageTabSearchResult.TargetTab, messageTabSearchResult.Count);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/ChatSearchPage.xaml", UriKind.Relative));
      this.LayoutRootZoomBox = (ZoomBox) this.FindName("LayoutRootZoomBox");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.SearchField = (EmojiTextBox) this.FindName("SearchField");
      this.Pivot = (Pivot) this.FindName("Pivot");
      this.ChatsPivotItem = (PivotItem) this.FindName("ChatsPivotItem");
      this.ChatResultsList = (WhatsApp.CompatibilityShims.LongListSelector) this.FindName("ChatResultsList");
      this.ChatsFooterTooltip = (TextBlock) this.FindName("ChatsFooterTooltip");
      this.MessagesPivotItem = (PivotItem) this.FindName("MessagesPivotItem");
      this.MessageResultsList = (WhatsApp.CompatibilityShims.LongListSelector) this.FindName("MessageResultsList");
      this.MessagesFooterTooltip = (TextBlock) this.FindName("MessagesFooterTooltip");
      this.ContactsPivotItem = (PivotItem) this.FindName("ContactsPivotItem");
      this.ContactResultsList = (WhatsApp.CompatibilityShims.LongListSelector) this.FindName("ContactResultsList");
      this.ContactsFooterTooltip = (TextBlock) this.FindName("ContactsFooterTooltip");
    }

    private enum Tabs
    {
      Undefined,
      Chats,
      Messages,
      Contacts,
    }

    private class SearchResult
    {
      protected ObservableCollection<JidItemViewModel> resultViewModels;

      public ChatSearchPage.Tabs TargetTab { get; set; }

      public string SearchQuery { get; set; }

      public ObservableCollection<JidItemViewModel> ResultViewModels
      {
        get => this.resultViewModels ?? new ObservableCollection<JidItemViewModel>();
      }

      public virtual int Count => this.resultViewModels != null ? this.resultViewModels.Count : 0;

      public bool HasNoResults => this.SearchQuery != null && this.Count <= 0;

      public SearchResult(string query) => this.SearchQuery = query;

      public virtual void AddResultViewModels(IEnumerable<JidItemViewModel> vms)
      {
        if (vms == null)
          return;
        if (this.resultViewModels == null)
        {
          this.resultViewModels = new ObservableCollection<JidItemViewModel>(vms);
        }
        else
        {
          foreach (JidItemViewModel vm in vms)
            this.resultViewModels.Add(vm);
        }
      }
    }

    private class MessageTabSearchResult : ChatSearchPage.SearchResult
    {
      private KeyedObservableCollection<string, JidItemViewModel> msgResultVms;
      private KeyedObservableCollection<string, JidItemViewModel> starredMsgResultVms;

      public bool ShouldRenderAsFlatList
      {
        get
        {
          return this.msgResultVms == null || !this.msgResultVms.Any<JidItemViewModel>() || this.starredMsgResultVms == null || !this.starredMsgResultVms.Any<JidItemViewModel>();
        }
      }

      public KeyedObservableCollection<string, JidItemViewModel> MessageResultViewModels
      {
        get
        {
          return this.msgResultVms ?? new KeyedObservableCollection<string, JidItemViewModel>(AppResources.MessagesUpper, (IEnumerable<JidItemViewModel>) new JidItemViewModel[0]);
        }
        set => this.msgResultVms = value;
      }

      public KeyedObservableCollection<string, JidItemViewModel> StarredMessageResultViewModels
      {
        get
        {
          return this.starredMsgResultVms ?? new KeyedObservableCollection<string, JidItemViewModel>(AppResources.StarredMessagesUpper, (IEnumerable<JidItemViewModel>) new JidItemViewModel[0]);
        }
        set => this.starredMsgResultVms = value;
      }

      public override int Count
      {
        get
        {
          return (this.starredMsgResultVms == null ? 0 : this.starredMsgResultVms.Count) + (this.msgResultVms == null ? 0 : this.msgResultVms.Count);
        }
      }

      public MessageTabSearchResult(string query)
        : base(query)
      {
      }

      public override void AddResultViewModels(IEnumerable<JidItemViewModel> vms)
      {
      }

      public void AddMessageResultViewModels(IEnumerable<JidItemViewModel> vms)
      {
        if (vms == null)
          return;
        if (this.msgResultVms == null)
        {
          this.msgResultVms = new KeyedObservableCollection<string, JidItemViewModel>(AppResources.MessagesUpper, vms);
        }
        else
        {
          foreach (JidItemViewModel vm in vms)
            this.msgResultVms.Add(vm);
        }
      }

      public void AddStarredMessageResultViewModels(IEnumerable<JidItemViewModel> vms)
      {
        if (vms == null)
          return;
        if (this.starredMsgResultVms == null)
        {
          this.starredMsgResultVms = new KeyedObservableCollection<string, JidItemViewModel>(AppResources.StarredMessagesUpper, vms);
        }
        else
        {
          foreach (JidItemViewModel vm in vms)
            this.starredMsgResultVms.Add(vm);
        }
      }
    }
  }
}
