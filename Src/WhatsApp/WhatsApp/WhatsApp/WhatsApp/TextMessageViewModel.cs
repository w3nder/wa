// Decompiled with JetBrains decompiler
// Type: WhatsApp.TextMessageViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using WhatsApp.WaCollections;


namespace WhatsApp
{
  public class TextMessageViewModel : MessageViewModel
  {
    protected override bool ShouldAddFooterPlaceHolder
    {
      get
      {
        return this.MergedPosition == MessageViewModel.GroupingPosition.None || this.MergedPosition == MessageViewModel.GroupingPosition.Bottom;
      }
    }

    public override LinkDetector.Result[] TextPerformanceHint
    {
      get
      {
        LinkDetector.Result[] textPerformanceHint1 = base.TextPerformanceHint;
        LinkDetector.Result[] textPerformanceHint2;
        if (this.TextChop == null)
        {
          textPerformanceHint2 = textPerformanceHint1;
        }
        else
        {
          int num = this.TextChop.Offset + this.TextChop.Length;
          string textStr = this.TextStr;
          List<LinkDetector.Result> resultList = new List<LinkDetector.Result>();
          if (textPerformanceHint1 != null && ((IEnumerable<LinkDetector.Result>) textPerformanceHint1).Any<LinkDetector.Result>())
          {
            foreach (LinkDetector.Result result in textPerformanceHint1)
            {
              if (result.Index < this.TextChop.Offset)
              {
                if (result.Index + result.Length > this.TextChop.Offset)
                {
                  int index = 0;
                  int length = Math.Min(result.Length - (this.TextChop.Offset - result.Index), this.TextChop.Length);
                  resultList.Add(new LinkDetector.Result(index, length, result.type, textStr, result.AuxiliaryInfo));
                }
              }
              else if (result.Index < num)
              {
                int index = result.Index - this.TextChop.Offset;
                int length = Math.Min(result.Length, num - result.Index);
                resultList.Add(new LinkDetector.Result(index, length, result.type, textStr, result.AuxiliaryInfo));
              }
            }
          }
          textPerformanceHint2 = resultList.ToArray();
        }
        return textPerformanceHint2;
      }
    }

    public override WaRichText.Chunk[] InlineFormattings
    {
      get
      {
        if (this.SearchResult == null)
          return (WaRichText.Chunk[]) null;
        Pair<int, int>[] source = this.SearchResult.DataOffsets;
        MessageViewModel.ChopState textChop = this.TextChop;
        if (textChop != null)
          source = ((IEnumerable<Pair<int, int>>) source).Where<Pair<int, int>>((Func<Pair<int, int>, bool>) (offset =>
          {
            int num = offset.First - textChop.Offset;
            return num >= 0 && num < textChop.Length;
          })).Select<Pair<int, int>, Pair<int, int>>((Func<Pair<int, int>, Pair<int, int>>) (offset =>
          {
            int first = offset.First - textChop.Offset;
            int second = offset.Second;
            if (first + second >= textChop.Length)
              second = textChop.Length - first;
            return new Pair<int, int>(first, second);
          })).ToArray<Pair<int, int>>();
        return source != null ? ((IEnumerable<Pair<int, int>>) source).Select<Pair<int, int>, WaRichText.Chunk>((Func<Pair<int, int>, WaRichText.Chunk>) (p => new WaRichText.Chunk(p.First, p.Second, WaRichText.Formats.Bold))).ToArray<WaRichText.Chunk>() : (WaRichText.Chunk[]) null;
      }
    }

    public override Thickness ViewPanelMargin
    {
      get
      {
        double num = 12.0 * this.zoomMultiplier;
        return new Thickness(num, 0.0, num, num);
      }
    }

    public override bool ShouldShowText => true;

    public override HorizontalAlignment HorizontalAlignment
    {
      get
      {
        return !this.Message.HasPaymentInfo() ? HorizontalAlignment.Stretch : base.HorizontalAlignment;
      }
    }

    public override double MaxBubbleWidth
    {
      get
      {
        return !this.Message.HasPaymentInfo() ? double.PositiveInfinity : MessageViewModel.DefaultBubbleWidth;
      }
    }

    public TextMessageViewModel(Message m)
      : base(m)
    {
    }

    protected override string GetTextStr()
    {
      string textStr = this.Message.GetTextForDisplay();
      if (this.TextChop != null)
        textStr = textStr.Substring(this.TextChop.Offset, this.TextChop.Length);
      return textStr;
    }

    public override void RefreshTextFontSize() => this.Notify("TextFontSizeChanged");
  }
}
