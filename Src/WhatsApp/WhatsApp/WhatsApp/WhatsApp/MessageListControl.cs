// Decompiled with JetBrains decompiler
// Type: WhatsApp.MessageListControl
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace WhatsApp
{
  public class MessageListControl : UserControl, IMessageListControl
  {
    private MessageViewModelCollection msgViewModels = new MessageViewModelCollection();
    private FrameworkElement msgListElem;
    private WhatsApp.Controls.LongListMultiSelector msgListMLLS;
    private WhatsApp.CompatibilityShims.LongListSelector msgListLLS;
    private MessageViewModel nextTargetLandingItem;
    private bool EnableOverlay;
    private DateDividerViewModel overlayDateViewModel;
    private bool enableScrollButton = true;
    private Subject<Message> newerMsgRealizedSubj;
    private bool newerRealizedMsgNoticationPending;
    private ScrollBar scrollbar;
    private ViewportControl scroller;
    private FrameworkElement currentFirstVisibleMessage;
    private Message realizedNewestMsg;
    private Dictionary<MessageViewModel, WeakReference<FrameworkElement>> realizedItems = new Dictionary<MessageViewModel, WeakReference<FrameworkElement>>();
    private HashSet<int> realizedMessageIDs = new HashSet<int>();
    private bool dateDividerOverlayVisible;
    private DispatcherTimer hideOverlayTimer;
    private bool HasMediaElementMessages;
    private double lastHandledMediaElementPos = double.MinValue;
    private MessageListControl.LayoutUpdatedScrollState scrollState;
    private double preLayoutUpdatedBounds;
    internal Storyboard OverlayDateStoryboard;
    internal DoubleAnimation OverlayDateAnimation;
    internal Grid ContentPanel;
    internal MessageBubbleContainer DateDividerOverlayContainer;
    internal CompositeTransform DateDividerOverlayXForm;
    internal Grid ScrollButton;
    internal Rectangle ScrollButtonBackground;
    internal Image ScrollButtonIcon;
    private bool _contentLoaded;

    public DataTemplate ItemTemplate
    {
      set
      {
        if (this.msgListMLLS != null)
        {
          this.msgListMLLS.ItemTemplate = value;
        }
        else
        {
          if (this.msgListLLS == null)
            return;
          this.msgListLLS.ItemTemplate = value;
        }
      }
    }

    public Message LastTappedQuotedMsg { get; set; }

    public Message LastTappedReplyMsg { get; set; }

    public bool EnableScrollButton
    {
      get => this.enableScrollButton;
      set
      {
        if (this.enableScrollButton == value)
          return;
        this.enableScrollButton = value;
        this.UpdateScrollButtonVisibility();
      }
    }

    public MessageListControl(bool useMLLS, bool renderInvertly, bool enableOverlay)
    {
      this.InitializeComponent();
      if (useMLLS)
      {
        WhatsApp.Controls.LongListMultiSelector listMultiSelector = new WhatsApp.Controls.LongListMultiSelector();
        listMultiSelector.Margin = new Thickness(0.0);
        listMultiSelector.IsGroupingEnabled = false;
        listMultiSelector.OverlapScrollBar = true;
        listMultiSelector.Style = this.Resources[(object) "MultiSelectableMessageListStyle"] as Style;
        listMultiSelector.CacheMode = (CacheMode) new BitmapCache();
        listMultiSelector.Background = (Brush) new SolidColorBrush(Colors.Transparent);
        listMultiSelector.ItemTemplate = this.Resources[(object) "MessageListItemTemplateMLLS"] as DataTemplate;
        Rectangle rectangle = new Rectangle();
        rectangle.Height = 10.0 * ResolutionHelper.ZoomMultiplier;
        listMultiSelector.ListHeader = (object) rectangle;
        listMultiSelector.ListFooter = (object) new Rectangle();
        WhatsApp.Controls.LongListMultiSelector source = listMultiSelector;
        TiltEffect.SetSuppressTilt((DependencyObject) source, true);
        source.MultiSelectionsChanged += new SelectionChangedEventHandler(this.MessageList_MultiSelectionsChanged);
        source.ItemRealized += new EventHandler<ItemRealizationEventArgs>(this.MessageList_ItemRealized);
        source.ItemUnrealized += new EventHandler<ItemRealizationEventArgs>(this.MessageList_ItemUnrealized);
        this.msgListElem = (FrameworkElement) (this.msgListMLLS = source);
      }
      else
      {
        WhatsApp.CompatibilityShims.LongListSelector longListSelector1 = new WhatsApp.CompatibilityShims.LongListSelector();
        longListSelector1.Margin = new Thickness(0.0);
        longListSelector1.IsFlatList = true;
        longListSelector1.ItemTemplate = this.Resources[(object) "MessageListItemTemplateLLS"] as DataTemplate;
        longListSelector1.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
        longListSelector1.RenderTransform = (Transform) new CompositeTransform()
        {
          ScaleY = (renderInvertly ? -1.0 : 1.0)
        };
        longListSelector1.OverlapScrollBar = true;
        longListSelector1.ListHeader = (object) new Rectangle();
        longListSelector1.ListFooter = (object) new Rectangle();
        WhatsApp.CompatibilityShims.LongListSelector longListSelector2 = longListSelector1;
        longListSelector2.ItemRealized += new EventHandler<ItemRealizationEventArgs>(this.MessageList_ItemRealized);
        longListSelector2.ItemUnrealized += new EventHandler<ItemRealizationEventArgs>(this.MessageList_ItemUnrealized);
        this.msgListElem = (FrameworkElement) (this.msgListLLS = longListSelector2);
      }
      this.IsRenderingInverted = renderInvertly;
      this.msgListElem.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.MessageList_Tap);
      this.msgListElem.Loaded += new RoutedEventHandler(this.MessageList_Loaded);
      this.msgListElem.Unloaded += new RoutedEventHandler(this.MessageList_Unloaded);
      this.ContentPanel.Children.Insert(0, (UIElement) this.msgListElem);
      this.EnableOverlay = enableOverlay;
      if (!this.EnableOverlay)
        return;
      Message m = new Message();
      m.MediaWaType = FunXMPP.FMessage.Type.System;
      DateTime now = DateTime.Now;
      m.FunTimestamp = new DateTime?(now.Date);
      now = DateTime.Now;
      this.overlayDateViewModel = new DateDividerViewModel(m, now.Date)
      {
        OverlayMode = true
      };
      this.DateDividerOverlayContainer.Margin = new Thickness(0.0, -4.0 * ResolutionHelper.ZoomMultiplier - 60.0, 0.0, 0.0);
      this.DateDividerOverlayContainer.ViewModel = (object) this.overlayDateViewModel;
    }

    private void MessageList_Loaded(object sender, RoutedEventArgs e)
    {
      this.Scroller.ViewportChanged += new EventHandler<ViewportChangedEventArgs>(this.Scroller_ViewportChanged);
      this.Scroller.ManipulationStateChanged += new EventHandler<ManipulationStateChangedEventArgs>(this.Scroller_ManipulationStateChanged);
      this.InitializeScrollButtonContainer();
    }

    private void MessageList_Unloaded(object sender, RoutedEventArgs e)
    {
      this.realizedItems.Clear();
    }

    public bool IsRenderingInverted { get; set; }

    public event EventHandler SelectionChanged;

    protected void NotifySelectionChanged()
    {
      if (this.SelectionChanged == null)
        return;
      this.SelectionChanged((object) this, new EventArgs());
    }

    public event EventHandler OlderMessagesRequested;

    protected void NotifyOlderMessagesRequested()
    {
      if (this.OlderMessagesRequested == null)
        return;
      this.OlderMessagesRequested((object) this, new EventArgs());
    }

    public event EventHandler NewerMessagesRequested;

    protected void NotifyNewerMessagesRequested()
    {
      if (this.NewerMessagesRequested == null)
        return;
      this.NewerMessagesRequested((object) this, new EventArgs());
    }

    public event EventHandler MultiSelectionsChanged;

    protected void NotifyMultiSelectionsChanged(SelectionChangedEventArgs e)
    {
      if (this.MultiSelectionsChanged == null)
        return;
      this.MultiSelectionsChanged((object) this, (EventArgs) e);
    }

    public IObservable<Message> NewerMessageRealized()
    {
      return (IObservable<Message>) this.newerMsgRealizedSubj ?? (IObservable<Message>) (this.newerMsgRealizedSubj = new Subject<Message>());
    }

    public bool IsMultiSelectionEnabled
    {
      get => this.msgListMLLS != null && this.msgListMLLS.IsSelectionEnabled;
      set
      {
        if (this.msgListMLLS == null)
          return;
        this.msgListMLLS.IsSelectionEnabled = value;
      }
    }

    public bool IsAnyMessagesSelected
    {
      get
      {
        return this.msgListMLLS != null && this.msgListMLLS.IsSelectionEnabled && this.msgListMLLS.SelectedItems.Count > 0;
      }
    }

    public List<Message> GetMultiSelectedMessages()
    {
      return this.msgListMLLS != null && this.msgListMLLS.IsSelectionEnabled ? this.msgListMLLS.SelectedItems.Cast<MessageViewModel>().Where<MessageViewModel>((Func<MessageViewModel, bool>) (vm => vm != null && vm.Message != null)).Select<MessageViewModel, Message>((Func<MessageViewModel, Message>) (vm => vm.Message)).MakeUnique<Message, int>((Func<Message, int>) (m => m.MessageID)).ToList<Message>() : new List<Message>();
    }

    public void Dispose()
    {
      this.msgViewModels.SafeDispose();
      MessageViewPanel.ClearCache();
    }

    public void RefreshMessages(Action<MessageViewModel> refresh)
    {
      this.msgViewModels.ApplyToEach(refresh);
      if (!this.EnableOverlay)
        return;
      refresh((MessageViewModel) this.overlayDateViewModel);
    }

    public void SetMessages(
      Message[] msgs,
      bool asc,
      int? targetLandingIndex,
      MessageSearchResult searchRes = null,
      bool forStarredMessagesView = false,
      string jidForContactCardView = null)
    {
      this.msgViewModels.Set(msgs, asc, this.IsRenderingInverted, targetLandingIndex, searchRes, forStarredMessagesView, jidForContactCardView);
      if (this.msgListMLLS != null)
      {
        this.msgListElem = (FrameworkElement) this.msgListMLLS;
        this.msgListMLLS.ItemsSource = (IList) this.msgViewModels.Get();
      }
      else if (this.msgListLLS != null)
      {
        this.msgListElem = (FrameworkElement) this.msgListLLS;
        this.msgListLLS.ItemsSource = (IList) this.msgViewModels.Get();
      }
      this.msgListElem.Opacity = 0.0;
      this.msgListElem.GetLayoutUpdatedAsync().Take<Unit>(1).Subscribe<Unit>((Action<Unit>) (_ =>
      {
        WaAnimations.AnimateTo(this.msgListElem, new double?(), new double?(), new double?(200.0), new double?(0.0), new double?(), new double?(), new double?(), new double?(), (Action) null);
        WaAnimations.PerformFade((DependencyObject) this.msgListElem, WaAnimations.FadeType.FadeIn, TimeSpan.FromMilliseconds(250.0), (Action) (() => this.msgListElem.Opacity = 1.0));
      }));
    }

    public void AddToRecent(Message msg)
    {
      this.AddToRecent(new Message[1]{ msg });
    }

    public void AddToRecent(Message[] msgs) => this.msgViewModels.AddToRecent(msgs);

    public void AddToOldest(Message[] msgs) => this.msgViewModels.AddToOldest(msgs);

    public void UpdateMessages(Message[] msgs)
    {
      foreach (Message msg in msgs)
        this.msgViewModels.OnMessageUpdated(msg);
    }

    public void Remove(Message msg) => this.msgViewModels.Remove(msg);

    public void RemoveUnreadDivider() => this.msgViewModels.RemoveUnreadDivider();

    public bool IncreaseUnreadDivider(int n = 1) => this.msgViewModels.IncreaseUnreadDivider(n);

    public void RemoveAll() => this.msgViewModels.Clear();

    public bool IsEmpty() => this.msgViewModels.Count == 0;

    public void InitializeScrolling()
    {
    }

    public bool IsScrolledToRecent()
    {
      return this.Scroller != null && this.Scroller.Bounds.Y + 20.0 > this.Scroller.Viewport.Y;
    }

    private void ScrollTo(object item)
    {
      if (this.msgListMLLS != null)
      {
        this.msgListMLLS.ScrollTo(item);
      }
      else
      {
        if (this.msgListLLS == null)
          return;
        this.msgListLLS.ScrollTo(item);
      }
    }

    private void ScrollToPretty(double offset)
    {
      if (this.msgListMLLS != null)
      {
        this.msgListMLLS.ScrollToPretty(offset);
      }
      else
      {
        if (this.msgListLLS == null)
          return;
        this.msgListLLS.ScrollToPretty(offset);
      }
    }

    public void ScrollToRecent(string context)
    {
      if (this.Scroller != null)
      {
        this.scrollState = MessageListControl.LayoutUpdatedScrollState.ScrollToRecent;
        this.preLayoutUpdatedBounds = this.Scroller.Bounds.Y;
        this.msgListElem.LayoutUpdated += new EventHandler(this.OnLayoutUpdatedFromNewMessage);
      }
      else
        this.ScrollTo((object) this.msgViewModels.NewestOrDefault());
      this.SetScrollButtonVisibility(false);
    }

    public void ScrollToOldest()
    {
    }

    public void ScrollToInitialPosition()
    {
      if (this.msgViewModels.TargetLandingItem == null)
      {
        this.ScrollToRecent("scroll to unread divider -> scroll to recent");
      }
      else
      {
        this.ScrollTo((object) this.msgViewModels.TargetLandingItem);
        if (this.msgViewModels.TargetLandingItem == this.msgViewModels.OldestOrDefault())
          return;
        this.nextTargetLandingItem = this.msgViewModels.TargetLandingItem;
        this.msgListElem.LayoutUpdated += new EventHandler(this.OnLayoutUpdatedFromUnreadDividerScrolling);
      }
    }

    public bool ScrollToMessage(Message m) => m != null && this.ScrollToMessage(m.MessageID);

    public bool ScrollToMessage(int msgId)
    {
      bool message = false;
      MessageViewModel messageViewModel = this.msgViewModels.FirstOrDefaultForMessage(msgId);
      if (messageViewModel != null)
      {
        this.ScrollTo((object) messageViewModel);
        if (messageViewModel != this.msgViewModels.OldestOrDefault())
        {
          this.nextTargetLandingItem = messageViewModel;
          this.msgListElem.LayoutUpdated += new EventHandler(this.OnLayoutUpdatedFromUnreadDividerScrolling);
        }
        message = true;
      }
      return message;
    }

    public void ScrollFromNewMessage()
    {
      if (this.Scroller == null)
        return;
      this.scrollState = this.Scroller.Viewport.Y >= this.Scroller.Bounds.Y + 200.0 ? MessageListControl.LayoutUpdatedScrollState.ScrollFromNewMessage : MessageListControl.LayoutUpdatedScrollState.ScrollFromNewMessageNearTop;
      this.preLayoutUpdatedBounds = this.Scroller.Bounds.Y;
      this.msgListElem.LayoutUpdated += new EventHandler(this.OnLayoutUpdatedFromNewMessage);
      this.Dispatcher.BeginInvoke((Action) (() =>
      {
        if (this.Scroller.ManipulationState != System.Windows.Controls.Primitives.ManipulationState.Idle)
          return;
        this.SelectMediaElementMessageToAutoplay(true);
      }));
    }

    public void SetTopPadding(double padding)
    {
      Rectangle rectangle = (Rectangle) null;
      if (this.msgListMLLS != null)
        rectangle = this.msgListMLLS.ListFooter as Rectangle;
      else if (this.msgListLLS != null)
        rectangle = this.msgListLLS.ListFooter as Rectangle;
      if (rectangle != null)
        rectangle.Height = padding;
      if (this.Scrollbar == null)
        return;
      if (!(this.Scrollbar.RenderTransform is CompositeTransform compositeTransform))
      {
        compositeTransform = new CompositeTransform();
        this.Scrollbar.RenderTransform = (Transform) compositeTransform;
      }
      double d = (this.msgListElem.ActualHeight - padding) / this.msgListElem.ActualHeight;
      if (double.IsNaN(d) || double.IsInfinity(d) || d == 0.0)
        return;
      compositeTransform.ScaleY = d;
    }

    private void ScheduleNotifyNewerRealizedMessage()
    {
      if (this.newerMsgRealizedSubj == null)
        Log.d("msglist", "notify newer realized msg | skip | not subscribed");
      else if (this.newerRealizedMsgNoticationPending)
      {
        Log.d("msglist", "notify newer realized msg | skip | already scheduled");
      }
      else
      {
        this.newerRealizedMsgNoticationPending = true;
        AppState.Worker.RunAfterDelay(TimeSpan.FromMilliseconds(900.0), (Action) (() => this.Dispatcher.BeginInvoke((Action) (() =>
        {
          this.newerRealizedMsgNoticationPending = false;
          if (this.newerMsgRealizedSubj == null)
            return;
          this.newerMsgRealizedSubj.OnNext(this.realizedNewestMsg);
        }))));
      }
    }

    public void OnViewReload() => this.SelectMediaElementMessageToAutoplay(true);

    private ScrollBar Scrollbar
    {
      get
      {
        if (this.scrollbar == null)
          this.scrollbar = TemplatedVisualTreeExtensions.GetFirstLogicalChildByType<ScrollBar>(this.msgListElem, false);
        return this.scrollbar;
      }
    }

    private ViewportControl Scroller
    {
      get
      {
        if (this.scroller == null)
          this.scroller = TemplatedVisualTreeExtensions.GetFirstLogicalChildByType<ViewportControl>(this.msgListElem, false);
        return this.scroller;
      }
    }

    private void Scroller_ManipulationStateChanged(
      object sender,
      ManipulationStateChangedEventArgs e)
    {
      if (this.Scroller.ManipulationState != System.Windows.Controls.Primitives.ManipulationState.Idle)
        this.scrollState = MessageListControl.LayoutUpdatedScrollState.None;
      if (this.EnableOverlay)
        this.SetDateDividerOverlayFromManipulation(this.Scroller.ManipulationState != 0);
      if (this.HasMediaElementMessages && this.Scroller.ManipulationState == System.Windows.Controls.Primitives.ManipulationState.Idle)
        this.SelectMediaElementMessageToAutoplay(true);
      this.UpdateScrollButtonVisibility();
    }

    private void Scroller_ViewportChanged(object sender, ViewportChangedEventArgs e)
    {
      if (this.EnableOverlay)
      {
        double top = this.Scroller.Viewport.Top;
        double bottom = this.Scroller.Viewport.Bottom;
        bool flag1 = true;
        if (this.currentFirstVisibleMessage != null)
        {
          double num = (double) this.currentFirstVisibleMessage.GetValue(Canvas.TopProperty);
          if (num + this.currentFirstVisibleMessage.ActualHeight > bottom && num < bottom && bottom - num > 40.0)
            flag1 = false;
        }
        if (flag1)
        {
          MessageViewModel messageViewModel = (MessageViewModel) null;
          this.currentFirstVisibleMessage = (FrameworkElement) null;
          double num1 = 0.0;
          foreach (MessageViewModel key in this.realizedItems.Keys.Where<MessageViewModel>((Func<MessageViewModel, bool>) (m => !(m is DateDividerViewModel))))
          {
            FrameworkElement target = (FrameworkElement) null;
            this.realizedItems[key].TryGetTarget(out target);
            if (target != null)
            {
              FrameworkElement parent = VisualTreeHelper.GetParent((DependencyObject) target) as FrameworkElement;
              double num2 = (double) parent.GetValue(Canvas.TopProperty);
              double num3 = num2 + parent.ActualHeight;
              if (num3 > bottom && num2 < bottom && bottom - num2 > 40.0)
              {
                this.currentFirstVisibleMessage = parent;
                messageViewModel = key;
                break;
              }
              if (num3 < bottom)
              {
                bool flag2 = true;
                if (this.currentFirstVisibleMessage != null && num2 < num1)
                  flag2 = false;
                if (flag2)
                {
                  this.currentFirstVisibleMessage = parent;
                  num1 = num2;
                  messageViewModel = key;
                }
              }
            }
          }
          if (messageViewModel != null)
          {
            DateTime? displayTimestamp = messageViewModel.DisplayTimestamp;
            if (displayTimestamp.HasValue)
            {
              displayTimestamp = messageViewModel.DisplayTimestamp;
              this.SetDateDividerOverlay(displayTimestamp.Value.Date);
            }
          }
        }
      }
      if (!this.HasMediaElementMessages)
        return;
      this.SelectMediaElementMessageToAutoplay(false);
    }

    private void InitializeScrollButtonContainer()
    {
      if (!this.EnableScrollButton)
      {
        this.SetScrollButtonVisibility(false);
      }
      else
      {
        double zoomMultiplier = ResolutionHelper.ZoomMultiplier;
        this.ScrollButton.UseOptimizedManipulationRouting = false;
        this.ScrollButton.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.ScrollButtonContainer_Tap);
        this.ScrollButton.Height = zoomMultiplier * 52.0;
        this.ScrollButton.Width = zoomMultiplier * 68.0;
        this.ScrollButtonIcon.Height = zoomMultiplier * 22.0;
        this.ScrollButtonIcon.Width = zoomMultiplier * 22.0;
        this.ScrollButtonIcon.Source = MessageViewModel.IsOverWallpaper ? (System.Windows.Media.ImageSource) AssetStore.DoubleChevronIconBlack : (System.Windows.Media.ImageSource) AssetStore.DoubleChevronIcon;
        this.ScrollButtonBackground.Fill = (Brush) UnreadDividerViewModel.GetBackgroundBrush();
        this.UpdateScrollButtonVisibility();
      }
    }

    private void ScrollButtonContainer_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      this.NotifyScrollToBottomRequested();
    }

    public event EventHandler ScrollToBottomRequested;

    protected void NotifyScrollToBottomRequested()
    {
      this.SetScrollButtonVisibility(false);
      EventHandler toBottomRequested = this.ScrollToBottomRequested;
      if (toBottomRequested == null)
        return;
      toBottomRequested((object) this, EventArgs.Empty);
    }

    private void UpdateScrollButtonVisibility()
    {
      if (!this.EnableScrollButton)
        return;
      this.SetScrollButtonVisibility(!this.IsScrolledToRecent());
    }

    private void SetScrollButtonVisibility(bool isVisible)
    {
      bool flag = this.ScrollButton.Opacity > 0.9;
      if (isVisible && this.EnableScrollButton)
      {
        if (flag)
          return;
        Storyboarder.Perform(WaAnimations.CreateStoryboard(WaAnimations.Fade(WaAnimations.FadeType.FadeIn, TimeSpan.FromMilliseconds(250.0), (DependencyObject) this.ScrollButton)), onComplete: (Action) (() => this.ScrollButton.Opacity = 1.0));
      }
      else
        this.ScrollButton.Opacity = 0.0;
    }

    public void SaveTappedReplyMsgBookmarks(Message quotedMsg, Message replyMsg)
    {
      this.LastTappedQuotedMsg = quotedMsg;
      this.LastTappedReplyMsg = replyMsg;
    }

    public void ClearTappedReplyMsgBookmarks()
    {
      this.LastTappedQuotedMsg = (Message) null;
      this.LastTappedReplyMsg = (Message) null;
    }

    private void MessageList_ItemRealized(object sender, ItemRealizationEventArgs e)
    {
      if (!(e.Container.Content is MessageViewModel content))
        return;
      FrameworkElement target = (FrameworkElement) null;
      if (this.msgListMLLS != null)
      {
        if (VisualTreeHelper.GetChild((DependencyObject) e.Container, 0) is SelectableMessageListItem child)
        {
          child.OnItemRealized(content);
          target = (FrameworkElement) child;
        }
      }
      else if (this.msgListLLS != null)
        target = VisualTreeHelper.GetChild((DependencyObject) e.Container, 0) as FrameworkElement;
      if (target != null)
        this.realizedItems[content] = new WeakReference<FrameworkElement>(target);
      if (content == this.msgViewModels.OldestOrDefault())
        this.NotifyOlderMessagesRequested();
      else if (content == this.msgViewModels.NewestOrDefault())
        this.NotifyNewerMessagesRequested();
      if (this.realizedNewestMsg == null || this.realizedNewestMsg.MessageID < content.Message.MessageID)
      {
        this.realizedNewestMsg = content.Message;
        this.ScheduleNotifyNewerRealizedMessage();
      }
      content.InitLazyResources();
      if (!this.HasMediaElementMessages && this.msgViewModels.HasMediaElementMessages && this.Scroller != null)
        this.HasMediaElementMessages = true;
      this.realizedMessageIDs.Add(content.MessageID);
    }

    private void MessageList_ItemUnrealized(object sender, ItemRealizationEventArgs e)
    {
      try
      {
        if (!(e.Container.Content is MessageViewModel content))
          return;
        content.DisposeLazyResources();
        this.realizedItems.Remove(content);
        this.realizedMessageIDs.Remove(content.MessageID);
      }
      catch (AccessViolationException ex)
      {
      }
    }

    public bool IsMessageRealized(Message msg)
    {
      return msg != null && this.realizedMessageIDs.Contains(msg.MessageID);
    }

    private void SetDateDividerOverlay(DateTime dateToDisplay)
    {
      this.overlayDateViewModel.DisplayTimestamp = new DateTime?(dateToDisplay);
      this.DateDividerOverlayVisible = true;
    }

    private bool DateDividerOverlayVisible
    {
      get => this.dateDividerOverlayVisible;
      set
      {
        if (value)
        {
          DateTime? displayTimestamp = this.overlayDateViewModel.DisplayTimestamp;
          ref DateTime? local = ref displayTimestamp;
          DateTime? nullable = local.HasValue ? new DateTime?(local.GetValueOrDefault().Date) : new DateTime?();
          DateTime today = DateTime.Today;
          if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() == today ? 1 : 0) : 1) : 0) != 0)
            value = false;
        }
        if (this.dateDividerOverlayVisible == value)
          return;
        this.dateDividerOverlayVisible = value;
        this.OverlayDateStoryboard.Pause();
        this.OverlayDateAnimation.To = new double?(this.dateDividerOverlayVisible ? 60.0 : 0.0);
        this.OverlayDateStoryboard.Begin();
        if (!this.dateDividerOverlayVisible)
          return;
        this.DateDividerOverlayContainer.Opacity = 1.0;
      }
    }

    private void SetDateDividerOverlayFromManipulation(bool showOverlay)
    {
      if (showOverlay)
      {
        this.DateDividerOverlayVisible = true;
      }
      else
      {
        if (this.hideOverlayTimer == null)
        {
          this.hideOverlayTimer = new DispatcherTimer()
          {
            Interval = TimeSpan.FromMilliseconds(500.0)
          };
          this.hideOverlayTimer.Tick += new EventHandler(this.HideOverlayTimer_Tick);
        }
        this.hideOverlayTimer.Start();
      }
    }

    private void HideOverlayTimer_Tick(object sender, EventArgs e)
    {
      this.DateDividerOverlayVisible = false;
      this.hideOverlayTimer.Stop();
    }

    private void SelectMediaElementMessageToAutoplay(bool checkImmediately)
    {
      if (this.Scroller == null)
        return;
      Rect viewport = this.Scroller.Viewport;
      if (this.Scroller.Viewport.Height <= 0.0 || !checkImmediately && Math.Abs(this.Scroller.Viewport.Y - this.lastHandledMediaElementPos) < 50.0)
        return;
      if (!checkImmediately)
        this.lastHandledMediaElementPos = this.Scroller.Viewport.Y;
      double top = this.Scroller.Viewport.Top;
      double bottom = this.Scroller.Viewport.Bottom;
      double num1 = 0.0;
      double num2 = 0.0;
      MessageViewModel messageViewModel1 = (MessageViewModel) null;
      foreach (MessageViewModel mediaElementMessage in this.msgViewModels.MediaElementMessages)
      {
        if (this.realizedItems.ContainsKey(mediaElementMessage))
        {
          FrameworkElement target = (FrameworkElement) null;
          this.realizedItems[mediaElementMessage].TryGetTarget(out target);
          if (target != null)
          {
            FrameworkElement parent = VisualTreeHelper.GetParent((DependencyObject) target) as FrameworkElement;
            double num3 = (double) parent.GetValue(Canvas.TopProperty);
            double num4 = num3 + parent.ActualHeight;
            if (num4 > top)
            {
              if (messageViewModel1 == null)
              {
                if (num3 < bottom)
                {
                  num1 = num4;
                  num2 = num3;
                  messageViewModel1 = mediaElementMessage;
                }
              }
              else if (num4 < bottom && num3 > top)
              {
                if (num4 < num1)
                {
                  num1 = num4;
                  num2 = num3;
                  messageViewModel1 = mediaElementMessage;
                }
                else if (num2 < top)
                {
                  num1 = num4;
                  num2 = num3;
                  messageViewModel1 = mediaElementMessage;
                }
              }
              else if ((num1 > bottom || num2 < top) && num2 < top)
              {
                num1 = num4;
                num2 = num3;
                messageViewModel1 = mediaElementMessage;
              }
            }
          }
        }
      }
      if (messageViewModel1 == null || !(messageViewModel1 is InlineVideoMessageViewModel messageViewModel2))
        return;
      messageViewModel2.AutoPlay();
    }

    private void MessageList_MultiSelectionsChanged(object sender, SelectionChangedEventArgs e)
    {
      this.NotifyMultiSelectionsChanged(e);
    }

    private void MessageList_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      this.NotifySelectionChanged();
    }

    private void OnLayoutUpdatedFromNewMessage(object sender, EventArgs e)
    {
      if (this.Scroller.Bounds.Y < this.preLayoutUpdatedBounds)
      {
        switch (this.scrollState)
        {
          case MessageListControl.LayoutUpdatedScrollState.ScrollFromNewMessage:
          case MessageListControl.LayoutUpdatedScrollState.ScrollFromNewMessageNearTop:
            double val1 = this.Scroller.Bounds.Y - this.preLayoutUpdatedBounds;
            if (this.scrollState == MessageListControl.LayoutUpdatedScrollState.ScrollFromNewMessage)
              val1 = Math.Max(val1, -88.0);
            double num = 0.0;
            if (this.msgListMLLS != null)
              num = this.msgListMLLS.ViewportFinalY;
            else if (this.msgListLLS != null)
              num = this.msgListLLS.ViewportFinalY;
            this.ScrollToPretty(Math.Max(this.Scroller.Bounds.Y, num + val1));
            break;
          case MessageListControl.LayoutUpdatedScrollState.ScrollToRecent:
            this.ScrollToPretty(this.Scroller.Bounds.Y);
            break;
        }
        this.scrollState = MessageListControl.LayoutUpdatedScrollState.None;
      }
      if (this.scrollState != MessageListControl.LayoutUpdatedScrollState.None)
        return;
      this.msgListElem.LayoutUpdated -= new EventHandler(this.OnLayoutUpdatedFromNewMessage);
    }

    private void OnLayoutUpdatedFromUnreadDividerScrolling(object sender, EventArgs e)
    {
      this.msgListElem.LayoutUpdated -= new EventHandler(this.OnLayoutUpdatedFromUnreadDividerScrolling);
      FrameworkElement target = (FrameworkElement) null;
      WeakReference<FrameworkElement> weakReference = (WeakReference<FrameworkElement>) null;
      MessageViewModel targetLandingItem = this.nextTargetLandingItem;
      this.nextTargetLandingItem = (MessageViewModel) null;
      if (targetLandingItem != null && this.realizedItems.TryGetValue(targetLandingItem, out weakReference) && weakReference != null && weakReference.TryGetTarget(out target) && target != null)
      {
        double num = (this.msgListElem.ActualHeight - target.ActualHeight) * 0.8;
        ViewportControl scroller = this.Scroller;
        Rect rect = this.Scroller.Viewport;
        double x = rect.X;
        rect = this.Scroller.Bounds;
        double y1 = rect.Y;
        rect = this.Scroller.Viewport;
        double val2 = rect.Y - num;
        double y2 = Math.Max(y1, val2);
        System.Windows.Point location = new System.Windows.Point(x, y2);
        scroller.SetViewportOrigin(location);
      }
      else
        Log.l("msglist", "target landing item not found while re-positioning");
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/MessageListControl.xaml", UriKind.Relative));
      this.OverlayDateStoryboard = (Storyboard) this.FindName("OverlayDateStoryboard");
      this.OverlayDateAnimation = (DoubleAnimation) this.FindName("OverlayDateAnimation");
      this.ContentPanel = (Grid) this.FindName("ContentPanel");
      this.DateDividerOverlayContainer = (MessageBubbleContainer) this.FindName("DateDividerOverlayContainer");
      this.DateDividerOverlayXForm = (CompositeTransform) this.FindName("DateDividerOverlayXForm");
      this.ScrollButton = (Grid) this.FindName("ScrollButton");
      this.ScrollButtonBackground = (Rectangle) this.FindName("ScrollButtonBackground");
      this.ScrollButtonIcon = (Image) this.FindName("ScrollButtonIcon");
    }

    private enum LayoutUpdatedScrollState
    {
      None,
      ScrollFromNewMessage,
      ScrollFromNewMessageNearTop,
      ScrollToRecent,
    }
  }
}
