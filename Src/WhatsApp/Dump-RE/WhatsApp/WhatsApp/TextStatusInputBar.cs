// Decompiled with JetBrains decompiler
// Type: WhatsApp.TextStatusInputBar
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WhatsApp.WaCollections;
using WhatsAppNative;

#nullable disable
namespace WhatsApp
{
  public class TextStatusInputBar : UserControl, IWaInputBar
  {
    private TextStatusInputBarViewModel viewModel;
    private Subject<Pair<TextStatusInputBar.Actions, object>> actionSubject = new Subject<Pair<TextStatusInputBar.Actions, object>>();
    private Subject<SIPStates> sipStateChangedSubject = new Subject<SIPStates>();
    private Subject<Unit> linkDataUpdatedSubject = new Subject<Unit>();
    private const string LogHeader = "status input bar";
    private DateTime? lastButtonClickedAt;
    private List<IDisposable> disposables = new List<IDisposable>();
    private List<IDisposable> pendingAnimations = new List<IDisposable>();
    private bool isInited;
    private EmojiKeyboard emojiKeyboard;
    private string currentText = "";
    private IDisposable linkDetectionDelaySub;
    private IDisposable linkMetadataSub;
    private string currentMatchedUri;
    private WebPageMetadata currentWebPageMetadata;
    private SIPStates currSIPState = SIPStates.Collapsed;
    private PhoneApplicationPage ownerPage;
    private TranslateTransform shiftedRootFrameTransform = new TranslateTransform();
    private int stallTimeMillisecs = -1;
    private IDisposable delaySub;
    internal Grid LayoutRoot;
    internal Grid MainPanel;
    internal Button EmojiButton;
    internal Image EmojiButtonIcon;
    internal TextBox TextBox;
    private bool _contentLoaded;

    public SIPStates CurrentSIPState => this.currSIPState;

    public Brush TextBackground
    {
      get => this.viewModel.TextBackground;
      set => this.viewModel.TextBackground = value;
    }

    public WhatsApp.ProtoBuf.Message.ExtendedTextMessage.FontType Font
    {
      get => this.viewModel.Font;
      set => this.viewModel.Font = value;
    }

    public TextStatusInputBar()
    {
      this.InitializeComponent();
      this.InitOnCreation();
    }

    private void InitOnCreation()
    {
      this.DataContext = (object) (this.viewModel = new TextStatusInputBarViewModel(false));
      this.Loaded += (RoutedEventHandler) ((sender, e) => this.TryInitOnLoaded());
      this.MainPanel.Margin = this.viewModel.ContentMargin;
    }

    private void TryInitOnLoaded()
    {
      if (this.isInited)
        return;
      this.isInited = true;
      this.ownerPage = UIUtils.GetVisualTreeAncestors((UIElement) this).FirstOrDefault<UIElement>((Func<UIElement, bool>) (elem => elem is PhoneApplicationPage)) as PhoneApplicationPage;
    }

    public void Clear() => this.SetText((string) null);

    public void Dispose()
    {
      this.AbortPendingAnimations();
      this.disposables.ForEach((Action<IDisposable>) (d => d.SafeDispose()));
      this.disposables.Clear();
      this.linkDetectionDelaySub.SafeDispose();
      this.linkDetectionDelaySub = (IDisposable) null;
      this.linkMetadataSub.SafeDispose();
      this.linkMetadataSub = (IDisposable) null;
    }

    private void NotifyAction(TextStatusInputBar.Actions actType, object arg = null)
    {
      this.actionSubject.OnNext(new Pair<TextStatusInputBar.Actions, object>(actType, arg));
    }

    public void SetText(string s)
    {
      if (s == null)
        s = "";
      this.TextBox.Text = s;
      this.currentText = s;
      this.viewModel.IsTextEmptyOrWhiteSpace = string.IsNullOrWhiteSpace(s);
      this.viewModel.HasAnyText = s.Any<char>();
      this.DetectLink();
    }

    public ExtendedTextInputData GetInputData()
    {
      return new ExtendedTextInputData()
      {
        Text = this.GetText(),
        LinkPreviewData = this.GetLinkPreviewData()
      };
    }

