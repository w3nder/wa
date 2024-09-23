// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaStatusViewPage
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
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WhatsApp.CommonOps;
using WhatsApp.WaCollections;


namespace WhatsApp
{
  public class WaStatusViewPage : PhoneApplicationPage
  {
    private static WaStatusThread nextInstanceStartingThread = (WaStatusThread) null;
    private static WaStatusThread[] nextInstanceThreads = (WaStatusThread[]) null;
    private static bool nextInstanceStartFromUnviewed = false;
    private static bool nextInstanceMuteSound = true;
    public static readonly DependencyProperty RootFrameTranslateYProperty = DependencyProperty.Register("RootFrameTranslateY", typeof (double), typeof (WaStatusViewPage), new PropertyMetadata((object) 0.0, new PropertyChangedCallback(WaStatusViewPage.OnRootFrameTranslateYChanged)));
    private WaStatusThread[] threads;
    private WaStatusThread currentThread;
    private WaStatus currentStatus;
    private ObservableCollection<StatusReadRecipientViewModel> readRecipients;
    private List<ReceiptState> pendingNewReceipts;
    private bool startFromUnviewed;
    private bool startWithSoundMute = true;
    private bool isPageLoaded;
    private bool isPageRemoved;
    private bool isPageNavigatedTo;
    private Storyboard slideDownSb;
    private ChatInputBar inputBar;
    private CompositeTransform statusViewTransform = new CompositeTransform();
    private List<IDisposable> disposables = new List<IDisposable>();
    private IDisposable sbSub;
    private IDisposable readReceiptsSub;
    private IDisposable sysTrayHideSub;
    private bool isStatusInPlay;
    private System.Windows.Media.ImageSource msgIcon;
    private System.Windows.Media.ImageSource viewsIcon;
    private Color bottomBarBgColor = new Color()
    {
      R = 32,
      G = 30,
      B = 33
    };
    internal Grid LayoutRoot;
    internal WaStatusViewControl StatusView;
    internal Rectangle Mask;
    internal ZoomBox ReadRecipientListZoomBox;
    internal CompositeTransform ReadRecipientPanelTransform;
    internal TextBlock ReadCountBlock;
    internal WhatsApp.CompatibilityShims.LongListSelector ReadRecipientList;
    internal TextBlock ReadRecipientListTooltipBlock;
    internal ZoomBox BottomBarsZoomBox;
    internal Grid BottomBar;
    internal Rectangle BottomBarBackground;
    internal Button MainButton;
    internal Image MainButtonIcon;
    internal TextBlock MainButtonCaptionBlock;
    internal TextBlock MoreButton;
    internal StackPanel InfoPanel;
    internal CompositeTransform InfoPanelTransform;
    internal TextBlock InfoBlock;
    private bool _contentLoaded;

    public WaStatusViewPage()
    {
      this.InitializeComponent();
      this.currentThread = WaStatusViewPage.nextInstanceStartingThread;
      WaStatusViewPage.nextInstanceStartingThread = (WaStatusThread) null;
      this.threads = WaStatusViewPage.nextInstanceThreads;
      WaStatusViewPage.nextInstanceThreads = (WaStatusThread[]) null;
      this.startFromUnviewed = WaStatusViewPage.nextInstanceStartFromUnviewed;
      WaStatusViewPage.nextInstanceStartFromUnviewed = false;
      this.startWithSoundMute = WaStatusViewPage.nextInstanceMuteSound;
      WaStatusViewPage.nextInstanceMuteSound = true;
      this.InitPage();
    }

