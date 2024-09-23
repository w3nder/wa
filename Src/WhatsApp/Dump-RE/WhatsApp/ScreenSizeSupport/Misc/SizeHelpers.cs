// Decompiled with JetBrains decompiler
// Type: ScreenSizeSupport.Misc.SizeHelpers
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Windows;

#nullable disable
namespace ScreenSizeSupport.Misc
{
  public static class SizeHelpers
  {
    public static readonly Size WvgaPhysicalResolution = new Size(480.0, 800.0);
    public static readonly Size Hd720PhysicalResolution = new Size(720.0, 1280.0);
    public static readonly Size WxgaPhysicalResolution = new Size(768.0, 1280.0);
    public static readonly Size FullHd1080PhysicalResolution = new Size(1080.0, 1920.0);

    public static Size Scale(this Size size, double scaleFactor)
    {
      Size size1 = new Size();
      double height = size.Height;
      size1.Height = double.IsInfinity(height) ? height : height * scaleFactor;
      double width = size.Width;
      size1.Width = double.IsInfinity(width) ? width : width * scaleFactor;
      return size1;
    }

    public static double GetWidthInInchesFromDiagonal(double diagonal, double aspectRatio)
    {
      if (aspectRatio.IsCloseEnoughTo(16.0 / 9.0))
        return diagonal * DisplayConstants.DiagonalToWidthRatio16To9;
      if (aspectRatio.IsCloseEnoughTo(5.0 / 3.0))
        return diagonal * DisplayConstants.DiagonalToWidthRatio15To9;
      if (aspectRatio.IsCloseEnoughTo(0.0))
        return 0.0;
      throw new ArgumentOutOfRangeException(nameof (aspectRatio));
    }

    public static Size MakeSize(double width, double aspectRatio)
    {
      return new Size(width, width * aspectRatio);
    }

    public static Size MakeSizeFromDiagonal(double diagonal, double aspectRatio)
    {
      return SizeHelpers.MakeSize(SizeHelpers.GetWidthInInchesFromDiagonal(diagonal, aspectRatio), aspectRatio);
    }
  }
}
