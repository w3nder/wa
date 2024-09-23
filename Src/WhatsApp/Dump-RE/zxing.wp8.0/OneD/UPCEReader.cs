// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.UPCEReader
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System.Text;
using ZXing.Common;

#nullable disable
namespace ZXing.OneD
{
  /// <summary>
  ///   <p>Implements decoding of the UPC-E format.</p>
  ///   <p />
  ///   <p><a href="http://www.barcodeisland.com/upce.phtml">This</a>is a great reference for
  /// UPC-E information.</p>
  ///   <author>Sean Owen</author>
  /// </summary>
  public sealed class UPCEReader : UPCEANReader
  {
    /// <summary>
    /// The pattern that marks the middle, and end, of a UPC-E pattern.
    /// There is no "second half" to a UPC-E barcode.
    /// </summary>
    private static readonly int[] MIDDLE_END_PATTERN = new int[6]
    {
      1,
      1,
      1,
      1,
      1,
      1
    };
    /// <summary>
    /// See L_AND_G_PATTERNS these values similarly represent patterns of
    /// even-odd parity encodings of digits that imply both the number system (0 or 1)
    /// used, and the check digit.
    /// </summary>
    private static readonly int[][] NUMSYS_AND_CHECK_DIGIT_PATTERNS = new int[2][]
    {
      new int[10]{ 56, 52, 50, 49, 44, 38, 35, 42, 41, 37 },
      new int[10]{ 7, 11, 13, 14, 19, 25, 28, 21, 22, 26 }
    };
    private readonly int[] decodeMiddleCounters;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.OneD.UPCEReader" /> class.
    /// </summary>
    public UPCEReader() => this.decodeMiddleCounters = new int[4];

    /// <summary>Decodes the middle.</summary>
    /// <param name="row">The row.</param>
    /// <param name="startRange">The start range.</param>
    /// <param name="result">The result.</param>
    /// <returns></returns>
    protected internal override int decodeMiddle(
      BitArray row,
      int[] startRange,
      StringBuilder result)
    {
      int[] decodeMiddleCounters = this.decodeMiddleCounters;
      decodeMiddleCounters[0] = 0;
      decodeMiddleCounters[1] = 0;
      decodeMiddleCounters[2] = 0;
      decodeMiddleCounters[3] = 0;
      int size = row.Size;
      int rowOffset = startRange[1];
      int lgPatternFound = 0;
      for (int index = 0; index < 6 && rowOffset < size; ++index)
      {
        int digit;
        if (!UPCEANReader.decodeDigit(row, decodeMiddleCounters, rowOffset, UPCEANReader.L_AND_G_PATTERNS, out digit))
          return -1;
        result.Append((char) (48 + digit % 10));
        foreach (int num in decodeMiddleCounters)
          rowOffset += num;
        if (digit >= 10)
          lgPatternFound |= 1 << 5 - index;
      }
      return !UPCEReader.determineNumSysAndCheckDigit(result, lgPatternFound) ? -1 : rowOffset;
    }

    /// <summary>Decodes the end.</summary>
    /// <param name="row">The row.</param>
    /// <param name="endStart">The end start.</param>
    /// <returns></returns>
    protected override int[] decodeEnd(BitArray row, int endStart)
    {
      return UPCEANReader.findGuardPattern(row, endStart, true, UPCEReader.MIDDLE_END_PATTERN);
    }

    /// <summary>
    ///   <returns>see checkStandardUPCEANChecksum(String)</returns>
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    protected override bool checkChecksum(string s)
    {
      return base.checkChecksum(UPCEReader.convertUPCEtoUPCA(s));
    }

    /// <summary>Determines the num sys and check digit.</summary>
    /// <param name="resultString">The result string.</param>
    /// <param name="lgPatternFound">The lg pattern found.</param>
    /// <returns></returns>
    private static bool determineNumSysAndCheckDigit(StringBuilder resultString, int lgPatternFound)
    {
      for (int index1 = 0; index1 <= 1; ++index1)
      {
        for (int index2 = 0; index2 < 10; ++index2)
        {
          if (lgPatternFound == UPCEReader.NUMSYS_AND_CHECK_DIGIT_PATTERNS[index1][index2])
          {
            resultString.Insert(0, new char[1]
            {
              (char) (48 + index1)
            });
            resultString.Append((char) (48 + index2));
            return true;
          }
        }
      }
      return false;
    }

    /// <summary>
    /// Get the format of this decoder.
    /// <returns>The 1D format.</returns>
    /// </summary>
    internal override BarcodeFormat BarcodeFormat => BarcodeFormat.UPC_E;

    /// <summary>
    /// Expands a UPC-E value back into its full, equivalent UPC-A code value.
    /// 
    /// <param name="upce">UPC-E code as string of digits</param>
    /// <returns>equivalent UPC-A code as string of digits</returns>
    /// </summary>
    public static string convertUPCEtoUPCA(string upce)
    {
      string str = upce.Substring(1, 6);
      StringBuilder stringBuilder = new StringBuilder(12);
      stringBuilder.Append(upce[0]);
      char ch = str[5];
      switch (ch)
      {
        case '0':
        case '1':
        case '2':
          stringBuilder.Append(str, 0, 2);
          stringBuilder.Append(ch);
          stringBuilder.Append("0000");
          stringBuilder.Append(str, 2, 3);
          break;
        case '3':
          stringBuilder.Append(str, 0, 3);
          stringBuilder.Append("00000");
          stringBuilder.Append(str, 3, 2);
          break;
        case '4':
          stringBuilder.Append(str, 0, 4);
          stringBuilder.Append("00000");
          stringBuilder.Append(str[4]);
          break;
        default:
          stringBuilder.Append(str, 0, 5);
          stringBuilder.Append("0000");
          stringBuilder.Append(ch);
          break;
      }
      stringBuilder.Append(upce[7]);
      return stringBuilder.ToString();
    }
  }
}
