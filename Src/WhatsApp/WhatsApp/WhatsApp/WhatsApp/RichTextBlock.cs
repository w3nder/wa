// Decompiled with JetBrains decompiler
// Type: WhatsApp.RichTextBlock
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;


namespace WhatsApp
{
  public class RichTextBlock : RichTextBox
  {
    private const string LogHeader = "rich text block";
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof (Text), typeof (RichTextBlock.TextSet), typeof (RichTextBlock), new PropertyMetadata((PropertyChangedCallback) ((dep, e) =>
    {
      if (!(dep is RichTextBlock richTextBlock2))
        return;
      richTextBlock2.Refresh();
    })));
    public static readonly DependencyProperty AllowLinksProperty = DependencyProperty.Register(nameof (AllowLinks), typeof (bool), typeof (RichTextBlock), new PropertyMetadata((object) false));
    public static readonly DependencyProperty LinkBackgroundProperty = DependencyProperty.Register(nameof (LinkBackground), typeof (Brush), typeof (RichTextBlock), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty ScaleEmojiSizeProperty = DependencyProperty.Register("ScaleEmojiSize", typeof (bool), typeof (RichTextBlock), new PropertyMetadata((object) false));
    public static readonly DependencyProperty EnableScanProperty = DependencyProperty.Register(nameof (EnableScan), typeof (bool), typeof (RichTextBlock), new PropertyMetadata((object) true));
    public static readonly DependencyProperty ExpandableProperty = DependencyProperty.Register(nameof (Expandable), typeof (bool), typeof (RichTextBlock), new PropertyMetadata((object) false));
    public static readonly DependencyProperty ExpandedProperty = DependencyProperty.Register(nameof (Expanded), typeof (bool), typeof (RichTextBlock), new PropertyMetadata((object) false));
    public static readonly DependencyProperty TextWrapLinesProperty = DependencyProperty.Register(nameof (TextWrapLines), typeof (int), typeof (RichTextBlock), new PropertyMetadata((PropertyChangedCallback) null));
    private bool allowMentions = true;
    private bool enableMentionLinks = true;
    private bool allowTextFormatting = true;

    public RichTextBlock() => this.LineStackingStrategy = LineStackingStrategy.BaselineToBaseline;

    public RichTextBlock.TextSet Text
    {
      get => this.GetValue(RichTextBlock.TextProperty) as RichTextBlock.TextSet;
      set => this.SetValue(RichTextBlock.TextProperty, (object) value);
    }

    public bool AllowLinks
    {
      get => (this.GetValue(RichTextBlock.AllowLinksProperty) as bool?).Value;
      set => this.SetValue(RichTextBlock.AllowLinksProperty, (object) value);
    }

    public Brush LinkBackground
    {
      get => this.GetValue(RichTextBlock.LinkBackgroundProperty) as Brush;
      set => this.SetValue(RichTextBlock.LinkBackgroundProperty, (object) value);
    }

    public bool LargeEmojiSize
    {
      get => (this.GetValue(RichTextBlock.ScaleEmojiSizeProperty) as bool?).Value;
      set => this.SetValue(RichTextBlock.ScaleEmojiSizeProperty, (object) value);
    }

    public bool EnableScan
    {
      get => this.GetValue(RichTextBlock.EnableScanProperty) as bool? ?? true;
      set => this.SetValue(RichTextBlock.EnableScanProperty, (object) value);
    }

    public bool Expandable
    {
      get => (this.GetValue(RichTextBlock.ExpandableProperty) as bool?).Value;
      set => this.SetValue(RichTextBlock.ExpandableProperty, (object) value);
    }

    public bool Expanded
    {
      get => (this.GetValue(RichTextBlock.ExpandedProperty) as bool?).Value;
      set => this.SetValue(RichTextBlock.ExpandedProperty, (object) value);
    }

    public int TextWrapLines
    {
      get => (int?) this.GetValue(RichTextBlock.TextWrapLinesProperty) ?? 1;
      set => this.SetValue(RichTextBlock.TextWrapLinesProperty, (object) value);
    }

    private bool AllowMentions
    {
      get => this.allowMentions;
      set => this.allowMentions = value;
    }

    public bool EnableMentionLinks
    {
      get => this.enableMentionLinks;
      set => this.enableMentionLinks = value;
    }

    public bool AllowTextFormatting
    {
      get => this.allowTextFormatting;
      set => this.allowTextFormatting = value;
    }

    private static int SingleLineTargetLength(
      double desiredWidth,
      double availableWidth,
      string text)
    {
      double num = Math.Max(6.0, (1.0 - availableWidth / desiredWidth) * (double) text.Length + 3.0);
      return (int) Math.Max(0.0, (double) text.Length - num);
    }

    private static int MultiLineTargetLength(
      double availableHeight,
      TextBlock measuringTextBlock,
      string text)
    {
      int length = text.Length;
      Size availableSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
      for (int index = length / 2; index >= 2; index = (index + 1) / 2)
      {
        string str = text.Substring(0, length) + "…";
        measuringTextBlock.Text = str;
        measuringTextBlock.Measure(availableSize);
        if (measuringTextBlock.ActualHeight <= availableHeight)
        {
          if (length >= text.Length)
          {
            length = text.Length;
            break;
          }
          length += index;
        }
        else
          length -= index;
      }
      return length;
    }

    private static int TrimLength(double desiredWidth, double availableWidth, string text)
    {
      int targetNewLength = RichTextBlock.SingleLineTargetLength(desiredWidth, availableWidth, text);
      return RichTextBlock.AdjustForEmoji(text, targetNewLength);
    }

    private static int TrimHeight(
      double availableHeight,
      TextBlock measuringTextBlock,
      string text)
    {
      int targetNewLength = RichTextBlock.MultiLineTargetLength(availableHeight + 1.0, measuringTextBlock, text);
      return RichTextBlock.AdjustForEmoji(text, targetNewLength);
    }

    private static int AdjustForEmoji(string text, int targetNewLength)
    {
      int funkynessLength = 0;
      if (Utils.IsFunkyUnicode(text, targetNewLength, out funkynessLength))
        targetNewLength = Math.Min(text.Length, targetNewLength + funkynessLength);
      return targetNewLength;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      if (this.TextWrapping == TextWrapping.NoWrap && this.Text != null)
      {
        if (string.IsNullOrEmpty(this.Text.Text))
        {
          Log.d(nameof (RichTextBlock), "Text.Text is null or empty in MeasureOverride in RichTextBlock");
          return base.MeasureOverride(availableSize);
        }
        TextBlock measuringTextBlock = new TextBlock()
        {
          TextWrapping = this.TextWrapping,
          TextTrimming = TextTrimming.None
        };
        Paragraph paragraph = (Paragraph) this.Blocks.FirstOrDefault<Block>();
        if (paragraph == null)
          return base.MeasureOverride(availableSize);
        Inline inline1 = paragraph.Inlines.FirstOrDefault<Inline>();
        bool flag1 = inline1 != null && inline1 is Run;
        measuringTextBlock.FontFamily = !flag1 || inline1.FontFamily == null ? this.FontFamily : inline1.FontFamily;
        TextBlock textBlock1 = measuringTextBlock;
        FontStyle fontStyle1;
        if (flag1)
        {
          FontStyle fontStyle2 = inline1.FontStyle;
          fontStyle1 = inline1.FontStyle;
        }
        else
          fontStyle1 = this.FontStyle;
        textBlock1.FontStyle = fontStyle1;
        TextBlock textBlock2 = measuringTextBlock;
        FontWeight fontWeight1;
        if (flag1)
        {
          FontWeight fontWeight2 = inline1.FontWeight;
          fontWeight1 = inline1.FontWeight;
        }
        else
          fontWeight1 = this.FontWeight;
        textBlock2.FontWeight = fontWeight1;
        measuringTextBlock.FontSize = this.FontSize;
        measuringTextBlock.FontStretch = this.FontStretch;
        double availableWidth = availableSize.Width * ResolutionHelper.ZoomMultiplier;
        int num1 = this.Text.Text.Length;
        bool flag2;
        if (this.TextWrapLines > 1)
        {
          this.TextWrapping = TextWrapping.Wrap;
          measuringTextBlock.Text = (string) null;
          measuringTextBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
          double availableHeight = measuringTextBlock.ActualHeight * (double) this.TextWrapLines;
          measuringTextBlock.Text = this.Text.Text;
          measuringTextBlock.TextWrapping = TextWrapping.Wrap;
          measuringTextBlock.Width = availableWidth;
          measuringTextBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
          double actualHeight = measuringTextBlock.ActualHeight;
          flag2 = availableHeight > 0.0 && actualHeight > availableHeight;
          if (flag2)
            num1 = RichTextBlock.TrimHeight(availableHeight, measuringTextBlock, this.Text.Text);
        }
        else
        {
          measuringTextBlock.Text = this.Text.Text;
          measuringTextBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
          double actualWidth = measuringTextBlock.ActualWidth;
          flag2 = availableWidth > 0.0 && actualWidth > availableWidth;
          if (flag2)
            num1 = RichTextBlock.TrimLength(actualWidth, availableWidth, this.Text.Text);
        }
        if (flag2)
        {
          int num2 = 0;
          List<Inline> source = new List<Inline>();
          bool flag3 = false;
          for (int index = 0; index < paragraph.Inlines.Count<Inline>(); ++index)
          {
            Inline inline2 = paragraph.Inlines.ElementAt<Inline>(index);
            if (flag3)
              source.Add(inline2);
            else if (inline2 is Run)
            {
              Run run = (Run) inline2;
              if (num2 + run.Text.Length > num1)
              {
                int length = num1 - num2;
                if (length > 0)
                {
                  string str = this.Expandable ? "…\n" : "…";
                  run.Text = run.Text.Substring(0, length) + str;
                  flag3 = true;
                }
              }
              num2 += run.Text.Length;
            }
            else
            {
              if (num2 + 2 >= num1 && num1 - num2 > 0)
              {
                source.Add(inline2);
                flag3 = true;
              }
              num2 += 2;
            }
          }
          for (int index = 0; index < source.Count<Inline>(); ++index)
            paragraph.Inlines.Remove(source.ElementAt<Inline>(index));
          if (flag3 && this.Expandable)
          {
            InlineCollection inlines = paragraph.Inlines;
            this.MakeExpandable(ref inlines);
          }
        }
        else
          this.Expanded = false;
      }
      return base.MeasureOverride(availableSize);
    }

    public EventHandler<GestureEventArgs> SeeMoreEventHandler { get; set; }

    private void MakeExpandable(ref InlineCollection inlines)
    {
      TextBlock textBlock1 = new TextBlock();
      textBlock1.Text = AppResources.SeeMore;
      textBlock1.FontStyle = this.FontStyle;
      textBlock1.FontFamily = this.FontFamily;
      textBlock1.FontSize = this.FontSize;
      textBlock1.Foreground = (Brush) UIUtils.ForegroundBrush;
      textBlock1.FontWeight = FontWeights.SemiBold;
      textBlock1.Margin = new Thickness(0.0, 4.0, 0.0, 4.0);
      TextBlock textBlock2 = textBlock1;
      if (this.SeeMoreEventHandler == null)
        textBlock2.Tap += new EventHandler<GestureEventArgs>(this.SeeMore_Tap);
      else
        textBlock2.Tap += this.SeeMoreEventHandler;
      inlines.Add((Inline) new InlineUIContainer()
      {
        Child = (UIElement) textBlock2
      });
    }

    private void SeeMore_Tap(object sender, GestureEventArgs e)
    {
      this.Expanded = true;
      this.Refresh();
    }

    public void Refresh()
    {
      this.Blocks.Clear();
      if (this.Text == null)
        return;
      Paragraph paragraph = new Paragraph();
      InlineCollection inlines = paragraph.Inlines;
      if (this.Expandable)
      {
        if (this.Expanded)
          this.TextWrapping = TextWrapping.Wrap;
        else
          this.TextWrapping = TextWrapping.NoWrap;
      }
      try
      {
        this.RenderImpl(ref inlines, this.Text, new WaRichTextRendering.RenderArgs(this.FontSize)
        {
          AllowLinks = this.AllowLinks,
          LinkBackground = this.LinkBackground,
          EnableMentionLinks = this.EnableMentionLinks,
          AllowMentions = this.AllowMentions,
          AllowTextFormatting = this.AllowTextFormatting,
          FontFamily = this.FontFamily,
          FontStyle = new FontStyle?(this.FontStyle),
          FontWeight = new FontWeight?(this.FontWeight),
          Foreground = this.Foreground,
          ForegroundBindingSource = (object) this,
          UseLargeEmojiSize = this.LargeEmojiSize
        });
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "rich text block rendering");
        inlines.Add(this.Text.Text);
      }
      this.Blocks.Add((Block) paragraph);
    }

