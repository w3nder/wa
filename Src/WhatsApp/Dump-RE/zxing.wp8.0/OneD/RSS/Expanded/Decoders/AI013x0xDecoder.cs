// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.RSS.Expanded.Decoders.AI013x0xDecoder
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
  internal abstract class AI013x0xDecoder : AI01weightDecoder
  {
    private static int HEADER_SIZE = 5;
    private static int WEIGHT_SIZE = 15;

    internal AI013x0xDecoder(BitArray information)
      : base(information)
    {
    }

    public override string parseInformation()
    {
      if (this.getInformation().Size != AI013x0xDecoder.HEADER_SIZE + AI01decoder.GTIN_SIZE + AI013x0xDecoder.WEIGHT_SIZE)
        return (string) null;
      StringBuilder buf = new StringBuilder();
      this.encodeCompressedGtin(buf, AI013x0xDecoder.HEADER_SIZE);
      this.encodeCompressedWeight(buf, AI013x0xDecoder.HEADER_SIZE + AI01decoder.GTIN_SIZE, AI013x0xDecoder.WEIGHT_SIZE);
      return buf.ToString();
    }
  }
}
