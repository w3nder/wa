// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.ProductHeaderValue
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Diagnostics.Contracts;

#nullable disable
namespace System.Net.Http.Headers
{
  /// <summary>Represents a product header value.</summary>
  public class ProductHeaderValue : ICloneable
  {
    private string name;
    private string version;

    /// <returns>Returns <see cref="T:System.String" />.</returns>
    public string Name => this.name;

    /// <returns>Returns <see cref="T:System.String" />.</returns>
    public string Version => this.version;

    public ProductHeaderValue(string name)
      : this(name, (string) null)
    {
    }

    public ProductHeaderValue(string name, string version)
    {
      HeaderUtilities.CheckValidToken(name, nameof (name));
      if (!string.IsNullOrEmpty(version))
      {
        HeaderUtilities.CheckValidToken(version, nameof (version));
        this.version = version;
      }
      this.name = name;
    }

    private ProductHeaderValue(ProductHeaderValue source)
    {
      Contract.Requires(source != null);
      this.name = source.name;
      this.version = source.version;
    }

    private ProductHeaderValue()
    {
    }

    /// <returns>Returns <see cref="T:System.String" />.</returns>
    public override string ToString()
    {
      return string.IsNullOrEmpty(this.version) ? this.name : this.name + "/" + this.version;
    }

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public override bool Equals(object obj)
    {
      return obj is ProductHeaderValue productHeaderValue && string.Compare(this.name, productHeaderValue.name, StringComparison.OrdinalIgnoreCase) == 0 && string.Compare(this.version, productHeaderValue.version, StringComparison.OrdinalIgnoreCase) == 0;
    }

    /// <returns>Returns <see cref="T:System.Int32" />.</returns>
    public override int GetHashCode()
    {
      int hashCode = this.name.ToLowerInvariant().GetHashCode();
      if (!string.IsNullOrEmpty(this.version))
        hashCode ^= this.version.ToLowerInvariant().GetHashCode();
      return hashCode;
    }

    /// <returns>Returns <see cref="T:System.Net.Http.Headers.ProductHeaderValue" />.</returns>
    public static ProductHeaderValue Parse(string input)
    {
      int index = 0;
      return (ProductHeaderValue) GenericHeaderParser.SingleValueProductParser.ParseValue(input, (object) null, ref index);
    }

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public static bool TryParse(string input, out ProductHeaderValue parsedValue)
    {
      int index = 0;
      parsedValue = (ProductHeaderValue) null;
      object parsedValue1;
      if (!GenericHeaderParser.SingleValueProductParser.TryParseValue(input, (object) null, ref index, out parsedValue1))
        return false;
      parsedValue = (ProductHeaderValue) parsedValue1;
      return true;
    }

    internal static int GetProductLength(
      string input,
      int startIndex,
      out ProductHeaderValue parsedValue)
    {
      Contract.Requires(startIndex >= 0);
      parsedValue = (ProductHeaderValue) null;
      if (string.IsNullOrEmpty(input) || startIndex >= input.Length)
        return 0;
      int tokenLength1 = HttpRuleParser.GetTokenLength(input, startIndex);
      if (tokenLength1 == 0)
        return 0;
      ProductHeaderValue productHeaderValue = new ProductHeaderValue();
      productHeaderValue.name = input.Substring(startIndex, tokenLength1);
      int startIndex1 = startIndex + tokenLength1;
      int index = startIndex1 + HttpRuleParser.GetWhitespaceLength(input, startIndex1);
      if (index == input.Length || input[index] != '/')
      {
        parsedValue = productHeaderValue;
        return index - startIndex;
      }
      int startIndex2 = index + 1;
      int startIndex3 = startIndex2 + HttpRuleParser.GetWhitespaceLength(input, startIndex2);
      int tokenLength2 = HttpRuleParser.GetTokenLength(input, startIndex3);
      if (tokenLength2 == 0)
        return 0;
      productHeaderValue.version = input.Substring(startIndex3, tokenLength2);
      int startIndex4 = startIndex3 + tokenLength2;
      int num = startIndex4 + HttpRuleParser.GetWhitespaceLength(input, startIndex4);
      parsedValue = productHeaderValue;
      return num - startIndex;
    }

    /// <returns>Returns <see cref="T:System.Object" />.</returns>
    object ICloneable.Clone() => (object) new ProductHeaderValue(this);
  }
}
