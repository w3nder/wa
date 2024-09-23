// Decompiled with JetBrains decompiler
// Type: WhatsApp.LinkDetector
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using WhatsApp.RegularExpressions;
using WhatsApp.WaCollections;

#nullable disable
namespace WhatsApp
{
  public class LinkDetector
  {
    public const string MailTo = "mailto:";
    public static readonly HashSet<char> delimeterSet = new HashSet<char>()
    {
      '*',
      ' ',
      '_',
      '~'
    };
    private static readonly int maxLinkifiedTextLength = AppState.IsLowMemoryDevice ? 2048 : 4096;
    private static readonly string[] formattingChars = new string[4]
    {
      "```",
      "*",
      "~",
      "_"
    };
    private static RegexOptions RegexOptions = RegexOptions.CultureInvariant | RegexOptions.IgnoreCase;
    private static Regex Ipv6Regex = new Regex("([0-9a-fA-F]{0,4}:){2,7}(:|[0-9a-fA-F]{1,4})", LinkDetector.RegexOptions);
    private static Regex Ipv4Regex = new Regex("\\b\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\b", LinkDetector.RegexOptions);
    public static Regex UrlRegexes = new Regex(LinkDetector.UrlRegexesStrings().ToRegexString(), LinkDetector.RegexOptions);
    private const string UriCharRegexString = "(?:[\\w\\d\\-\\._~:/\\?#\\[\\]@!$&'\\(\\)\\*\\+,;=]|%[0-9A-Fa-f][0-9A-Fa-f])";
    private const string BoldRegexString = "(?:^|[*_~\\s]|\\B)\\*((?:[^\n])+?)\\*(?=_|\\W|$)";
    private const string ItalicRegexString = "(?:^|[*_~\\s]|\\B)\\_((?:[^\n])+?)\\_(?=_|\\W|$)";
    private const string StrikthroughRegexString = "(?:^|[*_~\\s]|\\B)\\~((?:[^\n])+?)\\~(?=_|\\W|$)";
    private const string CodeRegexString = "```([\\s\\S]+?)```";
    public static Regex BoldRegex = new Regex("(?:^|[*_~\\s]|\\B)\\*((?:[^\n])+?)\\*(?=_|\\W|$)", LinkDetector.RegexOptions);
    public static Regex ItalicRegex = new Regex("(?:^|[*_~\\s]|\\B)\\_((?:[^\n])+?)\\_(?=_|\\W|$)", LinkDetector.RegexOptions);
    public static Regex StrikthroughRegex = new Regex("(?:^|[*_~\\s]|\\B)\\~((?:[^\n])+?)\\~(?=_|\\W|$)", LinkDetector.RegexOptions);
    public static Regex CodeRegex = new Regex("```([\\s\\S]+?)```", LinkDetector.RegexOptions);
    public static Regex EmailRegex = new Regex("^" + LinkDetector.EmailRegexString + "$", LinkDetector.RegexOptions);
    public static Regex ProtocolRegex = new Regex("^[a-z]+:", LinkDetector.RegexOptions);

    public static LinkDetector.Result Convert(LinkDetector.Result match, int type)
    {
      return new LinkDetector.Result(match.Index, match.Length, type, (Func<string>) (() => match.Value));
    }

    public static LinkDetector.Result Convert(
      LinkDetector.Result match,
      int type,
      string originalString)
    {
      return new LinkDetector.Result(match.Index, match.Length, type, originalString);
    }

    public static LinkDetector.Result Convert(Match match, int type)
    {
      return new LinkDetector.Result(match.Index, match.Length, type, (Func<string>) (() => match.Value));
    }

    public static LinkDetector.Result Convert(Match match, int type, string originalString)
    {
      return new LinkDetector.Result(match.Index, match.Length, type, originalString);
    }

    public static bool ValidUriCharacter(char ch)
    {
      return char.IsLetterOrDigit(ch) || "-._~:/?#[]@!$&'()*+,;=".IndexOf(ch) >= 0;
    }

    public static Emoji.EmojiChar InferEmoji(LinkDetector.Result match)
    {
      return Emoji.GetEmojiChar(match.Value);
    }

    public static Emoji.EmojiChar InferEmoji(Match match) => Emoji.GetEmojiChar(match.Value);

    public static Emoji.EmojiChar InferEmoji(string codepoints) => Emoji.GetEmojiChar(codepoints);

