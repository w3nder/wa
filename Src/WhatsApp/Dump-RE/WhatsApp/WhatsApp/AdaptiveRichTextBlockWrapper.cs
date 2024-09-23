// Decompiled with JetBrains decompiler
// Type: WhatsApp.AdaptiveRichTextBlockWrapper
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Windows;
using System.Windows.Controls;

#nullable disable
namespace WhatsApp
{
  public class AdaptiveRichTextBlockWrapper : ContentControl
  {
    private RichTextBlock innerBlock;

    public AdaptiveRichTextBlockWrapper(RichTextBlock rtb)
    {
      this.Content = (object) (this.innerBlock = rtb);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      if (this.innerBlock != null)
      {
        double fontSize = this.innerBlock.FontSize;
        bool flag;
        do
        {
          flag = false;
          this.innerBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
          if (this.innerBlock.DesiredSize.Height > availableSize.Height)
          {
            flag = true;
            fontSize -= Math.Max(1.0, fontSize * 0.15);
            this.innerBlock.FontSize = fontSize;
          }
        }
        while (flag);
      }
      return base.MeasureOverride(availableSize);
    }
  }
}
