// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.EAN8Reader
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
  ///   <p>Implements decoding of the EAN-8 format.</p>
  ///   <author>Sean Owen</author>
  /// </summary>
  public sealed class EAN8Reader : UPCEANReader
  {
    private readonly int[] decodeMiddleCounters;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.OneD.EAN8Reader" /> class.
    /// </summary>
    public EAN8Reader() => this.decodeMiddleCounters = new int[4];

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
      int rowOffset1 = startRange[1];
      for (int index = 0; index < 4 && rowOffset1 < size; ++index)
      {
        int digit;
        if (!UPCEANReader.decodeDigit(row, decodeMiddleCounters, rowOffset1, UPCEANReader.L_PATTERNS, out digit))
          return -1;
        result.Append((char) (48 + digit));
        foreach (int num in decodeMiddleCounters)
          rowOffset1 += num;
      }
      int[] guardPattern = UPCEANReader.findGuardPattern(row, rowOffset1, true, UPCEANReader.MIDDLE_PATTERN);
      if (guardPattern == null)
        return -1;
      int rowOffset2 = guardPattern[1];
      for (int index = 0; index < 4 && rowOffset2 < size; ++index)
      {
        int digit;
        if (!UPCEANReader.decodeDigit(row, decodeMiddleCounters, rowOffset2, UPCEANReader.L_PATTERNS, out digit))
          return -1;
        result.Append((char) (48 + digit));
        foreach (int num in decodeMiddleCounters)
          rowOffset2 += num;
      }
      return rowOffset2;
    }

    /// <summary>
    /// Get the format of this decoder.
    /// <returns>The 1D format.</returns>
    /// </summary>
    internal override BarcodeFormat BarcodeFormat => BarcodeFormat.EAN_8;
  }
}
