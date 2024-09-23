// Decompiled with JetBrains decompiler
// Type: WhatsApp.PageTitlePanel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace WhatsApp
{
  public class PageTitlePanel : ContentControl
  {
    public static readonly DependencyProperty ModeProperty = DependencyProperty.Register(nameof (Mode), typeof (PageTitlePanel.Modes), typeof (PageTitlePanel), new PropertyMetadata((PropertyChangedCallback) ((dep, e) => (dep as PageTitlePanel).OnModeChanged((PageTitlePanel.Modes) e.NewValue))));
    public static readonly DependencyProperty SmallTitleProperty = DependencyProperty.Register(nameof (SmallTitle), typeof (string), typeof (PageTitlePanel), new PropertyMetadata((PropertyChangedCallback) ((dep, e) => (dep as PageTitlePanel).OnSmallTitleChanged(e.NewValue as string))));
    public static readonly DependencyProperty LargeTitleProperty = DependencyProperty.Register(nameof (LargeTitle), typeof (string), typeof (PageTitlePanel), new PropertyMetadata((PropertyChangedCallback) ((dep, e) => (dep as PageTitlePanel).OnLargeTitleChanged(e.NewValue as string))));
    public static readonly DependencyProperty SubtitleProperty = DependencyProperty.Register(nameof (Subtitle), typeof (string), typeof (PageTitlePanel), new PropertyMetadata((PropertyChangedCallback) ((dep, e) => (dep as PageTitlePanel).OnSubtitleChanged(e.NewValue as string))));
    public static readonly DependencyProperty TextBrushProperty = DependencyProperty.Register(nameof (TextBrush), typeof (Brush), typeof (PageTitlePanel), new PropertyMetadata((PropertyChangedCallback) ((dep, e) => (dep as PageTitlePanel).OnTextBrushChanged(e.NewValue as Brush))));
    private RichTextBlock smallRichTitleBlock;
    private TextBlock largeTitleBlock;
    private TextBlock subtitleBlock;
    private StackPanel contentPanel;
    protected StackPanel smallTitlePanel;
    private ZoomBox zoomBox;

    public PageTitlePanel.Modes Mode
    {
      get => (PageTitlePanel.Modes) this.GetValue(PageTitlePanel.ModeProperty);
      set => this.SetValue(PageTitlePanel.ModeProperty, (object) value);
    }

    public string SmallTitle
    {
      get => this.GetValue(PageTitlePanel.SmallTitleProperty) as string;
      set => this.SetValue(PageTitlePanel.SmallTitleProperty, (object) value);
    }

    public string LargeTitle
    {
      get => this.GetValue(PageTitlePanel.LargeTitleProperty) as string;
      set => this.SetValue(PageTitlePanel.LargeTitleProperty, (object) value);
    }

    public TextWrapping LargeTitleWrapping
    {
      set
      {
        if (this.largeTitleBlock == null)
          return;
        this.largeTitleBlock.TextWrapping = value;
      }
    }

    public string Subtitle
    {
      get => this.GetValue(PageTitlePanel.SubtitleProperty) as string;
      set => this.SetValue(PageTitlePanel.SubtitleProperty, (object) value);
    }

    public Brush TextBrush
    {
      get => this.GetValue(PageTitlePanel.TextBrushProperty) as Brush;
      set => this.SetValue(PageTitlePanel.TextBrushProperty, (object) value);
    }

    public bool KeepOriginalSubtitleCase { get; set; }

    public bool KeepOriginalSmallTitleCase { get; set; }

    public bool KeepOriginalLargeTitleCase { get; set; }

    public PageTitlePanel()
    {
      this.VerticalContentAlignment = VerticalAlignment.Top;
      this.HorizontalContentAlignment = HorizontalAlignment.Left;
      StackPanel stackPanel1 = new StackPanel();
      stackPanel1.Margin = new Thickness(24.0, 12.0, 0.0, 0.0);
      StackPanel stackPanel2 = stackPanel1;
      this.contentPanel = stackPanel1;
      this.Content = (object) stackPanel2;
      UIElementCollection children1 = this.contentPanel.Children;
      StackPanel stackPanel3 = new StackPanel();
      stackPanel3.Margin = new Thickness(0.0, 0.0, 0.0, 0.0);
      stackPanel3.Orientation = Orientation.Horizontal;
      StackPanel stackPanel4 = stackPanel3;
      this.smallTitlePanel = stackPanel3;
      StackPanel stackPanel5 = stackPanel4;
      children1.Add((UIElement) stackPanel5);
      UIElementCollection children2 = this.smallTitlePanel.Children;
      RichTextBlock richTextBlock1 = new RichTextBlock();
      richTextBlock1.FontWeight = FontWeights.Medium;
      richTextBlock1.HorizontalAlignment = HorizontalAlignment.Stretch;
      richTextBlock1.Margin = new Thickness(-12.0, 0.0, 0.0, 0.0);
      richTextBlock1.FontSize = 23.0;
      richTextBlock1.TextWrapping = TextWrapping.NoWrap;
      RichTextBlock richTextBlock2 = richTextBlock1;
      this.smallRichTitleBlock = richTextBlock1;
      RichTextBlock richTextBlock3 = richTextBlock2;
      children2.Add((UIElement) richTextBlock3);
      this.Mode = PageTitlePanel.Modes.NotZoomed;
    }

    private void OnSmallTitleChanged(string val)
    {
      if (string.IsNullOrEmpty(val))
      {
        this.smallRichTitleBlock.Visibility = Visibility.Collapsed;
      }
      else
      {
        this.smallRichTitleBlock.Text = new RichTextBlock.TextSet()
        {
          Text = this.KeepOriginalSmallTitleCase ? val : val.ToUpper()
        };
        this.smallRichTitleBlock.Visibility = Visibility.Visible;
      }
    }

    private void OnLargeTitleChanged(string val)
    {
      if (this.largeTitleBlock == null)
      {
        UIElementCollection children = this.contentPanel.Children;
        TextBlock textBlock1 = new TextBlock();
        textBlock1.Margin = new Thickness(-2.0, -7.0, 0.0, 28.0);
        textBlock1.TextWrapping = TextWrapping.Wrap;
        textBlock1.FontFamily = Application.Current.Resources[(object) "PhoneFontFamilySemiLight"] as FontFamily;
        textBlock1.FontSize = 54.0;
        TextBlock textBlock2 = textBlock1;
        this.largeTitleBlock = textBlock1;
        TextBlock textBlock3 = textBlock2;
        children.Add((UIElement) textBlock3);
        if (this.TextBrush != null)
          this.largeTitleBlock.Foreground = this.TextBrush;
      }
      if (string.IsNullOrEmpty(val) || !string.IsNullOrEmpty(this.Subtitle))
      {
        this.largeTitleBlock.Visibility = Visibility.Collapsed;
      }
      else
      {
        this.largeTitleBlock.Text = this.KeepOriginalLargeTitleCase ? val : val.ToLangFriendlyLower();
        this.largeTitleBlock.Visibility = Visibility.Visible;
      }
    }

    private void OnSubtitleChanged(string val)
    {
      if (string.IsNullOrEmpty(val) || !string.IsNullOrEmpty(this.LargeTitle))
      {
        if (this.subtitleBlock == null)
          return;
        this.subtitleBlock.Visibility = Visibility.Collapsed;
      }
      else
      {
        if (this.subtitleBlock == null)
        {
          UIElementCollection children = this.contentPanel.Children;
          TextBlock textBlock1 = new TextBlock();
          textBlock1.Margin = new Thickness(0.0);
          textBlock1.TextWrapping = TextWrapping.Wrap;
          textBlock1.Style = Application.Current.Resources[(object) "PhoneTextSubtleStyle"] as Style;
          textBlock1.FontSize = 22.0;
          TextBlock textBlock2 = textBlock1;
          this.subtitleBlock = textBlock1;
          TextBlock textBlock3 = textBlock2;
          children.Add((UIElement) textBlock3);
        }
        this.subtitleBlock.Text = this.KeepOriginalSubtitleCase ? val : val.ToLangFriendlyLower();
        this.subtitleBlock.Visibility = Visibility.Visible;
      }
    }

    private void OnTextBrushChanged(Brush val)
    {
      this.smallRichTitleBlock.Foreground = val;
      if (this.largeTitleBlock == null)
        return;
      this.largeTitleBlock.Foreground = val;
    }

    private void OnModeChanged(PageTitlePanel.Modes val)
    {
      if (!ResolutionHelper.ShouldEnableZoomBox)
        val = PageTitlePanel.Modes.NotZoomed;
      if (val == PageTitlePanel.Modes.NotZoomed)
      {
        if (this.zoomBox != null)
          this.zoomBox.Content = (object) null;
        this.Content = (object) this.contentPanel;
        this.contentPanel.Margin = new Thickness(24.0, 12.0, 0.0, 0.0);
      }
      else
      {
        if (this.zoomBox == null)
          this.zoomBox = new ZoomBox()
          {
            ZoomFactor = ResolutionHelper.ZoomFactor
          };
        this.Content = (object) this.zoomBox;
        this.zoomBox.Content = (object) this.contentPanel;
        this.contentPanel.Margin = new Thickness(24.0, 12.0, 0.0, 0.0);
        this.zoomBox.Margin = new Thickness(0.0);
      }
    }

    public enum Modes
    {
      NotZoomed,
      Zoomed,
    }
  }
}