    public static void Start(
      WaStatusThread startingThread,
      WaStatusThread[] allThreads,
      bool startFromUnviewed,
      bool muteSound = true,
      bool replacePage = false)
    {
      if (startingThread == null)
        return;
      WaStatusViewPage.nextInstanceStartingThread = startingThread;
      WaStatusViewPage.nextInstanceThreads = allThreads;
      WaStatusViewPage.nextInstanceStartFromUnviewed = startFromUnviewed;
      WaUriParams uriParams = new WaUriParams();
      uriParams.AddString("jid", startingThread.Jid);
      if (replacePage)
        uriParams.AddBool("PageReplace", replacePage);
      NavUtils.NavigateToPage(nameof (WaStatusViewPage), uriParams);
    }

    private void InitPage()
    {
      BackKeyBroker.Get((PhoneApplicationPage) this, 1).Subscribe<CancelEventArgs>(new Action<CancelEventArgs>(this.OnBackKey));
      this.Loaded += new RoutedEventHandler(this.OnLoaded);
      this.disposables.Add(this.StatusView.ExitObservable().Take<int>(1).ObserveOnDispatcher<int>().Subscribe<int>((Action<int>) (res =>
      {
        bool flag = true;
        if (res != 0)
        {
          int num = -1;
          WaStatusThread[] threads1 = this.threads;
          int length = threads1 != null ? threads1.Length : 0;
          for (int index = 0; index < length; ++index)
          {
            if (this.threads[index].Jid == this.currentThread.Jid)
            {
              num = index;
              break;
            }
          }
          if (num >= 0)
          {
            int index = num + (res > 0 ? 1 : -1);
            WaStatusThread[] threads2 = this.threads;
            WaStatusThread targetThread = threads2 != null ? ((IEnumerable<WaStatusThread>) threads2).ElementAtOrDefault<WaStatusThread>(index) : (WaStatusThread) null;
            if (targetThread != null)
            {
              flag = false;
              bool toMuteSound = this.StatusView.IsSoundMuted;
              Storyboarder.Perform(WaAnimations.PageTransition(res > 0 ? PageTransitionAnimation.TurnstileForwardOut : PageTransitionAnimation.TurnstileBackwardOut), (DependencyObject) this.LayoutRoot, false, (Action) (() => WaStatusViewPage.Start(targetThread, this.threads, this.startFromUnviewed, toMuteSound, true)));
            }
          }
        }
        if (!flag)
          return;
        this.SlideDownAndBackOut();
      })));
      this.disposables.Add(this.StatusView.FlickedUpObservable().ObserveOnDispatcher<Unit>().Subscribe<Unit>((Action<Unit>) (_ => this.MainButton_Click((object) null, (RoutedEventArgs) null))));
      this.disposables.Add(this.StatusView.StatusLoadedObservable().ObserveOnDispatcher<Pair<WaStatus, bool>>().Subscribe<Pair<WaStatus, bool>>((Action<Pair<WaStatus, bool>>) (p => this.OnStatusLoaded(p.First, p.Second))));
      this.disposables.Add(this.StatusView.LinkPreviewShownObservable().ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (shown => this.ShowBottomBar(!shown))));
      this.disposables.Add(this.StatusView.PlaybackStateObservable().ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (inPlay =>
      {
        this.isStatusInPlay = inPlay;
        if (inPlay)
        {
          this.ShowBottomBar(inPlay);
        }
        else
        {
          IDisposable timerSub = (IDisposable) null;
          timerSub = Observable.Timer(TimeSpan.FromMilliseconds(500.0)).Take<long>(1).ObserveOnDispatcher<long>().Subscribe<long>((Action<long>) (_ =>
          {
            timerSub.SafeDispose();
            timerSub = (IDisposable) null;
            if (this.isStatusInPlay)
              this.ShowBottomBar(true, true);
            else
              this.ShowBottomBar(false);
          }));
        }
      })));
      ChatInputBar chatInputBar = new ChatInputBar(false);
      chatInputBar.VerticalAlignment = VerticalAlignment.Bottom;
      chatInputBar.CacheMode = (CacheMode) new BitmapCache();
      chatInputBar.Opacity = 0.0;
      chatInputBar.IsHitTestVisible = false;
      this.inputBar = chatInputBar;
      this.inputBar.SetOrientation(PageOrientation.Portrait);
      this.inputBar.ShowAttachButton(false);
      this.inputBar.EnableMentions(false);
      this.LayoutRoot.Children.Add((UIElement) this.inputBar);
      this.disposables.Add(this.inputBar.SIPStateChangedObservable().Subscribe<SIPStates>((Action<SIPStates>) (sipState =>
      {
        if (sipState == SIPStates.Collapsed)
        {
          this.StatusView.Resume();
          this.inputBar.Opacity = 0.0;
          this.inputBar.IsHitTestVisible = false;
        }
        else
        {
          this.StatusView.Pause(false);
          this.inputBar.Opacity = 1.0;
          this.inputBar.IsHitTestVisible = true;
        }
        this.UpdateForSIPOffset(0.0);
      })));
      this.disposables.Add(this.inputBar.ActionObservable().ObserveOnDispatcher<Pair<ChatInputBar.Actions, object>>().Subscribe<Pair<ChatInputBar.Actions, object>>((Action<Pair<ChatInputBar.Actions, object>>) (p =>
      {
        if (p.First != ChatInputBar.Actions.SendText)
          return;
        this.inputBar.CloseEmojiKeyboard(SIPStates.Undefined);
        this.CloseKeyboard();
        this.inputBar.Clear();
        this.SendReply(p.Second as ExtendedTextInputData).SubscribeOn<bool>(WAThreadPool.Scheduler).ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (sent =>
        {
          this.ShowReplyConfirmation(true);
          FieldStats.ReportFsStatusReplyEvent(sent ? wam_enum_status_reply_result.OK : wam_enum_status_reply_result.ERROR_UNKNOWN);
        }));
      })));
      this.disposables.Add(EmojiKeyboard.ActionSubject.ObserveOnDispatcher<Pair<EmojiKeyboard.Actions, object>>().Subscribe<Pair<EmojiKeyboard.Actions, object>>((Action<Pair<EmojiKeyboard.Actions, object>>) (p =>
      {
        if (p.First != EmojiKeyboard.Actions.ActionedSticker)
          return;
        this.inputBar.CloseEmojiKeyboard(SIPStates.Undefined);
        this.CloseKeyboard();
        this.SendSticker(p.Second as Sticker).SubscribeOn<bool>(WAThreadPool.Scheduler).ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (sent =>
        {
          this.ShowReplyConfirmation(sent);
          this.inputBar.Clear();
        }));
      })));
      this.StatusView.RenderTransform = (Transform) this.statusViewTransform;
      this.ReadRecipientListZoomBox.ZoomFactor = this.BottomBarsZoomBox.ZoomFactor = ResolutionHelper.ZoomFactor;
      this.BottomBarBackground.Opacity = 0.5;
    }

    private void ShowBottomBar(bool show, bool skipAnimation = false)
    {
      if (skipAnimation)
      {
        this.BottomBarsZoomBox.Opacity = show ? 1.0 : 0.0;
      }
      else
      {
        Storyboard storyboard = WaAnimations.CreateStoryboard(WaAnimations.Fade(show ? WaAnimations.FadeType.FadeIn : WaAnimations.FadeType.FadeOut, TimeSpan.FromMilliseconds(show ? 150.0 : 250.0), (DependencyObject) this.BottomBarsZoomBox));
        this.sbSub.SafeDispose();
        this.sbSub = Storyboarder.PerformWithDisposable(storyboard, (DependencyObject) null, false, (Action) (() =>
        {
          this.sbSub = (IDisposable) null;
          this.BottomBarsZoomBox.Opacity = show ? 1.0 : 0.0;
        }), (Action) null, "toggle bottom bar");
      }
    }

    private void CloseKeyboard() => this.Focus();

    private void BindRootFrameTranslateY()
    {
      if (!(App.CurrentApp.OriginalRootFrameTransform is TransformGroup rootFrameTransform))
        return;
      Transform transform = rootFrameTransform.Children.FirstOrDefault<Transform>((Func<Transform, bool>) (t => t is TranslateTransform));
      if (transform == null)
        return;
      this.SetBinding(WaStatusViewPage.RootFrameTranslateYProperty, new Binding("Y")
      {
        Source = (object) transform
      });
    }

    private void UpdateForSIPOffset(double sipOffset)
    {
      if (this.inputBar.CurrentSIPState == SIPStates.EmojiKeyboard)
        this.statusViewTransform.TranslateY = UIUtils.SIPHeightPortrait;
      else
        this.statusViewTransform.TranslateY = sipOffset;
    }

    private void SlideDownAndBackOut()
    {
      if (this.slideDownSb == null)
        this.slideDownSb = WaAnimations.PageTransition(PageTransitionAnimation.SlideDownFadeOut);
      Storyboarder.Perform(this.slideDownSb, (DependencyObject) this.LayoutRoot, false, (Action) (() => NavUtils.GoBack(this.NavigationService)));
    }

    private void ShowReplyConfirmation(bool show)
    {
      this.InfoBlock.FontSize = 20.0 * ResolutionHelper.ZoomMultiplier;
      this.InfoBlock.Text = AppResources.StatusReplySent;
      double fromY;
      this.InfoPanelTransform.TranslateY = fromY = show ? -72.0 * ResolutionHelper.ZoomMultiplier : 0.0;
      double toY = show ? 0.0 : -72.0 * ResolutionHelper.ZoomMultiplier;
      this.InfoPanel.Opacity = show ? 0.0 : 1.0;
      this.InfoPanel.Visibility = Visibility.Visible;
      Storyboard storyboard = WaAnimations.CreateStoryboard((IEnumerable<DoubleAnimation>) new DoubleAnimation[2]
      {
        WaAnimations.VerticalSlide(fromY, toY, TimeSpan.FromMilliseconds(show ? 500.0 : 350.0), (DependencyObject) this.InfoPanel),
        WaAnimations.Fade(show ? WaAnimations.FadeType.FadeIn : WaAnimations.FadeType.FadeOut, TimeSpan.FromMilliseconds(show ? 150.0 : 250.0), (DependencyObject) this.InfoPanel)
      });
      this.sbSub.SafeDispose();
      bool disposed = false;
      this.sbSub = Storyboarder.PerformWithDisposable(storyboard, shouldStop: false, onComplete: (Action) (() =>
      {
        this.sbSub = (IDisposable) null;
        if (show)
        {
          this.InfoPanel.Opacity = 1.0;
          this.InfoPanel.Visibility = Visibility.Visible;
          this.InfoPanelTransform.TranslateY = toY;
          this.Dispatcher.RunAfterDelay(TimeSpan.FromSeconds(2.0), (Action) (() =>
          {
            if (disposed || this.isPageRemoved)
              return;
            this.ShowReplyConfirmation(false);
          }));
        }
        else
        {
          this.InfoPanel.Opacity = 0.0;
          this.InfoPanel.Visibility = Visibility.Collapsed;
        }
      }), onDisposing: (Action) (() => disposed = true), context: "reply sent on status view");
    }

    private IObservable<bool> SendReply(ExtendedTextInputData inputData)
    {
      if (string.IsNullOrEmpty(inputData?.Text) || this.currentThread?.Jid == null)
        return Observable.Return<bool>(false);
      string jid = this.currentThread.Jid;
      Log.l("statusv3", "sending reply | jid:{0}", (object) jid);
      return Observable.Create<bool>((Func<IObserver<bool>, Action>) (observer =>
      {
        SendMessage.ValidateRecipientPreSending(jid).Take<bool>(1).ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (proceeed =>
        {
          if (!proceeed)
          {
            this.inputBar.SetText(inputData.Text);
            observer.OnCompleted();
          }
          else
          {
            MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
            {
              db.GetConversation(jid, CreateOptions.CreateToDbIfNotFound);
              Message m = new Message(true)
              {
                KeyFromMe = true,
                KeyRemoteJid = jid,
                KeyId = FunXMPP.GenerateMessageId(),
                Data = inputData.Text,
                Status = FunXMPP.FMessage.Status.Unsent,
                MediaWaType = FunXMPP.FMessage.Type.Undefined
              };
              WebPageMetadata linkPreviewData = inputData.LinkPreviewData;
              if (linkPreviewData != null)
              {
                m.MediaWaType = FunXMPP.FMessage.Type.ExtendedText;
                UriMessageWrapper uriMessageWrapper = new UriMessageWrapper(m)
                {
                  Title = linkPreviewData.Title,
                  Description = linkPreviewData.Description,
                  CanonicalUrl = linkPreviewData.CanonicalUrl,
                  MatchedText = linkPreviewData.OriginalUrl
                };
                m.BinaryData = linkPreviewData.ThumbnailBytes;
              }
              Message quotedMessage = inputData.QuotedMessage;
              if (quotedMessage != null)
                m.SetQuote(quotedMessage);
              db.InsertMessageOnSubmit(m);
              db.SubmitChanges();
            }));
            observer.OnNext(true);
            observer.OnCompleted();
          }
        }));
        return (Action) (() => { });
      }));
    }

    private IObservable<ObservableCollection<StatusReadRecipientViewModel>> GetReadRecipients(
      WaStatus status)
    {
      int msgId = status.MessageId;
      return Observable.Create<ObservableCollection<StatusReadRecipientViewModel>>((Func<IObserver<ObservableCollection<StatusReadRecipientViewModel>>, Action>) (observer =>
      {
        object disposeLock = new object();
        bool disposed = false;
        IDisposable newReceiptSub = (IDisposable) null;
        List<ReceiptState> receipts = (List<ReceiptState>) null;
        lock (disposeLock)
        {
          if (!disposed)
            MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
            {
              receipts = db.GetReceiptsForMessage(msgId, new FunXMPP.FMessage.Status[2]
              {
                FunXMPP.FMessage.Status.ReadByTarget,
                FunXMPP.FMessage.Status.PlayedByTarget
              }, sortByTimestamp: true);
              newReceiptSub = MessagesContext.Events.NewReceiptSubject.Where<ReceiptState>((Func<ReceiptState, bool>) (rs => rs.MessageId == msgId && rs.Status.GetOverrideWeight() >= FunXMPP.FMessage.Status.ReadByTarget.GetOverrideWeight())).ObserveOnDispatcher<ReceiptState>().Subscribe<ReceiptState>(new Action<ReceiptState>(this.OnNewReadReceipt));
            }));
        }
        if (receipts != null)
        {
          ObservableCollection<StatusReadRecipientViewModel> observableCollection = new ObservableCollection<StatusReadRecipientViewModel>(receipts.MakeUnique<ReceiptState, string>((Func<ReceiptState, string>) (rs => rs.Jid)).Select<ReceiptState, StatusReadRecipientViewModel>((Func<ReceiptState, StatusReadRecipientViewModel>) (rs => new StatusReadRecipientViewModel(rs))));
          observer.OnNext(observableCollection);
        }
        return (Action) (() =>
        {
          lock (disposeLock)
          {
            disposed = true;
            newReceiptSub.SafeDispose();
            newReceiptSub = (IDisposable) null;
          }
        });
      }));
    }

    private void ShowReadRecipients(bool show)
    {
      if (show)
      {
        this.StatusView.Pause(false);
        this.Mask.Visibility = Visibility.Visible;
        if (Settings.EnableReadReceipts)
        {
          if (this.ReadRecipientList.ItemsSource == null)
            this.ReadRecipientList.ItemsSource = (IList) this.readRecipients;
          if (this.readRecipients != null && this.readRecipients.Any<StatusReadRecipientViewModel>())
          {
            this.ReadRecipientListTooltipBlock.Visibility = Visibility.Collapsed;
          }
          else
          {
            this.ReadRecipientListTooltipBlock.Text = AppResources.StatusTooltipNoViews;
            this.ReadRecipientListTooltipBlock.Visibility = Visibility.Visible;
          }
        }
        else
        {
          this.ReadRecipientList.ItemsSource = (IList) new object[0];
          this.ReadRecipientListTooltipBlock.Text = AppResources.StatusTooltipReadReceiptsDisabled;
          this.ReadRecipientListTooltipBlock.Visibility = Visibility.Visible;
        }
        this.ReadRecipientListZoomBox.Visibility = Visibility.Visible;
        this.BottomBarBackground.Opacity = 1.0;
      }
      else
      {
        this.StatusView.Resume();
        this.Mask.Visibility = this.ReadRecipientListZoomBox.Visibility = Visibility.Collapsed;
        this.BottomBarBackground.Opacity = 0.5;
      }
    }

    private void OnStatusLoaded(WaStatus status, bool allowReply)
    {
      Log.d("statusv3", string.Format("Status loaded from {0} - {1}", (object) status.Jid, (object) status.StatusId));
      this.isStatusInPlay = true;
      this.ShowBottomBar(true, true);
      this.ShowReadRecipients(false);
      this.ReadRecipientList.ItemsSource = (IList) null;
      this.readReceiptsSub.SafeDispose();
      this.readReceiptsSub = (IDisposable) null;
      this.readRecipients = (ObservableCollection<StatusReadRecipientViewModel>) null;
      this.pendingNewReceipts = (List<ReceiptState>) null;
      this.currentStatus = status;
      if (JidHelper.IsSelfJid(status.Jid))
      {
        this.MainButtonIcon.Source = this.viewsIcon ?? (this.viewsIcon = (System.Windows.Media.ImageSource) AssetStore.EyeIconWhite);
        this.MainButtonIcon.Width = this.MainButtonIcon.Height = 36.0;
        this.MainButtonCaptionBlock.Visibility = Visibility.Visible;
        this.MoreButton.Visibility = Visibility.Collapsed;
        int msgId = status.MessageId;
        this.readReceiptsSub = this.GetReadRecipients(status).SubscribeOn<ObservableCollection<StatusReadRecipientViewModel>>((IScheduler) AppState.Worker).ObserveOnDispatcher<ObservableCollection<StatusReadRecipientViewModel>>().Subscribe<ObservableCollection<StatusReadRecipientViewModel>>((Action<ObservableCollection<StatusReadRecipientViewModel>>) (recipients =>
        {
          if (msgId == this.currentStatus.MessageId)
          {
            int ackedCount = -1;
            int sentCount = -1;
            this.OnReadRecipientsLoaded(recipients, ackedCount, sentCount);
          }
          else
            Log.l("statusv3", "skip loaded read receipts | {0} vs {1}", (object) this.currentStatus.MessageId, (object) msgId);
        }));
        this.BottomBar.Visibility = Visibility.Visible;
      }
      else if (JidHelper.IsUserJid(status.Jid))
      {
        this.MainButtonIcon.Source = this.msgIcon ?? (this.msgIcon = (System.Windows.Media.ImageSource) AssetStore.MessageIconWhite);
        this.MainButtonIcon.Width = this.MainButtonIcon.Height = 56.0;
        this.MainButtonCaptionBlock.Visibility = this.MoreButton.Visibility = Visibility.Collapsed;
        this.MainButton.Opacity = allowReply ? 1.0 : 0.35;
        this.MainButton.IsEnabled = allowReply;
        this.BottomBar.Visibility = Visibility.Visible;
      }
      else
        this.BottomBar.Visibility = Visibility.Collapsed;
    }

    private void OnNewReadReceipt(ReceiptState rs)
    {
      if (this.readRecipients == null)
      {
        if (this.pendingNewReceipts == null)
          this.pendingNewReceipts = new List<ReceiptState>();
        this.pendingNewReceipts.Add(rs);
      }
      else
      {
        if (this.readRecipients.FirstOrDefault<StatusReadRecipientViewModel>((Func<StatusReadRecipientViewModel, bool>) (vm => vm.Jid == rs.Jid)) == null)
        {
          StatusReadRecipientViewModel item = new StatusReadRecipientViewModel(rs);
          this.readRecipients.InsertInOrder<StatusReadRecipientViewModel>(item, (Func<StatusReadRecipientViewModel, StatusReadRecipientViewModel, bool>) ((vm1, vm2) => vm1.Timestamp > vm2.Timestamp));
          this.Dispatcher.BeginInvoke((Action) (() =>
          {
            try
            {
              this.ReadRecipientList.ScrollTo((object) item);
            }
            catch (Exception ex)
            {
            }
          }));
        }
        int count = this.readRecipients.Count;
        this.MainButtonCaptionBlock.Text = count.ToString();
        this.ReadCountBlock.Text = Plurals.Instance.GetString(AppResources.StatusReadRecipientCountPlural, count);
      }
    }

    private void OnReadRecipientsLoaded(
      ObservableCollection<StatusReadRecipientViewModel> recipients,
      int ackedCount,
      int sentCount)
    {
      if (recipients == null)
        return;
      this.readRecipients = recipients;
      int count = this.readRecipients.Count;
      this.MainButtonCaptionBlock.Text = count.ToString();
      this.ReadCountBlock.Text = Plurals.Instance.GetString(AppResources.StatusReadRecipientCountPlural, count);
      if (this.pendingNewReceipts == null || !this.pendingNewReceipts.Any<ReceiptState>())
        return;
      foreach (ReceiptState pendingNewReceipt in this.pendingNewReceipts)
        this.OnNewReadReceipt(pendingNewReceipt);
    }

    private static void OnRootFrameTranslateYChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      if (!(d is WaStatusViewPage waStatusViewPage))
        return;
      waStatusViewPage.UpdateForSIPOffset(-(double) e.NewValue);
    }

    private void OnLoaded(object sender, EventArgs e)
    {
      if (this.isPageLoaded)
        return;
      this.isPageLoaded = true;
      if (this.currentThread == null)
        return;
      this.StatusView.RenderStatusThread(this.currentThread, this.startFromUnviewed, this.startWithSoundMute);
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);
      if (this.sysTrayHideSub == null)
        this.sysTrayHideSub = SysTrayHelper.SysTrayKeeper.Instance.RequestHide(false);
      if ((this.currentThread == null ? 1 : (this.isPageNavigatedTo ? 1 : 0)) != 0)
      {
        this.Dispatcher.BeginInvoke((Action) (() =>
        {
          if (this.isPageRemoved)
            return;
          NavUtils.GoBack(this.NavigationService);
        }));
      }
      else
      {
        this.isPageNavigatedTo = true;
        this.BindRootFrameTranslateY();
        UIUtils.EnableWakeLock(true);
      }
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
      App.CurrentApp.RootFrame.RenderTransform = App.CurrentApp.OriginalRootFrameTransform;
      base.OnNavigatedFrom(e);
      this.sysTrayHideSub.SafeDispose();
      this.sysTrayHideSub = (IDisposable) null;
      UIUtils.EnableWakeLock(false);
      this.ClearValue(WaStatusViewPage.RootFrameTranslateYProperty);
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      base.OnRemovedFromJournal(e);
      Log.d("statusv3", "WaStatusViewPage removed from journal");
      this.readReceiptsSub.SafeDispose();
      this.readReceiptsSub = (IDisposable) null;
      this.disposables.ForEach((Action<IDisposable>) (d => d.SafeDispose()));
      this.disposables.Clear();
      this.sbSub.SafeDispose();
      this.sbSub = (IDisposable) null;
      this.StatusView.Dispose();
      try
      {
        this.inputBar.Dispose();
      }
      catch (Exception ex)
      {
        Log.SendCrashLog(ex, "dispose input bar");
      }
    }

    private void OnBackKey(CancelEventArgs e)
    {
      e.Cancel = true;
      if (this.ReadRecipientListZoomBox.Visibility == Visibility.Visible)
        this.ShowReadRecipients(false);
      else
        this.SlideDownAndBackOut();
    }

    private void MainButton_Click(object sender, RoutedEventArgs e)
    {
      if (!JidHelper.IsUserJid(this.currentThread?.Jid))
        return;
      if (JidHelper.IsSelfJid(this.currentThread.Jid))
      {
        this.ShowReadRecipients(this.ReadRecipientListZoomBox.Visibility == Visibility.Collapsed);
      }
      else
      {
        this.inputBar.SetTargetJid(this.currentThread.Jid);
        this.inputBar.Opacity = 1.0;
        this.inputBar.IsHitTestVisible = true;
        this.inputBar.SetQuotedMessage(this.StatusView.CurrentStatusMessageViewModel);
        this.inputBar.OpenKeyboard();
      }
    }

    private void Mask_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      this.ShowReadRecipients(false);
    }

    public IObservable<bool> SendSticker(Sticker sticker)
    {
      if (sticker.MediaKey == null || sticker.EncodedFileHash == null || this.currentThread?.Jid == null)
        return Observable.Return<bool>(false);
      string jid = this.currentThread.Jid;
      Log.l("statusv3", "sending reply | jid:{0}", (object) jid);
      return Observable.Create<bool>((Func<IObserver<bool>, Action>) (observer =>
      {
        SendMessage.ValidateRecipientPreSending(jid).Take<bool>(1).ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (proceeed =>
        {
          if (!proceeed)
          {
            observer.OnCompleted();
          }
          else
          {
            WhatsApp.CommonOps.SendSticker.Send(sticker, this.currentThread.Jid, quotedMessage: this.inputBar.GetQuotedMessage());
            observer.OnNext(true);
            observer.OnCompleted();
          }
        }));
        return (Action) (() => { });
      }));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/WaStatusViewPage.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.StatusView = (WaStatusViewControl) this.FindName("StatusView");
      this.Mask = (Rectangle) this.FindName("Mask");
      this.ReadRecipientListZoomBox = (ZoomBox) this.FindName("ReadRecipientListZoomBox");
      this.ReadRecipientPanelTransform = (CompositeTransform) this.FindName("ReadRecipientPanelTransform");
      this.ReadCountBlock = (TextBlock) this.FindName("ReadCountBlock");
      this.ReadRecipientList = (WhatsApp.CompatibilityShims.LongListSelector) this.FindName("ReadRecipientList");
      this.ReadRecipientListTooltipBlock = (TextBlock) this.FindName("ReadRecipientListTooltipBlock");
      this.BottomBarsZoomBox = (ZoomBox) this.FindName("BottomBarsZoomBox");
      this.BottomBar = (Grid) this.FindName("BottomBar");
      this.BottomBarBackground = (Rectangle) this.FindName("BottomBarBackground");
      this.MainButton = (Button) this.FindName("MainButton");
      this.MainButtonIcon = (Image) this.FindName("MainButtonIcon");
      this.MainButtonCaptionBlock = (TextBlock) this.FindName("MainButtonCaptionBlock");
      this.MoreButton = (TextBlock) this.FindName("MoreButton");
      this.InfoPanel = (StackPanel) this.FindName("InfoPanel");
      this.InfoPanelTransform = (CompositeTransform) this.FindName("InfoPanelTransform");
      this.InfoBlock = (TextBlock) this.FindName("InfoBlock");
    }
  }
}
