// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.TransitionService
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System;
using System.Windows;

#nullable disable
namespace Microsoft.Phone.Controls
{
  public static class TransitionService
  {
    public static readonly DependencyProperty NavigationInTransitionProperty = DependencyProperty.RegisterAttached("NavigationInTransition", typeof (NavigationInTransition), typeof (TransitionService), (PropertyMetadata) null);
    public static readonly DependencyProperty NavigationOutTransitionProperty = DependencyProperty.RegisterAttached("NavigationOutTransition", typeof (NavigationOutTransition), typeof (TransitionService), (PropertyMetadata) null);

    public static NavigationInTransition GetNavigationInTransition(UIElement element)
    {
      return element != null ? (NavigationInTransition) element.GetValue(TransitionService.NavigationInTransitionProperty) : throw new ArgumentNullException(nameof (element));
    }

    public static NavigationOutTransition GetNavigationOutTransition(UIElement element)
    {
      return element != null ? (NavigationOutTransition) element.GetValue(TransitionService.NavigationOutTransitionProperty) : throw new ArgumentNullException(nameof (element));
    }

    public static void SetNavigationInTransition(UIElement element, NavigationInTransition value)
    {
      if (element == null)
        throw new ArgumentNullException(nameof (element));
      element.SetValue(TransitionService.NavigationInTransitionProperty, (object) value);
    }

    public static void SetNavigationOutTransition(UIElement element, NavigationOutTransition value)
    {
      if (element == null)
        throw new ArgumentNullException(nameof (element));
      element.SetValue(TransitionService.NavigationOutTransitionProperty, (object) value);
    }
  }
}
