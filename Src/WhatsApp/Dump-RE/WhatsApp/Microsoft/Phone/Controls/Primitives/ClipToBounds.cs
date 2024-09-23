// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Primitives.ClipToBounds
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

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
