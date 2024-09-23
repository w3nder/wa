// Decompiled with JetBrains decompiler
// Type: ZXing.QrCode.Internal.MaskUtil
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;

#nullable disable
namespace ZXing.QrCode.Internal
{
  /// <summary>
  /// 
  /// </summary>
  /// <author>Satoru Takabayashi</author>
  /// <author>Daniel Switkin</author>
  /// <author>Sean Owen</author>
  public static class MaskUtil
  {
    private const int N1 = 3;
    private const int N2 = 3;
    private const int N3 = 40;
    private const int N4 = 10;

    /// <summary>
    /// Apply mask penalty rule 1 and return the penalty. Find repetitive cells with the same color and
    /// give penalty to them. Example: 00000 or 11111.
    /// </summary>
    /// <param name="matrix">The matrix.</param>
    /// <returns></returns>
    public static int applyMaskPenaltyRule1(ByteMatrix matrix)
    {
      return MaskUtil.applyMaskPenaltyRule1Internal(matrix, true) + MaskUtil.applyMaskPenaltyRule1Internal(matrix, false);
    }

    /// <summary>
    /// Apply mask penalty rule 2 and return the penalty. Find 2x2 blocks with the same color and give
    /// penalty to them. This is actually equivalent to the spec's rule, which is to find MxN blocks and give a
    /// penalty proportional to (M-1)x(N-1), because this is the number of 2x2 blocks inside such a block.
    /// </summary>
    /// <param name="matrix">The matrix.</param>
    /// <returns></returns>
    public static int applyMaskPenaltyRule2(ByteMatrix matrix)
    {
      int num1 = 0;
      byte[][] array = matrix.Array;
      int width = matrix.Width;
      int height = matrix.Height;
      for (int index1 = 0; index1 < height - 1; ++index1)
      {
        for (int index2 = 0; index2 < width - 1; ++index2)
        {
          int num2 = (int) array[index1][index2];
          if (num2 == (int) array[index1][index2 + 1] && num2 == (int) array[index1 + 1][index2] && num2 == (int) array[index1 + 1][index2 + 1])
            ++num1;
        }
      }
      return 3 * num1;
    }

    /// <summary>
    /// Apply mask penalty rule 3 and return the penalty. Find consecutive cells of 00001011101 or
    /// 10111010000, and give penalty to them.  If we find patterns like 000010111010000, we give
    /// penalties twice (i.e. 40 * 2).
    /// </summary>
    /// <param name="matrix">The matrix.</param>
    /// <returns></returns>
    public static int applyMaskPenaltyRule3(ByteMatrix matrix)
    {
      int num = 0;
      byte[][] array = matrix.Array;
      int width = matrix.Width;
      int height = matrix.Height;
      for (int to = 0; to < height; ++to)
      {
        for (int index = 0; index < width; ++index)
        {
          byte[] rowArray = array[to];
          if (index + 6 < width && rowArray[index] == (byte) 1 && rowArray[index + 1] == (byte) 0 && rowArray[index + 2] == (byte) 1 && rowArray[index + 3] == (byte) 1 && rowArray[index + 4] == (byte) 1 && rowArray[index + 5] == (byte) 0 && rowArray[index + 6] == (byte) 1 && (MaskUtil.isWhiteHorizontal(rowArray, index - 4, index) || MaskUtil.isWhiteHorizontal(rowArray, index + 7, index + 11)))
            ++num;
          if (to + 6 < height && array[to][index] == (byte) 1 && array[to + 1][index] == (byte) 0 && array[to + 2][index] == (byte) 1 && array[to + 3][index] == (byte) 1 && array[to + 4][index] == (byte) 1 && array[to + 5][index] == (byte) 0 && array[to + 6][index] == (byte) 1 && (MaskUtil.isWhiteVertical(array, index, to - 4, to) || MaskUtil.isWhiteVertical(array, index, to + 7, to + 11)))
            ++num;
        }
      }
      return num * 40;
    }

