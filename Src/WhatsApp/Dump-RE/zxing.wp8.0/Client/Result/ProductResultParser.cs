// Decompiled with JetBrains decompiler
// Type: ZXing.Client.Result.ProductResultParser
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using ZXing.OneD;

#nullable disable
namespace ZXing.Client.Result
{
  /// <summary>Parses strings of digits that represent a UPC code.</summary>
  /// <author>dswitkin@google.com (Daniel Switkin)</author>
  internal sealed class ProductResultParser : ResultParser
  {
    public override ParsedResult parse(ZXing.Result result)
    {
      BarcodeFormat barcodeFormat = result.BarcodeFormat;
      switch (barcodeFormat)
      {
        case BarcodeFormat.EAN_8:
        case BarcodeFormat.EAN_13:
        case BarcodeFormat.UPC_A:
        case BarcodeFormat.UPC_E:
          string text = result.Text;
          if (text == null)
            return (ParsedResult) null;
          if (!ResultParser.isStringOfDigits(text, text.Length))
            return (ParsedResult) null;
          string normalizedProductID = barcodeFormat != BarcodeFormat.UPC_E || text.Length != 8 ? text : UPCEReader.convertUPCEtoUPCA(text);
          return (ParsedResult) new ProductParsedResult(text, normalizedProductID);
        default:
          return (ParsedResult) null;
      }
    }
  }
}
