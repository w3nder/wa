// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaBitmapEx
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace WhatsApp
{
  public static class WaBitmapEx
  {
    public static WriteableBitmap Crop(this WriteableBitmap bitmap, System.Windows.Point cropPos, Size cropSize)
    {
      return bitmap.Crop(new Rect(cropPos, cropSize));
    }

    public static WriteableBitmap CropRelatively(
      this WriteableBitmap bitmap,
      System.Windows.Point relativeCropPos,
      Size relativeCropSize)
    {
      System.Windows.Point location = new System.Windows.Point(relativeCropPos.X * (double) bitmap.PixelWidth, relativeCropPos.Y * (double) bitmap.PixelHeight);
      Size size = new Size(relativeCropSize.Width * (double) bitmap.PixelWidth, relativeCropSize.Height * (double) bitmap.PixelHeight);
      return bitmap.Crop(new Rect(location, size));
    }

    public static WriteableBitmap Scale(this WriteableBitmap bitmap, double scale)
    {
      double width = (double) bitmap.PixelWidth * scale;
      double height = (double) bitmap.PixelHeight * scale;
      return bitmap.Resize((int) width, (int) height, WriteableBitmapExtensions.Interpolation.Bilinear);
    }

    public static WriteableBitmap Transform(this BitmapSource originalSrc, System.Windows.Media.Transform transform)
    {
      Image image = new Image();
      image.Source = (ImageSource) originalSrc;
      image.Width = (double) originalSrc.PixelWidth;
      image.Height = (double) originalSrc.PixelHeight;
      Image element = image;
      try
      {
        return new WriteableBitmap((UIElement) element, transform);
      }
      finally
      {
        element.Source = (ImageSource) null;
      }
    }

    public static WriteableBitmap Blur(this WriteableBitmap bitmap)
    {
      if (bitmap == null)
        return (WriteableBitmap) null;
      WriteableBitmap bmp = bitmap.Convolute(WriteableBitmapExtensions.KernelGaussianBlur3x3);
      if (AppState.IsDecentMemoryDevice)
        bmp = bmp.Convolute(WriteableBitmapExtensions.KernelGaussianBlur3x3);
      return bmp;
    }

    public static byte[] ToJpegByteArray(
      this WriteableBitmap bitmap,
      int maxResultBytes,
      int? jpegQuality = null)
    {
      return bitmap.ToJpegByteArray(bitmap.PixelWidth, bitmap.PixelHeight, maxResultBytes, jpegQuality);
    }

    public static byte[] ToJpegByteArray(
      this WriteableBitmap bitmap,
      int targetWidth,
      int targetHeight,
      int maxResultBytes,
      int? jpegQuality = null)
    {
      MemoryStream jpegStream = bitmap.ToJpegStream(targetWidth, targetHeight, maxResultBytes, jpegQuality);
      if (jpegStream == null)
        return (byte[]) null;
      using (jpegStream)
      {
        jpegStream.Position = 0L;
        return jpegStream.ToArray();
      }
    }

    public static MemoryStream ToJpegStream(
      this WriteableBitmap bitmap,
      int maxResultBytes,
      int? jpegQuality = null)
    {
      return bitmap.ToJpegStream(bitmap.PixelWidth, bitmap.PixelHeight, maxResultBytes, jpegQuality);
    }

    public static MemoryStream ToJpegStream(
      this WriteableBitmap bitmap,
      int targetWidth,
      int targetHeight,
      int maxResultBytes,
      int? jpegQuality = null)
    {
      if (bitmap == null)
        return (MemoryStream) null;
      MemoryStream jpegStream = new MemoryStream();
      try
      {
        bitmap.SaveJpegWithMaxSize((Stream) jpegStream, targetWidth, targetHeight, 0, jpegQuality ?? Settings.JpegQuality, maxResultBytes);
        jpegStream.Position = 0L;
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "to jpeg stream");
        jpegStream = (MemoryStream) null;
      }
      return jpegStream;
    }
  }
}
