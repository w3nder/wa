// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.HeaderUtilities
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Net.Mail;
using System.Text;

#nullable disable
namespace System.Net.Http.Headers
{
  internal static class HeaderUtilities
  {
    private const string qualityName = "q";
    internal const string ConnectionClose = "close";
    internal const string BytesUnit = "bytes";
    internal static readonly TransferCodingHeaderValue TransferEncodingChunked = new TransferCodingHeaderValue("chunked");
    internal static readonly NameValueWithParametersHeaderValue ExpectContinue = new NameValueWithParametersHeaderValue("100-continue");
    internal static readonly Action<HttpHeaderValueCollection<string>, string> TokenValidator = new Action<HttpHeaderValueCollection<string>, string>(HeaderUtilities.ValidateToken);

    internal static void SetQuality(ICollection<NameValueHeaderValue> parameters, double? value)
    {
      Contract.Requires(parameters != null);
      NameValueHeaderValue valueHeaderValue = NameValueHeaderValue.Find(parameters, "q");
      if (value.HasValue)
      {
        double? nullable1 = value;
        if ((nullable1.GetValueOrDefault() >= 0.0 ? 0 : (nullable1.HasValue ? 1 : 0)) == 0)
        {
          double? nullable2 = value;
          if ((nullable2.GetValueOrDefault() <= 1.0 ? 0 : (nullable2.HasValue ? 1 : 0)) == 0)
          {
            string str = value.Value.ToString("0.0##", (IFormatProvider) NumberFormatInfo.InvariantInfo);
            if (valueHeaderValue != null)
            {
              valueHeaderValue.Value = str;
              return;
            }
            parameters.Add(new NameValueHeaderValue("q", str));
            return;
          }
        }
        throw new System.Net.Http.ArgumentOutOfRangeException(nameof (value));
      }
      if (valueHeaderValue == null)
        return;
      parameters.Remove(valueHeaderValue);
    }

    internal static double? GetQuality(ICollection<NameValueHeaderValue> parameters)
    {
      Contract.Requires(parameters != null);
      NameValueHeaderValue valueHeaderValue = NameValueHeaderValue.Find(parameters, "q");
      if (valueHeaderValue != null)
      {
        double result = 0.0;
        if (double.TryParse(valueHeaderValue.Value, NumberStyles.AllowDecimalPoint, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result))
          return new double?(result);
        if (Logging.On)
          Logging.PrintError(Logging.Http, string.Format((IFormatProvider) CultureInfo.InvariantCulture, System.Net.Http.SR.net_http_log_headers_invalid_quality, (object) valueHeaderValue.Value));
      }
      return new double?();
    }

