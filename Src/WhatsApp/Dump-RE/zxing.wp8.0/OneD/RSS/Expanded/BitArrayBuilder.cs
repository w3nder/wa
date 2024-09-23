// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.RSS.Expanded.BitArrayBuilder
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System.Collections.Generic;
using ZXing.Common;

#nullable disable
namespace ZXing.OneD.RSS.Expanded
{
  /// <summary>
  /// <author>Pablo Orduña, University of Deusto (pablo.orduna@deusto.es)</author>
  /// <author>Eduardo Castillejo, University of Deusto (eduardo.castillejo@deusto.es)</author>
  /// </summary>
  internal static class BitArrayBuilder
  {
    internal static BitArray buildBitArray(List<ExpandedPair> pairs)
    {
      int num1 = (pairs.Count << 1) - 1;
      if (pairs[pairs.Count - 1].RightChar == null)
        --num1;
      BitArray bitArray = new BitArray(12 * num1);
      int i = 0;
      int num2 = pairs[0].RightChar.Value;
      for (int index = 11; index >= 0; --index)
      {
        if ((num2 & 1 << index) != 0)
          bitArray[i] = true;
        ++i;
      }
      for (int index1 = 1; index1 < pairs.Count; ++index1)
      {
        ExpandedPair pair = pairs[index1];
        int num3 = pair.LeftChar.Value;
        for (int index2 = 11; index2 >= 0; --index2)
        {
          if ((num3 & 1 << index2) != 0)
            bitArray[i] = true;
          ++i;
        }
        if (pair.RightChar != null)
        {
          int num4 = pair.RightChar.Value;
          for (int index3 = 11; index3 >= 0; --index3)
          {
            if ((num4 & 1 << index3) != 0)
              bitArray[i] = true;
            ++i;
          }
        }
      }
      return bitArray;
    }
  }
}
