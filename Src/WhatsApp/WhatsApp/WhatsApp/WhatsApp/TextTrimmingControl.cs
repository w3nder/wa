// Decompiled with JetBrains decompiler
// Type: WhatsApp.TextTrimmingControl
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;


namespace WhatsApp
{
  public class TextTrimmingControl : ContentControl
  {
    private TextBlock actualText;
    private TextBlock dummyText;
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof (Text), typeof (string), typeof (TextTrimmingControl), new PropertyMetadata((PropertyChangedCallback) ((dep, args) =>
    {
      TextTrimmingControl control = dep as TextTrimmingControl;
      control.ContentFunction = (Func<Inline>) (() => (Inline) new Run()
      {
        Text = (args.NewValue as string)
      });
      control.renderingEventHandler = (EventHandler) ((s, e) =>
      {
        control.InvalidateMeasure();
        CompositionTarget.Rendering -= control.renderingEventHandler;
        control.renderingEventHandler = (EventHandler) null;
      });
      CompositionTarget.Rendering += control.renderingEventHandler;
    })));
    private EventHandler renderingEventHandler;
    public Func<Inline> ContentFunction;

    public double ReservedEndSpace { get; set; }

    public TextTrimmingControl()
    {
      TextBlock textBlock1 = new TextBlock();
      textBlock1.TextWrapping = TextWrapping.NoWrap;
      textBlock1.TextTrimming = TextTrimming.None;
      textBlock1.Margin = new Thickness(0.0);
      textBlock1.Padding = new Thickness(0.0);
      this.actualText = textBlock1;
      TextBlock textBlock2 = new TextBlock();
      textBlock2.TextWrapping = TextWrapping.NoWrap;
      textBlock2.TextTrimming = TextTrimming.None;
      textBlock2.Margin = new Thickness(0.0);
      textBlock2.Padding = new Thickness(0.0);
      this.dummyText = textBlock2;
      // ISSUE: explicit constructor call
      base.\u002Ector();
      this.Content = (object) this.actualText;
    }

    public string Text
    {
      get => this.GetValue(TextTrimmingControl.TextProperty) as string;
      set => this.SetValue(TextTrimmingControl.TextProperty, (object) value);
    }

    public int MaxLines { get; set; }

    private void AssignFont(ref TextTrimmingControl.FontProperties font, TextElement elem)
    {
    }

    private IEnumerable<TextTrimmingControl.RunInfo> GetRuns(
      TextTrimmingControl.FontProperties font,
      Inline inline)
    {
      this.AssignFont(ref font, (TextElement) inline);
      font.TextDecorations = inline.TextDecorations;
      if (inline is Run run1)
        yield return new TextTrimmingControl.RunInfo()
        {
          Run = run1,
          Font = font
        };
      if (inline is Span span)
      {
        foreach (TextTrimmingControl.RunInfo run2 in this.GetRuns(font, span.Inlines))
          yield return run2;
      }
    }

    private IEnumerable<TextTrimmingControl.RunInfo> GetRuns(
      TextTrimmingControl.FontProperties font,
      InlineCollection ic)
    {
      foreach (Inline inline in (PresentationFrameworkCollection<Inline>) ic)
      {
        foreach (TextTrimmingControl.RunInfo run in this.GetRuns(font, inline))
          yield return run;
      }
    }

    private IEnumerable<TextTrimmingControl.RunInfo> GetRuns(
      TextTrimmingControl.FontProperties font,
      Block b)
    {
      this.AssignFont(ref font, (TextElement) b);
      if (b is Paragraph paragraph)
      {
        foreach (TextTrimmingControl.RunInfo run in this.GetRuns(font, paragraph.Inlines))
          yield return run;
      }
      if (b is Section section)
      {
        foreach (TextTrimmingControl.RunInfo run in section.Blocks.SelectMany<Block, TextTrimmingControl.RunInfo, TextTrimmingControl.RunInfo>((Func<Block, IEnumerable<TextTrimmingControl.RunInfo>>) (bl => this.GetRuns(font, bl)), (Func<Block, TextTrimmingControl.RunInfo, TextTrimmingControl.RunInfo>) ((bl, r) => r)))
          yield return run;
      }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      if (this.renderingEventHandler != null)
      {
        CompositionTarget.Rendering -= this.renderingEventHandler;
        this.renderingEventHandler = (EventHandler) null;
      }
      if (this.ContentFunction == null)
        return base.MeasureOverride(availableSize);
      this.actualText.Inlines.Clear();
      Inline inline = this.ContentFunction();
      foreach (TextTrimmingControl.RunInfo run1 in this.GetRuns(new TextTrimmingControl.FontProperties()
      {
        FontFamily = this.FontFamily,
        FontSize = this.FontSize,
        FontStyle = this.FontStyle,
        FontWeight = this.FontWeight,
        TextDecorations = this.actualText.TextDecorations
      }, inline))
      {
        int maxLines = this.MaxLines;
        Run run2 = run1.Run;
        string[] source = (run2 != null ? run2.Text : "").Split(new char[1]
        {
          ' '
        }, StringSplitOptions.RemoveEmptyEntries);
        if (!((IEnumerable<string>) source).Any<string>())
          return base.MeasureOverride(availableSize);
        this.dummyText.FontFamily = run1.Font.FontFamily;
        this.dummyText.FontStyle = run1.Font.FontStyle;
        this.dummyText.FontSize = run1.Font.FontSize;
        this.dummyText.FontWeight = run1.Font.FontWeight;
        this.dummyText.FontStretch = this.actualText.FontStretch;
        string str1 = "";
        int index1 = 0;
        StringBuilder stringBuilder = new StringBuilder();
        string str2 = source[index1];
        string str3;
        this.dummyText.Text = str3 = source[index1];
        this.dummyText.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        double width = availableSize.Width;
        while (maxLines > 0 && index1 < source.Length)
        {
          if (this.dummyText.ActualWidth > width)
          {
            if (--maxLines == 1)
              width -= this.ReservedEndSpace;
            stringBuilder.Append(str1);
            stringBuilder.Append('\n');
            str1 = "";
            str3 = source[index1];
          }
          else
          {
            str1 = str3;
            if (++index1 < source.Length)
            {
              string str4 = source[index1];
              str3 = str1.Length == 0 ? str4 : string.Format("{0} {1}", (object) str1, (object) str4);
            }
          }
          if (maxLines > 0)
          {
            this.dummyText.Text = str3;
            this.dummyText.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
          }
        }
        if (str3.Length != 0)
          stringBuilder.Append(str1);
        if (stringBuilder.Length != 0 && stringBuilder[stringBuilder.Length - 1] == '\n')
          stringBuilder.Remove(stringBuilder.Length - 1, 1);
        if (index1 != source.Length)
        {
          int index2 = stringBuilder.Length - 1;
          int length = 0;
          while (index2 >= 0 && char.IsPunctuation(stringBuilder[index2]))
          {
            --index2;
            ++length;
          }
          if (length != 0)
            stringBuilder.Remove(stringBuilder.Length - length, length);
          stringBuilder.Append('…');
        }
        run2.Text = stringBuilder.ToString();
        this.actualText.Inlines.Add((Inline) run2);
      }
      return base.MeasureOverride(availableSize);
    }

    private struct RunInfo
    {
      public Run Run;
      public TextTrimmingControl.FontProperties Font;
    }

    private struct FontProperties
    {
      public FontFamily FontFamily;
      public double FontSize;
      public FontStyle FontStyle;
      public FontWeight FontWeight;
      public TextDecorationCollection TextDecorations;
    }
  }
}
