// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.ContentDispositionHeaderValue
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Text;

#nullable disable
namespace System.Net.Http.Headers
{
  public class ContentDispositionHeaderValue : ICloneable
  {
    private const string fileName = "filename";
    private const string name = "name";
    private const string fileNameStar = "filename*";
    private const string creationDate = "creation-date";
    private const string modificationDate = "modification-date";
    private const string readDate = "read-date";
    private const string size = "size";
    private ICollection<NameValueHeaderValue> parameters;
    private string dispositionType;

    public string DispositionType
    {
      get => this.dispositionType;
      set
      {
        ContentDispositionHeaderValue.CheckDispositionTypeFormat(value, nameof (value));
        this.dispositionType = value;
      }
    }

    public ICollection<NameValueHeaderValue> Parameters
    {
      get
      {
        if (this.parameters == null)
          this.parameters = (ICollection<NameValueHeaderValue>) new ObjectCollection<NameValueHeaderValue>();
        return this.parameters;
      }
    }

    public string Name
    {
      get => this.GetName("name");
      set => this.SetName("name", value);
    }

    public string FileName
    {
      get => this.GetName("filename");
      set => this.SetName("filename", value);
    }

    public string FileNameStar
    {
      get => this.GetName("filename*");
      set => this.SetName("filename*", value);
    }

    public DateTimeOffset? CreationDate
    {
      get => this.GetDate("creation-date");
      set => this.SetDate("creation-date", value);
    }

    public DateTimeOffset? ModificationDate
    {
      get => this.GetDate("modification-date");
      set => this.SetDate("modification-date", value);
    }

    public DateTimeOffset? ReadDate
    {
      get => this.GetDate("read-date");
      set => this.SetDate("read-date", value);
    }

    public long? Size
    {
      get
      {
        NameValueHeaderValue valueHeaderValue = NameValueHeaderValue.Find(this.parameters, "size");
        ulong result;
        return valueHeaderValue != null && ulong.TryParse(valueHeaderValue.Value, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result) ? new long?((long) result) : new long?();
      }
      set
      {
        NameValueHeaderValue valueHeaderValue = NameValueHeaderValue.Find(this.parameters, "size");
        if (!value.HasValue)
        {
          if (valueHeaderValue == null)
            return;
          this.parameters.Remove(valueHeaderValue);
        }
        else
        {
          long? nullable = value;
          if ((nullable.GetValueOrDefault() >= 0L ? 0 : (nullable.HasValue ? 1 : 0)) != 0)
            throw new System.Net.Http.ArgumentOutOfRangeException(nameof (value));
          if (valueHeaderValue != null)
            valueHeaderValue.Value = value.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          else
            this.parameters.Add(new NameValueHeaderValue("size", value.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
        }
      }
    }

    internal ContentDispositionHeaderValue()
    {
    }

    protected ContentDispositionHeaderValue(ContentDispositionHeaderValue source)
    {
      Contract.Requires(source != null);
      this.dispositionType = source.dispositionType;
      if (source.parameters == null)
        return;
      foreach (ICloneable parameter in (IEnumerable<NameValueHeaderValue>) source.parameters)
        this.Parameters.Add((NameValueHeaderValue) parameter.Clone());
    }

    public ContentDispositionHeaderValue(string dispositionType)
    {
      ContentDispositionHeaderValue.CheckDispositionTypeFormat(dispositionType, nameof (dispositionType));
      this.dispositionType = dispositionType;
    }

    public override string ToString()
    {
      return this.dispositionType + NameValueHeaderValue.ToString(this.parameters, ';', true);
    }

    public override bool Equals(object obj)
    {
      return obj is ContentDispositionHeaderValue dispositionHeaderValue && string.Compare(this.dispositionType, dispositionHeaderValue.dispositionType, StringComparison.OrdinalIgnoreCase) == 0 && HeaderUtilities.AreEqualCollections<NameValueHeaderValue>(this.parameters, dispositionHeaderValue.parameters);
    }

    public override int GetHashCode()
    {
      return this.dispositionType.ToLowerInvariant().GetHashCode() ^ NameValueHeaderValue.GetHashCode(this.parameters);
    }

    object ICloneable.Clone() => (object) new ContentDispositionHeaderValue(this);

    public static ContentDispositionHeaderValue Parse(string input)
    {
      int index = 0;
      return (ContentDispositionHeaderValue) GenericHeaderParser.ContentDispositionParser.ParseValue(input, (object) null, ref index);
    }

    public static bool TryParse(string input, out ContentDispositionHeaderValue parsedValue)
    {
      int index = 0;
      parsedValue = (ContentDispositionHeaderValue) null;
      object parsedValue1;
      if (!GenericHeaderParser.ContentDispositionParser.TryParseValue(input, (object) null, ref index, out parsedValue1))
        return false;
      parsedValue = (ContentDispositionHeaderValue) parsedValue1;
      return true;
    }

    internal static int GetDispositionTypeLength(
      string input,
      int startIndex,
      out object parsedValue)
    {
      Contract.Requires(startIndex >= 0);
      parsedValue = (object) null;
      if (string.IsNullOrEmpty(input) || startIndex >= input.Length)
        return 0;
      string dispositionType = (string) null;
      int expressionLength = ContentDispositionHeaderValue.GetDispositionTypeExpressionLength(input, startIndex, out dispositionType);
      if (expressionLength == 0)
        return 0;
      int startIndex1 = startIndex + expressionLength;
      int index = startIndex1 + HttpRuleParser.GetWhitespaceLength(input, startIndex1);
      ContentDispositionHeaderValue dispositionHeaderValue = new ContentDispositionHeaderValue();
      dispositionHeaderValue.dispositionType = dispositionType;
      if (index < input.Length && input[index] == ';')
      {
        int startIndex2 = index + 1;
        int nameValueListLength = NameValueHeaderValue.GetNameValueListLength(input, startIndex2, ';', dispositionHeaderValue.Parameters);
        if (nameValueListLength == 0)
          return 0;
        parsedValue = (object) dispositionHeaderValue;
        return startIndex2 + nameValueListLength - startIndex;
      }
      parsedValue = (object) dispositionHeaderValue;
      return index - startIndex;
    }

    private static int GetDispositionTypeExpressionLength(
      string input,
      int startIndex,
      out string dispositionType)
    {
      Contract.Requires(input != null && input.Length > 0 && startIndex < input.Length);
      dispositionType = (string) null;
      int tokenLength = HttpRuleParser.GetTokenLength(input, startIndex);
      if (tokenLength == 0)
        return 0;
      dispositionType = input.Substring(startIndex, tokenLength);
      return tokenLength;
    }

    private static void CheckDispositionTypeFormat(string dispositionType, string parameterName)
    {
      if (string.IsNullOrEmpty(dispositionType))
        throw new ArgumentException(SR.net_http_argument_empty_string, parameterName);
      string dispositionType1;
      if (ContentDispositionHeaderValue.GetDispositionTypeExpressionLength(dispositionType, 0, out dispositionType1) == 0 || dispositionType1.Length != dispositionType.Length)
        throw new FormatException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, SR.net_http_headers_invalid_value, (object) dispositionType));
    }