    private void RenderImpl(
      ref InlineCollection inlines,
      RichTextBlock.TextSet text,
      WaRichTextRendering.RenderArgs args)
    {
      if (text == null)
        return;
      string text1 = text.Text;
      IEnumerable<LinkDetector.Result> results = text.SerializedFormatting;
      if ((results == null || !results.Any<LinkDetector.Result>()) && this.EnableScan)
      {
        WaRichText.Formats applicableFormats = WaRichText.Formats.Link | WaRichText.Formats.Emoji;
        if (this.AllowTextFormatting)
          applicableFormats |= WaRichText.Formats.TextFormattings;
        results = LinkDetector.GetMatches(text1, new WaRichText.DetectionArgs(applicableFormats));
      }
      if ((text.PartialFormattings == null || !text.PartialFormattings.Any<WaRichText.Chunk>()) && (results == null || !results.Any<LinkDetector.Result>()))
      {
        inlines.Add(text.Text ?? "");
      }
      else
      {
        List<LinkDetector.Result> chunks = WaRichText.MergeFormatings(results, text.PartialFormattings, text1);
        WaRichTextRendering.RenderRichText(ref inlines, (IEnumerable<LinkDetector.Result>) chunks, args);
      }
    }

    public class TextSet
    {
      public string Text { get; set; }

      public IEnumerable<LinkDetector.Result> SerializedFormatting { get; set; }

      public IEnumerable<WaRichText.Chunk> PartialFormattings { get; set; }

      public static RichTextBlock.TextSet Create(string value)
      {
        return new RichTextBlock.TextSet()
        {
          Text = value ?? ""
        };
      }
    }
  }
}
