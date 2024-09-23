// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.UPCEANExtension5Support
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System.Collections.Generic;
using System.Text;
using ZXing.Common;

#nullable disable
namespace ZXing.OneD
{
  /// @see UPCEANExtension2Support
  internal sealed class UPCEANExtension5Support
  {
    private static readonly int[] CHECK_DIGIT_ENCODINGS = new int[10]
    {
      24,
      20,
      18,
      17,
      12,
      6,
      3,
      10,
      9,
      5
    };
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
      IDictionary<ResultMetadataType, object> extensionString = UPCEANExtension5Support.parseExtensionString(str);
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
      int lgPatternFound = 0;
      for (int index = 0; index < 5 && nextUnset < size; ++index)
      {
        int digit;
        if (!UPCEANReader.decodeDigit(row, decodeMiddleCounters, nextUnset, UPCEANReader.L_AND_G_PATTERNS, out digit))
          return -1;
        resultString.Append((char) (48 + digit % 10));
        foreach (int num in decodeMiddleCounters)
          nextUnset += num;
        if (digit >= 10)
          lgPatternFound |= 1 << 4 - index;
        if (index != 4)
        {
          int nextSet = row.getNextSet(nextUnset);
          nextUnset = row.getNextUnset(nextSet);
        }
      }
      int checkDigit;
      return resultString.Length != 5 || !UPCEANExtension5Support.determineCheckDigit(lgPatternFound, out checkDigit) || UPCEANExtension5Support.extensionChecksum(resultString.ToString()) != checkDigit ? -1 : nextUnset;
    }

    private static int extensionChecksum(string s)
    {
      int length = s.Length;
      int num1 = 0;
      for (int index = length - 2; index >= 0; index -= 2)
        num1 += (int) s[index] - 48;
      int num2 = num1 * 3;
      for (int index = length - 1; index >= 0; index -= 2)
        num2 += (int) s[index] - 48;
      return num2 * 3 % 10;
    }

    private static bool determineCheckDigit(int lgPatternFound, out int checkDigit)
    {
      checkDigit = 0;
      while (checkDigit < 10)
      {
        if (lgPatternFound == UPCEANExtension5Support.CHECK_DIGIT_ENCODINGS[checkDigit])
          return true;
        ++checkDigit;
      }
      return false;
    }

    /// <summary>Parses the extension string.</summary>
    /// <param name="raw">raw content of extension</param>
    /// <returns>formatted interpretation of raw content as a {@link Map} mapping
    /// one {@link ResultMetadataType} to appropriate value, or {@code null} if not known</returns>
    private static IDictionary<ResultMetadataType, object> parseExtensionString(string raw)
    {
      if (raw.Length != 5)
        return (IDictionary<ResultMetadataType, object>) null;
      object extension5String = (object) UPCEANExtension5Support.parseExtension5String(raw);
      if (extension5String == null)
        return (IDictionary<ResultMetadataType, object>) null;
      IDictionary<ResultMetadataType, object> extensionString = (IDictionary<ResultMetadataType, object>) new Dictionary<ResultMetadataType, object>();
      extensionString[ResultMetadataType.SUGGESTED_PRICE] = extension5String;
      return extensionString;
    }

    private static string parseExtension5String(string raw)
    {
      string str1;
      switch (raw[0])
      {
        case '0':
          str1 = "£";
          break;
        case '5':
          str1 = "$";
          break;
        case '9':
          if ("90000".Equals(raw))
            return (string) null;
          if ("99991".Equals(raw))
            return "0.00";
          if ("99990".Equals(raw))
            return "Used";
          str1 = "";
          break;
        default:
          str1 = "";
          break;
      }
      int num1 = int.Parse(raw.Substring(1));
      string str2 = (num1 / 100).ToString();
      int num2 = num1 % 100;
      string str3 = num2 < 10 ? "0" + (object) num2 : num2.ToString();
      return str1 + str2 + (object) '.' + str3;
    }
  }
}
