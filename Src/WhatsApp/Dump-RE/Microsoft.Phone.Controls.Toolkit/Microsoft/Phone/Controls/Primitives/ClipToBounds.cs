// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Primitives.ClipToBounds
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System.Windows;
using System.Windows.Media;

#nullable disable
namespace Microsoft.Phone.Controls.Primitives
{
  public class ClipToBounds : DependencyObject
  {
    public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof (bool), typeof (ClipToBounds), new PropertyMetadata((object) false, new PropertyChangedCallback(ClipToBounds.OnIsEnabledPropertyChanged)));

    public static bool GetIsEnabled(DependencyObject obj)
    {
      return (bool) obj.GetValue(ClipToBounds.IsEnabledProperty);
    }

    public static void SetIsEnabled(DependencyObject obj, bool value)
    {
      obj.SetValue(ClipToBounds.IsEnabledProperty, (object) value);
    }

    private static void OnIsEnabledPropertyChanged(
      DependencyObject obj,
      DependencyPropertyChangedEventArgs e)
    {
      if (!(obj is FrameworkElement frameworkElement))
        return;
      if ((bool) e.NewValue)
        frameworkElement.SizeChanged += new SizeChangedEventHandler(ClipToBounds.OnSizeChanged);
      else
        frameworkElement.SizeChanged -= new SizeChangedEventHandler(ClipToBounds.OnSizeChanged);
    }

    private static void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
      if (!(sender is FrameworkElement frameworkElement))
        return;
      frameworkElement.Clip = (Geometry) new RectangleGeometry()
      {
        Rect = new Rect(0.0, 0.0, frameworkElement.ActualWidth, frameworkElement.ActualHeight)
      };
    }
  }
}