    public RichTextBlock.TextSet GetRichText()
    {
      string s = this.GetText();
      IEnumerable<WaRichText.Chunk> chunks = (IEnumerable<WaRichText.Chunk>) null;
      if (this.currentWebPageMetadata != null && !string.IsNullOrEmpty(this.currentWebPageMetadata.MatchedText))
      {
        int num = s.IndexOf(this.currentWebPageMetadata.MatchedText);
        if (num >= 0)
        {
          string renderLinkText = WaStatusViewControl.ShrinkLink(this.currentWebPageMetadata.MatchedText);
          if (num > 0 && s.Substring(num - 1, 1) != "\n")
          {
            s = s.Substring(0, num) + "\n" + s.Substring(num);
            ++num;
          }
          chunks = (IEnumerable<WaRichText.Chunk>) new WaRichText.Chunk[1]
          {
            new WaRichText.Chunk(num, this.currentWebPageMetadata.MatchedText.Length, WaRichText.Formats.Link, this.currentWebPageMetadata.CanonicalUrl ?? this.currentWebPageMetadata.MatchedText, (Func<string>) (() => renderLinkText), (Action) (() => { }))
          };
        }
      }
      WaRichText.Formats applicableFormats = WaRichText.Formats.TextFormattings | WaRichText.Formats.Emoji;
      IEnumerable<LinkDetector.Result> matches = LinkDetector.GetMatches(s, new WaRichText.DetectionArgs(applicableFormats));
      return new RichTextBlock.TextSet()
      {
        Text = s,
        SerializedFormatting = matches,
        PartialFormattings = chunks
      };
    }

    public string GetText() => this.TextBox.Text.Trim().ConvertLineEndings();

    public WebPageMetadata GetLinkPreviewData() => this.currentWebPageMetadata;

    public bool IsTextEmptyOrWhiteSpace() => this.viewModel.IsTextEmptyOrWhiteSpace;

    public bool IsEmojiKeyboardOpen() => this.viewModel.IsEmojiKeyboardOpen;

    public void SetOrientation(PageOrientation pageOrientation)
    {
      this.viewModel.OwnerPageOrientation = pageOrientation;
      this.MainPanel.Margin = this.viewModel.ContentMargin;
    }

    public IObservable<TextChangedEventArgs> TextChangedObservable()
    {
      return this.TextBox.GetTextChangedAsync();
    }

    public IObservable<Unit> LinkDataUpdatedObservable()
    {
      return (IObservable<Unit>) this.linkDataUpdatedSubject;
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
        this.emojiKeyboard = new EmojiKeyboard(insertionTextBox: this.TextBox);
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
          this.NotifyAction(TextStatusInputBar.Actions.CloseEmojiKeyboard);
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
    }

    public double GetSIPHeight()
    {
      return this.IsEmojiSearchOpen() ? UIUtils.SIPHeightPortrait + this.emojiKeyboard.PickerHeightVisible() + UIUtils.SIPHeightAdjustmentForWP10 : UIUtils.SIPHeightPortrait;
    }

    public IObservable<Pair<TextStatusInputBar.Actions, object>> ActionObservable()
    {
      return (IObservable<Pair<TextStatusInputBar.Actions, object>>) this.actionSubject;
    }

    private void AbortPendingAnimations()
    {
      IDisposable[] array = this.pendingAnimations.ToArray();
      this.pendingAnimations.Clear();
      foreach (IDisposable d in array)
        d.SafeDispose();
    }

    private void DetectLink()
    {
      Log.d("status input bar", "detecting link");
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
        Log.d("status input bar", "text:{0}, uri:{1}", (object) link, (object) (str ?? ""));
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
        Log.d("status input bar", "started request | uri:[{0}] | text:[{1}]", (object) matchedUri, (object) matchedText);
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
      Log.d("status input bar", "delayed request | uri:[{0}]", (object) matchedUri);
    }

