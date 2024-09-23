// Decompiled with JetBrains decompiler
// Type: ZXing.Common.BitSource
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;

#nullable disable
namespace ZXing.Common
{
  /// <summary> <p>This provides an easy abstraction to read bits at a time from a sequence of bytes, where the
  /// number of bits read is not often a multiple of 8.</p>
  /// 
  /// <p>This class is thread-safe but not reentrant. Unless the caller modifies the bytes array
  /// it passed in, in which case all bets are off.</p>
  /// 
  /// </summary>
  /// <author>Sean Owen</author>
  /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source
  /// </author>
  public sealed class BitSource
  {
    private readonly byte[] bytes;
    private int byteOffset;
    private int bitOffset;

    /// <param name="bytes">bytes from which this will read bits. Bits will be read from the first byte first.
    /// Bits are read within a byte from most-significant to least-significant bit.
    /// </param>
    public BitSource(byte[] bytes) => this.bytes = bytes;

    /// <summary>
    /// index of next bit in current byte which would be read by the next call to {@link #readBits(int)}.
    /// </summary>
    public int BitOffset => this.bitOffset;

    /// <summary>
    /// index of next byte in input byte array which would be read by the next call to {@link #readBits(int)}.
    /// </summary>
    public int ByteOffset => this.byteOffset;

    /// <param name="numBits">number of bits to read</param>
    /// <returns> int representing the bits read. The bits will appear as the least-significant
    /// bits of the int
    /// </returns>
    /// <exception cref="T:System.ArgumentException">if numBits isn't in [1,32] or more than is available</exception>
    public int readBits(int numBits)
    {
      if (numBits < 1 || numBits > 32 || numBits > this.available())
        throw new ArgumentException(numBits.ToString(), nameof (numBits));
      int num1 = 0;
      if (this.bitOffset > 0)
      {
        int num2 = 8 - this.bitOffset;
        int num3 = numBits < num2 ? numBits : num2;
        int num4 = num2 - num3;
        num1 = ((int) this.bytes[this.byteOffset] & (int) byte.MaxValue >> 8 - num3 << num4) >> num4;
        numBits -= num3;
        this.bitOffset += num3;
        if (this.bitOffset == 8)
        {
          this.bitOffset = 0;
          ++this.byteOffset;
        }
      }
      if (numBits > 0)
      {
        for (; numBits >= 8; numBits -= 8)
        {
          num1 = num1 << 8 | (int) this.bytes[this.byteOffset] & (int) byte.MaxValue;
          ++this.byteOffset;
        }
        if (numBits > 0)
        {
          int num5 = 8 - numBits;
          int num6 = (int) byte.MaxValue >> num5 << num5;
          num1 = num1 << numBits | ((int) this.bytes[this.byteOffset] & num6) >> num5;
          this.bitOffset += numBits;
        }
      }
      return num1;
    }

    /// <returns>number of bits that can be read successfully</returns>
    public int available() => 8 * (this.bytes.Length - this.byteOffset) - this.bitOffset;
  }
}
