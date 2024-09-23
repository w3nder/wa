// Decompiled with JetBrains decompiler
// Type: WhatsApp.EmojiTextBox
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;


namespace WhatsApp
{
  public class EmojiTextBox : UserControl
  {
    public static readonly DependencyProperty MaxLengthProperty = DependencyProperty.Register(nameof (MaxLength), typeof (int), typeof (EmojiTextBox), new PropertyMetadata((PropertyChangedCallback) ((dep, e) => (dep as EmojiTextBox).OnMaxLengthChanged((int) e.NewValue))));
    private EmojiTextBoxViewModel viewModel_;
    private string originalText;
    public string PreviousText;
    internal UserControl EmojiTB;
    internal Grid LayoutRoot;
    internal TextBox TextBox;
    internal TextBlock TextInputTooltipBlock;
    internal Button ActionButton;
    internal Polygon OuterPolygon;
    internal Polygon InnerPolygon;
    internal StackPanel LengthHintPanel;
    private bool _contentLoaded;

    public int MaxLength
    {
      get => UTF32Utils.GetMaxLength((DependencyObject) this.TextBox) ?? 0;
      set => this.SetValue(EmojiTextBox.MaxLengthProperty, (object) value);
    }

    public EmojiKeyboard EmojiKeyboard { get; set; }

    public bool IsSIPUp => this.viewModel_.IsTextBoxFocused || this.viewModel_.IsEmojiKeyboardOpen;

    public bool IsEmojiKeyboardOpen => this.viewModel_.IsEmojiKeyboardOpen;

    public bool IsReadOnly
    {
      get => this.TextBox.IsReadOnly;
      set => this.TextBox.IsReadOnly = value;
    }

    public bool AcceptsReturn
    {
      get => this.TextBox.AcceptsReturn;
      set => this.TextBox.AcceptsReturn = value;
    }

    public TextWrapping TextWrapping
    {
      get => this.TextBox.TextWrapping;
      set => this.TextBox.TextWrapping = value;
    }

    public double TextFontSize
    {
      get => this.TextBox.FontSize;
      set => this.TextBox.FontSize = value;
    }

    public Brush TextForeground
    {
      get => this.TextBox.Foreground;
      set => this.TextBox.Foreground = value;
    }

    public string OriginalText
    {
      get
      {
        if (this.originalText == null)
          this.originalText = Emoji.ConvertToRichText(this.Text ?? "");
        return this.originalText;
      }
      set => this.originalText = value;
    }

    public string Text
    {
      get => this.TextBox.Text ?? "";
      set
      {
        this.TextBox.Text = value ?? "";
        this.viewModel_.CurrentTextLength = this.OriginalText.GetRealCharLength();
        if (this.PreviousText != null)
          return;
        this.PreviousText = this.TextBox.Text;
      }
    }

    public Visibility TextInputTooltipVisibility => (!this.TextBox.Text.Any<char>()).ToVisibility();

    public bool ShowTail
    {
      get => this.OuterPolygon.Visibility == Visibility.Visible;
      set
      {
        this.OuterPolygon.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        this.InnerPolygon.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
      }
    }

    public EmojiTextBox.Alignment CounterLocation
    {
      get
      {
        return Grid.GetRow((FrameworkElement) this.LengthHintPanel) != 3 ? EmojiTextBox.Alignment.TopRight : EmojiTextBox.Alignment.BottomRight;
      }
      set
      {
        Grid.SetRow((FrameworkElement) this.LengthHintPanel, value == EmojiTextBox.Alignment.TopRight ? 0 : 3);
      }
    }

    public double CounterHeight
    {
      get => this.LengthHintPanel.Height;
      set => this.LengthHintPanel.Height = value;
    }

    public bool AccentHighlight
    {
      get => this.viewModel_.AccentHighlight;
      set => this.viewModel_.AccentHighlight = value;
    }

    public bool EnableActionButton
    {
      get => this.ActionButton.Visibility == Visibility.Visible;
      set => this.ActionButton.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
    }

    public bool ForceBorderHighlight
    {
      set => this.viewModel_.ForceBorderHighlight = value;
    }

    public event EventHandler EmojiKeyboardInited;

    protected void NotifyEmojiKeyboardInited()
    {
      if (this.EmojiKeyboardInited == null)
        return;
      this.EmojiKeyboardInited((object) this, new EventArgs());
    }

    public event EventHandler EmojiKeyboardOpening;

    protected void NotifyEmojiKeyboardOpening()
    {
      if (this.EmojiKeyboardOpening == null)
        return;
      this.EmojiKeyboardOpening((object) this, new EventArgs());
    }

    public event EventHandler EmojiKeyboardClosed;

    protected void NotifyEmojiKeyboardClosed()
    {
      if (this.EmojiKeyboardClosed == null)
        return;
      this.EmojiKeyboardClosed((object) this, new EventArgs());
    }

    public event EventHandler SIPChanged;

