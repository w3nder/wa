// Decompiled with JetBrains decompiler
// Type: ZXing.Client.Result.VINResultParser
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Text.RegularExpressions;

#nullable disable
namespace ZXing.Client.Result
{
  /// <summary>
  /// Detects a result that is likely a vehicle identification number.
  /// @author Sean Owen
  /// </summary>
  public class VINResultParser : ResultParser
  {
    private static readonly Regex IOQ = new Regex("[IOQ]", RegexOptions.Compiled);
    private static readonly Regex AZ09 = new Regex("\\A(?:[A-Z0-9]{17})\\z", RegexOptions.Compiled);

    public override ParsedResult parse(ZXing.Result result)
    {
      try
      {
        if (result.BarcodeFormat != BarcodeFormat.CODE_39)
          return (ParsedResult) null;
        string text = result.Text;
        string str1 = VINResultParser.IOQ.Replace(text, "").Trim();
        if (!VINResultParser.AZ09.Match(str1).Success || !VINResultParser.checkChecksum(str1))
          return (ParsedResult) null;
        string str2 = str1.Substring(0, 3);
        return (ParsedResult) new VINParsedResult(str1, str2, str1.Substring(3, 6), str1.Substring(9, 8), VINResultParser.countryCode(str2), str1.Substring(3, 5), VINResultParser.modelYear(str1[9]), str1[10], str1.Substring(11));
      }
      catch
      {
        return (ParsedResult) null;
      }
    }

    private static bool checkChecksum(string vin)
    {
      int num = 0;
      for (int index = 0; index < vin.Length; ++index)
        num += VINResultParser.vinPositionWeight(index + 1) * VINResultParser.vinCharValue(vin[index]);
      return (int) vin[8] == (int) VINResultParser.checkChar(num % 11);
    }

    private static int vinCharValue(char c)
    {
      if (c >= 'A' && c <= 'I')
        return (int) c - 65 + 1;
      if (c >= 'J' && c <= 'R')
        return (int) c - 74 + 1;
      if (c >= 'S' && c <= 'Z')
        return (int) c - 83 + 2;
      if (c >= '0' && c <= '9')
        return (int) c - 48;
      throw new ArgumentException(c.ToString());
    }

    private static int vinPositionWeight(int position)
    {
      if (position >= 1 && position <= 7)
        return 9 - position;
      if (position == 8)
        return 10;
      if (position == 9)
        return 0;
      if (position >= 10 && position <= 17)
        return 19 - position;
      throw new ArgumentException();
    }

    private static char checkChar(int remainder)
    {
      if (remainder < 10)
        return (char) (48 + remainder);
      if (remainder == 10)
        return 'X';
      throw new ArgumentException();
    }

    private static int modelYear(char c)
    {
      if (c >= 'E' && c <= 'H')
        return (int) c - 69 + 1984;
      if (c >= 'J' && c <= 'N')
        return (int) c - 74 + 1988;
      if (c == 'P')
        return 1993;
      if (c >= 'R' && c <= 'T')
        return (int) c - 82 + 1994;
      if (c >= 'V' && c <= 'Y')
        return (int) c - 86 + 1997;
      if (c >= '1' && c <= '9')
        return (int) c - 49 + 2001;
      if (c >= 'A' && c <= 'D')
        return (int) c - 65 + 2010;
      throw new ArgumentException(c.ToString());
    }

    private static string countryCode(string wmi)
    {
      char ch1 = wmi[0];
      char ch2 = wmi[1];
      switch (ch1)
      {
        case '1':
        case '4':
        case '5':
          return "US";
        case '2':
          return "CA";
        case '3':
          if (ch2 >= 'A' && ch2 <= 'W')
            return "MX";
          break;
        case '9':
          if (ch2 >= 'A' && ch2 <= 'E' || ch2 >= '3' && ch2 <= '9')
            return "BR";
          break;
        case 'J':
          if (ch2 >= 'A' && ch2 <= 'T')
            return "JP";
          break;
        case 'K':
          if (ch2 >= 'L' && ch2 <= 'R')
            return "KO";
          break;
        case 'L':
          return "CN";
        case 'M':
          if (ch2 >= 'A' && ch2 <= 'E')
            return "IN";
          break;
        case 'S':
          if (ch2 >= 'A' && ch2 <= 'M')
            return "UK";
          if (ch2 >= 'N' && ch2 <= 'T')
            return "DE";
          break;
        case 'V':
          if (ch2 >= 'F' && ch2 <= 'R')
            return "FR";
          if (ch2 >= 'S' && ch2 <= 'W')
            return "ES";
          break;
        case 'W':
          return "DE";
        case 'X':
          if (ch2 == '0' || ch2 >= '3' && ch2 <= '9')
            return "RU";
          break;
        case 'Z':
          if (ch2 >= 'A' && ch2 <= 'R')
            return "IT";
          break;
      }
      return (string) null;
    }
  }
}
