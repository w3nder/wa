// Decompiled with JetBrains decompiler
// Type: ZXing.RGBLuminanceSource
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;

#nullable disable
namespace ZXing
{
  /// <summary>
  /// Luminance source class which support different formats of images.
  /// </summary>
  public class RGBLuminanceSource : BaseLuminanceSource
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.RGBLuminanceSource" /> class.
    /// </summary>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    protected RGBLuminanceSource(int width, int height)
      : base(width, height)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.RGBLuminanceSource" /> class.
    /// It supports a byte array with 3 bytes per pixel (RGB24).
    /// </summary>
    /// <param name="rgbRawBytes">The RGB raw bytes.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    public RGBLuminanceSource(byte[] rgbRawBytes, int width, int height)
      : this(rgbRawBytes, width, height, RGBLuminanceSource.BitmapFormat.RGB24)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.RGBLuminanceSource" /> class.
    /// It supports a byte array with 1 byte per pixel (Gray8).
    /// That means the whole array consists of the luminance values (grayscale).
    /// </summary>
    /// <param name="luminanceArray">The luminance array.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <param name="is8Bit">if set to <c>true</c> [is8 bit].</param>
    [Obsolete("Use RGBLuminanceSource(luminanceArray, width, height, BitmapFormat.Gray8)")]
    public RGBLuminanceSource(byte[] luminanceArray, int width, int height, bool is8Bit)
      : this(luminanceArray, width, height, RGBLuminanceSource.BitmapFormat.Gray8)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.RGBLuminanceSource" /> class.
    /// It supports a byte array with 3 bytes per pixel (RGB24).
    /// </summary>
    /// <param name="rgbRawBytes">The RGB raw bytes.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <param name="bitmapFormat">The bitmap format.</param>
    public RGBLuminanceSource(
      byte[] rgbRawBytes,
      int width,
      int height,
      RGBLuminanceSource.BitmapFormat bitmapFormat)
      : base(width, height)
    {
      this.CalculateLuminance(rgbRawBytes, bitmapFormat);
    }

    /// <summary>
    /// Should create a new luminance source with the right class type.
    /// The method is used in methods crop and rotate.
    /// </summary>
    /// <param name="newLuminances">The new luminances.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <returns></returns>
    protected override LuminanceSource CreateLuminanceSource(
      byte[] newLuminances,
      int width,
      int height)
    {
      RGBLuminanceSource luminanceSource = new RGBLuminanceSource(width, height);
      luminanceSource.luminances = newLuminances;
      return (LuminanceSource) luminanceSource;
    }

    private static RGBLuminanceSource.BitmapFormat DetermineBitmapFormat(
      byte[] rgbRawBytes,
      int width,
      int height)
    {
      int num = width * height;
      switch (rgbRawBytes.Length / num)
      {
        case 1:
          return RGBLuminanceSource.BitmapFormat.Gray8;
        case 2:
          return RGBLuminanceSource.BitmapFormat.RGB565;
        case 3:
          return RGBLuminanceSource.BitmapFormat.RGB24;
        case 4:
          return RGBLuminanceSource.BitmapFormat.RGB32;
        default:
          throw new ArgumentException("The bitmap format could not be determined. Please specify the correct value.");
      }
    }

    protected void CalculateLuminance(
      byte[] rgbRawBytes,
      RGBLuminanceSource.BitmapFormat bitmapFormat)
    {
      if (bitmapFormat == RGBLuminanceSource.BitmapFormat.Unknown)
        bitmapFormat = RGBLuminanceSource.DetermineBitmapFormat(rgbRawBytes, this.Width, this.Height);
      switch (bitmapFormat - 1)
      {
        case RGBLuminanceSource.BitmapFormat.Unknown:
          Buffer.BlockCopy((Array) rgbRawBytes, 0, (Array) this.luminances, 0, rgbRawBytes.Length < this.luminances.Length ? rgbRawBytes.Length : this.luminances.Length);
          break;
        case RGBLuminanceSource.BitmapFormat.Gray8:
          this.CalculateLuminanceRGB24(rgbRawBytes);
          break;
        case RGBLuminanceSource.BitmapFormat.RGB24:
          this.CalculateLuminanceRGB32(rgbRawBytes);
          break;
        case RGBLuminanceSource.BitmapFormat.RGB32:
          this.CalculateLuminanceARGB32(rgbRawBytes);
          break;
        case RGBLuminanceSource.BitmapFormat.ARGB32:
          this.CalculateLuminanceBGR24(rgbRawBytes);
          break;
        case RGBLuminanceSource.BitmapFormat.BGR24:
          this.CalculateLuminanceBGR32(rgbRawBytes);
          break;
        case RGBLuminanceSource.BitmapFormat.BGR32:
          this.CalculateLuminanceBGRA32(rgbRawBytes);
          break;
        case RGBLuminanceSource.BitmapFormat.BGRA32:
          this.CalculateLuminanceRGB565(rgbRawBytes);
          break;
        case RGBLuminanceSource.BitmapFormat.RGB565:
          this.CalculateLuminanceRGBA32(rgbRawBytes);
          break;
        default:
          throw new ArgumentException("The bitmap format isn't supported.", bitmapFormat.ToString());
      }
    }

