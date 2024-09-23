// Decompiled with JetBrains decompiler
// Type: WhatsApp.ChatInputBar
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Devices;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using WhatsApp.WaCollections;
using WhatsAppNative;

#nullable disable
namespace WhatsApp
{
  public class ChatInputBar : UserControl, IWaInputBar
  {
    private ChatInputBarViewModel viewModel;
    private Subject<Pair<ChatInputBar.Actions, object>> actionSubject = new Subject<Pair<ChatInputBar.Actions, object>>();
    private Subject<WaAudioArgs> recordingResultSubject = new Subject<WaAudioArgs>();
    private Subject<SIPStates> sipStateChangedSubject = new Subject<SIPStates>();
    private Subject<bool> recordingStateSubject = new Subject<bool>();
    private Subject<bool> textBoxFocusChangedSubject = new Subject<bool>();
    private string chatJid;
    private bool isGroupJid;
    private string[] recentlyMentionedMe;
    private DateTime? lastButtonClickedAt;
    private DateTime? lastVibratedAt;
    private AudioRecording recorder_;
    private List<IDisposable> disposables = new List<IDisposable>();
    private List<IDisposable> pendingAnimations = new List<IDisposable>();
    private IDisposable slideToCancelSbSub;
    private SimpleSound startRecordingSound;
    private SimpleSound stopRecordingSound;
    private SimpleSound errorRecordingSound;
    private Color? slideToCancelMaskColor;
    private bool slidingToCancel;
    private bool recordingStopIntended;
    private bool isInited;
    private bool isRecording;
    private EmojiKeyboard emojiKeyboard;
    private QuotedMessageViewPanel quotedMsgPanel;
    private MentionsListControl mentionsListPanel;
    private Rectangle expansionPanelDivider0;
    private LinkPreviewPanel linkPreviewPanel;
    private Rectangle expansionPanelDivider1;
    private FrameworkElement tooltipPanel;
    private IDisposable tooltipTimerSub;
    private string currentText = "";
    private IDisposable linkDetectionDelaySub;
    private IDisposable linkMetadataSub;
    private string currentMatchedUri;
    private WebPageMetadata currentWebPageMetadata;
    private Message currentQuotedMsg;
    private string currentQuotedChat;
    private Grid innerActionButton;
    private Image innerActionButtonIcon;
    private bool enableMentions = true;
    private TextBoxInputInterpreter textBoxInterpreter = new TextBoxInputInterpreter();
    private List<ChatInputBar.MentionState> mentions = new List<ChatInputBar.MentionState>();
    private bool startingNewMention;
    private bool skipInterpretTextSelChangeOnce;
    private bool skipInterpretTextChangedOnce;
    private ChatInputBar.MentionState recentUpdatedMention;
    private GifSendingPage GifSendingPage;
    private SIPStates currSIPState = SIPStates.Collapsed;
    private PhoneApplicationPage ownerPage;
    private TranslateTransform shiftedRootFrameTransform = new TranslateTransform();
    private bool forChatPage;
    private int stallTimeMillisecs = -1;
    private IDisposable delaySub;
    internal Grid LayoutRoot;
    internal Rectangle FooterCover;
    internal Grid ExpansionPanel;
    internal CompositeTransform ExpansionPanelTransform;
    internal Rectangle Divider;
    internal Grid RecordingPanel;
    internal CompositeTransform RecordingPanelTransform;
    internal Grid SlideToCancelPanel;
    internal AdaptiveTextBlock SlideToCancelBlock;
    internal CompositeTransform SlideToCancelBlockTransform;
    internal Rectangle SlideToCancelMask;
    internal CompositeTransform SlideToCancelMaskTransform;
    internal Grid RecordingIndicatorPanel;
    internal Rectangle RecordingIndicator;
    internal ImageBrush RecordingIndicatorIcon;
    internal TextBlock DurationBlock;
    internal Grid MainPanel;
    internal Button AttachButton;
    internal CompositeTransform AttachButtonTransform;
    internal Image AttachButtonIcon;
    internal Grid TextBoxContainer;
    internal CompositeTransform TextBoxTransform;
    internal TextBox TextBox;
    internal TextBlock TextInputTooltipBlock;
    internal Button EmojiButton;
    internal Image EmojiButtonIcon;
    internal Button RightButton;
    internal Image RightButtonIcon;
    private bool _contentLoaded;

    private string LogHeader => string.Format("input bar | jid={0}", (object) this.chatJid);

    private AudioRecording Recorder => this.recorder_ ?? (this.recorder_ = AudioRecording.Create());

    private Color SlideToCancelMaskColor
    {
      get
      {
        if (!this.slideToCancelMaskColor.HasValue)
        {
          Color color = UIUtils.BackgroundBrush.Color;
          this.slideToCancelMaskColor = new Color?(Color.FromArgb((byte) 143, color.R, color.G, color.B));
        }
        return this.slideToCancelMaskColor.Value;
      }
    }

    public bool IsRecording
    {
      get => this.isRecording;
      set
      {
        if (this.isRecording == value)
          return;
        this.isRecording = value;
        this.recordingStateSubject.OnNext(this.isRecording);
      }
    }

    public SIPStates CurrentSIPState => this.currSIPState;

    public ChatInputBar()
      : this(true)
    {
    }

    public ChatInputBar(bool forChatPage, GifSendingPage gifsendingpage = null)
    {
      this.InitializeComponent();
      this.GifSendingPage = gifsendingpage;
      this.forChatPage = forChatPage;
      this.InitOnCreation();
    }

    private void InitOnCreation()
    {
      this.DataContext = (object) (this.viewModel = new ChatInputBarViewModel());
      this.Loaded += (RoutedEventHandler) ((sender, e) => this.TryInitOnLoaded());
      this.TextBox.Tap += (EventHandler<System.Windows.Input.GestureEventArgs>) ((sender, e) => this.NotifyAction(ChatInputBar.Actions.OpenKeyboard));
      this.TextBox.Hold += (EventHandler<System.Windows.Input.GestureEventArgs>) ((sender, e) => this.NotifyAction(ChatInputBar.Actions.OpenKeyboard));
      this.RecordingPanel.Height = this.SlideToCancelPanel.Height = this.viewModel.MainPanelMinHeight;
      this.RecordingIndicatorPanel.Width = this.RecordingIndicatorPanel.Height = this.viewModel.ActionButtonSize;
      this.RecordingIndicator.Width = this.RecordingIndicator.Height = this.viewModel.ActionButtonIconSize;
      this.DurationBlock.FontSize = this.viewModel.DurationBlockFontSize;
      this.DurationBlock.Margin = this.viewModel.DurationBlockMargin;
      this.MainPanel.MinHeight = this.viewModel.MainPanelMinHeight;
      this.MainPanel.MaxHeight = this.viewModel.MainPanelMaxHeight;
      this.AttachButton.Width = this.AttachButton.Height = this.viewModel.ActionButtonSize;
      this.AttachButtonIcon.Width = this.AttachButtonIcon.Height = this.viewModel.ActionButtonIconSize;
      this.TextBox.FontSize = this.viewModel.TextBoxFontSize;
      this.TextInputTooltipBlock.Margin = this.viewModel.TextInputTooltipMargin;
      this.TextInputTooltipBlock.FontSize = this.viewModel.TextBoxFontSize;
      this.TextInputTooltipBlock.Text = this.viewModel.TextInputTooltipStr;
      this.EmojiButton.Width = this.EmojiButton.Height = this.viewModel.ActionButtonSize;
      this.EmojiButtonIcon.Width = this.EmojiButtonIcon.Height = this.viewModel.EmojiButtonIconSize;
      this.RightButton.Width = this.RightButton.Height = this.viewModel.ActionButtonSize;
      this.RightButtonIcon.Width = this.RightButtonIcon.Height = this.viewModel.ActionButtonIconSize;
      this.RightButtonIcon.FlowDirection = App.CurrentApp.RootFrame.FlowDirection;
      this.MainPanel.Margin = this.RecordingPanel.Margin = this.viewModel.MainPanelMargin;
    }

