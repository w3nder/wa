// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.ViaHeaderValue
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Diagnostics.Contracts;
using System.Globalization;
using System.Text;

#nullable disable
namespace System.Net.Http.Headers
{
  /// <summary>Represents the value of a Via header.</summary>
  public class ViaHeaderValue : ICloneable
  {
    private string protocolName;
    private string protocolVersion;
    private string receivedBy;
    private string comment;

    /// <returns>Returns <see cref="T:System.String" />.</returns>
    public string ProtocolName => this.protocolName;

    /// <returns>Returns <see cref="T:System.String" />.</returns>
    public string ProtocolVersion => this.protocolVersion;

    /// <returns>Returns <see cref="T:System.String" />.</returns>
    public string ReceivedBy => this.receivedBy;

    /// <returns>Returns <see cref="T:System.String" />.</returns>
    public string Comment => this.comment;

    public ViaHeaderValue(string protocolVersion, string receivedBy)
      : this(protocolVersion, receivedBy, (string) null, (string) null)
    {
    }

    public ViaHeaderValue(string protocolVersion, string receivedBy, string protocolName)
      : this(protocolVersion, receivedBy, protocolName, (string) null)
    {
    }

    public ViaHeaderValue(
      string protocolVersion,
      string receivedBy,
      string protocolName,
      string comment)
    {
      HeaderUtilities.CheckValidToken(protocolVersion, nameof (protocolVersion));
      ViaHeaderValue.CheckReceivedBy(receivedBy);
      if (!string.IsNullOrEmpty(protocolName))
      {
        HeaderUtilities.CheckValidToken(protocolName, nameof (protocolName));
        this.protocolName = protocolName;
      }
      if (!string.IsNullOrEmpty(comment))
      {
        HeaderUtilities.CheckValidComment(comment, nameof (comment));
        this.comment = comment;
      }
      this.protocolVersion = protocolVersion;
      this.receivedBy = receivedBy;
    }

    private ViaHeaderValue()
    {
    }

    private ViaHeaderValue(ViaHeaderValue source)
    {
      Contract.Requires(source != null);
      this.protocolName = source.protocolName;
      this.protocolVersion = source.protocolVersion;
      this.receivedBy = source.receivedBy;
      this.comment = source.comment;
    }

    /// <returns>Returns <see cref="T:System.String" />.</returns>
    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (!string.IsNullOrEmpty(this.protocolName))
      {
        stringBuilder.Append(this.protocolName);
        stringBuilder.Append('/');
      }
      stringBuilder.Append(this.protocolVersion);
      stringBuilder.Append(' ');
      stringBuilder.Append(this.receivedBy);
      if (!string.IsNullOrEmpty(this.comment))
      {
        stringBuilder.Append(' ');
        stringBuilder.Append(this.comment);
      }
      return stringBuilder.ToString();
    }

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public override bool Equals(object obj)
    {
      return obj is ViaHeaderValue viaHeaderValue && string.Compare(this.protocolVersion, viaHeaderValue.protocolVersion, StringComparison.OrdinalIgnoreCase) == 0 && string.Compare(this.receivedBy, viaHeaderValue.receivedBy, StringComparison.OrdinalIgnoreCase) == 0 && string.Compare(this.protocolName, viaHeaderValue.protocolName, StringComparison.OrdinalIgnoreCase) == 0 && string.CompareOrdinal(this.comment, viaHeaderValue.comment) == 0;
    }

    /// <returns>Returns <see cref="T:System.Int32" />.</returns>
    public override int GetHashCode()
    {
      int hashCode = this.protocolVersion.ToLowerInvariant().GetHashCode() ^ this.receivedBy.ToLowerInvariant().GetHashCode();
      if (!string.IsNullOrEmpty(this.protocolName))
        hashCode ^= this.protocolName.ToLowerInvariant().GetHashCode();
      if (!string.IsNullOrEmpty(this.comment))
        hashCode ^= this.comment.GetHashCode();
      return hashCode;
    }

