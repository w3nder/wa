// Decompiled with JetBrains decompiler
// Type: WhatsApp.CallLogItemTitleBlock
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

#nullable disable
namespace WhatsApp
{
  public class CallLogItemTitleBlock : ContentControl
  {
    private TextBlock baseBlock;
    private CallLogItemTitleBlock.State state;

    public Style TextStyle
    {
      set => this.baseBlock.Style = value;
    }

    public double TextFontSize
    {
      set => this.baseBlock.FontSize = value;
    }

    public CallLogItemTitleBlock()
    {
      TextBlock textBlock = new TextBlock();
      textBlock.TextWrapping = TextWrapping.NoWrap;
      textBlock.TextTrimming = TextTrimming.None;
      textBlock.Margin = new Thickness(0.0, -6.0, 0.0, 0.0);
      textBlock.Padding = new Thickness(0.0);
      textBlock.FontFamily = UIUtils.FontFamilySemiLight;
      this.baseBlock = textBlock;
      // ISSUE: explicit constructor call
      base.\u002Ector();
      this.Content = (object) this.baseBlock;
      this.HorizontalAlignment = this.HorizontalContentAlignment = HorizontalAlignment.Left;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      if (this.state != null)
      {
        this.baseBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        double desiredWidth = this.baseBlock.ActualWidth;
        double availableWidth = availableSize.Width;
        if (desiredWidth > availableWidth)
        {
          CallLogItemTitleBlock.State cached = this.state;
          this.Dispatcher.BeginInvoke((Action) (() =>
          {
            if (cached != this.state)
              return;
            this.ShrinkTitle(desiredWidth, availableWidth);
          }));
        }
      }
      return base.MeasureOverride(availableSize);
    }

    public void SetContent(string titleStr, string countStr, List<string> peersNames = null)
    {
      string str = titleStr.Replace(' ', ' ');
      this.state = new CallLogItemTitleBlock.State()
      {
        OriginalTitle = str,
        TitleInUse = str,
        Count = countStr,
        Peers = peersNames != null ? new List<string>((IEnumerable<string>) peersNames) : (List<string>) null
      };
      if (peersNames != null)
        this.baseBlock.Text = string.Format("{0}{1}", (object) str, (object) countStr);
      else
        this.baseBlock.Text = string.Format("{0}{1}{2}", (object) str, (object) ' ', (object) countStr);
    }

    private void ShrinkTitle(double desiredWidth, double availableWidth)
    {
      if (this.state == null)
        return;
      double length = Math.Max(0.0, (double) this.state.TitleInUse.Length - Math.Max(6.0, (1.0 - availableWidth / desiredWidth) * (double) this.state.TitleInUse.Length));
      if (this.state.Peers == null)
      {
        this.state.TitleInUse = string.Format("{0}...", (object) this.state.TitleInUse.Substring(0, (int) length));
        this.baseBlock.Text = string.Format("{0}{1}{2}", (object) this.state.TitleInUse, (object) ' ', (object) this.state.Count);
      }
      else
      {
        List<string> peers = this.state.Peers;
        int num;
        if ((double) peers.First<string>().Length > length)
        {
          this.state.TitleInUse = string.Format("{0}...", (object) peers.First<string>().Substring(0, (int) length));
          num = this.state.Peers.Count - 1;
        }
        else if (peers.Count > 1 && (double) string.Join(", ", peers.Take<string>(2)).Length > length)
        {
          this.state.TitleInUse = peers.First<string>();
          num = this.state.Peers.Count - 1;
        }
        else if (peers.Count > 2 && (double) string.Join(", ", peers.Take<string>(3)).Length > length)
        {
          this.state.TitleInUse = string.Join(", ", peers.Take<string>(2));
          num = this.state.Peers.Count - 2;
        }
        else
        {
          this.state.TitleInUse = string.Join(", ", peers.Take<string>(3));
          num = this.state.Peers.Count - 3;
        }
        this.state.TitleInUse = this.state.TitleInUse.Replace(' ', ' ');
        this.state.TitleInUse += ",";
        this.baseBlock.Text = string.Format("{0}{1}{2}", (object) this.state.TitleInUse, (object) ' ', (object) string.Format("+{0}", (object) num));
      }
      this.InvalidateMeasure();
    }

    private class State
    {
      public string OriginalTitle;
      public string TitleInUse;
      public string Count;
      public List<string> Peers;
    }
  }
}
