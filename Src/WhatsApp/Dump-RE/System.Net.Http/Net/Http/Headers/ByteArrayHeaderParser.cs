// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.ByteArrayHeaderParser
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Diagnostics.Contracts;
using System.Globalization;

#nullable disable
namespace System.Net.Http.Headers
{
  internal class ByteArrayHeaderParser : HttpHeaderParser
  {
    internal static readonly ByteArrayHeaderParser Parser = new ByteArrayHeaderParser();

    private ByteArrayHeaderParser()
      : base(false)
    {
    }

    public override string ToString(object value)
    {
      Contract.Assert(value is byte[]);
      return Convert.ToBase64String((byte[]) value);
    }

    public override bool TryParseValue(
      string value,
      object storeValue,
      ref int index,
      out object parsedValue)
    {
      parsedValue = (object) null;
      if (string.IsNullOrEmpty(value) || index == value.Length)
        return false;
      string s = value;
      if (index > 0)
        s = value.Substring(index);
      try
      {
        parsedValue = (object) Convert.FromBase64String(s);
        index = value.Length;
        return true;
      }
      catch (FormatException ex)
      {
        if (Logging.On)
          Logging.PrintError(Logging.Http, string.Format((IFormatProvider) CultureInfo.InvariantCulture, SR.net_http_parser_invalid_base64_string, (object) s, (object) ex.Message));
      }
      return false;
    }
  }
}
