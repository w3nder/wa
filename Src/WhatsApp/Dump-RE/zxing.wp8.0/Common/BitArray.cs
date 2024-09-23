// Decompiled with JetBrains decompiler
// Type: ZXing.Common.BitArray
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Text;

#nullable disable
namespace ZXing.Common
{
  /// <summary>
  /// A simple, fast array of bits, represented compactly by an array of ints internally.
  /// </summary>
  /// <author>Sean Owen</author>
  public sealed class BitArray
  {
    private int[] bits;
    private int size;
    private static readonly int[] _lookup = new int[37]
    {
      32,
      0,
      1,
      26,
      2,
      23,
      27,
      0,
      3,
      16,
      24,
      30,
      28,
      11,
      0,
      13,
      4,
      7,
      17,
      0,
      25,
      22,
      31,
      15,
      29,
      10,
      12,
      6,
      0,
      21,
      14,
      9,
      5,
      20,
      8,
      19,
      18
    };

    public int Size => this.size;

    public int SizeInBytes => this.size + 7 >> 3;

    public bool this[int i]
    {
      get => (this.bits[i >> 5] & 1 << i) != 0;
      set
      {
        if (!value)
          return;
        this.bits[i >> 5] |= 1 << i;
      }
    }

    public BitArray()
    {
      this.size = 0;
      this.bits = new int[1];
    }

    public BitArray(int size)
    {
      this.size = size >= 1 ? size : throw new ArgumentException("size must be at least 1");
      this.bits = BitArray.makeArray(size);
    }

    private BitArray(int[] bits, int size)
    {
      this.bits = bits;
      this.size = size;
    }

    private void ensureCapacity(int size)
    {
      if (size <= this.bits.Length << 5)
        return;
      int[] destinationArray = BitArray.makeArray(size);
      System.Array.Copy((System.Array) this.bits, 0, (System.Array) destinationArray, 0, this.bits.Length);
      this.bits = destinationArray;
    }

    /// <summary>Flips bit i.</summary>
    /// <param name="i">bit to set</param>
    public void flip(int i) => this.bits[i >> 5] ^= 1 << i;

    private static int numberOfTrailingZeros(int num)
    {
      int index = (-num & num) % 37;
      if (index < 0)
        index *= -1;
      return BitArray._lookup[index];
    }

    /// <summary>Gets the next set.</summary>
    /// <param name="from">first bit to check</param>
    /// <returns>index of first bit that is set, starting from the given index, or size if none are set
    /// at or beyond this given index</returns>
    public int getNextSet(int from)
    {
      if (from >= this.size)
        return this.size;
      int index = from >> 5;
      int num1;
      for (num1 = this.bits[index] & ~((1 << from) - 1); num1 == 0; num1 = this.bits[index])
      {
        if (++index == this.bits.Length)
          return this.size;
      }
      int num2 = (index << 5) + BitArray.numberOfTrailingZeros(num1);
      return num2 <= this.size ? num2 : this.size;
    }

    /// <summary>see getNextSet(int)</summary>
    /// <param name="from"></param>
    /// <returns></returns>
    public int getNextUnset(int from)
    {
      if (from >= this.size)
        return this.size;
      int index = from >> 5;
      int num1;
      for (num1 = ~this.bits[index] & ~((1 << from) - 1); num1 == 0; num1 = ~this.bits[index])
      {
        if (++index == this.bits.Length)
          return this.size;
      }
      int num2 = (index << 5) + BitArray.numberOfTrailingZeros(num1);
      return num2 <= this.size ? num2 : this.size;
    }

    /// <summary>Sets a block of 32 bits, starting at bit i.</summary>
    /// <param name="i">first bit to set</param>
    /// <param name="newBits">the new value of the next 32 bits. Note again that the least-significant bit
    /// corresponds to bit i, the next-least-significant to i+1, and so on.
    /// </param>
    public void setBulk(int i, int newBits) => this.bits[i >> 5] = newBits;

