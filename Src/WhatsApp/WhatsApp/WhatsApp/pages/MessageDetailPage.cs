// Decompiled with JetBrains decompiler
// Type: WhatsApp.MessageDetailPage
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
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using WhatsApp.WaCollections;


namespace WhatsApp
{
  public class MessageDetailPage : PhoneApplicationPage
  {
    private static Message nextInstanceMsg_;
    private static WallpaperStore.WallpaperState nextInstanceWallpaper_;
    private MessageDetailPageViewModel viewModel_;
    private Message msg_;
    private Conversation convo_;
    private WallpaperStore.WallpaperState wallpaper_;
    private bool isPageEverLoaded_;
    private bool isDataLoaded_;
    private bool wallpaperSet_;
    private object deliveryStateLock_ = new object();
    private MessageDeliveryState deliveryState_;
    private bool? isMultiParticipants_;
    private double textFontSize_;
    private IDisposable receiptsSub_;
    private IDisposable deleteSub_;
    private IDisposable statusChangedSub_;
    private IDisposable audioRoutingSub;
    private double initialOffsetPortrait_;
    private double initialOffsetLandscape_;
    private MessageDeliveryState.RecipientItem prevSelItem;
    private bool ignoreSelChangeOnce_;
    private Dictionary<MessageDeliveryState.ItemBase, WeakReference<FrameworkElement>> realizedItems = new Dictionary<MessageDeliveryState.ItemBase, WeakReference<FrameworkElement>>();
    private IDisposable listSbSub_;
    internal Grid LayoutRoot;
    internal WallpaperPanel WallpaperPanel;
    internal PageTitlePanel TitlePanel;
    internal Grid ListPanel;
    internal ParallaxLongListSelector MainList;
    internal ItemsControl MessageList;
    internal Canvas FooterBackground;
    internal StackPanel FooterTextPanel;
    internal TextBlock FooterText;
    private bool _contentLoaded;

    public MessageDeliveryState DeliveryState => this.deliveryState_;

    public bool IsMultiParticipants
    {
      get
      {
        return this.isMultiParticipants_ ?? (this.isMultiParticipants_ = new bool?(this.msg_ != null && JidHelper.IsMultiParticipantsChatJid(this.msg_.KeyRemoteJid))).Value;
      }
    }

    public double TextFontSize
    {
      get => this.textFontSize_;
      set
      {
        if (this.textFontSize_ == value)
          return;
        this.textFontSize_ = value;
        if (!this.isPageEverLoaded_)
          return;
        this.MessageList.ItemsSource = (IEnumerable) this.GetMessageViewModels();
      }
    }

    public MessageDetailPage()
    {
      this.InitializeComponent();
      this.MainList.OverlapScrollBar = true;
      this.viewModel_ = new MessageDetailPageViewModel(this.Orientation);
      this.DataContext = (object) this.viewModel_;
      this.msg_ = MessageDetailPage.nextInstanceMsg_;
      this.wallpaper_ = MessageDetailPage.nextInstanceWallpaper_;
      MessageDetailPage.nextInstanceMsg_ = (Message) null;
      MessageDetailPage.nextInstanceWallpaper_ = (WallpaperStore.WallpaperState) null;
      if (this.wallpaper_ == null && this.msg_ != null)
        MessagesContext.Run((MessagesContext.MessagesCallback) (db => this.wallpaper_ = WallpaperStore.Get(db, this.msg_.KeyRemoteJid, true)));
      this.Loaded += new RoutedEventHandler(this.OnLoaded);
    }

    public static void Start(Message msg, WallpaperStore.WallpaperState wallpaper = null)
    {
      if (msg == null || !msg.KeyFromMe)
        return;
      MessageDetailPage.nextInstanceMsg_ = msg;
      MessageDetailPage.nextInstanceWallpaper_ = wallpaper;
      WaUriParams uriParams = new WaUriParams();
      uriParams.AddString("jid", msg.KeyRemoteJid);
      uriParams.AddString("KeyId", msg.KeyId);
      NavUtils.NavigateToPage(nameof (MessageDetailPage), uriParams);
    }

    private void InitAudioPlayback()
    {
      if (this.msg_.MediaWaType != FunXMPP.FMessage.Type.Audio || this.audioRoutingSub != null)
        return;
      this.audioRoutingSub = (IDisposable) new PttPlaybackWrapper();
    }

    private void DisposeAudioPlayback()
    {
      this.audioRoutingSub.SafeDispose();
      this.audioRoutingSub = (IDisposable) null;
    }