    private void CalculateLuminanceRGB565(byte[] rgb565RawData)
    {
      int index1 = 0;
      for (int index2 = 0; index2 < rgb565RawData.Length && index1 < this.luminances.Length; ++index1)
      {
        byte num1 = rgb565RawData[index2];
        byte num2 = rgb565RawData[index2 + 1];
        int num3 = (int) num1 & 31;
        int num4 = (((int) num1 & 224) >> 5 | ((int) num2 & 3) << 3) & 31;
        int num5 = ((int) num2 >> 2 & 31) * 527 + 23 >> 6;
        int num6 = num4 * 527 + 23 >> 6;
        int num7 = num3 * 527 + 23 >> 6;
        this.luminances[index1] = (byte) (19562 * num5 + 38550 * num6 + 7424 * num7 >> 16);
        index2 += 2;
      }
    }

    private void CalculateLuminanceRGB24(byte[] rgbRawBytes)
    {
      int num1 = 0;
      for (int index1 = 0; num1 < rgbRawBytes.Length && index1 < this.luminances.Length; ++index1)
      {
        byte[] numArray1 = rgbRawBytes;
        int index2 = num1;
        int num2 = index2 + 1;
        int num3 = (int) numArray1[index2];
        byte[] numArray2 = rgbRawBytes;
        int index3 = num2;
        int num4 = index3 + 1;
        int num5 = (int) numArray2[index3];
        byte[] numArray3 = rgbRawBytes;
        int index4 = num4;
        num1 = index4 + 1;
        int num6 = (int) numArray3[index4];
        this.luminances[index1] = (byte) (19562 * num3 + 38550 * num5 + 7424 * num6 >> 16);
      }
    }

    private void CalculateLuminanceBGR24(byte[] rgbRawBytes)
    {
      int num1 = 0;
      for (int index1 = 0; num1 < rgbRawBytes.Length && index1 < this.luminances.Length; ++index1)
      {
        byte[] numArray1 = rgbRawBytes;
        int index2 = num1;
        int num2 = index2 + 1;
        int num3 = (int) numArray1[index2];
        byte[] numArray2 = rgbRawBytes;
        int index3 = num2;
        int num4 = index3 + 1;
        int num5 = (int) numArray2[index3];
        byte[] numArray3 = rgbRawBytes;
        int index4 = num4;
        num1 = index4 + 1;
        int num6 = (int) numArray3[index4];
        this.luminances[index1] = (byte) (19562 * num6 + 38550 * num5 + 7424 * num3 >> 16);
      }
    }

    private void CalculateLuminanceRGB32(byte[] rgbRawBytes)
    {
      int num1 = 0;
      for (int index1 = 0; num1 < rgbRawBytes.Length && index1 < this.luminances.Length; ++index1)
      {
        byte[] numArray1 = rgbRawBytes;
        int index2 = num1;
        int num2 = index2 + 1;
        int num3 = (int) numArray1[index2];
        byte[] numArray2 = rgbRawBytes;
        int index3 = num2;
        int num4 = index3 + 1;
        int num5 = (int) numArray2[index3];
        byte[] numArray3 = rgbRawBytes;
        int index4 = num4;
        int num6 = index4 + 1;
        int num7 = (int) numArray3[index4];
        num1 = num6 + 1;
        this.luminances[index1] = (byte) (19562 * num3 + 38550 * num5 + 7424 * num7 >> 16);
      }
    }

    private void CalculateLuminanceBGR32(byte[] rgbRawBytes)
    {
      int num1 = 0;
      for (int index1 = 0; num1 < rgbRawBytes.Length && index1 < this.luminances.Length; ++index1)
      {
        byte[] numArray1 = rgbRawBytes;
        int index2 = num1;
        int num2 = index2 + 1;
        int num3 = (int) numArray1[index2];
        byte[] numArray2 = rgbRawBytes;
        int index3 = num2;
        int num4 = index3 + 1;
        int num5 = (int) numArray2[index3];
        byte[] numArray3 = rgbRawBytes;
        int index4 = num4;
        int num6 = index4 + 1;
        int num7 = (int) numArray3[index4];
        num1 = num6 + 1;
        this.luminances[index1] = (byte) (19562 * num7 + 38550 * num5 + 7424 * num3 >> 16);
      }
    }

