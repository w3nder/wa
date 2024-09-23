// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.RSS.Expanded.Decoders.AI01393xDecoder
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
  /// </summary>
  internal sealed class AI01393xDecoder : AI01decoder
  {
    private static int HEADER_SIZE = 8;
    private static int LAST_DIGIT_SIZE = 2;
    private static int FIRST_THREE_DIGITS_SIZE = 10;

    internal AI01393xDecoder(BitArray information)
      : base(information)
    {
    }

    public override string parseInformation()
    {
      if (this.getInformation().Size < AI01393xDecoder.HEADER_SIZE + AI01decoder.GTIN_SIZE)
        return (string) null;
      StringBuilder buf = new StringBuilder();
      this.encodeCompressedGtin(buf, AI01393xDecoder.HEADER_SIZE);
      int valueFromBitArray1 = this.getGeneralDecoder().extractNumericValueFromBitArray(AI01393xDecoder.HEADER_SIZE + AI01decoder.GTIN_SIZE, AI01393xDecoder.LAST_DIGIT_SIZE);
      buf.Append("(393");
      buf.Append(valueFromBitArray1);
      buf.Append(')');
      int valueFromBitArray2 = this.getGeneralDecoder().extractNumericValueFromBitArray(AI01393xDecoder.HEADER_SIZE + AI01decoder.GTIN_SIZE + AI01393xDecoder.LAST_DIGIT_SIZE, AI01393xDecoder.FIRST_THREE_DIGITS_SIZE);
      if (valueFromBitArray2 / 100 == 0)
        buf.Append('0');
      if (valueFromBitArray2 / 10 == 0)
        buf.Append('0');
      buf.Append(valueFromBitArray2);
      DecodedInformation decodedInformation = this.getGeneralDecoder().decodeGeneralPurposeField(AI01393xDecoder.HEADER_SIZE + AI01decoder.GTIN_SIZE + AI01393xDecoder.LAST_DIGIT_SIZE + AI01393xDecoder.FIRST_THREE_DIGITS_SIZE, (string) null);
      buf.Append(decodedInformation.getNewString());
      return buf.ToString();
    }
  }
}
