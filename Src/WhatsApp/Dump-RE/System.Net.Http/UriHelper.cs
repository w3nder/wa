// Decompiled with JetBrains decompiler
// Type: System.UriHelper
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

#nullable disable
namespace System
{
  internal static class UriHelper
  {
    private const char c_DummyChar = '\uFFFF';
    private static readonly char[] HexUpperChars = new char[16]
    {
      '0',
      '1',
      '2',
      '3',
      '4',
      '5',
      '6',
      '7',
      '8',
      '9',
      'A',
      'B',
      'C',
      'D',
      'E',
      'F'
    };

    public static Uri CombineUri(Uri baseAddress, Uri relativeUri)
    {
      Uri uri = new Uri(baseAddress, relativeUri);
      string str = relativeUri.OriginalString.Trim();
      if (str.Length > 0 && str[0] == '?' && baseAddress.AbsolutePath != uri.AbsolutePath)
        uri = new UriBuilder(baseAddress)
        {
          Query = str.Substring(1)
        }.Uri;
      return uri;
    }

    private static char EscapedAscii(char digit, char next)
    {
      if ((digit < '0' || digit > '9') && (digit < 'A' || digit > 'F') && (digit < 'a' || digit > 'f'))
        return char.MaxValue;
      int num = digit <= '9' ? (int) digit - 48 : (digit <= 'F' ? (int) digit - 65 : (int) digit - 97) + 10;
      return (next < '0' || next > '9') && (next < 'A' || next > 'F') && (next < 'a' || next > 'f') ? char.MaxValue : (char) ((num << 4) + (next <= '9' ? (int) next - 48 : (next <= 'F' ? (int) next - 65 : (int) next - 97) + 10));
    }

    private static void EscapeAsciiChar(char ch, char[] to, ref int pos)
    {
      to[pos++] = '%';
      to[pos++] = UriHelper.HexUpperChars[((int) ch & 240) >> 4];
      to[pos++] = UriHelper.HexUpperChars[(int) ch & 15];
    }

    public static bool IsHexEncoding(string pattern, int index)
    {
      return pattern.Length - index >= 3 && pattern[index] == '%' && UriHelper.EscapedAscii(pattern[index + 1], pattern[index + 2]) != char.MaxValue;
    }

    public static string HexEscape(char character)
    {
      if (character > 'ÿ')
        throw new ArgumentOutOfRangeException(nameof (character));
      char[] to = new char[3];
      int pos = 0;
      UriHelper.EscapeAsciiChar(character, to, ref pos);
      return new string(to);
    }

    public static char HexUnescape(string pattern, ref int index)
    {
      if (index < 0 || index >= pattern.Length)
        throw new ArgumentOutOfRangeException(nameof (index));
      if (pattern[index] == '%' && pattern.Length - index >= 3)
      {
        char ch = UriHelper.EscapedAscii(pattern[index + 1], pattern[index + 2]);
        if (ch != char.MaxValue)
        {
          index += 3;
          return ch;
        }
      }
      return pattern[index++];
    }

    public static bool IsDefaultPort(Uri uri)
    {
      return new Uri(uri.Scheme + "://" + uri.Host).Port == uri.Port;
    }
  }
}
