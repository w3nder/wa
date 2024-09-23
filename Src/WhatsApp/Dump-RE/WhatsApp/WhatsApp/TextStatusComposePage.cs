// Decompiled with JetBrains decompiler
// Type: WhatsApp.TextStatusComposePage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using WhatsApp.WaCollections;

#nullable disable
namespace WhatsApp
{
  public class TextStatusComposePage : PhoneApplicationPage
  {
    private const int maxGlyphs = 700;
    private const int glyphsPerNewLine = 50;
    private List<Color> bgColors;
    private int currBgColor;
    private List<WhatsApp.ProtoBuf.Message.ExtendedTextMessage.FontType> fonts;
    private int currFont;
    private int fontSize = -1;
    private RichTextBlock.TextSet prevText;
    private List<IDisposable> disposables = new List<IDisposable>();
    internal ZoomBox LayoutRootZoomBox;
    internal Grid LayoutRoot;
    internal ScrollViewer ContentPanel;
    internal RichTextBlock DisplayBlock;
    internal TextStatusInputBar InputBar;
    private bool _contentLoaded;

    public TextStatusComposePage()
    {
      this.InitializeComponent();
      this.LayoutRootZoomBox.ZoomFactor = ResolutionHelper.ZoomFactor;
      BackKeyBroker.Get((PhoneApplicationPage) this, 1).Subscribe<CancelEventArgs>(new Action<CancelEventArgs>(this.OnBackKey));
      this.Loaded += (RoutedEventHandler) ((sender, e) => this.Dispatcher.BeginInvoke((Action) (() => this.InputBar.OpenKeyboard())));
      this.SwitchFont();
      this.SwitchBackground();
      this.ResetDisplayBlock();
      this.InitInputBar();
    }

    private void InitInputBar()
    {
      this.disposables.Add(this.InputBar.ActionObservable().ObserveOnDispatcher<Pair<TextStatusInputBar.Actions, object>>().Subscribe<Pair<TextStatusInputBar.Actions, object>>((Action<Pair<TextStatusInputBar.Actions, object>>) (p =>
      {
        switch (p.First)
        {
          case TextStatusInputBar.Actions.SendText:
            this.PostTextStatus();
            break;
          case TextStatusInputBar.Actions.ChangeFont:
            this.SwitchFont();
            this.MoveCursorToEnd();
            break;
          case TextStatusInputBar.Actions.ChangeBackground:
            this.SwitchBackground();
            this.MoveCursorToEnd();
            break;
        }
      })));
      this.disposables.Add(this.InputBar.SIPStateChangedObservable().Subscribe<SIPStates>((Action<SIPStates>) (sipState =>
      {
        double num = 0.0;
        if (sipState == SIPStates.Keyboard || sipState == SIPStates.EmojiKeyboard)
          num = UIUtils.SIPHeightPortrait;
        this.ContentPanel.Margin = new Thickness(24.0, 72.0 + num, 24.0, 72.0);
      })));
      this.disposables.Add(this.InputBar.TextChangedObservable().ObserveOnDispatcher<TextChangedEventArgs>().Subscribe<TextChangedEventArgs>((Action<TextChangedEventArgs>) (_ => this.UpdateText())));
      this.disposables.Add(this.InputBar.LinkDataUpdatedObservable().ObserveOnDispatcher<Unit>().Subscribe<Unit>((Action<Unit>) (_ => this.UpdateText())));
    }

    private void UpdateText()
    {
      RichTextBlock.TextSet textSet = this.InputBar.GetRichText();
      if (string.IsNullOrEmpty(textSet.Text))
      {
        this.ResetDisplayBlock();
      }
      else
      {
        if (this.DisplayBlock.Opacity < 0.9)
          this.DisplayBlock.Opacity = 1.0;
        int length = textSet.Text.Length;
        if (length > 14)
        {
          int num = 0;
          foreach (char ch in textSet.Text)
          {
            if (ch == '\n')
              ++num;
          }
          length += 50 * num;
          if (length > 700)
          {
            if (this.prevText == null)
              this.prevText = new RichTextBlock.TextSet()
              {
                Text = ""
              };
            this.InputBar.SetText(this.prevText.Text);
            this.InputBar.TextBox.SelectionStart = this.prevText.Text.Length;
            textSet = this.prevText;
          }
        }
        this.UpdateFontSize(length);
        this.DisplayBlock.Text = textSet;
      }
      this.prevText = textSet;
    }

    private void ResetDisplayBlock()
    {
      this.UpdateFontSize(0);
      this.DisplayBlock.Opacity = 0.3;
      this.DisplayBlock.Text = new RichTextBlock.TextSet()
      {
        Text = AppResources.TypeAStatusTooltip
      };
    }

