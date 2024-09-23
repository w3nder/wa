// Decompiled with JetBrains decompiler
// Type: WhatsApp.ChatPage
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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using WhatsApp.CommonOps;
using WhatsApp.Events;
using WhatsApp.WaCollections;
using WhatsApp.WaViewModels;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;

#nullable disable
namespace WhatsApp
{
  public class ChatPage : PhoneApplicationPage, IMessageLoadingHandler, GifSendingPage
  {
    public static readonly DependencyProperty RootFrameTranslateYProperty = DependencyProperty.Register("RootFrameTranslateY", typeof (double), typeof (ChatPage), new PropertyMetadata((object) 0.0, new PropertyChangedCallback(ChatPage.OnRootFrameTranslateYChanged)));
    private string logHeader;
    private Conversation convo_;
    private PresenceEventArgs lastPresence_;
    private object composingStateLock_ = new object();
    private ChatPage.ComposingState composingState_;
    private bool isPageRemoved_;
    private List<IDisposable> uiDisposables = new List<IDisposable>();
    private List<IDisposable> asyncDisposables = new List<IDisposable>();
    private IDisposable openWindowSub;
    private IDisposable presenceSendSub;
    private IMessageListControl MessageList;
    private FrameworkElement MessageListElement;
    private ChatInputBar inputBar;
    private FrameworkElement inputBarElement;
    private ChatPageTitlePanelWrapper titlePanel;
    private CompositeTransform titlePanelTransform;
    private GlobalProgressIndicator progressIndicator;
    private bool inputHasFocus;
    private bool ignoreTextInputFocusChangeOnce_;
    private double rootFrameShift_;
    private TranslateTransform shiftedRootFrameTransformPortrait_ = new TranslateTransform();
    private ChatPage.KeyboardGapWorkaround keyboardGapWorkaround_ = ChatPage.KeyboardGapWorkaround.TranslateRootFrame;
    private int layoutRootBottomShift_;
    private PttPlaybackWrapper audioPlayback_;
    private Popup attachPanelPopup;
    private AttachPanel attachPanel;
    private DispatcherTimer disablePttTimeoutTimer_;
    private MessageLoader msgLoader_;
    private bool forceInitialScrollToRecent;
    private bool c2cUriPhoneNumberFlag;
    private DeepLinkData c2cDeepLinkData;
    private bool isWallpaperDirty;
    private IDisposable wallpaperChangedSub;
    private ChatPageViewModel viewModel;
    private double? textFontSize;
    private static ChatPage.InitState nextInstanceInitState = (ChatPage.InitState) null;
    private ChatPage.InitState initState;
    private Microsoft.Phone.Shell.ApplicationBar selectModeBar;
    private Microsoft.Phone.Shell.ApplicationBar defaultBar;
    private bool? isSuspicious;
    private Panel numberChangePanel;
    private bool shouldCheckSuspicious = true;
    private Grid spamReportPanel;
    private Button blockToggleButton;
    private IDisposable blockListChangedSub;
    private Grid contactVCardPanel;
    private WallpaperPanel wallpaperPanel;
    private static SimpleSound messageSendSound = (SimpleSound) null;
    private bool deferMarkRead;
    private IReadOnlyList<StorageFile> filePickerReturnFiles;
    private NativeMediaPickerState currentMediaPickerState;
    private FileOpenPicker filePicker;
    private NavigationOutTransition savedOutTransition;
    private CoreApplicationView appView;
    private bool shouldUpdateUnreadDivider = true;
    private bool useTitleBarMenu = true;
    private bool onLoadedInited_;
    private DateTime lastInputModeSetTime_ = DateTime.Now;
    private bool isInSelectionMode;
    private OptimisticUploadManager optUploadManager;
    private object optimisticUploadLock = new object();
    private MediaSharingState currentOptUploadSharingState;
    private IDisposable currentSharingStateUnsub;
    private List<MediaSharingState.PicInfo> optimisticUploadPicItemList = new List<MediaSharingState.PicInfo>();
    private bool isForwardButtonEnabled;
    private bool isCopyButtonEnabled;
    public static readonly DependencyProperty ViewportOriginYProperty = DependencyProperty.Register("ViewportOriginY", typeof (double), typeof (ViewportControl), new PropertyMetadata((object) 0.0, new PropertyChangedCallback(ChatPage.OnViewportOriginYChanged)));
    internal Storyboard ChatTitleContinuumIn;
    internal Grid LayoutRoot;
    internal Grid ReadOnlyPanel;
    internal RichTextBox ReadOnlyHelpTextBox;
    private bool _contentLoaded;

    public static ChatPage Current { get; set; }

    public static string SearchTermForNextEntrance { get; set; }

    public string LogHeader
    {
      get
      {
        if (this.logHeader == null)
        {
          if (this.Jid == null)
            return "chatpage";
          this.logHeader = string.Format("chatpage {0}", (object) this.Jid);
        }
        return this.logHeader;
      }
    }

    public string Jid { get; private set; }

    public bool IsScrolledToRecent => this.MessageList.IsScrolledToRecent();

    internal ChatPage.InputMode CurrentInputMode
    {
      get => this.viewModel.InputMode;
      set => this.viewModel.InputMode = value;
    }

    public bool IsPttRecordingInProgress { get; set; }

    public Popup AttachPanelPopup
    {
      get
      {
        this.EnsureAttachPanelPopup();
        return this.attachPanelPopup;
      }
    }

    private AttachPanel AttachPanel
    {
      get
      {
        if (this.attachPanel == null)
        {
          AttachPanel attachPanel = new AttachPanel();
          attachPanel.Orientation = this.Orientation;
          attachPanel.FlowDirection = App.CurrentApp.RootFrame.FlowDirection;
          attachPanel.VerticalAlignment = VerticalAlignment.Center;
          attachPanel.HorizontalAlignment = HorizontalAlignment.Center;
          this.attachPanel = attachPanel;
          this.attachPanel.AttachmentActionChosen += (AttachPanel.AttachmentActionChosenHandler) ((sender, actionChosen) =>
          {
            this.GetAudioPlayback().Player.Stop(true);
            this.SetInputMode(ChatPage.InputMode.None, false, "attachment type chosen");
            this.currentMediaPickerState = (NativeMediaPickerState) null;
            switch (actionChosen)
            {
              case AttachPanel.ActionType.RecordVideo:
                this.LaunchVideoRecorder();
                break;
              case AttachPanel.ActionType.TakePicture:
              case AttachPanel.ActionType.TakePictureOrVideo:
                this.LaunchCamera();
                break;
              case AttachPanel.ActionType.ChooseAudio:
                this.LaunchAudioPicker();
                break;
              case AttachPanel.ActionType.ChooseDocument:
                this.LaunchDocumentPicker();
                break;
              case AttachPanel.ActionType.ChoosePicture:
                this.LaunchAlbumsPicker(MediaSharingState.SharingMode.ChoosePicture);
                break;
              case AttachPanel.ActionType.ChooseVideo:
                this.LaunchAlbumsPicker(MediaSharingState.SharingMode.ChooseVideo);
                break;
              case AttachPanel.ActionType.ChoosePictureAndVideo:
                this.LaunchAlbumsPicker(MediaSharingState.SharingMode.ChooseMedia);
                break;
              case AttachPanel.ActionType.ShareContact:
                this.LaunchContactPicker();
                break;
              case AttachPanel.ActionType.ShareLocation:
                this.LaunchLocationShare();
                break;
            }
          });
        }
        return this.attachPanel;
      }
    }

    public bool IsWallpaperSet => this.viewModel.Wallpaper != null;

    private double TextFontSize
    {
      set
      {
        if (this.textFontSize.HasValue && this.textFontSize.Value == value)
          return;
        int num = this.textFontSize.HasValue ? 1 : 0;
        this.textFontSize = new double?(value);
        if (num == 0)
          return;
        this.MessageList.RefreshMessages((Action<MessageViewModel>) (vm => vm.RefreshTextFontSize()));
        this.inputBar.RefreshTextFontSize();
      }
    }

    public static ChatPage.InitState NextInstanceInitState
    {
      set
      {
        ChatPage.nextInstanceInitState.SafeDispose();
        ChatPage.nextInstanceInitState = value;
      }
    }

    private bool IsSuspicious
    {
      get => this.isSuspicious ?? false;
      set => this.isSuspicious = new bool?(value);
    }

    public bool IsPickingFiles => this.filePicker != null;

    public ChatPage()
    {
      this.appView = CoreApplication.GetCurrentView();
      this.InitializeComponent();
      if (ChatPage.nextInstanceInitState != null)
      {
        this.initState = ChatPage.nextInstanceInitState;
        this.msgLoader_ = this.initState.MessageLoader;
        ChatPage.nextInstanceInitState = (ChatPage.InitState) null;
        if (this.initState.SearchResult != null)
          this.deferMarkRead = true;
        this.forceInitialScrollToRecent = this.initState.ForceInitialScrollToRecent;
        if (this.initState.SharedDeepLinkData != null)
        {
          this.c2cUriPhoneNumberFlag = true;
          this.c2cDeepLinkData = this.initState.SharedDeepLinkData;
        }
      }
      this.viewModel = new ChatPageViewModel(this.Orientation);
      this.DataContext = (object) this.viewModel;
      this.InitPage();
      this.keyboardGapWorkaround_ = ChatPage.KeyboardGapWorkaround.None;
    }

    private void InitPage()
    {
      this.progressIndicator = new GlobalProgressIndicator((DependencyObject) this);
      this.InitDefaultAppBar();
      this.InitTitlePanelWrapper();
      this.InitMessageList();
      this.InitInputBar();
      this.UpdateElementsForInputModeChange(ChatPage.InputMode.None);
      BackKeyBroker.Get((PhoneApplicationPage) this, 1).Subscribe<CancelEventArgs>(new Action<CancelEventArgs>(this.OnBackKey));
      this.Loaded += new RoutedEventHandler(this.OnLoaded);
    }

    private void InitMessageList()
    {
      MessageListControl messageListControl1 = new MessageListControl(true, true, true);
      messageListControl1.CacheMode = (CacheMode) new BitmapCache();
      MessageListControl messageListControl2 = messageListControl1;
      this.MessageList = (IMessageListControl) messageListControl2;
      this.MessageListElement = (FrameworkElement) messageListControl2;
      Grid.SetRow(this.MessageListElement, 1);
      this.LayoutRoot.Children.Insert(this.LayoutRoot.Children.IndexOf((UIElement) this.titlePanel), (UIElement) this.MessageListElement);
      this.MessageList.SelectionChanged += new EventHandler(this.MessageList_SelectionChanged);
      this.MessageList.OlderMessagesRequested += new EventHandler(this.MessageList_OlderMessagesRequested);
      this.MessageList.NewerMessagesRequested += new EventHandler(this.MessageList_NewerMessagesRequested);
      this.MessageList.MultiSelectionsChanged += new EventHandler(this.MessageList_MultiSelectionsChanged);
      this.MessageList.ScrollToBottomRequested += new EventHandler(this.MessageList_ScrollToBottomRequested);
      if (!this.deferMarkRead)
        return;
      this.asyncDisposables.Add(this.MessageList.NewerMessageRealized().ObserveOn<Message>((IScheduler) AppState.Worker).Subscribe<Message>(new Action<Message>(this.OnNewerMessageRealized)));
    }

    private void InitOnLoaded()
    {
      if (this.onLoadedInited_ || this.isPageRemoved_)
        return;
      this.onLoadedInited_ = true;
      this.ResetPageMenu();
      this.titlePanel.UpdateTitleInfoPanel();
      if (JidHelper.IsUserJid(this.Jid) && !JidHelper.IsJidInAddressBook(this.Jid))
        this.InitContactVCardPanel();
      this.InitSubscriptions();
      if (this.initState?.QuotedMessage != null)
      {
        MessageViewModel mvm = MessageViewModel.CreateForMessage(this.initState.QuotedMessage, false, false, false).FirstOrDefault<MessageViewModel>();
        if (mvm != null)
          this.inputBar.SetQuotedMessage(mvm);
        this.initState.QuotedMessage = (Message) null;
      }
      else
      {
        if (this.initState?.QuotedChat == null)
          return;
        this.inputBar.SetQuotedChat(this.initState.QuotedChat);
        this.initState.QuotedChat = (string) null;
      }
    }

