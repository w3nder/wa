// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.QuadKey
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;

#nullable disable
namespace Microsoft.Phone.Controls.Maps
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  public struct QuadKey
  {
    public QuadKey(int x, int y, int zoomLevel)
      : this()
    {
      this.ZoomLevel = zoomLevel;
      this.X = x;
      this.Y = y;
    }

    public QuadKey(string quadKey)
      : this()
    {
      int x;
      int y;
      int zoomLevel;
      QuadKey.QuadKeyToQuadPixel(quadKey, out x, out y, out zoomLevel);
      this.ZoomLevel = zoomLevel;
      this.X = x;
      this.Y = y;
    }

    public int ZoomLevel { get; set; }

    public int X { get; set; }

    public int Y { get; set; }

    public string Key => QuadKey.QuadPixelToQuadKey(this.X, this.Y, this.ZoomLevel);

    private static string QuadPixelToQuadKey(int x, int y, int zoomLevel)
    {
      int num = (int) Math.Pow(2.0, (double) zoomLevel);
      string quadKey = "";
      if (y < 0 || y >= num)
        return (string) null;
      while (x < 0)
        x += num;
      while (x > num)
        x -= num;
      for (int index = 1; index <= zoomLevel; ++index)
      {
        switch (2 * (y % 2) + x % 2)
        {
          case 0:
            quadKey = "0" + quadKey;
            break;
          case 1:
            quadKey = "1" + quadKey;
            break;
          case 2:
            quadKey = "2" + quadKey;
            break;
          case 3:
            quadKey = "3" + quadKey;
            break;
        }
        x /= 2;
        y /= 2;
      }
      return quadKey;
    }

    private static void QuadKeyToQuadPixel(
      string quadKey,
      out int x,
      out int y,
      out int zoomLevel)
    {
      x = 0;
      y = 0;
      zoomLevel = 0;
      if (string.IsNullOrEmpty(quadKey))
        return;
      zoomLevel = quadKey.Length;
      for (int index = 0; index < quadKey.Length; ++index)
      {
        switch (quadKey[index])
        {
          case '0':
            x *= 2;
            y *= 2;
            break;
          case '1':
            x = x * 2 + 1;
            y *= 2;
            break;
          case '2':
            x *= 2;
            y = y * 2 + 1;
            break;
          case '3':
            x = x * 2 + 1;
            y = y * 2 + 1;
            break;
        }
      }
    }

    public static bool operator ==(QuadKey tile1, QuadKey tile2)
    {
      return tile1.X == tile2.X && tile1.Y == tile2.Y && tile1.ZoomLevel == tile2.ZoomLevel;
    }

    public static bool operator !=(QuadKey tile1, QuadKey tile2) => !(tile1 == tile2);

    public override bool Equals(object obj)
    {
      return obj != null && obj is QuadKey quadKey && this == quadKey;
    }

    public override int GetHashCode()
    {
      return this.X.GetHashCode() ^ this.Y.GetHashCode() ^ this.ZoomLevel.GetHashCode();
    }
  }
}
