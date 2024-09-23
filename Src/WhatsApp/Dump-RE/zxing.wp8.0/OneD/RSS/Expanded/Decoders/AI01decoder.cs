// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.RSS.Expanded.Decoders.AI01decoder
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System.Text;
using ZXing.Common;

#nullable disable
namespace ZXing.OneD.RSS.Expanded.Decoders
{
  /// <summary>
  /// <author>Pablo Orduña, University of Deusto (pablo.orduna@deusto.es)</author>
  /// <author>Eduardo Castillejo, University of Deusto (eduardo.castillejo@deusto.es)</author>
  /// </summary>
  internal abstract class AI01decoder : AbstractExpandedDecoder
  {
    protected static int GTIN_SIZE = 40;

    internal AI01decoder(BitArray information)
      : base(information)
    {
    }

    protected void encodeCompressedGtin(StringBuilder buf, int currentPos)
    {
      buf.Append("(01)");
      int length = buf.Length;
      buf.Append('9');
      this.encodeCompressedGtinWithoutAI(buf, currentPos, length);
    }

    protected void encodeCompressedGtinWithoutAI(
      StringBuilder buf,
      int currentPos,
      int initialBufferPosition)
    {
      for (int index = 0; index < 4; ++index)
      {
        int valueFromBitArray = this.getGeneralDecoder().extractNumericValueFromBitArray(currentPos + 10 * index, 10);
        if (valueFromBitArray / 100 == 0)
          buf.Append('0');
        if (valueFromBitArray / 10 == 0)
          buf.Append('0');
        buf.Append(valueFromBitArray);
      }
      AI01decoder.appendCheckDigit(buf, initialBufferPosition);
    }

    private static void appendCheckDigit(StringBuilder buf, int currentPos)
    {
      int num1 = 0;
      for (int index = 0; index < 13; ++index)
      {
        int num2 = (int) buf[index + currentPos] - 48;
        num1 += (index & 1) == 0 ? 3 * num2 : num2;
      }
      int num3 = 10 - num1 % 10;
      if (num3 == 10)
        num3 = 0;
      buf.Append(num3);
    }
  }
}
