// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.RSS.Expanded.Decoders.AI01AndOtherAIs
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
  internal sealed class AI01AndOtherAIs : AI01decoder
  {
    private static int HEADER_SIZE = 4;

    internal AI01AndOtherAIs(BitArray information)
      : base(information)
    {
    }

    public override string parseInformation()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("(01)");
      int length = stringBuilder.Length;
      int valueFromBitArray = this.getGeneralDecoder().extractNumericValueFromBitArray(AI01AndOtherAIs.HEADER_SIZE, 4);
      stringBuilder.Append(valueFromBitArray);
      this.encodeCompressedGtinWithoutAI(stringBuilder, AI01AndOtherAIs.HEADER_SIZE + 4, length);
      return this.getGeneralDecoder().decodeAllCodes(stringBuilder, AI01AndOtherAIs.HEADER_SIZE + 44);
    }
  }
}
