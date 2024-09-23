// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaRichText
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using WhatsApp.WaCollections;


namespace WhatsApp
{
  public static class WaRichText
  {
    public static byte[] Serialize(IEnumerable<WaRichText.Chunk> items)
    {
      if (items == null)
        return (byte[]) null;
      BinaryData binaryData = new BinaryData();
      binaryData.AppendByte((byte) 11);
      foreach (WaRichText.Chunk chunk in items)
      {
        binaryData.AppendInt32((int) chunk.Format);
        binaryData.AppendInt32(chunk.Offset);
        binaryData.AppendInt32(chunk.Length);
      }
      return binaryData.Get();
    }

    public static List<WaRichText.Chunk> Deserialize(byte[] chunksBuf, string s)
    {
      List<WaRichText.Chunk> chunkList = (List<WaRichText.Chunk>) null;
      if (chunksBuf != null && chunksBuf.Length >= 13 && chunksBuf[0] == (byte) 11)
      {
        chunkList = new List<WaRichText.Chunk>();
        BinaryData binaryData = new BinaryData(chunksBuf);
        int length = chunksBuf.Length;
        int offset1 = 1;
        while (offset1 + 12 <= length)
        {
          int format = binaryData.ReadInt32(offset1);
          int offset2 = offset1 + 4;
          int offset3 = binaryData.ReadInt32(offset2);
          int offset4 = offset2 + 4;
          int len = binaryData.ReadInt32(offset4);
          offset1 = offset4 + 4;
          string info = "";
          if (len > 0)
            chunkList.Add(new WaRichText.Chunk(offset3, len, (WaRichText.Formats) format, info, s));
        }
      }
      return chunkList;
    }

    public static bool? BufferContainsValidChunks(byte[] chunksBuf)
    {
      if (chunksBuf == null || chunksBuf.Length <= 1)
        return new bool?(false);
      return chunksBuf[0] == (byte) 11 ? new bool?(chunksBuf.Length >= 13) : new bool?();
    }

    public static WaRichText.Chunk SingleChunkOrDefault(byte[] chunksBuf)
    {
      if (chunksBuf == null || chunksBuf.Length <= 1 || chunksBuf[0] != (byte) 11 || chunksBuf.Length != 13)
        return (WaRichText.Chunk) null;
      List<WaRichText.Chunk> source = WaRichText.Deserialize(chunksBuf, (string) null);
      return source == null ? (WaRichText.Chunk) null : source.FirstOrDefault<WaRichText.Chunk>();
    }

    public static int? NumChunks(byte[] chunksBuf)
    {
      return chunksBuf == null || chunksBuf.Length < 1 || chunksBuf[0] != (byte) 11 || (chunksBuf.Length - 1) % 12 != 0 ? new int?() : new int?((chunksBuf.Length - 1) / 12);
    }

    public static IEnumerable<WaRichText.Chunk> GetRichTextChunks(Message m)
    {
      WaRichText.Formats applicableFormats = WaRichText.Formats.TextFormattings | WaRichText.Formats.Link | WaRichText.Formats.Emoji;
      List<string> mentionedJids = m.GetMentionedJids();
      if (mentionedJids.Any<string>())
        applicableFormats |= WaRichText.Formats.Mention;
      return WaRichText.GetRichTextChunks(m.GetTextForDisplay(), new WaRichText.DetectionArgs(applicableFormats)
      {
        Mentions = mentionedJids
      });
    }

    public static IEnumerable<WaRichText.Chunk> GetRichTextChunks(
      string s,
      WaRichText.DetectionArgs args)
    {
      return (IEnumerable<WaRichText.Chunk>) null;
    }

    public static WaRichText.Chunk[] GetMentionChunks(Message m)
    {
      return WaRichText.GetMentionChunks(m.GetTextForDisplay(), m.GetMentionedJids());
    }

    public static WaRichText.Chunk[] GetMentionChunks(string s, List<string> mentionedJids)
    {
      if (mentionedJids == null || !mentionedJids.Any<string>() || string.IsNullOrEmpty(s))
        return new WaRichText.Chunk[0];
      Queue<Pair<string, string>> source1 = new Queue<Pair<string, string>>(mentionedJids.Select<string, Pair<string, string>>((Func<string, Pair<string, string>>) (jid => new Pair<string, string>(JidHelper.GetPhoneNumber(jid, false), jid))).Where<Pair<string, string>>((Func<Pair<string, string>, bool>) (item => !string.IsNullOrEmpty(item.First))));
      if (!source1.Any<Pair<string, string>>())
        return new WaRichText.Chunk[0];
      LinkedList<WaRichText.Chunk> source2 = new LinkedList<WaRichText.Chunk>();
      int length1 = s.Length;
      int count = source1.Count;
      for (int index1 = 0; index1 < length1; ++index1)
      {
        if (s[index1] == '@' && index1 + 1 < length1 && char.IsDigit(s[index1 + 1]))
        {
          int startIndex = index1 + 1;
          for (int index2 = 0; index2 < count; ++index2)
          {
            Pair<string, string> pair = source1.Dequeue();
            source1.Enqueue(pair);
            int length2 = pair.First.Length;
            if (length1 - startIndex >= length2 && s.IndexOf(pair.First, startIndex, length2) == startIndex)
            {
              source2.AddLast(new WaRichText.Chunk(index1, length2 + 1, WaRichText.Formats.Mention, pair.Second));
              index1 += length2;
              break;
            }
          }
        }
      }
      return source2.ToArray<WaRichText.Chunk>();
    }

