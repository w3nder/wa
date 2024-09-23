// Decompiled with JetBrains decompiler
// Type: ZXing.PDF417.Internal.PDF417CodewordDecoder
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

#nullable disable
namespace ZXing.PDF417.Internal
{
  /// <summary>
  /// 
  /// </summary>
  /// <author>Guenther Grau</author>
  /// <author>creatale GmbH (christoph.schulz@creatale.de)</author>
  public static class PDF417CodewordDecoder
  {
    /// <summary>The ratios table</summary>
    private static readonly float[][] RATIOS_TABLE = new float[PDF417Common.SYMBOL_TABLE.Length][];

    static PDF417CodewordDecoder()
    {
      for (int index = 0; index < PDF417CodewordDecoder.RATIOS_TABLE.Length; ++index)
        PDF417CodewordDecoder.RATIOS_TABLE[index] = new float[PDF417Common.BARS_IN_MODULE];
      for (int index1 = 0; index1 < PDF417Common.SYMBOL_TABLE.Length; ++index1)
      {
        int num1 = PDF417Common.SYMBOL_TABLE[index1];
        int num2 = num1 & 1;
        for (int index2 = 0; index2 < PDF417Common.BARS_IN_MODULE; ++index2)
        {
          float num3 = 0.0f;
          for (; (num1 & 1) == num2; num1 >>= 1)
            ++num3;
          num2 = num1 & 1;
          PDF417CodewordDecoder.RATIOS_TABLE[index1][PDF417Common.BARS_IN_MODULE - index2 - 1] = num3 / (float) PDF417Common.MODULES_IN_CODEWORD;
        }
      }
    }

    /// <summary>Gets the decoded value.</summary>
    /// <returns>The decoded value.</returns>
    /// <param name="moduleBitCount">Module bit count.</param>
    public static int getDecodedValue(int[] moduleBitCount)
    {
      int decodedCodewordValue = PDF417CodewordDecoder.getDecodedCodewordValue(PDF417CodewordDecoder.sampleBitCounts(moduleBitCount));
      return decodedCodewordValue != PDF417Common.INVALID_CODEWORD ? decodedCodewordValue : PDF417CodewordDecoder.getClosestDecodedValue(moduleBitCount);
    }

    /// <summary>Samples the bit counts.</summary>
    /// <returns>The bit counts.</returns>
    /// <param name="moduleBitCount">Module bit count.</param>
    private static int[] sampleBitCounts(int[] moduleBitCount)
    {
      float bitCountSum = (float) PDF417Common.getBitCountSum(moduleBitCount);
      int[] numArray = new int[PDF417Common.BARS_IN_MODULE];
      int index1 = 0;
      int num1 = 0;
      for (int index2 = 0; index2 < PDF417Common.MODULES_IN_CODEWORD; ++index2)
      {
        float num2 = (float) ((double) bitCountSum / (double) (2 * PDF417Common.MODULES_IN_CODEWORD) + (double) index2 * (double) bitCountSum / (double) PDF417Common.MODULES_IN_CODEWORD);
        if ((double) (num1 + moduleBitCount[index1]) <= (double) num2)
        {
          num1 += moduleBitCount[index1];
          ++index1;
        }
        ++numArray[index1];
      }
      return numArray;
    }

    /// <summary>Gets the decoded codeword value.</summary>
    /// <returns>The decoded codeword value.</returns>
    /// <param name="moduleBitCount">Module bit count.</param>
    private static int getDecodedCodewordValue(int[] moduleBitCount)
    {
      int bitValue = PDF417CodewordDecoder.getBitValue(moduleBitCount);
      return PDF417Common.getCodeword((long) bitValue) != PDF417Common.INVALID_CODEWORD ? bitValue : PDF417Common.INVALID_CODEWORD;
    }

    /// <summary>Gets the bit value.</summary>
    /// <returns>The bit value.</returns>
    /// <param name="moduleBitCount">Module bit count.</param>
    private static int getBitValue(int[] moduleBitCount)
    {
      ulong bitValue = 0;
      for (ulong index1 = 0; index1 < (ulong) moduleBitCount.Length; ++index1)
      {
        for (int index2 = 0; index2 < moduleBitCount[index1]; ++index2)
          bitValue = (ulong) ((long) bitValue << 1 | (index1 % 2UL == 0UL ? 1L : 0L));
      }
      return (int) bitValue;
    }

    /// <summary>Gets the closest decoded value.</summary>
    /// <returns>The closest decoded value.</returns>
    /// <param name="moduleBitCount">Module bit count.</param>
    private static int getClosestDecodedValue(int[] moduleBitCount)
    {
      int bitCountSum = PDF417Common.getBitCountSum(moduleBitCount);
      float[] numArray1 = new float[PDF417Common.BARS_IN_MODULE];
      for (int index = 0; index < numArray1.Length; ++index)
        numArray1[index] = (float) moduleBitCount[index] / (float) bitCountSum;
      float num1 = float.MaxValue;
      int invalidCodeword = PDF417Common.INVALID_CODEWORD;
      for (int index1 = 0; index1 < PDF417CodewordDecoder.RATIOS_TABLE.Length; ++index1)
      {
        float num2 = 0.0f;
        float[] numArray2 = PDF417CodewordDecoder.RATIOS_TABLE[index1];
        for (int index2 = 0; index2 < PDF417Common.BARS_IN_MODULE; ++index2)
        {
          float num3 = numArray2[index2] - numArray1[index2];
          num2 += num3 * num3;
          if ((double) num2 >= (double) num1)
            break;
        }
        if ((double) num2 < (double) num1)
        {
          num1 = num2;
          invalidCodeword = PDF417Common.SYMBOL_TABLE[index1];
        }
      }
      return invalidCodeword;
    }
  }
}
