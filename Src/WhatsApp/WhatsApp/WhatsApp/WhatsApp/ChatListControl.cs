// Decompiled with JetBrains decompiler
// Type: WhatsApp.ChatListControl
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using WhatsApp.Controls;
using WhatsApp.WaViewModels;


namespace WhatsApp
{
  public class ChatListControl : UserControl, IChatListControl
  {
    private const int asyncLoadingInitialBatch = 10;
    private ChatItemViewModelCollection chatCollection;
    private List<ConversationItem> pendingItems;
    private ViewportControl viewport;
    private ChatItemViewModel selectedChatItem;
    private int selectedChatIndex;
    private Dictionary<object, WeakReference<FrameworkElement>> realizedItems = new Dictionary<object, WeakReference<FrameworkElement>>();
    private Subject<Conversation> chatRequestedSubj = new Subject<Conversation>();
    private Subject<Unit> multiSelectionChangedSubj = new Subject<Unit>();
    private Subject<bool> pendingSelectionSubj = new Subject<bool>();
    internal StackPanel TooltipPanel;
    internal TextBlock TooltipBlock;
    internal WhatsApp.Controls.LongListMultiSelector ChatList;
    internal StackPanel FooterPanel;
    private bool _contentLoaded;

    public void OnChatCollectionChanged() => throw new NotImplementedException();

    public bool IsMultiSelectionAllowed
    {
      get => this.ChatList.IsSelectionAllowed;
      set => this.ChatList.IsSelectionAllowed = value;
    }

    public bool IsMultiSelectionEnabled
    {
      get => this.ChatList.IsSelectionEnabled;
      set => this.ChatList.IsSelectionEnabled = value;
    }

    public ChatListControl()
    {
      this.InitializeComponent();
      this.ChatList.IsGroupingEnabled = false;
      this.ChatList.SingleSelectionChanged += new EventHandler<SingleSelectionChangedArgs>(this.ChatList_SingleSelectionChanged);
      this.ChatList.MultiSelectionsChanged += new SelectionChangedEventHandler(this.ChatList_MultiSelectionsChanged);
      this.ChatList.ItemRealized += new EventHandler<ItemRealizationEventArgs>(this.ChatList_ItemRealized);
      this.ChatList.ItemUnrealized += new EventHandler<ItemRealizationEventArgs>(this.ChatList_ItemUnrealized);
      this.ChatList.OverlapScrollBar = true;
    }

    public void Clear()
    {
      this.SetChats((ChatItemViewModelCollection) null, (List<ConversationItem>) null);
      this.ChatList.ItemsSource = (IList) null;
    }

    private void SetChats(ChatItemViewModelCollection chats, List<ConversationItem> pending)
    {
      this.chatCollection.SafeDispose();
      this.chatCollection = chats;
      this.pendingItems = pending;
    }

