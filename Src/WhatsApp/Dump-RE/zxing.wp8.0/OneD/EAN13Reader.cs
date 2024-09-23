// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.EAN13Reader
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
  /// <p>Implements decoding of the EAN-13 format.</p>
  /// 
  /// <author>dswitkin@google.com (Daniel Switkin)</author>
  /// <author>Sean Owen</author>
  /// <author>alasdair@google.com (Alasdair Mackintosh)</author>
  /// </summary>
  public sealed class EAN13Reader : UPCEANReader
  {
    internal static int[] FIRST_DIGIT_ENCODINGS = new int[10]
    {
      0,
      11,
      13,
      14,
      19,
      25,
      28,
      21,
      22,
      26
    };
    private readonly int[] decodeMiddleCounters;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.OneD.EAN13Reader" /> class.
    /// </summary>
    public EAN13Reader() => this.decodeMiddleCounters = new int[4];

    /// <summary>
    /// Subclasses override this to decode the portion of a barcode between the start
    /// and end guard patterns.
    /// </summary>
    /// <param name="row">row of black/white values to search</param>
    /// <param name="startRange">start/end offset of start guard pattern</param>
    /// <param name="resultString"><see cref="T:System.Text.StringBuilder" />to append decoded chars to</param>
    /// <returns>
    /// horizontal offset of first pixel after the "middle" that was decoded or -1 if decoding could not complete successfully
    /// </returns>
    protected internal override int decodeMiddle(
      BitArray row,
      int[] startRange,
      StringBuilder resultString)
    {
      int[] decodeMiddleCounters = this.decodeMiddleCounters;
      decodeMiddleCounters[0] = 0;
      decodeMiddleCounters[1] = 0;
      decodeMiddleCounters[2] = 0;
      decodeMiddleCounters[3] = 0;
      int size = row.Size;
      int rowOffset1 = startRange[1];
      int lgPatternFound = 0;
      for (int index = 0; index < 6 && rowOffset1 < size; ++index)
      {
        int digit;
        if (!UPCEANReader.decodeDigit(row, decodeMiddleCounters, rowOffset1, UPCEANReader.L_AND_G_PATTERNS, out digit))
          return -1;
        resultString.Append((char) (48 + digit % 10));
        foreach (int num in decodeMiddleCounters)
          rowOffset1 += num;
        if (digit >= 10)
          lgPatternFound |= 1 << 5 - index;
      }
      if (!EAN13Reader.determineFirstDigit(resultString, lgPatternFound))
        return -1;
      int[] guardPattern = UPCEANReader.findGuardPattern(row, rowOffset1, true, UPCEANReader.MIDDLE_PATTERN);
      if (guardPattern == null)
        return -1;
      int rowOffset2 = guardPattern[1];
      for (int index = 0; index < 6 && rowOffset2 < size; ++index)
      {
        int digit;
        if (!UPCEANReader.decodeDigit(row, decodeMiddleCounters, rowOffset2, UPCEANReader.L_PATTERNS, out digit))
          return -1;
        resultString.Append((char) (48 + digit));
        foreach (int num in decodeMiddleCounters)
          rowOffset2 += num;
      }
      return rowOffset2;
    }

    /// <summary>
    /// Get the format of this decoder.
    /// <returns>The 1D format.</returns>
    /// </summary>
    internal override BarcodeFormat BarcodeFormat => BarcodeFormat.EAN_13;

    /// <summary>
    /// Based on pattern of odd-even ('L' and 'G') patterns used to encoded the explicitly-encoded
    /// digits in a barcode, determines the implicitly encoded first digit and adds it to the
    /// result string.
    /// </summary>
    /// <param name="resultString">string to insert decoded first digit into</param>
    /// <param name="lgPatternFound">int whose bits indicates the pattern of odd/even L/G patterns used to</param>
    /// 
    ///              encode digits
    ///             <return>-1 if first digit cannot be determined</return>
    private static bool determineFirstDigit(StringBuilder resultString, int lgPatternFound)
    {
      for (int index = 0; index < 10; ++index)
      {
        if (lgPatternFound == EAN13Reader.FIRST_DIGIT_ENCODINGS[index])
        {
          resultString.Insert(0, new char[1]
          {
            (char) (48 + index)
          });
          return true;
        }
      }
      return false;
    }
  }
}