    private DateTimeOffset? GetDate(string parameter)
    {
      NameValueHeaderValue valueHeaderValue = NameValueHeaderValue.Find(this.parameters, parameter);
      if (valueHeaderValue != null)
      {
        string input = valueHeaderValue.Value;
        if (this.IsQuoted(input))
          input = input.Substring(1, input.Length - 2);
        DateTimeOffset result;
        if (HttpRuleParser.TryStringToDate(input, out result))
          return new DateTimeOffset?(result);
      }
      return new DateTimeOffset?();
    }

    private void SetDate(string parameter, DateTimeOffset? date)
    {
      NameValueHeaderValue valueHeaderValue = NameValueHeaderValue.Find(this.parameters, parameter);
      if (!date.HasValue)
      {
        if (valueHeaderValue == null)
          return;
        this.parameters.Remove(valueHeaderValue);
      }
      else
      {
        string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"{0}\"", (object) HttpRuleParser.DateToString(date.Value));
        if (valueHeaderValue != null)
          valueHeaderValue.Value = str;
        else
          this.Parameters.Add(new NameValueHeaderValue(parameter, str));
      }
    }

    private string GetName(string parameter)
    {
      NameValueHeaderValue valueHeaderValue = NameValueHeaderValue.Find(this.parameters, parameter);
      if (valueHeaderValue == null)
        return (string) null;
      string output1;
      string output2;
      return parameter.EndsWith("*", StringComparison.Ordinal) ? (this.TryDecode5987(valueHeaderValue.Value, out output1) ? output1 : (string) null) : (this.TryDecodeMime(valueHeaderValue.Value, out output2) ? output2 : valueHeaderValue.Value);
    }