    private void TryInitOnLoaded()
    {
      if (this.isInited)
        return;
      this.isInited = true;
      this.startRecordingSound = new SimpleSound("Sounds/voice_note_start.wav", 0.5f);
      this.stopRecordingSound = new SimpleSound("Sounds/voice_note_stop.wav", 0.5f);
      this.errorRecordingSound = new SimpleSound("Sounds/voice_note_error.wav", 0.5f);
      this.disposables.Add(this.Recorder.DurationChangedSubject.ObserveOnDispatcher<Unit>().Subscribe<Unit>((Action<Unit>) (_ => this.OnRecorderDurationChanged())));
      this.disposables.Add(this.Recorder.AudioResultSubject.Subscribe<WaAudioArgs>(new Action<WaAudioArgs>(this.OnRecorderResult)));
      this.Recorder.DeviceAsyncStoppedSubject.Subscribe<Unit>((Action<Unit>) (_ => { }));
      this.disposables.Add(this.RecordingStateObservable().ObserveOnDispatcher<bool>().Subscribe<bool>(new Action<bool>(this.OnRecordingStateChanged)));
      this.SlideToCancelPanel.Margin = this.viewModel.SlideToCancelPanelMargin;
      this.SlideToCancelBlock.Text = string.Format("{0} <", (object) AppResources.SlideToCancel);
      this.SlideToCancelMask.Height = this.viewModel.MainPanelMinHeight;
      this.SlideToCancelMask.Width = 480.0;
      this.SlideToCancelMask.Fill = (Brush) new SolidColorBrush(this.SlideToCancelMaskColor);
      this.RecordingIndicatorIcon.ImageSource = this.viewModel.RecordingIndicatorIconSource;
      this.ExpansionPanel.Width = this.LayoutRoot.ActualWidth;
      this.ownerPage = UIUtils.GetVisualTreeAncestors((UIElement) this).FirstOrDefault<UIElement>((Func<UIElement, bool>) (elem => elem is PhoneApplicationPage)) as PhoneApplicationPage;
    }

    public void Clear()
    {
      this.SetText((string) null);
      this.ClearQuote();
    }

    public void Dispose()
    {
      this.AbortPendingAnimations();
      this.linkPreviewPanel.SafeDispose();
      this.disposables.ForEach((Action<IDisposable>) (d => d.SafeDispose()));
      this.disposables.Clear();
      this.tooltipTimerSub.SafeDispose();
      this.tooltipTimerSub = (IDisposable) null;
      this.startRecordingSound.SafeDispose();
      this.stopRecordingSound.SafeDispose();
      this.errorRecordingSound.SafeDispose();
      this.linkDetectionDelaySub.SafeDispose();
      this.linkDetectionDelaySub = (IDisposable) null;
      this.linkMetadataSub.SafeDispose();
      this.linkMetadataSub = (IDisposable) null;
    }

    private void NotifyAction(ChatInputBar.Actions actType, object arg = null)
    {
      this.actionSubject.OnNext(new Pair<ChatInputBar.Actions, object>(actType, arg));
    }

    public void SetText(string s)
    {
      if (s == null)
        s = "";
      this.TextBox.Text = s;
      this.currentText = s;
      this.viewModel.IsTextEmptyOrWhiteSpace = !LinkDetector.IsSendableText(s);
      this.viewModel.HasAnyText = s.Any<char>();
      this.DetectLink();
    }

    public void SetTargetJid(string jid)
    {
      this.chatJid = jid;
      this.isGroupJid = JidHelper.IsGroupJid(jid);
      this.EnableMentions(this.isGroupJid);
    }

    public void SetRecentMentionedMeJids(string[] jids)
    {
      this.recentlyMentionedMe = jids;
      if (this.mentionsListPanel == null)
        return;
      this.mentionsListPanel.RecentMentions = jids;
    }

    public ExtendedTextInputData GetInputData()
    {
      return new ExtendedTextInputData()
      {
        Text = this.GetText(),
        LinkPreviewData = this.GetLinkPreviewData(),
        MentionedJids = this.GetMentionedJids(),
        QuotedMessage = this.GetQuotedMessage(),
        QuotedChat = this.GetQuotedChat()
      };
    }

    public string GetText()
    {
      string text = this.TextBox.Text.Trim().ConvertLineEndings();
      ChatInputBar.MentionState[] array = this.mentions.Where<ChatInputBar.MentionState>((Func<ChatInputBar.MentionState, bool>) (m => m.Jid != null)).ToArray<ChatInputBar.MentionState>();
      if (((IEnumerable<ChatInputBar.MentionState>) array).Any<ChatInputBar.MentionState>())
      {
        int length1 = text.Length;
        StringBuilder stringBuilder = new StringBuilder();
        int num1 = 0;
        int startIndex = num1;
        int num2 = 0;
        ChatInputBar.MentionState mentionState = ((IEnumerable<ChatInputBar.MentionState>) array).FirstOrDefault<ChatInputBar.MentionState>();
        while (num1 < length1)
        {
          if (text[num1] == '@')
          {
            int length2 = mentionState.Text.Length;
            if (length1 - num1 >= length2 && text.IndexOf(mentionState.Text, num1, length2) >= 0)
            {
              stringBuilder.Append(text.Substring(startIndex, num1 - startIndex));
              stringBuilder.AppendFormat("@{0}", (object) JidHelper.GetPhoneNumber(mentionState.Jid, false));
              num1 = startIndex = num1 + length2;
              mentionState = ((IEnumerable<ChatInputBar.MentionState>) array).ElementAtOrDefault<ChatInputBar.MentionState>(++num2);
              if (mentionState != null)
                continue;
              break;
            }
          }
          ++num1;
        }
        stringBuilder.Append(startIndex > 0 ? text.Substring(startIndex) : text);
        text = stringBuilder.ToString();
      }
      return text;
    }

    public string[] GetMentionedJids()
    {
      return this.mentions.Where<ChatInputBar.MentionState>((Func<ChatInputBar.MentionState, bool>) (m => JidHelper.IsUserJid(m?.Jid))).Select<ChatInputBar.MentionState, string>((Func<ChatInputBar.MentionState, string>) (m => m.Jid)).ToArray<string>();
    }

    public Message GetQuotedMessage() => this.currentQuotedMsg;

    public string GetQuotedChat() => this.currentQuotedChat;

    public WebPageMetadata GetLinkPreviewData() => this.currentWebPageMetadata;

    public bool IsTextEmptyOrWhiteSpace() => this.viewModel.IsTextEmptyOrWhiteSpace;

    public bool IsEmojiKeyboardOpen() => this.viewModel.IsEmojiKeyboardOpen;

    public bool IsMentionsListOpen()
    {
      return this.mentionsListPanel != null && this.ExpansionPanel.Children.Contains((UIElement) this.mentionsListPanel);
    }

    public void SetOrientation(PageOrientation pageOrientation)
    {
      this.viewModel.OwnerPageOrientation = pageOrientation;
      this.RecordingPanel.Margin = this.MainPanel.Margin = this.viewModel.MainPanelMargin;
      if (this.linkPreviewPanel != null)
        this.linkPreviewPanel.Margin = this.viewModel.ContentMargin;
      this.ExpansionPanel.Width = this.LayoutRoot.ActualWidth;
    }

    public void RefreshTextFontSize() => this.TextBox.FontSize = this.viewModel.TextBoxFontSize;

    public IObservable<bool> TextBoxFocusChangedObservable()
    {
      return (IObservable<bool>) this.textBoxFocusChangedSubject;
    }

    public IObservable<TextChangedEventArgs> TextChangedObservable()
    {
      return this.TextBox.GetTextChangedAsync();
    }

    public IObservable<SIPStates> SIPStateChangedObservable()
    {
      return (IObservable<SIPStates>) this.sipStateChangedSubject;
    }

    private void SetSIPState(SIPStates state)
    {
      if (state == SIPStates.Undefined)
        return;
      int currSipState = (int) this.currSIPState;
      this.currSIPState = state;
      this.sipStateChangedSubject.OnNext(state);
      if (this.forChatPage)
        return;
      double num = 0.0;
      bool flag = false;
      if (!this.ownerPage.Orientation.IsLandscape() && this.currSIPState == SIPStates.EmojiKeyboard)
      {
        num = -this.GetSIPHeight();
        flag = true;
      }
      TransitionFrame rootFrame = App.CurrentApp.RootFrame;
      if (flag)
      {
        this.shiftedRootFrameTransform.Y = num;
        rootFrame.RenderTransform = (Transform) this.shiftedRootFrameTransform;
      }
      else
        rootFrame.RenderTransform = App.CurrentApp.OriginalRootFrameTransform;
    }

    public void OpenKeyboard() => this.TextBox.Focus();

    public void OpenEmojiKeyboard()
    {
      if (this.emojiKeyboard == null)
      {
        this.emojiKeyboard = new EmojiKeyboard(gifsendingpage: this.GifSendingPage, insertionTextBox: this.TextBox, showStickerPicker: true);
        this.emojiKeyboard.OwnerPage = this.ownerPage;
        this.emojiKeyboard.Opened += (EventHandler) ((sender, e) =>
        {
          this.viewModel.IsEmojiKeyboardOpen = true;
          this.EmojiButtonIcon.Source = this.viewModel.EmojiButtonIconSource;
        });
        this.emojiKeyboard.Closed += (EventHandler) ((sender, e) =>
        {
          this.viewModel.IsEmojiKeyboardOpen = false;
          this.EmojiButtonIcon.Source = this.viewModel.EmojiButtonIconSource;
        });
        this.emojiKeyboard.DismissedByBackKey += (EventHandler<EventArgs>) ((sender, e) =>
        {
          this.SetSIPState(SIPStates.Collapsed);
          this.NotifyAction(ChatInputBar.Actions.CloseEmojiKeyboard);
        });
      }
      this.emojiKeyboard.Open(ChatPage.SearchTermForNextEntrance);
      ChatPage.SearchTermForNextEntrance = (string) null;
      this.SetSIPState(SIPStates.EmojiKeyboard);
    }

