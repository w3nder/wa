// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.MediaTypeHeaderValue
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;

#nullable disable
namespace System.Net.Http.Headers
{
  /// <summary>Represents a media-type as defined in the RFC 2616.</summary>
  public class MediaTypeHeaderValue : ICloneable
  {
    private const string charSet = "charset";
    private ICollection<NameValueHeaderValue> parameters;
    private string mediaType;

    /// <returns>Returns <see cref="T:System.String" />.</returns>
    public string CharSet
    {
      get => NameValueHeaderValue.Find(this.parameters, "charset")?.Value;
      set
      {
        NameValueHeaderValue valueHeaderValue = NameValueHeaderValue.Find(this.parameters, "charset");
        if (string.IsNullOrEmpty(value))
        {
          if (valueHeaderValue == null)
            return;
          this.parameters.Remove(valueHeaderValue);
        }
        else if (valueHeaderValue != null)
          valueHeaderValue.Value = value;
        else
          this.Parameters.Add(new NameValueHeaderValue("charset", value));
      }
    }

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

    /// <returns>Returns <see cref="T:System.String" />.</returns>
    public string MediaType
    {
      get => this.mediaType;
      set
      {
        MediaTypeHeaderValue.CheckMediaTypeFormat(value, nameof (value));
        this.mediaType = value;
      }
    }

    internal MediaTypeHeaderValue()
    {
    }

    protected MediaTypeHeaderValue(MediaTypeHeaderValue source)
    {
      Contract.Requires(source != null);
      this.mediaType = source.mediaType;
      if (source.parameters == null)
        return;
      foreach (ICloneable parameter in (IEnumerable<NameValueHeaderValue>) source.parameters)
        this.Parameters.Add((NameValueHeaderValue) parameter.Clone());
    }

    public MediaTypeHeaderValue(string mediaType)
    {
      MediaTypeHeaderValue.CheckMediaTypeFormat(mediaType, nameof (mediaType));
      this.mediaType = mediaType;
    }

    /// <returns>Returns <see cref="T:System.String" />.</returns>
    public override string ToString()
    {
      return this.mediaType + NameValueHeaderValue.ToString(this.parameters, ';', true);
    }

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public override bool Equals(object obj)
    {
      return obj is MediaTypeHeaderValue mediaTypeHeaderValue && string.Compare(this.mediaType, mediaTypeHeaderValue.mediaType, StringComparison.OrdinalIgnoreCase) == 0 && HeaderUtilities.AreEqualCollections<NameValueHeaderValue>(this.parameters, mediaTypeHeaderValue.parameters);
    }

    /// <returns>Returns <see cref="T:System.Int32" />.</returns>
    public override int GetHashCode()
    {
      return this.mediaType.ToLowerInvariant().GetHashCode() ^ NameValueHeaderValue.GetHashCode(this.parameters);
    }

    /// <returns>Returns <see cref="T:System.Net.Http.Headers.MediaTypeHeaderValue" />.</returns>
    public static MediaTypeHeaderValue Parse(string input)
    {
      int index = 0;
      return (MediaTypeHeaderValue) MediaTypeHeaderParser.SingleValueParser.ParseValue(input, (object) null, ref index);
    }

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public static bool TryParse(string input, out MediaTypeHeaderValue parsedValue)
    {
      int index = 0;
      parsedValue = (MediaTypeHeaderValue) null;
      object parsedValue1;
      if (!MediaTypeHeaderParser.SingleValueParser.TryParseValue(input, (object) null, ref index, out parsedValue1))
        return false;
      parsedValue = (MediaTypeHeaderValue) parsedValue1;
      return true;
    }

    internal static int GetMediaTypeLength(
      string input,
      int startIndex,
      Func<MediaTypeHeaderValue> mediaTypeCreator,
      out MediaTypeHeaderValue parsedValue)
    {
      Contract.Requires(mediaTypeCreator != null);
      Contract.Requires(startIndex >= 0);
      parsedValue = (MediaTypeHeaderValue) null;
      if (string.IsNullOrEmpty(input) || startIndex >= input.Length)
        return 0;
      string mediaType = (string) null;
      int expressionLength = MediaTypeHeaderValue.GetMediaTypeExpressionLength(input, startIndex, out mediaType);
      if (expressionLength == 0)
        return 0;
      int startIndex1 = startIndex + expressionLength;
      int index = startIndex1 + HttpRuleParser.GetWhitespaceLength(input, startIndex1);
      if (index < input.Length && input[index] == ';')
      {
        MediaTypeHeaderValue mediaTypeHeaderValue = mediaTypeCreator();
        mediaTypeHeaderValue.mediaType = mediaType;
        int startIndex2 = index + 1;
        int nameValueListLength = NameValueHeaderValue.GetNameValueListLength(input, startIndex2, ';', mediaTypeHeaderValue.Parameters);
        if (nameValueListLength == 0)
          return 0;
        parsedValue = mediaTypeHeaderValue;
        return startIndex2 + nameValueListLength - startIndex;
      }
      MediaTypeHeaderValue mediaTypeHeaderValue1 = mediaTypeCreator();
      mediaTypeHeaderValue1.mediaType = mediaType;
      parsedValue = mediaTypeHeaderValue1;
      return index - startIndex;
    }

    private static int GetMediaTypeExpressionLength(
      string input,
      int startIndex,
      out string mediaType)
    {
      Contract.Requires(input != null && input.Length > 0 && startIndex < input.Length);
      mediaType = (string) null;
      int tokenLength1 = HttpRuleParser.GetTokenLength(input, startIndex);
      if (tokenLength1 == 0)
        return 0;
      int startIndex1 = startIndex + tokenLength1;
      int index = startIndex1 + HttpRuleParser.GetWhitespaceLength(input, startIndex1);
      if (index >= input.Length || input[index] != '/')
        return 0;
      int startIndex2 = index + 1;
      int startIndex3 = startIndex2 + HttpRuleParser.GetWhitespaceLength(input, startIndex2);
      int tokenLength2 = HttpRuleParser.GetTokenLength(input, startIndex3);
      if (tokenLength2 == 0)
        return 0;
      int length = startIndex3 + tokenLength2 - startIndex;
      mediaType = tokenLength1 + tokenLength2 + 1 != length ? input.Substring(startIndex, tokenLength1) + "/" + input.Substring(startIndex3, tokenLength2) : input.Substring(startIndex, length);
      return length;
    }

    private static void CheckMediaTypeFormat(string mediaType, string parameterName)
    {
      if (string.IsNullOrEmpty(mediaType))
        throw new ArgumentException(SR.net_http_argument_empty_string, parameterName);
      string mediaType1;
      if (MediaTypeHeaderValue.GetMediaTypeExpressionLength(mediaType, 0, out mediaType1) == 0 || mediaType1.Length != mediaType.Length)
        throw new FormatException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, SR.net_http_headers_invalid_value, (object) mediaType));
    }

    /// <returns>Returns <see cref="T:System.Object" />.</returns>
    object ICloneable.Clone() => (object) new MediaTypeHeaderValue(this);
  }
}
