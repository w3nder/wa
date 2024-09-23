// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.WrapPanel
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

#nullable disable
namespace Microsoft.Phone.Controls
{
  public class WrapPanel : Panel
  {
    private bool _ignorePropertyChange;
    public static readonly DependencyProperty ItemHeightProperty = DependencyProperty.Register(nameof (ItemHeight), typeof (double), typeof (WrapPanel), new PropertyMetadata((object) double.NaN, new PropertyChangedCallback(WrapPanel.OnItemHeightOrWidthPropertyChanged)));
    public static readonly DependencyProperty ItemWidthProperty = DependencyProperty.Register(nameof (ItemWidth), typeof (double), typeof (WrapPanel), new PropertyMetadata((object) double.NaN, new PropertyChangedCallback(WrapPanel.OnItemHeightOrWidthPropertyChanged)));
    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof (Orientation), typeof (Orientation), typeof (WrapPanel), new PropertyMetadata((object) Orientation.Horizontal, new PropertyChangedCallback(WrapPanel.OnOrientationPropertyChanged)));

    [TypeConverter(typeof (LengthConverter))]
    public double ItemHeight
    {
      get => (double) this.GetValue(WrapPanel.ItemHeightProperty);
      set => this.SetValue(WrapPanel.ItemHeightProperty, (object) value);
    }

    [TypeConverter(typeof (LengthConverter))]
    public double ItemWidth
    {
      get => (double) this.GetValue(WrapPanel.ItemWidthProperty);
      set => this.SetValue(WrapPanel.ItemWidthProperty, (object) value);
    }

    public Orientation Orientation
    {
      get => (Orientation) this.GetValue(WrapPanel.OrientationProperty);
      set => this.SetValue(WrapPanel.OrientationProperty, (object) value);
    }

