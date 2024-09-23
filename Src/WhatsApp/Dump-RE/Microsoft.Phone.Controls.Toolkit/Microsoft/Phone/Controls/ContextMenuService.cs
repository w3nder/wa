// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.ContextMenuService
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System;
using System.Windows;

#nullable disable
namespace Microsoft.Phone.Controls
{
  public static class ContextMenuService
  {
    public static readonly DependencyProperty ContextMenuProperty = DependencyProperty.RegisterAttached("ContextMenu", typeof (ContextMenu), typeof (ContextMenuService), new PropertyMetadata((object) null, new PropertyChangedCallback(ContextMenuService.OnContextMenuChanged)));

    public static ContextMenu GetContextMenu(DependencyObject element)
    {
      return element != null ? (ContextMenu) element.GetValue(ContextMenuService.ContextMenuProperty) : throw new ArgumentNullException(nameof (element));
    }

    public static void SetContextMenu(DependencyObject element, ContextMenu value)
    {
      if (element == null)
        throw new ArgumentNullException(nameof (element));
      element.SetValue(ContextMenuService.ContextMenuProperty, (object) value);
    }

    private static void OnContextMenuChanged(
      DependencyObject o,
      DependencyPropertyChangedEventArgs e)
    {
      if (!(o is FrameworkElement frameworkElement))
        return;
      if (e.OldValue is ContextMenu oldValue)
        oldValue.Owner = (DependencyObject) null;
      if (!(e.NewValue is ContextMenu newValue))
        return;
      newValue.Owner = (DependencyObject) frameworkElement;
    }
  }
}