    public static string InferUriScheme(string uriString)
    {
      if (!LinkDetector.ProtocolRegex.IsMatch(uriString))
        uriString = !LinkDetector.EmailRegex.IsMatch(uriString) ? "https://" + uriString : "mailto:" + uriString;
      return uriString;
    }

    public static bool ValidateOutsideLinkBounds(string s, int start, int end)
    {
      if (start != 0 && LinkDetector.ValidUriCharacter(s[start - 1]) && !char.IsPunctuation(s[start - 1]) && s[start - 1] != '~')
        return false;
      return end >= s.Length || !LinkDetector.ValidUriCharacter(s[end]) || char.IsPunctuation(s[end]) || s[end] == '~';
    }

    private static bool ValidUrl(string s, LinkDetector.Result match)
    {
      if (LinkDetector.InferEmoji(match) != null)
        return true;
      if (!LinkDetector.ValidateOutsideLinkBounds(s, match.Index, match.Index + match.Length))
        return false;
      string str = LinkDetector.InferUriScheme(match.Value);
      if (str.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase))
        return true;
      Uri result = (Uri) null;
      return Uri.TryCreate(str, UriKind.Absolute, out result) && !LinkDetector.Ipv6Regex.IsMatch(str) && !LinkDetector.Ipv4Regex.IsMatch(str);
    }

