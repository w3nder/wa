// Decompiled with JetBrains decompiler
// Type: ScreenSizeSupport.Misc.DisplayConstants
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;

#nullable disable
namespace ScreenSizeSupport.Misc
{
  public static class DisplayConstants
  {
    public const double AspectRatio16To9 = 1.7777777777777777;
    public const double AspectRatio15To9 = 1.6666666666666667;
    public static readonly double DiagonalToWidthRatio16To9 = 9.0 / Math.Sqrt(Math.Pow(16.0, 2.0) + Math.Pow(9.0, 2.0));
    public static readonly double DiagonalToWidthRatio15To9 = 9.0 / Math.Sqrt(Math.Pow(15.0, 2.0) + Math.Pow(9.0, 2.0));
    public const double BaselineDiagonalInInches15To9HighRes = 4.5;
    public const double BaselineDiagonalInInches15To9LoRes = 4.0;
    public const double BaselineDiagonalInInches16To9 = 4.3;
    internal static readonly double BaselineWidthInInches = 4.5 * DisplayConstants.DiagonalToWidthRatio15To9;
    internal const int BaselineWidthInViewPixels = 480;
  }
}
