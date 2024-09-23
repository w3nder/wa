// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.GestureService
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System;
using System.Windows;

#nullable disable
namespace Microsoft.Phone.Controls
{
  public static class GestureService
  {
    public static readonly DependencyProperty GestureListenerProperty = DependencyProperty.RegisterAttached("GestureListener", typeof (GestureListener), typeof (GestureService), new PropertyMetadata((PropertyChangedCallback) null));

    public static GestureListener GetGestureListener(DependencyObject obj)
    {
      return obj != null ? GestureService.GetGestureListenerInternal(obj, true) : throw new ArgumentNullException(nameof (obj));
    }

    internal static GestureListener GetGestureListenerInternal(
      DependencyObject obj,
      bool createIfMissing)
    {
      GestureListener listenerInternal = (GestureListener) obj.GetValue(GestureService.GestureListenerProperty);
      if (listenerInternal == null && createIfMissing)
      {
        listenerInternal = new GestureListener();
        GestureService.SetGestureListenerInternal(obj, listenerInternal);
      }
      return listenerInternal;
    }

    [Obsolete("Do not add handlers using this method. Instead, use GetGestureListener, which will create a new instance if one is not already set, to add your handlers to an element.", true)]
    public static void SetGestureListener(DependencyObject obj, GestureListener value)
    {
      if (obj == null)
        throw new ArgumentNullException(nameof (obj));
      GestureService.SetGestureListenerInternal(obj, value);
    }

    private static void SetGestureListenerInternal(DependencyObject obj, GestureListener value)
    {
      obj.SetValue(GestureService.GestureListenerProperty, (object) value);
    }
  }
}
