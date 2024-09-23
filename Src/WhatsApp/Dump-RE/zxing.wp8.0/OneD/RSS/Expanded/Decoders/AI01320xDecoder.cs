// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.RSS.Expanded.Decoders.AI01320xDecoder
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
  internal sealed class AI01320xDecoder : AI013x0xDecoder
  {
    internal AI01320xDecoder(BitArray information)
      : base(information)
    {
    }

    protected override void addWeightCode(StringBuilder buf, int weight)
    {
      if (weight < 10000)
        buf.Append("(3202)");
      else
        buf.Append("(3203)");
    }

    protected override int checkWeight(int weight) => weight < 10000 ? weight : weight - 10000;
  }
}