    public bool IsEmojiSearchOpen()
    {
      return this.emojiKeyboard != null && this.emojiKeyboard.IsSearchOpen();
    }

    public void CloseEmojiKeyboard(SIPStates nextState = SIPStates.Undefined)
    {
      if (this.emojiKeyboard == null)
        return;
      if (nextState == SIPStates.Undefined)
        this.SetSIPState(SIPStates.Collapsed);
      else
        this.SetSIPState(nextState);
      this.emojiKeyboard.Close();
    }

    public void Enable(bool enable)
    {
      this.IsEnabled = enable;
      this.AbortPendingAnimations();
      this.EnableRecording(enable);
    }

    public void ShowAttachButton(bool show)
    {
      this.AttachButton.Visibility = show.ToVisibility();
      this.TextBoxContainer.Margin = new Thickness(show ? 0.0 : 12.0, 0.0, 0.0, 0.0);
    }

    public void EnableRecording(bool enable) => this.viewModel.EnableRecordAction = enable;

    public void EnableMentions(bool enable)
    {
      if (this.isGroupJid)
        this.enableMentions = enable;
      else
        this.enableMentions = false;
    }

    public IObservable<WaAudioArgs> RecordingResultObservable()
    {
      return (IObservable<WaAudioArgs>) this.recordingResultSubject;
    }

    public double GetSIPHeight()
    {
      return this.IsEmojiSearchOpen() ? UIUtils.SIPHeightPortrait + this.emojiKeyboard.PickerHeightVisible() + UIUtils.SIPHeightAdjustmentForWP10 : UIUtils.SIPHeightPortrait;
    }

    public IObservable<bool> RecordingStateObservable()
    {
      return (IObservable<bool>) this.recordingStateSubject;
    }

    public IObservable<Pair<ChatInputBar.Actions, object>> ActionObservable()
    {
      return (IObservable<Pair<ChatInputBar.Actions, object>>) this.actionSubject;
    }

    private void AbortPendingAnimations()
    {
      this.slideToCancelSbSub.SafeDispose();
      this.slideToCancelSbSub = (IDisposable) null;
      IDisposable[] array = this.pendingAnimations.ToArray();
      this.pendingAnimations.Clear();
      foreach (IDisposable d in array)
        d.SafeDispose();
    }

    private void StartRecording()
    {
      Log.d(this.LogHeader, "start recording");
      this.IsRecording = false;
      if (!this.viewModel.EnableRecordAction)
        return;
      bool flag = false;
      AppState.Client clientInstance = AppState.ClientInstance;
      if (clientInstance != null)
      {
        FunXMPP.Connection connection = clientInstance.GetConnection();
        if (connection != null)
          flag = connection.IsConnected;
      }
      StreamingUploadContext context = (StreamingUploadContext) null;
      if (flag && this.Recorder.ShouldStream)
      {
        if (Mms4ServerPropHelper.IsMms4EnabledForType(FunXMPP.FMessage.FunMediaType.Ptt, true))
        {
          context = (StreamingUploadContext) new StreamingUploadContextMms4();
          MediaUploadMms4.StreamMedia((StreamingUploadContextMms4) context, this.chatJid, FunXMPP.FMessage.FunMediaType.Ptt, this.Recorder.MimeType, this.Recorder.FileExtension);
        }
        else
        {
          context = new StreamingUploadContext();
          MediaUpload.StreamMedia(context, this.chatJid, FunXMPP.FMessage.FunMediaType.Ptt, this.Recorder.MimeType, this.Recorder.FileExtension);
        }
        if (!context.Active)
        {
          context.Cancel();
          context = (StreamingUploadContext) null;
        }
      }
      if (this.Recorder.Start(context))
      {
        Log.d(this.LogHeader, "recording started");
        this.IsRecording = true;
      }
      else
      {
        Log.d(this.LogHeader, "start recording failed");
        this.TerminateRecording();
        context?.Cancel();
      }
    }

    private void EndRecording(ChatInputBar.RecordingEndMode endMode)
    {
      this.recordingStopIntended = true;
      Log.d(this.LogHeader, "ending recording");
      if (!this.IsRecording)
      {
        Log.l(this.LogHeader, "skip | recording not in progress");
      }
      else
      {
        try
        {
          this.Recorder.Stop(endMode != 0);
          if (endMode != ChatInputBar.RecordingEndMode.Finished)
            FieldStats.ReportPtt(wam_enum_ptt_result_type.CANCELLED, wam_enum_ptt_source_type.FROM_CONVERSATION, (double) this.Recorder.FileSize);
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "stop recording");
        }
        this.IsRecording = false;
        Log.d(this.LogHeader, "recording ended");
      }
    }

    public void TerminateRecording()
    {
      this.EndRecording(ChatInputBar.RecordingEndMode.Terminated);
      this.CloseRecordingPanel(false);
    }

    private void CancelRecording()
    {
      this.Dispatcher.BeginInvoke((Action) (() => this.Vibrate()));
      this.CloseRecordingPanel(this.IsRecording);
      this.EndRecording(ChatInputBar.RecordingEndMode.Canceled);
    }

    private void ShowRecordingPanel()
    {
      this.AbortPendingAnimations();
      int num = this.TextBoxContainer.ActualHeight > this.viewModel.MainPanelMinHeight * 1.05 ? 1 : 0;
      Action updateUI = (Action) (() =>
      {
        this.AttachButton.Opacity = this.EmojiButton.Opacity = this.TextBoxContainer.Opacity = 0.0;
        this.RecordingPanel.Opacity = 1.0;
        this.RecordingPanelTransform.TranslateX = 0.0;
        this.TextBoxContainer.Visibility = Visibility.Collapsed;
      });
      List<DoubleAnimation> animations = new List<DoubleAnimation>();
      if (this.AttachButton.Visibility == Visibility.Visible)
        animations.Add(WaAnimations.HorizontalSlide(0.0, -400.0, TimeSpan.FromMilliseconds(200.0), (DependencyObject) this.AttachButton));
      if (num == 0)
      {
        animations.Add(WaAnimations.HorizontalSlide(0.0, -400.0, TimeSpan.FromMilliseconds(200.0), (DependencyObject) this.TextBoxContainer));
        animations.Add(WaAnimations.Fade(WaAnimations.FadeType.FadeOut, TimeSpan.FromMilliseconds(100.0), (DependencyObject) this.TextBoxContainer));
      }
      animations.Add(WaAnimations.Fade(WaAnimations.FadeType.FadeOut, TimeSpan.FromMilliseconds(100.0), (DependencyObject) this.EmojiButton));
      animations.Add(WaAnimations.Fade(WaAnimations.FadeType.FadeIn, TimeSpan.FromMilliseconds(200.0), (DependencyObject) this.RecordingPanel));
      animations.Add(WaAnimations.HorizontalSlide(480.0, 0.0, TimeSpan.FromMilliseconds(300.0), (DependencyObject) this.RecordingPanel));
      if (num != 0)
        this.TextBoxContainer.Visibility = Visibility.Collapsed;
      this.DurationBlock.Text = "0:00";
      this.SlideToCancelBlock.Opacity = 1.0;
      this.SlideToCancelBlockTransform.TranslateX = 0.0;
      Storyboard storyboard1 = WaAnimations.CreateStoryboard((IEnumerable<DoubleAnimation>) animations);
      IDisposable sub = (IDisposable) null;
      sub = Storyboarder.PerformWithDisposable(storyboard1, shouldStop: false, onComplete: (Action) (() =>
      {
        sub.SafeDispose();
        this.pendingAnimations.Remove(sub);
        updateUI();
        this.StartSlideToCancelAnimation();
        DoubleAnimation animation = WaAnimations.Fade(WaAnimations.FadeType.FadeIn, TimeSpan.FromMilliseconds(500.0), (DependencyObject) this.RecordingIndicator);
        animation.AutoReverse = true;
        Storyboard storyboard2 = WaAnimations.CreateStoryboard(animation);
        storyboard2.Duration = (Duration) TimeSpan.FromMilliseconds(1200.0);
        storyboard2.RepeatBehavior = RepeatBehavior.Forever;
        this.pendingAnimations.Add(Storyboarder.PerformWithDisposable(storyboard2, (DependencyObject) null, true, (Action) null, (Action) null, "blinking mic"));
      }), onDisposing: (Action) (() =>
      {
        this.pendingAnimations.Remove(sub);
        updateUI();
      }), context: "show recording panel");
      this.pendingAnimations.Add(sub);
    }

