// Decompiled with JetBrains decompiler
// Type: WhatsApp.CharGroupingsLegacy
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

#nullable disable
namespace WhatsApp
{
  public class CharGroupingsLegacy : CharGroupingImpl
  {
    private Dictionary<char, char> diacriticToPrimaryChar = new Dictionary<char, char>();
    private string[] alphabet;
    private string[] digraphs;
    private List<string> groupingKeys;

    public CharGroupingsLegacy(CultureInfo cult)
    {
      CompareInfo compareInfo = cult.CompareInfo;
      string lang;
      string locale;
      cult.GetLangAndLocale(out lang, out locale);
      if (!this.TryGetAlphabet(lang, locale, out this.alphabet))
        this.alphabet = Enumerable.Range(97, 122).Select<int, string>((Func<int, string>) (c => new string((char) c, 1))).ToArray<string>();
      this.digraphs = this.GetDigraphs().ToArray<string>();
      foreach (CharGroupingsLegacy.Diacritic diacritic in CharGroupingsLegacy.GetDiacritics())
      {
        foreach (char diacriticChar in diacritic.DiacriticChars)
        {
          if (compareInfo.Compare(diacriticChar.ToString(), diacritic.PrimaryChar.ToString(), CompareOptions.IgnoreNonSpace) == 0)
            this.diacriticToPrimaryChar[diacriticChar] = diacritic.PrimaryChar;
        }
      }
    }

    public List<string> GroupingKeys
    {
      get
      {
        List<string> groupingKeys = this.groupingKeys;
        if (groupingKeys != null)
          return groupingKeys;
        return this.groupingKeys = ((IEnumerable<string>) new string[1]
        {
          "#"
        }).Concat<string>((IEnumerable<string>) this.alphabet).Concat<string>((IEnumerable<string>) new string[1]
        {
          "..."
        }).ToList<string>();
      }
    }

    public string GetGroupingKey(string str)
    {
      foreach (string digraph in this.digraphs)
      {
        if (str.StartsWith(digraph, StringComparison.CurrentCultureIgnoreCase))
          return digraph;
      }
      char ch = str.FirstOrDefault<char>();
      if (ch != char.MinValue)
        ch = char.ToLower(ch);
      if (char.IsDigit(ch) || ch == '+')
        return "#";
      char minValue = char.MinValue;
      if (this.diacriticToPrimaryChar.TryGetValue(ch, out minValue))
        return minValue.ToString();
      return !CharGroupingsLegacy.IsLatinChar(ch) ? "\uD83C\uDF10" : ch.ToString();
    }

    private static bool IsLatinChar(char ch)
    {
      return ch < '\u0080' && char.IsLetter(ch) || ch >= 'Ā' && ch <= 'ė' || ch >= 'ƀ' && ch <= 'ɏ';
    }

    private IEnumerable<string> GetDigraphs()
    {
      return ((IEnumerable<string>) this.alphabet).Where<string>((Func<string, bool>) (s => s.Length > 1));
    }

    private bool TryGetAlphabet(string lg, string lc, out string[] letters)
    {
      letters = (string[]) null;
      switch (lg)
      {
        case "es":
          letters = new string[27]
          {
            "a",
            "b",
            "c",
            "d",
            "e",
            "f",
            "g",
            "h",
            "i",
            "j",
            "k",
            "l",
            "m",
            "n",
            "ñ",
            "o",
            "p",
            "q",
            "r",
            "s",
            "t",
            "u",
            "v",
            "w",
            "x",
            "y",
            "z"
          };
          break;
        case "hu":
          letters = new string[44]
          {
            "a",
            "á",
            "b",
            "c",
            "cs",
            "d",
            "dz",
            "dzs",
            "e",
            "ë",
            "é",
            "f",
            "g",
            "gy",
            "h",
            "i",
            "j",
            "k",
            "l",
            "ly",
            "m",
            "n",
            "ny",
            "o",
            "ó",
            "ö",
            "ő",
            "p",
            "q",
            "r",
            "s",
            "sz",
            "t",
            "ty",
            "u",
            "ú",
            "ü",
            "ű",
            "v",
            "w",
            "x",
            "y",
            "z",
            "zs"
          };
          break;
        case "sr":
        case "hr":
          letters = new string[34]
          {
            "a",
            "b",
            "c",
            "č",
            "ć",
            "d",
            "dž",
            "đ",
            "e",
            "f",
            "g",
            "h",
            "i",
            "j",
            "k",
            "l",
            "lj",
            "m",
            "n",
            "nj",
            "o",
            "p",
            "q",
            "r",
            "s",
            "š",
            "t",
            "u",
            "v",
            "w",
            "x",
            "y",
            "z",
            "ž"
          };
          break;
      }
      return letters != null;
    }

    private static IEnumerable<CharGroupingsLegacy.Diacritic> GetDiacritics()
    {
      return (IEnumerable<CharGroupingsLegacy.Diacritic>) new CharGroupingsLegacy.Diacritic[14]
      {
        new CharGroupingsLegacy.Diacritic()
        {
          PrimaryChar = 'a',
          DiacriticChars = "áàäãæåâąăầẵẫắấạặậ"
        },
        new CharGroupingsLegacy.Diacritic()
        {
          PrimaryChar = 'e',
          DiacriticChars = "éèëĕėęěêềẽễếệ"
        },
        new CharGroupingsLegacy.Diacritic()
        {
          PrimaryChar = 'i',
          DiacriticChars = "íìïıįĩị"
        },
        new CharGroupingsLegacy.Diacritic()
        {
          PrimaryChar = 'o',
          DiacriticChars = "óòöõøœơôồờỏổở"
        },
        new CharGroupingsLegacy.Diacritic()
        {
          PrimaryChar = 'u',
          DiacriticChars = "úùųūưủửửựụũ"
        },
        new CharGroupingsLegacy.Diacritic()
        {
          PrimaryChar = 'n',
          DiacriticChars = "ñń"
        },
        new CharGroupingsLegacy.Diacritic()
        {
          PrimaryChar = 'c',
          DiacriticChars = "çĉčć"
        },
        new CharGroupingsLegacy.Diacritic()
        {
          PrimaryChar = 'l',
          DiacriticChars = "ł"
        },
        new CharGroupingsLegacy.Diacritic()
        {
          PrimaryChar = 's',
          DiacriticChars = "śşš"
        },
        new CharGroupingsLegacy.Diacritic()
        {
          PrimaryChar = 'z',
          DiacriticChars = "źżž"
        },
        new CharGroupingsLegacy.Diacritic()
        {
          PrimaryChar = 'g',
          DiacriticChars = "ğ"
        },
        new CharGroupingsLegacy.Diacritic()
        {
          PrimaryChar = 'd',
          DiacriticChars = "đ"
        },
        new CharGroupingsLegacy.Diacritic()
        {
          PrimaryChar = 'y',
          DiacriticChars = "ỳỷỹýỵ"
        },
        new CharGroupingsLegacy.Diacritic()
        {
          PrimaryChar = 't',
          DiacriticChars = "ť"
        }
      };
    }

    private class Diacritic
    {
      public char PrimaryChar;
      public string DiacriticChars;
    }
  }
}
