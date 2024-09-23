// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.RSS.Expanded.Decoders.AI01392xDecoder
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
  internal sealed class AI01392xDecoder : AI01decoder
  {
    private const int HEADER_SIZE = 8;
    private const int LAST_DIGIT_SIZE = 2;

    internal AI01392xDecoder(BitArray information)
      : base(information)
    {
    }

    public override string parseInformation()
    {
      if (this.getInformation().Size < 8 + AI01decoder.GTIN_SIZE)
        return (string) null;
      StringBuilder buf = new StringBuilder();
      this.encodeCompressedGtin(buf, 8);
      int valueFromBitArray = this.getGeneralDecoder().extractNumericValueFromBitArray(8 + AI01decoder.GTIN_SIZE, 2);
      buf.Append("(392");
      buf.Append(valueFromBitArray);
      buf.Append(')');
      DecodedInformation decodedInformation = this.getGeneralDecoder().decodeGeneralPurposeField(8 + AI01decoder.GTIN_SIZE + 2, (string) null);
      buf.Append(decodedInformation.getNewString());
      return buf.ToString();
    }
  }
}