    private void CloseRecordingPanel(bool animate)
    {
      this.AbortPendingAnimations();
      if (!animate)
      {
        this.RestoreTextInputMode();
      }
      else
      {
        List<DoubleAnimation> animations = new List<DoubleAnimation>();
        animations.Add(WaAnimations.Fade(WaAnimations.FadeType.FadeOut, TimeSpan.FromMilliseconds(100.0), (DependencyObject) this.RecordingPanel));
        animations.Add(WaAnimations.Fade(WaAnimations.FadeType.FadeIn, TimeSpan.FromMilliseconds(200.0), (DependencyObject) this.EmojiButton));
        animations.Add(WaAnimations.HorizontalSlide(-400.0, 0.0, TimeSpan.FromMilliseconds(200.0), (DependencyObject) this.TextBoxContainer));
        if (this.AttachButton.Visibility == Visibility.Visible)
          animations.Add(WaAnimations.HorizontalSlide(-400.0, 0.0, TimeSpan.FromMilliseconds(200.0), (DependencyObject) this.AttachButton));
        animations.Add(WaAnimations.Fade(WaAnimations.FadeType.FadeIn, TimeSpan.FromMilliseconds(100.0), (DependencyObject) this.TextBoxContainer));
        this.TextBoxContainer.Opacity = 0.0;
        this.TextBoxContainer.Visibility = Visibility.Visible;
        Storyboard storyboard = WaAnimations.CreateStoryboard((IEnumerable<DoubleAnimation>) animations);
        IDisposable sub = (IDisposable) null;
        sub = Storyboarder.PerformWithDisposable(storyboard, shouldStop: false, onComplete: (Action) (() =>
        {
          this.RestoreTextInputMode();
          sub.SafeDispose();
          this.pendingAnimations.Remove(sub);
        }), callOnCompleteOnDisposing: true, context: "show recording panel");
        this.pendingAnimations.Add(sub);
      }
    }

    private void StartSlideToCancelAnimation()
    {
      if (this.slideToCancelSbSub != null)
        return;
      double num1 = 100.0 * ResolutionHelper.ZoomMultiplier;
      double num2 = 50.0 * ResolutionHelper.ZoomMultiplier;
      double actualWidth = this.SlideToCancelPanel.ActualWidth;
      double num3 = actualWidth * 2.0 + num2 * 2.0 + num1;
      double num4 = num3 - actualWidth;
      double num5 = (actualWidth + num2) / num3;
      double num6 = 1.0 - num5;
      this.SlideToCancelMask.Height = this.viewModel.MainPanelMinHeight;
      this.SlideToCancelMask.Width = num3;
      Color toCancelMaskColor = this.SlideToCancelMaskColor;
      Color color = toCancelMaskColor with { A = 0 };
      Rectangle slideToCancelMask = this.SlideToCancelMask;
      LinearGradientBrush linearGradientBrush = new LinearGradientBrush();
      GradientStopCollection gradientStopCollection = new GradientStopCollection();
      gradientStopCollection.Add(new GradientStop()
      {
        Color = toCancelMaskColor,
        Offset = 0.0
      });
      gradientStopCollection.Add(new GradientStop()
      {
        Color = toCancelMaskColor,
        Offset = num5
      });
      gradientStopCollection.Add(new GradientStop()
      {
        Color = color,
        Offset = 0.5
      });
      gradientStopCollection.Add(new GradientStop()
      {
        Color = toCancelMaskColor,
        Offset = num6
      });
      gradientStopCollection.Add(new GradientStop()
      {
        Color = toCancelMaskColor,
        Offset = 1.0
      });
      linearGradientBrush.GradientStops = gradientStopCollection;
      linearGradientBrush.StartPoint = new System.Windows.Point(0.0, 0.0);
      linearGradientBrush.EndPoint = new System.Windows.Point(1.0, 0.0);
      slideToCancelMask.Fill = (Brush) linearGradientBrush;
      double fromX = 0.0;
      double toX;
      if (UIUtils.IsRightToLeft())
      {
        toX = num4;
      }
      else
      {
        fromX = 0.0;
        toX = -num4;
      }
      Storyboard storyboard = WaAnimations.CreateStoryboard(WaAnimations.HorizontalSlide(fromX, toX, TimeSpan.FromSeconds(2.0), (DependencyObject) this.SlideToCancelMask));
      storyboard.Duration = (Duration) TimeSpan.FromSeconds(3.0);
      storyboard.RepeatBehavior = RepeatBehavior.Forever;
      this.SlideToCancelMaskTransform.TranslateX = fromX;
      this.slideToCancelSbSub = Storyboarder.PerformWithDisposable(storyboard, onComplete: (Action) (() => this.SlideToCancelMaskTransform.TranslateX = fromX), callOnCompleteOnDisposing: true, context: "slide to cancel animation");
      this.pendingAnimations.Add(this.slideToCancelSbSub);
    }

    private void RestoreTextInputMode()
    {
      this.AttachButtonTransform.TranslateX = this.TextBoxTransform.TranslateX = 0.0;
      this.AttachButton.Opacity = this.EmojiButton.Opacity = this.TextBoxContainer.Opacity = 1.0;
      this.RecordingPanel.Opacity = 0.0;
      this.TextBoxContainer.Visibility = Visibility.Visible;
    }

    private void ShowRecordingTooltip()
    {
      this.tooltipTimerSub.SafeDispose();
      this.tooltipTimerSub = (IDisposable) null;
      this.AbortPendingAnimations();
      if (this.tooltipPanel == null)
      {
        Grid grid = new Grid();
        grid.Background = (Brush) UIUtils.BlackBrush;
        grid.Height = this.viewModel.MainPanelMinHeight;
        grid.HorizontalAlignment = HorizontalAlignment.Stretch;
        grid.VerticalAlignment = VerticalAlignment.Top;
        grid.RenderTransform = (Transform) new CompositeTransform();
        grid.Opacity = 0.0;
        grid.Visibility = Visibility.Collapsed;
        Grid element = grid;
        element.Tap += (EventHandler<System.Windows.Input.GestureEventArgs>) ((sender, e) =>
        {
          this.tooltipTimerSub.SafeDispose();
          this.tooltipTimerSub = (IDisposable) null;
          this.AbortPendingAnimations();
          this.CloseRecordingTooltip();
        });
        Grid.SetRow((FrameworkElement) element, 2);
        this.LayoutRoot.Children.Add((UIElement) element);
        Rectangle rectangle1 = new Rectangle();
        rectangle1.Height = 1.0;
        rectangle1.VerticalAlignment = VerticalAlignment.Top;
        rectangle1.HorizontalAlignment = HorizontalAlignment.Stretch;
        rectangle1.Opacity = 0.35;
        rectangle1.Fill = (Brush) UIUtils.WhiteBrush;
        Rectangle rectangle2 = rectangle1;
        element.Children.Add((UIElement) rectangle2);
        TextBlock textBlock1 = new TextBlock();
        textBlock1.Foreground = (Brush) UIUtils.WhiteBrush;
        textBlock1.HorizontalAlignment = HorizontalAlignment.Center;
        textBlock1.VerticalAlignment = VerticalAlignment.Center;
        textBlock1.Text = AppResources.VoiceMessageTooltip;
        TextBlock textBlock2 = textBlock1;
        element.Children.Add((UIElement) textBlock2);
        this.tooltipPanel = (FrameworkElement) element;
      }
      if (this.tooltipPanel.RenderTransform is CompositeTransform renderTransform)
        renderTransform.TranslateY = -this.tooltipPanel.Height - 1.0;
      this.tooltipPanel.Opacity = 0.0;
      this.tooltipPanel.Visibility = Visibility.Visible;
      Storyboard storyboard = WaAnimations.CreateStoryboard(WaAnimations.Fade(WaAnimations.FadeType.FadeIn, TimeSpan.FromMilliseconds(350.0), (DependencyObject) this.tooltipPanel));
      IDisposable sub = (IDisposable) null;
      sub = Storyboarder.PerformWithDisposable(storyboard, onComplete: (Action) (() =>
      {
        sub.SafeDispose();
        this.pendingAnimations.Remove(sub);
        this.tooltipPanel.Opacity = 1.0;
        this.tooltipTimerSub.SafeDispose();
        this.tooltipTimerSub = Observable.Timer(TimeSpan.FromSeconds(4.0)).Take<long>(1).ObserveOnDispatcher<long>().Subscribe<long>((Action<long>) (_ =>
        {
          this.tooltipTimerSub.SafeDispose();
          this.tooltipTimerSub = (IDisposable) null;
          this.CloseRecordingTooltip();
        }));
      }), onDisposing: (Action) (() =>
      {
        this.tooltipPanel.Opacity = 0.0;
        this.tooltipPanel.Visibility = Visibility.Collapsed;
      }), context: "fade in tooltip");
      this.pendingAnimations.Add(sub);
    }

    private void CloseRecordingTooltip()
    {
      this.AbortPendingAnimations();
      if (this.tooltipPanel == null)
        return;
      Storyboard storyboard = WaAnimations.CreateStoryboard(WaAnimations.Fade(WaAnimations.FadeType.FadeOut, TimeSpan.FromMilliseconds(500.0), (DependencyObject) this.tooltipPanel));
      IDisposable sub = (IDisposable) null;
      sub = Storyboarder.PerformWithDisposable(storyboard, onComplete: (Action) (() =>
      {
        sub.SafeDispose();
        this.pendingAnimations.Remove(sub);
        this.tooltipPanel.Opacity = 0.0;
        this.tooltipPanel.Visibility = Visibility.Collapsed;
      }), callOnCompleteOnDisposing: true, context: "fade in tooltip");
      this.pendingAnimations.Add(sub);
    }

