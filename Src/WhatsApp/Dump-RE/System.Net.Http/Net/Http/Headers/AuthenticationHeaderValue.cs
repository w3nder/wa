// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.AuthenticationHeaderValue
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Diagnostics.Contracts;

#nullable disable
namespace System.Net.Http.Headers
{
  /// <summary>Represents authentication information in Authorization, ProxyAuthorization, WWW-Authneticate, and Proxy-Authenticate header values.</summary>
  public class AuthenticationHeaderValue : ICloneable
  {
    private string scheme;
    private string parameter;

    /// <returns>Returns <see cref="T:System.String" />.</returns>
    public string Scheme => this.scheme;

    /// <returns>Returns <see cref="T:System.String" />.</returns>
    public string Parameter => this.parameter;

    public AuthenticationHeaderValue(string scheme)
      : this(scheme, (string) null)
    {
    }

    public AuthenticationHeaderValue(string scheme, string parameter)
    {
      HeaderUtilities.CheckValidToken(scheme, nameof (scheme));
      this.scheme = scheme;
      this.parameter = parameter;
    }

    private AuthenticationHeaderValue(AuthenticationHeaderValue source)
    {
      Contract.Requires(source != null);
      this.scheme = source.scheme;
      this.parameter = source.parameter;
    }

    private AuthenticationHeaderValue()
    {
    }

    /// <returns>Returns <see cref="T:System.String" />.</returns>
    public override string ToString()
    {
      return string.IsNullOrEmpty(this.parameter) ? this.scheme : this.scheme + " " + this.parameter;
    }

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public override bool Equals(object obj)
    {
      if (!(obj is AuthenticationHeaderValue authenticationHeaderValue))
        return false;
      if (string.IsNullOrEmpty(this.parameter) && string.IsNullOrEmpty(authenticationHeaderValue.parameter))
        return string.Compare(this.scheme, authenticationHeaderValue.scheme, StringComparison.OrdinalIgnoreCase) == 0;
      return string.Compare(this.scheme, authenticationHeaderValue.scheme, StringComparison.OrdinalIgnoreCase) == 0 && string.CompareOrdinal(this.parameter, authenticationHeaderValue.parameter) == 0;
    }

    /// <returns>Returns <see cref="T:System.Int32" />.</returns>
    public override int GetHashCode()
    {
      int hashCode = this.scheme.ToLowerInvariant().GetHashCode();
      if (!string.IsNullOrEmpty(this.parameter))
        hashCode ^= this.parameter.GetHashCode();
      return hashCode;
    }

    /// <returns>Returns <see cref="T:System.Net.Http.Headers.AuthenticationHeaderValue" />.</returns>
    public static AuthenticationHeaderValue Parse(string input)
    {
      int index = 0;
      return (AuthenticationHeaderValue) GenericHeaderParser.SingleValueAuthenticationParser.ParseValue(input, (object) null, ref index);
    }

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public static bool TryParse(string input, out AuthenticationHeaderValue parsedValue)
    {
      int index = 0;
      parsedValue = (AuthenticationHeaderValue) null;
      object parsedValue1;
      if (!GenericHeaderParser.SingleValueAuthenticationParser.TryParseValue(input, (object) null, ref index, out parsedValue1))
        return false;
      parsedValue = (AuthenticationHeaderValue) parsedValue1;
      return true;
    }

    internal static int GetAuthenticationLength(
      string input,
      int startIndex,
      out object parsedValue)
    {
      Contract.Requires(startIndex >= 0);
      parsedValue = (object) null;
      if (string.IsNullOrEmpty(input) || startIndex >= input.Length)
        return 0;
      int tokenLength = HttpRuleParser.GetTokenLength(input, startIndex);
      if (tokenLength == 0)
        return 0;
      AuthenticationHeaderValue authenticationHeaderValue = new AuthenticationHeaderValue();
      authenticationHeaderValue.scheme = input.Substring(startIndex, tokenLength);
      int startIndex1 = startIndex + tokenLength;
      int whitespaceLength = HttpRuleParser.GetWhitespaceLength(input, startIndex1);
      int index = startIndex1 + whitespaceLength;
      if (index == input.Length || input[index] == ',')
      {
        parsedValue = (object) authenticationHeaderValue;
        return index - startIndex;
      }
      if (whitespaceLength == 0)
        return 0;
      int startIndex2 = index;
      int parameterEndIndex = index;
      if (!AuthenticationHeaderValue.TrySkipFirstBlob(input, ref index, ref parameterEndIndex) || index < input.Length && !AuthenticationHeaderValue.TryGetParametersEndIndex(input, ref index, ref parameterEndIndex))
        return 0;
      authenticationHeaderValue.parameter = input.Substring(startIndex2, parameterEndIndex - startIndex2 + 1);
      parsedValue = (object) authenticationHeaderValue;
      return index - startIndex;
    }

    private static bool TrySkipFirstBlob(string input, ref int current, ref int parameterEndIndex)
    {
      while (current < input.Length && input[current] != ',')
      {
        if (input[current] == '"')
        {
          int length = 0;
          if (HttpRuleParser.GetQuotedStringLength(input, current, out length) != HttpParseResult.Parsed)
            return false;
          current += length;
          parameterEndIndex = current - 1;
        }
        else
        {
          int whitespaceLength = HttpRuleParser.GetWhitespaceLength(input, current);
          if (whitespaceLength == 0)
          {
            parameterEndIndex = current;
            ++current;
          }
          else
            current += whitespaceLength;
        }
      }
      return true;
    }

    private static bool TryGetParametersEndIndex(
      string input,
      ref int parseEndIndex,
      ref int parameterEndIndex)
    {
      Contract.Requires(parseEndIndex < input.Length, "Expected string to have at least 1 char");
      Contract.Assert(input[parseEndIndex] == ',');
      int index1 = parseEndIndex;
      do
      {
        int startIndex1 = index1 + 1;
        bool separatorFound = false;
        int orWhitespaceIndex = HeaderUtilities.GetNextNonEmptyOrWhitespaceIndex(input, startIndex1, true, out separatorFound);
        if (orWhitespaceIndex == input.Length)
          return true;
        int tokenLength = HttpRuleParser.GetTokenLength(input, orWhitespaceIndex);
        if (tokenLength == 0)
          return false;
        int startIndex2 = orWhitespaceIndex + tokenLength;
        int index2 = startIndex2 + HttpRuleParser.GetWhitespaceLength(input, startIndex2);
        if (index2 == input.Length || input[index2] != '=')
          return true;
        int startIndex3 = index2 + 1;
        int startIndex4 = startIndex3 + HttpRuleParser.GetWhitespaceLength(input, startIndex3);
        int valueLength = NameValueHeaderValue.GetValueLength(input, startIndex4);
        if (valueLength == 0)
          return false;
        int startIndex5 = startIndex4 + valueLength;
        parameterEndIndex = startIndex5 - 1;
        index1 = startIndex5 + HttpRuleParser.GetWhitespaceLength(input, startIndex5);
        parseEndIndex = index1;
      }
      while (index1 < input.Length && input[index1] == ',');
      return true;
    }

    /// <returns>Returns <see cref="T:System.Object" />.</returns>
    object ICloneable.Clone() => (object) new AuthenticationHeaderValue(this);
  }
}