    private void UpdateFontSize(int glyphs)
    {
      int num = 64;
      if (glyphs > 200)
        num = 18;
      else if (glyphs > 100)
        num = 36;
      if (this.fontSize == num)
        return;
      this.DisplayBlock.FontSize = (double) (this.fontSize = num);
    }

    private void MoveCursorToEnd()
    {
      this.Dispatcher.BeginInvoke((Action) (() =>
      {
        this.InputBar.MoveCursorToEnd();
        this.Dispatcher.BeginInvoke((Action) (() =>
        {
          if (this.InputBar.TextBox.SelectionLength <= 0)
            return;
          this.InputBar.MoveCursorToEnd();
        }));
      }));
    }

    private void SwitchFont()
    {
      int index = 0;
      if (this.fonts == null)
        this.fonts = WaStatusHelper.GetTextStatusFonts();
      else
        index = this.currFont + 1;
      if (index >= this.fonts.Count)
        index = 0;
      WhatsApp.ProtoBuf.Message.ExtendedTextMessage.FontType font = this.fonts[index];
      FontFamily fontFamily;
      switch (font)
      {
        case WhatsApp.ProtoBuf.Message.ExtendedTextMessage.FontType.SERIF:
          fontFamily = new FontFamily("MS Serif");
          break;
        case WhatsApp.ProtoBuf.Message.ExtendedTextMessage.FontType.NORICAN_REGULAR:
          fontFamily = Application.Current.Resources[(object) "Norican"] as FontFamily;
          break;
        case WhatsApp.ProtoBuf.Message.ExtendedTextMessage.FontType.BRYNDAN_WRITE:
          fontFamily = Application.Current.Resources[(object) "BryndanWrite"] as FontFamily;
          break;
        case WhatsApp.ProtoBuf.Message.ExtendedTextMessage.FontType.OSWALD_HEAVY:
          fontFamily = Application.Current.Resources[(object) "Oswald"] as FontFamily;
          break;
        default:
          fontFamily = new FontFamily("MS Sans Serif");
          break;
      }
      this.InputBar.Font = font;
      this.currFont = index;
      this.DisplayBlock.FontFamily = fontFamily;
      this.DisplayBlock.Refresh();
    }

    private void SwitchBackground()
    {
      int index = 0;
      if (this.bgColors == null)
        this.bgColors = WaStatusHelper.GetTextStatusBackgroundColors(true);
      else
        index = this.currBgColor + 1;
      if (index >= this.bgColors.Count)
        index = 0;
      Color bgColor = this.bgColors[index];
      this.LayoutRoot.Background = this.InputBar.TextBackground = (Brush) new SolidColorBrush(bgColor);
      this.currBgColor = index;
      this.DisplayBlock.LinkBackground = (Brush) new SolidColorBrush(WaStatusHelper.GetLinkBackgroundColor(bgColor));
      if (this.InputBar.GetLinkPreviewData() == null)
        return;
      this.UpdateText();
    }

    private void PostTextStatus()
    {
      string text = this.InputBar.GetText();
      Color bg = this.bgColors[this.currBgColor];
      WhatsApp.ProtoBuf.Message.ExtendedTextMessage.FontType font = this.fonts[this.currFont];
      WebPageMetadata linkData = this.InputBar.GetLinkPreviewData();
      if (LinkDetector.IsSendableText(text))
        AppState.Worker.Enqueue((Action) (() => WaStatusHelper.PostTextStatus(text, bg, font, linkData)));
      this.SlideDownAndBackOut();
    }

    private void SlideDownAndBackOut()
    {
      this.InputBar.CloseEmojiKeyboard(SIPStates.Undefined);
      Storyboarder.Perform(WaAnimations.PageTransition(PageTransitionAnimation.SlideDownFadeOut), (DependencyObject) this.LayoutRoot, false, (Action) (() => NavUtils.GoBack(this.NavigationService)), "text status: slide down and fade out");
    }

    private void OnBackKey(CancelEventArgs e)
    {
      e.Cancel = true;
      this.SlideDownAndBackOut();
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      base.OnRemovedFromJournal(e);
      try
      {
        this.InputBar.Dispose();
      }
      catch (Exception ex)
      {
        Log.SendCrashLog(ex, "dispose input bar");
      }
      IDisposable[] array = this.disposables.ToArray();
      this.disposables.Clear();
      foreach (IDisposable d in array)
        d.SafeDispose();
    }

    private void ContentPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      this.InputBar.OpenKeyboard();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/TextStatusComposePage.xaml", UriKind.Relative));
      this.LayoutRootZoomBox = (ZoomBox) this.FindName("LayoutRootZoomBox");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.ContentPanel = (ScrollViewer) this.FindName("ContentPanel");
      this.DisplayBlock = (RichTextBlock) this.FindName("DisplayBlock");
      this.InputBar = (TextStatusInputBar) this.FindName("InputBar");
    }
  }
}
