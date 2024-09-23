// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.ItemsControlExtensions
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

#nullable disable
namespace Microsoft.Phone.Controls
{
  internal static class ItemsControlExtensions
  {
    public static IList<WeakReference> GetItemsInViewPort(this ItemsControl list)
    {
      IList<WeakReference> items = (IList<WeakReference>) new List<WeakReference>();
      list.GetItemsInViewPort(items);
      return items;
    }

    public static void GetItemsInViewPort(this ItemsControl list, IList<WeakReference> items)
    {
      if (VisualTreeHelper.GetChildrenCount((DependencyObject) list) == 0)
        return;
      ScrollViewer child = VisualTreeHelper.GetChild((DependencyObject) list, 0) as ScrollViewer;
      list.UpdateLayout();
      if (child == null)
        return;
      int index;
      GeneralTransform generalTransform;
      Rect rect;
      for (index = 0; index < list.Items.Count; ++index)
      {
        FrameworkElement target = (FrameworkElement) list.ItemContainerGenerator.ContainerFromIndex(index);
        if (target != null)
        {
          generalTransform = (GeneralTransform) null;
          GeneralTransform visual;
          try
          {
            visual = target.TransformToVisual((UIElement) child);
          }
          catch (ArgumentException ex)
          {
            return;
          }
          rect = new Rect(visual.Transform(new Point()), visual.Transform(new Point(target.ActualWidth, target.ActualHeight)));
          if (rect.Bottom > 0.0)
          {
            items.Add(new WeakReference((object) target));
            ++index;
            break;
          }
        }
      }
      for (; index < list.Items.Count; ++index)
      {
        FrameworkElement target = (FrameworkElement) list.ItemContainerGenerator.ContainerFromIndex(index);
        generalTransform = (GeneralTransform) null;
        GeneralTransform visual;
        try
        {
          visual = target.TransformToVisual((UIElement) child);
        }
        catch (ArgumentException ex)
        {
          break;
        }
        rect = new Rect(visual.Transform(new Point()), visual.Transform(new Point(target.ActualWidth, target.ActualHeight)));
        if (rect.Top >= child.ActualHeight)
          break;
        items.Add(new WeakReference((object) target));
      }
    }
  }
}
