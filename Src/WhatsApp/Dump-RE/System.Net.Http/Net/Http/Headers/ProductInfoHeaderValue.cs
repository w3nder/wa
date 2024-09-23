// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.ProductInfoHeaderValue
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Diagnostics.Contracts;
using System.Globalization;

#nullable disable
namespace System.Net.Http.Headers
{
  /// <summary>Represents a value which can either be a product or a comment.</summary>
  public class ProductInfoHeaderValue : ICloneable
  {
    private ProductHeaderValue product;
    private string comment;

    /// <returns>Returns <see cref="T:System.Net.Http.Headers.ProductHeaderValue" />.</returns>
    public ProductHeaderValue Product => this.product;

    /// <returns>Returns <see cref="T:System.String" />.</returns>
    public string Comment => this.comment;

    public ProductInfoHeaderValue(string productName, string productVersion)
      : this(new ProductHeaderValue(productName, productVersion))
    {
    }

    public ProductInfoHeaderValue(ProductHeaderValue product)
    {
      this.product = product != null ? product : throw new ArgumentNullException(nameof (product));
    }

    public ProductInfoHeaderValue(string comment)
    {
      HeaderUtilities.CheckValidComment(comment, nameof (comment));
      this.comment = comment;
    }

    private ProductInfoHeaderValue(ProductInfoHeaderValue source)
    {
      Contract.Requires(source != null);
      this.product = source.product;
      this.comment = source.comment;
    }

    private ProductInfoHeaderValue()
    {
    }

    /// <returns>Returns <see cref="T:System.String" />.</returns>
    public override string ToString()
    {
      return this.product == null ? this.comment : this.product.ToString();
    }

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public override bool Equals(object obj)
    {
      if (!(obj is ProductInfoHeaderValue productInfoHeaderValue))
        return false;
      return this.product == null ? string.CompareOrdinal(this.comment, productInfoHeaderValue.comment) == 0 : this.product.Equals((object) productInfoHeaderValue.product);
    }

    /// <returns>Returns <see cref="T:System.Int32" />.</returns>
    public override int GetHashCode()
    {
      return this.product == null ? this.comment.GetHashCode() : this.product.GetHashCode();
    }

    /// <returns>Returns <see cref="T:System.Net.Http.Headers.ProductInfoHeaderValue" />.</returns>
    public static ProductInfoHeaderValue Parse(string input)
    {
      int index = 0;
      object obj = ProductInfoHeaderParser.SingleValueParser.ParseValue(input, (object) null, ref index);
      if (index < input.Length)
        throw new FormatException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, SR.net_http_headers_invalid_value, (object) input.Substring(index)));
      return (ProductInfoHeaderValue) obj;
    }

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public static bool TryParse(string input, out ProductInfoHeaderValue parsedValue)
    {
      int index = 0;
      parsedValue = (ProductInfoHeaderValue) null;
      object parsedValue1;
      if (!ProductInfoHeaderParser.SingleValueParser.TryParseValue(input, (object) null, ref index, out parsedValue1) || index < input.Length)
        return false;
      parsedValue = (ProductInfoHeaderValue) parsedValue1;
      return true;
    }

    internal static int GetProductInfoLength(
      string input,
      int startIndex,
      out ProductInfoHeaderValue parsedValue)
    {
      Contract.Requires(startIndex >= 0);
      parsedValue = (ProductInfoHeaderValue) null;
      if (string.IsNullOrEmpty(input) || startIndex >= input.Length)
        return 0;
      int num1 = startIndex;
      string str = (string) null;
      ProductHeaderValue parsedValue1 = (ProductHeaderValue) null;
      int num2;
      if (input[num1] == '(')
      {
        int length = 0;
        if (HttpRuleParser.GetCommentLength(input, num1, out length) != HttpParseResult.Parsed)
          return 0;
        str = input.Substring(num1, length);
        int startIndex1 = num1 + length;
        num2 = startIndex1 + HttpRuleParser.GetWhitespaceLength(input, startIndex1);
      }
      else
      {
        int productLength = ProductHeaderValue.GetProductLength(input, num1, out parsedValue1);
        if (productLength == 0)
          return 0;
        num2 = num1 + productLength;
      }
      parsedValue = new ProductInfoHeaderValue();
      parsedValue.product = parsedValue1;
      parsedValue.comment = str;
      return num2 - startIndex;
    }

    /// <returns>Returns <see cref="T:System.Object" />.</returns>
    object ICloneable.Clone() => (object) new ProductInfoHeaderValue(this);
  }
}
