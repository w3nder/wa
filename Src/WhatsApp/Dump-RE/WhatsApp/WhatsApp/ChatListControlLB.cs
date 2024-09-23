// Decompiled with JetBrains decompiler
// Type: WhatsApp.ChatListControlLB
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WhatsApp.WaViewModels;

#nullable disable
namespace WhatsApp
{
  public class ChatListControlLB : UserControl, IChatListControl
  {
    private const int asyncLoadingInitialBatch = 10;
    private ChatItemViewModelCollection chatCollection;
    private List<ConversationItem> pendingItems;
    private ChatItemViewModel lastSelectedItem;
    private Subject<Conversation> chatRequestedSubj = new Subject<Conversation>();
    private Subject<Unit> multiSelectionChangedSubj = new Subject<Unit>();
    private Subject<bool> pendingSelectionSubj = new Subject<bool>();
    private bool isMultiSelectionEnabled;
    private bool pendingSelection;
    private IDisposable pendingAnimationSub;
    internal Style ListItemTransform;
    internal Grid LayoutRoot;
    internal StackPanel TooltipPanel;
    internal TextBlock TooltipBlock;
    internal ListBox ChatList;
    private bool _contentLoaded;

    public bool IsMultiSelectionEnabled
    {
      get => this.IsMultiSelectionAllowed && this.isMultiSelectionEnabled;
      set
      {
        if (!this.IsMultiSelectionAllowed)
          return;
        this.isMultiSelectionEnabled = value;
        this.multiSelectionChangedSubj.OnNext(new Unit());
      }
    }

    public bool IsMultiSelectionAllowed { get; set; }

    public void EnableMultiSelection(bool enable)
    {
      if (this.IsMultiSelectionEnabled == enable || !this.IsMultiSelectionAllowed)
        return;
      if (this.chatCollection != null)
      {
        if (enable)
          this.chatCollection.GetViewModels().All<ChatItemViewModel>((Func<ChatItemViewModel, bool>) (vm =>
          {
            vm.EnableContextMenu = false;
            return true;
          }));
        else
          this.chatCollection.GetViewModels().All<ChatItemViewModel>((Func<ChatItemViewModel, bool>) (vm =>
          {
            vm.EnableContextMenu = true;
            vm.IsSelected = false;
            return true;
          }));
      }
      this.IsMultiSelectionEnabled = enable;
    }

    public ChatListControlLB() => this.InitializeComponent();

    public IObservable<Conversation> ChatRequestedObservable()
    {
      return (IObservable<Conversation>) this.chatRequestedSubj;
    }

    public IObservable<Unit> MultiSelectionChangedObservable()
    {
      return (IObservable<Unit>) this.multiSelectionChangedSubj;
    }

