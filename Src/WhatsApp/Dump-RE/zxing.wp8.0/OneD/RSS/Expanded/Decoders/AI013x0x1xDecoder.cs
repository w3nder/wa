// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.RSS.Expanded.Decoders.AI013x0x1xDecoder
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
  internal sealed class AI013x0x1xDecoder : AI01weightDecoder
  {
    private static int HEADER_SIZE = 8;
    private static int WEIGHT_SIZE = 20;
    private static int DATE_SIZE = 16;
    private string dateCode;
    private string firstAIdigits;

    internal AI013x0x1xDecoder(BitArray information, string firstAIdigits, string dateCode)
      : base(information)
    {
      this.dateCode = dateCode;
      this.firstAIdigits = firstAIdigits;
    }

    public override string parseInformation()
    {
      if (this.getInformation().Size != AI013x0x1xDecoder.HEADER_SIZE + AI01decoder.GTIN_SIZE + AI013x0x1xDecoder.WEIGHT_SIZE + AI013x0x1xDecoder.DATE_SIZE)
        return (string) null;
      StringBuilder buf = new StringBuilder();
      this.encodeCompressedGtin(buf, AI013x0x1xDecoder.HEADER_SIZE);
      this.encodeCompressedWeight(buf, AI013x0x1xDecoder.HEADER_SIZE + AI01decoder.GTIN_SIZE, AI013x0x1xDecoder.WEIGHT_SIZE);
      this.encodeCompressedDate(buf, AI013x0x1xDecoder.HEADER_SIZE + AI01decoder.GTIN_SIZE + AI013x0x1xDecoder.WEIGHT_SIZE);
      return buf.ToString();
    }

    private void encodeCompressedDate(StringBuilder buf, int currentPos)
    {
      int valueFromBitArray = this.getGeneralDecoder().extractNumericValueFromBitArray(currentPos, AI013x0x1xDecoder.DATE_SIZE);
      if (valueFromBitArray == 38400)
        return;
      buf.Append('(');
      buf.Append(this.dateCode);
      buf.Append(')');
      int num1 = valueFromBitArray % 32;
      int num2 = valueFromBitArray / 32;
      int num3 = num2 % 12 + 1;
      int num4 = num2 / 12;
      if (num4 / 10 == 0)
        buf.Append('0');
      buf.Append(num4);
      if (num3 / 10 == 0)
        buf.Append('0');
      buf.Append(num3);
      if (num1 / 10 == 0)
        buf.Append('0');
      buf.Append(num1);
    }

    protected override void addWeightCode(StringBuilder buf, int weight)
    {
      int num = weight / 100000;
      buf.Append('(');
      buf.Append(this.firstAIdigits);
      buf.Append(num);
      buf.Append(')');
    }

    protected override int checkWeight(int weight) => weight % 100000;
  }
}