    public static IEnumerable<LinkDetector.Result> GetMatches(
      string s,
      WaRichText.DetectionArgs args = null)
    {
      List<LinkDetector.Result> source1 = new List<LinkDetector.Result>();
      if (s == null)
        return (IEnumerable<LinkDetector.Result>) source1;
      if (args == null)
        args = new WaRichText.DetectionArgs();
      PerformanceTimer.Start(PerformanceTimer.Mode.DebugAndBeta);
      int[] numArray = new int[s.Length];
      List<int> source2 = new List<int>();
      char[] chArray = (char[]) null;
      string str = "start";
      try
      {
        if (args.ShouldDetect(WaRichText.Formats.Code))
        {
          str = "code";
          chArray = chArray ?? s.ToCharArray();
          foreach (Match match in (IEnumerable<Match>) LinkDetector.GetRegexForFormat(WaRichText.Formats.Code).Matches(s))
          {
            if (match.Value.StartsWith("```") && match.Length > 6)
            {
              source2.Add(match.Index);
              source2.Add(match.Index + 1);
              source2.Add(match.Index + 2);
              source2.Add(match.Index + match.Length - 1);
              source2.Add(match.Index + match.Length - 2);
              source2.Add(match.Index + match.Length - 3);
              match.Length -= 3;
              match.Index += 3;
            }
            for (int index = match.Index; index < match.Index + match.Length; ++index)
            {
              numArray[index] = 16 | numArray[index];
              chArray[index] = ' ';
            }
          }
        }
        if (args.ShouldDetect(WaRichText.Formats.Link))
        {
          str = "link";
          chArray = chArray ?? s.ToCharArray();
          string input = s.Length > LinkDetector.maxLinkifiedTextLength ? s.Substring(0, LinkDetector.maxLinkifiedTextLength) : s;
          foreach (Match match in (IEnumerable<Match>) LinkDetector.GetRegexForFormat(WaRichText.Formats.Link).Matches(input))
          {
            for (int index = match.Index; index < match.Index + match.Length; ++index)
            {
              if (!LinkDetector.delimeterSet.Contains(chArray[index]))
              {
                match.Length -= index - match.Index;
                match.Index = index;
                break;
              }
            }
            if (LinkDetector.ValidUrl(s, LinkDetector.Convert(match, 1, s)))
            {
              for (int index = match.Index; index < match.Index + match.Length; ++index)
              {
                numArray[index] = 1 | numArray[index];
                chArray[index] = ' ';
              }
            }
          }
        }
        string input1 = chArray != null ? new string(chArray) : s;
        if (args.ShouldDetect(WaRichText.Formats.Bold))
        {
          str = "bold";
          foreach (Match match in (IEnumerable<Match>) LinkDetector.GetRegexForFormat(WaRichText.Formats.Bold).Matches(input1))
          {
            bool flag = false;
            int num = match.Index + match.Length;
            for (int index = match.Index; index < num; ++index)
            {
              numArray[index] = 4 | numArray[index];
              if (LinkDetector.delimeterSet.Contains(input1[index]) && !flag && input1[index] == '*')
              {
                source2.Add(index);
                flag = true;
              }
              if (index == num - 1)
              {
                source2.Add(index);
                match.Length = match.Length - (index - match.Index) - 1;
                match.Index = index;
              }
            }
          }
        }
        if (args.ShouldDetect(WaRichText.Formats.Italic))
        {
          str = "italic";
          foreach (Match match in (IEnumerable<Match>) LinkDetector.GetRegexForFormat(WaRichText.Formats.Italic).Matches(input1))
          {
            bool flag = false;
            int num = match.Index + match.Length;
            for (int index = match.Index; index < num; ++index)
            {
              numArray[index] = 8 | numArray[index];
              if (LinkDetector.delimeterSet.Contains(input1[index]) && !flag && input1[index] == '_')
              {
                source2.Add(index);
                flag = true;
              }
              if (index == num - 1)
              {
                source2.Add(index);
                match.Length = match.Length - (index - match.Index) - 1;
                match.Index = index;
              }
            }
          }
        }
        if (args.ShouldDetect(WaRichText.Formats.Strikethrough))
        {
          str = "strikethrough";
          foreach (Match match in (IEnumerable<Match>) LinkDetector.GetRegexForFormat(WaRichText.Formats.Strikethrough).Matches(input1))
          {
            bool flag = false;
            int num = match.Index + match.Length;
            for (int index = match.Index; index < num; ++index)
            {
              if (input1[index] != '\n')
                numArray[index] = 32 | numArray[index];
              if (LinkDetector.delimeterSet.Contains(input1[index]) && !flag && input1[index] == '~')
              {
                source2.Add(index);
                flag = true;
              }
              if (index == num - 1)
              {
                source2.Add(index);
                match.Length = match.Length - (index - match.Index) - 1;
                match.Index = index;
              }
            }
          }
        }
        if (args.ShouldDetect(WaRichText.Formats.Emoji))
        {
          str = "emoji";
          foreach (LinkDetector.Result emojiMatch in LinkDetector.GetEmojiMatches(s))
          {
            int index1 = emojiMatch.Index;
            int length = emojiMatch.Length;
            for (int index2 = emojiMatch.Index; index2 < emojiMatch.Index + emojiMatch.Length; ++index2)
              source2.Add(index2);
            source1.Add(LinkDetector.Convert(emojiMatch, 2 | numArray[emojiMatch.Index], s));
          }
        }
        str = "remove delimeter characters";
        int num1 = source2.Count<int>();
        for (int index = 0; index < num1; ++index)
          numArray[source2[index]] = -1;
      }
      catch (Exception ex)
      {
        string context = "GetMatches failed on Regex: " + str;
        Log.LogException(ex, context);
        throw;
      }
      try
      {
        int type = 0;
        int startIndex = 0;
        for (int index = 0; index <= s.Length; ++index)
        {
          if (index == s.Length)
          {
            if (type != -1)
            {
              Match match = new Match()
              {
                Index = startIndex,
                Length = index - startIndex,
                Value = s.Substring(startIndex, index - startIndex),
                Success = true
              };
              source1.Add(LinkDetector.Convert(match, type, s));
            }
          }
          else if (numArray[index] != type)
          {
            if (index != 0 && type != -1)
            {
              Match match = new Match()
              {
                Index = startIndex,
                Length = index - startIndex,
                Value = s.Substring(startIndex, index - startIndex),
                Success = true
              };
              source1.Add(LinkDetector.Convert(match, type, s));
            }
            type = numArray[index];
            startIndex = index;
          }
        }
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "GetMatches failed on assembing segments");
        throw;
      }
      return (IEnumerable<LinkDetector.Result>) source1.OrderBy<LinkDetector.Result, int>((Func<LinkDetector.Result, int>) (r => r.Index)).ToArray<LinkDetector.Result>();
    }

    public static bool IsSendableText(string testString)
    {
      return LinkDetector.IsSendAbleTextWIthFormat(testString, WaRichText.Formats.TextFormattings);
    }

    private static bool IsSendAbleTextWIthFormat(
      string testString,
      WaRichText.Formats uncheckedformats)
    {
      if (string.IsNullOrEmpty(testString))
        return false;
      bool flag1 = false;
      foreach (string formattingChar in LinkDetector.formattingChars)
      {
        if (testString.Length > formattingChar.Length && testString.StartsWith(formattingChar))
          flag1 = true;
      }
      if (!flag1)
        return true;
      IEnumerable<LinkDetector.Result> matches = LinkDetector.GetMatches(testString, new WaRichText.DetectionArgs(uncheckedformats));
      bool flag2 = false;
      foreach (LinkDetector.Result result in (IEnumerable<LinkDetector.Result>) ((object) matches ?? (object) new LinkDetector.Result[0]))
      {
        string testString1 = result.Value != null ? result.Value.Trim() : "";
        if (((WaRichText.Formats) result.type & uncheckedformats) != WaRichText.Formats.None && (result.type & 16) == 0)
        {
          int uncheckedformats1 = (int) (uncheckedformats & (WaRichText.Formats) ~result.type);
          flag2 = uncheckedformats1 == 0 ? !string.IsNullOrEmpty(testString1) : LinkDetector.IsSendAbleTextWIthFormat(testString1, (WaRichText.Formats) uncheckedformats1);
        }
        else
          flag2 = !string.IsNullOrEmpty(testString1);
        if (flag2)
          break;
      }
      return flag2;
    }

    public static IEnumerable<LinkDetector.Result> GetEmojiMatches(string s)
    {
      List<LinkDetector.Result> emojiMatches = new List<LinkDetector.Result>();
      foreach (LinkDetector.Result allEmojiMatch in Emoji.GetAllEmojiMatches(s, 0, s.Length))
      {
        if (LinkDetector.InferEmoji(allEmojiMatch) != null)
          emojiMatches.Add(LinkDetector.Convert(allEmojiMatch, 2));
      }
      return (IEnumerable<LinkDetector.Result>) emojiMatches;
    }

    private static string TldRegex
    {
      get
      {
        return CountryInfo.Instance.GetSortedCountryInfos().Select<CountryInfoItem, string>((Func<CountryInfoItem, string>) (country => country.IsoCode)).Concat<string>((IEnumerable<string>) new string[7]
        {
          "com",
          "net",
          "org",
          "edu",
          "biz",
          "info",
          "uk"
        }).OrderByDescending<string, int>((Func<string, int>) (s => s.Length)).ToRegexString();
      }
    }

    private static IEnumerable<string> UrlRegexesStrings()
    {
      return (IEnumerable<string>) new string[4]
      {
        LinkPreviewUtils.WEB_URL_STRING,
        "mailto:(?:[\\w\\d\\-\\._~:/\\?#\\[\\]@!$&'\\(\\)\\*\\+,;=]|%[0-9A-Fa-f][0-9A-Fa-f])+",
        LinkDetector.EmailRegexString,
        "(?:[\\w\\d\\-\\._~:/\\?#\\[\\]@!$&'\\(\\)\\*\\+,;=]|%[0-9A-Fa-f][0-9A-Fa-f]){1,255}\\." + LinkDetector.TldRegex + "(?:\\/(?:[\\w\\d\\-\\._~:/\\?#\\[\\]@!$&'\\(\\)\\*\\+,;=]|%[0-9A-Fa-f][0-9A-Fa-f]){0,1024}|)"
      };
    }

    public static Regex GetRegexForFormat(WaRichText.Formats f)
    {
      switch (f)
      {
        case WaRichText.Formats.Link:
          return LinkDetector.UrlRegexes;
        case WaRichText.Formats.Bold:
          return LinkDetector.BoldRegex;
        case WaRichText.Formats.Italic:
          return LinkDetector.ItalicRegex;
        case WaRichText.Formats.Code:
          return LinkDetector.CodeRegex;
        case WaRichText.Formats.Strikethrough:
          return LinkDetector.StrikthroughRegex;
        default:
          throw new InvalidOperationException();
      }
    }

    public static string EmailRegexString
    {
      get
      {
        return string.Format("{0}{2}@{0}{3}\\.{1}", (object) "(?:[\\w\\d\\-\\._~:/\\?#\\[\\]@!$&'\\(\\)\\*\\+,;=]|%[0-9A-Fa-f][0-9A-Fa-f])".Replace("@", ""), (object) LinkDetector.TldRegex, (object) "{1,64}", (object) "{1,255}");
      }
    }

    public class Result
    {
      public const byte Version = 11;
      public int type;
      private Func<string> createFunc;
      private string cachedValue;

      public int Index { get; set; }

      public int Length { get; set; }

      public string OriginalStr { get; set; }

      public string Value
      {
        get
        {
          if (this.cachedValue == null)
            this.cachedValue = this.PopulateValue();
          return this.cachedValue;
        }
      }

      public string AuxiliaryInfo { get; set; }

      public Color? ForegroundColor { get; set; }

      public Action ClickAction { get; set; }

      public bool LinkUnderscore { get; set; } = true;

      public Result(int index, int length, int type, string originalString, string auxiliaryInfo = null)
      {
        this.Index = index;
        this.Length = length;
        this.type = type;
        this.OriginalStr = originalString;
        this.AuxiliaryInfo = auxiliaryInfo;
      }

      public Result(int index, int length, int type, Func<string> create, string auxiliaryInfo = null)
      {
        this.createFunc = create;
        this.Index = index;
        this.Length = length;
        this.type = type;
        this.AuxiliaryInfo = auxiliaryInfo;
      }

      public Result(LinkDetector.Result other)
      {
        this.createFunc = other.createFunc;
        this.OriginalStr = other.OriginalStr;
        this.Index = other.Index;
        this.Length = other.Length;
        this.type = other.type;
        this.AuxiliaryInfo = other.AuxiliaryInfo;
        this.ClickAction = other.ClickAction;
        this.ForegroundColor = other.ForegroundColor;
        this.LinkUnderscore = other.LinkUnderscore;
      }

      public WaRichText.Chunk ToRichTextChunk()
      {
        WaRichText.Chunk richTextChunk = this.OriginalStr == null ? (this.createFunc == null ? new WaRichText.Chunk(this.Index, this.Length, (WaRichText.Formats) this.type, this.AuxiliaryInfo) : new WaRichText.Chunk(this.Index, this.Length, (WaRichText.Formats) this.type, this.AuxiliaryInfo, this.createFunc)) : new WaRichText.Chunk(this.Index, this.Length, (WaRichText.Formats) this.type, this.AuxiliaryInfo, this.OriginalStr);
        richTextChunk.ClickAction = this.ClickAction;
        richTextChunk.ForegroundColor = this.ForegroundColor;
        richTextChunk.LinkUnderscore = this.LinkUnderscore;
        return richTextChunk;
      }

      private string PopulateValue()
      {
        string str = (string) null;
        if (this.createFunc == null)
        {
          if (this.OriginalStr != null && this.Index < this.OriginalStr.Length)
            str = this.OriginalStr.Substring(this.Index, Math.Min(this.Length, this.OriginalStr.Length - this.Index));
        }
        else
          str = this.createFunc();
        return str;
      }

      public static byte[] Serialize(IEnumerable<LinkDetector.Result> results)
      {
        if (results == null)
          return (byte[]) null;
        BinaryData binaryData = new BinaryData();
        binaryData.AppendByte((byte) 11);
        foreach (LinkDetector.Result result in results)
        {
          binaryData.AppendInt32(result.type);
          binaryData.AppendInt32(result.Index);
          binaryData.AppendInt32(result.Length);
        }
        return binaryData.Get();
      }

      public static LinkDetector.Result[] Deserialize(string s, byte[] buffer)
      {
        if (buffer == null || buffer.Length < 1 || buffer[0] != (byte) 11)
          return (LinkDetector.Result[]) null;
        List<LinkDetector.Result> source = new List<LinkDetector.Result>();
        int offset1 = 1;
        int num = buffer.Length - offset1;
        while (num >= 12)
        {
          int type = BinaryData.ReadInt32(buffer, offset1);
          int offset2 = offset1 + 4;
          int index = BinaryData.ReadInt32(buffer, offset2);
          int offset3 = offset2 + 4;
          int length = BinaryData.ReadInt32(buffer, offset3);
          offset1 = offset3 + 4;
          num -= 12;
          if (s != null && index + length <= s.Length)
            source.Add(new LinkDetector.Result(index, length, type, s));
          else
            break;
        }
        return source.Where<LinkDetector.Result>((Func<LinkDetector.Result, bool>) (r => r.Length != 0)).ToArray<LinkDetector.Result>();
      }
    }
  }
}
