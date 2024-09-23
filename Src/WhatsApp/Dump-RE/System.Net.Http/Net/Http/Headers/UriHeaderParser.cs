// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.UriHeaderParser
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Diagnostics.Contracts;
using System.Text;

#nullable disable
namespace System.Net.Http.Headers
{
  internal class UriHeaderParser : HttpHeaderParser
  {
    private UriKind uriKind;
    internal static readonly UriHeaderParser RelativeOrAbsoluteUriParser = new UriHeaderParser(UriKind.RelativeOrAbsolute);

    private UriHeaderParser(UriKind uriKind)
      : base(false)
    {
      this.uriKind = uriKind;
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
      string str = value;
      if (index > 0)
        str = value.Substring(index);
      Uri result;
      if (!Uri.TryCreate(str, this.uriKind, out result) && !Uri.TryCreate(UriHeaderParser.DecodeUtf8FromString(str), this.uriKind, out result))
        return false;
      index = value.Length;
      parsedValue = (object) result;
      return true;
    }

    internal static string DecodeUtf8FromString(string input)
    {
      if (string.IsNullOrWhiteSpace(input))
        return input;
      bool flag = false;
      for (int index = 0; index < input.Length; ++index)
      {
        if (input[index] > 'ÿ')
          return input;
        if (input[index] > '\u007F')
        {
          flag = true;
          break;
        }
      }
      if (flag)
      {
        byte[] bytes = new byte[input.Length];
        for (int index = 0; index < input.Length; ++index)
        {
          if (input[index] > 'ÿ')
            return input;
          bytes[index] = (byte) input[index];
        }
        try
        {
          return new UTF8Encoding(true, true).GetString(bytes);
        }
        catch (ArgumentException ex)
        {
        }
      }
      return input;
    }

    public override string ToString(object value)
    {
      Contract.Assert(value is Uri);
      Uri uri = (Uri) value;
      return uri.IsAbsoluteUri ? uri.AbsoluteUri : uri.GetComponents(UriComponents.SerializationInfoString, UriFormat.UriEscaped);
    }
  }
}