    public static WaRichText.Chunk[] GetHtmlLinkChunks(string s)
    {
      return WaRichText.GetHtmlChunks(s, "a");
    }

    public static WaRichText.Chunk[] GetHtmlChunks(string s, string tag)
    {
      if (s == null)
        return new WaRichText.Chunk[0];
      List<WaRichText.Chunk> chunkList = new List<WaRichText.Chunk>();
      string str1 = string.Format("<{0}>", (object) tag);
      string str2 = string.Format("</{0}>", (object) tag);
      int offset = s.IndexOf(str1);
      int num;
      for (; offset >= 0; offset = s.IndexOf(str1, num + str2.Length))
      {
        num = s.IndexOf(str2, offset + str1.Length);
        if (num >= 0)
        {
          string tagContent = s.Substring(offset + str1.Length, num - offset - str1.Length);
          chunkList.Add(new WaRichText.Chunk(offset, num - offset + str2.Length, WaRichText.Formats.Link, (string) null, (Func<string>) (() => tagContent)));
        }
        else
          break;
      }
      return chunkList.ToArray();
    }

    public static List<LinkDetector.Result> MergeFormatings(
      IEnumerable<LinkDetector.Result> serializedChunks,
      IEnumerable<WaRichText.Chunk> floatingChunks,
      string originalStr)
    {
      if (serializedChunks == null || !serializedChunks.Any<LinkDetector.Result>())
      {
        if (floatingChunks == null || !floatingChunks.Any<WaRichText.Chunk>())
          return (List<LinkDetector.Result>) null;
        serializedChunks = (IEnumerable<LinkDetector.Result>) new List<LinkDetector.Result>()
        {
          new LinkDetector.Result(0, originalStr.Length, 0, originalStr)
        };
      }
      else if (floatingChunks == null || !floatingChunks.Any<WaRichText.Chunk>())
        return serializedChunks.ToList<LinkDetector.Result>();
      List<LinkDetector.Result> resultList = new List<LinkDetector.Result>();
      LinkedList<LinkDetector.Result> source1 = new LinkedList<LinkDetector.Result>(serializedChunks);
      LinkedList<WaRichText.Chunk> source2 = new LinkedList<WaRichText.Chunk>(floatingChunks);
      while (source2.Any<WaRichText.Chunk>())
      {
        if (!source1.Any<LinkDetector.Result>())
        {
          Log.d("failed merging hint lists", "continuing");
          break;
        }
        LinkDetector.Result other = source1.First.Value;
        WaRichText.Chunk chunk1 = source2.First.Value;
        if (other.Index > chunk1.Offset)
        {
          source2.RemoveFirst();
          Log.d("failed merging hint lists", "dropped partial hint, continuing");
        }
        else
        {
          int length1 = chunk1.Length;
          int offset = chunk1.Offset;
          int num1 = offset + length1 - 1;
          int num2 = other.Index + other.Length - 1;
          if (offset > num2)
          {
            resultList.Add(other);
            source1.RemoveFirst();
          }
          else
          {
            source1.RemoveFirst();
            source2.RemoveFirst();
            if (offset > other.Index)
              resultList.Add(new LinkDetector.Result(other)
              {
                Length = offset - other.Index
              });
            int startIndex = offset - other.Index;
            int num3 = startIndex + length1;
            string part1string = chunk1.Value ?? other.Value?.Substring(startIndex, num3 - startIndex);
            if (num1 > num2)
            {
              int length2 = num1 - num2;
              part1string = other.Value?.Substring(startIndex, length2);
              WaRichText.Chunk chunk2 = new WaRichText.Chunk(offset, startIndex + chunk1.Length - length2, chunk1.Format, chunk1.AuxiliaryInfo, chunk1.ClickAction);
              source2.AddFirst(chunk2);
            }
            resultList.Add(new LinkDetector.Result(offset, chunk1.Length, (int) ((WaRichText.Formats) other.type | chunk1.Format), (Func<string>) (() => part1string), chunk1.AuxiliaryInfo)
            {
              ClickAction = chunk1.ClickAction,
              ForegroundColor = chunk1.ForegroundColor,
              LinkUnderscore = chunk1.LinkUnderscore
            });
            if (num1 < num2)
              source1.AddFirst(new LinkDetector.Result(other)
              {
                Index = num1 + 1,
                Length = num2 - num1
              });
          }
        }
      }
      if (source1.Any<LinkDetector.Result>())
      {
        foreach (LinkDetector.Result result in source1)
          resultList.Add(result);
      }
      return resultList;
    }