    private void CalculateLuminanceBGRA32(byte[] rgbRawBytes)
    {
      int num1 = 0;
      for (int index1 = 0; num1 < rgbRawBytes.Length && index1 < this.luminances.Length; ++index1)
      {
        byte[] numArray1 = rgbRawBytes;
        int index2 = num1;
        int num2 = index2 + 1;
        byte num3 = numArray1[index2];
        byte[] numArray2 = rgbRawBytes;
        int index3 = num2;
        int num4 = index3 + 1;
        byte num5 = numArray2[index3];
        byte[] numArray3 = rgbRawBytes;
        int index4 = num4;
        int num6 = index4 + 1;
        byte num7 = numArray3[index4];
        byte[] numArray4 = rgbRawBytes;
        int index5 = num6;
        num1 = index5 + 1;
        byte num8 = numArray4[index5];
        byte num9 = (byte) (19562 * (int) num7 + 38550 * (int) num5 + 7424 * (int) num3 >> 16);
        this.luminances[index1] = (byte) (((int) num9 * (int) num8 >> 8) + ((int) byte.MaxValue * ((int) byte.MaxValue - (int) num8) >> 8));
      }
    }

    private void CalculateLuminanceRGBA32(byte[] rgbRawBytes)
    {
      int num1 = 0;
      for (int index1 = 0; num1 < rgbRawBytes.Length && index1 < this.luminances.Length; ++index1)
      {
        byte[] numArray1 = rgbRawBytes;
        int index2 = num1;
        int num2 = index2 + 1;
        byte num3 = numArray1[index2];
        byte[] numArray2 = rgbRawBytes;
        int index3 = num2;
        int num4 = index3 + 1;
        byte num5 = numArray2[index3];
        byte[] numArray3 = rgbRawBytes;
        int index4 = num4;
        int num6 = index4 + 1;
        byte num7 = numArray3[index4];
        byte[] numArray4 = rgbRawBytes;
        int index5 = num6;
        num1 = index5 + 1;
        byte num8 = numArray4[index5];
        byte num9 = (byte) (19562 * (int) num3 + 38550 * (int) num5 + 7424 * (int) num7 >> 16);
        this.luminances[index1] = (byte) (((int) num9 * (int) num8 >> 8) + ((int) byte.MaxValue * ((int) byte.MaxValue - (int) num8) >> 8));
      }
    }

    private void CalculateLuminanceARGB32(byte[] rgbRawBytes)
    {
      int num1 = 0;
      for (int index1 = 0; num1 < rgbRawBytes.Length && index1 < this.luminances.Length; ++index1)
      {
        byte[] numArray1 = rgbRawBytes;
        int index2 = num1;
        int num2 = index2 + 1;
        byte num3 = numArray1[index2];
        byte[] numArray2 = rgbRawBytes;
        int index3 = num2;
        int num4 = index3 + 1;
        byte num5 = numArray2[index3];
        byte[] numArray3 = rgbRawBytes;
        int index4 = num4;
        int num6 = index4 + 1;
        byte num7 = numArray3[index4];
        byte[] numArray4 = rgbRawBytes;
        int index5 = num6;
        num1 = index5 + 1;
        byte num8 = numArray4[index5];
        byte num9 = (byte) (19562 * (int) num5 + 38550 * (int) num7 + 7424 * (int) num8 >> 16);
        this.luminances[index1] = (byte) (((int) num9 * (int) num3 >> 8) + ((int) byte.MaxValue * ((int) byte.MaxValue - (int) num3) >> 8));
      }
    }

    /// <summary>
    /// enumeration of supported bitmap format which the RGBLuminanceSource can process
    /// </summary>
    public enum BitmapFormat
    {
      /// <summary>
      /// format of the byte[] isn't known. RGBLuminanceSource tries to determine the best possible value
      /// </summary>
      Unknown,
      /// <summary>
      /// grayscale array, the byte array is a luminance array with 1 byte per pixel
      /// </summary>
      Gray8,
      /// <summary>
      /// 3 bytes per pixel with the channels red, green and blue
      /// </summary>
      RGB24,
      /// <summary>
      /// 4 bytes per pixel with the channels red, green and blue
      /// </summary>
      RGB32,
      /// <summary>
      /// 4 bytes per pixel with the channels alpha, red, green and blue
      /// </summary>
      ARGB32,
      /// <summary>
      /// 3 bytes per pixel with the channels blue, green and red
      /// </summary>
      BGR24,
      /// <summary>
      /// 4 bytes per pixel with the channels blue, green and red
      /// </summary>
      BGR32,
      /// <summary>
      /// 4 bytes per pixel with the channels blue, green, red and alpha
      /// </summary>
      BGRA32,
      /// <summary>
      /// 2 bytes per pixel, 5 bit red, 6 bits green and 5 bits blue
      /// </summary>
      RGB565,
      /// <summary>
      /// 4 bytes per pixel with the channels red, green, blue and alpha
      /// </summary>
      RGBA32,
    }
  }
}