    /// <summary>Sets a range of bits.</summary>
    /// <param name="start">start of range, inclusive.</param>
    /// <param name="end">end of range, exclusive</param>
    public void setRange(int start, int end)
    {
      if (end < start)
        throw new ArgumentException();
      if (end == start)
        return;
      --end;
      int num1 = start >> 5;
      int num2 = end >> 5;
      for (int index1 = num1; index1 <= num2; ++index1)
      {
        int num3 = index1 > num1 ? 0 : start & 31;
        int num4 = index1 < num2 ? 31 : end & 31;
        int num5;
        if (num3 == 0 && num4 == 31)
        {
          num5 = -1;
        }
        else
        {
          num5 = 0;
          for (int index2 = num3; index2 <= num4; ++index2)
            num5 |= 1 << index2;
        }
        this.bits[index1] |= num5;
      }
    }

    /// <summary> Clears all bits (sets to false).</summary>
    public void clear()
    {
      int length = this.bits.Length;
      for (int index = 0; index < length; ++index)
        this.bits[index] = 0;
    }

    /// <summary> Efficient method to check if a range of bits is set, or not set.
    /// 
    /// </summary>
    /// <param name="start">start of range, inclusive.</param>
    /// <param name="end">end of range, exclusive</param>
    /// <param name="value">if true, checks that bits in range are set, otherwise checks that they are not set
    /// </param>
    /// <returns> true iff all bits are set or not set in range, according to value argument
    /// </returns>
    /// <throws>  IllegalArgumentException if end is less than or equal to start </throws>
    public bool isRange(int start, int end, bool value)
    {
      if (end < start)
        throw new ArgumentException();
      if (end == start)
        return true;
      --end;
      int num1 = start >> 5;
      int num2 = end >> 5;
      for (int index1 = num1; index1 <= num2; ++index1)
      {
        int num3 = index1 > num1 ? 0 : start & 31;
        int num4 = index1 < num2 ? 31 : end & 31;
        int num5;
        if (num3 == 0 && num4 == 31)
        {
          num5 = -1;
        }
        else
        {
          num5 = 0;
          for (int index2 = num3; index2 <= num4; ++index2)
            num5 |= 1 << index2;
        }
        if ((this.bits[index1] & num5) != (value ? num5 : 0))
          return false;
      }
      return true;
    }

    /// <summary>Appends the bit.</summary>
    /// <param name="bit">The bit.</param>
    public void appendBit(bool bit)
    {
      this.ensureCapacity(this.size + 1);
      if (bit)
        this.bits[this.size >> 5] |= 1 << this.size;
      ++this.size;
    }

    /// <returns> underlying array of ints. The first element holds the first 32 bits, and the least
    /// significant bit is bit 0.
    /// </returns>
    public int[] Array => this.bits;

    /// <summary>
    /// Appends the least-significant bits, from value, in order from most-significant to
    /// least-significant. For example, appending 6 bits from 0x000001E will append the bits
    /// 0, 1, 1, 1, 1, 0 in that order.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="numBits">The num bits.</param>
    public void appendBits(int value, int numBits)
    {
      if (numBits < 0 || numBits > 32)
        throw new ArgumentException("Num bits must be between 0 and 32");
      this.ensureCapacity(this.size + numBits);
      for (int index = numBits; index > 0; --index)
        this.appendBit((value >> index - 1 & 1) == 1);
    }

    public void appendBitArray(BitArray other)
    {
      int size = other.size;
      this.ensureCapacity(this.size + size);
      for (int i = 0; i < size; ++i)
        this.appendBit(other[i]);
    }

    public void xor(BitArray other)
    {
      if (this.bits.Length != other.bits.Length)
        throw new ArgumentException("Sizes don't match");
      for (int index = 0; index < this.bits.Length; ++index)
        this.bits[index] ^= other.bits[index];
    }

