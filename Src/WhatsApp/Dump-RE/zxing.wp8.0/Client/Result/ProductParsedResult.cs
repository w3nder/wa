// Decompiled with JetBrains decompiler
// Type: ZXing.Client.Result.ProductParsedResult
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

#nullable disable
namespace ZXing.Client.Result
{
  /// <author>dswitkin@google.com (Daniel Switkin)</author>
  public sealed class ProductParsedResult : ParsedResult
  {
    internal ProductParsedResult(string productID)
      : this(productID, productID)
    {
    }

    internal ProductParsedResult(string productID, string normalizedProductID)
      : base(ParsedResultType.PRODUCT)
    {
      this.ProductID = productID;
      this.NormalizedProductID = normalizedProductID;
      this.displayResultValue = productID;
    }

    public string ProductID { get; private set; }

    public string NormalizedProductID { get; private set; }
  }
}
