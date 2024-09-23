// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.PlesseyWriter
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Collections.Generic;
using ZXing.Common;

#nullable disable
namespace ZXing.OneD
{
  /// <summary>
  /// This object renders a Plessey code as a <see cref="T:ZXing.Common.BitMatrix" />.
  /// </summary>
  public sealed class PlesseyWriter : OneDimensionalCodeWriter
  {
    private const string ALPHABET_STRING = "0123456789ABCDEF";
    private static readonly int[] startWidths = new int[8]
    {
      14,
      11,
      14,
      11,
      5,
      20,
      14,
      11
    };
    private static readonly int[] terminationWidths = new int[1]
    {
      25
    };
    private static readonly int[] endWidths = new int[8]
    {
      20,
      5,
      20,
      5,
      14,
      11,
      14,
      11
    };
    private static readonly int[][] numberWidths = new int[16][]
    {
      new int[8]{ 5, 20, 5, 20, 5, 20, 5, 20 },
      new int[8]{ 14, 11, 5, 20, 5, 20, 5, 20 },
      new int[8]{ 5, 20, 14, 11, 5, 20, 5, 20 },
      new int[8]{ 14, 11, 14, 11, 5, 20, 5, 20 },
      new int[8]{ 5, 20, 5, 20, 14, 11, 5, 20 },
      new int[8]{ 14, 11, 5, 20, 14, 11, 5, 20 },
      new int[8]{ 5, 20, 14, 11, 14, 11, 5, 20 },
      new int[8]{ 14, 11, 14, 11, 14, 11, 5, 20 },
      new int[8]{ 5, 20, 5, 20, 5, 20, 14, 11 },
      new int[8]{ 14, 11, 5, 20, 5, 20, 14, 11 },
      new int[8]{ 5, 20, 14, 11, 5, 20, 14, 11 },
      new int[8]{ 14, 11, 14, 11, 5, 20, 14, 11 },
      new int[8]{ 5, 20, 5, 20, 14, 11, 14, 11 },
      new int[8]{ 14, 11, 5, 20, 14, 11, 14, 11 },
      new int[8]{ 5, 20, 14, 11, 14, 11, 14, 11 },
      new int[8]{ 14, 11, 14, 11, 14, 11, 14, 11 }
    };
    private static readonly byte[] crcGrid = new byte[9]
    {
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 0,
      (byte) 1,
      (byte) 0,
      (byte) 0,
      (byte) 1
    };
    private static readonly int[] crc0Widths = new int[2]
    {
      5,
      20
    };
    private static readonly int[] crc1Widths = new int[2]
    {
      14,
      11
    };

    /// <summary>
    /// Encode the contents following specified format.
    /// {@code width} and {@code height} are required size. This method may return bigger size
    /// {@code BitMatrix} when specified size is too small. The user can set both {@code width} and
    /// {@code height} to zero to get minimum size barcode. If negative value is set to {@code width}
    /// or {@code height}, {@code IllegalArgumentException} is thrown.
    /// </summary>
    /// <param name="contents"></param>
    /// <param name="format"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="hints"></param>
    /// <returns></returns>
    public override BitMatrix encode(
      string contents,
      BarcodeFormat format,
      int width,
      int height,
      IDictionary<EncodeHintType, object> hints)
    {
      if (format != BarcodeFormat.PLESSEY)
        throw new ArgumentException("Can only encode Plessey, but got " + (object) format);
      return base.encode(contents, format, width, height, hints);
    }

    /// <summary>
    /// Encode the contents to byte array expression of one-dimensional barcode.
    /// Start code and end code should be included in result, and side margins should not be included.
    /// <returns>a {@code boolean[]} of horizontal pixels (false = white, true = black)</returns>
    /// </summary>
    /// <param name="contents"></param>
    /// <returns></returns>
    public override bool[] encode(string contents)
    {
      int length = contents.Length;
      for (int index = 0; index < length; ++index)
      {
        if ("0123456789ABCDEF".IndexOf(contents[index]) < 0)
          throw new ArgumentException("Requested contents contains a not encodable character: '" + (object) contents[index] + "'");
      }
      bool[] target = new bool[200 + length * 100 + 200 + 25 + 100 + 100];
      byte[] numArray1 = new byte[4 * length + 8];
      int num1 = 0;
      int pos1 = 100;
      int pos2 = pos1 + OneDimensionalCodeWriter.appendPattern(target, pos1, PlesseyWriter.startWidths, true);
      for (int index1 = 0; index1 < length; ++index1)
      {
        int index2 = "0123456789ABCDEF".IndexOf(contents[index1]);
        int[] numberWidth = PlesseyWriter.numberWidths[index2];
        pos2 += OneDimensionalCodeWriter.appendPattern(target, pos2, numberWidth, true);
        byte[] numArray2 = numArray1;
        int index3 = num1;
        int num2 = index3 + 1;
        int num3 = (int) (byte) (index2 & 1);
        numArray2[index3] = (byte) num3;
        byte[] numArray3 = numArray1;
        int index4 = num2;
        int num4 = index4 + 1;
        int num5 = (int) (byte) (index2 >> 1 & 1);
        numArray3[index4] = (byte) num5;
        byte[] numArray4 = numArray1;
        int index5 = num4;
        int num6 = index5 + 1;
        int num7 = (int) (byte) (index2 >> 2 & 1);
        numArray4[index5] = (byte) num7;
        byte[] numArray5 = numArray1;
        int index6 = num6;
        num1 = index6 + 1;
        int num8 = (int) (byte) (index2 >> 3 & 1);
        numArray5[index6] = (byte) num8;
      }
      for (int index7 = 0; index7 < 4 * length; ++index7)
      {
        if (numArray1[index7] != (byte) 0)
        {
          for (int index8 = 0; index8 < 9; ++index8)
            numArray1[index7 + index8] ^= PlesseyWriter.crcGrid[index8];
        }
      }
      for (int index = 0; index < 8; ++index)
      {
        switch (numArray1[length * 4 + index])
        {
          case 0:
            pos2 += OneDimensionalCodeWriter.appendPattern(target, pos2, PlesseyWriter.crc0Widths, true);
            break;
          case 1:
            pos2 += OneDimensionalCodeWriter.appendPattern(target, pos2, PlesseyWriter.crc1Widths, true);
            break;
        }
      }
      int pos3 = pos2 + OneDimensionalCodeWriter.appendPattern(target, pos2, PlesseyWriter.terminationWidths, true);
      OneDimensionalCodeWriter.appendPattern(target, pos3, PlesseyWriter.endWidths, false);
      return target;
    }
  }
}