    /// <summary>Toes the bytes.</summary>
    /// <param name="bitOffset">first bit to start writing</param>
    /// <param name="array">array to write into. Bytes are written most-significant byte first. This is the opposite
    /// of the internal representation, which is exposed by BitArray</param>
    /// <param name="offset">position in array to start writing</param>
    /// <param name="numBytes">how many bytes to write</param>
    public void toBytes(int bitOffset, byte[] array, int offset, int numBytes)
    {
      for (int index1 = 0; index1 < numBytes; ++index1)
      {
        int num = 0;
        for (int index2 = 0; index2 < 8; ++index2)
        {
          if (this[bitOffset])
            num |= 1 << 7 - index2;
          ++bitOffset;
        }
        array[offset + index1] = (byte) num;
      }
    }

    /// <summary> Reverses all bits in the array.</summary>
    public void reverse()
    {
      int[] numArray = new int[this.bits.Length];
      int num1 = this.size - 1 >> 5;
      int num2 = num1 + 1;
      for (int index = 0; index < num2; ++index)
      {
        long bit = (long) this.bits[index];
        long num3 = bit >> 1 & 1431655765L | (bit & 1431655765L) << 1;
        long num4 = num3 >> 2 & 858993459L | (num3 & 858993459L) << 2;
        long num5 = num4 >> 4 & 252645135L | (num4 & 252645135L) << 4;
        long num6 = num5 >> 8 & 16711935L | (num5 & 16711935L) << 8;
        long num7 = num6 >> 16 & (long) ushort.MaxValue | (num6 & (long) ushort.MaxValue) << 16;
        numArray[num1 - index] = (int) num7;
      }
      if (this.size != num2 * 32)
      {
        int num8 = num2 * 32 - this.size;
        int num9 = 1;
        for (int index = 0; index < 31 - num8; ++index)
          num9 = num9 << 1 | 1;
        int num10 = numArray[0] >> num8 & num9;
        for (int index = 1; index < num2; ++index)
        {
          int num11 = numArray[index];
          int num12 = num10 | num11 << 32 - num8;
          numArray[index - 1] = num12;
          num10 = num11 >> num8 & num9;
        }
        numArray[num2 - 1] = num10;
      }
      this.bits = numArray;
    }

    private static int[] makeArray(int size) => new int[size + 31 >> 5];

    /// <summary>
    /// Determines whether the specified <see cref="T:System.Object" /> is equal to this instance.
    /// </summary>
    /// <param name="o">The <see cref="T:System.Object" /> to compare with this instance.</param>
    /// <returns>
    ///   <c>true</c> if the specified <see cref="T:System.Object" /> is equal to this instance; otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object o)
    {
      if (!(o is BitArray bitArray) || this.size != bitArray.size)
        return false;
      for (int index = 0; index < this.size; ++index)
      {
        if (this.bits[index] != bitArray.bits[index])
          return false;
      }
      return true;
    }

    /// <summary>Returns a hash code for this instance.</summary>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
    /// </returns>
    public override int GetHashCode()
    {
      int hashCode = this.size;
      foreach (int bit in this.bits)
        hashCode = 31 * hashCode + bit.GetHashCode();
      return hashCode;
    }

    /// <summary>
    /// Returns a <see cref="T:System.String" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder(this.size);
      for (int i = 0; i < this.size; ++i)
      {
        if ((i & 7) == 0)
          stringBuilder.Append(' ');
        stringBuilder.Append(this[i] ? 'X' : '.');
      }
      return stringBuilder.ToString();
    }

    /// <summary>
    /// Erstellt ein neues Objekt, das eine Kopie der aktuellen Instanz darstellt.
    /// </summary>
    /// <returns>
    /// Ein neues Objekt, das eine Kopie dieser Instanz darstellt.
    /// </returns>
    public object Clone() => (object) new BitArray((int[]) this.bits.Clone(), this.size);
  }
}