    public static string FormatMentions(string s, List<string> mentionedJids)
    {
      if (!mentionedJids.Any<string>() || string.IsNullOrEmpty(s))
        return s;
      StringBuilder sb = new StringBuilder();
      WaRichText.Chunk[] mentionChunks = WaRichText.GetMentionChunks(s, mentionedJids);
      int i = 0;
      ContactsContext.Instance((Action<ContactsContext>) (db =>
      {
        foreach (WaRichText.Chunk chunk in mentionChunks)
        {
          string auxiliaryInfo = chunk.AuxiliaryInfo;
          UserStatus userStatus = db.GetUserStatus(auxiliaryInfo, false);
          sb.Append(s.Substring(i, chunk.Offset - i));
          string str = userStatus?.GetDisplayName(getNumberIfNoName: false, getFormattedNumber: false);
          if (string.IsNullOrEmpty(str))
          {
            str = userStatus?.PushName ?? (auxiliaryInfo == Settings.MyJid ? Settings.PushName : (string) null);
            if (string.IsNullOrEmpty(str))
              str = JidHelper.GetPhoneNumber(auxiliaryInfo, true);
          }
          sb.AppendFormat("@{0}", (object) str);
          i = chunk.Offset + chunk.Length;
        }
      }));
      sb.Append(s.Substring(i));
      return sb.ToString();
    }

    public static bool ContainsFormat(WaRichText.Formats formats, WaRichText.Formats format)
    {
      return (formats & format) == format;
    }

    public static bool ContainsFormat(
      IEnumerable<LinkDetector.Result> chunks,
      WaRichText.Formats format)
    {
      if (chunks == null || !chunks.Any<LinkDetector.Result>())
        return false;
      foreach (LinkDetector.Result chunk in chunks)
      {
        if (((WaRichText.Formats) chunk.type & format) != WaRichText.Formats.None)
          return true;
      }
      return false;
    }

    [Flags]
    public enum Formats
    {
      None = 0,
      Link = 1,
      Emoji = 2,
      Bold = 4,
      Italic = 8,
      Code = 16, // 0x00000010
      Strikethrough = 32, // 0x00000020
      Emoticon = 64, // 0x00000040
      Foreground = 128, // 0x00000080
      Mention = 256, // 0x00000100
      TextFormattings = Strikethrough | Code | Italic | Bold, // 0x0000003C
    }

    public class Chunk
    {
      public const byte Version = 11;
      public const int MinBytesPerChunk = 12;
      private string valueCache;

      public int Offset { get; set; }

      public int Length { get; set; }

      public WaRichText.Formats Format { get; set; }

      public string AuxiliaryInfo { get; set; }

      public Color? ForegroundColor { get; set; }

      public Action ClickAction { get; set; }

      public bool LinkUnderscore { get; set; } = true;

      public string OriginalString { get; private set; }

      public Func<string> ValueFunc { get; set; }

      public string Value
      {
        get
        {
          if (this.valueCache == null)
            this.valueCache = this.PopulateValue();
          return this.valueCache;
        }
      }

      public Chunk(int offset, int len, WaRichText.Formats format, string info = null, Action clickAct = null)
      {
        this.Offset = offset;
        this.Length = len;
        this.Format = format;
        this.AuxiliaryInfo = info;
        this.ClickAction = clickAct;
      }

      public Chunk(
        int offset,
        int len,
        WaRichText.Formats format,
        string info,
        Func<string> valueFunc,
        Action clickAct = null)
      {
        this.Offset = offset;
        this.Length = len;
        this.Format = format;
        this.AuxiliaryInfo = info;
        this.ValueFunc = valueFunc;
        this.ClickAction = clickAct;
      }

      public Chunk(
        int offset,
        int len,
        WaRichText.Formats format,
        string info,
        string originalString,
        Action clickAct = null)
      {
        this.Offset = offset;
        this.Length = len;
        this.Format = format;
        this.AuxiliaryInfo = info;
        this.OriginalString = originalString;
        this.ClickAction = clickAct;
      }

      public bool ContainsFormat(WaRichText.Formats format)
      {
        return WaRichText.ContainsFormat(this.Format, format);
      }

      private string PopulateValue()
      {
        string str = (string) null;
        if (this.ValueFunc == null)
        {
          if (this.OriginalString != null && this.Offset < this.OriginalString.Length)
            str = this.OriginalString.Substring(this.Offset, Math.Min(this.Length, this.OriginalString.Length - this.Offset));
        }
        else
          str = this.ValueFunc();
        return str;
      }
    }

    public class DetectionArgs
    {
      public List<string> Mentions { get; set; }

      public WaRichText.Formats ApplicableFormats { get; private set; }

      public DetectionArgs()
      {
        this.ApplicableFormats = WaRichText.Formats.TextFormattings | WaRichText.Formats.Link | WaRichText.Formats.Emoji;
      }

      public DetectionArgs(WaRichText.Formats applicableFormats)
      {
        this.ApplicableFormats = applicableFormats;
      }

      public bool ShouldDetect(WaRichText.Formats format)
      {
        return WaRichText.ContainsFormat(this.ApplicableFormats, format);
      }
    }
  }
}
