// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.UPCEANExtension2Support
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Collections.Generic;
using System.Text;
using ZXing.Common;

#nullable disable
namespace ZXing.OneD
{
  /// <summary>@see UPCEANExtension5Support</summary>
  internal sealed class UPCEANExtension2Support
  {
    private readonly int[] decodeMiddleCounters = new int[4];
    private readonly StringBuilder decodeRowStringBuffer = new StringBuilder();

    internal Result decodeRow(int rowNumber, BitArray row, int[] extensionStartRange)
    {
      StringBuilder decodeRowStringBuffer = this.decodeRowStringBuffer;
      decodeRowStringBuffer.Length = 0;
      int x = this.decodeMiddle(row, extensionStartRange, decodeRowStringBuffer);
      if (x < 0)
        return (Result) null;
      string str = decodeRowStringBuffer.ToString();
      IDictionary<ResultMetadataType, object> extensionString = UPCEANExtension2Support.parseExtensionString(str);
      Result result = new Result(str, (byte[]) null, new ResultPoint[2]
      {
        new ResultPoint((float) (extensionStartRange[0] + extensionStartRange[1]) / 2f, (float) rowNumber),
        new ResultPoint((float) x, (float) rowNumber)
      }, BarcodeFormat.UPC_EAN_EXTENSION);
      if (extensionString != null)
        result.putAllMetadata(extensionString);
      return result;
    }

    private int decodeMiddle(BitArray row, int[] startRange, StringBuilder resultString)
    {
      int[] decodeMiddleCounters = this.decodeMiddleCounters;
      decodeMiddleCounters[0] = 0;
      decodeMiddleCounters[1] = 0;
      decodeMiddleCounters[2] = 0;
      decodeMiddleCounters[3] = 0;
      int size = row.Size;
      int nextUnset = startRange[1];
      int num1 = 0;
      for (int index = 0; index < 2 && nextUnset < size; ++index)
      {
        int digit;
        if (!UPCEANReader.decodeDigit(row, decodeMiddleCounters, nextUnset, UPCEANReader.L_AND_G_PATTERNS, out digit))
          return -1;
        resultString.Append((char) (48 + digit % 10));
        foreach (int num2 in decodeMiddleCounters)
          nextUnset += num2;
        if (digit >= 10)
          num1 |= 1 << 1 - index;
        if (index != 1)
        {
          int nextSet = row.getNextSet(nextUnset);
          nextUnset = row.getNextUnset(nextSet);
        }
      }
      return resultString.Length != 2 || int.Parse(resultString.ToString()) % 4 != num1 ? -1 : nextUnset;
    }

    private static IDictionary<ResultMetadataType, object> parseExtensionString(string raw)
    {
      if (raw.Length != 2)
        return (IDictionary<ResultMetadataType, object>) null;
      IDictionary<ResultMetadataType, object> extensionString = (IDictionary<ResultMetadataType, object>) new Dictionary<ResultMetadataType, object>();
      extensionString[ResultMetadataType.ISSUE_NUMBER] = (object) Convert.ToInt32(raw);
      return extensionString;
    }
  }
}