    private void UpdateMentionsListPanel(string prefix = "")
    {
      this.AbortPendingAnimations();
      if (!this.enableMentions)
        return;
      if (this.mentionsListPanel == null)
      {
        this.mentionsListPanel = new MentionsListControl();
        this.mentionsListPanel.CacheMode = (CacheMode) new BitmapCache();
        this.mentionsListPanel.RecentMentions = this.recentlyMentionedMe;
        this.mentionsListPanel.SetChatJid(this.chatJid);
        Grid.SetRow((FrameworkElement) this.mentionsListPanel, 0);
        Grid.SetRowSpan((FrameworkElement) this.mentionsListPanel, 1);
        this.disposables.Add(this.mentionsListPanel.GetMentionedJidObservable().ObserveOnDispatcher<string>().Subscribe<string>(new Action<string>(this.OnMentionSelected)));
      }
      this.mentionsListPanel.UpdateList(prefix);
      if (this.IsMentionsListOpen())
        return;
      this.ExpansionPanel.Children.Add((UIElement) this.mentionsListPanel);
      this.UpdateExpansionPanel();
    }

    private void ClearMentions()
    {
      this.mentions.Clear();
      this.CloseMentionsList();
    }

    public void CloseMentionsList()
    {
      this.AbortPendingAnimations();
      if (!this.IsMentionsListOpen())
        return;
      this.ExpansionPanel.Children.Remove((UIElement) this.mentionsListPanel);
      this.UpdateExpansionPanel();
    }

    private void AddMention(ChatInputBar.MentionState newMention)
    {
      int count = this.mentions.Count;
      ChatInputBar.MentionState mentionState = (ChatInputBar.MentionState) null;
      int index1 = -1;
      for (int index2 = 0; index2 < count; ++index2)
      {
        mentionState = this.mentions[index2];
        if (mentionState.Start >= newMention.Start)
        {
          index1 = index2;
          break;
        }
      }
      if (index1 < 0)
      {
        this.mentions.Add(newMention);
      }
      else
      {
        if (mentionState.Start == newMention.Start)
          return;
        this.mentions.Insert(index1, newMention);
      }
    }

    private void DeleteMention(ChatInputBar.MentionState m)
    {
      this.textBoxInterpreter.Clear();
      this.skipInterpretTextSelChangeOnce = this.skipInterpretTextChangedOnce = true;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(this.currentText.Substring(0, m.Start));
      stringBuilder.Append(this.currentText.Substring(m.Start + m.Length));
      this.TextBox.Text = stringBuilder.ToString();
      this.TextBox.SelectionStart = m.Start;
      this.TextBox.SelectionLength = 0;
      this.UpdateMentionsOnTextChange(new TextBoxInputInterpreter.TextBoxDelta(m.Start, m.Length, m.Start, 0));
    }

    private void UpdateMentionsOnTextChange(TextBoxInputInterpreter.TextBoxDelta d)
    {
      if (d == null)
        return;
      List<ChatInputBar.MentionState> source1 = new List<ChatInputBar.MentionState>();
      List<ChatInputBar.MentionState> source2 = new List<ChatInputBar.MentionState>();
      ChatInputBar.MentionState m1 = (ChatInputBar.MentionState) null;
      foreach (ChatInputBar.MentionState mention in this.mentions)
      {
        if (mention.Start + mention.Length >= d.OldSegmentStart)
        {
          if (mention.Start >= d.OldSegmentStart + d.OldSegmentLen)
            mention.Start += d.LengthDelta;
          else if (d.OldSegmentLen > 0)
          {
            if (mention.Start < d.OldSegmentStart)
            {
              if (mention.Start + mention.Length != d.NewSegmentStart)
              {
                if (d.LengthDelta < 0 && d.NewSegmentLen == 0 && mention.Start + mention.Length == d.NewSegmentStart + 1 && !mention.IsInComposing())
                {
                  --mention.Length;
                  m1 = mention;
                }
                else
                  source1.Add(mention);
              }
            }
            else
              source2.Add(mention);
          }
          else if (d.OldSegmentStart > mention.Start && d.OldSegmentStart <= mention.Start + mention.Length)
            source1.Add(mention);
        }
      }
      if (source1.Any<ChatInputBar.MentionState>())
      {
        ChatInputBar.MentionState mentionToUpdate = source1.First<ChatInputBar.MentionState>();
        this.Dispatcher.BeginInvoke((Action) (() => this.UpdateMentionComposing(mentionToUpdate)));
      }
      if (source2.Any<ChatInputBar.MentionState>())
      {
        source2.ForEach((Action<ChatInputBar.MentionState>) (m => this.mentions.Remove(m)));
        if (!source1.Any<ChatInputBar.MentionState>())
          this.CloseMentionsList();
      }
      if (m1 == null)
        return;
      this.DeleteMention(m1);
    }

    private void UpdateMentionComposing(ChatInputBar.MentionState m)
    {
      if (m == null)
        return;
      m.Jid = (string) null;
      int selectionStart = this.TextBox.SelectionStart;
      if (selectionStart <= m.Start)
      {
        this.CloseMentionsList();
      }
      else
      {
        this.UpdateMentionsListPanel(this.currentText.Substring(m.Start + 1, selectionStart - m.Start - 1));
        if (this.mentionsListPanel == null)
          return;
        if (this.mentionsListPanel.CurrentList.Any<MentionsListControl.MentionItemViewModel>())
        {
          m.Length = selectionStart - m.Start;
          this.recentUpdatedMention = m;
        }
        else
          this.CloseMentionsList();
      }
    }

    private void DetectLink()
    {
      Log.d(this.LogHeader, "detecting link");
      string link = LinkPreviewUtils.GetLink(this.currentText ?? this.TextBox.Text);
      if (link != null)
      {
        string str;
        try
        {
          str = new Uri(link).ToString();
        }
        catch (Exception ex)
        {
          string context = "Exception creating Uri for " + link;
          Log.LogException(ex, context);
          str = (string) null;
        }
        Log.d(this.LogHeader, "text:{0}, uri:{1}", (object) link, (object) (str ?? ""));
        if (!string.IsNullOrEmpty(str) && !(this.currentMatchedUri == str))
          this.StallBeforeGettingLinkMetaData(str, link);
        if (string.Compare(this.currentWebPageMetadata?.OriginalUrl, str, StringComparison.OrdinalIgnoreCase) == 0)
          return;
        this.ClearLinkPreview();
      }
      else
      {
        this.currentMatchedUri = (string) null;
        this.ClearLinkPreview();
      }
    }

    private void StallBeforeGettingLinkMetaData(string matchedUri, string matchedText)
    {
      this.currentMatchedUri = matchedUri;
      this.linkMetadataSub.SafeDispose();
      if (this.stallTimeMillisecs < 200)
      {
        this.stallTimeMillisecs = 500;
        switch (AppState.GetUserConnectionType())
        {
          case ConnectionType.Wifi:
          case ConnectionType.Cellular_3G:
            break;
          default:
            this.stallTimeMillisecs = 1000;
            break;
        }
      }
      this.delaySub.SafeDispose();
      this.delaySub = (IDisposable) null;
      this.delaySub = Observable.Timer(TimeSpan.FromMilliseconds((double) this.stallTimeMillisecs)).ObserveOnDispatcher<long>().Subscribe<long>((Action<long>) (_ =>
      {
        Log.d(this.LogHeader, "started request | uri:[{0}] | text:[{1}]", (object) matchedUri, (object) matchedText);
        this.delaySub.SafeDispose();
        this.delaySub = (IDisposable) null;
        this.linkMetadataSub = LinkPreviewUtils.FetchWebPageMetadata(matchedUri).SubscribeOn<WebPageMetadata>((IScheduler) AppState.Worker).ObserveOnDispatcher<WebPageMetadata>().Subscribe<WebPageMetadata>((Action<WebPageMetadata>) (data =>
        {
          data.OriginalUrl = matchedUri;
          data.MatchedText = matchedText;
          this.UpdateLinkPreview(data);
        }), (Action<Exception>) (ex =>
        {
          this.linkMetadataSub.SafeDispose();
          this.linkMetadataSub = (IDisposable) null;
          this.ClearLinkPreview();
        }), (Action) (() => this.linkMetadataSub = (IDisposable) null));
      }));
      Log.d(this.LogHeader, "delayed request | uri:[{0}]", (object) matchedUri);
    }