    private void UpdateLinkPreview(WebPageMetadata metadata)
    {
      bool flag = false;
      if (metadata == null)
      {
        flag = true;
        Log.d("status input bar", "skip link preview update | null input | curr matched:{0}", (object) (this.currentMatchedUri ?? "n/a"));
      }
      if (this.currentMatchedUri == null)
      {
        flag = true;
        Log.d("status input bar", "skip link preview update | null matched uri | fetched data:{0}", (object) metadata.OriginalUrl);
      }
      if (string.Compare(metadata.OriginalUrl, this.currentMatchedUri, StringComparison.OrdinalIgnoreCase) != 0)
        Log.d("status input bar", "skip link preview update | outdated uri | matched:{0} fetched:{1}", (object) this.currentMatchedUri, (object) metadata.OriginalUrl);
      else if (flag)
        this.ClearLinkPreview();
      else if (this.currentWebPageMetadata != null && string.Compare(metadata.OriginalUrl, this.currentWebPageMetadata.OriginalUrl, StringComparison.OrdinalIgnoreCase) != 0)
      {
        Log.d("status input bar", "skip link preview update | same uri: {0}", (object) this.currentMatchedUri);
      }
      else
      {
        this.currentWebPageMetadata = metadata;
        this.linkDataUpdatedSubject.OnNext(new Unit());
      }
    }

    private void ClearLinkPreview()
    {
      this.currentWebPageMetadata.SafeDispose();
      this.currentWebPageMetadata = (WebPageMetadata) null;
      this.linkDataUpdatedSubject.OnNext(new Unit());
      this.AbortPendingAnimations();
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

    public void MoveCursorToEnd()
    {
      this.TextBox.SelectionLength = 0;
      this.TextBox.SelectionStart = this.TextBox.Text.Length;
    }

    private void EmojiButton_Click(object sender, RoutedEventArgs e)
    {
      if (this.ShouldBlockFrequentButtonClick())
        return;
      TextStatusInputBar.Actions actType = this.viewModel.IsEmojiKeyboardOpen ? TextStatusInputBar.Actions.OpenKeyboard : TextStatusInputBar.Actions.OpenEmojiKeyboard;
      this.NotifyAction(actType);
      if (actType == TextStatusInputBar.Actions.OpenEmojiKeyboard)
        this.OpenEmojiKeyboard();
      else
        this.OpenKeyboard();
    }

    private void TextBox_KeyDown(object sender, KeyEventArgs e)
    {
      TimeSpentManager.GetInstance().UserAction();
      if (e.Key != System.Windows.Input.Key.Back)
        return;
      EmojiPickerViewModel.InsertBackspace(this.TextBox, ref this.currentText);
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
      this.SetSIPState(SIPStates.Keyboard);
    }

    private void TextBox_LostFocus(object sender, RoutedEventArgs e)
    {
      this.viewModel.IsTextBoxFocused = false;
      this.SetSIPState(SIPStates.Collapsed);
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      string currentText = this.currentText;
      this.currentText = this.TextBox.Text;
      bool flag = string.IsNullOrWhiteSpace(this.currentText);
      this.viewModel.IsTextEmptyOrWhiteSpace = flag;
      this.viewModel.HasAnyText = this.currentText.Any<char>();
      this.linkDetectionDelaySub.SafeDispose();
      this.linkDetectionDelaySub = (IDisposable) null;
      if (flag)
        this.ClearLinkPreview();
      else
        this.linkDetectionDelaySub = Observable.Timer(TimeSpan.FromMilliseconds(350.0)).ObserveOnDispatcher<long>().Subscribe<long>((Action<long>) (_ => this.DetectLink()));
    }

    private void FontButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (this.ShouldBlockFrequentButtonClick())
        return;
      this.NotifyAction(TextStatusInputBar.Actions.ChangeFont);
    }

    private void BackgroundButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (this.ShouldBlockFrequentButtonClick())
        return;
      this.NotifyAction(TextStatusInputBar.Actions.ChangeBackground);
    }

    private void SendButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (this.ShouldBlockFrequentButtonClick() || !LinkDetector.IsSendableText((this.TextBox.Text ?? "").Trim()))
        return;
      this.NotifyAction(TextStatusInputBar.Actions.SendText, (object) this.GetInputData());
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/TextStatusInputBar.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.MainPanel = (Grid) this.FindName("MainPanel");
      this.EmojiButton = (Button) this.FindName("EmojiButton");
      this.EmojiButtonIcon = (Image) this.FindName("EmojiButtonIcon");
      this.TextBox = (TextBox) this.FindName("TextBox");
    }

    public enum Actions
    {
      Undefined,
      SendText,
      OpenKeyboard,
      OpenEmojiKeyboard,
      CloseEmojiKeyboard,
      ChangeFont,
      ChangeBackground,
    }
  }
}