    protected void NotifySIPChanged()
    {
      if (this.SIPChanged == null)
        return;
      this.SIPChanged((object) this, new EventArgs());
    }

    public Brush CounterForeground
    {
      set => this.viewModel_.CounterBrush = value;
    }

    public EmojiTextBox()
    {
      this.InitializeComponent();
      this.DataContext = (object) (this.viewModel_ = new EmojiTextBoxViewModel());
      this.TextBox.GetFocusChangeObservable().Subscribe<bool>((Action<bool>) (focused =>
      {
        this.TextBox_FocusChangedAsync(focused);
        this.NotifySIPChanged();
      }));
      this.TextInputTooltipBlock.Margin = new Thickness(6.0 * ResolutionHelper.ZoomMultiplier, -4.0 * ResolutionHelper.ZoomMultiplier, 0.0, 0.0);
      this.TextInputTooltipBlock.FontSize = Settings.SystemFontSize * ResolutionHelper.ZoomMultiplier;
    }

    public IObservable<bool> TextBoxFocusChangedObservable()
    {
      return this.TextBox.GetFocusChangeObservable();
    }

    public IObservable<TextChangedEventArgs> GetTextChangedAsync()
    {
      return this.TextBox.GetTextChangedAsync();
    }

    public void OpenTextKeyboard() => this.TextBox.Focus();

    public void OpenEmojiKeyboard()
    {
      if (this.EmojiKeyboard == null)
      {
        this.EmojiKeyboard = new EmojiKeyboard(insertionTextBox: this.TextBox);
        this.EmojiKeyboard.OwnerPage = UIUtils.GetVisualTreeAncestors((UIElement) this).FirstOrDefault<UIElement>((Func<UIElement, bool>) (ancestor => ancestor is PhoneApplicationPage)) as PhoneApplicationPage;
        this.EmojiKeyboard.Opened += (EventHandler) ((sender, e) =>
        {
          this.viewModel_.IsEmojiKeyboardOpen = true;
          this.NotifySIPChanged();
        });
        this.EmojiKeyboard.Closed += (EventHandler) ((sender, e) =>
        {
          this.viewModel_.IsEmojiKeyboardOpen = false;
          this.NotifyEmojiKeyboardClosed();
          this.NotifySIPChanged();
        });
        this.NotifyEmojiKeyboardInited();
      }
      this.NotifyEmojiKeyboardOpening();
      this.EmojiKeyboard.Open();
    }

    public void CloseEmojiKeyboard()
    {
      if (this.EmojiKeyboard == null)
        return;
      this.EmojiKeyboard.Close();
      this.viewModel_.IsEmojiKeyboardOpen = false;
    }

    private void ActionButton_Tap(object sender, RoutedEventArgs e)
    {
      Log.p("emoji textbox", "action button tap");
      if (this.IsEmojiKeyboardOpen)
      {
        this.TextBox.Focus();
      }
      else
      {
        if (!this.viewModel_.HasRoomForEmoji)
          return;
        this.OpenEmojiKeyboard();
      }
    }

    private void TextBox_FocusChangedAsync(bool focused)
    {
      if (focused)
        this.CloseEmojiKeyboard();
      this.viewModel_.IsTextBoxFocused = focused;
    }

    private void TextBox_TextChanged(object sender, EventArgs e)
    {
      if (this.viewModel_ != null && this.viewModel_.ShowLengthHint)
      {
        this.OriginalText = (string) null;
        int realCharLength = this.OriginalText.GetRealCharLength();
        if (realCharLength > this.MaxLength)
        {
          this.Text = this.PreviousText;
          this.TextBox.SelectionStart = this.PreviousText.Length;
        }
        else
          this.viewModel_.CurrentTextLength = realCharLength;
      }
      this.PreviousText = this.Text;
    }

    private void TextBox_KeyDown(object sender, KeyEventArgs e)
    {
      TimeSpentManager.GetInstance().UserAction();
      if (e.Key != System.Windows.Input.Key.Back)
        return;
      EmojiPickerViewModel.InsertBackspace(this.TextBox, ref this.PreviousText);
    }

    private void OnMaxLengthChanged(int newVal)
    {
      UTF32Utils.SetMaxLength((DependencyObject) this.TextBox, new int?(newVal));
      this.viewModel_.MaxLength = newVal;
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/EmojiTextBox.xaml", UriKind.Relative));
      this.EmojiTB = (UserControl) this.FindName("EmojiTB");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.TextBox = (TextBox) this.FindName("TextBox");
      this.TextInputTooltipBlock = (TextBlock) this.FindName("TextInputTooltipBlock");
      this.ActionButton = (Button) this.FindName("ActionButton");
      this.OuterPolygon = (Polygon) this.FindName("OuterPolygon");
      this.InnerPolygon = (Polygon) this.FindName("InnerPolygon");
      this.LengthHintPanel = (StackPanel) this.FindName("LengthHintPanel");
    }

    public enum Alignment
    {
      BottomRight,
      TopRight,
    }
  }
}
