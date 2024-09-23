// Decompiled with JetBrains decompiler
// Type: ZXing.Dimension
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;

#nullable disable
namespace ZXing
{
  /// <summary>Simply encapsulates a width and height.</summary>
  public sealed class Dimension
  {
    private readonly int width;
    private readonly int height;

    public Dimension(int width, int height)
    {
      this.width = width >= 0 && height >= 0 ? width : throw new ArgumentException();
      this.height = height;
    }

    public int Width => this.width;

    public int Height => this.height;

    public override bool Equals(object other)
    {
      if (!(other is Dimension))
        return false;
      Dimension dimension = (Dimension) other;
      return this.width == dimension.width && this.height == dimension.height;
    }

    public override int GetHashCode() => this.width * 32713 + this.height;

    public override string ToString() => this.width.ToString() + "x" + (object) this.height;
  }
}
