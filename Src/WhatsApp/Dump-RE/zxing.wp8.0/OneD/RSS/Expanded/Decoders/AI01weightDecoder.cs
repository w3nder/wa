// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.RSS.Expanded.Decoders.AI01weightDecoder
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
  internal abstract class AI01weightDecoder : AI01decoder
  {
    internal AI01weightDecoder(BitArray information)
      : base(information)
    {
    }

    protected void encodeCompressedWeight(StringBuilder buf, int currentPos, int weightSize)
    {
      int valueFromBitArray = this.getGeneralDecoder().extractNumericValueFromBitArray(currentPos, weightSize);
      this.addWeightCode(buf, valueFromBitArray);
      int num1 = this.checkWeight(valueFromBitArray);
      int num2 = 100000;
      for (int index = 0; index < 5; ++index)
      {
        if (num1 / num2 == 0)
          buf.Append('0');
        num2 /= 10;
      }
      buf.Append(num1);
    }

    protected abstract void addWeightCode(StringBuilder buf, int weight);

    protected abstract int checkWeight(int weight);
  }
}