    /// <returns>Returns <see cref="T:System.Net.Http.Headers.ViaHeaderValue" />.</returns>
    public static ViaHeaderValue Parse(string input)
    {
      int index = 0;
      return (ViaHeaderValue) GenericHeaderParser.SingleValueViaParser.ParseValue(input, (object) null, ref index);
    }

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public static bool TryParse(string input, out ViaHeaderValue parsedValue)
    {
      int index = 0;
      parsedValue = (ViaHeaderValue) null;
      object parsedValue1;
      if (!GenericHeaderParser.SingleValueViaParser.TryParseValue(input, (object) null, ref index, out parsedValue1))
        return false;
      parsedValue = (ViaHeaderValue) parsedValue1;
      return true;
    }

    internal static int GetViaLength(string input, int startIndex, out object parsedValue)
    {
      Contract.Requires(startIndex >= 0);
      parsedValue = (object) null;
      if (string.IsNullOrEmpty(input) || startIndex >= input.Length)
        return 0;
      string protocolName = (string) null;
      string protocolVersion = (string) null;
      int protocolEndIndex = ViaHeaderValue.GetProtocolEndIndex(input, startIndex, out protocolName, out protocolVersion);
      if (protocolEndIndex == startIndex || protocolEndIndex == input.Length)
        return 0;
      Contract.Assert(protocolVersion != null);
      string host = (string) null;
      int hostLength = HttpRuleParser.GetHostLength(input, protocolEndIndex, true, out host);
      if (hostLength == 0)
        return 0;
      int startIndex1 = protocolEndIndex + hostLength;
      int num = startIndex1 + HttpRuleParser.GetWhitespaceLength(input, startIndex1);
      string str = (string) null;
      if (num < input.Length && input[num] == '(')
      {
        int length = 0;
        if (HttpRuleParser.GetCommentLength(input, num, out length) != HttpParseResult.Parsed)
          return 0;
        str = input.Substring(num, length);
        int startIndex2 = num + length;
        num = startIndex2 + HttpRuleParser.GetWhitespaceLength(input, startIndex2);
      }
      parsedValue = (object) new ViaHeaderValue()
      {
        protocolVersion = protocolVersion,
        protocolName = protocolName,
        receivedBy = host,
        comment = str
      };
      return num - startIndex;
    }

    private static int GetProtocolEndIndex(
      string input,
      int startIndex,
      out string protocolName,
      out string protocolVersion)
    {
      protocolName = (string) null;
      protocolVersion = (string) null;
      int startIndex1 = startIndex;
      int tokenLength1 = HttpRuleParser.GetTokenLength(input, startIndex1);
      if (tokenLength1 == 0)
        return 0;
      int startIndex2 = startIndex + tokenLength1;
      int whitespaceLength = HttpRuleParser.GetWhitespaceLength(input, startIndex2);
      int index = startIndex2 + whitespaceLength;
      if (index == input.Length)
        return 0;
      if (input[index] == '/')
      {
        protocolName = input.Substring(startIndex, tokenLength1);
        int startIndex3 = index + 1;
        int startIndex4 = startIndex3 + HttpRuleParser.GetWhitespaceLength(input, startIndex3);
        int tokenLength2 = HttpRuleParser.GetTokenLength(input, startIndex4);
        if (tokenLength2 == 0)
          return 0;
        protocolVersion = input.Substring(startIndex4, tokenLength2);
        int startIndex5 = startIndex4 + tokenLength2;
        whitespaceLength = HttpRuleParser.GetWhitespaceLength(input, startIndex5);
        index = startIndex5 + whitespaceLength;
      }
      else
        protocolVersion = input.Substring(startIndex, tokenLength1);
      return whitespaceLength == 0 ? 0 : index;
    }

    /// <returns>Returns <see cref="T:System.Object" />.</returns>
    object ICloneable.Clone() => (object) new ViaHeaderValue(this);

    private static void CheckReceivedBy(string receivedBy)
    {
      if (string.IsNullOrEmpty(receivedBy))
        throw new ArgumentException(SR.net_http_argument_empty_string, nameof (receivedBy));
      string host = (string) null;
      if (HttpRuleParser.GetHostLength(receivedBy, 0, true, out host) != receivedBy.Length)
        throw new FormatException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, SR.net_http_headers_invalid_value, (object) receivedBy));
    }
  }
}
