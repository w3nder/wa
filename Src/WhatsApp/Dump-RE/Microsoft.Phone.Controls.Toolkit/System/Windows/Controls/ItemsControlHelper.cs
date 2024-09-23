// Decompiled with JetBrains decompiler
// Type: System.Windows.Controls.ItemsControlHelper
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System.Windows.Media;

#nullable disable
namespace System.Windows.Controls
{
  internal sealed class ItemsControlHelper
  {
    private Panel _itemsHost;
    private ScrollViewer _scrollHost;

    private ItemsControl ItemsControl { get; set; }

    internal Panel ItemsHost
    {
      get
      {
        if (this._itemsHost == null && this.ItemsControl != null && this.ItemsControl.ItemContainerGenerator != null)
        {
          DependencyObject reference = this.ItemsControl.ItemContainerGenerator.ContainerFromIndex(0);
          if (reference != null)
            this._itemsHost = VisualTreeHelper.GetParent(reference) as Panel;
        }
        return this._itemsHost;
      }
    }

    internal ScrollViewer ScrollHost
    {
      get
      {
        if (this._scrollHost == null)
        {
          Panel itemsHost = this.ItemsHost;
          if (itemsHost != null)
          {
            for (DependencyObject reference = (DependencyObject) itemsHost; reference != this.ItemsControl && reference != null; reference = VisualTreeHelper.GetParent(reference))
            {
              if (reference is ScrollViewer scrollViewer)
              {
                this._scrollHost = scrollViewer;
                break;
              }
            }
          }
        }
        return this._scrollHost;
      }
    }

    internal ItemsControlHelper(ItemsControl control) => this.ItemsControl = control;

    internal void OnApplyTemplate()
    {
      this._itemsHost = (Panel) null;
      this._scrollHost = (ScrollViewer) null;
    }

    internal static void PrepareContainerForItemOverride(
      DependencyObject element,
      Style parentItemContainerStyle)
    {
      Control control = element as Control;
      if (parentItemContainerStyle == null || control == null || control.Style != null)
        return;
      control.SetValue(FrameworkElement.StyleProperty, (object) parentItemContainerStyle);
    }

    internal void UpdateItemContainerStyle(Style itemContainerStyle)
    {
      if (itemContainerStyle == null)
        return;
      Panel itemsHost = this.ItemsHost;
      if (itemsHost == null || itemsHost.Children == null)
        return;
      foreach (UIElement child in (PresentationFrameworkCollection<UIElement>) itemsHost.Children)
      {
        FrameworkElement frameworkElement = child as FrameworkElement;
        if (frameworkElement.Style == null)
          frameworkElement.Style = itemContainerStyle;
      }
    }

    internal void ScrollIntoView(FrameworkElement element)
    {
      ScrollViewer scrollHost = this.ScrollHost;
      if (scrollHost == null)
        return;
      GeneralTransform visual;
      try
      {
        visual = element.TransformToVisual((UIElement) scrollHost);
      }
      catch (ArgumentException ex)
      {
        return;
      }
      Rect rect = new Rect(visual.Transform(new Point()), visual.Transform(new Point(element.ActualWidth, element.ActualHeight)));
      double verticalOffset = scrollHost.VerticalOffset;
      double num1 = 0.0;
      double viewportHeight = scrollHost.ViewportHeight;
      double bottom = rect.Bottom;
      if (viewportHeight < bottom)
      {
        num1 = bottom - viewportHeight;
        verticalOffset += num1;
      }
      double top = rect.Top;
      if (top - num1 < 0.0)
        verticalOffset -= num1 - top;
      scrollHost.ScrollToVerticalOffset(verticalOffset);
      double horizontalOffset = scrollHost.HorizontalOffset;
      double num2 = 0.0;
      double viewportWidth = scrollHost.ViewportWidth;
      double right = rect.Right;
      if (viewportWidth < right)
      {
        num2 = right - viewportWidth;
        horizontalOffset += num2;
      }
      double left = rect.Left;
      if (left - num2 < 0.0)
        horizontalOffset -= num2 - left;
      scrollHost.ScrollToHorizontalOffset(horizontalOffset);
    }
  }
}
