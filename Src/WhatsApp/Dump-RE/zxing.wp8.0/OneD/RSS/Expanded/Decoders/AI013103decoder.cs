// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.RSS.Expanded.Decoders.AI013103decoder
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
  internal sealed class AI013103decoder : AI013x0xDecoder
  {
    internal AI013103decoder(BitArray information)
      : base(information)
    {
    }

    protected override void addWeightCode(StringBuilder buf, int weight) => buf.Append("(3103)");

    protected override int checkWeight(int weight) => weight;
  }
}
