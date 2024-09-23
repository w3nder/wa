// Decompiled with JetBrains decompiler
// Type: WhatsApp.ImageUtils
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

#nullable disable
namespace WhatsApp
{
  public static class ImageUtils
  {
    public static bool CheckPrevalentImageAreaColor(
      BitmapSource img,
      List<Color> colors,
      System.Windows.Point anchor,
      int width,
      int height)
    {
      int num1 = -1;
      if (!(img is WriteableBitmap writeableBitmap1))
        writeableBitmap1 = new WriteableBitmap(img);
      WriteableBitmap writeableBitmap2 = writeableBitmap1;
      int[] pixels = writeableBitmap2.Pixels;
      int x = (int) anchor.X;
      int y = (int) anchor.Y;
      int index = x + y * writeableBitmap2.PixelWidth;
      int num2 = Math.Min(pixels.Length, x + width + (y + height) * writeableBitmap2.PixelWidth);
      int num3 = (width + height * writeableBitmap2.PixelWidth) / 10;
      for (; index < num2; index += num3)
      {
        int pixel = pixels[index];
        num1 += colors.Any<Color>((Func<Color, bool>) (c => ImageUtils.AreClose(pixel, c))) ? 1 : -1;
      }
      return num1 >= 0;
    }

    private static bool AreClose(int c1, Color c2)
    {
      int num1 = (int) (byte) (c1 >>> 24);
      byte num2 = (byte) (c1 >>> 16);
      byte num3 = (byte) (c1 >>> 8);
      byte num4 = (byte) (c1 & (int) byte.MaxValue);
      int a = (int) c2.A;
      return num1 == a && (int) num2 <= (int) c2.R + 1 && (int) num2 >= (int) c2.R - 1 && (int) num3 <= (int) c2.G + 2 && (int) num3 >= (int) c2.G - 2 && (int) num4 <= (int) c2.B + 2 && (int) num4 >= (int) c2.B - 2;
    }
  }
}
