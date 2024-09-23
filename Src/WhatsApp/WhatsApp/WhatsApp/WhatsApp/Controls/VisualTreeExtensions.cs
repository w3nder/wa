// Decompiled with JetBrains decompiler
// Type: WhatsApp.Controls.VisualTreeExtensions
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System.Windows;
using System.Windows.Media;


namespace WhatsApp.Controls
{
  internal static class VisualTreeExtensions
  {
    internal static T GetParentByType<T>(this DependencyObject element) where T : FrameworkElement
    {
      T obj = default (T);
      for (DependencyObject parent = VisualTreeHelper.GetParent(element); parent != null; parent = VisualTreeHelper.GetParent(parent))
      {
        if (parent is T parentByType)
          return parentByType;
      }
      return default (T);
    }
  }
}
