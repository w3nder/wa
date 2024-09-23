// Decompiled with JetBrains decompiler
// Type: WhatsApp.IconUtils
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

#nullable disable
namespace WhatsApp
{
  public class IconUtils
  {
    public static WriteableBitmap CreateBackgroundThemeIcon(BitmapSource iconSource, double? size = null)
    {
      Canvas element = IconUtils.DrawBackgroundThemeIcon(iconSource, size);
      return element != null ? new WriteableBitmap((UIElement) element, (Transform) null) : (WriteableBitmap) null;
    }

    public static WriteableBitmap CreateColorIcon(BitmapSource iconSrc, Brush brush, double? size)
    {
      Canvas element = IconUtils.DrawForegroundColorIcon(iconSrc, brush, size);
      return element != null ? new WriteableBitmap((UIElement) element, (Transform) null) : (WriteableBitmap) null;
    }

    public static WriteableBitmap CreateColorIcon(
      BitmapSource iconSrc,
      Brush brush,
      Size? iconSize = null)
    {
      Canvas element = IconUtils.DrawForegroundColorIcon(iconSrc, brush, iconSize);
      return element != null ? new WriteableBitmap((UIElement) element, (Transform) null) : (WriteableBitmap) null;
    }

    public static WriteableBitmap CreateAccentIcon(string iconPath, double? size = null)
    {
      return IconUtils.CreateColorIcon((BitmapSource) ImageStore.GetStockIcon(iconPath, iconPath), (Brush) UIUtils.AccentBrush, size);
    }

    public static WriteableBitmap CreateAccentIcon(BitmapSource iconSrc, double? size = null)
    {
      return IconUtils.CreateColorIcon(iconSrc, (Brush) UIUtils.AccentBrush, size);
    }

    public static WriteableBitmap CreateAccentIcon(BitmapSource iconSrc, Size? iconSize = null)
    {
      return IconUtils.CreateColorIcon(iconSrc, (Brush) UIUtils.AccentBrush, iconSize);
    }

    private static Canvas DrawIconBackground(
      Size iconImgSize,
      Size? iconSize,
      Brush backgroundBrush,
      out double scale)
    {
      scale = 1.0;
      double num1;
      double num2;
      if (iconSize.HasValue)
      {
        Size size = iconSize.Value;
        num1 = size.Width;
        size = iconSize.Value;
        num2 = size.Height;
        double val1 = num1 < iconImgSize.Width ? num1 / iconImgSize.Width : 1.0;
        double val2 = num2 < iconImgSize.Height ? num2 / iconImgSize.Height : 1.0;
        scale = Math.Min(val1, val2);
      }
      else
      {
        num1 = iconImgSize.Width / 0.6;
        num2 = iconImgSize.Height / 0.6;
      }
      Canvas canvas = new Canvas();
      canvas.Width = num1;
      canvas.Height = num2;
      canvas.Background = backgroundBrush;
      return canvas;
    }

    private static Canvas DrawForegroundColorIcon(BitmapSource iconSrc, Brush brush, double? size = null)
    {
      Size? size1 = new Size?();
      if (size.HasValue)
        size1 = new Size?(new Size(size.Value, size.Value));
      return IconUtils.DrawForegroundColorIcon(iconSrc, brush, size1);
    }

    private static Canvas DrawForegroundColorIcon(BitmapSource iconSrc, Brush brush, Size? size)
    {
      if (iconSrc == null)
        return (Canvas) null;
      double scale = 1.0;
      Canvas canvas = IconUtils.DrawIconBackground(new Size((double) iconSrc.PixelWidth, (double) iconSrc.PixelHeight), size, brush, out scale);
      System.Windows.Media.ImageSource imageSource;
      if ((Math.Abs(scale - 1.0) > 0.0001 ? 1 : 0) != 0)
      {
        if (!(iconSrc is WriteableBitmap bitmap))
          bitmap = new WriteableBitmap(iconSrc);
        imageSource = (System.Windows.Media.ImageSource) bitmap.Scale(scale);
      }
      else
        imageSource = (System.Windows.Media.ImageSource) iconSrc;
      ImageBrush imageBrush = new ImageBrush();
      imageBrush.Stretch = Stretch.None;
      imageBrush.ImageSource = imageSource;
      canvas.OpacityMask = (Brush) imageBrush;
      return canvas;
    }

    public static Canvas DrawBackgroundThemeIcon(BitmapSource iconSource, double? size = null)
    {
      BitmapSource bitmapSource = iconSource;
      if (bitmapSource == null)
        return (Canvas) null;
      double scale = 1.0;
      Size? iconSize = new Size?();
      if (size.HasValue)
        iconSize = new Size?(new Size(size.Value, size.Value));
      Size iconImgSize = new Size((double) bitmapSource.PixelWidth, (double) bitmapSource.PixelHeight);
      Canvas canvas = IconUtils.DrawIconBackground(iconImgSize, iconSize, Application.Current.Resources[(object) "PhoneAccentBrush"] as Brush, out scale);
      Image element;
      if ((Math.Abs(scale - 1.0) > 0.0001 ? 1 : 0) != 0)
      {
        int pixelWidth = bitmapSource.PixelWidth;
        int pixelHeight = bitmapSource.PixelHeight;
        Image image = new Image();
        image.Source = (System.Windows.Media.ImageSource) bitmapSource;
        image.Width = (double) pixelWidth;
        image.Height = (double) pixelHeight;
        image.RenderTransform = (Transform) new ScaleTransform()
        {
          ScaleX = scale,
          ScaleY = scale
        };
        element = image;
        iconImgSize.Width = (double) pixelWidth * scale;
        iconImgSize.Height = (double) pixelHeight * scale;
      }
      else
        element = new Image()
        {
          Source = (System.Windows.Media.ImageSource) bitmapSource
        };
      canvas.Children.Add((UIElement) element);
      Canvas.SetLeft((UIElement) element, (canvas.Width - iconImgSize.Width) / 2.0);
      Canvas.SetTop((UIElement) element, (canvas.Height - iconImgSize.Height) / 2.0);
      return canvas;
    }
  }
}
