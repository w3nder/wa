// Decompiled with JetBrains decompiler
// Type: WhatsApp.RegularExpressions.Regex
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Text;
using WhatsApp.RegularExpressions.Impl;

#nullable disable
namespace WhatsApp.RegularExpressions
{
  public class Regex : NativeRegex
  {
    public Regex(string expr, RegexOptions options = RegexOptions.Default)
      : base(expr, options)
    {
    }

    public WhatsApp.RegularExpressions.Match Match(string input, int offset = 0)
    {
      int length = input.Length - offset;
      if (length < 0)
        throw new IndexOutOfRangeException("Index past end of buffer");
      return this.Match(input, offset, length);
    }

    public IList<WhatsApp.RegularExpressions.Match> Matches(string input)
    {
      List<WhatsApp.RegularExpressions.Match> matchList = new List<WhatsApp.RegularExpressions.Match>();
      if (input == null)
        return (IList<WhatsApp.RegularExpressions.Match>) matchList;
      int offset = 0;
      WhatsApp.RegularExpressions.Match match;
      while (offset < input.Length && (match = this.Match(input, offset)) != null && match.Success)
      {
        matchList.Add(match);
        offset = match.Index + match.Length;
        if (match.Length == 0)
          ++offset;
      }
      return (IList<WhatsApp.RegularExpressions.Match>) matchList;
    }

    public string Replace(string input, string replacement)
    {
      Func<WhatsApp.RegularExpressions.Match, string> replacementFunc = (Func<WhatsApp.RegularExpressions.Match, string>) (m => replacement);
      List<Func<WhatsApp.RegularExpressions.Match, string>> chunks = new List<Func<WhatsApp.RegularExpressions.Match, string>>();
      int length = 0;
      for (int index = 0; index < replacement.Length; ++index)
      {
        if (replacement[index] == '$')
        {
          if (index + 1 >= replacement.Length)
            throw new IndexOutOfRangeException("invalid substitution");
          if (length != index)
          {
            string substr = replacement.Substring(length, index - length);
            chunks.Add((Func<WhatsApp.RegularExpressions.Match, string>) (m => substr));
          }
          ++index;
          switch (replacement[index])
          {
            case '$':
              chunks.Add((Func<WhatsApp.RegularExpressions.Match, string>) (m => "$"));
              break;
            case '&':
              chunks.Add((Func<WhatsApp.RegularExpressions.Match, string>) (m => m.Value));
              break;
            case '\'':
              chunks.Add((Func<WhatsApp.RegularExpressions.Match, string>) (m => input.Substring(m.Index + m.Length)));
              break;
            case '0':
            case '1':
            case '2':
            case '3':
            case '4':
            case '5':
            case '6':
            case '7':
            case '8':
            case '9':
              int startIndex = index;
              while (index < replacement.Length && replacement[index] >= '0' && replacement[index] <= '9')
                ++index;
              int which = int.Parse(replacement.Substring(startIndex, index - startIndex));
              chunks.Add((Func<WhatsApp.RegularExpressions.Match, string>) (m => m.Groups[which].Value));
              break;
            case '_':
              chunks.Add((Func<WhatsApp.RegularExpressions.Match, string>) (m => input));
              break;
            case '`':
              chunks.Add((Func<WhatsApp.RegularExpressions.Match, string>) (m => input.Substring(0, m.Index)));
              break;
            default:
              throw new InvalidOperationException("Unknown or unsupported replacement $" + replacement[index].ToString());
          }
          length = index;
        }
      }
      if (chunks.Count != 0)
      {
        if (length != replacement.Length)
          chunks.Add((Func<WhatsApp.RegularExpressions.Match, string>) (m => replacement.Substring(length)));
        replacementFunc = (Func<WhatsApp.RegularExpressions.Match, string>) (m =>
        {
          StringBuilder stringBuilder = new StringBuilder();
          foreach (Func<WhatsApp.RegularExpressions.Match, string> func in chunks)
            stringBuilder.Append(func(m));
          return stringBuilder.ToString();
        });
      }
      return this.Replace(input, replacementFunc);
    }

    public string Replace(string input, Func<WhatsApp.RegularExpressions.Match, string> replacementFunc)
    {
      StringBuilder stringBuilder = (StringBuilder) null;
      int startIndex = 0;
      char[] chArray = (char[]) null;
      foreach (WhatsApp.RegularExpressions.Match match in (IEnumerable<WhatsApp.RegularExpressions.Match>) this.Matches(input))
      {
        if (match.Index >= startIndex)
        {
          if (stringBuilder == null)
          {
            stringBuilder = new StringBuilder();
            chArray = input.ToCharArray();
          }
          stringBuilder.Append(chArray, startIndex, match.Index - startIndex);
          string str = replacementFunc(match);
          if (str != null)
            stringBuilder.Append(str);
        }
        startIndex = match.Index + match.Length;
      }
      if (stringBuilder == null)
        return input;
      if (startIndex < input.Length)
        stringBuilder.Append(chArray, startIndex, chArray.Length - startIndex);
      return stringBuilder.ToString();
    }

    public bool IsMatch(string input)
    {
      WhatsApp.RegularExpressions.Match match = this.Match(input);
      return match != null && match.Success;
    }

    public static IList<WhatsApp.RegularExpressions.Match> Matches(
      string input,
      string expr,
      RegexOptions options = RegexOptions.Default)
    {
      return new Regex(expr, options).Matches(input);
    }

    public static WhatsApp.RegularExpressions.Match Match(
      string input,
      string pattern,
      RegexOptions options = RegexOptions.Default)
    {
      return new Regex(pattern, options).Match(input);
    }
  }
}
