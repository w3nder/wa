// Decompiled with JetBrains decompiler
// Type: Coding4Fun.Phone.Controls.ButtonBaseHelper
// Assembly: Coding4Fun.Phone.Controls, Version=1.6.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5583BFDF-52F3-4F66-A397-92165DEE5729
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Coding4Fun.Phone.Controls.dll

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

#nullable disable
namespace Coding4Fun.Phone.Controls
{
  internal static class ButtonBaseHelper
  {
    public static void OnImageChange(DependencyPropertyChangedEventArgs e, ImageBrush brush)
    {
      if (e.NewValue == e.OldValue)
        return;
      ButtonBaseHelper.SetImageBrush(brush, e.NewValue as ImageSource);
    }

    public static void SetImageBrush(ImageBrush brush, ImageSource imageSource)
    {
      if (brush == null)
        return;
      brush.ImageSource = imageSource;
    }

    public static void OnStretch(DependencyPropertyChangedEventArgs e, ImageBrush brush)
    {
      if (e.NewValue == e.OldValue)
        return;
      ButtonBaseHelper.SetStretch(brush, (Stretch) e.NewValue);
    }

    public static void SetStretch(ImageBrush brush, Stretch stretch)
    {
      if (brush == null)
        return;
      ((TileBrush) brush).Stretch = stretch;
    }

    public static void ApplyTemplate(
      FrameworkElement item,
      ImageBrush brush,
      ContentControl contentBody,
      Stretch stretch,
      DependencyProperty imageDependencyProperty)
    {
      ButtonBaseHelper.SetStretch(brush, stretch);
      if (contentBody != null)
      {
        double num1 = -(((Control) contentBody).FontSize / 8.0);
        double num2 = -(((Control) contentBody).FontSize / 2.0) - num1;
        ((FrameworkElement) contentBody).Margin = new Thickness(0.0, num2, 0.0, num1);
      }
      if (!(((DependencyObject) item).GetValue(imageDependencyProperty) is ImageSource imageSource))
        ((DependencyObject) item).SetValue(imageDependencyProperty, (object) new BitmapImage(new Uri("/Coding4Fun.Phone.Controls;component/Media/icons/appbar.check.rest.png", UriKind.RelativeOrAbsolute)));
      else
        ButtonBaseHelper.SetImageBrush(brush, imageSource);
    }
  }
}