    private static void OnOrientationPropertyChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      WrapPanel wrapPanel = (WrapPanel) d;
      Orientation newValue = (Orientation) e.NewValue;
      if (wrapPanel._ignorePropertyChange)
      {
        wrapPanel._ignorePropertyChange = false;
      }
      else
      {
        if (newValue != Orientation.Horizontal && newValue != Orientation.Vertical)
        {
          wrapPanel._ignorePropertyChange = true;
          wrapPanel.SetValue(WrapPanel.OrientationProperty, (object) (Orientation) e.OldValue);
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Microsoft.Phone.Controls.Properties.Resources.WrapPanel_OnOrientationPropertyChanged_InvalidValue, (object) newValue), "value");
        }
        wrapPanel.InvalidateMeasure();
      }
    }

    private static void OnItemHeightOrWidthPropertyChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      WrapPanel wrapPanel = (WrapPanel) d;
      double newValue = (double) e.NewValue;
      if (wrapPanel._ignorePropertyChange)
      {
        wrapPanel._ignorePropertyChange = false;
      }
      else
      {
        if (!newValue.IsNaN() && (newValue <= 0.0 || double.IsPositiveInfinity(newValue)))
        {
          wrapPanel._ignorePropertyChange = true;
          wrapPanel.SetValue(e.Property, (object) (double) e.OldValue);
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Microsoft.Phone.Controls.Properties.Resources.WrapPanel_OnItemHeightOrWidthPropertyChanged_InvalidValue, (object) newValue), "value");
        }
        wrapPanel.InvalidateMeasure();
      }
    }

    protected override Size MeasureOverride(Size constraint)
    {
      Orientation orientation = this.Orientation;
      OrientedSize orientedSize1 = new OrientedSize(orientation);
      OrientedSize orientedSize2 = new OrientedSize(orientation);
      OrientedSize orientedSize3 = new OrientedSize(orientation, constraint.Width, constraint.Height);
      double itemWidth = this.ItemWidth;
      double itemHeight = this.ItemHeight;
      bool flag1 = !itemWidth.IsNaN();
      bool flag2 = !itemHeight.IsNaN();
      Size availableSize = new Size(flag1 ? itemWidth : constraint.Width, flag2 ? itemHeight : constraint.Height);
      foreach (UIElement child in (PresentationFrameworkCollection<UIElement>) this.Children)
      {
        child.Measure(availableSize);
        OrientedSize orientedSize4 = new OrientedSize(orientation, flag1 ? itemWidth : child.DesiredSize.Width, flag2 ? itemHeight : child.DesiredSize.Height);
        if (NumericExtensions.IsGreaterThan(orientedSize1.Direct + orientedSize4.Direct, orientedSize3.Direct))
        {
          orientedSize2.Direct = Math.Max(orientedSize1.Direct, orientedSize2.Direct);
          orientedSize2.Indirect += orientedSize1.Indirect;
          orientedSize1 = orientedSize4;
          if (NumericExtensions.IsGreaterThan(orientedSize4.Direct, orientedSize3.Direct))
          {
            orientedSize2.Direct = Math.Max(orientedSize4.Direct, orientedSize2.Direct);
            orientedSize2.Indirect += orientedSize4.Indirect;
            orientedSize1 = new OrientedSize(orientation);
          }
        }
        else
        {
          orientedSize1.Direct += orientedSize4.Direct;
          orientedSize1.Indirect = Math.Max(orientedSize1.Indirect, orientedSize4.Indirect);
        }
      }
      orientedSize2.Direct = Math.Max(orientedSize1.Direct, orientedSize2.Direct);
      orientedSize2.Indirect += orientedSize1.Indirect;
      return new Size(orientedSize2.Width, orientedSize2.Height);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
      Orientation orientation = this.Orientation;
      OrientedSize orientedSize1 = new OrientedSize(orientation);
      OrientedSize orientedSize2 = new OrientedSize(orientation, finalSize.Width, finalSize.Height);
      double itemWidth = this.ItemWidth;
      double itemHeight = this.ItemHeight;
      bool flag1 = !itemWidth.IsNaN();
      bool flag2 = !itemHeight.IsNaN();
      double indirectOffset = 0.0;
      double? directDelta = orientation == Orientation.Horizontal ? (flag1 ? new double?(itemWidth) : new double?()) : (flag2 ? new double?(itemHeight) : new double?());
      UIElementCollection children = this.Children;
      int count = children.Count;
      int lineStart = 0;
      for (int index = 0; index < count; ++index)
      {
        UIElement uiElement = children[index];
        OrientedSize orientedSize3 = new OrientedSize(orientation, flag1 ? itemWidth : uiElement.DesiredSize.Width, flag2 ? itemHeight : uiElement.DesiredSize.Height);
        if (NumericExtensions.IsGreaterThan(orientedSize1.Direct + orientedSize3.Direct, orientedSize2.Direct))
        {
          this.ArrangeLine(lineStart, index, directDelta, indirectOffset, orientedSize1.Indirect);
          indirectOffset += orientedSize1.Indirect;
          orientedSize1 = orientedSize3;
          if (NumericExtensions.IsGreaterThan(orientedSize3.Direct, orientedSize2.Direct))
          {
            this.ArrangeLine(index, ++index, directDelta, indirectOffset, orientedSize3.Indirect);
            indirectOffset += orientedSize1.Indirect;
            orientedSize1 = new OrientedSize(orientation);
          }
          lineStart = index;
        }
        else
        {
          orientedSize1.Direct += orientedSize3.Direct;
          orientedSize1.Indirect = Math.Max(orientedSize1.Indirect, orientedSize3.Indirect);
        }
      }
      if (lineStart < count)
        this.ArrangeLine(lineStart, count, directDelta, indirectOffset, orientedSize1.Indirect);
      return finalSize;
    }

    private void ArrangeLine(
      int lineStart,
      int lineEnd,
      double? directDelta,
      double indirectOffset,
      double indirectGrowth)
    {
      double num1 = 0.0;
      Orientation orientation = this.Orientation;
      bool flag = orientation == Orientation.Horizontal;
      UIElementCollection children = this.Children;
      for (int index = lineStart; index < lineEnd; ++index)
      {
        UIElement uiElement = children[index];
        OrientedSize orientedSize = new OrientedSize(orientation, uiElement.DesiredSize.Width, uiElement.DesiredSize.Height);
        double num2 = directDelta.HasValue ? directDelta.Value : orientedSize.Direct;
        Rect finalRect = flag ? new Rect(num1, indirectOffset, num2, indirectGrowth) : new Rect(indirectOffset, num1, indirectGrowth, num2);
        uiElement.Arrange(finalRect);
        num1 += num2;
      }
    }
  }
}
