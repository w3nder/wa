// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Utilities.DateTimeParser
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using System;

#nullable disable
namespace Newtonsoft.Json.Utilities
{
  internal struct DateTimeParser
  {
    private const short MaxFractionDigits = 7;
    public int Year;
    public int Month;
    public int Day;
    public int Hour;
    public int Minute;
    public int Second;
    public int Fraction;
    public int ZoneHour;
    public int ZoneMinute;
    public ParserTimeZone Zone;
    private string _text;
    private int _length;
    private static readonly int[] Power10 = new int[7]
    {
      -1,
      10,
      100,
      1000,
      10000,
      100000,
      1000000
    };
    private static readonly int Lzyyyy = "yyyy".Length;
    private static readonly int Lzyyyy_ = "yyyy-".Length;
    private static readonly int Lzyyyy_MM = "yyyy-MM".Length;
    private static readonly int Lzyyyy_MM_ = "yyyy-MM-".Length;
    private static readonly int Lzyyyy_MM_dd = "yyyy-MM-dd".Length;
    private static readonly int Lzyyyy_MM_ddT = "yyyy-MM-ddT".Length;
    private static readonly int LzHH = "HH".Length;
    private static readonly int LzHH_ = "HH:".Length;
    private static readonly int LzHH_mm = "HH:mm".Length;
    private static readonly int LzHH_mm_ = "HH:mm:".Length;
    private static readonly int LzHH_mm_ss = "HH:mm:ss".Length;
    private static readonly int Lz_ = "-".Length;
    private static readonly int Lz_zz = "-zz".Length;

    public bool Parse(string text)
    {
      this._text = text;
      this._length = text.Length;
      return this.ParseDate(0) && this.ParseChar(DateTimeParser.Lzyyyy_MM_dd, 'T') && this.ParseTimeAndZoneAndWhitespace(DateTimeParser.Lzyyyy_MM_ddT);
    }

    private bool ParseDate(int start)
    {
      return this.Parse4Digit(start, out this.Year) && 1 <= this.Year && this.ParseChar(start + DateTimeParser.Lzyyyy, '-') && this.Parse2Digit(start + DateTimeParser.Lzyyyy_, out this.Month) && 1 <= this.Month && this.Month <= 12 && this.ParseChar(start + DateTimeParser.Lzyyyy_MM, '-') && this.Parse2Digit(start + DateTimeParser.Lzyyyy_MM_, out this.Day) && 1 <= this.Day && this.Day <= DateTime.DaysInMonth(this.Year, this.Month);
    }

    private bool ParseTimeAndZoneAndWhitespace(int start)
    {
      return this.ParseTime(ref start) && this.ParseZone(start);
    }

    private bool ParseTime(ref int start)
    {
      if (!this.Parse2Digit(start, out this.Hour) || this.Hour >= 24 || !this.ParseChar(start + DateTimeParser.LzHH, ':') || !this.Parse2Digit(start + DateTimeParser.LzHH_, out this.Minute) || this.Minute >= 60 || !this.ParseChar(start + DateTimeParser.LzHH_mm, ':') || !this.Parse2Digit(start + DateTimeParser.LzHH_mm_, out this.Second) || this.Second >= 60)
        return false;
      start += DateTimeParser.LzHH_mm_ss;
      if (this.ParseChar(start, '.'))
      {
        this.Fraction = 0;
        int num1;
        for (num1 = 0; ++start < this._length && num1 < 7; ++num1)
        {
          int num2 = (int) this._text[start] - 48;
          switch (num2)
          {
            case 0:
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
            case 7:
            case 8:
            case 9:
              this.Fraction = this.Fraction * 10 + num2;
              continue;
            default:
              goto label_7;
          }
        }
label_7:
        if (num1 < 7)
        {
          if (num1 == 0)
            return false;
          this.Fraction *= DateTimeParser.Power10[7 - num1];
        }
      }
      return true;
    }

    private bool ParseZone(int start)
    {
      if (start < this._length)
      {
        char ch = this._text[start];
        switch (ch)
        {
          case 'Z':
          case 'z':
            this.Zone = ParserTimeZone.Utc;
            ++start;
            break;
          default:
            if (start + 2 < this._length && this.Parse2Digit(start + DateTimeParser.Lz_, out this.ZoneHour) && this.ZoneHour <= 99)
            {
              switch (ch)
              {
                case '+':
                  this.Zone = ParserTimeZone.LocalEastOfUtc;
                  start += DateTimeParser.Lz_zz;
                  break;
                case '-':
                  this.Zone = ParserTimeZone.LocalWestOfUtc;
                  start += DateTimeParser.Lz_zz;
                  break;
              }
            }
            if (start < this._length)
            {
              if (this.ParseChar(start, ':'))
              {
                ++start;
                if (start + 1 < this._length && this.Parse2Digit(start, out this.ZoneMinute) && this.ZoneMinute <= 99)
                {
                  start += 2;
                  break;
                }
                break;
              }
              if (start + 1 < this._length && this.Parse2Digit(start, out this.ZoneMinute) && this.ZoneMinute <= 99)
              {
                start += 2;
                break;
              }
              break;
            }
            break;
        }
      }
      return start == this._length;
    }

    private bool Parse4Digit(int start, out int num)
    {
      if (start + 3 < this._length)
      {
        int num1 = (int) this._text[start] - 48;
        int num2 = (int) this._text[start + 1] - 48;
        int num3 = (int) this._text[start + 2] - 48;
        int num4 = (int) this._text[start + 3] - 48;
        if (0 <= num1 && num1 < 10 && 0 <= num2 && num2 < 10 && 0 <= num3 && num3 < 10 && 0 <= num4 && num4 < 10)
        {
          num = ((num1 * 10 + num2) * 10 + num3) * 10 + num4;
          return true;
        }
      }
      num = 0;
      return false;
    }

    private bool Parse2Digit(int start, out int num)
    {
      if (start + 1 < this._length)
      {
        int num1 = (int) this._text[start] - 48;
        int num2 = (int) this._text[start + 1] - 48;
        if (0 <= num1 && num1 < 10 && 0 <= num2 && num2 < 10)
        {
          num = num1 * 10 + num2;
          return true;
        }
      }
      num = 0;
      return false;
    }

    private bool ParseChar(int start, char ch)
    {
      return start < this._length && (int) this._text[start] == (int) ch;
    }
  }
}