    private void InitOnNavigatedTo()
    {
      Conversation convo = (Conversation) null;
      int unreadCount = 0;
      int? firstUnreadMsgId = new int?();
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        convo = this.GetConversation(db, true, "init");
        this.viewModel.Conversation = convo;
        firstUnreadMsgId = convo.FirstUnreadMessageID;
        unreadCount = convo.GetUnreadMessagesCount();
        this.TryMarkRead(db, convo.Jid, true, new int?(), "init on nav to");
        if (convo.IsGroup())
        {
          this.viewModel.IsReadOnly = convo.IsReadOnly();
          this.viewModel.IsGroupParticipant = convo.IsGroupParticipant();
          this.viewModel.IsAnnouncementOnly = convo.IsAnnounceOnlyForUser();
          this.viewModel.IsMuted = convo.IsMuted();
          this.UpdateReadOnlyPanel();
          if (!convo.IsConversationSeen())
          {
            this.viewModel.WasConversationSeenOnEntry = false;
            this.InitTitleInfoPanel();
            convo.SetConversationSeen();
          }
          this.AddSubscription(db.UpdatedConversationSubject.Select<ConvoAndMessage, Conversation>((Func<ConvoAndMessage, Conversation>) (convoAndMsg => convoAndMsg.Conversation)).Merge<Conversation>(FunEventHandler.Events.GroupMembershipUpdatedSubject.Select<FunEventHandler.Events.ConversationWithFlags, Conversation>((Func<FunEventHandler.Events.ConversationWithFlags, Conversation>) (convoWithFlags => convoWithFlags.Conversation))).Where<Conversation>((Func<Conversation, bool>) (c => c.Jid == convo.Jid)).ObserveOnDispatcher<Conversation>().Subscribe<Conversation>((Action<Conversation>) (c =>
          {
            if (this.viewModel.IsReadOnly != c.IsReadOnly())
              this.ResetPageMenu();
            this.viewModel.IsReadOnly = c.IsReadOnly();
            this.viewModel.IsGroupParticipant = c.IsGroupParticipant();
            this.viewModel.IsAnnouncementOnly = c.IsAnnounceOnlyForUser();
            this.viewModel.IsMuted = c.IsMuted();
            this.UpdateReadOnlyPanel();
            if (this.spamReportPanel == null)
              return;
            bool shouldHide = true;
            MessagesContext.Run((MessagesContext.MessagesCallback) (db2 =>
            {
              if (!SuspiciousJid.IsJidSuspicious(db2, c.Jid, false))
                return;
              shouldHide = false;
            }));
            if (!shouldHide)
              return;
            this.TryShowSpamReportingPanel(false, true);
          })));
        }
        else if (convo.IsPsaChat())
          this.Dispatcher.BeginInvoke((Action) (() =>
          {
            this.viewModel.IsReadOnly = true;
            this.ReadOnlyPanel.Background = UIUtils.PhoneChromeBrush;
            this.ReadOnlyPanel.MinHeight = 0.0;
            this.ReadOnlyHelpTextBox.FontSize = UIUtils.FontSizeSmall;
            this.ReadOnlyHelpTextBox.Foreground = (Brush) UIUtils.ForegroundBrush;
            this.ReadOnlyHelpTextBox.HorizontalAlignment = HorizontalAlignment.Center;
            this.ReadOnlyHelpTextBox.Blocks.Add((Block) new Paragraph()
            {
              Inlines = {
                AppResources.OfficialAnnouncementsPanelText
              }
            });
            this.inputBarElement.Visibility = Visibility.Collapsed;
          }));
        this.AddSubscription(db.NewReceiptSubject.Where<ReceiptState>((Func<ReceiptState, bool>) (r =>
        {
          if (r.Status == FunXMPP.FMessage.Status.ReceivedByServer && Settings.EnableInAppNotificationSound)
          {
            Message m = (Message) null;
            MessagesContext.Run((MessagesContext.MessagesCallback) (mdb => m = mdb.GetMessageById(r.MessageId)));
            if (m != null && m.KeyRemoteJid == convo.Jid)
              return true;
          }
          return false;
        })).ObserveOnDispatcher<ReceiptState>().Subscribe<ReceiptState>((Action<ReceiptState>) (r =>
        {
          if (ChatPage.messageSendSound == null)
            ChatPage.messageSendSound = new SimpleSound("Sounds/send_message.wav", 0.5f);
          ChatPage.messageSendSound.Play();
        })));
        db.SubmitChanges();
      }));
      if (this.msgLoader_ == null || this.msgLoader_.Jid != this.Jid)
        this.msgLoader_ = MessageLoader.Get(this.Jid, firstUnreadMsgId, unreadCount);
      this.msgLoader_.MessageListener = (IMessageLoadingHandler) this;
      this.titlePanel.Render(convo);
      if (!string.IsNullOrEmpty(convo.ComposingText))
        this.inputBar.SetText(convo.ComposingText.Trim());
      Log.l(this.LogHeader, "init on nav to | done | unread={0}", (object) unreadCount);
    }

    private void TryMarkRead(
      MessagesContext db,
      string jid,
      bool markReadRegardlessChatOpenState,
      int? lastReadMsgId,
      string context)
    {
      Log.d(this.LogHeader, "mark read | context:{0}", (object) context);
      if (this.deferMarkRead)
        Log.l(this.LogHeader, "mark read | deferred");
      else if ((markReadRegardlessChatOpenState ? 1 : (AppState.IsConversationOpen(jid) ? 1 : 0)) != 0)
      {
        if (lastReadMsgId.HasValue)
          MarkChatRead.MarkReadToMessage(db, jid, lastReadMsgId, true, true);
        else
          MarkChatRead.MarkRead(db, jid, true, true);
      }
      else
        Log.d(this.LogHeader, "mark read | skipped");
    }

    private void InitSubscriptions()
    {
      List<IDisposable> subs = new List<IDisposable>();
      subs.Add(QuotedMessageViewPanel.JumpToQuotedMessageObservable().ObserveOnDispatcher<Pair<FunXMPP.FMessage.Key, FunXMPP.FMessage.Key>>().Subscribe<Pair<FunXMPP.FMessage.Key, FunXMPP.FMessage.Key>>((Action<Pair<FunXMPP.FMessage.Key, FunXMPP.FMessage.Key>>) (msgKeyPair => this.OnJumpToMessageRequest(msgKeyPair.First, msgKeyPair.Second))));
      subs.Add(QuotedMessageViewPanel.JumpToQuotedGroupObservable().ObserveOnDispatcher<string>().Subscribe<string>(new Action<string>(this.OnJumpToGroupRequest)));
      subs.Add(MessageMenu.GetMessageQuotedObservable().ObserveOnDispatcher<Pair<MessageViewModel, bool>>().Subscribe<Pair<MessageViewModel, bool>>((Action<Pair<MessageViewModel, bool>>) (p =>
      {
        MessageViewModel first = p.First;
        Message message = first?.Message;
        if (message == null)
          return;
        if (p.Second)
        {
          ChatPage.NextInstanceInitState = new ChatPage.InitState()
          {
            QuotedMessage = message,
            InputMode = ChatPage.InputMode.Keyboard
          };
          NavUtils.NavigateToChat(message.GetSenderJid(), false);
        }
        else
        {
          this.inputBar.SetQuotedMessage(first);
          this.Dispatcher.BeginInvoke((Action) (() => this.inputBar.OpenKeyboard()));
        }
      })));
      if (!JidHelper.IsBroadcastJid(this.Jid))
      {
        IObservable<TextChangedEventArgs> source1 = this.inputBar.TextChangedObservable().SubscribeOnDispatcher<TextChangedEventArgs>().ObserveOn<TextChangedEventArgs>((IScheduler) AppState.Worker);
        bool previousTypingState = false;
        IObservable<bool> source2 = this.HasValueWithTimeout<TextChangedEventArgs>(source1, TimeSpan.FromMilliseconds(2500.0)).Synchronize<bool>(this.composingStateLock_).Merge<bool>(MessagesContext.Events.NewMessagesSubject.Where<Message>((Func<Message, bool>) (msg => msg.KeyFromMe && msg.KeyRemoteJid == this.Jid)).ObserveOn<Message>((IScheduler) AppState.Worker).Synchronize<Message>(this.composingStateLock_).Select<Message, bool>((Func<Message, bool>) (_ =>
        {
          this.presenceSendSub = (IDisposable) null;
          this.composingState_ = ChatPage.ComposingState.None;
          return false;
        }))).Do<bool>((Action<bool>) (typing =>
        {
          if (typing == previousTypingState)
            return;
          if (typing)
          {
            if (this.lastPresence_ != null && this.lastPresence_.State != Presence.Offline || this.Jid.IsGroupJid())
              this.SendComposing();
            this.composingState_ = ChatPage.ComposingState.Typing;
          }
          else if (this.composingState_ == ChatPage.ComposingState.Typing)
          {
            this.composingState_ = ChatPage.ComposingState.None;
            this.DisposePresence();
          }
          previousTypingState = typing;
        }));
        subs.Add(source2.Subscribe<bool>());
        IDisposable disposable1 = (Voip.IsInCall ? Observable.Return<Unit>(new Unit()) : Observable.Empty<Unit>()).Concat<Unit>(VoipHandler.CallStateChangedSubject.Select<WaCallStateChangedArgs, Unit>((Func<WaCallStateChangedArgs, Unit>) (_ => new Unit()))).ObserveOnDispatcher<Unit>().Subscribe<Unit>((Action<Unit>) (_ =>
        {
          this.UpdatePttRecordButton();
          bool enable = !Voip.IsInCall;
          if (JidHelper.IsUserJid(this.Jid))
            this.EnableVoipButtons(enable);
          this.AttachPanel.EnableActions(new AttachPanel.ActionType[2]
          {
            AttachPanel.ActionType.RecordVideo,
            AttachPanel.ActionType.TakePictureOrVideo
          }, enable);
        }));
        subs.Add(disposable1);
        if (JidHelper.IsGroupJid(this.Jid) || JidHelper.IsUserJid(this.Jid))
        {
          IDisposable disposable2 = PresenceState.Instance.GetPresence(this.Jid, this.Jid.IsUserJid()).ObserveOnDispatcher<PresenceEventArgs>().Subscribe<PresenceEventArgs>((Action<PresenceEventArgs>) (presence =>
          {
            this.lastPresence_ = presence;
            this.titlePanel.LastPresence = presence == null ? (ProcessedPresence) null : presence.FormattedString;
          }));
          subs.Add(disposable2);
        }
      }
      this.AddSubscriptions(subs);
    }

    private PlayAudioMessage GetAudioPlayback()
    {
      if (this.audioPlayback_ == null)
        this.InitAudioPlayback();
      return this.audioPlayback_.Device;
    }

    private void InitAudioPlayback()
    {
      if (this.audioPlayback_ != null)
        return;
      this.audioPlayback_ = new PttPlaybackWrapper();
      this.audioPlayback_.PlaybackStarted = new Action(this.OnPttPlaybackStarted);
      this.audioPlayback_.PlaybackStopped = new Action(this.OnPttPlaybackStopped);
    }

    private void DisposeAudioPlayback()
    {
      this.audioPlayback_.SafeDispose();
      this.audioPlayback_ = (PttPlaybackWrapper) null;
    }

    private IObservable<bool> HasValueWithTimeout<T>(IObservable<T> source, TimeSpan timeout)
    {
      return Observable.Create<bool>((Func<IObserver<bool>, Action>) (observer =>
      {
        IDisposable timerDisp = (IDisposable) null;
        IDisposable sourceDisp = (IDisposable) null;
        bool cancel = false;
        Action cancelLast = (Action) null;
        object @lock = new object();
        Action gotValue = (Action) (() =>
        {
          lock (@lock)
          {
            observer.OnNext(true);
            timerDisp.SafeDispose();
            if (cancelLast != null)
              cancelLast();
            bool innerCancel = false;
            timerDisp = cancel ? (IDisposable) null : PooledTimer.Instance.Schedule(timeout, (Action) (() =>
            {
              lock (@lock)
              {
                if (innerCancel)
                  return;
                observer.OnNext(false);
              }
            }));
            cancelLast = (Action) (() => innerCancel = true);
          }
        });
        sourceDisp = source.Subscribe<T>((Action<T>) (_ => gotValue()));
        return (Action) (() =>
        {
          if (cancel)
            return;
          lock (@lock)
          {
            cancel = true;
            timerDisp.SafeDispose();
            sourceDisp.SafeDispose();
            timerDisp = sourceDisp = (IDisposable) null;
            if (cancelLast == null)
              return;
            cancelLast();
          }
        });
      }));
    }

    private void SendComposing(Presence p = Presence.OnlineAndTyping)
    {
      lock (this.composingStateLock_)
      {
        if (this.composingState_ == ChatPage.ComposingState.Typing && p == Presence.OnlineAndTyping || this.composingState_ == ChatPage.ComposingState.Recording && p == Presence.OnlineAndRecording)
          return;
        PresenceState.Instance.SendComposing(this.Jid, p);
        this.presenceSendSub = (IDisposable) new DisposableAction((Action) (() =>
        {
          if ((this.lastPresence_ == null || this.lastPresence_.State == Presence.Offline) && !JidHelper.IsGroupJid(this.Jid))
            return;
          PresenceState.Instance.SendPaused(this.Jid);
        }));
      }
    }

    private void DisposePresence()
    {
      lock (this.composingStateLock_)
      {
        this.presenceSendSub.SafeDispose();
        this.presenceSendSub = (IDisposable) null;
      }
    }

    private void SendRecordingVoice() => this.SendComposing(Presence.OnlineAndRecording);

    private void AddSubscriptions(List<IDisposable> subs)
    {
      if (this.isPageRemoved_)
      {
        foreach (IDisposable sub in subs)
          sub.Dispose();
        subs.Clear();
      }
      else
        this.asyncDisposables.AddRange((IEnumerable<IDisposable>) subs);
    }

    private void AddSubscription(IDisposable sub)
    {
      if (this.isPageRemoved_)
        sub.Dispose();
      else
        this.asyncDisposables.Add(sub);
    }

    private void DisposeSubscription(ref IDisposable sub)
    {
      if (sub == null)
        return;
      this.asyncDisposables.Remove(sub);
      sub.SafeDispose();
      sub = (IDisposable) null;
    }

    private void InitialScroll()
    {
      if (this.isPageRemoved_)
        return;
      IDisposable sub = (IDisposable) null;
      sub = this.MessageListElement.GetLayoutUpdatedAsync().Take<Unit>(1).Subscribe<Unit>((Action<Unit>) (args =>
      {
        if (this.isPageRemoved_)
          return;
        if (this.forceInitialScrollToRecent)
          this.MessageList.ScrollToRecent("initial scroll to recent requested");
        else
          this.MessageList.ScrollToInitialPosition();
      }), (Action) (() =>
      {
        sub.SafeDispose();
        sub = (IDisposable) null;
      }));
    }

    private void ReloadMessages()
    {
      this.msgLoader_.SafeDispose();
      this.msgLoader_ = MessageLoader.Get(this.Jid, new int?(), 0);
      this.msgLoader_.MessageListener = (IMessageLoadingHandler) this;
    }

    private void SetInputMode(
      ChatPage.InputMode newMode,
      bool skipFrequentChange,
      string context = null,
      bool skipKeyboardCheck = false)
    {
      ChatPage.InputMode currentInputMode = this.CurrentInputMode;
      Log.d(this.LogHeader, "input mode {0} -> {1} | {2}", (object) currentInputMode, (object) newMode, (object) context);
      if (newMode == currentInputMode)
        return;
      DateTime now = DateTime.Now;
      if (skipFrequentChange && now - this.lastInputModeSetTime_ < TimeSpan.FromMilliseconds(500.0))
      {
        Log.d(this.LogHeader, "skip | input mode changing too often");
      }
      else
      {
        this.lastInputModeSetTime_ = now;
        this.CurrentInputMode = newMode;
        this.OnInputModeChanged(newMode, currentInputMode, skipKeyboardCheck);
      }
    }

    public Conversation GetConversation(MessagesContext db, bool createIfNotExists, string context = null)
    {
      if (this.convo_ == null && !string.IsNullOrEmpty(this.Jid))
      {
        CreateResult result;
        this.convo_ = db.GetConversation(this.Jid, createIfNotExists ? CreateOptions.CreateIfNotFound : CreateOptions.None, out result);
        if (result == CreateResult.Created)
          Log.l(this.LogHeader, "created temp conversation object for {1} | {0}", (object) (context ?? ""), (object) this.Jid);
      }
      return this.convo_;
    }

    private void DisposeAll()
    {
      try
      {
        this.inputBar.Dispose();
      }
      catch (Exception ex)
      {
        Log.SendCrashLog(ex, "dispose input bar");
      }
      try
      {
        this.titlePanel.Dispose();
      }
      catch (Exception ex)
      {
        Log.SendCrashLog(ex, "dispose title panel");
      }
      try
      {
        this.MessageList.Dispose();
      }
      catch (Exception ex)
      {
        Log.SendCrashLog(ex, "dispose msg list");
      }
      List<IDisposable> list = this.uiDisposables.ToList<IDisposable>();
      this.uiDisposables.Clear();
      list.ForEach((Action<IDisposable>) (d => d.SafeDispose()));
      List<IDisposable> toDisposeAsync = this.asyncDisposables.ToList<IDisposable>();
      this.asyncDisposables.Clear();
      toDisposeAsync.Add((IDisposable) this.msgLoader_);
      this.msgLoader_ = (MessageLoader) null;
      toDisposeAsync.Add(this.wallpaperChangedSub);
      this.wallpaperChangedSub = (IDisposable) null;
      toDisposeAsync.Add((IDisposable) this.viewModel);
      toDisposeAsync.Add((IDisposable) new DisposableAction((Action) (() => this.StopAndClearOptimisticUploads())));
      WAThreadPool.QueueUserWorkItem((Action) (() =>
      {
        toDisposeAsync.ForEach((Action<IDisposable>) (d =>
        {
          try
          {
            d.SafeDispose();
          }
          catch (Exception ex)
          {
            Log.LogException(ex, "dispose chat page");
          }
        }));
        toDisposeAsync.Clear();
      }));
    }

    private void InitTitlePanelWrapper()
    {
      ChatPageTitlePanelWrapper titlePanelWrapper = new ChatPageTitlePanelWrapper(this.viewModel);
      titlePanelWrapper.CacheMode = (CacheMode) new BitmapCache();
      titlePanelWrapper.HorizontalAlignment = HorizontalAlignment.Stretch;
      titlePanelWrapper.VerticalAlignment = VerticalAlignment.Top;
      titlePanelWrapper.RenderTransform = (Transform) (this.titlePanelTransform = new CompositeTransform());
      titlePanelWrapper.Margin = new Thickness(0.0, (UIUtils.SystemTraySizePortrait + 6.0) * ResolutionHelper.ZoomMultiplier, 0.0, 6.0 * ResolutionHelper.ZoomMultiplier);
      this.titlePanel = titlePanelWrapper;
      this.titlePanel.Init(this.initState);
      Grid.SetRow((FrameworkElement) this.titlePanel, 0);
      this.LayoutRoot.Children.Insert(0, (UIElement) this.titlePanel);
      Canvas.SetZIndex((UIElement) this.titlePanel, 32766);
    }

    private void InitTitleInfoPanel()
    {
      this.titlePanel.InitTitleInfoPanel();
      IDisposable sub = (IDisposable) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        this.convo_ = db.GetConversation(this.Jid, CreateOptions.None);
        if (this.convo_ == null)
          return;
        sub = db.UpdatedConversationSubject.Select<ConvoAndMessage, Conversation>((Func<ConvoAndMessage, Conversation>) (convoAndMsg => convoAndMsg.Conversation)).Where<Conversation>((Func<Conversation, bool>) (c => c.Jid == this.convo_.Jid)).ObserveOnDispatcher<Conversation>().Subscribe<Conversation>((Action<Conversation>) (_ => this.titlePanel.UpdateTitleInfoPanel()));
        this.uiDisposables.Add(sub);
      }));
    }

    private void InitInputBar()
    {
      ChatInputBar chatInputBar1 = new ChatInputBar(true, (GifSendingPage) this);
      chatInputBar1.VerticalAlignment = VerticalAlignment.Bottom;
      chatInputBar1.CacheMode = (CacheMode) new BitmapCache();
      ChatInputBar chatInputBar2 = chatInputBar1;
      this.inputBar = chatInputBar2;
      this.inputBarElement = (FrameworkElement) chatInputBar2;
      this.inputBar.SetOrientation(this.Orientation);
      Grid.SetRow(this.inputBarElement, 3);
      this.LayoutRoot.Children.Insert(this.LayoutRoot.Children.IndexOf((UIElement) this.titlePanel) + 1, (UIElement) this.inputBarElement);
      IDisposable disposable = (IDisposable) null;
      this.uiDisposables.Add(this.inputBar.TextBoxFocusChangedObservable().Subscribe<bool>((Action<bool>) (hasFocus =>
      {
        int num = this.inputHasFocus == hasFocus ? 0 : (this.ignoreTextInputFocusChangeOnce_ ? 1 : 0);
        this.ignoreTextInputFocusChangeOnce_ = false;
        this.inputHasFocus = hasFocus;
        if (num != 0)
          return;
        if (hasFocus)
        {
          if (this.CurrentInputMode == ChatPage.InputMode.Keyboard)
            return;
          this.SetInputMode(ChatPage.InputMode.Keyboard, false, "input box got focus");
        }
        else
        {
          if (this.CurrentInputMode != ChatPage.InputMode.Keyboard)
            return;
          this.SetInputMode(ChatPage.InputMode.None, false, "input box lost focus");
        }
      })));
      this.uiDisposables.Add(chatInputBar2.SIPStateChangedObservable().Subscribe<SIPStates>((Action<SIPStates>) (_ =>
      {
        if (!this.Orientation.IsLandscape())
          return;
        this.UpdateForCurrentInputMode();
      })));
      this.asyncDisposables.Add(this.inputBar.RecordingStateObservable().ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (isRecording =>
      {
        if (this.isPageRemoved_)
          return;
        if (isRecording)
        {
          this.IsPttRecordingInProgress = true;
          UIUtils.EnableWakeLock(true);
          UIUtils.LockOrientation((PhoneApplicationPage) this, true);
          this.GetAudioPlayback().Player.Stop(true, true);
          lock (this.composingStateLock_)
          {
            if (this.lastPresence_ != null && this.lastPresence_.State != Presence.Offline || JidHelper.IsGroupJid(this.Jid))
              this.SendRecordingVoice();
            this.composingState_ = ChatPage.ComposingState.Recording;
          }
        }
        else
        {
          this.RestorePrePttRecordingStates();
          lock (this.composingStateLock_)
          {
            this.composingState_ = ChatPage.ComposingState.None;
            this.DisposePresence();
          }
        }
      })));
      disposable = this.inputBar.RecordingResultObservable().Subscribe<WaAudioArgs>((Action<WaAudioArgs>) (args =>
      {
        args.QuotedMessage = this.inputBar.GetQuotedMessage();
        args.QuotedChat = this.inputBar.GetQuotedChat();
        this.inputBar.ClearQuote();
        args.C2cStarted = this.c2cUriPhoneNumberFlag;
        this.c2cUriPhoneNumberFlag = false;
        Log.l(this.LogHeader, "got ptt recording result");
        this.ValidateRecipientPreMessageSend().Take<bool>(1).ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (toSend =>
        {
          if (!toSend)
            return;
          Log.l(this.LogHeader, "ptt send");
          MediaUpload.SendAudio(new List<string>()
          {
            this.Jid
          }, args, true, ctx: args.AudioStreamingUploadContext);
        }));
      }));
      this.asyncDisposables.Add(chatInputBar2.ActionObservable().ObserveOnDispatcher<Pair<ChatInputBar.Actions, object>>().Subscribe<Pair<ChatInputBar.Actions, object>>((Action<Pair<ChatInputBar.Actions, object>>) (p =>
      {
        switch (p.First)
        {
          case ChatInputBar.Actions.SendText:
            this.InputBar_SendTextNotified(p.Second as ExtendedTextInputData);
            break;
          case ChatInputBar.Actions.Attach:
            this.Attach_Click((object) null, (EventArgs) null);
            break;
          case ChatInputBar.Actions.OpenKeyboard:
            this.SetInputMode(ChatPage.InputMode.Keyboard, false, "tap/hold on input box");
            break;
          case ChatInputBar.Actions.OpenEmojiKeyboard:
            this.SetInputMode(ChatPage.InputMode.Emoji, false, "open emoji keyboard");
            break;
          case ChatInputBar.Actions.CloseEmojiKeyboard:
            this.SetInputMode(ChatPage.InputMode.None, false, "close emoji keyboard");
            break;
        }
      })));
      this.asyncDisposables.Add(EmojiKeyboard.ActionSubject.ObserveOnDispatcher<Pair<EmojiKeyboard.Actions, object>>().Subscribe<Pair<EmojiKeyboard.Actions, object>>((Action<Pair<EmojiKeyboard.Actions, object>>) (p =>
      {
        if (p.First != EmojiKeyboard.Actions.ActionedSticker)
          return;
        this.SendSticker(p.Second as Sticker);
      })));
      this.asyncDisposables.Add(EmojiKeyboard.EmojiPopupManagerSizeChangedSubject.ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (_ => this.UpdateForCurrentInputMode())));
    }

    private void EnableMultiSelection(bool enable)
    {
      if (this.isInSelectionMode == enable)
        return;
      IApplicationBar applicationBar = (IApplicationBar) null;
      if (enable)
      {
        this.SetInputMode(ChatPage.InputMode.None, false, "multi-selection toggle");
        this.TryInitSelectModeAppBar();
        applicationBar = (IApplicationBar) this.selectModeBar;
        applicationBar.IsVisible = true;
      }
      else
        this.ApplicationBar.IsVisible = false;
      if (applicationBar != this.ApplicationBar)
        this.ApplicationBar = applicationBar;
      this.inputBar.Enable(!enable);
      this.MessageList.IsMultiSelectionEnabled = this.isInSelectionMode = enable;
    }

    private void RestorePrePttRecordingStates()
    {
      this.IsPttRecordingInProgress = false;
      UIUtils.EnableWakeLock(false);
      UIUtils.LockOrientation((PhoneApplicationPage) this, false);
      this.audioPlayback_?.DisposeLcd();
    }

    private void BindRootFrameTranslateY()
    {
      if (!(App.CurrentApp.OriginalRootFrameTransform is TransformGroup rootFrameTransform))
        return;
      Transform transform = rootFrameTransform.Children.FirstOrDefault<Transform>();
      if (transform == null)
        return;
      this.SetBinding(ChatPage.RootFrameTranslateYProperty, new Binding("Y")
      {
        Source = (object) transform
      });
      if (this.keyboardGapWorkaround_ == ChatPage.KeyboardGapWorkaround.None)
        return;
      this.keyboardGapWorkaround_ = ChatPage.KeyboardGapWorkaround.AdjustMargin;
      this.layoutRootBottomShift_ = -72;
    }

    private void EnsureAttachPanelPopup()
    {
      if (this.attachPanelPopup != null)
        return;
      Popup popup = new Popup();
      popup.Width = this.ActualWidth;
      popup.CacheMode = (CacheMode) new BitmapCache();
      this.attachPanelPopup = popup;
      this.attachPanelPopup.Child = (UIElement) new Border()
      {
        Background = UIUtils.PhoneChromeBrush,
        Child = (UIElement) this.AttachPanel
      };
    }

    private void OpenAttachPanel()
    {
      if (this.attachPanelPopup != null && this.attachPanelPopup.IsOpen)
        return;
      this.EnsureAttachPanelPopup();
      PopupManager popupManager = new PopupManager(this.attachPanelPopup, true);
      popupManager.OrientationChanged += new EventHandler<EventArgs>(this.AttachPanelPopupManager_OrientationChanged);
      popupManager.BackKeyHandled += (EventHandler<EventArgs>) ((sender, e) => this.SetInputMode(ChatPage.InputMode.None, false, "attach panel dismissed"));
      popupManager.Show();
    }

    private void CloseAttachPanel()
    {
      if (this.attachPanelPopup == null)
        return;
      this.attachPanelPopup.IsOpen = false;
    }

    private void OpenEmojiPicker() => this.inputBar.OpenEmojiKeyboard();

    private void CloseEmojiPicker() => this.inputBar.CloseEmojiKeyboard(SIPStates.Undefined);

    private void OpenKeyboard() => this.inputBar.OpenKeyboard();

    private void CloseKeyboard() => this.Focus();

    private bool IsDelayOpenInputNeeded(
      ChatPage.InputMode newMode,
      ChatPage.InputMode oldMode,
      bool inLandscape)
    {
      if (!inLandscape)
        return false;
      if (newMode == ChatPage.InputMode.Emoji && oldMode == ChatPage.InputMode.Keyboard)
        return true;
      return oldMode == ChatPage.InputMode.Emoji && newMode == ChatPage.InputMode.Keyboard;
    }

    private bool IsHidingPttNeeded(ChatPage.InputMode mode)
    {
      bool flag = false;
      switch (mode)
      {
        case ChatPage.InputMode.Keyboard:
        case ChatPage.InputMode.Emoji:
          flag = true;
          break;
        case ChatPage.InputMode.Attachment:
          flag = false;
          break;
      }
      return flag;
    }

    private void LaunchPicturePreview(MediaSharingState sharingState)
    {
      this.StartOptimisticUploading(sharingState);
      bool isGif = false;
      if (sharingState is GifSharingState)
        isGif = true;
      PicturePreviewPage.Start(sharingState, isGif).ObserveOnDispatcher<MediaSharingArgs>().Subscribe<MediaSharingArgs>((Action<MediaSharingArgs>) (args =>
      {
        if (args.Status == MediaSharingArgs.SharingStatus.Canceled)
        {
          CameraPage.TakePictureOnly = false;
          MediaPickerPage.preChosenChild = (MediaPickerState.Item) null;
          if (args.NavTransition != null)
            args.NavTransition((Action) (() => NavUtils.NavigateBackToChat(args.NavService)));
          else
            NavUtils.NavigateBackToChat(args.NavService);
          if (isGif)
          {
            ChatPage.NextInstanceInitState = new ChatPage.InitState()
            {
              InputMode = ChatPage.InputMode.Emoji
            };
            ChatPage.SearchTermForNextEntrance = (sharingState as GifSharingState).SearchTerm;
          }
          this.StopAndClearOptimisticUploads();
          args.SharingState.SafeDispose();
        }
        else if (args.Status == MediaSharingArgs.SharingStatus.Submitted)
        {
          Message quotedMsg = this.inputBar.GetQuotedMessage();
          string quotedChat = this.inputBar.GetQuotedChat();
          this.inputBar.ClearQuote();
          bool c2cStarted = this.c2cUriPhoneNumberFlag;
          this.c2cUriPhoneNumberFlag = false;
          CameraPage.TakePictureOnly = false;
          int imageMaxEdge = Settings.ImageMaxEdge;
          List<MediaSharingState.PicInfo> picInfos = new List<MediaSharingState.PicInfo>();
          List<WaVideoArgs> videoInfo = new List<WaVideoArgs>();
          foreach (MediaSharingState.IItem selectedItem in args.SharingState.SelectedItems)
          {
            if (selectedItem.GetMediaType() == FunXMPP.FMessage.Type.Image)
            {
              MediaSharingState.PicInfo picInfo = selectedItem.ToPicInfo(new System.Windows.Size((double) imageMaxEdge, (double) imageMaxEdge));
              picInfos.Add(picInfo);
            }
            else
            {
              if (selectedItem.GetMediaType() == FunXMPP.FMessage.Type.Gif && selectedItem.GifInfo != null)
              {
                Stream stream = (Stream) null;
                if (selectedItem is GifSharingState.Item)
                {
                  stream = ((GifSharingState.Item) selectedItem).GetGifStream();
                }
                else
                {
                  using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
                    stream = nativeMediaStorage.OpenFile(selectedItem.GetFullPath(), FileMode.Open, FileAccess.Read);
                }
                WriteableBitmap bitmap1 = selectedItem.GetBitmap(new System.Windows.Size((double) imageMaxEdge, (double) imageMaxEdge));
                WaVideoArgs waVideoArgs = new WaVideoArgs()
                {
                  FileExtension = ".mp4",
                  ContentType = "video/mp4",
                  FullPath = selectedItem.GetFullPath(),
                  Stream = stream,
                  LargeThumbnail = bitmap1,
                  Thumbnail = bitmap1,
                  OrientationAngle = selectedItem.RotatedTimes * -90,
                  LoopingPlayback = true,
                  TimeCrop = selectedItem.GifInfo.TimeCrop,
                  GifAttribution = selectedItem.GifInfo.GifAttribution,
                  Caption = selectedItem.Caption
                };
                if (selectedItem.GifInfo.GifAttribution <= MessageProperties.MediaProperties.Attribution.NONE)
                {
                  waVideoArgs.FileExtension = ".gif";
                  waVideoArgs.ContentType = "image/gif";
                  waVideoArgs.CodecInfo = CodecInfo.NeedsTranscode;
                  waVideoArgs.TranscodeReason = TranscodeReason.BadCodec;
                }
                new GifFromProviderSent()
                {
                  gifSearchProvider = new wam_enum_gif_search_provider?(GifProviders.Instance.GetProviderForFieldStats())
                }.SaveEvent();
                if (selectedItem.RelativeCropPos.HasValue)
                {
                  WriteableBitmap bitmap2 = selectedItem.GetBitmap(new System.Windows.Size((double) imageMaxEdge, (double) imageMaxEdge), false, false);
                  CropRectangle cropRectangle1 = new CropRectangle();
                  ref CropRectangle local1 = ref cropRectangle1;
                  double pixelHeight = (double) bitmap2.PixelHeight;
                  System.Windows.Size? relativeCropSize = selectedItem.RelativeCropSize;
                  double height = relativeCropSize.Value.Height;
                  int num1 = (int) (pixelHeight * height);
                  local1.Height = num1;
                  ref CropRectangle local2 = ref cropRectangle1;
                  double pixelWidth = (double) bitmap2.PixelWidth;
                  relativeCropSize = selectedItem.RelativeCropSize;
                  double width = relativeCropSize.Value.Width;
                  int num2 = (int) (pixelWidth * width);
                  local2.Width = num2;
                  ref CropRectangle local3 = ref cropRectangle1;
                  System.Windows.Point? relativeCropPos = selectedItem.RelativeCropPos;
                  System.Windows.Point point = relativeCropPos.Value;
                  int num3 = (int) (point.X * (double) bitmap2.PixelWidth);
                  local3.XOffset = num3;
                  ref CropRectangle local4 = ref cropRectangle1;
                  relativeCropPos = selectedItem.RelativeCropPos;
                  point = relativeCropPos.Value;
                  int num4 = (int) (point.Y * (double) bitmap2.PixelHeight);
                  local4.YOffset = num4;
                  CropRectangle cropRectangle2 = cropRectangle1;
                  waVideoArgs.CropRectangle = new CropRectangle?(cropRectangle2);
                }
                selectedItem.VideoInfo = waVideoArgs;
              }
              videoInfo.Add(selectedItem.VideoInfo);
            }
          }
          if (args.NavTransition != null)
          {
            Log.l(this.LogHeader, "back to chat after image sending - 0");
            args.NavTransition((Action) (() => NavUtils.NavigateBackToChat(args.NavService)));
          }
          else
          {
            Log.l(this.LogHeader, "back to chat after image sending - 1");
            NavUtils.NavigateBackToChat(args.NavService);
          }
          MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
          {
            if (picInfos.Count <= 1)
              return;
            int count = picInfos.Count - 1;
            IDisposable sub = (IDisposable) null;
            sub = db.NewMessagesSubject.Where<Message>((Func<Message, bool>) (m => m.KeyRemoteJid == this.Jid)).Take<Message>(1).ObserveOnDispatcher<Message>().Subscribe<Message>((Action<Message>) (_ =>
            {
              this.DisposeSubscription(ref sub);
              if (this.msgLoader_ == null)
                return;
              this.msgLoader_.IncreaseDelayedNewMessageCount(count);
            }));
            this.AddSubscription(sub);
          }));
          int thumbnailWidth = MessageViewModel.LargeThumbPixelWidth;
          Dictionary<string, OptimisticUpload> optimisticUploads = this.StopAndRetrieveOptimisticUploads();
          WAThreadPool.QueueUserWorkItem((Action) (() =>
          {
            int count3 = picInfos.Count;
            for (int index = 0; index < count3; ++index)
            {
              this.RefreshDebugInfo(string.Format("chat page: processing outgoing image {0}/{1}", (object) (index + 1), (object) count3));
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
                  string optimisticUniqueId = pi.GetOptimisticUniqueId();
                  if (args.SharingState.Mode == MediaSharingState.SharingMode.TakePicture)
                  {
                    MediaDownload.SaveMediaToCameraRoll(pi.PathForDb, FunXMPP.FMessage.Type.Image, saveAlbum: "Camera Roll");
                    pi.PathForDb = (string) null;
                  }
                  Message msgQuoted = quotedMsg;
                  string chatQuoted = quotedChat;
                  Action<Message> messageModifier = (Action<Message>) (msg => msg.SetQuote(msgQuoted, chatQuoted));
                  if (c2cStarted)
                  {
                    Action<Message> prevMessageModifier = messageModifier;
                    messageModifier = (Action<Message>) (msg =>
                    {
                      Action<Message> action = prevMessageModifier;
                      if (action != null)
                        action(msg);
                      msg.SetC2cFlags(true);
                    });
                  }
                  OptimisticUpload optimisticUpload = (OptimisticUpload) null;
                  if (!pi.isChangedByUser() && optimisticUploads != null && optimisticUploads.TryGetValue(optimisticUniqueId, out optimisticUpload) && !optimisticUpload.SetUseFileOnServer())
                    optimisticUpload = (OptimisticUpload) null;
                  if (optimisticUpload != null && optimisticUpload.OptimisticMessage != null)
                  {
                    Action<Message> prevMessageModifier = messageModifier;
                    Message optimisticMsg = optimisticUpload.OptimisticMessage;
                    messageModifier = (Action<Message>) (msg =>
                    {
                      Action<Message> action = prevMessageModifier;
                      if (action != null)
                        action(msg);
                      msg.UploadContext = optimisticMsg.UploadContext;
                      msg.MediaKey = optimisticMsg.MediaKey;
                      msg.MediaUploadUrl = optimisticMsg.MediaUploadUrl;
                      msg.MediaHash = optimisticMsg.MediaHash;
                    });
                  }
                  MediaUpload.SendPicture(new List<string>()
                  {
                    this.Jid
                  }, jpegStream, thumbnailWidth, picture, pi.PathForDb, pi.Caption, messageModifier);
                }
              }
              picInfos[index] = pi = (MediaSharingState.PicInfo) null;
              this.RefreshDebugInfo(string.Format("chat page: processed outgoing image {0}/{1}", (object) (index + 1), (object) count3), false);
            }
            int count4 = videoInfo.Count;
            for (int index = 0; index < count4; ++index)
            {
              this.RefreshDebugInfo(string.Format("chat page: processing outgoing video {0}/{1}", (object) (index + 1), (object) count4));
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
              int mode = (int) args.SharingState.Mode;
              WaVideoArgs args1 = videoInfo[index];
              args1.QuotedMessage = quotedMsg;
              args1.QuotedChat = quotedChat;
              args1.C2cStarted = c2cStarted;
              this.SendVideo(args1);
              this.RefreshDebugInfo(string.Format("chat page: processed outgoing video {0}/{1}", (object) (index + 1), (object) count4), false);
            }
            args.SharingState.SafeDispose();
            if (optimisticUploads == null)
              return;
            foreach (IDisposable d in optimisticUploads.Values)
              d.SafeDispose();
          }));
        }
        else if (args.SharingState.Mode == MediaSharingState.SharingMode.TakePicture)
        {
          TakePicture.Launch(TakePicture.Mode.Regular, true).Take<TakePicture.CapturedPictureArgs>(1).ObserveOnDispatcher<TakePicture.CapturedPictureArgs>().Subscribe<TakePicture.CapturedPictureArgs>((Action<TakePicture.CapturedPictureArgs>) (camArgs =>
          {
            if (args != null && camArgs.CameraRollItem != null)
              args.SharingState.AddItem((MediaSharingState.IItem) camArgs.CameraRollItem);
            else if (camArgs != null && camArgs.Bitmap != null)
              args.SharingState.AddItem((MediaSharingState.IItem) new CameraSharingState.Item(camArgs.TempFileStream, camArgs.Bitmap, camArgs.ZoomScale, wam_enum_media_picker_origin_type.CHAT_BUTTON_CAMERA_CAPTURE));
            else if (camArgs != null && camArgs.VideoArgs != null)
              args.SharingState.AddItem((MediaSharingState.IItem) new CameraSharingState.Item(camArgs.VideoArgs, camArgs.ZoomScale, wam_enum_media_picker_origin_type.CHAT_BUTTON_CAMERA_CAPTURE));
            NavUtils.GoBack(args.NavService);
          }));
        }
        else
        {
          if (args.SharingState.SelectedItems.Count<MediaSharingState.IItem>() > 0 && MediaPickerPage.preChosenChild != null)
            this.LaunchAlbumsPicker(MediaSharingState.SharingMode.ChooseMedia);
          if (args.NavTransition != null)
            args.NavTransition((Action) (() => NavUtils.GoBack(args.NavService)));
          else
            NavUtils.GoBack(args.NavService);
        }
      }));
    }

    private void LaunchAlbumsPicker(MediaSharingState.SharingMode mode, MediaPickerState state = null)
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
          this.LaunchPicturePreview(args.SharingState);
        }
      }));
    }

    private void LaunchCamera()
    {
      int imageMaxEdge = Settings.ImageMaxEdge;
      TakePicture.Launch(TakePicture.Mode.Regular, true, imageMaxEdge, imageMaxEdge).Take<TakePicture.CapturedPictureArgs>(1).ObserveOnDispatcher<TakePicture.CapturedPictureArgs>().Subscribe<TakePicture.CapturedPictureArgs>((Action<TakePicture.CapturedPictureArgs>) (args =>
      {
        if (args == null)
        {
          MediaPickerPage.preChosenChild = (MediaPickerState.Item) null;
          this.LaunchAlbumsPicker(MediaSharingState.SharingMode.ChooseMedia);
        }
        if (args != null && args.CameraRollItem != null)
        {
          MediaPickerState sharingState = new MediaPickerState(true, MediaSharingState.SharingMode.ChooseMedia);
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
          MediaPickerPage.preChosenChild = args.CameraRollItem;
          sharingState.AddItem((MediaSharingState.IItem) args.CameraRollItem);
          this.LaunchPicturePreview((MediaSharingState) sharingState);
        }
        else if (args != null && args.Bitmap != null)
        {
          MediaSharingState sharingState = (MediaSharingState) new CameraSharingState();
          sharingState.AddItem((MediaSharingState.IItem) new CameraSharingState.Item(args.TempFileStream, args.Bitmap, args.ZoomScale, wam_enum_media_picker_origin_type.CHAT_BUTTON_CAMERA_CAPTURE));
          this.LaunchPicturePreview(sharingState);
        }
        else
        {
          if (args == null || args.VideoArgs == null)
            return;
          MediaSharingState sharingState = (MediaSharingState) new CameraSharingState();
          sharingState.AddItem((MediaSharingState.IItem) new CameraSharingState.Item(args.VideoArgs, args.ZoomScale, wam_enum_media_picker_origin_type.CHAT_BUTTON_CAMERA_CAPTURE));
          this.LaunchPicturePreview(sharingState);
        }
      }));
    }

    private void LaunchLocationShare()
    {
      IDisposable successfulShareSubscription = (IDisposable) null;
      successfulShareSubscription = ShareLocationPage.Start(this.NavigationService, this.Jid, this.inputBar.GetQuotedMessage(), this.inputBar.GetQuotedChat(), this.c2cUriPhoneNumberFlag).ObserveOnDispatcher<Unit>().Subscribe<Unit>((Action<Unit>) (_ =>
      {
        this.inputBar.ClearQuote();
        this.c2cUriPhoneNumberFlag = false;
        successfulShareSubscription.SafeDispose();
        successfulShareSubscription = (IDisposable) null;
      }), (Action) (() =>
      {
        successfulShareSubscription.SafeDispose();
        successfulShareSubscription = (IDisposable) null;
      }));
    }

    private void LaunchContactPicker()
    {
      RecipientListPage.StartPhoneContactPicker(AppResources.ShareContact, (Brush) new SolidColorBrush(Color.FromArgb(byte.MaxValue, (byte) 58, (byte) 117, (byte) 242)), AppResources.NContactsPlural).ObserveOnDispatcher<RecipientListPage.RecipientListResults>().Subscribe<RecipientListPage.RecipientListResults>((Action<RecipientListPage.RecipientListResults>) (recipientListResults =>
      {
        List<Contact> selectedContacts = recipientListResults?.SelectedContacts;
        if (selectedContacts == null || !selectedContacts.Any<Contact>())
          return;
        List<ContactVCard> cards = new List<ContactVCard>();
        ContactVCard[] array = selectedContacts.Select<Contact, ContactVCard>((Func<Contact, ContactVCard>) (c => ContactVCard.Create(c))).ToArray<ContactVCard>();
        if (array == null)
          return;
        ShareContactPage.Start((IEnumerable<ContactVCard>) array).Where<ContactVCard>((Func<ContactVCard, bool>) (vCard => vCard != null)).ObserveOnDispatcher<ContactVCard>().Subscribe<ContactVCard>((Action<ContactVCard>) (vCard => cards.Add(vCard)), (Action) (() =>
        {
          Message quotedMsg = this.inputBar.GetQuotedMessage();
          string quotedChat = this.inputBar.GetQuotedChat();
          this.inputBar.ClearQuote();
          bool c2cStarted = this.c2cUriPhoneNumberFlag;
          this.c2cUriPhoneNumberFlag = false;
          if (cards.Count<ContactVCard>() > 0)
            MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
            {
              Message message;
              if (cards.Count<ContactVCard>() > 1)
              {
                message = cards.ToMessage(db);
                message.MediaName = Plurals.Instance.GetString(AppResources.NumContactsInBulkCardPlural, cards.Count<ContactVCard>());
              }
              else
                message = cards.FirstOrDefault<ContactVCard>().ToMessage(db);
              message.KeyRemoteJid = this.Jid;
              message.SetQuote(quotedMsg, quotedChat);
              message.SetC2cFlags(c2cStarted);
              db.InsertMessageOnSubmit(message);
              db.SubmitChanges();
            }));
          this.NavigationService.JumpBackTo(nameof (ChatPage));
        }));
      }));
    }

    private void LaunchVideoRecorder()
    {
      VideoRecorderPage.Start(this.NavigationService).Subscribe<WaVideoArgs>((Action<WaVideoArgs>) (args =>
      {
        MediaSharingState sharingState = (MediaSharingState) new CameraSharingState();
        sharingState.Mode = MediaSharingState.SharingMode.TakeVideo;
        sharingState.AddItem((MediaSharingState.IItem) new CameraSharingState.Item(args, new System.Windows.Point(1.0, 1.0), wam_enum_media_picker_origin_type.CHAT_BUTTON_CAMERA_CAPTURE));
        this.LaunchPicturePreview(sharingState);
      }));
    }

    private void SendVideo(WaVideoArgs args)
    {
      if (args == null)
        return;
      this.progressIndicator.Acquire();
      Log.l(this.LogHeader, "video send");
      try
      {
        MediaUpload.SendVideo(new List<string>()
        {
          this.Jid
        }, args, (Action) (() => this.Dispatcher.BeginInvoke((Action) (() => this.progressIndicator.Release()))));
      }
      catch (Exception ex)
      {
        this.Dispatcher.BeginInvoke((Action) (() =>
        {
          this.progressIndicator.Release();
          int num = (int) MessageBox.Show(AppResources.VideoCaptureFailedtoSend);
        }));
        Log.SendCrashLog(ex, "Send video");
      }
    }

    public void AddJidToContactsUseVCards(string jid, Action onFinished = null)
    {
      Message[] vCardMessages = new Message[0];
      MessagesContext.Run((MessagesContext.MessagesCallback) (db => vCardMessages = ((IEnumerable<Message>) db.GetTrustedContactVCardsWithJid(jid)).ToArray<Message>()));
      if (vCardMessages.Length != 0)
      {
        ContactVCard vcard = (ContactVCard) null;
        foreach (Message msg in vCardMessages)
        {
          foreach (ContactVCard otherCard in (IEnumerable<ContactVCard>) ((object) msg.GetContactCards() ?? (object) new ContactVCard[0]))
          {
            if (otherCard.PhoneNumbers != null && ((IEnumerable<ContactVCard.PhoneNumber>) otherCard.PhoneNumbers).Where<ContactVCard.PhoneNumber>((Func<ContactVCard.PhoneNumber, bool>) (p => p.Jid == jid)).Any<ContactVCard.PhoneNumber>())
            {
              if (vcard != null)
                vcard.Merge(otherCard);
              else
                vcard = otherCard;
            }
          }
        }
        vcard.ToSaveContactTask().GetShowTaskAsync<SaveContactResult>().Subscribe<IEvent<SaveContactResult>>((Action<IEvent<SaveContactResult>>) (e =>
        {
          if (e.EventArgs.TaskResult != TaskResult.OK)
            return;
          Action action = onFinished;
          if (action == null)
            return;
          action();
        }));
      }
      else
        this.AddJidToContacts(jid, onFinished: onFinished);
    }

    public void AddJidToContacts(string jid, bool toExisting = false, Action onFinished = null)
    {
      AddContact.Launch(jid, toExisting, new Action(this.progressIndicator.Acquire), new Action(this.progressIndicator.Release), (Action) (() =>
      {
        switch (JidHelper.GetJidType(this.Jid))
        {
          case JidHelper.JidTypes.User:
            this.ResetPageMenu();
            this.titlePanel.UpdateTitle();
            break;
          case JidHelper.JidTypes.Group:
            this.RefreshMessagesOnContactChanged(jid);
            break;
        }
        if (onFinished == null)
          return;
        onFinished();
      }));
    }

    private void ToggleBlock(string reportSpamSource = "block_dialog", Conversation convo = null, Message[] msgsToReport = null)
    {
      this.IsEnabled = false;
      this.progressIndicator.Acquire();
      string jidToBlock = this.Jid;
      bool exitedPage = false;
      BlockContact.ToggleBlock(jidToBlock, reportSpamSource).ObserveOnDispatcher<Pair<bool, bool>>().Subscribe<Pair<bool, bool>>((Action<Pair<bool, bool>>) (blockSelection => exitedPage = BlockContact.OnBlockDialogSelection(blockSelection, reportSpamSource, jidToBlock, msgsToReport)), (Action) (() =>
      {
        if (exitedPage)
          return;
        this.IsEnabled = true;
        this.progressIndicator.Release();
      }));
    }

    private void SubscribeToBlockListChange()
    {
      if (this.blockListChangedSub != null)
        return;
      this.blockListChangedSub = BlockContact.BlockListChangedSubject.ObserveOnDispatcher<Unit>().Subscribe<Unit>((Action<Unit>) (_ =>
      {
        this.ResetPageMenu();
        if (this.blockToggleButton == null)
          return;
        bool isBlocked = false;
        ContactsContext.Instance((Action<ContactsContext>) (cdb => isBlocked = this.Jid != null && cdb.BlockListSet.ContainsKey(this.Jid)));
        this.blockToggleButton.Content = isBlocked ? (object) AppResources.UnblockContact : (object) AppResources.Block;
      }));
    }

    private IObservable<bool> ValidateRecipientPreMessageSend()
    {
      this.SubscribeToBlockListChange();
      return SendMessage.ValidateRecipientPreSending(this.Jid);
    }

    private void SetWallpaper(WallpaperStore.WallpaperState wallpaper, bool init)
    {
      if (this.IsSuspicious)
        return;
      bool flag = true;
      this.viewModel.Wallpaper = wallpaper;
      if (this.viewModel.Wallpaper == null)
      {
        if (init)
        {
          flag = false;
        }
        else
        {
          if (this.wallpaperPanel != null)
            this.wallpaperPanel.Visibility = Visibility.Collapsed;
          this.titlePanel.UpdateTitleForeground();
          SysTrayHelper.SetForegroundColor((DependencyObject) this, this.viewModel.SysTrayForegroundColor);
        }
      }
      else
      {
        if (this.wallpaperPanel == null)
        {
          this.wallpaperPanel = ChatPageHelper.CreateWallpaperPanel();
          Grid.SetRow((FrameworkElement) this.wallpaperPanel, 0);
          Grid.SetRowSpan((FrameworkElement) this.wallpaperPanel, 4);
          this.LayoutRoot.Children.Insert(0, (UIElement) this.wallpaperPanel);
        }
        this.wallpaperPanel.Set(this.viewModel.Wallpaper);
        this.wallpaperPanel.Visibility = Visibility.Visible;
        this.titlePanel.UpdateTitleForeground();
        SysTrayHelper.SetForegroundColor((DependencyObject) this, this.viewModel.SysTrayForegroundColor);
      }
      if (flag)
        this.RefreshMessagesOnBackgroundChanged();
      if (this.wallpaperChangedSub != null)
        return;
      this.wallpaperChangedSub = WallpaperStore.WallpaperChangedSubject.Where<string>((Func<string, bool>) (jid => jid == this.Jid)).Subscribe<string>((Action<string>) (jid => this.isWallpaperDirty = true));
    }

    private void RefreshMessagesOnBackgroundChanged()
    {
      this.MessageList.RefreshMessages((Action<MessageViewModel>) (vm => vm.RefreshOnChatBackgroundChanged()));
    }

    private void RefreshMessagesOnContactChanged(string targetJid = null)
    {
      this.MessageList.RefreshMessages((Action<MessageViewModel>) (vm => vm.RefreshOnContactChanged(targetJid)));
    }

    private void RefreshMessagesOnSuspicionChanged()
    {
      bool allowLinks = !this.IsSuspicious;
      this.MessageList.RefreshMessages((Action<MessageViewModel>) (vm => vm.AllowLinks = allowLinks));
    }

    private void UpdateRootFrameTransform(ChatPage.InputMode mode, bool isLandscape)
    {
      double num = 0.0;
      bool flag = false;
      if (!isLandscape)
      {
        switch (mode)
        {
          case ChatPage.InputMode.Keyboard:
            if (this.keyboardGapWorkaround_ == ChatPage.KeyboardGapWorkaround.TranslateRootFrame)
            {
              num = -UIUtils.SIPHeightPortrait;
              flag = true;
              break;
            }
            break;
          case ChatPage.InputMode.Emoji:
            num = -this.inputBar.GetSIPHeight();
            flag = true;
            break;
        }
      }
      TransitionFrame rootFrame = App.CurrentApp.RootFrame;
      if (flag)
      {
        TranslateTransform transformPortrait = this.shiftedRootFrameTransformPortrait_;
        transformPortrait.Y = this.rootFrameShift_ = num;
        rootFrame.RenderTransform = (Transform) transformPortrait;
      }
      else
      {
        this.rootFrameShift_ = 0.0;
        rootFrame.RenderTransform = App.CurrentApp.OriginalRootFrameTransform;
      }
    }

    private double MessageListShrinkFromSIP(double sipOffset)
    {
      return sipOffset - (!this.viewModel.IsSIPUp || this.Orientation.IsLandscape() ? 0.0 : this.titlePanel.ActualHeight);
    }

    private void UpdateForSIPOffset(double sipOffset)
    {
      if (this.wallpaperPanel != null)
        (this.wallpaperPanel.RenderTransform as TranslateTransform).Y = sipOffset;
      this.titlePanelTransform.TranslateY = sipOffset;
      this.MessageListShrinkFromSIP(sipOffset);
      double val1 = sipOffset;
      if (this.MessageListElement.ActualHeight <= 0.0 || this.MessageListElement.ActualWidth <= 0.0)
        return;
      if (!(this.MessageListElement.Clip is RectangleGeometry rectangleGeometry))
      {
        rectangleGeometry = new RectangleGeometry();
        this.MessageListElement.Clip = (Geometry) rectangleGeometry;
      }
      System.Windows.Rect rect = new System.Windows.Rect(0.0, Math.Max(val1, 0.0), this.ActualWidth, this.ActualHeight);
      rectangleGeometry.Rect = rect;
    }

    private void UpdateForInputModeChanged(ChatPage.InputMode oldMode, ChatPage.InputMode newMode)
    {
      bool isLandscape = this.Orientation.IsLandscape();
      this.UpdateRootFrameTransform(newMode, isLandscape);
      if (this.keyboardGapWorkaround_ == ChatPage.KeyboardGapWorkaround.AdjustMargin && newMode == ChatPage.InputMode.Keyboard && !isLandscape)
        this.LayoutRoot.Margin = new Thickness(0.0, 0.0, 0.0, (double) this.layoutRootBottomShift_);
      else
        this.LayoutRoot.Margin = new Thickness(0.0);
      switch (newMode)
      {
        case ChatPage.InputMode.Emoji:
          this.UpdateForSIPOffset(-this.rootFrameShift_);
          break;
        case ChatPage.InputMode.Attachment:
          this.UpdateForSIPOffset(-this.rootFrameShift_);
          break;
        default:
          if (oldMode != ChatPage.InputMode.Keyboard || newMode != ChatPage.InputMode.None)
          {
            this.UpdateForSIPOffset(0.0);
            break;
          }
          break;
      }
      this.UpdateElementsForInputModeChange(newMode);
    }

    private void UpdateElementsForInputModeChange(ChatPage.InputMode newMode)
    {
      bool flag1 = this.Orientation.IsLandscape();
      double zoomMultiplier = ResolutionHelper.ZoomMultiplier;
      double left = 0.0;
      double top = 0.0;
      double right = 0.0;
      double num = 0.0;
      switch (this.Orientation)
      {
        case PageOrientation.LandscapeLeft:
          left = UIUtils.SystemTraySizeLandscape;
          if (newMode == ChatPage.InputMode.Emoji)
          {
            num = UIUtils.SIPHeightLandscape;
            break;
          }
          break;
        case PageOrientation.LandscapeRight:
          right = UIUtils.SystemTraySizeLandscape;
          if (newMode == ChatPage.InputMode.Emoji)
          {
            num = UIUtils.SIPHeightLandscape;
            break;
          }
          break;
        default:
          top = (UIUtils.SystemTraySizePortrait + 6.0) * zoomMultiplier;
          break;
      }
      bool flag2 = this.viewModel != null ? this.viewModel.IsSIPUp : newMode == ChatPage.InputMode.Keyboard || newMode == ChatPage.InputMode.Emoji;
      this.titlePanel.Visibility = (!(flag1 & flag2)).ToVisibility();
      this.titlePanel.Margin = new Thickness(0.0, top, 0.0, 6.0 * zoomMultiplier);
      this.titlePanel.SetTitlePanelMargin(new Thickness(Math.Floor(left + UIUtils.PageSideMargin), 0.0, Math.Floor(right + UIUtils.PageSideMargin), 0.0));
      if (this.spamReportPanel != null)
        this.spamReportPanel.Margin = new Thickness(left + UIUtils.PageSideMargin, 6.0, right + UIUtils.PageSideMargin, 6.0);
      this.MessageListElement.Margin = new Thickness(left, this.MessageListElement.Margin.Top, right, 0.0);
      this.inputBarElement.Margin = new Thickness(0.0, 0.0, 0.0, num - 1.0);
      this.ReadOnlyHelpTextBox.Margin = new Thickness(24.0 + left, this.convo_.IsPsaChat() ? 18.0 : 12.0, 24.0 + right, this.convo_.IsPsaChat() ? 18.0 : 12.0);
      double padding = 0.0;
      if (flag2)
        padding = this.MessageListShrinkFromSIP(flag1 ? UIUtils.SIPHeightLandscape : UIUtils.SIPHeightPortrait);
      this.MessageList.SetTopPadding(padding);
    }

    private void TryShowContactVCardPanel(bool show, bool remove = false)
    {
      if (this.contactVCardPanel == null)
        return;
      this.contactVCardPanel.Visibility = show.ToVisibility();
      if (!remove)
        return;
      this.LayoutRoot.Children.Remove((UIElement) this.contactVCardPanel);
      this.contactVCardPanel = (Grid) null;
    }

    private void TryShowSpamReportingPanel(bool show, bool remove = false)
    {
      if (this.spamReportPanel == null)
        return;
      this.spamReportPanel.Visibility = show.ToVisibility();
      if (!(!show & remove))
        return;
      this.LayoutRoot.Children.Remove((UIElement) this.spamReportPanel);
      this.spamReportPanel = (Grid) null;
    }

    private void InitContactVCardPanel()
    {
      if (this.contactVCardPanel != null)
        return;
      string jid = this.Jid;
      if (JidHelper.IsBroadcastJid(jid) || JidHelper.IsGroupJid(jid) || JidHelper.IsJidInAddressBook(jid))
        return;
      bool shouldShow = true;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        JidInfo jidInfo = db.GetJidInfo(jid, CreateOptions.None);
        shouldShow = jidInfo != null && jidInfo.ShouldPromptVCard();
      }));
      if (!shouldShow)
        return;
      int numVcards = 0;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db => numVcards = ((IEnumerable<Message>) db.GetTrustedContactVCardsWithJid(this.Jid)).Count<Message>()));
      if (numVcards <= 0)
        return;
      if (this.spamReportPanel != null)
        this.TryShowSpamReportingPanel(false, true);
      double pageSideMargin = UIUtils.PageSideMargin;
      Grid grid = new Grid();
      grid.CacheMode = (CacheMode) new BitmapCache();
      grid.Margin = new Thickness(pageSideMargin, 6.0, pageSideMargin, 6.0);
      grid.Background = (Brush) UIUtils.BackgroundBrush;
      Grid element1 = grid;
      element1.RowDefinitions.Add(new RowDefinition()
      {
        Height = GridLength.Auto
      });
      element1.RowDefinitions.Add(new RowDefinition()
      {
        Height = GridLength.Auto
      });
      element1.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(0.5, GridUnitType.Star)
      });
      element1.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(0.5, GridUnitType.Star)
      });
      TextBlock textBlock = new TextBlock();
      textBlock.Style = Application.Current.Resources[(object) "PhoneTextSubtleStyle"] as Style;
      textBlock.Margin = new Thickness(0.0);
      textBlock.TextWrapping = TextWrapping.Wrap;
      TextBlock element2 = textBlock;
      Grid.SetRow((FrameworkElement) element2, 0);
      Grid.SetColumn((FrameworkElement) element2, 0);
      Grid.SetColumnSpan((FrameworkElement) element2, 2);
      element1.Children.Add((UIElement) element2);
      Button button1 = new Button();
      button1.Padding = new Thickness(0.0, 6.0, 0.0, 6.0);
      button1.Margin = new Thickness(-12.0, 0.0, pageSideMargin / 2.0 - 12.0, 0.0);
      Button element3 = button1;
      if (numVcards == 1)
      {
        element3.Content = (object) AppResources.AddToContactsButton;
        element3.Click += new RoutedEventHandler(this.AddToContacts_Click);
        element2.Text = AppResources.AddVCardPanelTooltip;
      }
      else
      {
        element3.Click += new RoutedEventHandler(this.ViewVCards_Click);
        element3.Content = (object) AppResources.ViewBulkContactCard;
        element2.Text = Plurals.Instance.GetString(AppResources.ViewVCardPanelTooltipPlural, numVcards);
      }
      Grid.SetRow((FrameworkElement) element3, 1);
      Grid.SetColumn((FrameworkElement) element3, 0);
      element1.Children.Add((UIElement) element3);
      Button button2 = new Button();
      button2.Padding = new Thickness(0.0, 6.0, 0.0, 6.0);
      button2.Margin = new Thickness(pageSideMargin / 2.0 - 12.0, 0.0, -12.0, 0.0);
      button2.Content = (object) AppResources.SkipButton;
      Button element4 = button2;
      element4.Click += new RoutedEventHandler(this.ViewVCardsDismiss_Click);
      Grid.SetRow((FrameworkElement) element4, 1);
      Grid.SetColumn((FrameworkElement) element4, 1);
      element1.Children.Add((UIElement) element4);
      Grid.SetRow((FrameworkElement) element1, 2);
      this.contactVCardPanel = element1;
      this.LayoutRoot.Children.Insert(3, (UIElement) element1);
    }

    private void InitChangeNumberNotificationPanel(string oldJid, string newJid, Action onDimiss = null)
    {
      if (this.numberChangePanel != null)
        return;
      string jid = this.Jid;
      if (!JidHelper.IsUserJid(jid))
        return;
      bool isOldChat = jid == oldJid;
      double pageSideMargin = UIUtils.PageSideMargin;
      StackPanel stackPanel = new StackPanel();
      stackPanel.CacheMode = (CacheMode) new BitmapCache();
      stackPanel.Margin = new Thickness(0.0, -2.0, 0.0, 0.0);
      stackPanel.VerticalAlignment = VerticalAlignment.Top;
      stackPanel.Background = (Brush) UIUtils.BackgroundBrush;
      StackPanel panel = stackPanel;
      string name = (string) null;
      ContactsContext.Instance((Action<ContactsContext>) (db =>
      {
        UserStatus userStatus = db.GetUserStatus(oldJid, false);
        if (userStatus != null)
          name = userStatus.GetDisplayName();
        else
          name = JidHelper.GetPhoneNumber(oldJid, true);
      }));
      string str = string.Format(AppResources.ChangeNumberSystemMessage, (object) name);
      if (!isOldChat)
        str = string.Format("{0} {1}", (object) str, (object) AppResources.ChangeNumberCurrentlyChatting);
      TextBlock textBlock1 = new TextBlock();
      textBlock1.Margin = new Thickness(pageSideMargin, 12.0, pageSideMargin, 8.0);
      textBlock1.TextWrapping = TextWrapping.Wrap;
      textBlock1.Text = str;
      TextBlock textBlock2 = textBlock1;
      panel.Children.Add((UIElement) textBlock2);
      bool isNewJidInAddressBook = false;
      ContactsContext.Instance((Action<ContactsContext>) (db =>
      {
        UserStatus userStatus = db.GetUserStatus(newJid, false);
        isNewJidInAddressBook = userStatus != null && userStatus.IsInDevicePhonebook;
      }));
      if (!isNewJidInAddressBook)
      {
        Button button = new Button();
        button.Padding = new Thickness(12.0, 0.0, 12.0, 0.0);
        button.Margin = new Thickness(pageSideMargin - 12.0, 0.0, pageSideMargin - 12.0, -6.0);
        button.Content = (object) AppResources.AddToContactsButton;
        button.HorizontalAlignment = HorizontalAlignment.Left;
        Button addToContactsButton = button;
        addToContactsButton.Click += (RoutedEventHandler) ((sender, e) => this.AddJidToContacts(newJid, true, (Action) (() =>
        {
          if (isOldChat)
          {
            addToContactsButton.Visibility = Visibility.Collapsed;
          }
          else
          {
            if (this.numberChangePanel == null)
              return;
            this.numberChangePanel.Visibility = Visibility.Collapsed;
            this.LayoutRoot.Children.Remove((UIElement) this.numberChangePanel);
            this.numberChangePanel = (Panel) null;
          }
        })));
        panel.Children.Add((UIElement) addToContactsButton);
      }
      if (isOldChat)
      {
        Button button1 = new Button();
        button1.Padding = new Thickness(12.0, 0.0, 12.0, 0.0);
        button1.Margin = new Thickness(pageSideMargin - 12.0, 0.0, pageSideMargin - 12.0, -6.0);
        button1.Content = (object) AppResources.MessageNewNumber;
        button1.HorizontalAlignment = HorizontalAlignment.Left;
        Button button2 = button1;
        button2.Click += (RoutedEventHandler) ((sender, e) => NavUtils.NavigateToChat(newJid, false));
        panel.Children.Add((UIElement) button2);
      }
      Button button3 = new Button();
      button3.Padding = new Thickness(12.0, 0.0, 12.0, 0.0);
      button3.Margin = new Thickness(pageSideMargin - 12.0, 0.0, pageSideMargin - 12.0, 0.0);
      button3.Content = (object) AppResources.DismissButton;
      button3.HorizontalAlignment = HorizontalAlignment.Left;
      Button button4 = button3;
      button4.Click += (RoutedEventHandler) ((sender, e) =>
      {
        panel.Visibility = Visibility.Collapsed;
        Action action = onDimiss;
        if (action == null)
          return;
        action();
      });
      panel.Children.Add((UIElement) button4);
      Rectangle rectangle1 = new Rectangle();
      rectangle1.Height = 1.0;
      rectangle1.VerticalAlignment = VerticalAlignment.Top;
      rectangle1.HorizontalAlignment = HorizontalAlignment.Stretch;
      rectangle1.Opacity = 0.35;
      rectangle1.Fill = (Brush) UIUtils.ForegroundBrush;
      rectangle1.Margin = new Thickness(0.0, 8.0, 0.0, 0.0);
      Rectangle rectangle2 = rectangle1;
      panel.Children.Add((UIElement) rectangle2);
      Grid.SetRow((FrameworkElement) panel, 1);
      this.numberChangePanel = (Panel) panel;
      this.LayoutRoot.Children.Add((UIElement) panel);
    }

    private bool IsTrustedBusiness(UserStatus user)
    {
      if (user == null || user.VerifiedLevel != VerifiedLevel.high)
        return false;
      UserStatusProperties internalProperties = user.InternalProperties;
      int num;
      if (internalProperties == null)
      {
        num = 1;
      }
      else
      {
        bool? hasSentHsm = internalProperties.HasSentHsm;
        bool flag = true;
        num = hasSentHsm.GetValueOrDefault() == flag ? (!hasSentHsm.HasValue ? 1 : 0) : 1;
      }
      return num == 0;
    }

    private void InitSpamReportingPanel(Conversation convo)
    {
      if (this.spamReportPanel != null)
        return;
      string jid = this.Jid;
      if (JidHelper.IsBroadcastJid(jid) || JidHelper.IsPsaJid(jid))
        return;
      bool flag1 = JidHelper.IsGroupJid(jid);
      UserStatus user = flag1 ? (UserStatus) null : UserCache.Get(jid, true);
      if (this.IsTrustedBusiness(user))
        return;
      bool flag2 = user != null && user.IsVerified() && user.VerifiedLevel != 0;
      double pageSideMargin = UIUtils.PageSideMargin;
      Grid grid = new Grid();
      grid.CacheMode = (CacheMode) new BitmapCache();
      grid.Margin = new Thickness(pageSideMargin, 6.0, pageSideMargin, 6.0);
      grid.Background = (Brush) UIUtils.BackgroundBrush;
      Grid element1 = grid;
      element1.RowDefinitions.Add(new RowDefinition()
      {
        Height = GridLength.Auto
      });
      element1.RowDefinitions.Add(new RowDefinition()
      {
        Height = GridLength.Auto
      });
      element1.RowDefinitions.Add(new RowDefinition()
      {
        Height = GridLength.Auto
      });
      element1.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(0.5, GridUnitType.Star)
      });
      element1.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(0.5, GridUnitType.Star)
      });
      TextBlock textBlock = new TextBlock();
      textBlock.Style = Application.Current.Resources[(object) "PhoneTextSubtleStyle"] as Style;
      textBlock.Margin = new Thickness(0.0);
      textBlock.TextWrapping = TextWrapping.Wrap;
      TextBlock element2 = textBlock;
      Grid.SetRow((FrameworkElement) element2, 0);
      Grid.SetColumn((FrameworkElement) element2, 0);
      Grid.SetColumnSpan((FrameworkElement) element2, 2);
      element1.Children.Add((UIElement) element2);
      element2.Text = flag2 ? (user.VerifiedLevel == VerifiedLevel.high ? AppResources.VerifiedOfficialBizNotInContacts : AppResources.VerifiedBizNotInContacts) : (flag1 ? AppResources.GroupSpamReportingTooltip : AppResources.SenderNotInContacts);
      if (flag1)
      {
        Button button1 = new Button();
        button1.Padding = new Thickness(0.0, 6.0, 0.0, 6.0);
        button1.Margin = new Thickness(-12.0, 0.0, -12.0, 0.0);
        Button element3 = button1;
        element3.Click += new RoutedEventHandler(this.ReportSpam_Click);
        Grid.SetRow((FrameworkElement) element3, 1);
        Grid.SetColumn((FrameworkElement) element3, 0);
        Grid.SetColumnSpan((FrameworkElement) element3, 2);
        element1.Children.Add((UIElement) element3);
        if (this.viewModel != null && this.viewModel.IsReadOnly)
          element3.Content = (object) AppResources.ReportSpamButton;
        else
          element3.Content = (object) AppResources.ReportSpamAndLeave;
        Button button2 = new Button();
        button2.Padding = new Thickness(0.0, 6.0, 0.0, 6.0);
        button2.Margin = new Thickness(-12.0, 0.0, -12.0, 0.0);
        button2.Content = (object) AppResources.NotSpam;
        Button element4 = button2;
        element4.Click += new RoutedEventHandler(this.NotSpam_Click);
        Grid.SetRow((FrameworkElement) element4, 2);
        Grid.SetColumn((FrameworkElement) element4, 0);
        Grid.SetColumnSpan((FrameworkElement) element4, 2);
        element1.Children.Add((UIElement) element4);
      }
      else
      {
        if ((user != null ? (user.IsEnterprise() ? 1 : 0) : 0) == 0)
        {
          bool currentlyBlocked = false;
          if (JidHelper.IsUserJid(jid))
            ContactsContext.Instance((Action<ContactsContext>) (cdb => currentlyBlocked = this.Jid != null && cdb.BlockListSet.ContainsKey(this.Jid)));
          Button button3 = new Button();
          button3.Padding = new Thickness(0.0, 6.0, 0.0, 6.0);
          button3.Margin = new Thickness(-12.0, 0.0, pageSideMargin / 2.0 - 12.0, 0.0);
          button3.Content = currentlyBlocked ? (object) AppResources.UnblockContact : (object) AppResources.Block;
          Button element5 = button3;
          element5.Click += new RoutedEventHandler(this.ToggleBlock_Click);
          Grid.SetRow((FrameworkElement) element5, 1);
          Grid.SetColumn((FrameworkElement) element5, 0);
          element1.Children.Add((UIElement) element5);
          this.blockToggleButton = element5;
          this.SubscribeToBlockListChange();
          Button button4 = new Button();
          button4.Padding = new Thickness(0.0, 6.0, 0.0, 6.0);
          button4.Margin = new Thickness(pageSideMargin / 2.0 - 12.0, 0.0, -12.0, 0.0);
          button4.Content = (object) AppResources.ReportSpamButton;
          Button element6 = button4;
          element6.Click += new RoutedEventHandler(this.ReportSpam_Click);
          Grid.SetRow((FrameworkElement) element6, 1);
          Grid.SetColumn((FrameworkElement) element6, 1);
          element1.Children.Add((UIElement) element6);
        }
        Button button = new Button();
        button.Padding = new Thickness(0.0, 6.0, 0.0, 6.0);
        button.Margin = new Thickness(-12.0, 0.0, -12.0, 0.0);
        button.Content = (object) AppResources.AddToContactsButton;
        Button element7 = button;
        element7.Click += new RoutedEventHandler(this.AddToContacts_Click);
        Grid.SetRow((FrameworkElement) element7, 2);
        Grid.SetColumn((FrameworkElement) element7, 0);
        Grid.SetColumnSpan((FrameworkElement) element7, 2);
        element1.Children.Add((UIElement) element7);
      }
      Grid.SetRow((FrameworkElement) element1, 2);
      this.spamReportPanel = element1;
      this.LayoutRoot.Children.Insert(3, (UIElement) element1);
    }

    private void UpdateForCurrentInputMode()
    {
      ChatPage.InputMode currentInputMode = this.CurrentInputMode;
      this.UpdateForInputModeChanged(currentInputMode, currentInputMode);
    }

    public void RefreshDebugInfo(string context = null, bool gc = true)
    {
      AppState.Worker.Enqueue((Action) (() => Stats.LogMemoryUsage(context, gc)));
    }

    private void StartOptimisticUploading(MediaSharingState sharingState)
    {
      if (this.currentOptUploadSharingState == null)
      {
        Log.l("OPC", "Sharing state started {0}", (object) sharingState.Mode);
        this.currentSharingStateUnsub.SafeDispose();
        this.currentOptUploadSharingState = sharingState;
        this.OptUploadSharingStateSelectedItemsChanged(MediaSharingState.SelectedItemsChangeCause.Add);
        this.currentSharingStateUnsub = sharingState.SubscribeToSelectedItemsChange(new Action<MediaSharingState.SelectedItemsChangeCause>(this.OptUploadSharingStateSelectedItemsChanged));
      }
      else
      {
        if (this.currentOptUploadSharingState == sharingState)
          return;
        Log.l("OPC", "Sharing state changed {0} {1}", (object) sharingState.Mode, (object) this.currentOptUploadSharingState.Mode);
        this.currentSharingStateUnsub.SafeDispose();
        this.currentOptUploadSharingState = sharingState;
        this.OptUploadSharingStateSelectedItemsChanged(MediaSharingState.SelectedItemsChangeCause.Undefined);
        this.currentSharingStateUnsub = sharingState.SubscribeToSelectedItemsChange(new Action<MediaSharingState.SelectedItemsChangeCause>(this.OptUploadSharingStateSelectedItemsChanged));
      }
    }

    private Dictionary<string, OptimisticUpload> StopAndRetrieveOptimisticUploads()
    {
      Dictionary<string, OptimisticUpload> dictionary = (Dictionary<string, OptimisticUpload>) null;
      lock (this.optimisticUploadLock)
      {
        dictionary = this.optUploadManager?.RetrieveOptimisticUploads();
        if (dictionary != null && dictionary.Count > 0)
        {
          Log.l("OPC", "Retrieved optimistic uploads, {0} from {1} supplied", (object) dictionary.Count, (object) this.optimisticUploadPicItemList.Count);
          for (int index = this.optimisticUploadPicItemList.Count - 1; index >= 0; --index)
          {
            MediaSharingState.PicInfo optimisticUploadPicItem = this.optimisticUploadPicItemList[index];
            OptimisticUpload optimisticUpload = (OptimisticUpload) null;
            if (dictionary.TryGetValue(optimisticUploadPicItem.GetOptimisticUniqueId(), out optimisticUpload))
            {
              if (optimisticUploadPicItem.isChangedByUser())
                dictionary.Remove(optimisticUploadPicItem.GetOptimisticUniqueId());
              else
                this.optimisticUploadPicItemList.RemoveAt(index);
            }
          }
        }
        else
          Log.l("OPC", "No optimistic uploads");
      }
      this.StopAndClearOptimisticUploads();
      return dictionary;
    }

    private void StopAndClearOptimisticUploads()
    {
      Log.l("OPC", "Clear unprocessed optimistic uploads | Count: {0}", (object) this.optimisticUploadPicItemList.Count);
      this.currentSharingStateUnsub.SafeDispose();
      this.currentSharingStateUnsub = (IDisposable) null;
      lock (this.optimisticUploadLock)
      {
        foreach (MediaSharingState.PicInfo optimisticUploadPicItem in this.optimisticUploadPicItemList)
          this.optUploadManager?.RemoveFromOptimisticUploadQueue(optimisticUploadPicItem);
        this.optimisticUploadPicItemList.Clear();
      }
      this.optUploadManager.SafeDispose();
    }

    private void OptUploadSharingStateSelectedItemsChanged(
      MediaSharingState.SelectedItemsChangeCause cause)
    {
      Log.l("OPC", "Detected change: {0} | current count: {1} | new count: {2} | allowed: {3}", (object) cause, (object) this.optimisticUploadPicItemList.Count, (object) this.currentOptUploadSharingState.SelectedCount, (object) MediaUpload.OptimisticUploadAllowed);
      if (!MediaUpload.OptimisticUploadAllowed)
        return;
      int imageMaxEdge = Settings.ImageMaxEdge;
      List<MediaSharingState.PicInfo> picInfoList1 = new List<MediaSharingState.PicInfo>();
      foreach (MediaSharingState.IItem selectedItem in this.currentOptUploadSharingState.SelectedItems)
      {
        if (selectedItem.GetMediaType() == FunXMPP.FMessage.Type.Image)
        {
          MediaSharingState.PicInfo picInfo = selectedItem.ToPicInfo(new System.Windows.Size((double) imageMaxEdge, (double) imageMaxEdge));
          if (!picInfo.isChangedByUser())
            picInfoList1.Add(picInfo);
        }
      }
      lock (this.optimisticUploadLock)
      {
        if (this.optUploadManager == null)
          this.optUploadManager = new OptimisticUploadManager(wam_enum_opt_upload_context_type.CHAT_MEDIA_PICKER);
        int num1 = 0;
        int num2 = 0;
        if (cause == MediaSharingState.SelectedItemsChangeCause.Add || cause == MediaSharingState.SelectedItemsChangeCause.Undefined)
        {
          foreach (MediaSharingState.PicInfo optimisticUploadPicItem in this.optimisticUploadPicItemList)
          {
            foreach (MediaSharingState.PicInfo picInfo in picInfoList1)
            {
              if (picInfo.GetOptimisticUniqueId() == optimisticUploadPicItem.GetOptimisticUniqueId())
              {
                picInfoList1.Remove(picInfo);
                break;
              }
            }
          }
          foreach (MediaSharingState.PicInfo picInfo in picInfoList1)
          {
            if (!picInfo.isChangedByUser())
            {
              bool optimisticUploadQueue = this.optUploadManager.AddToOptimisticUploadQueue(picInfo, picInfo.GetPathForOptimisticUpload());
              Log.l("OPC", "Attempting to add {0} - result {1}", (object) picInfo.GetOptimisticUniqueId(), (object) optimisticUploadQueue);
              if (optimisticUploadQueue)
              {
                this.optimisticUploadPicItemList.Add(picInfo);
                ++num1;
              }
            }
            else
              Log.l("OPC", "Ignoring item changed by user {0}", (object) picInfo.GetOptimisticUniqueId());
          }
        }
        if (cause == MediaSharingState.SelectedItemsChangeCause.Delete || cause == MediaSharingState.SelectedItemsChangeCause.Undefined)
        {
          List<MediaSharingState.PicInfo> picInfoList2 = new List<MediaSharingState.PicInfo>();
          foreach (MediaSharingState.PicInfo optimisticUploadPicItem in this.optimisticUploadPicItemList)
          {
            bool flag = false;
            foreach (MediaSharingState.PicInfo picInfo in picInfoList1)
            {
              if (picInfo.GetOptimisticUniqueId() == optimisticUploadPicItem.GetOptimisticUniqueId())
              {
                flag = true;
                break;
              }
            }
            if (!flag)
            {
              picInfoList2.Add(optimisticUploadPicItem);
              break;
            }
          }
          using (List<MediaSharingState.PicInfo>.Enumerator enumerator = picInfoList2.GetEnumerator())
          {
            if (enumerator.MoveNext())
            {
              MediaSharingState.PicInfo current = enumerator.Current;
              bool flag = this.optUploadManager.RemoveFromOptimisticUploadQueue(current);
              Log.l("OPC", "Attempting to remove {0} - result {1}", (object) current.GetOptimisticUniqueId(), (object) flag);
              this.optimisticUploadPicItemList.Remove(current);
              ++num2;
            }
          }
        }
        Log.l("OPC", "Processed change: {0} | current count: {1} | added: {2} | removed: {3}", (object) cause, (object) this.optimisticUploadPicItemList.Count, (object) num1, (object) num2);
      }
    }

    private void UpdatePttRecordButton(bool updateVisibility = true, bool updateEnableStatus = true)
    {
      if (!updateEnableStatus)
        return;
      this.inputBar.EnableRecording(!Voip.IsInCall && !this.GetAudioPlayback().Player.IsActive && !this.MessageList.IsAnyMessagesSelected);
    }

    private void ResetPageMenu()
    {
      if (this.Jid == null)
        return;
      string jid = this.Jid;
      List<FlyoutCommand> flyoutCommandList = new List<FlyoutCommand>();
      flyoutCommandList.Add(new FlyoutCommand(AppResources.SelectMessages, true, (Action) (() => this.SelectMessages_Click((object) null, (EventArgs) null)), "AidSelectMessagesButton"));
      bool isBlockedContact = false;
      switch (JidHelper.GetJidType(jid))
      {
        case JidHelper.JidTypes.User:
          bool inContacts = false;
          ContactsContext.Instance((Action<ContactsContext>) (cdb =>
          {
            inContacts = cdb.GetUserStatus(this.Jid).IsInDeviceContactList;
            isBlockedContact = cdb.BlockListSet.ContainsKey(this.Jid);
          }));
          flyoutCommandList.Add(new FlyoutCommand(AppResources.ContactInfo, true, (Action) (() => this.UserInfo_Click((object) null, (EventArgs) null)), "AidInfoButton"));
          if (!inContacts)
          {
            flyoutCommandList.Add(new FlyoutCommand(AppResources.AddToContactsButton, true, (Action) (() => this.AddToExistingContact_Click((object) null, (EventArgs) null)), "AidAddToContactsButton"));
            break;
          }
          break;
        case JidHelper.JidTypes.Group:
          flyoutCommandList.Add(new FlyoutCommand(AppResources.GroupInfo, true, (Action) (() => this.MultiParticipantsChatInfo_Click((object) null, (EventArgs) null)), "AidGroupInfoButton"));
          break;
        case JidHelper.JidTypes.Broadcast:
          flyoutCommandList.Add(new FlyoutCommand(AppResources.AppBarListInfo, true, (Action) (() => this.MultiParticipantsChatInfo_Click((object) null, (EventArgs) null)), "AidListInfoButton"));
          break;
      }
      if (!isBlockedContact && !this.IsSuspicious)
        flyoutCommandList.Add(new FlyoutCommand(AppResources.WallpaperSettings, true, (Action) (() => this.Wallpaper_Click((object) null, (EventArgs) null)), "AidWallpaperButton"));
      if (!flyoutCommandList.Any<FlyoutCommand>())
        return;
      if (this.useTitleBarMenu)
        this.titlePanel.SetMoreActions((IEnumerable<FlyoutCommand>) flyoutCommandList);
      else
        this.ResetDefaultAppBar(flyoutCommandList);
    }

    private void EnableVoipButtons(bool enable) => this.titlePanel.EnableVoipButtons(enable);

    private void InitDefaultAppBar()
    {
      if (this.useTitleBarMenu)
        return;
      if (this.defaultBar == null)
        this.defaultBar = new Microsoft.Phone.Shell.ApplicationBar();
      this.ApplicationBar = (IApplicationBar) this.defaultBar;
    }

    private void ResetDefaultAppBar(List<FlyoutCommand> commands)
    {
      if (this.useTitleBarMenu)
        return;
      if (this.defaultBar == null)
        this.defaultBar = new Microsoft.Phone.Shell.ApplicationBar();
      else
        this.defaultBar.MenuItems.Clear();
      commands.ForEach((Action<FlyoutCommand>) (c =>
      {
        ApplicationBarMenuItem applicationBarMenuItem = new ApplicationBarMenuItem()
        {
          Text = c.Title,
          IsEnabled = c.IsEnabled
        };
        Action a = c.ClickAction;
        applicationBarMenuItem.Click += (EventHandler) ((sender, e) => a());
        this.defaultBar.MenuItems.Add((object) applicationBarMenuItem);
      }));
      if (this.defaultBar == this.ApplicationBar)
        return;
      this.ApplicationBar = (IApplicationBar) this.defaultBar;
    }

    private void TryInitSelectModeAppBar()
    {
      if (this.selectModeBar != null)
        return;
      this.selectModeBar = new Microsoft.Phone.Shell.ApplicationBar();
      ApplicationBarIconButton applicationBarIconButton1 = new ApplicationBarIconButton()
      {
        IconUri = new Uri("/Images/assets/dark/forward.png", UriKind.Relative),
        Text = "Forward",
        IsEnabled = false
      };
      applicationBarIconButton1.Click += new EventHandler(this.ForwardMessages_Click);
      this.selectModeBar.Buttons.Add((object) applicationBarIconButton1);
      ApplicationBarIconButton applicationBarIconButton2 = new ApplicationBarIconButton()
      {
        IconUri = new Uri("/Images/assets/dark/copy.png", UriKind.Relative),
        Text = "Copy",
        IsEnabled = false
      };
      applicationBarIconButton2.Click += new EventHandler(this.CopyMessages_Click);
      this.selectModeBar.Buttons.Add((object) applicationBarIconButton2);
      ApplicationBarIconButton applicationBarIconButton3 = new ApplicationBarIconButton()
      {
        IconUri = new Uri("/Images/assets/dark/delete.png", UriKind.Relative),
        Text = "Delete",
        IsEnabled = false
      };
      applicationBarIconButton3.Click += new EventHandler(this.DeleteMessages_Click);
      this.selectModeBar.Buttons.Add((object) applicationBarIconButton3);
      Localizable.LocalizeAppBar(this.selectModeBar);
    }

    private void UpdateSelectionModeAppBar()
    {
      if (this.selectModeBar == null)
        this.TryInitSelectModeAppBar();
      List<Message> selectedMessages = this.MessageList.GetMultiSelectedMessages();
      int count = selectedMessages.Count;
      if (count == 0)
      {
        new AppBarWrapper((IApplicationBar) this.selectModeBar).IsEnabled = this.isForwardButtonEnabled = this.isCopyButtonEnabled = false;
      }
      else
      {
        int num = count <= 0 ? 0 : (count <= 10 ? 1 : 0);
        bool flag1 = (num & (Settings.StickersEnabled ? (true ? 1 : 0) : (selectedMessages.All<Message>((Func<Message, bool>) (m => m.MediaWaType != FunXMPP.FMessage.Type.Sticker)) ? 1 : 0)) & (selectedMessages.All<Message>((Func<Message, bool>) (m => m.MediaWaType != FunXMPP.FMessage.Type.Revoked)) ? 1 : 0)) != 0;
        if (this.isForwardButtonEnabled != flag1)
          (this.selectModeBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = this.isForwardButtonEnabled = flag1;
        bool flag2 = num != 0 && !selectedMessages.Any<Message>((Func<Message, bool>) (m => m.MediaWaType != FunXMPP.FMessage.Type.Undefined && m.MediaWaType != FunXMPP.FMessage.Type.ExtendedText));
        if (this.isCopyButtonEnabled != flag2)
          (this.selectModeBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = this.isCopyButtonEnabled = flag2;
        (this.selectModeBar.Buttons[2] as ApplicationBarIconButton).IsEnabled = count > 0;
      }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
      this.InitOnLoaded();
      if (this.initState == null || this.initState.InputMode == ChatPage.InputMode.None)
        this.UpdateForCurrentInputMode();
      else
        this.SetInputMode(this.initState.InputMode, false, "set input mode on entrance");
      ChatPageHelper.MaybeAskForContactDetails(this.Jid);
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        Conversation conversation = this.GetConversation(db, false);
        this.viewModel.WasConversationSeenOnEntry = conversation != null && conversation.IsConversationSeen();
      }));
      FieldStats.ReportChatOpen(this.Jid);
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      ChatPage.Current = this;
      LongMessageSplitter.ResetCharSizeEstimation();
      this.TextFontSize = Settings.SystemFontSize;
      if (this.Jid == null)
      {
        if (e.NavigationMode == NavigationMode.Reset)
        {
          WAThreadPool.Scheduler.Schedule((Action) (() =>
          {
            if (ChatPage.Current != this || this.isPageRemoved_ || this.Jid != null)
              return;
            this.Dispatcher.BeginInvoke((Action) (() =>
            {
              Log.d(this.LogHeader, "OnNavigatedTo | Reset | Workaround Applied");
              this.OnNavigatedTo(new NavigationEventArgs(e.Content, e.Uri, NavigationMode.New, e.IsNavigationInitiator));
            }));
          }), TimeSpan.FromMilliseconds(750.0));
          Log.d(this.LogHeader, "OnNavigatedTo | Reset | Skipped");
          return;
        }
        string jid = (string) null;
        if (!this.NavigationContext.QueryString.TryGetValue("jid", out jid) || string.IsNullOrEmpty(jid))
        {
          this.Dispatcher.BeginInvoke((Action) (() => NavUtils.GoBack()));
          return;
        }
        this.Jid = jid;
        this.inputBar.SetTargetJid(jid);
        FieldStats.SetChatOpenStartTime(this.Jid);
        this.InitOnNavigatedTo();
      }
      else
      {
        FieldStats.SetChatOpenStartTime(this.Jid);
        this.RestorePrePttRecordingStates();
        if (this.isWallpaperDirty)
        {
          this.isWallpaperDirty = false;
          WallpaperStore.WallpaperState wallpaper = (WallpaperStore.WallpaperState) null;
          MessagesContext.Run((MessagesContext.MessagesCallback) (db => wallpaper = WallpaperStore.Get(db, this.Jid, true)));
          this.SetWallpaper(wallpaper, false);
        }
        if (this.viewModel.IsConversationStatusChanged)
        {
          this.ReloadMessages();
          this.viewModel.IsConversationStatusChanged = false;
        }
        else
          this.MessageList.RemoveUnreadDivider();
        this.EnableMultiSelection(false);
        if (JidHelper.IsUserJid(this.Jid))
          this.EnableVoipButtons(!Voip.IsInCall);
      }
      ConversionRecordHelper.MaybeUpdateConversionRecord(this.Jid, (DateTime?) this.convo_?.LocalTimestamp);
      if (this.openWindowSub == null)
      {
        Action<Action> action = (Action<Action>) (a => a());
        if (e.NavigationMode == NavigationMode.Reset)
        {
          IObservable<int> obs = Observable.CreateWithDisposable<int>((Func<IObserver<int>, IDisposable>) (observer => WAThreadPool.Scheduler.Schedule((Action) (() => observer.OnNext(3)), TimeSpan.FromMilliseconds(750.0)))).SubscribeOnDispatcher<int>().ObserveOnDispatcher<int>().Select<int, int>((Func<int, int>) (flags => !this.isPageRemoved_ ? flags : 0)).Merge<int>(BackKeyBroker.Get((PhoneApplicationPage) this, 0).Select<CancelEventArgs, int>((Func<CancelEventArgs, int>) (_ => 1))).Take<int>(1);
          action = (Action<Action>) (a => obs.Subscribe<int>((Action<int>) (result =>
          {
            if ((result & 16) != 0)
            {
              Log.d(this.LogHeader, "reset nav workaround | timer due");
              if (AppState.IsConversationOpen(this.Jid))
              {
                App.CurrentApp.ToHandleFastResumeTargetNav = false;
                Log.l(this.LogHeader, "reset nav workaround | skip fast resume handling");
              }
            }
            else if ((result & 32) != 0)
              Log.d(this.LogHeader, "reset nav workaround | back key pressed");
            if ((result & 2) != 0 && this.openWindowSub == null)
              a();
            if ((result & 1) == 0)
              return;
            WAThreadPool.QueueUserWorkItem((Action) (() => MessagesContext.Run((MessagesContext.MessagesCallback) (db => this.TryMarkRead(db, this.Jid, true, new int?(), "reset workaround")))));
          })));
        }
        action((Action) (() =>
        {
          if (this.openWindowSub != null)
            return;
          this.openWindowSub = AppState.MarkConverationAsOpen(this.Jid);
        }));
      }
      if (e.NavigationMode == NavigationMode.Back || e.NavigationMode == NavigationMode.Refresh)
        MessagesContext.Run((MessagesContext.MessagesCallback) (db => this.TryMarkRead(db, this.Jid, true, new int?(), "on nav back/refresh")));
      this.BindRootFrameTranslateY();
      if (this.HasPendingFilesToSend)
        this.SendFilePickerSelectedFiles();
      this.InitAudioPlayback();
      if (this.MessageList != null)
        this.MessageList.OnViewReload();
      base.OnNavigatedTo(e);
    }

    public void OnNavigatedFromImpl()
    {
      ChatPageHelper.SaveDraft(this.convo_, this.inputBar.GetText());
      InlineVideoMessageViewPanel.OnNavigatedFrom(this);
    }

    protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
    {
      ChatPage.Current = (ChatPage) null;
      this.openWindowSub.SafeDispose();
      this.openWindowSub = (IDisposable) null;
      this.RestorePrePttRecordingStates();
      base.OnNavigatingFrom(e);
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
      this.SetInputMode(ChatPage.InputMode.None, false, "reset on navigated from");
      try
      {
        this.inputBar.TerminateRecording();
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "chatpage: navigating from");
      }
      if (this.isPageRemoved_)
      {
        Log.d(this.LogHeader, "on navigated from | skipped");
      }
      else
      {
        this.OnNavigatedFromImpl();
        this.DisposePresence();
        this.ClearValue(ChatPage.RootFrameTranslateYProperty);
        base.OnNavigatedFrom(e);
      }
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      this.isPageRemoved_ = true;
      this.DisposeAudioPlayback();
      this.DisposeAll();
      base.OnRemovedFromJournal(e);
    }

    public bool OnRotationTransitionStarting(PageOrientation newOrientation)
    {
      if (this.IsPttRecordingInProgress)
        return false;
      this.viewModel.Orientation = newOrientation;
      this.inputBar.SetOrientation(newOrientation);
      bool flag = false;
      switch (this.CurrentInputMode)
      {
        case ChatPage.InputMode.Keyboard:
          this.SetInputMode(ChatPage.InputMode.None, false, "orientation change while sip is up");
          break;
        case ChatPage.InputMode.Emoji:
        case ChatPage.InputMode.Attachment:
          flag = true;
          break;
      }
      this.UpdateForCurrentInputMode();
      return !flag;
    }

    public void OnRotationTransitionFinished(ChatPage.InputMode modePreRotation)
    {
      this.SetInputMode(modePreRotation, false, "restore pre orientation change input mode");
    }

    private void OnBackKey(CancelEventArgs e)
    {
      if (this.inputBar.IsMentionsListOpen())
      {
        e.Cancel = true;
        this.inputBar.CloseMentionsList();
      }
      else if (this.MessageList.IsMultiSelectionEnabled)
      {
        e.Cancel = true;
        this.EnableMultiSelection(false);
      }
      else
      {
        if (UIUtils.FindFirstInVisualTree((DependencyObject) this, (Func<DependencyObject, bool>) (obj =>
        {
          ContextMenu contextMenu = ContextMenuService.GetContextMenu(obj);
          return contextMenu != null && contextMenu.IsOpen;
        })) != null)
          return;
        e.Cancel = true;
        Storyboarder.Perform(WaAnimations.PageTransition(PageTransitionAnimation.ContinuumBackwardOut), (DependencyObject) this.LayoutRoot);
        NavUtils.GoBack(this.NavigationService);
      }
    }

    private static void OnRootFrameTranslateYChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      if (!(d is ChatPage chatPage) || chatPage.CurrentInputMode != ChatPage.InputMode.Keyboard && chatPage.CurrentInputMode != ChatPage.InputMode.None)
        return;
      chatPage.UpdateForSIPOffset(-(double) e.NewValue);
    }

    private void MessageList_MultiSelectionsChanged(object sender, EventArgs e)
    {
      this.UpdatePttRecordButton();
      this.UpdateSelectionModeAppBar();
      this.EnableMultiSelection(this.MessageList.IsAnyMessagesSelected);
    }

    private void MessageList_SelectionChanged(object sender, EventArgs e)
    {
      if (this.isInSelectionMode)
        return;
      this.SetInputMode(ChatPage.InputMode.None, true, "tap on message list");
    }

    private void MessageList_OlderMessagesRequested(object sender, EventArgs e)
    {
      if (this.msgLoader_ == null)
        return;
      this.msgLoader_.RequestOlderMessages();
    }

    private void MessageList_NewerMessagesRequested(object sender, EventArgs e)
    {
      if (this.msgLoader_ == null)
        return;
      this.msgLoader_.RequestNewerMessages();
    }

    private void MessageList_ScrollToBottomRequested(object sender, EventArgs e)
    {
      if (this.convo_ != null)
      {
        int? targetLandingMsgId = this.convo_.FirstUnreadMessageID ?? this.convo_.LastMessageID;
        if (this.MessageList.LastTappedReplyMsg != null && this.MessageList.IsMessageRealized(this.MessageList.LastTappedQuotedMsg))
          targetLandingMsgId = new int?(this.MessageList.LastTappedReplyMsg.MessageID);
        if (targetLandingMsgId.HasValue && this.MessageList.ScrollToMessage(targetLandingMsgId.Value))
        {
          Log.d(this.LogHeader, "jumped to realized msg bubble");
        }
        else
        {
          Log.d(this.LogHeader, "jumped to unrealized msg bubble");
          this.msgLoader_.ReInit(targetLandingMsgId);
        }
      }
      this.MessageList.ClearTappedReplyMsgBookmarks();
    }

    private void AttachPanelPopupManager_OrientationChanged(object sender, EventArgs e)
    {
      double num1 = Math.Max(this.ActualWidth, this.ActualHeight);
      double num2 = Math.Min(this.ActualWidth, this.ActualHeight);
      int flowDirection = (int) this.AttachPanel.FlowDirection;
      int num3 = 2;
      System.Windows.Size size = new System.Windows.Size();
      PopupManager popupManager = sender as PopupManager;
      if (popupManager.Orientation.IsLandscape())
      {
        size.Width = AttachPanel.ButtonSize * 2.0 + AttachPanel.ButtonGap + 48.0 * ResolutionHelper.ZoomMultiplier;
        size.Height = num2;
        this.attachPanelPopup.Height = size.Height;
        this.attachPanelPopup.Width = size.Width + (double) num3;
        this.attachPanelPopup.HorizontalOffset = 0.0;
        this.attachPanelPopup.VerticalOffset = popupManager.Orientation != PageOrientation.LandscapeRight ? num1 - size.Width : -(Math.Max(Application.Current.Host.Content.ActualWidth, Application.Current.Host.Content.ActualHeight) - num1 - (double) num3);
      }
      else
      {
        size.Width = num2;
        size.Height = AttachPanel.ButtonSize * 2.0 + AttachPanel.ButtonGap + 48.0 * ResolutionHelper.ZoomMultiplier;
        this.attachPanelPopup.Width = num2;
        this.attachPanelPopup.Height = size.Height + (double) num3;
        this.attachPanelPopup.HorizontalOffset = 0.0;
        this.attachPanelPopup.VerticalOffset = num1 - size.Height + this.Margin.Top - this.inputBarElement.ActualHeight;
      }
      if (this.attachPanelPopup.Child is FrameworkElement child)
      {
        child.Height = this.attachPanelPopup.Height;
        child.Width = this.attachPanelPopup.Width;
      }
      this.AttachPanel.Orientation = popupManager.Orientation;
    }

    private void Attach_Click(object sender, EventArgs e)
    {
      if (this.attachPanelPopup != null && this.attachPanelPopup.IsOpen)
        this.SetInputMode(ChatPage.InputMode.None, true, "close attach panel");
      else
        this.ValidateRecipientPreMessageSend().Take<bool>(1).Where<bool>((Func<bool, bool>) (notBlocked => notBlocked)).ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (_ => this.SetInputMode(ChatPage.InputMode.Attachment, false, "toggle attach panel")));
    }

    private void InputBar_SendTextNotified(ExtendedTextInputData inputData)
    {
      if (!LinkDetector.IsSendableText(inputData?.Text))
        return;
      inputData.Text = Emoji.ConvertToRichText(Emoji.ConvertToUnicode(inputData.Text));
      bool flag = false;
      if (inputData.Text.Length > 65536 && inputData.Text.GetRealCharLength() > 65536)
        flag = true;
      if (!flag)
      {
        this.SendText(inputData);
      }
      else
      {
        string stringWithIndex = Plurals.Instance.GetStringWithIndex(AppResources.ConfirmTextTruncationPlural, 1, (object) 65536.ToString("###,###", (IFormatProvider) CultureInfo.CurrentCulture), (object) 65536);
        Observable.Return<bool>(true).Decision(stringWithIndex).ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (confirmed =>
        {
          if (!confirmed)
            return;
          inputData.Text = inputData.Text.MaybeTruncateToMaxRealCharLength(65536);
          this.SendText(inputData);
        }));
      }
    }

    private void SendText(ExtendedTextInputData inputData)
    {
      if (this.CurrentInputMode == ChatPage.InputMode.Keyboard)
      {
        this.ignoreTextInputFocusChangeOnce_ = true;
        this.Focus();
        this.inputBar.SetText((string) null);
        this.OpenKeyboard();
      }
      else
        this.inputBar.SetText((string) null);
      this.inputBar.Clear();
      this.ValidateRecipientPreMessageSend().Take<bool>(1).ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (proceeed =>
      {
        if (!proceeed)
          this.inputBar.SetText(inputData.Text);
        else
          MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
          {
            Conversation conversation = this.GetConversation(db, false);
            if (conversation != null)
              conversation.ComposingText = (string) null;
            Message message = new Message(true)
            {
              KeyFromMe = true,
              KeyRemoteJid = this.Jid,
              KeyId = FunXMPP.GenerateMessageId(),
              Data = inputData.Text,
              Status = FunXMPP.FMessage.Status.Unsent,
              MediaWaType = FunXMPP.FMessage.Type.Undefined
            };
            WebPageMetadata linkPreviewData = inputData.LinkPreviewData;
            if (linkPreviewData != null)
            {
              message.MediaWaType = FunXMPP.FMessage.Type.ExtendedText;
              UriMessageWrapper uriMessageWrapper = new UriMessageWrapper(message)
              {
                Title = linkPreviewData.Title,
                Description = linkPreviewData.Description,
                CanonicalUrl = linkPreviewData.CanonicalUrl,
                MatchedText = linkPreviewData.MatchedText
              };
              message.BinaryData = linkPreviewData.ThumbnailBytes;
            }
            Message quotedMessage = inputData.QuotedMessage;
            string quotedChat = inputData.QuotedChat;
            message.SetQuote(quotedMessage, quotedChat);
            string[] mentionedJids = inputData.MentionedJids;
            if (mentionedJids != null && ((IEnumerable<string>) mentionedJids).Any<string>())
              message.SetMentionedJids(mentionedJids);
            if (this.c2cUriPhoneNumberFlag)
              message.SetC2cFlags(true, this.c2cDeepLinkData.SharedText != null);
            db.InsertMessageOnSubmit(message);
            db.SubmitChanges();
          }));
      }));
    }

    private void AddToContacts_Click(object sender, EventArgs e)
    {
      this.AddJidToContactsUseVCards(this.Jid, (Action) (() =>
      {
        if (this.spamReportPanel != null)
        {
          bool shouldHide = true;
          MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
          {
            if (!SuspiciousJid.IsJidSuspicious(db, this.Jid, false))
              return;
            shouldHide = false;
          }));
          if (shouldHide)
          {
            this.TryShowSpamReportingPanel(false, true);
            this.isSuspicious = new bool?(false);
            this.RefreshMessagesOnSuspicionChanged();
            AppState.QrPersistentAction.NotifyChatStatus(this.Jid, FunXMPP.ChatStatusForwardAction.NotSpam);
          }
        }
        if (this.contactVCardPanel == null)
          return;
        this.TryShowContactVCardPanel(false, true);
      }));
    }

    private void AddToExistingContact_Click(object sender, EventArgs e)
    {
      this.AddJidToContacts(this.Jid, true);
    }

    private void ToggleBlock_Click(object sender, EventArgs e) => this.ToggleBlock();

    private void ViewVCards_Click(object sender, EventArgs e)
    {
      if (this.Jid == null || !this.Jid.IsUserJid())
        return;
      ContactVCardPage.Start(this.Jid);
    }

    private void ViewVCardsDismiss_Click(object sender, EventArgs e)
    {
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        db.GetJidInfo(this.Jid, CreateOptions.CreateToDbIfNotFound).PromptedVCards = new bool?(true);
        db.SubmitChanges();
      }));
      this.TryShowContactVCardPanel(false, true);
    }

    private void ReportSpam_Click(object sender, EventArgs e)
    {
      Conversation convo = (Conversation) null;
      Message[] msgsToReport = (Message[]) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        convo = this.GetConversation(db, false, "report spam");
        if (convo == null)
          return;
        msgsToReport = db.GetLatestMessages(convo.Jid, convo.MessageLoadingStart(), new int?(5), new int?(0));
      }));
      if (JidHelper.IsUserJid(this.Jid))
      {
        this.ToggleBlock("chat", convo, msgsToReport);
      }
      else
      {
        if (!JidHelper.IsGroupJid(this.Jid) || convo == null)
          return;
        bool flag = !convo.IsGroupParticipant();
        Observable.Return<bool>(true).Decision(flag ? AppResources.ReportSpamAndLeaveConfirmBodyNoHistory : AppResources.ReportSpamConfirmBodyNoHistory, flag ? AppResources.ReportAndLeaveButton : AppResources.ReportSpamButton, AppResources.CancelButton).ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (confirmed =>
        {
          if (!confirmed)
            return;
          this.TryShowSpamReportingPanel(false, true);
          DeleteChat.Delete(this.Jid);
          this.Dispatcher.BeginInvoke((Action) (() => NavUtils.GoBack()));
          App.CurrentApp.Connection.InvokeWhenConnected((Action) (() =>
          {
            try
            {
              App.CurrentApp.Connection.SendSpamReport(msgsToReport, "chat", convo.Jid, convo.GroupOwner, convo.GroupSubject);
            }
            catch (Exception ex)
            {
              string context = string.Format("Sending spam report for {0}", (object) convo.Jid);
              Log.LogException(ex, context);
            }
          }));
        }));
      }
    }

    private void NotSpam_Click(object sender, EventArgs e)
    {
      this.TryShowSpamReportingPanel(false, true);
      MessagesContext.Run((MessagesContext.MessagesCallback) (db => SuspiciousJid.MarkJidSuspicious((SqliteMessagesContext) db, this.Jid, false)));
      AppState.QrPersistentAction.NotifyChatStatus(this.Jid, FunXMPP.ChatStatusForwardAction.NotSpam);
    }

    private void ForwardMessages_Click(object sender, EventArgs e)
    {
      Message[] array = this.MessageList.GetMultiSelectedMessages().OrderBy<Message, int>((Func<Message, int>) (m => m.MessageID)).ToArray<Message>();
      this.EnableMultiSelection(false);
      if (array == null || !((IEnumerable<Message>) array).Any<Message>())
        return;
      if (((IEnumerable<Message>) array).Where<Message>((Func<Message, bool>) (m => m.MediaKey != null && !m.LocalFileExists() || !m.ShouldSend())).Any<Message>())
        AppState.ClientInstance.ShowMessageBox(AppResources.BadForward);
      else
        SendMessage.ChooseRecipientAndForwardExisting(array);
    }

    private void CopyMessages_Click(object sender, EventArgs e)
    {
      Message[] array = this.MessageList.GetMultiSelectedMessages().OrderBy<Message, int>((Func<Message, int>) (m => m.MessageID)).ToArray<Message>();
      this.EnableMultiSelection(false);
      if (array == null || !((IEnumerable<Message>) array).Any<Message>())
        return;
      Dictionary<string, string> namesCache = new Dictionary<string, string>();
      Clipboard.SetText(Emoji.ConvertToTextOnly(string.Join("\n", ((IEnumerable<Message>) array).Where<Message>((Func<Message, bool>) (m => m.MediaWaType == FunXMPP.FMessage.Type.Undefined || m.MediaWaType == FunXMPP.FMessage.Type.ExtendedText)).Select<Message, string>((Func<Message, string>) (m =>
      {
        string str = (string) null;
        string senderJid = m.GetSenderJid();
        if (!namesCache.TryGetValue(senderJid, out str))
          namesCache[senderJid] = str = m.GetSenderDisplayName(false);
        DateTime? localTimestamp = m.LocalTimestamp;
        return !localTimestamp.HasValue ? string.Format("{0}: {1}", (object) str, (object) m.Data) : string.Format("{0}: {1}: {2}", (object) localTimestamp.Value, (object) str, (object) m.Data);
      }))), (byte[]) null));
    }

    private void DeleteMessages_Click(object sender, EventArgs e)
    {
      Message[] selectedMsgs = this.MessageList.GetMultiSelectedMessages().ToArray();
      if (selectedMsgs == null || !((IEnumerable<Message>) selectedMsgs).Any<Message>())
      {
        this.EnableMultiSelection(false);
      }
      else
      {
        int num = ((IEnumerable<Message>) selectedMsgs).Count<Message>((Func<Message, bool>) (m => !m.CanRevoke()));
        string message = Plurals.Instance.GetString(AppResources.DeleteMultipleMessagesConfirmPlural, selectedMsgs.Length);
        if (num == 0)
          Observable.Return<bool>(true).Decisions(message, new string[3]
          {
            AppResources.DeleteForSelfButton,
            AppResources.DeleteForEveryoneButton,
            AppResources.Cancel
          }).Take<int>(1).ObserveOnDispatcher<int>().Subscribe<int>((Action<int>) (result =>
          {
            switch (result)
            {
              case 0:
                this.EnableMultiSelection(false);
                MessagesContext.Run((MessagesContext.MessagesCallback) (db => db.DeleteMessages(selectedMsgs)));
                break;
              case 1:
                this.EnableMultiSelection(false);
                RevokeMessage.RevokeMessagesFromMe(selectedMsgs);
                if (Settings.RevokeFirstUseMessageDisplayed)
                  break;
                UIUtils.ShowMessageBoxWithLearnMore(AppResources.RevokeFirstUse, WaWebUrls.FaqUrlRecallWp);
                Settings.RevokeFirstUseMessageDisplayed = true;
                break;
            }
          }));
        else
          Observable.Return<bool>(true).Decision(message, AppResources.Delete, AppResources.Cancel).Take<bool>(1).Subscribe<bool>((Action<bool>) (confirmed =>
          {
            if (!confirmed)
              return;
            this.EnableMultiSelection(false);
            MessagesContext.Run((MessagesContext.MessagesCallback) (db => db.DeleteMessages(selectedMsgs)));
          }));
      }
    }

    private void UserInfo_Click(object sender, EventArgs e)
    {
      if (!JidHelper.IsUserJid(this.Jid))
        return;
      IDisposable sub = (IDisposable) null;
      System.Windows.Media.ImageSource pic = (System.Windows.Media.ImageSource) null;
      sub = ChatPictureStore.Get(this.Jid, false, false, true, ChatPictureStore.SubMode.GetCurrent).Select<ChatPictureStore.PicState, BitmapSource>((Func<ChatPictureStore.PicState, BitmapSource>) (picState => picState?.Image)).Timeout<BitmapSource>(TimeSpan.FromMilliseconds(1000.0)).Take<BitmapSource>(1).ObserveOnDispatcher<BitmapSource>().Subscribe<BitmapSource>((Action<BitmapSource>) (imgSrc => pic = (System.Windows.Media.ImageSource) imgSrc), (Action<Exception>) (ex =>
      {
        ContactInfoPage.Start(UserCache.Get(this.Jid, true));
        sub.SafeDispose();
        sub = (IDisposable) null;
      }), (Action) (() =>
      {
        ContactInfoPage.Start(UserCache.Get(this.Jid, true), pic);
        sub.SafeDispose();
        sub = (IDisposable) null;
      }));
    }

    private void MultiParticipantsChatInfo_Click(object sender, EventArgs e)
    {
      if (JidHelper.IsGroupJid(this.Jid))
      {
        Conversation convo = (Conversation) null;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db => convo = this.GetConversation(db, false, "group info")));
        GroupInfoPage.Start(this.NavigationService, convo);
      }
      else
      {
        if (!JidHelper.IsBroadcastJid(this.Jid))
          return;
        BroadcastListInfoPage.Start(this.NavigationService, this.Jid);
      }
    }

    private void Call_Click(object sender, EventArgs e)
    {
      CallContact.Call(this.Jid, context: "from chat page app bar");
    }

    private void VideoCall_Click(object sender, EventArgs e) => CallContact.VideoCall(this.Jid);

    private void SelectMessages_Click(object sender, EventArgs e)
    {
      this.EnableMultiSelection(!this.isInSelectionMode);
    }

    private void Camera_Click(object sender, EventArgs e) => this.LaunchCamera();

    private void Wallpaper_Click(object sender, EventArgs e) => SetWallpaperPage.Start(this.Jid);

    private void OnInputModeChanged(
      ChatPage.InputMode newMode,
      ChatPage.InputMode oldMode,
      bool skipKeyboardCheck)
    {
      this.Orientation.IsLandscape();
      if (newMode != ChatPage.InputMode.Attachment)
        this.CloseAttachPanel();
      if (newMode != ChatPage.InputMode.Emoji)
        this.CloseEmojiPicker();
      if (newMode != ChatPage.InputMode.Keyboard)
        this.CloseKeyboard();
      this.TryShowSpamReportingPanel(newMode == ChatPage.InputMode.None);
      switch (newMode - 2)
      {
        case ChatPage.InputMode.Undefined:
          this.OpenKeyboard();
          break;
        case ChatPage.InputMode.None:
          this.OpenEmojiPicker();
          break;
        case ChatPage.InputMode.Keyboard:
          this.OpenAttachPanel();
          break;
      }
      this.UpdateForInputModeChanged(oldMode, newMode);
      if (this.IsHidingPttNeeded(newMode))
      {
        this.inputBar.EnableRecording(false);
      }
      else
      {
        if (!this.IsHidingPttNeeded(oldMode))
          return;
        this.inputBar.EnableRecording(true);
      }
    }

    private void OnPttPlaybackStarted()
    {
      if (this.disablePttTimeoutTimer_ != null)
        this.disablePttTimeoutTimer_.Stop();
      this.UpdatePttRecordButton();
    }

    private void OnPttPlaybackStopped()
    {
      if (this.disablePttTimeoutTimer_ == null)
      {
        this.disablePttTimeoutTimer_ = new DispatcherTimer()
        {
          Interval = TimeSpan.FromMilliseconds(3000.0)
        };
        this.disablePttTimeoutTimer_.Tick += (EventHandler) ((sender2, e2) =>
        {
          this.disablePttTimeoutTimer_.Stop();
          this.UpdatePttRecordButton();
        });
      }
      else
        this.disablePttTimeoutTimer_.Stop();
      this.disablePttTimeoutTimer_.Start();
    }

    private void OnJumpToGroupRequest(string quotedGroupJid)
    {
      if (quotedGroupJid == null)
        Log.l(this.LogHeader, "jump to group | null group jid");
      else if (quotedGroupJid == this.Jid)
      {
        Log.l(this.LogHeader, "already in target group | quotedGroupJid:{0}", (object) quotedGroupJid);
      }
      else
      {
        Log.l(this.LogHeader, "jump to group | quotedGroupJid:{0}", (object) quotedGroupJid);
        Conversation targetConversation = (Conversation) null;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db => targetConversation = db.GetConversation(quotedGroupJid, CreateOptions.None)));
        if (targetConversation == null)
        {
          Log.l(this.LogHeader, "jump to group aborted | group not found");
        }
        else
        {
          Log.l(this.LogHeader, "jump to another chat | jid:{0}", (object) quotedGroupJid);
          NavUtils.NavigateToChat(quotedGroupJid, false);
        }
      }
    }

    private void OnJumpToMessageRequest(
      FunXMPP.FMessage.Key replyMsgKey,
      FunXMPP.FMessage.Key quotedMsgKey)
    {
      if (quotedMsgKey == null)
      {
        Log.l(this.LogHeader, "jump to msg | null msg key");
      }
      else
      {
        Log.l(this.LogHeader, "jump to msg | jid:{0},id:{1},from me:{2}", (object) quotedMsgKey.remote_jid, (object) quotedMsgKey.id, (object) quotedMsgKey.from_me);
        Message targetMsg = (Message) null;
        Message replyMsg = (Message) null;
        WaStatus targetStatus = (WaStatus) null;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          replyMsg = db.GetMessage(replyMsgKey.remote_jid, replyMsgKey.id, replyMsgKey.from_me);
          targetMsg = db.GetMessage(quotedMsgKey.remote_jid, quotedMsgKey.id, quotedMsgKey.from_me);
          if (targetMsg == null || !targetMsg.IsStatus())
            return;
          targetStatus = db.GetWaStatus(targetMsg.GetSenderJid(), targetMsg.MessageID);
        }));
        if (targetMsg == null)
          Log.l(this.LogHeader, "jump to msg aborted | msg not found");
        else if (targetStatus != null)
          WaStatusViewPage.Start((WaStatusThread) new SingleWaStatusThread(targetStatus), (WaStatusThread[]) null, false);
        else if (this.Jid != targetMsg.KeyRemoteJid)
        {
          Log.l(this.LogHeader, "jump to msg from another chat | jid:{0}", (object) quotedMsgKey.remote_jid);
          ChatPage.NextInstanceInitState = new ChatPage.InitState()
          {
            MessageLoader = MessageLoader.Get(targetMsg.KeyRemoteJid, new int?(), 0, targetInitialLandingMsgId: new int?(targetMsg.MessageID))
          };
          NavUtils.NavigateToChat(targetMsg.KeyRemoteJid, false);
        }
        else
        {
          if (this.MessageList.ScrollToMessage(targetMsg))
          {
            Log.l(this.LogHeader, "jumped to realized msg bubble");
          }
          else
          {
            Log.l(this.LogHeader, "jumping to unrealized msg bubble | re init");
            this.msgLoader_.ReInit(new int?(targetMsg.MessageID));
          }
          this.MessageList.SaveTappedReplyMsgBookmarks(targetMsg, replyMsg);
        }
      }
    }

    public void OnInitialMessages(Message[] msgs, int? targetLandingIndex, bool isReInit)
    {
      int length = msgs.Length;
      this.Dispatcher.BeginInvokeIfNeeded((Action) (() =>
      {
        if (this.isPageRemoved_)
        {
          Log.l(this.LogHeader, "init msgs ignored");
        }
        else
        {
          Conversation convo = (Conversation) null;
          WallpaperStore.WallpaperState wallpaper = (WallpaperStore.WallpaperState) null;
          bool showSpamReporting = false;
          bool showNumberChange = false;
          string oldJid = (string) null;
          string newJid = (string) null;
          if (JidHelper.IsUserJid(this.Jid))
            ContactsContext.Instance((Action<ContactsContext>) (db =>
            {
              Pair<string, string> numberJidsForPanel = db.GetChangeNumberJidsForPanel(this.Jid);
              if (numberJidsForPanel == null)
                return;
              oldJid = numberJidsForPanel.First;
              newJid = numberJidsForPanel.Second;
              showNumberChange = true;
              this.shouldCheckSuspicious = false;
            }));
          MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
          {
            convo = this.GetConversation(db, true, "on initial msgs");
            if (convo == null)
              return;
            if (this.shouldCheckSuspicious)
            {
              this.shouldCheckSuspicious = false;
              if (((IEnumerable<Message>) msgs).Any<Message>())
              {
                this.IsSuspicious = showSpamReporting = SuspiciousJid.IsJidSuspicious(db, convo.Jid, true);
                if (this.IsSuspicious)
                {
                  IDisposable jidInfoChangeSub = (IDisposable) null;
                  jidInfoChangeSub = MessagesContext.Events.JidInfoUpdateSubject.Where<DbDataUpdate>((Func<DbDataUpdate, bool>) (u => this.Jid == (u.UpdatedObj is JidInfo updatedObj2 ? updatedObj2.Jid : (string) null) && ((IEnumerable<string>) u.ModifiedColumns).Contains<string>("IsSuspicious"))).Select<DbDataUpdate, JidInfo>((Func<DbDataUpdate, JidInfo>) (u => u.UpdatedObj as JidInfo)).Where<JidInfo>((Func<JidInfo, bool>) (ji => ji != null && ji.IsSuspicious.HasValue)).ObserveOnDispatcher<JidInfo>().Subscribe<JidInfo>((Action<JidInfo>) (ji =>
                  {
                    this.IsSuspicious = ji.IsSuspicious.Value;
                    if (!this.IsSuspicious)
                    {
                      this.TryShowSpamReportingPanel(false, true);
                      this.RefreshMessagesOnSuspicionChanged();
                      this.titlePanel.UpdateTitleInfoPanel();
                    }
                    this.DisposeSubscription(ref jidInfoChangeSub);
                  }));
                  this.AddSubscription(jidInfoChangeSub);
                }
              }
            }
            if (isReInit)
            {
              Message message = ((IEnumerable<Message>) msgs).FirstOrDefault<Message>();
              int? nullable;
              if (message != null)
              {
                int messageId = message.MessageID;
                nullable = convo.LastMessageID;
                int valueOrDefault = nullable.GetValueOrDefault();
                if ((messageId == valueOrDefault ? (nullable.HasValue ? 1 : 0) : 0) == 0)
                  goto label_9;
              }
              MessagesContext db2 = db;
              string jid = convo.Jid;
              nullable = new int?();
              int? lastReadMsgId = nullable;
              this.TryMarkRead(db2, jid, true, lastReadMsgId, "mark initial msgs as read");
            }
label_9:
            wallpaper = WallpaperStore.Get(db, convo.Jid, true);
          }));
          if (showNumberChange)
          {
            this.InitChangeNumberNotificationPanel(oldJid, newJid, (Action) (() => ContactsContext.Instance((Action<ContactsContext>) (db => db.DeleteChangeNumberRecordsForJid(new string[2]
            {
              oldJid,
              newJid
            })))));
            this.ResetPageMenu();
          }
          else if (showSpamReporting)
          {
            this.InitSpamReportingPanel(convo);
            this.ResetPageMenu();
          }
          else
          {
            this.SetWallpaper(wallpaper, true);
            if (this.blockListChangedSub != null)
            {
              this.blockListChangedSub.SafeDispose();
              this.blockListChangedSub = (IDisposable) null;
            }
          }
          this.MessageList.SetMessages(msgs, false, targetLandingIndex, this.initState == null ? (MessageSearchResult) null : this.initState.SearchResult);
          this.MessageList.InitializeScrolling();
          this.inputBar.SetRecentMentionedMeJids(this.msgLoader_.GetJidsMentionedMeRecently());
          this.InitialScroll();
        }
      }));
    }

    public void OnMessageDeletion(Message msg)
    {
      this.Dispatcher.BeginInvoke((Action) (() =>
      {
        this.MessageList.RemoveUnreadDivider();
        this.MessageList.Remove(msg);
      }));
    }

    public void OnMessageInsertion(Message msg)
    {
      this.Dispatcher.BeginInvoke((Action) (() =>
      {
        if (this.shouldUpdateUnreadDivider)
        {
          if (msg.KeyFromMe)
          {
            this.MessageList.RemoveUnreadDivider();
            this.shouldUpdateUnreadDivider = false;
          }
          else
            this.shouldUpdateUnreadDivider = this.MessageList.IncreaseUnreadDivider();
        }
        this.MessageList.AddToRecent(msg);
        if (msg.KeyFromMe)
        {
          this.MessageList.ScrollToRecent("msg from self");
        }
        else
        {
          this.MessageList.ScrollFromNewMessage();
          if (this.msgLoader_ != null && this.inputBar != null)
            this.inputBar.SetRecentMentionedMeJids(this.msgLoader_.GetJidsMentionedMeRecently());
          if (this.spamReportPanel == null || !this.IsTrustedBusiness(UserCache.Get(msg.GetSenderJid(), false)))
            return;
          this.TryShowSpamReportingPanel(false, true);
        }
      }));
    }

    public void OnNewerMessages(Message[] msgs)
    {
      if (msgs == null || !((IEnumerable<Message>) msgs).Any<Message>())
        return;
      this.Dispatcher.BeginInvoke((Action) (() =>
      {
        if (this.isPageRemoved_)
          return;
        this.MessageList.AddToRecent(msgs);
      }));
      int num = this.deferMarkRead ? 1 : 0;
    }

    public void OnOlderMessages(Message[] msgs)
    {
      this.Dispatcher.BeginInvoke((Action) (() =>
      {
        if (this.isPageRemoved_)
          return;
        this.MessageList.AddToOldest(msgs);
      }));
    }

    public void OnUpdatedMessages(Message[] msgs)
    {
      this.Dispatcher.BeginInvoke((Action) (() =>
      {
        if (this.isPageRemoved_)
          return;
        this.MessageList.UpdateMessages(msgs);
      }));
    }

    public void OnDbReset(Message[] newerMsgs)
    {
      Conversation convo = (Conversation) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        this.convo_ = (Conversation) null;
        convo = this.GetConversation(db, true, "db reset");
        if (convo == null || !((IEnumerable<Message>) newerMsgs).Any<Message>())
          return;
        this.TryMarkRead(db, convo.Jid, false, new int?(), "on db reset");
        db.SubmitChanges();
      }));
      if (((IEnumerable<Message>) newerMsgs).Any<Message>())
      {
        this.MessageList.AddToRecent(newerMsgs);
        this.MessageList.ScrollFromNewMessage();
      }
      this.viewModel.Conversation = convo;
    }

    private void OnNewerMessageRealized(Message msg)
    {
      if (msg == null)
        return;
      Log.d(this.LogHeader, "newer msg realized | mid:{0}", (object) msg.MessageID);
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        Conversation conversation = this.GetConversation(db, false, "on newer msgs");
        if (conversation == null)
          return;
        int? nullable;
        if (conversation.LastMessageID.HasValue)
        {
          int messageId = msg.MessageID;
          nullable = conversation.LastMessageID;
          int valueOrDefault = nullable.GetValueOrDefault();
          if ((messageId >= valueOrDefault ? (nullable.HasValue ? 1 : 0) : 0) != 0)
          {
            this.deferMarkRead = false;
            MessagesContext db1 = db;
            string jid = conversation.Jid;
            nullable = new int?();
            int? lastReadMsgId = nullable;
            this.TryMarkRead(db1, jid, true, lastReadMsgId, "last msg realized");
            Log.d(this.LogHeader, "newer msg realized | mark till last");
            return;
          }
        }
        nullable = conversation.FirstUnreadMessageID;
        if (nullable.HasValue)
        {
          nullable = conversation.FirstUnreadMessageID;
          if (nullable.Value < msg.MessageID)
          {
            string logHeader = this.LogHeader;
            object[] objArray = new object[2]
            {
              (object) msg.MessageID,
              null
            };
            nullable = conversation.LastMessageID;
            objArray[1] = (object) (nullable ?? -1);
            Log.d(logHeader, "newer msg realized | mark till:{0},last:{1}", objArray);
            this.TryMarkRead(db, conversation.Jid, true, new int?(msg.MessageID), "newer msg realized");
          }
          else
            Log.d(this.LogHeader, "newer msg realized | no change on unread");
        }
        else
        {
          Log.d(this.LogHeader, "newer msg realized | all read now");
          this.deferMarkRead = false;
        }
      }));
    }

    public void OnMessagesRequestProcessing()
    {
      this.Dispatcher.BeginInvoke((Action) (() => this.progressIndicator.Acquire()));
    }

    public void OnMessagesRequestProcessed()
    {
      this.Dispatcher.BeginInvoke((Action) (() => this.progressIndicator.Release()));
    }

    private bool HasPendingFilesToSend => this.filePickerReturnFiles != null;

    public void LaunchGifPreview(GifSearchResult args, GifPicker picker, byte[] mp4Bytes)
    {
      if (this.inputBar.IsEmojiKeyboardOpen())
      {
        MemoryStream stream = new MemoryStream(mp4Bytes, 0, mp4Bytes.Length, false, true);
        this.Dispatcher.BeginInvoke((Action) (() =>
        {
          GifSharingState sharingState = new GifSharingState();
          sharingState.AddItem((MediaSharingState.IItem) new GifSharingState.Item(args, (Stream) stream));
          new GifPreviewFetched()
          {
            gifSearchProvider = new wam_enum_gif_search_provider?(GifProviders.Instance.GetProviderForFieldStats())
          }.SaveEventSampled(10U);
          sharingState.SearchTerm = picker.CurrentSearchTerm;
          this.LaunchPicturePreview((MediaSharingState) sharingState);
        }));
      }
      else
        new GifSearchCancelled()
        {
          gifSearchProvider = new wam_enum_gif_search_provider?(GifProviders.Instance.GetProviderForFieldStats())
        }.SaveEvent();
    }

    private string[] GetFileTypeFilter(FunXMPP.FMessage.Type waType)
    {
      string[] fileTypeFilter = (string[]) null;
      switch (waType)
      {
        case FunXMPP.FMessage.Type.Image:
          fileTypeFilter = new string[4]
          {
            ".jpg",
            ".jpeg",
            ".png",
            ".gif"
          };
          break;
        case FunXMPP.FMessage.Type.Audio:
          fileTypeFilter = new string[7]
          {
            ".mp3",
            ".wma",
            ".aac",
            ".m4a",
            ".3ga",
            ".amr",
            ".opus"
          };
          break;
        case FunXMPP.FMessage.Type.Video:
          fileTypeFilter = new string[5]
          {
            ".mp4",
            ".avi",
            ".3gp",
            ".wmv",
            ".mov"
          };
          break;
        case FunXMPP.FMessage.Type.Document:
          fileTypeFilter = new string[1]{ "*" };
          break;
      }
      return fileTypeFilter;
    }

    private void LaunchAudioPicker()
    {
      this.LaunchFilePicker((PickerLocationId) 5, this.GetFileTypeFilter(FunXMPP.FMessage.Type.Audio));
    }

    private void LaunchDocumentPicker()
    {
      this.LaunchFilePicker((PickerLocationId) 0, this.GetFileTypeFilter(FunXMPP.FMessage.Type.Document));
    }

    private void LaunchFilePicker(PickerLocationId suggestedStartLocation, string[] fileTypes)
    {
      this.savedOutTransition = TransitionService.GetNavigationOutTransition((UIElement) this);
      TransitionService.SetNavigationOutTransition((UIElement) this, (NavigationOutTransition) null);
      FileOpenPicker fileOpenPicker1 = new FileOpenPicker();
      fileOpenPicker1.put_ViewMode((PickerViewMode) 1);
      fileOpenPicker1.put_SuggestedStartLocation(suggestedStartLocation);
      FileOpenPicker fileOpenPicker2 = fileOpenPicker1;
      if (fileTypes != null)
      {
        foreach (string fileType in fileTypes)
          fileOpenPicker2.FileTypeFilter.Add(fileType);
      }
      fileOpenPicker2.PickSingleFileAndContinue();
      CoreApplicationView appView = this.appView;
      // ISSUE: method pointer
      WindowsRuntimeMarshal.AddEventHandler<TypedEventHandler<CoreApplicationView, IActivatedEventArgs>>(new Func<TypedEventHandler<CoreApplicationView, IActivatedEventArgs>, EventRegistrationToken>(appView.add_Activated), new Action<EventRegistrationToken>(appView.remove_Activated), new TypedEventHandler<CoreApplicationView, IActivatedEventArgs>((object) this, __methodptr(OnFilePickerReturnActivated)));
    }

    private void OnFilePickerReturnActivated(object sender, IActivatedEventArgs activatedArgs)
    {
      // ISSUE: method pointer
      WindowsRuntimeMarshal.RemoveEventHandler<TypedEventHandler<CoreApplicationView, IActivatedEventArgs>>(new Action<EventRegistrationToken>(this.appView.remove_Activated), new TypedEventHandler<CoreApplicationView, IActivatedEventArgs>((object) this, __methodptr(OnFilePickerReturnActivated)));
      TransitionService.SetNavigationOutTransition((UIElement) this, this.savedOutTransition);
      this.filePicker = (FileOpenPicker) null;
      if (activatedArgs.Kind != 1002 || !(activatedArgs is FileOpenPickerContinuationEventArgs continuationEventArgs) || continuationEventArgs.Files == null || !continuationEventArgs.Files.Any<StorageFile>())
        return;
      this.filePickerReturnFiles = continuationEventArgs.Files;
    }

    private void LaunchAlbumFilePicker(PickerLocationId suggestedStartLocation, string[] fileTypes)
    {
      FileOpenPicker fileOpenPicker = new FileOpenPicker();
      fileOpenPicker.put_SuggestedStartLocation((PickerLocationId) 6);
      fileOpenPicker.put_ViewMode((PickerViewMode) 1);
      if (fileTypes != null)
      {
        foreach (string fileType in fileTypes)
          fileOpenPicker.FileTypeFilter.Add(fileType);
      }
      fileOpenPicker.PickMultipleFilesAndContinue();
      CoreApplicationView appView = this.appView;
      // ISSUE: method pointer
      WindowsRuntimeMarshal.AddEventHandler<TypedEventHandler<CoreApplicationView, IActivatedEventArgs>>(new Func<TypedEventHandler<CoreApplicationView, IActivatedEventArgs>, EventRegistrationToken>(appView.add_Activated), new Action<EventRegistrationToken>(appView.remove_Activated), new TypedEventHandler<CoreApplicationView, IActivatedEventArgs>((object) this, __methodptr(OnAlbumFilePickerReturnActivated)));
    }

    private async void OnAlbumFilePickerReturnActivated(
      CoreApplicationView sender,
      IActivatedEventArgs activatedArgs)
    {
      // ISSUE: method pointer
      WindowsRuntimeMarshal.RemoveEventHandler<TypedEventHandler<CoreApplicationView, IActivatedEventArgs>>(new Action<EventRegistrationToken>(this.appView.remove_Activated), new TypedEventHandler<CoreApplicationView, IActivatedEventArgs>((object) this, __methodptr(OnAlbumFilePickerReturnActivated)));
      TransitionService.SetNavigationOutTransition((UIElement) this, this.savedOutTransition);
      this.filePicker = (FileOpenPicker) null;
      if (activatedArgs.Kind != 1002)
        return;
      FileOpenPickerContinuationEventArgs fileArgs = activatedArgs as FileOpenPickerContinuationEventArgs;
      if (fileArgs != null && fileArgs.Files != null && fileArgs.Files.Any<StorageFile>())
      {
        if (this.currentMediaPickerState == null || MediaPickerPage.preChosenChild != null)
        {
          this.currentMediaPickerState = new NativeMediaPickerState();
          if (MediaPickerPage.preChosenChild != null)
          {
            MediaPickerState.Item preChosenChild = MediaPickerPage.preChosenChild;
            NativeMediaPickerState.Item obj;
            if (preChosenChild.VideoInfo != null)
            {
              obj = new NativeMediaPickerState.Item(preChosenChild.VideoInfo);
            }
            else
            {
              StorageFile file = await StorageFile.GetFileFromPathAsync(preChosenChild.GetFullPath());
              IRandomAccessStream stream = await file.OpenAsync((FileAccessMode) 0);
              obj = new NativeMediaPickerState.Item(file, (FileRandomAccessStream) stream);
              file = (StorageFile) null;
            }
            this.currentMediaPickerState.AddItem((MediaSharingState.IItem) obj);
            MediaPickerPage.preChosenChild = (MediaPickerState.Item) null;
          }
        }
        List<StorageFile> fileList = fileArgs.Files.ToList<StorageFile>();
        for (int i = 0; i < fileList.Count; ++i)
        {
          StorageFile file = fileList[i];
          FileRandomAccessStream stream = (FileRandomAccessStream) await file.OpenAsync((FileAccessMode) 0);
          bool flag = false;
          foreach (NativeMediaPickerState.Item selectedItem in this.currentMediaPickerState.SelectedItems)
          {
            if (selectedItem.VideoInfo != null && selectedItem.VideoInfo.FullPath == file.Path || selectedItem.VideoInfo == null && selectedItem.file.FolderRelativeId == file.FolderRelativeId)
              flag = true;
          }
          if (!flag)
          {
            if (((IEnumerable<string>) this.GetFileTypeFilter(FunXMPP.FMessage.Type.Video)).Contains<string>(fileList[i].FileType))
            {
              try
              {
                StorageFile storageFile = file;
                Log.d(this.LogHeader, "getting video properties");
                VideoProperties videoPropertiesAsync = await storageFile.Properties.GetVideoPropertiesAsync();
                WaVideoArgs videoargs = new WaVideoArgs()
                {
                  FileExtension = file.FileType,
                  ContentType = file.ContentType,
                  FullPath = file.Path,
                  Duration = (int) videoPropertiesAsync.Duration.TotalSeconds,
                  Stream = ((IRandomAccessStream) stream).AsStream()
                };
                videoargs.OrientationAngle = -1;
                this.currentMediaPickerState.AddItem((MediaSharingState.IItem) new NativeMediaPickerState.Item(videoargs));
              }
              catch (Exception ex)
              {
                Log.LogException(ex, "creating video args for album video");
              }
            }
            else
              this.currentMediaPickerState.AddItem((MediaSharingState.IItem) new NativeMediaPickerState.Item(fileList[i], stream));
          }
          file = (StorageFile) null;
          stream = (FileRandomAccessStream) null;
        }
        fileList = (List<StorageFile>) null;
      }
      if (this.currentMediaPickerState != null)
        this.Dispatcher.BeginInvoke((Action) (() => this.LaunchPicturePreview((MediaSharingState) this.currentMediaPickerState)));
      fileArgs = (FileOpenPickerContinuationEventArgs) null;
    }

    private void SendFilePickerSelectedFiles()
    {
      StorageFile[] array = this.filePickerReturnFiles.ToArray<StorageFile>();
      this.filePickerReturnFiles = (IReadOnlyList<StorageFile>) null;
      if (array == null || !((IEnumerable<StorageFile>) array).Any<StorageFile>())
        return;
      StorageFile file = ((IEnumerable<StorageFile>) array).FirstOrDefault<StorageFile>();
      StorageFile storageFile = file;
      if (storageFile == null)
        return;
      UIUtils.Decision(string.Format(JidHelper.IsUserJid(this.Jid) ? AppResources.MediaSendingConfirmBody : AppResources.MediaSendingConfirmBodyGroup, (object) storageFile.DisplayName, (object) this.viewModel.Conversation.GetName()), AppResources.Yes, AppResources.Cancel).ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (confirmed =>
      {
        if (!confirmed)
          return;
        Message quotedMessage = this.inputBar.GetQuotedMessage();
        string quotedChat = this.inputBar.GetQuotedChat();
        this.inputBar.ClearQuote();
        bool uriPhoneNumberFlag = this.c2cUriPhoneNumberFlag;
        this.c2cUriPhoneNumberFlag = false;
        ExternalShare81.ShareStorageFileAsync(new List<string>()
        {
          this.Jid
        }, (IStorageFile) file, quotedMessage, quotedChat, uriPhoneNumberFlag).ToObservable<ExternalShare.ExternalShareResult>().ObserveOnDispatcher<ExternalShare.ExternalShareResult>().Subscribe<ExternalShare.ExternalShareResult>((Action<ExternalShare.ExternalShareResult>) (sent => Log.l(this.LogHeader, "send picked file | {0}", (object) sent)), (Action<Exception>) (ex =>
        {
          if (!(ex.InnerException is ExternalShareException innerException2))
            return;
          UIUtils.ShowMessageBox(AppResources.DocumentNotSentTitle, innerException2.Message).Subscribe<Unit>();
        }));
      }));
    }

    private static void OnViewportOriginYChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      if (!(d is ViewportControl viewportControl))
        return;
      viewportControl.SetViewportOrigin(new System.Windows.Point(viewportControl.Viewport.X, (double) e.NewValue));
    }

    private void MessageAdminLink_Tap()
    {
      Log.d(this.LogHeader, "message admin link tapped");
      if (this.convo_.GetAdminJids(true).Length < 1)
        Log.l(this.LogHeader, "message admin link tapped but no admins to message");
      else
        GroupParticipantPickerPage.Start(this.convo_, AppResources.MessageAdmin, AppResources.SelectAdmin, (Func<UserStatus, bool>) (u => ((IEnumerable<string>) this.convo_.GetAdminJids(true)).Contains<string>(u.Jid)), counterType: GroupParticipantPickerPage.CounterType.Participant).ObserveOnDispatcher<List<string>>().Subscribe<List<string>>((Action<List<string>>) (selJids =>
        {
          ChatPage.NextInstanceInitState = new ChatPage.InitState()
          {
            QuotedChat = this.Jid
          };
          NavUtils.NavigateToChat(selJids.FirstOrDefault<string>(), false);
        }));
    }

    public void SendSticker(Sticker sticker)
    {
      this.ValidateRecipientPreMessageSend().Take<bool>(1).ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (proceeed =>
      {
        if (!proceeed)
          return;
        Message quotedMessage = this.inputBar.GetQuotedMessage();
        string quotedChat = this.inputBar.GetQuotedChat();
        this.inputBar.ClearQuote();
        this.c2cUriPhoneNumberFlag = false;
        WhatsApp.CommonOps.SendSticker.Send(sticker, this.Jid, this.c2cUriPhoneNumberFlag, quotedMessage, quotedChat);
      }));
    }

    private void UpdateReadOnlyPanel()
    {
      this.ReadOnlyPanel.MinHeight = ResolutionHelper.ZoomMultiplier * 72.0;
      Paragraph linkInParagraph = UIUtils.TryCreateLinkInParagraph(this.viewModel.ReadOnlyHelpTextString, new Action(this.MessageAdminLink_Tap));
      linkInParagraph.TextAlignment = TextAlignment.Center;
      this.ReadOnlyHelpTextBox.Blocks.Clear();
      this.ReadOnlyHelpTextBox.Blocks.Add((Block) linkInParagraph);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/ChatPage.xaml", UriKind.Relative));
      this.ChatTitleContinuumIn = (Storyboard) this.FindName("ChatTitleContinuumIn");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.ReadOnlyPanel = (Grid) this.FindName("ReadOnlyPanel");
      this.ReadOnlyHelpTextBox = (RichTextBox) this.FindName("ReadOnlyHelpTextBox");
    }

    public enum InputMode
    {
      Undefined,
      None,
      Keyboard,
      Emoji,
      Attachment,
    }

    private enum ComposingState
    {
      None,
      Typing,
      Recording,
    }

    public class InitState : WaDisposable
    {
      public ChatPage.InputMode InputMode { get; set; } = ChatPage.InputMode.None;

      public string Title { get; set; }

      public System.Windows.Media.ImageSource Picture { get; set; }

      public MessageLoader MessageLoader { get; set; }

      public MessageSearchResult SearchResult { get; set; }

      public bool ForceInitialScrollToRecent { get; set; }

      public DeepLinkData SharedDeepLinkData { get; set; }

      public Message QuotedMessage { get; set; }

      public string QuotedChat { get; set; }

      protected override void DisposeManagedResources()
      {
        this.MessageLoader.SafeDispose();
        base.DisposeManagedResources();
      }
    }

    private enum KeyboardGapWorkaround
    {
      None,
      TranslateRootFrame,
      AdjustMargin,
    }
  }
}