    public void SetSourceAsync(
      IObservable<List<ConversationItem>> itemsObs,
      Func<Conversation, bool> chatFilter,
      Action<ChatCollection> onComplete,
      string context)
    {
      IDisposable sub = (IDisposable) null;
      sub = itemsObs.SubscribeOn<List<ConversationItem>>(WAThreadPool.Scheduler).ObserveOn<List<ConversationItem>>(WAThreadPool.Scheduler).Subscribe<List<ConversationItem>>((Action<List<ConversationItem>>) (items =>
      {
        if (items == null)
          items = new List<ConversationItem>();
        int count = AppState.IsDecentMemoryDevice ? 10 : 1;
        List<ConversationItem> list = items.Skip<ConversationItem>(count).Reverse<ConversationItem>().ToList<ConversationItem>();
        items = items.Take<ConversationItem>(count).ToList<ConversationItem>();
        ChatItemViewModelCollection vms = (ChatItemViewModelCollection) null;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          vms = new ChatItemViewModelCollection(db, (IEnumerable<ConversationItem>) items, chatFilter, context)
          {
            EnableUpdate = false
          };
        }));
        this.SetChats(vms, list);
        this.Dispatcher.BeginInvoke((Action) (() =>
        {
          this.ChatList.ItemsSource = (IList) null;
          this.InitList();
        }));
        if (onComplete != null)
          onComplete((ChatCollection) this.chatCollection);
        sub.SafeDispose();
        sub = (IDisposable) null;
      }), (Action<Exception>) (ex =>
      {
        Log.LogException(ex, "chat list set source");
        sub.SafeDispose();
        sub = (IDisposable) null;
      }), (Action) (() =>
      {
        sub.SafeDispose();
        sub = (IDisposable) null;
      }));
    }

    public IObservable<Conversation> ChatRequestedObservable()
    {
      return (IObservable<Conversation>) this.chatRequestedSubj;
    }

    public IObservable<Unit> MultiSelectionChangedObservable()
    {
      return (IObservable<Unit>) this.multiSelectionChangedSubj;
    }

    private IObservable<bool> PendingSelectionObservable()
    {
      return (IObservable<bool>) this.pendingSelectionSubj;
    }

    private void InitList()
    {
      if (this.chatCollection == null)
      {
        this.ChatList.ItemsSource = (IList) null;
      }
      else
      {
        if (this.ChatList.ItemsSource != null)
          return;
        this.ChatList.ItemsSource = (IList) this.chatCollection.GetViewModels();
        this.ReducePending();
      }
    }

    private void ReducePending()
    {
      if (this.pendingItems == null || !this.pendingItems.Any<ConversationItem>())
      {
        this.pendingItems = (List<ConversationItem>) null;
        if (this.chatCollection == null)
          return;
        this.chatCollection.EnableUpdate = true;
        this.chatCollection.GetViewModels().CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnCollectionChanged);
      }
      else
      {
        if (this.chatCollection == null)
          return;
        this.chatCollection.EnableUpdate = false;
        if (this.chatCollection.Count > 10)
        {
          ConversationItem[] array = ((IEnumerable<ConversationItem>) this.pendingItems.ToArray()).Reverse<ConversationItem>().ToArray<ConversationItem>();
          this.pendingItems = (List<ConversationItem>) null;
          foreach (ConversationItem ci in array)
            this.chatCollection.Append(ci);
        }
        else
        {
          this.chatCollection.Append(this.pendingItems.LastOrDefault<ConversationItem>());
          this.pendingItems.RemoveAt(this.pendingItems.Count - 1);
        }
        this.Dispatcher.BeginInvoke(new Action(this.ReducePending));
      }
    }

    public List<string> GetMultiSelectedChats()
    {
      return !this.ChatList.IsSelectionEnabled ? new List<string>() : this.ChatList.SelectedItems.Cast<ChatItemViewModel>().Where<ChatItemViewModel>((Func<ChatItemViewModel, bool>) (vm => vm != null && vm.Conversation != null && vm.Conversation.Jid != null)).Select<ChatItemViewModel, string>((Func<ChatItemViewModel, string>) (vm => vm.Conversation.Jid)).ToList<string>();
    }

    public void SetTooltip(string s)
    {
      if (string.IsNullOrEmpty(s))
      {
        this.TooltipPanel.Visibility = Visibility.Collapsed;
      }
      else
      {
        this.TooltipBlock.SetValue(TextOptions.DisplayColorEmojiProperty, (object) false);
        this.TooltipBlock.Text = s;
        this.TooltipPanel.Visibility = Visibility.Visible;
      }
    }

    private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (e.Action != NotifyCollectionChangedAction.Add)
        return;
      this.ShiftViewport();
    }

    private void ShiftViewport()
    {
      if (this.viewport == null)
        this.viewport = TemplatedVisualTreeExtensions.GetFirstLogicalChildByType<ViewportControl>(this.ChatList, false);
      if (this.viewport == null)
        return;
      Rect r = this.viewport.Bounds;
      EventHandler shiftAction = (EventHandler) null;
      shiftAction = (EventHandler) ((sender, args) =>
      {
        if (this.viewport.Bounds.Y < r.Y)
          this.viewport.SetViewportOrigin(new System.Windows.Point(this.viewport.Viewport.X, this.viewport.Viewport.Y + (this.viewport.Bounds.Y - r.Y)));
        this.ChatList.LayoutUpdated -= shiftAction;
      });
      this.ChatList.LayoutUpdated += shiftAction;
    }

    private void ChatList_MultiSelectionsChanged(object sender, SelectionChangedEventArgs e)
    {
      this.multiSelectionChangedSubj.OnNext(new Unit());
    }

    private void ChatList_SingleSelectionChanged(object sender, SingleSelectionChangedArgs e)
    {
      this.OnSingleSelectionChanged(e.Selected);
    }

    private void OnSingleSelectionChanged(object selected)
    {
      if (!(selected is ChatItemViewModel chatItemViewModel) || chatItemViewModel.Conversation == null)
        return;
      Conversation conversation = chatItemViewModel.Conversation;
      this.selectedChatItem = chatItemViewModel;
      this.selectedChatIndex = this.chatCollection.GetViewModels().IndexOf(chatItemViewModel);
      Log.d("chat list", "start chat loading | jid={0}", (object) chatItemViewModel.Conversation.Jid);
      MessageLoader messageLoader = MessageLoader.Get(conversation.Jid, conversation.FirstUnreadMessageID, conversation.GetUnreadMessagesCount(), false);
      System.Windows.Media.ImageSource cached = (System.Windows.Media.ImageSource) null;
      chatItemViewModel.GetCachedPicSource(out cached);
      ChatPage.NextInstanceInitState = new ChatPage.InitState()
      {
        MessageLoader = messageLoader,
        Picture = cached
      };
      this.chatRequestedSubj.OnNext(conversation);
    }

    private void ChatList_ItemRealized(object sender, ItemRealizationEventArgs e)
    {
      this.realizedItems[e.Container.Content] = new WeakReference<FrameworkElement>((FrameworkElement) e.Container);
    }

    private void ChatList_ItemUnrealized(object sender, ItemRealizationEventArgs e)
    {
      this.realizedItems.Remove(e.Container.Content);
    }

    public void ScrollToTop()
    {
      if (this.chatCollection == null)
        return;
      ObservableCollection<ChatItemViewModel> viewModels = this.chatCollection.GetViewModels();
      if (viewModels == null)
        return;
      ChatItemViewModel chatItemViewModel = viewModels.FirstOrDefault<ChatItemViewModel>();
      if (chatItemViewModel == null)
        return;
      try
      {
        this.ChatList.ScrollTo((object) chatItemViewModel);
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "scroll to chat list top");
      }
    }

    public void EnsurePreviousSelectedChatVisible()
    {
      ChatItemViewModel selItem = this.selectedChatItem;
      if (selItem == null)
        return;
      int newIndex = this.chatCollection.GetViewModels().IndexOf(selItem);
      int oldIndex = this.selectedChatIndex;
      if (oldIndex == -1 || newIndex == -1)
        return;
      EventHandler postLayout = (EventHandler) null;
      postLayout = (EventHandler) ((sender, args) =>
      {
        this.ChatList.LayoutUpdated -= postLayout;
        if (this.viewport == null)
          this.viewport = TemplatedVisualTreeExtensions.GetFirstLogicalChildByType<ViewportControl>(this.ChatList, false);
        FrameworkElement target = (FrameworkElement) null;
        if (this.realizedItems.ContainsKey((object) selItem))
          this.realizedItems[(object) selItem].TryGetTarget(out target);
        if (target == null)
          return;
        System.Windows.Point location = new System.Windows.Point(this.viewport.Viewport.X, this.viewport.Viewport.Y - (double) (newIndex - oldIndex) * target.ActualHeight);
        double top = Canvas.GetTop((UIElement) target);
        location.Y = Math.Min(location.Y, top);
        location.Y = Math.Max(location.Y, top + target.ActualHeight - this.viewport.Viewport.Height);
        if (location.Y == this.viewport.Viewport.Y)
          return;
        this.viewport.SetViewportOrigin(location);
      });
      this.ChatList.LayoutUpdated += postLayout;
    }

    public void EnableMultiSelection(bool enable) => throw new NotImplementedException();

    IObservable<bool> IChatListControl.PendingSelectionObservable()
    {
      throw new NotImplementedException();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/ChatListControl.xaml", UriKind.Relative));
      this.TooltipPanel = (StackPanel) this.FindName("TooltipPanel");
      this.TooltipBlock = (TextBlock) this.FindName("TooltipBlock");
      this.ChatList = (WhatsApp.Controls.LongListMultiSelector) this.FindName("ChatList");
      this.FooterPanel = (StackPanel) this.FindName("FooterPanel");
    }
  }
}