    private static bool isWhiteHorizontal(byte[] rowArray, int from, int to)
    {
      for (int index = from; index < to; ++index)
      {
        if (index >= 0 && index < rowArray.Length && rowArray[index] == (byte) 1)
          return false;
      }
      return true;
    }

    private static bool isWhiteVertical(byte[][] array, int col, int from, int to)
    {
      for (int index = from; index < to; ++index)
      {
        if (index >= 0 && index < array.Length && array[index][col] == (byte) 1)
          return false;
      }
      return true;
    }

    /// <summary>
    /// Apply mask penalty rule 4 and return the penalty. Calculate the ratio of dark cells and give
    /// penalty if the ratio is far from 50%. It gives 10 penalty for 5% distance.
    /// </summary>
    /// <param name="matrix">The matrix.</param>
    /// <returns></returns>
    public static int applyMaskPenaltyRule4(ByteMatrix matrix)
    {
      int num1 = 0;
      byte[][] array = matrix.Array;
      int width = matrix.Width;
      int height = matrix.Height;
      for (int index1 = 0; index1 < height; ++index1)
      {
        byte[] numArray = array[index1];
        for (int index2 = 0; index2 < width; ++index2)
        {
          if (numArray[index2] == (byte) 1)
            ++num1;
        }
      }
      int num2 = matrix.Height * matrix.Width;
      return (int) (Math.Abs((double) num1 / (double) num2 - 0.5) * 20.0) * 10;
    }

    /// <summary>
    /// Return the mask bit for "getMaskPattern" at "x" and "y". See 8.8 of JISX0510:2004 for mask
    /// pattern conditions.
    /// </summary>
    /// <param name="maskPattern">The mask pattern.</param>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    /// <returns></returns>
    public static bool getDataMaskBit(int maskPattern, int x, int y)
    {
      int num1;
      switch (maskPattern)
      {
        case 0:
          num1 = y + x & 1;
          break;
        case 1:
          num1 = y & 1;
          break;
        case 2:
          num1 = x % 3;
          break;
        case 3:
          num1 = (y + x) % 3;
          break;
        case 4:
          num1 = (y >>> 1) + x / 3 & 1;
          break;
        case 5:
          int num2 = y * x;
          num1 = (num2 & 1) + num2 % 3;
          break;
        case 6:
          int num3 = y * x;
          num1 = (num3 & 1) + num3 % 3 & 1;
          break;
        case 7:
          num1 = y * x % 3 + (y + x & 1) & 1;
          break;
        default:
          throw new ArgumentException("Invalid mask pattern: " + (object) maskPattern);
      }
      return num1 == 0;
    }

    /// <summary>
    /// Helper function for applyMaskPenaltyRule1. We need this for doing this calculation in both
    /// vertical and horizontal orders respectively.
    /// </summary>
    /// <param name="matrix">The matrix.</param>
    /// <param name="isHorizontal">if set to <c>true</c> [is horizontal].</param>
    /// <returns></returns>
    private static int applyMaskPenaltyRule1Internal(ByteMatrix matrix, bool isHorizontal)
    {
      int num1 = 0;
      int num2 = isHorizontal ? matrix.Height : matrix.Width;
      int num3 = isHorizontal ? matrix.Width : matrix.Height;
      byte[][] array = matrix.Array;
      for (int index1 = 0; index1 < num2; ++index1)
      {
        int num4 = 0;
        int num5 = -1;
        for (int index2 = 0; index2 < num3; ++index2)
        {
          int num6 = isHorizontal ? (int) array[index1][index2] : (int) array[index2][index1];
          if (num6 == num5)
          {
            ++num4;
          }
          else
          {
            if (num4 >= 5)
              num1 += 3 + (num4 - 5);
            num4 = 1;
            num5 = num6;
          }
        }
        if (num4 >= 5)
          num1 += 3 + (num4 - 5);
      }
      return num1;
    }
  }
}
