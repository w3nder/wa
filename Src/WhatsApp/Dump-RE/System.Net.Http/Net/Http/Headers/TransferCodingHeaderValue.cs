// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.TransferCodingHeaderValue
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Collections.Generic;
using System.Diagnostics.Contracts;

#nullable disable
namespace System.Net.Http.Headers
{
  /// <summary>Represents a transfer-coding header value.</summary>
  public class TransferCodingHeaderValue : ICloneable
  {
    private ICollection<NameValueHeaderValue> parameters;
    private string value;

    /// <returns>Returns <see cref="T:System.String" />.</returns>
    public string Value => this.value;

    /// <returns>Returns <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
    public ICollection<NameValueHeaderValue> Parameters
    {
      get
      {
        if (this.parameters == null)
          this.parameters = (ICollection<NameValueHeaderValue>) new ObjectCollection<NameValueHeaderValue>();
        return this.parameters;
      }
    }

    internal TransferCodingHeaderValue()
    {
    }

    protected TransferCodingHeaderValue(TransferCodingHeaderValue source)
    {
      Contract.Requires(source != null);
      this.value = source.value;
      if (source.parameters == null)
        return;
      foreach (ICloneable parameter in (IEnumerable<NameValueHeaderValue>) source.parameters)
        this.Parameters.Add((NameValueHeaderValue) parameter.Clone());
    }

    public TransferCodingHeaderValue(string value)
    {
      HeaderUtilities.CheckValidToken(value, nameof (value));
      this.value = value;
    }

    /// <returns>Returns <see cref="T:System.Net.Http.Headers.TransferCodingHeaderValue" />.</returns>
    public static TransferCodingHeaderValue Parse(string input)
    {
      int index = 0;
      return (TransferCodingHeaderValue) TransferCodingHeaderParser.SingleValueParser.ParseValue(input, (object) null, ref index);
    }

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public static bool TryParse(string input, out TransferCodingHeaderValue parsedValue)
    {
      int index = 0;
      parsedValue = (TransferCodingHeaderValue) null;
      object parsedValue1;
      if (!TransferCodingHeaderParser.SingleValueParser.TryParseValue(input, (object) null, ref index, out parsedValue1))
        return false;
      parsedValue = (TransferCodingHeaderValue) parsedValue1;
      return true;
    }

    internal static int GetTransferCodingLength(
      string input,
      int startIndex,
      Func<TransferCodingHeaderValue> transferCodingCreator,
      out TransferCodingHeaderValue parsedValue)
    {
      Contract.Requires(transferCodingCreator != null);
      Contract.Requires(startIndex >= 0);
      parsedValue = (TransferCodingHeaderValue) null;
      if (string.IsNullOrEmpty(input) || startIndex >= input.Length)
        return 0;
      int tokenLength = HttpRuleParser.GetTokenLength(input, startIndex);
      if (tokenLength == 0)
        return 0;
      string str = input.Substring(startIndex, tokenLength);
      int startIndex1 = startIndex + tokenLength;
      int index = startIndex1 + HttpRuleParser.GetWhitespaceLength(input, startIndex1);
      if (index < input.Length && input[index] == ';')
      {
        TransferCodingHeaderValue codingHeaderValue = transferCodingCreator();
        codingHeaderValue.value = str;
        int startIndex2 = index + 1;
        int nameValueListLength = NameValueHeaderValue.GetNameValueListLength(input, startIndex2, ';', codingHeaderValue.Parameters);
        if (nameValueListLength == 0)
          return 0;
        parsedValue = codingHeaderValue;
        return startIndex2 + nameValueListLength - startIndex;
      }
      TransferCodingHeaderValue codingHeaderValue1 = transferCodingCreator();
      codingHeaderValue1.value = str;
      parsedValue = codingHeaderValue1;
      return index - startIndex;
    }

    /// <returns>Returns <see cref="T:System.String" />.</returns>
    public override string ToString()
    {
      return this.value + NameValueHeaderValue.ToString(this.parameters, ';', true);
    }

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public override bool Equals(object obj)
    {
      return obj is TransferCodingHeaderValue codingHeaderValue && string.Compare(this.value, codingHeaderValue.value, StringComparison.OrdinalIgnoreCase) == 0 && HeaderUtilities.AreEqualCollections<NameValueHeaderValue>(this.parameters, codingHeaderValue.parameters);
    }

    /// <returns>Returns <see cref="T:System.Int32" />.</returns>
    public override int GetHashCode()
    {
      return this.value.ToLowerInvariant().GetHashCode() ^ NameValueHeaderValue.GetHashCode(this.parameters);
    }

    /// <returns>Returns <see cref="T:System.Object" />.</returns>
    object ICloneable.Clone() => (object) new TransferCodingHeaderValue(this);
  }
}
