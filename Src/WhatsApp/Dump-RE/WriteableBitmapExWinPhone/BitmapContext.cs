// Decompiled with JetBrains decompiler
// Type: System.Windows.Media.Imaging.BitmapContext
// Assembly: WriteableBitmapExWinPhone, Version=1.0.9.0, Culture=neutral, PublicKeyToken=null
// MVID: 8B7E3D19-074F-4D11-AD72-780A064DB6A8
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WriteableBitmapExWinPhone.dll

#nullable disable
namespace System.Windows.Media.Imaging
{
  public struct BitmapContext : IDisposable
  {
    private readonly WriteableBitmap writeableBitmap;
    private readonly ReadWriteMode mode;

    public WriteableBitmap WriteableBitmap => this.writeableBitmap;

    public int Width => this.writeableBitmap.PixelWidth;

    public int Height => this.writeableBitmap.PixelHeight;

    public BitmapContext(WriteableBitmap writeableBitmap)
      : this(writeableBitmap, ReadWriteMode.ReadWrite)
    {
    }

    public BitmapContext(WriteableBitmap writeableBitmap, ReadWriteMode mode)
    {
      this.writeableBitmap = writeableBitmap;
      this.mode = mode;
    }

    public int[] Pixels => this.writeableBitmap.Pixels;

    public int Length => this.writeableBitmap.Pixels.Length;

    public static void BlockCopy(
      BitmapContext src,
      int srcOffset,
      BitmapContext dest,
      int destOffset,
      int count)
    {
      Buffer.BlockCopy((Array) src.Pixels, srcOffset, (Array) dest.Pixels, destOffset, count);
    }

    public static void BlockCopy(
      Array src,
      int srcOffset,
      BitmapContext dest,
      int destOffset,
      int count)
    {
      Buffer.BlockCopy(src, srcOffset, (Array) dest.Pixels, destOffset, count);
    }

    public static void BlockCopy(
      BitmapContext src,
      int srcOffset,
      Array dest,
      int destOffset,
      int count)
    {
      Buffer.BlockCopy((Array) src.Pixels, srcOffset, dest, destOffset, count);
    }

    public void Clear()
    {
      int[] pixels = this.writeableBitmap.Pixels;
      Array.Clear((Array) pixels, 0, pixels.Length);
    }

    public void Dispose()
    {
    }
  }
}