    private void UpdateLinkPreview(WebPageMetadata metadata)
    {
      bool flag1 = false;
      if (metadata == null)
      {
        flag1 = true;
        Log.d(this.LogHeader, "skip link preview update | null input | curr matched:{0}", (object) (this.currentMatchedUri ?? "n/a"));
      }
      if (this.currentMatchedUri == null)
      {
        flag1 = true;
        Log.d(this.LogHeader, "skip link preview update | null matched uri | fetched data:{0}", (object) metadata.OriginalUrl);
      }
      if (string.Compare(metadata.OriginalUrl, this.currentMatchedUri, StringComparison.OrdinalIgnoreCase) != 0)
        Log.d(this.LogHeader, "skip link preview update | outdated uri | matched:{0} fetched:{1}", (object) this.currentMatchedUri, (object) metadata.OriginalUrl);
      else if (flag1)
        this.ClearLinkPreview();
      else if (this.currentWebPageMetadata != null && string.Compare(metadata.OriginalUrl, this.currentWebPageMetadata.OriginalUrl, StringComparison.OrdinalIgnoreCase) != 0)
      {
        Log.d(this.LogHeader, "skip link preview update | same uri: {0}", (object) this.currentMatchedUri);
      }
      else
      {
        this.currentWebPageMetadata = metadata;
        bool flag2 = false;
        if (this.linkPreviewPanel == null)
        {
          LinkPreviewPanel linkPreviewPanel = new LinkPreviewPanel(ResolutionHelper.ZoomMultiplier);
          linkPreviewPanel.CacheMode = (CacheMode) new BitmapCache();
          linkPreviewPanel.Margin = this.viewModel.ContentMargin;
          this.linkPreviewPanel = linkPreviewPanel;
          this.linkPreviewPanel.DismissButton.Click += (RoutedEventHandler) ((sender, e) => this.ClearLinkPreview());
          Grid.SetRow((FrameworkElement) this.linkPreviewPanel, 1);
        }
        else if (this.ExpansionPanel.Children.Contains((UIElement) this.linkPreviewPanel))
          flag2 = true;
        this.linkPreviewPanel.Update(metadata);
        if (flag2)
          return;
        this.GetExpansionPanelExpectedHeight();
        this.ExpansionPanel.Children.Insert(0, (UIElement) this.linkPreviewPanel);
        this.UpdateExpansionPanel();
      }
    }

    private void ClearLinkPreview()
    {
      this.currentWebPageMetadata.SafeDispose();
      this.currentWebPageMetadata = (WebPageMetadata) null;
      this.AbortPendingAnimations();
      if (this.linkPreviewPanel == null || !this.ExpansionPanel.Children.Contains((UIElement) this.linkPreviewPanel))
        return;
      this.ExpansionPanel.Children.Remove((UIElement) this.linkPreviewPanel);
      this.UpdateExpansionPanel();
    }