    private void SetName(string parameter, string value)
    {
      NameValueHeaderValue valueHeaderValue = NameValueHeaderValue.Find(this.parameters, parameter);
      if (string.IsNullOrEmpty(value))
      {
        if (valueHeaderValue == null)
          return;
        this.parameters.Remove(valueHeaderValue);
      }
      else
      {
        string empty = string.Empty;
        string str = !parameter.EndsWith("*", StringComparison.Ordinal) ? this.EncodeAndQuoteMime(value) : this.Encode5987(value);
        if (valueHeaderValue != null)
          valueHeaderValue.Value = str;
        else
          this.Parameters.Add(new NameValueHeaderValue(parameter, str));
      }
    }

    private string EncodeAndQuoteMime(string input)
    {
      string input1 = input;
      bool flag = false;
      if (this.IsQuoted(input1))
      {
        input1 = input1.Substring(1, input1.Length - 2);
        flag = true;
      }
      if (input1.IndexOf("\"", 0, StringComparison.Ordinal) >= 0)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, SR.net_http_headers_invalid_value, (object) input));
      if (this.RequiresEncoding(input1))
      {
        flag = true;
        input1 = this.EncodeMime(input1);
      }
      else if (!flag && HttpRuleParser.GetTokenLength(input1, 0) != input1.Length)
        flag = true;
      if (flag)
        input1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"{0}\"", (object) input1);
      return input1;
    }

    private bool IsQuoted(string value)
    {
      Contract.Assert(value != null);
      return value.Length > 1 && value.StartsWith("\"", StringComparison.Ordinal) && value.EndsWith("\"", StringComparison.Ordinal);
    }

    private bool RequiresEncoding(string input)
    {
      Contract.Assert(input != null);
      foreach (char ch in input)
      {
        if (ch > '\u007F')
          return true;
      }
      return false;
    }

    private string EncodeMime(string input)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "=?utf-8?B?{0}?=", (object) Convert.ToBase64String(Encoding.UTF8.GetBytes(input)));
    }

    private bool TryDecodeMime(string input, out string output)
    {
      Contract.Assert(input != null);
      output = (string) null;
      string str = input;
      if (!this.IsQuoted(str) || str.Length < 10)
        return false;
      string[] strArray = str.Split('?');
      if (strArray.Length == 5 && !(strArray[0] != "\"=") && !(strArray[4] != "=\""))
      {
        if (!(strArray[2].ToLowerInvariant() != "b"))
        {
          try
          {
            Encoding encoding = Encoding.GetEncoding(strArray[1]);
            byte[] bytes = Convert.FromBase64String(strArray[3]);
            output = encoding.GetString(bytes);
            return true;
          }
          catch (ArgumentException ex)
          {
          }
          catch (FormatException ex)
          {
          }
          return false;
        }
      }
      return false;
    }

    private string Encode5987(string input)
    {
      StringBuilder stringBuilder = new StringBuilder("utf-8''");
      foreach (char character1 in input)
      {
        if (character1 > '\u007F')
        {
          foreach (byte character2 in Encoding.UTF8.GetBytes(character1.ToString()))
            stringBuilder.Append(UriHelper.HexEscape((char) character2));
        }
        else if (!HttpRuleParser.IsTokenChar(character1) || character1 == '*' || character1 == '\'' || character1 == '%')
          stringBuilder.Append(UriHelper.HexEscape(character1));
        else
          stringBuilder.Append(character1);
      }
      return stringBuilder.ToString();
    }

    private bool TryDecode5987(string input, out string output)
    {
      output = (string) null;
      string[] strArray = input.Split('\'');
      if (strArray.Length != 3)
        return false;
      StringBuilder stringBuilder = new StringBuilder();
      try
      {
        Encoding encoding = Encoding.GetEncoding(strArray[0]);
        string pattern = strArray[2];
        byte[] bytes = new byte[pattern.Length];
        int count = 0;
        for (int index = 0; index < pattern.Length; ++index)
        {
          if (UriHelper.IsHexEncoding(pattern, index))
          {
            bytes[count++] = (byte) UriHelper.HexUnescape(pattern, ref index);
            --index;
          }
          else
          {
            if (count > 0)
            {
              stringBuilder.Append(encoding.GetString(bytes, 0, count));
              count = 0;
            }
            stringBuilder.Append(pattern[index]);
          }
        }
        if (count > 0)
          stringBuilder.Append(encoding.GetString(bytes, 0, count));
      }
      catch (ArgumentException ex)
      {
        return false;
      }
      output = stringBuilder.ToString();
      return true;
    }
  }
}
