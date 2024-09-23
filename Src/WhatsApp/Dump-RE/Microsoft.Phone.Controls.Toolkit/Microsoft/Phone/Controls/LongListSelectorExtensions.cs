// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.LongListSelectorExtensions
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

#nullable disable
namespace Microsoft.Phone.Controls
{
  internal static class LongListSelectorExtensions
  {
    public static void GetItemsInViewPort(this LongListSelector list, IList<WeakReference> items)
    {
      DependencyObject reference = (DependencyObject) list;
      if (VisualTreeHelper.GetChildrenCount((DependencyObject) list) == 0)
        return;
      list.UpdateLayout();
      int childrenCount;
      do
      {
        reference = VisualTreeHelper.GetChild(reference, 0);
        childrenCount = VisualTreeHelper.GetChildrenCount(reference);
      }
      while (!(reference is Canvas canvas) && childrenCount > 0);
      if (canvas == null || childrenCount <= 0 || !(VisualTreeHelper.GetChild((DependencyObject) canvas, 0) is Canvas child))
        return;
      List<KeyValuePair<double, FrameworkElement>> keyValuePairList = new List<KeyValuePair<double, FrameworkElement>>();
      LongListSelectorExtensions.AddVisibileContainers(list, child, keyValuePairList, false);
      LongListSelectorExtensions.AddVisibileContainers(list, canvas, keyValuePairList, true);
      foreach (KeyValuePair<double, FrameworkElement> keyValuePair in (IEnumerable<KeyValuePair<double, FrameworkElement>>) keyValuePairList.OrderBy<KeyValuePair<double, FrameworkElement>, double>((Func<KeyValuePair<double, FrameworkElement>, double>) (selector => selector.Key)))
        items.Add(new WeakReference((object) keyValuePair.Value));
    }

    private static void AddVisibileContainers(
      LongListSelector list,
      Canvas itemsPanel,
      List<KeyValuePair<double, FrameworkElement>> items,
      bool selectContent)
    {
      foreach (DependencyObject visualChild in itemsPanel.GetVisualChildren())
      {
        if (visualChild is ContentPresenter reference && (!selectContent || VisualTreeHelper.GetChildrenCount((DependencyObject) reference) == 1 && VisualTreeHelper.GetChild((DependencyObject) reference, 0) is FrameworkElement))
        {
          GeneralTransform visual;
          try
          {
            visual = reference.TransformToVisual((UIElement) list);
          }
          catch (ArgumentException ex)
          {
            break;
          }
          Rect rect = new Rect(visual.Transform(new Point()), visual.Transform(new Point(reference.ActualWidth, reference.ActualHeight)));
          if (rect.Bottom > 0.0 && rect.Top < list.ActualHeight)
            items.Add(new KeyValuePair<double, FrameworkElement>(rect.Top, selectContent ? (FrameworkElement) VisualTreeHelper.GetChild((DependencyObject) reference, 0) : (FrameworkElement) reference));
        }
      }
    }
  }
}