    public IObservable<bool> PendingSelectionObservable()
    {
      return (IObservable<bool>) this.pendingSelectionSubj;
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
          this.ChatList.ItemsSource = (IEnumerable) null;
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

    private void InitList()
    {
      if (this.chatCollection == null)
      {
        this.ChatList.ItemsSource = (IEnumerable) null;
      }
      else
      {
        if (this.ChatList.ItemsSource != null)
          return;
        this.ChatList.ItemsSource = (IEnumerable) this.chatCollection.GetViewModels();
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
      return this.chatCollection.GetViewModels().Where<ChatItemViewModel>((Func<ChatItemViewModel, bool>) (vm => vm.IsSelected)).Select<ChatItemViewModel, string>((Func<ChatItemViewModel, string>) (vm => vm.Conversation.Jid)).ToList<string>();
    }

    private void ChatListItem_Tap(object sender, GestureEventArgs e)
    {
      if (!((sender as ChatItemControl).ViewModel is ChatItemViewModel viewModel) || viewModel.Conversation?.Jid == null)
        return;
      Log.d("chat list", "chat list item tapped | jid={0}", (object) viewModel.Conversation.Jid);
      if (this.IsMultiSelectionEnabled)
      {
        this.ToggleSelection((JidItemViewModel) viewModel);
      }
      else
      {
        Conversation conversation = viewModel.Conversation;
        this.lastSelectedItem = viewModel;
        Log.d("chat list", "start chat loading | jid={0}", (object) viewModel.Conversation.Jid);
        MessageLoader messageLoader = MessageLoader.Get(conversation.Jid, conversation.FirstUnreadMessageID, conversation.GetUnreadMessagesCount(), false);
        System.Windows.Media.ImageSource cached = (System.Windows.Media.ImageSource) null;
        viewModel.GetCachedPicSource(out cached);
        ChatPage.NextInstanceInitState = new ChatPage.InitState()
        {
          MessageLoader = messageLoader,
          Picture = cached
        };
        this.chatRequestedSubj.OnNext(conversation);
      }
    }

    private void OnSelectedChatItem(JidItemViewModel jidItemViewModel)
    {
      this.EnableMultiSelection(true);
      if (!this.IsMultiSelectionEnabled)
        return;
      this.ToggleSelection(jidItemViewModel);
    }

    public void OnChatCollectionChanged()
    {
      if (!this.IsMultiSelectionEnabled)
        return;
      if (!this.chatCollection.GetViewModels().Any<ChatItemViewModel>((Func<ChatItemViewModel, bool>) (vm => vm.IsSelected)))
        this.EnableMultiSelection(false);
      this.multiSelectionChangedSubj.OnNext(new Unit());
    }

    private void ToggleSelection(JidItemViewModel jidItemViewModel, bool? isSelected = null)
    {
      bool flag = ((int) isSelected ?? (!jidItemViewModel.IsSelected ? 1 : 0)) != 0;
      jidItemViewModel.IsSelected = flag;
      if (!this.chatCollection.GetViewModels().Any<ChatItemViewModel>((Func<ChatItemViewModel, bool>) (vm => vm.IsSelected)))
        this.EnableMultiSelection(false);
      this.multiSelectionChangedSubj.OnNext(new Unit());
    }

    public void Clear()
    {
      this.SetChats((ChatItemViewModelCollection) null, (List<ConversationItem>) null);
      this.ChatList.ItemsSource = (IEnumerable) null;
    }

    public void ScrollToTop()
    {
      UIUtils.FindFirstInVisualTreeByType<ScrollViewer>((DependencyObject) this.ChatList)?.ScrollToVerticalOffset(0.0);
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

    public void EnsurePreviousSelectedChatVisible()
    {
      if (this.chatCollection == null)
        return;
      ChatItemViewModel item = this.lastSelectedItem;
      if (item == null || this.chatCollection.GetViewModels().IndexOf(item) == -1)
        return;
      EventHandler postLayout = (EventHandler) null;
      bool handled = false;
      postLayout = (EventHandler) ((sender, args) =>
      {
        if (handled)
          return;
        handled = true;
        this.ChatList.LayoutUpdated -= postLayout;
        this.ChatList.ScrollIntoView((object) item);
      });
      this.ChatList.LayoutUpdated += postLayout;
    }

    private void HintPanel_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
    {
      FrameworkElement hintPanel = sender as FrameworkElement;
      if (!((sender is FrameworkElement frameworkElement ? frameworkElement.DataContext : (object) null) is JidItemViewModel dataContext) || dataContext.Jid == null)
        return;
      Log.d("chat list item", "hint panel manipulation started | jid={0}", (object) dataContext.Jid);
      this.pendingSelection = true;
      this.pendingSelectionSubj.OnNext(this.pendingSelection);
      if (dataContext.IsSelected)
        return;
      this.FadeInFrameworkElement(hintPanel);
    }

    private void HintPanel_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
    {
      FrameworkElement hintPanel = sender as FrameworkElement;
      if (((sender is FrameworkElement frameworkElement ? frameworkElement.DataContext : (object) null) is JidItemViewModel dataContext ? dataContext.Jid : (string) null) == null || !this.pendingSelection)
        return;
      System.Windows.Input.ManipulationDelta cumulativeManipulation1 = e.CumulativeManipulation;
      System.Windows.Point point;
      double num1;
      if (cumulativeManipulation1 == null)
      {
        num1 = 0.0;
      }
      else
      {
        point = cumulativeManipulation1.Translation;
        num1 = point.X;
      }
      double num2 = num1;
      System.Windows.Input.ManipulationDelta cumulativeManipulation2 = e.CumulativeManipulation;
      double num3;
      if (cumulativeManipulation2 == null)
      {
        num3 = 0.0;
      }
      else
      {
        point = cumulativeManipulation2.Translation;
        num3 = point.Y;
      }
      double num4 = num3;
      if (Math.Abs(num2) <= hintPanel.ActualWidth && Math.Abs(num4) <= 4.0)
        return;
      object[] objArray = new object[4];
      point = e.ManipulationOrigin;
      objArray[0] = (object) point.X;
      point = e.ManipulationOrigin;
      objArray[1] = (object) point.Y;
      objArray[2] = (object) num2;
      objArray[3] = (object) num4;
      Log.d("chat list item", "hint panel manipulation delta | originX={0}, originY={1}, translationX={2}, translationY={3}", objArray);
      this.FadeOutFrameworkElement(hintPanel);
      this.pendingSelection = false;
      e.Handled = true;
    }

    private void HintPanel_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
    {
      FrameworkElement hintPanel = sender as FrameworkElement;
      if (!((sender is FrameworkElement frameworkElement ? frameworkElement.DataContext : (object) null) is JidItemViewModel dataContext) || dataContext.Jid == null)
        return;
      Log.d("chat list item", "hint panel manipulation complete | jid={0}", (object) dataContext.Jid);
      Log.d("chat list item", "pending selection is {0}, opacity for hint panel is {1}", (object) this.pendingSelection, (object) hintPanel.Opacity);
      if (this.pendingSelection)
      {
        this.FadeOutFrameworkElement(hintPanel);
        this.OnSelectedChatItem(dataContext);
        this.pendingSelection = false;
      }
      this.pendingSelectionSubj.OnNext(this.pendingSelection);
    }

    private void FadeInFrameworkElement(FrameworkElement hintPanel, int millis = 400)
    {
      this.pendingAnimationSub.SafeDispose();
      this.pendingAnimationSub = (IDisposable) null;
      Storyboard storyboard = WaAnimations.CreateStoryboard(WaAnimations.Fade(WaAnimations.FadeType.FadeIn, TimeSpan.FromMilliseconds((double) millis), (DependencyObject) hintPanel));
      Log.d("chat list item", "hint panel fading in");
      this.pendingAnimationSub = Storyboarder.PerformWithDisposable(storyboard, (DependencyObject) null, true, (Action) (() =>
      {
        hintPanel.Opacity = 1.0;
        this.pendingAnimationSub = (IDisposable) null;
      }), false, "hint panel fade in");
    }

    private void FadeOutFrameworkElement(FrameworkElement hintPanel, int millis = 400)
    {
      this.pendingAnimationSub.SafeDispose();
      this.pendingAnimationSub = (IDisposable) null;
      Storyboard storyboard = WaAnimations.CreateStoryboard(WaAnimations.Fade(hintPanel.Opacity, 0.0, TimeSpan.FromMilliseconds((double) millis), (DependencyObject) hintPanel));
      Log.d("chat list item", "hint panel fading out");
      this.pendingAnimationSub = Storyboarder.PerformWithDisposable(storyboard, (DependencyObject) null, true, (Action) (() =>
      {
        hintPanel.Opacity = 0.0;
        this.pendingAnimationSub = (IDisposable) null;
      }), false, "hint panel fade out");
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/ChatListControlLB.xaml", UriKind.Relative));
      this.ListItemTransform = (Style) this.FindName("ListItemTransform");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.TooltipPanel = (StackPanel) this.FindName("TooltipPanel");
      this.TooltipBlock = (TextBlock) this.FindName("TooltipBlock");
      this.ChatList = (ListBox) this.FindName("ChatList");
    }
  }
}