    private void LoadData()
    {
      if (this.msg_ == null)
        return;
      if (this.convo_ == null)
        MessagesContext.Run((MessagesContext.MessagesCallback) (db => this.convo_ = db.GetConversation(this.msg_.KeyRemoteJid, CreateOptions.CreateToDbIfNotFound)));
      this.deliveryState_ = MessageDeliveryState.Create(this.msg_);
      WAThreadPool.QueueUserWorkItem((Action) (() =>
      {
        DateTime? start = PerformanceTimer.Start();
        List<UserStatus> participants = (List<UserStatus>) null;
        ContactsContext.Instance((Action<ContactsContext>) (db => participants = db.GetUserStatuses((IEnumerable<string>) this.convo_.GetParticipantJids(), false, false)));
        this.deliveryState_.SetParticipants((IEnumerable<UserStatus>) participants);
        List<ReceiptState> allReceipts = (List<ReceiptState>) null;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          allReceipts = db.GetReceiptsForMessage(this.msg_.MessageID);
          this.receiptsSub_ = db.NewReceiptSubject.Where<ReceiptState>((Func<ReceiptState, bool>) (r => r.MessageId == this.msg_.MessageID)).ObserveOnDispatcher<ReceiptState>().Subscribe<ReceiptState>(new Action<ReceiptState>(this.OnNewReceipt));
          this.deleteSub_ = db.DeletedMessagesSubject.Where<Message>((Func<Message, bool>) (m => m.MessageID == this.msg_.MessageID)).ObserveOnDispatcher<Message>().Subscribe<Message>(new Action<Message>(this.OnMessageDeleted));
        }));
        lock (this.deliveryStateLock_)
          this.deliveryState_.LoadInitialReceipts((IEnumerable<ReceiptState>) allReceipts);
        if (this.IsMultiParticipants)
          this.statusChangedSub_ = this.msg_.GetPropertyChangedAsync().Where<PropertyChangedEventArgs>((Func<PropertyChangedEventArgs, bool>) (p => p.PropertyName == "Status")).ObserveOnDispatcher<PropertyChangedEventArgs>().Subscribe<PropertyChangedEventArgs>((Action<PropertyChangedEventArgs>) (_ => this.OnMessageStatusChanged()));
        PerformanceTimer.End("msg detail page: data load", start);
        this.Dispatcher.BeginInvoke((Action) (() =>
        {
          this.isDataLoaded_ = true;
          if (!this.isPageEverLoaded_)
            return;
          this.InitWithLoadedData();
        }));
      }));
    }

    private void InitWithLoadedData()
    {
      if (this.deliveryState_ == null || this.MainList.ItemsSource != null)
        return;
      this.deliveryState_.SetItemMargin(this.GetRecipientItemMargin(this.Orientation), false);
      if (this.IsMultiParticipants)
      {
        this.MainList.JumpListStyle = (Style) null;
        this.MainList.IsFlatList = false;
        this.MainList.GroupHeaderTemplate = this.Resources[(object) "HeaderTemplate"] as DataTemplate;
        this.MainList.ItemTemplate = this.Resources[(object) "MultiParticipantChatItemTemplate"] as DataTemplate;
        this.MainList.ItemRealized += new EventHandler<ItemRealizationEventArgs>(this.MainList_ItemRealized);
        this.MainList.ItemUnrealized += new EventHandler<ItemRealizationEventArgs>(this.MainList_ItemUnrealized);
        this.MainList.SelectionChanged += new SelectionChangedEventHandler(this.MainList_SelectionChanged);
      }
      else
      {
        this.MainList.IsFlatList = true;
        this.MainList.ItemTemplate = this.Resources[(object) "IndividualChatItemTemplate"] as DataTemplate;
      }
      this.MainList.ItemsSource = this.deliveryState_.GetListSource();
      this.MainList.GetLayoutUpdatedAsync().Take<Unit>(1).Subscribe<Unit>((Action<Unit>) (_ =>
      {
        this.ResizeFooterBackground();
        this.InitialScroll();
      }));
    }

    private List<MessageViewModel> GetMessageViewModels()
    {
      Set<MessageMenu.MessageMenuItem> excludedMenuItems = new Set<MessageMenu.MessageMenuItem>((IEnumerable<MessageMenu.MessageMenuItem>) new MessageMenu.MessageMenuItem[1]
      {
        MessageMenu.MessageMenuItem.ShowDetails
      });
      return MessageViewModel.CreateForMessage(this.msg_, false, false, false, postModifier: (Action<MessageViewModel>) (vm => vm.ExcludedMenuItems = excludedMenuItems)).ToList<MessageViewModel>();
    }

    private void InitOnLoaded()
    {
      if (this.msg_ == null)
        return;
      this.SetWallpaper();
      this.TitlePanel.SmallTitle = AppResources.MessageInfoTitle;
      this.MessageList.ItemsSource = (IEnumerable) this.GetMessageViewModels();
      this.MessageList.Margin = this.viewModel_.HeaderMargin;
      if (!this.isDataLoaded_)
        return;
      this.InitWithLoadedData();
    }

    private void ResizeFooterBackground()
    {
      this.FooterBackground.Height = Math.Max(144.0, this.ListPanel.ActualHeight - (this.MainList.Viewport.Bounds.Height - this.FooterBackground.Height));
    }

    private double GetInitialOffset(PageOrientation orientation)
    {
      return !orientation.IsLandscape() ? this.initialOffsetPortrait_ : this.initialOffsetLandscape_;
    }

    private void InitialScroll()
    {
      double actualHeight = this.MessageList.ActualHeight;
      double num1 = (480.0 - this.TitlePanel.ActualHeight) / 2.0;
      if (actualHeight > num1)
        this.initialOffsetLandscape_ = actualHeight - num1;
      int num2 = 384;
      if (actualHeight > (double) num2)
        this.initialOffsetPortrait_ = actualHeight - (double) num2;
      double initialOffset = this.GetInitialOffset(this.Orientation);
      this.MainList.ParallaxStartOffset = initialOffset;
      this.MainList.Viewport.SetViewportOrigin(new System.Windows.Point(0.0, initialOffset));
    }

    private void SetWallpaper()
    {
      if (this.msg_ == null || this.wallpaperSet_)
        return;
      this.wallpaperSet_ = true;
      this.WallpaperPanel.Set(this.wallpaper_);
      if (this.wallpaper_ == null || !this.wallpaper_.HasWallpaper)
        return;
      this.TitlePanel.Foreground = (Brush) UIUtils.WhiteBrush;
      if (ImageStore.IsDarkTheme())
        return;
      SysTrayHelper.SetForegroundColor((DependencyObject) this, Constants.SysTrayOffWhite);
    }

    private Thickness GetRecipientItemMargin(PageOrientation orientation)
    {
      Thickness recipientItemMargin = this.IsMultiParticipants ? new Thickness(24.0, 8.0, 24.0, 8.0) : new Thickness(12.0, 20.0, 24.0, 0.0);
      switch (orientation)
      {
        case PageOrientation.LandscapeLeft:
          recipientItemMargin.Left += UIUtils.SystemTraySizeLandscape;
          break;
        case PageOrientation.LandscapeRight:
          recipientItemMargin.Right += UIUtils.SystemTraySizeLandscape;
          break;
      }
      return recipientItemMargin;
    }

    private void RefreshMainListItemsMargin(PageOrientation orientation)
    {
      if (this.deliveryState_ == null)
        return;
      this.deliveryState_.SetItemMargin(this.GetRecipientItemMargin(orientation), true);
      if (this.IsMultiParticipants)
        return;
      this.MainList.ItemsSource = this.deliveryState_.GetListSource();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      bool flag = false;
      if (this.msg_ == null)
      {
        IDictionary<string, string> queryString = this.NavigationContext.QueryString;
        string jid = (string) null;
        string keyId = (string) null;
        if (queryString.TryGetValue("jid", out jid) && queryString.TryGetValue("KeyId", out keyId) && !string.IsNullOrEmpty(jid) && !string.IsNullOrEmpty(keyId))
          MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
          {
            this.msg_ = db.GetMessage(jid, keyId, true);
            this.convo_ = db.GetConversation(jid, CreateOptions.None);
          }));
        if (this.msg_ == null)
          flag = true;
      }
      base.OnNavigatedTo(e);
      if (flag)
      {
        this.Dispatcher.BeginInvoke((Action) (() => NavUtils.GoBack(this.NavigationService)));
      }
      else
      {
        this.TextFontSize = Settings.SystemFontSize;
        this.InitAudioPlayback();
        if (this.isPageEverLoaded_)
          return;
        this.MainList.ItemsSource = (IList) null;
        LongMessageSplitter.ResetCharSizeEstimation();
        this.LoadData();
        if (this.wallpaper_ == null)
          return;
        this.SetWallpaper();
      }
    }

    protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
    {
      this.DisposeAudioPlayback();
      base.OnNavigatingFrom(e);
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      this.receiptsSub_.SafeDispose();
      this.deleteSub_.SafeDispose();
      this.statusChangedSub_.SafeDispose();
      this.receiptsSub_ = this.deleteSub_ = this.statusChangedSub_ = (IDisposable) null;
      base.OnRemovedFromJournal(e);
    }

    protected override void OnOrientationChanged(OrientationChangedEventArgs e)
    {
      this.MainList.EnableParallaxScrolling = false;
      this.viewModel_.Orientation = e.Orientation;
      this.MessageList.Margin = this.viewModel_.HeaderMargin;
      this.RefreshMainListItemsMargin(e.Orientation);
      base.OnOrientationChanged(e);
      this.MainList.GetLayoutUpdatedAsync().Take<Unit>(1).Subscribe<Unit>((Action<Unit>) (_ => this.ResizeFooterBackground()));
    }

    private void OnLoaded(object sender, EventArgs e)
    {
      if (this.isPageEverLoaded_)
        return;
      this.isPageEverLoaded_ = true;
      this.InitOnLoaded();
    }

    private void OnMessageDeleted(Message m) => NavUtils.GoBack();

    private void OnNewReceipt(ReceiptState receipt)
    {
      lock (this.deliveryStateLock_)
      {
        if (this.deliveryState_ == null)
          return;
        this.deliveryState_.AddReceipt(receipt);
        if (!JidHelper.IsUserJid(this.msg_.KeyRemoteJid) || !this.isDataLoaded_ || this.MainList.ItemsSource == null)
          return;
        this.MainList.ItemsSource = this.deliveryState_.GetListSource();
      }
    }

    private void OnMessageStatusChanged()
    {
      if (!this.IsMultiParticipants || !this.msg_.Status.IsReadByTarget() || this.MainList.ItemsSource == null || this.deliveryState_ == null)
        return;
      this.MainList.ItemsSource = this.deliveryState_.GetListSource();
    }

    private MessageDeliveryState.ItemBase GetItemFromRealizationArgs(ItemRealizationEventArgs e)
    {
      if (e.ItemKind == LongListSelectorItemKind.Item)
        return e.Container.Content as MessageDeliveryState.ItemBase;
      if (e.ItemKind != LongListSelectorItemKind.GroupHeader)
        return (MessageDeliveryState.ItemBase) null;
      return e.Container.Content is KeyedObservableCollection<MultiParticipantsMessageDeliveryState.GroupItem, MessageDeliveryState.RecipientItem> content ? (MessageDeliveryState.ItemBase) content.Key : (MessageDeliveryState.ItemBase) null;
    }

    private void MainList_ItemRealized(object sender, ItemRealizationEventArgs e)
    {
      FrameworkElement target1 = (FrameworkElement) null;
      if (e.ItemKind == LongListSelectorItemKind.Item)
        target1 = VisualTreeHelper.GetChild((DependencyObject) e.Container, 0) as FrameworkElement;
      else if (e.ItemKind == LongListSelectorItemKind.GroupHeader)
        target1 = VisualTreeHelper.GetChild((DependencyObject) e.Container, 0) as FrameworkElement;
      int count = this.realizedItems.Count;
      MessageDeliveryState.ItemBase fromRealizationArgs = this.GetItemFromRealizationArgs(e);
      if (fromRealizationArgs != null && target1 != null)
        this.realizedItems[fromRealizationArgs] = new WeakReference<FrameworkElement>(target1);
      int num = 0;
      foreach (Collection<MessageDeliveryState.RecipientItem> collection in ((LongListSelector) sender).ItemsSource as List<KeyedObservableCollection<MultiParticipantsMessageDeliveryState.GroupItem, MessageDeliveryState.RecipientItem>>)
      {
        foreach (MessageDeliveryState.RecipientItem key in collection)
        {
          FrameworkElement target2 = (FrameworkElement) null;
          if (this.realizedItems.ContainsKey((MessageDeliveryState.ItemBase) key))
            this.realizedItems[(MessageDeliveryState.ItemBase) key].TryGetTarget(out target2);
          if (target2 != null)
            Canvas.SetZIndex((UIElement) (VisualTreeHelper.GetParent((DependencyObject) target2) as FrameworkElement), num++);
        }
      }
    }

    private void MainList_ItemUnrealized(object sender, ItemRealizationEventArgs e)
    {
      int count = this.realizedItems.Count;
      MessageDeliveryState.ItemBase fromRealizationArgs = this.GetItemFromRealizationArgs(e);
      if (fromRealizationArgs == null)
        return;
      this.realizedItems.Remove(fromRealizationArgs);
    }

    private bool CompareUsersInSameGroup(
      MessageDeliveryState.RecipientItem current,
      MessageDeliveryState.RecipientItem baseline)
    {
      if (current == null)
        return false;
      return current.User == null ? current.Status.GetOverrideWeight() <= baseline.Status.GetOverrideWeight() : MessageDeliveryState.RecipientItem.CompareFunc(current, baseline) > 0;
    }

    private void MainList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (this.ignoreSelChangeOnce_)
      {
        this.ignoreSelChangeOnce_ = false;
      }
      else
      {
        MessageDeliveryState.RecipientItem baseline = this.MainList.SelectedItem as MessageDeliveryState.RecipientItem;
        MessageDeliveryState.RecipientItem recipientItem = this.prevSelItem;
        if (recipientItem == baseline)
        {
          if (baseline != null)
            baseline.IsSelected = !baseline.IsSelected;
        }
        else
        {
          if (recipientItem != null)
            recipientItem.IsSelected = false;
          if (baseline != null)
            baseline.IsSelected = true;
        }
        if (baseline != null && !baseline.IsSelected)
          baseline = (MessageDeliveryState.RecipientItem) null;
        if (recipientItem != null && recipientItem.IsSelected)
          recipientItem = (MessageDeliveryState.RecipientItem) null;
        this.prevSelItem = baseline;
        int num1 = App.ShouldAnimate(true) ? 1 : 0;
        if (recipientItem != null && !this.realizedItems.ContainsKey((MessageDeliveryState.ItemBase) recipientItem))
          recipientItem = (MessageDeliveryState.RecipientItem) null;
        if (num1 != 0 && (recipientItem != null || baseline != null))
        {
          TimeSpan timeSpan = TimeSpan.FromMilliseconds(750.0);
          PowerEase powerEase1 = new PowerEase();
          powerEase1.Power = 5.0;
          powerEase1.EasingMode = EasingMode.EaseOut;
          PowerEase powerEase2 = powerEase1;
          Storyboard sb = new Storyboard();
          foreach (KeyValuePair<MessageDeliveryState.ItemBase, WeakReference<FrameworkElement>> realizedItem in this.realizedItems)
          {
            int num2 = 0;
            MessageDeliveryState.ItemBase key = realizedItem.Key;
            MessageDeliveryState.RecipientItem current = key as MessageDeliveryState.RecipientItem;
            FrameworkElement target = (FrameworkElement) null;
            realizedItem.Value.TryGetTarget(out target);
            if (target != null)
            {
              if (recipientItem != null && recipientItem.User != null && (key.Status.GetOverrideWeight() < recipientItem.Status.GetOverrideWeight() || this.CompareUsersInSameGroup(current, recipientItem)))
                num2 -= recipientItem.ReceiptsListHeight;
              if (baseline != null && baseline.User != null && (key.Status.GetOverrideWeight() < baseline.Status.GetOverrideWeight() || this.CompareUsersInSameGroup(current, baseline)))
                num2 += baseline.ReceiptsListHeight;
              if (num2 != 0)
              {
                DoubleAnimation doubleAnimation = new DoubleAnimation();
                doubleAnimation.EasingFunction = (IEasingFunction) powerEase2;
                doubleAnimation.Duration = (Duration) timeSpan;
                doubleAnimation.From = new double?((double) -num2);
                doubleAnimation.To = new double?(0.0);
                DoubleAnimation element = doubleAnimation;
                if (target.FindName("SlideTransform") is CompositeTransform name)
                {
                  Storyboard.SetTarget((Timeline) element, (DependencyObject) name);
                  Storyboard.SetTargetProperty((Timeline) element, new PropertyPath("TranslateY", new object[0]));
                  sb.Children.Add((Timeline) element);
                }
              }
            }
          }
          this.listSbSub_.SafeDispose();
          this.listSbSub_ = Storyboarder.PerformWithDisposable(sb, (DependencyObject) null, true, (Action) (() => this.listSbSub_ = (IDisposable) null), false, "msg detail page lls");
        }
        if (this.MainList.SelectedItem == null)
          return;
        this.ignoreSelChangeOnce_ = true;
        this.MainList.SelectedItem = (object) null;
      }
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/MessageDetailPage.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.WallpaperPanel = (WallpaperPanel) this.FindName("WallpaperPanel");
      this.TitlePanel = (PageTitlePanel) this.FindName("TitlePanel");
      this.ListPanel = (Grid) this.FindName("ListPanel");
      this.MainList = (ParallaxLongListSelector) this.FindName("MainList");
      this.MessageList = (ItemsControl) this.FindName("MessageList");
      this.FooterBackground = (Canvas) this.FindName("FooterBackground");
      this.FooterTextPanel = (StackPanel) this.FindName("FooterTextPanel");
      this.FooterText = (TextBlock) this.FindName("FooterText");
    }
  }
}
