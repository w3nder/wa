// Decompiled with JetBrains decompiler
// Type: ZXing.QrCode.Internal.DataMask
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using ZXing.Common;

#nullable disable
namespace ZXing.QrCode.Internal
{
  /// <summary> <p>Encapsulates data masks for the data bits in a QR code, per ISO 18004:2006 6.8. Implementations
  /// of this class can un-mask a raw BitMatrix. For simplicity, they will unmask the entire BitMatrix,
  /// including areas used for finder patterns, timing patterns, etc. These areas should be unused
  /// after the point they are unmasked anyway.</p>
  /// 
  /// <p>Note that the diagram in section 6.8.1 is misleading since it indicates that i is column position
  /// and j is row position. In fact, as the text says, i is row position and j is column position.</p>
  /// 
  /// </summary>
  /// <author>Sean Owen</author>
  /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source
  /// </author>
  internal abstract class DataMask
  {
    /// <summary> See ISO 18004:2006 6.8.1</summary>
    private static readonly DataMask[] DATA_MASKS = new DataMask[8]
    {
      (DataMask) new DataMask.DataMask000(),
      (DataMask) new DataMask.DataMask001(),
      (DataMask) new DataMask.DataMask010(),
      (DataMask) new DataMask.DataMask011(),
      (DataMask) new DataMask.DataMask100(),
      (DataMask) new DataMask.DataMask101(),
      (DataMask) new DataMask.DataMask110(),
      (DataMask) new DataMask.DataMask111()
    };

    private DataMask()
    {
    }

    /// <summary> <p>Implementations of this method reverse the data masking process applied to a QR Code and
    /// make its bits ready to read.</p>
    /// 
    /// </summary>
    /// <param name="bits">representation of QR Code bits</param>
    /// <param name="dimension">dimension of QR Code, represented by bits, being unmasked
    /// </param>
    internal void unmaskBitMatrix(BitMatrix bits, int dimension)
    {
      for (int index1 = 0; index1 < dimension; ++index1)
      {
        for (int index2 = 0; index2 < dimension; ++index2)
        {
          if (this.isMasked(index1, index2))
            bits.flip(index2, index1);
        }
      }
    }

    internal abstract bool isMasked(int i, int j);

    /// <param name="reference">a value between 0 and 7 indicating one of the eight possible
    /// data mask patterns a QR Code may use
    /// </param>
    /// <returns>{@link DataMask} encapsulating the data mask pattern</returns>
    internal static DataMask forReference(int reference)
    {
      return reference >= 0 && reference <= 7 ? DataMask.DATA_MASKS[reference] : throw new ArgumentException();
    }

    /// <summary> 000: mask bits for which (x + y) mod 2 == 0</summary>
    private sealed class DataMask000 : DataMask
    {
      internal override bool isMasked(int i, int j) => (i + j & 1) == 0;
    }

    /// <summary> 001: mask bits for which x mod 2 == 0</summary>
    private sealed class DataMask001 : DataMask
    {
      internal override bool isMasked(int i, int j) => (i & 1) == 0;
    }

    /// <summary> 010: mask bits for which y mod 3 == 0</summary>
    private sealed class DataMask010 : DataMask
    {
      internal override bool isMasked(int i, int j) => j % 3 == 0;
    }

    /// <summary> 011: mask bits for which (x + y) mod 3 == 0</summary>
    private sealed class DataMask011 : DataMask
    {
      internal override bool isMasked(int i, int j) => (i + j) % 3 == 0;
    }

    /// <summary> 100: mask bits for which (x/2 + y/3) mod 2 == 0</summary>
    private sealed class DataMask100 : DataMask
    {
      internal override bool isMasked(int i, int j) => ((i >>> 1) + j / 3 & 1) == 0;
    }

    /// <summary> 101: mask bits for which xy mod 2 + xy mod 3 == 0</summary>
    private sealed class DataMask101 : DataMask
    {
      internal override bool isMasked(int i, int j)
      {
        int num = i * j;
        return (num & 1) + num % 3 == 0;
      }
    }

    /// <summary> 110: mask bits for which (xy mod 2 + xy mod 3) mod 2 == 0</summary>
    private sealed class DataMask110 : DataMask
    {
      internal override bool isMasked(int i, int j)
      {
        int num = i * j;
        return ((num & 1) + num % 3 & 1) == 0;
      }
    }

    /// <summary> 111: mask bits for which ((x+y)mod 2 + xy mod 3) mod 2 == 0</summary>
    private sealed class DataMask111 : DataMask
    {
      internal override bool isMasked(int i, int j) => ((i + j & 1) + i * j % 3 & 1) == 0;
    }
  }
}
