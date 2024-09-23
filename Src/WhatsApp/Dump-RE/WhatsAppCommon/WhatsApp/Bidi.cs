// Decompiled with JetBrains decompiler
// Type: WhatsApp.Bidi
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using WhatsApp.WaCollections;

#nullable disable
namespace WhatsApp
{
  public class Bidi
  {
    public const char LtrMarker = '\u200E';
    public const char RtlMarker = '\u200F';
    public const char PushLtrEmbed = '\u202A';
    public const char PushRtlEmbed = '\u202B';
    public const char PopEmbed = '\u202C';
    private static CharRange rtlRanges;
    private static Set<char> rtlSet;
    private static Set<char> ltrSet;

    private static CharRange RtlRanges
    {
      get
      {
        return Utils.LazyInit<CharRange>(ref Bidi.rtlRanges, (Func<CharRange>) (() => new CharRange(new ushort[12]
        {
          (ushort) 1424,
          (ushort) 1535,
          (ushort) 1536,
          (ushort) 1791,
          (ushort) 1872,
          (ushort) 1919,
          (ushort) 2208,
          (ushort) 2303,
          (ushort) 64336,
          (ushort) 65023,
          (ushort) 65136,
          (ushort) 65279
        })));
      }
    }

    private static Set<char> RtlSet
    {
      get
      {
        return Utils.LazyInit<Set<char>>(ref Bidi.rtlSet, (Func<Set<char>>) (() => new Set<char>((IEnumerable<char>) new char[2]
        {
          '\u200F',
          '\u202B'
        })));
      }
    }

    private static Set<char> LtrSet
    {
      get
      {
        return Utils.LazyInit<Set<char>>(ref Bidi.ltrSet, (Func<Set<char>>) (() => new Set<char>((IEnumerable<char>) new char[2]
        {
          '\u200E',
          '\u202A'
        })));
      }
    }

    public static Bidi.CharClass GetCharCategory(char ch)
    {
      if (Bidi.RtlRanges.Contains(ch) || Bidi.RtlSet.Contains(ch))
        return Bidi.CharClass.StrongRtl;
      return char.IsLetter(ch) || Bidi.LtrSet.Contains(ch) ? Bidi.CharClass.StrongLtr : Bidi.CharClass.None;
    }

    private static char EmbedForClass(Bidi.CharClass c)
    {
      if (c == Bidi.CharClass.StrongLtr)
        return '\u202A';
      if (c == Bidi.CharClass.StrongRtl)
        return '\u202B';
      throw new InvalidOperationException("invalid char class");
    }

    private static char MarkerForClass(Bidi.CharClass c)
    {
      if (c == Bidi.CharClass.StrongLtr)
        return '\u200E';
      if (c == Bidi.CharClass.StrongRtl)
        return '\u200F';
      throw new InvalidOperationException("invalid char class");
    }

    private static Bidi.ParsedFormatState Parse(string arg)
    {
      if (arg == null)
        return (Bidi.ParsedFormatState) null;
      Bidi.ParsedFormatState parsedFormatState = new Bidi.ParsedFormatState()
      {
        OriginalString = arg
      };
      int num1;
      int num2;
      int end;
      for (num1 = 0; num1 < arg.Length && (num2 = arg.IndexOf('\n', num1)) >= 0; num1 = end)
      {
        end = num2 + 1;
        parsedFormatState.ParseLine(num1, end);
      }
      if (num1 < arg.Length)
        parsedFormatState.ParseLine(num1, arg.Length);
      return parsedFormatState;
    }

    public static string Format(string fmt, params string[] args)
    {
      Bidi.CharClass c = new CultureInfo(AppResources.CultureString).IsRightToLeft() ? Bidi.CharClass.StrongRtl : Bidi.CharClass.StrongLtr;
      char osMarker = Bidi.MarkerForClass(c);
      List<Bidi.ParsedFormatState> parsedFormatStateList = new List<Bidi.ParsedFormatState>();
      Set<Bidi.CharClass> set = new Set<Bidi.CharClass>((IEnumerable<Bidi.CharClass>) new Bidi.CharClass[1]
      {
        c
      });
      for (int index = 0; index < args.Length; ++index)
      {
        Bidi.ParsedFormatState parsedFormatState = Bidi.Parse(args[index]);
        if (parsedFormatState != null)
        {
          if (parsedFormatState.CharClassSet.Count == 0)
          {
            parsedFormatState = (Bidi.ParsedFormatState) null;
          }
          else
          {
            foreach (Bidi.CharClass charClass in parsedFormatState.CharClassSet)
              set.Add(charClass);
          }
        }
        parsedFormatStateList.Add(parsedFormatState);
      }
      if (set.Count < 2)
        parsedFormatStateList = (List<Bidi.ParsedFormatState>) null;
      StringBuilder sb = new StringBuilder();
      foreach (Utils.FormatResult formatResult in Utils.Format(fmt, args))
      {
        Bidi.ParsedFormatState parsedFormatState = !formatResult.Index.HasValue || parsedFormatStateList == null ? (Bidi.ParsedFormatState) null : parsedFormatStateList[formatResult.Index.Value];
        if (parsedFormatState != null)
          parsedFormatState.Format(sb, osMarker);
        else
          sb.Append(formatResult.Value ?? "");
      }
      return sb.ToString();
    }

    public enum CharClass
    {
      None,
      StrongLtr,
      StrongRtl,
    }

    private class ParsedFormatState
    {
      public string OriginalString;
      public Set<Bidi.CharClass> CharClassSet = new Set<Bidi.CharClass>();
      private List<Bidi.ParsedFormatState.Range> Components = new List<Bidi.ParsedFormatState.Range>();

      public void ParseLine(int start, int end)
      {
        Bidi.CharClass key = Bidi.CharClass.None;
        for (int index = start; index < end; ++index)
        {
          key = Bidi.GetCharCategory(this.OriginalString[index]);
          if (key != Bidi.CharClass.None)
          {
            this.CharClassSet.Add(key);
            break;
          }
        }
        this.Components.Add(new Bidi.ParsedFormatState.Range()
        {
          Offset = start,
          EndOffset = end,
          Class = key
        });
      }

      public void Format(StringBuilder sb, char osMarker)
      {
        foreach (Bidi.ParsedFormatState.Range component in this.Components)
        {
          int count = component.EndOffset - component.Offset;
          if (count != 0)
          {
            int num = component.Class != 0 ? 1 : 0;
            bool flag = false;
            if (num != 0)
            {
              sb.Append(osMarker);
              sb.Append(Bidi.EmbedForClass(component.Class));
            }
            if (this.OriginalString[component.Offset + count - 1] == '\n')
            {
              --count;
              flag = true;
            }
            sb.Append(this.OriginalString, component.Offset, count);
            if (num != 0)
            {
              sb.Append('\u202C');
              sb.Append(osMarker);
            }
            if (flag)
              sb.Append('\n');
          }
        }
      }

      private struct Range
      {
        public int Offset;
        public int EndOffset;
        public Bidi.CharClass Class;
      }
    }
  }
}