    internal static void CheckValidToken(string value, string parameterName)
    {
      if (string.IsNullOrEmpty(value))
        throw new ArgumentException(System.Net.Http.SR.net_http_argument_empty_string, parameterName);
      if (HttpRuleParser.GetTokenLength(value, 0) != value.Length)
        throw new FormatException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, System.Net.Http.SR.net_http_headers_invalid_value, (object) value));
    }

    internal static void CheckValidComment(string value, string parameterName)
    {
      if (string.IsNullOrEmpty(value))
        throw new ArgumentException(System.Net.Http.SR.net_http_argument_empty_string, parameterName);
      int length = 0;
      if (HttpRuleParser.GetCommentLength(value, 0, out length) != HttpParseResult.Parsed || length != value.Length)
        throw new FormatException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, System.Net.Http.SR.net_http_headers_invalid_value, (object) value));
    }

    internal static void CheckValidQuotedString(string value, string parameterName)
    {
      if (string.IsNullOrEmpty(value))
        throw new ArgumentException(System.Net.Http.SR.net_http_argument_empty_string, parameterName);
      int length = 0;
      if (HttpRuleParser.GetQuotedStringLength(value, 0, out length) != HttpParseResult.Parsed || length != value.Length)
        throw new FormatException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, System.Net.Http.SR.net_http_headers_invalid_value, (object) value));
    }

    internal static bool AreEqualCollections<T>(ICollection<T> x, ICollection<T> y)
    {
      return HeaderUtilities.AreEqualCollections<T>(x, y, (IEqualityComparer<T>) null);
    }

    internal static bool AreEqualCollections<T>(
      ICollection<T> x,
      ICollection<T> y,
      IEqualityComparer<T> comparer)
    {
      if (x == null)
        return y == null || y.Count == 0;
      if (y == null)
        return x.Count == 0;
      if (x.Count != y.Count)
        return false;
      if (x.Count == 0)
        return true;
      bool[] collection = new bool[x.Count];
      foreach (T x1 in (IEnumerable<T>) x)
      {
        Contract.Assert((object) x1 != null);
        int index = 0;
        bool flag = false;
        foreach (T y1 in (IEnumerable<T>) y)
        {
          if (!collection[index] && (comparer == null && x1.Equals((object) y1) || comparer != null && comparer.Equals(x1, y1)))
          {
            collection[index] = true;
            flag = true;
            break;
          }
          ++index;
        }
        if (!flag)
          return false;
      }
      Contract.Assert(Contract.ForAll<bool>((IEnumerable<bool>) collection, (Predicate<bool>) (value => value)), "Expected all values in 'alreadyFound' to be true since collections are considered equal.");
      return true;
    }

    internal static int GetNextNonEmptyOrWhitespaceIndex(
      string input,
      int startIndex,
      bool skipEmptyValues,
      out bool separatorFound)
    {
      Contract.Requires(input != null);
      Contract.Requires(startIndex <= input.Length);
      separatorFound = false;
      int index1 = startIndex + HttpRuleParser.GetWhitespaceLength(input, startIndex);
      if (index1 == input.Length || input[index1] != ',')
        return index1;
      separatorFound = true;
      int startIndex1 = index1 + 1;
      int index2 = startIndex1 + HttpRuleParser.GetWhitespaceLength(input, startIndex1);
      int startIndex2;
      if (skipEmptyValues)
      {
        for (; index2 < input.Length && input[index2] == ','; index2 = startIndex2 + HttpRuleParser.GetWhitespaceLength(input, startIndex2))
          startIndex2 = index2 + 1;
      }
      return index2;
    }

    internal static DateTimeOffset? GetDateTimeOffsetValue(string headerName, HttpHeaders store)
    {
      Contract.Requires(store != null);
      object parsedValues = store.GetParsedValues(headerName);
      return parsedValues != null ? new DateTimeOffset?((DateTimeOffset) parsedValues) : new DateTimeOffset?();
    }

    internal static TimeSpan? GetTimeSpanValue(string headerName, HttpHeaders store)
    {
      Contract.Requires(store != null);
      object parsedValues = store.GetParsedValues(headerName);
      return parsedValues != null ? new TimeSpan?((TimeSpan) parsedValues) : new TimeSpan?();
    }

    internal static bool TryParseInt32(string value, out int result)
    {
      return int.TryParse(value, NumberStyles.None, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result);
    }

    internal static bool TryParseInt64(string value, out long result)
    {
      return long.TryParse(value, NumberStyles.None, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result);
    }

    internal static string DumpHeaders(params HttpHeaders[] headers)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("{\r\n");
      for (int index = 0; index < headers.Length; ++index)
      {
        if (headers[index] != null)
        {
          foreach (KeyValuePair<string, IEnumerable<string>> keyValuePair in headers[index])
          {
            foreach (string str in keyValuePair.Value)
            {
              stringBuilder.Append("  ");
              stringBuilder.Append(keyValuePair.Key);
              stringBuilder.Append(": ");
              stringBuilder.Append(str);
              stringBuilder.Append("\r\n");
            }
          }
        }
      }
      stringBuilder.Append('}');
      return stringBuilder.ToString();
    }

    internal static bool IsValidEmailAddress(string value)
    {
      try
      {
        MailAddress mailAddress = new MailAddress(value);
        return true;
      }
      catch (FormatException ex)
      {
        if (Logging.On)
          Logging.PrintError(Logging.Http, string.Format((IFormatProvider) CultureInfo.InvariantCulture, System.Net.Http.SR.net_http_log_headers_wrong_email_format, (object) value, (object) ex.Message));
      }
      return false;
    }

    private static void ValidateToken(HttpHeaderValueCollection<string> collection, string value)
    {
      HeaderUtilities.CheckValidToken(value, "item");
    }
  }
}