    public void SetQuotedChat(string groupJid)
    {
      this.AbortPendingAnimations();
      if (this.currentQuotedChat == groupJid)
        return;
      string quotedName = (string) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db => quotedName = db.GetConversation(groupJid, CreateOptions.None)?.GroupSubject));
      if (string.IsNullOrEmpty(quotedName))
        return;
      bool flag = false;
      if (this.quotedMsgPanel == null)
      {
        this.quotedMsgPanel = MessageViewPanel.Get(MessageViewPanel.ViewTypes.Quote) as QuotedMessageViewPanel;
        this.quotedMsgPanel.CacheMode = (CacheMode) new BitmapCache();
        this.quotedMsgPanel.Margin = this.viewModel.ContentMargin;
        this.quotedMsgPanel.IsComposing = true;
        this.disposables.Add(this.quotedMsgPanel.DimissedObservable().ObserveOnDispatcher<Unit>().Subscribe<Unit>((Action<Unit>) (_ => this.ClearQuote())));
        Grid.SetRow((FrameworkElement) this.quotedMsgPanel, 0);
      }
      else if (this.ExpansionPanel.Children.Contains((UIElement) this.quotedMsgPanel))
        flag = true;
      this.quotedMsgPanel.Render(groupJid);
      this.currentQuotedChat = groupJid;
      if (!flag)
        this.ExpansionPanel.Children.Insert(0, (UIElement) this.quotedMsgPanel);
      this.UpdateExpansionPanel();
    }

    public void SetQuotedMessage(MessageViewModel mvm)
    {
      this.AbortPendingAnimations();
      if (mvm?.Message == null)
      {
        this.ClearQuote();
      }
      else
      {
        if (this.currentQuotedMsg == mvm.Message)
          return;
        bool flag = false;
        if (this.quotedMsgPanel == null)
        {
          this.quotedMsgPanel = MessageViewPanel.Get(MessageViewPanel.ViewTypes.Quote) as QuotedMessageViewPanel;
          this.quotedMsgPanel.CacheMode = (CacheMode) new BitmapCache();
          this.quotedMsgPanel.Margin = this.viewModel.ContentMargin;
          this.quotedMsgPanel.IsComposing = true;
          this.disposables.Add(this.quotedMsgPanel.DimissedObservable().ObserveOnDispatcher<Unit>().Subscribe<Unit>((Action<Unit>) (_ => this.ClearQuote())));
          Grid.SetRow((FrameworkElement) this.quotedMsgPanel, 0);
        }
        else if (this.ExpansionPanel.Children.Contains((UIElement) this.quotedMsgPanel))
          flag = true;
        this.quotedMsgPanel.Render(mvm, true);
        this.currentQuotedMsg = mvm.Message;
        if (!flag)
          this.ExpansionPanel.Children.Insert(0, (UIElement) this.quotedMsgPanel);
        this.UpdateExpansionPanel();
      }
    }

    public void ClearQuote()
    {
      this.currentQuotedMsg = (Message) null;
      this.currentQuotedChat = (string) null;
      this.AbortPendingAnimations();
      if (this.quotedMsgPanel == null || !this.ExpansionPanel.Children.Contains((UIElement) this.quotedMsgPanel))
        return;
      this.ExpansionPanel.Children.Remove((UIElement) this.quotedMsgPanel);
      this.UpdateExpansionPanel();
    }

    private double GetExpansionPanelExpectedHeight()
    {
      double panelExpectedHeight = 0.0;
      if (this.IsMentionsListOpen())
      {
        panelExpectedHeight = this.mentionsListPanel.LayoutRoot.Height;
      }
      else
      {
        if (this.linkPreviewPanel != null && this.ExpansionPanel.Children.Contains((UIElement) this.linkPreviewPanel))
          panelExpectedHeight += (double) LinkPreviewPanel.ThumbsizeBase;
        if (this.quotedMsgPanel != null && this.ExpansionPanel.Children.Contains((UIElement) this.quotedMsgPanel))
          panelExpectedHeight += this.quotedMsgPanel.PreviewExpansionHeight;
      }
      return panelExpectedHeight;
    }

    private void UpdateExpansionPanel()
    {
      if (this.ExpansionPanel.Children.Any<UIElement>())
      {
        this.ExpansionPanel.Visibility = Visibility.Visible;
        if (this.expansionPanelDivider0 == null)
        {
          Rectangle rectangle = new Rectangle();
          rectangle.Height = 1.0;
          rectangle.VerticalAlignment = VerticalAlignment.Top;
          rectangle.HorizontalAlignment = HorizontalAlignment.Stretch;
          rectangle.Opacity = 0.35;
          rectangle.Fill = (Brush) UIUtils.ForegroundBrush;
          this.expansionPanelDivider0 = rectangle;
          Grid.SetRow((FrameworkElement) this.expansionPanelDivider0, 0);
        }
        if (!this.ExpansionPanel.Children.Contains((UIElement) this.expansionPanelDivider0))
          this.ExpansionPanel.Children.Add((UIElement) this.expansionPanelDivider0);
        if (this.linkPreviewPanel != null && this.ExpansionPanel.Children.Contains((UIElement) this.linkPreviewPanel) && this.quotedMsgPanel != null && this.ExpansionPanel.Children.Contains((UIElement) this.quotedMsgPanel))
        {
          if (this.expansionPanelDivider1 == null)
          {
            Rectangle rectangle = new Rectangle();
            rectangle.Height = 1.0;
            rectangle.VerticalAlignment = VerticalAlignment.Top;
            rectangle.HorizontalAlignment = HorizontalAlignment.Stretch;
            rectangle.Opacity = 0.35;
            rectangle.Fill = (Brush) UIUtils.ForegroundBrush;
            this.expansionPanelDivider1 = rectangle;
            Grid.SetRow((FrameworkElement) this.expansionPanelDivider1, 1);
          }
          if (!this.ExpansionPanel.Children.Contains((UIElement) this.expansionPanelDivider1))
            this.ExpansionPanel.Children.Add((UIElement) this.expansionPanelDivider1);
        }
        else if (this.expansionPanelDivider1 != null && this.ExpansionPanel.Children.Contains((UIElement) this.expansionPanelDivider1))
          this.ExpansionPanel.Children.Remove((UIElement) this.expansionPanelDivider1);
        double panelExpectedHeight = this.GetExpansionPanelExpectedHeight();
        if (!string.IsNullOrEmpty(this.currentQuotedChat))
          panelExpectedHeight /= 2.0;
        this.ExpansionPanel.Height = panelExpectedHeight;
        this.ExpansionPanelTransform.TranslateY = -panelExpectedHeight;
      }
      else
      {
        this.ExpansionPanel.Visibility = Visibility.Collapsed;
        this.ExpansionPanelTransform.TranslateY = 0.0;
      }
    }

    private bool ShouldBlockFrequentButtonClick()
    {
      bool flag = false;
      DateTime utcNow = DateTime.UtcNow;
      if (this.lastButtonClickedAt.HasValue && (utcNow - this.lastButtonClickedAt.Value).TotalMilliseconds < 350.0)
        flag = true;
      else
        this.lastButtonClickedAt = new DateTime?(utcNow);
      return flag;
    }

    private void Vibrate()
    {
      DateTime utcNow = DateTime.UtcNow;
      if (this.lastVibratedAt.HasValue && (utcNow - this.lastVibratedAt.Value).TotalMilliseconds < 500.0)
        Log.d(this.LogHeader, "skipped vibration");
      else
        VibrateController.Default.Start(TimeSpan.FromMilliseconds(75.0));
      this.lastVibratedAt = new DateTime?(utcNow);
    }

    private void AttachButton_Click(object sender, RoutedEventArgs e)
    {
      if (this.ShouldBlockFrequentButtonClick())
        return;
      this.NotifyAction(ChatInputBar.Actions.Attach);
    }

    private void EmojiButton_Click(object sender, RoutedEventArgs e)
    {
      if (this.ShouldBlockFrequentButtonClick())
        return;
      ChatInputBar.Actions actType = this.viewModel.IsEmojiKeyboardOpen ? ChatInputBar.Actions.OpenKeyboard : ChatInputBar.Actions.OpenEmojiKeyboard;
      this.NotifyAction(actType);
      if (this.forChatPage)
        return;
      if (actType == ChatInputBar.Actions.OpenEmojiKeyboard)
        this.OpenEmojiKeyboard();
      else
        this.OpenKeyboard();
    }

    private void RightButton_Click(object sender, RoutedEventArgs e)
    {
      if (this.ShouldBlockFrequentButtonClick() || this.viewModel.RightButtonType != ChatInputBar.Actions.SendText || !LinkDetector.IsSendableText((this.TextBox.Text ?? "").Trim()))
        return;
      this.NotifyAction(ChatInputBar.Actions.SendText, (object) this.GetInputData());
    }

    private void TextBox_KeyDown(object sender, KeyEventArgs e)
    {
      TimeSpentManager.GetInstance().UserAction();
      if (e.Key == System.Windows.Input.Key.Back)
      {
        EmojiPickerViewModel.InsertBackspace(this.TextBox, ref this.currentText);
      }
      else
      {
        if (e.Key != System.Windows.Input.Key.Enter || !Settings.EnterKeyIsSend)
          return;
        e.Handled = true;
        if (!LinkDetector.IsSendableText((this.TextBox.Text ?? "").Trim()))
          return;
        this.NotifyAction(ChatInputBar.Actions.SendText, (object) this.GetInputData());
      }
    }

    private void TextBox_KeyUp(object sender, KeyEventArgs e)
    {
    }

    private void TextBox_GotFocus(object sender, RoutedEventArgs e)
    {
      this.CloseEmojiKeyboard(SIPStates.Keyboard);
      if (ImageStore.IsDarkTheme() && sender is TextBox textBox)
      {
        textBox.Background = (Brush) UIUtils.BackgroundBrush;
        textBox.Foreground = (Brush) UIUtils.ForegroundBrush;
        textBox.CaretBrush = (Brush) new SolidColorBrush(Colors.White);
      }
      this.viewModel.IsTextBoxFocused = true;
      this.UpdateRightButtonVisibleState(this.viewModel.IsTextBoxFocused);
      this.textBoxFocusChangedSubject.OnNext(true);
      this.SetSIPState(SIPStates.Keyboard);
    }

    private void TextBox_LostFocus(object sender, RoutedEventArgs e)
    {
      this.viewModel.IsTextBoxFocused = false;
      this.UpdateRightButtonVisibleState(this.viewModel.IsTextBoxFocused);
      this.CloseMentionsList();
      this.textBoxFocusChangedSubject.OnNext(false);
      this.SetSIPState(SIPStates.Collapsed);
    }

    private void TextBox_SelectionChanged(object sender, RoutedEventArgs e)
    {
      if (!this.enableMentions)
        return;
      if (this.skipInterpretTextSelChangeOnce)
      {
        this.skipInterpretTextSelChangeOnce = false;
      }
      else
      {
        if (this.mentions.Any<ChatInputBar.MentionState>() && this.viewModel.IsTextBoxFocused)
        {
          TextBoxInputInterpreter.TextBoxDelta d = this.textBoxInterpreter.PushSelectionChangedEvent(this.TextBox.SelectionStart, this.TextBox.SelectionLength);
          if (d == null)
          {
            if (this.recentUpdatedMention != null)
            {
              int selectionStart = this.TextBox.SelectionStart;
              if (selectionStart < this.recentUpdatedMention.Start || selectionStart > this.recentUpdatedMention.Start + this.recentUpdatedMention.Length + 1)
              {
                this.recentUpdatedMention = (ChatInputBar.MentionState) null;
                this.CloseMentionsList();
              }
            }
          }
          else
            this.UpdateMentionsOnTextChange(d);
        }
        if (!this.startingNewMention)
          return;
        this.startingNewMention = false;
        this.AddMention(new ChatInputBar.MentionState()
        {
          Jid = (string) null,
          Text = "@",
          Start = this.TextBox.SelectionStart - 1,
          Length = 1
        });
        this.UpdateMentionsListPanel();
      }
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      string currentText = this.currentText;
      this.currentText = this.TextBox.Text;
      if (this.skipInterpretTextChangedOnce)
        this.skipInterpretTextChangedOnce = false;
      else if (this.enableMentions && this.viewModel.IsTextBoxFocused)
      {
        int lengthDelta = this.currentText.Length - currentText.Length;
        if (this.mentions.Any<ChatInputBar.MentionState>())
          this.textBoxInterpreter.PushTextChangedEvent(lengthDelta);
        if (lengthDelta == 1 && this.currentText[this.TextBox.SelectionStart - 1] == '@')
        {
          Log.d(this.LogHeader, "starting a mention");
          this.startingNewMention = true;
        }
      }
      bool flag = string.IsNullOrWhiteSpace(this.currentText);
      this.viewModel.IsTextEmptyOrWhiteSpace = flag;
      this.viewModel.HasAnyText = this.currentText.Any<char>();
      this.linkDetectionDelaySub.SafeDispose();
      this.linkDetectionDelaySub = (IDisposable) null;
      if (flag)
      {
        this.ClearLinkPreview();
        this.ClearMentions();
      }
      else
        this.linkDetectionDelaySub = Observable.Timer(TimeSpan.FromMilliseconds(350.0)).ObserveOnDispatcher<long>().Subscribe<long>((Action<long>) (_ => this.DetectLink()));
    }

    public void UpdateRightButtonVisibleState(bool textBoxFocused)
    {
      if (this.innerActionButton != null)
      {
        this.innerActionButton.Opacity = textBoxFocused ? 1.0 : 0.0;
        this.innerActionButton.IsHitTestVisible = textBoxFocused;
      }
      this.RightButton.Opacity = textBoxFocused ? 0.0 : 1.0;
      this.RightButton.IsHitTestVisible = !textBoxFocused;
    }

    private void RightButton_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
    {
      this.IsRecording = false;
      this.recordingStopIntended = false;
      this.slidingToCancel = false;
      if (this.viewModel.RightButtonType != ChatInputBar.Actions.Record || !this.viewModel.EnableRecordAction)
        return;
      if (!this.Recorder.IsReady)
      {
        Log.l(this.LogHeader, "manipulation start | skip | recorder not ready");
        this.TerminateRecording();
      }
      else
      {
        if (this.startRecordingSound != null)
          this.startRecordingSound.Play();
        this.Dispatcher.BeginInvoke((Action) (() =>
        {
          this.Vibrate();
          if (this.recordingStopIntended)
            Log.l(this.LogHeader, "skip show rec panel | stop intended already");
          else
            this.ShowRecordingPanel();
        }));
        this.Dispatcher.RunAfterDelay(TimeSpan.FromMilliseconds(250.0), (Action) (() =>
        {
          if (this.recordingStopIntended)
          {
            Log.l(this.LogHeader, "skip rec | stop intended already");
            if (this.errorRecordingSound != null)
              this.errorRecordingSound.Play();
            this.ShowRecordingTooltip();
          }
          else
            this.StartRecording();
        }));
      }
    }

    private void RightButton_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
    {
      if (this.viewModel.RightButtonType != ChatInputBar.Actions.Record || !this.viewModel.EnableRecordAction)
      {
        this.RestoreTextInputMode();
      }
      else
      {
        this.CloseRecordingPanel(this.IsRecording);
        this.EndRecording(ChatInputBar.RecordingEndMode.Finished);
      }
    }

    private void RightButton_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
    {
      if (this.viewModel.RightButtonType != ChatInputBar.Actions.Record || !this.viewModel.EnableRecordAction || this.recordingStopIntended || !this.IsRecording)
        return;
      bool flag = App.CurrentApp.RootFrame.FlowDirection == FlowDirection.RightToLeft;
      double num1 = (e.ManipulationOrigin.X + e.CumulativeManipulation.Translation.X) * (flag ? -1.0 : 1.0);
      int num2 = this.viewModel.OwnerPageOrientation.IsLandscape() ? -500 : -300;
      int num3 = -60;
      bool slidingToCancel = this.slidingToCancel;
      this.slidingToCancel = num1 < (double) num3;
      if (this.slidingToCancel)
      {
        this.slideToCancelSbSub.SafeDispose();
        this.slideToCancelSbSub = (IDisposable) null;
        if (!slidingToCancel)
          this.SlideToCancelMask.Fill = (Brush) new SolidColorBrush(this.SlideToCancelMaskColor);
        this.SlideToCancelBlock.Opacity = Math.Min(1.0, Math.Max(1.0 - (num1 - (double) num3) / (double) (num2 - num3), 0.0));
        this.SlideToCancelBlockTransform.TranslateX = (num1 - (double) num3) * 0.6;
      }
      else
      {
        this.SlideToCancelBlock.Opacity = 1.0;
        this.SlideToCancelBlockTransform.TranslateX = 0.0;
        this.StartSlideToCancelAnimation();
      }
      if (num1 >= (double) num2)
        return;
      this.CancelRecording();
    }

    private void OnRecorderResult(WaAudioArgs args)
    {
      bool tooShort = args.Duration < 1;
      this.Dispatcher.BeginInvoke((Action) (() => (tooShort ? this.errorRecordingSound : this.stopRecordingSound)?.Play()));
      this.Dispatcher.BeginInvoke((Action) (() => this.Vibrate()));
      if (tooShort)
      {
        Log.d(this.LogHeader, "recording too short | duration:{0}", (object) args.Duration);
        this.Dispatcher.BeginInvoke((Action) (() => this.ShowRecordingTooltip()));
        FieldStats.ReportPtt(wam_enum_ptt_result_type.TOO_SHORT, wam_enum_ptt_source_type.FROM_CONVERSATION, (double) args.FileSize);
      }
      else
      {
        Log.d(this.LogHeader, "notify recording result | duration:{0}", (object) args.Duration);
        this.recordingResultSubject.OnNext(args);
        FieldStats.ReportPtt(wam_enum_ptt_result_type.SENT, wam_enum_ptt_source_type.FROM_CONVERSATION, (double) args.FileSize);
      }
    }

    private void OnRecorderDurationChanged()
    {
      if (!this.IsRecording || this.recordingStopIntended)
        return;
      if (this.Recorder.Duration > 120 && this.Recorder.FileSize > (ulong) Settings.MaxMediaSize)
      {
        Log.l(this.LogHeader, "hit voice message size limit | size:{0}", (object) this.Recorder.FileSize);
        this.CloseRecordingPanel(this.IsRecording);
        this.EndRecording(ChatInputBar.RecordingEndMode.Finished);
      }
      else
        this.DurationBlock.Text = DateTimeUtils.FormatDuration(this.Recorder.Duration);
    }

    private void OnRecordingStateChanged(bool isRecording)
    {
      this.viewModel.IsRecordingInProgress = isRecording;
    }

    private void InnerAction_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      this.RightButton_Click((object) null, (RoutedEventArgs) null);
    }

    private void InnerSpacer_Loaded(object sender, RoutedEventArgs e)
    {
      (sender as Rectangle).Width = this.viewModel.ActionButtonSize;
    }

    private void InnerAction_Loaded(object sender, RoutedEventArgs e)
    {
      this.innerActionButton = sender as Grid;
      if (this.innerActionButton != null)
      {
        this.innerActionButton.Visibility = Visibility.Visible;
        this.innerActionButton.Width = this.innerActionButton.Height = this.viewModel.ActionButtonSize;
        this.innerActionButton.Margin = new Thickness(0.0, -3.0, 0.0, -3.0);
        this.innerActionButtonIcon = this.innerActionButton.Children.FirstOrDefault<UIElement>() as Image;
        if (this.innerActionButtonIcon != null)
        {
          this.innerActionButtonIcon.Width = this.innerActionButtonIcon.Height = this.viewModel.ActionButtonIconSize;
          this.innerActionButtonIcon.FlowDirection = App.CurrentApp.RootFrame.FlowDirection;
        }
      }
      this.UpdateRightButtonVisibleState(this.viewModel.IsTextBoxFocused);
    }

    private void OnMentionSelected(string mentionedJid)
    {
      this.Dispatcher.BeginInvoke((Action) (() => this.CloseMentionsList()));
      int selectionStart = this.TextBox.SelectionStart;
      ChatInputBar.MentionState newMention = (ChatInputBar.MentionState) null;
      foreach (ChatInputBar.MentionState mention in this.mentions)
      {
        if (mention.IsInComposing() && selectionStart > mention.Start && selectionStart <= mention.Start + mention.Length)
        {
          newMention = mention;
          break;
        }
      }
      if (newMention == null || newMention.Start >= this.currentText.Length)
        return;
      this.mentions.Remove(newMention);
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(this.currentText.Substring(0, newMention.Start));
      string str = string.Format("@{0}", (object) JidHelper.GetDisplayNameForContactJid(mentionedJid));
      stringBuilder.Append(str);
      stringBuilder.Append(" ");
      if (selectionStart < this.currentText.Length)
        stringBuilder.Append(this.currentText.Substring(selectionStart));
      this.TextBox.Text = stringBuilder.ToString();
      this.TextBox.SelectionStart = newMention.Start + str.Length + 1;
      this.TextBox.SelectionLength = 0;
      this.UpdateMentionsOnTextChange(new TextBoxInputInterpreter.TextBoxDelta(newMention.Start, selectionStart - newMention.Start, newMention.Start, str.Length + 1));
      newMention.Jid = mentionedJid;
      newMention.Text = str;
      newMention.Length = str.Length;
      this.AddMention(newMention);
      this.Dispatcher.BeginInvoke((Action) (() => this.OpenKeyboard()));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/ChatInputBar.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.FooterCover = (Rectangle) this.FindName("FooterCover");
      this.ExpansionPanel = (Grid) this.FindName("ExpansionPanel");
      this.ExpansionPanelTransform = (CompositeTransform) this.FindName("ExpansionPanelTransform");
      this.Divider = (Rectangle) this.FindName("Divider");
      this.RecordingPanel = (Grid) this.FindName("RecordingPanel");
      this.RecordingPanelTransform = (CompositeTransform) this.FindName("RecordingPanelTransform");
      this.SlideToCancelPanel = (Grid) this.FindName("SlideToCancelPanel");
      this.SlideToCancelBlock = (AdaptiveTextBlock) this.FindName("SlideToCancelBlock");
      this.SlideToCancelBlockTransform = (CompositeTransform) this.FindName("SlideToCancelBlockTransform");
      this.SlideToCancelMask = (Rectangle) this.FindName("SlideToCancelMask");
      this.SlideToCancelMaskTransform = (CompositeTransform) this.FindName("SlideToCancelMaskTransform");
      this.RecordingIndicatorPanel = (Grid) this.FindName("RecordingIndicatorPanel");
      this.RecordingIndicator = (Rectangle) this.FindName("RecordingIndicator");
      this.RecordingIndicatorIcon = (ImageBrush) this.FindName("RecordingIndicatorIcon");
      this.DurationBlock = (TextBlock) this.FindName("DurationBlock");
      this.MainPanel = (Grid) this.FindName("MainPanel");
      this.AttachButton = (Button) this.FindName("AttachButton");
      this.AttachButtonTransform = (CompositeTransform) this.FindName("AttachButtonTransform");
      this.AttachButtonIcon = (Image) this.FindName("AttachButtonIcon");
      this.TextBoxContainer = (Grid) this.FindName("TextBoxContainer");
      this.TextBoxTransform = (CompositeTransform) this.FindName("TextBoxTransform");
      this.TextBox = (TextBox) this.FindName("TextBox");
      this.TextInputTooltipBlock = (TextBlock) this.FindName("TextInputTooltipBlock");
      this.EmojiButton = (Button) this.FindName("EmojiButton");
      this.EmojiButtonIcon = (Image) this.FindName("EmojiButtonIcon");
      this.RightButton = (Button) this.FindName("RightButton");
      this.RightButtonIcon = (Image) this.FindName("RightButtonIcon");
    }

    public enum Actions
    {
      Undefined,
      SendText,
      Attach,
      Record,
      OpenKeyboard,
      OpenEmojiKeyboard,
      CloseEmojiKeyboard,
    }

    public enum RecordingEndMode
    {
      Finished,
      Canceled,
      Terminated,
    }

    private class MentionState
    {
      public string Jid;
      public string Text;
      public int Start;
      public int Length;

      public bool IsInComposing() => this.Jid == null;
    }
  }
}
