// Decompiled with JetBrains decompiler
// Type: WhatsApp.Utils
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using WhatsApp.RegularExpressions;
using WhatsApp.WaCollections;


namespace WhatsApp
{
  public static class Utils
  {
    private static object defaultLazyInitLock = new object();
    public static string CommaSeparator = (string) null;

    public static T LazyInit<T>(ref T obj, Func<T> create, object @lock = null, bool forceLock = false)
    {
      if ((object) obj == null | forceLock)
      {
        lock (@lock ?? Utils.defaultLazyInitLock)
        {
          if ((object) obj == null)
          {
            T obj1 = create();
            Interlocked.MemoryBarrier();
            obj = obj1;
          }
        }
      }
      return obj;
    }

    public static Action IgnoreMultipleInvokes(Action a)
    {
      return (Action) (() =>
      {
        Action action = Interlocked.Exchange<Action>(ref a, (Action) null);
        if (action == null)
          return;
        action();
      });
    }

    public static Action<T> IgnoreMultipleInvokes<T>(Action<T> a)
    {
      return (Action<T>) (t =>
      {
        Action<T> action = Interlocked.Exchange<Action<T>>(ref a, (Action<T>) null);
        if (action == null)
          return;
        action(t);
      });
    }

    public static bool NonShortCircuitOr(params bool[] Bools)
    {
      return ((IEnumerable<bool>) Bools).Where<bool>((Func<bool, bool>) (b => b)).Any<bool>();
    }

    public static bool NonShortCircuitAnd(params bool[] Bools)
    {
      return !((IEnumerable<bool>) Bools).Where<bool>((Func<bool, bool>) (b => !b)).Any<bool>();
    }

    public static T2 TakeFirstNonThrowing<T1, T2>(IEnumerable<T1> source, Func<T1, T2> selector)
    {
      List<Exception> innerExceptions = new List<Exception>();
      bool flag = false;
      Action action = (Action) null;
      T2 firstNonThrowing = default (T2);
      foreach (T1 obj in source)
      {
        try
        {
          firstNonThrowing = selector(obj);
          flag = true;
          break;
        }
        catch (Exception ex)
        {
          innerExceptions.Add(ex);
          if (action == null)
            action = ex.GetRethrowAction();
        }
      }
      if (!flag)
      {
        if (innerExceptions.Count > 1)
          throw new AggregateException((IEnumerable<Exception>) innerExceptions);
        if (action != null)
          action();
      }
      return firstNonThrowing;
    }

    public static bool IsLeadingSurrogate(char ch) => ((int) ch & 64512) == 55296;

    public static bool IsTrailingSurrogate(char ch) => ((int) ch & 64512) == 56320;

    public static bool IsCombiningChracter(char ch)
    {
      if (ch >= '̀' && ch < 'Ͱ' || ch >= '᪰' && ch < 'ᬀ' || ch >= '᷀' && ch < 'Ḁ' || ch >= '⃐' && ch < '℀')
        return true;
      return ch >= '︠' && ch < '︰';
    }

    private static bool IsOddRegionalIndicator(string str, int offset, int len)
    {
      int num = 0;
      for (; offset >= 0 && offset + 2 <= len && Utils.IsLeadingSurrogate(str[offset]) && Utils.IsTrailingSurrogate(str[offset + 1]) && Emoji.IsRegionalIndicator(UTF32.ToUtf32(str[offset], str[offset + 1])); offset -= 2)
        ++num;
      return num > 1 && num % 2 == 0;
    }

    public static bool IsFunkyUnicode(string str, int offset, int len, out int funkynessLength)
    {
      funkynessLength = 0;
      while (len > 0 && offset < len)
      {
        char ch = str[offset];
        if (Utils.IsTrailingSurrogate(ch) || Utils.IsCombiningChracter(ch) || Emoji.IsVariantSelector((uint) ch))
        {
          ++funkynessLength;
          ++offset;
          --len;
        }
        else if (Utils.IsOddRegionalIndicator(str, offset, len))
        {
          funkynessLength += 2;
          offset += 2;
          len -= 2;
        }
        else if (ch == '\u200D')
        {
          ++funkynessLength;
          ++offset;
          --len;
          if (len > 0 && offset < len)
          {
            ++offset;
            ++funkynessLength;
            --len;
          }
        }
        else
          break;
      }
      return funkynessLength != 0;
    }

    public static bool IsFunkyUnicode(string str, int offset, out int funkynessLength)
    {
      return Utils.IsFunkyUnicode(str, offset, str.Length, out funkynessLength);
    }

    public static int ExpandEmojiUnicodeRight(string str, int offset, bool searchRegionalFlag)
    {
      int length = str.Length;
      int index = offset;
      if (index == 0)
        return 0;
      Log.d("util", "expand emoji right | len={0}, offset={1}", (object) length, (object) offset);
      bool flag = !searchRegionalFlag;
      while (index < length)
      {
        char ch = str[index - 1];
        char highSurrogate = str[index];
        if (Utils.IsCombiningChracter(ch) || Emoji.IsVariantSelector((uint) ch))
          Log.d("util", "expand emoji right | error");
        if (Utils.IsLeadingSurrogate(ch) || ch == '\u200D' || highSurrogate == '\u200D' || highSurrogate == '\u200B')
          ++index;
        else if (!flag && Emoji.IsRegionalIndicator(UTF32.ToUtf32(highSurrogate, ch)))
        {
          flag = true;
          ++index;
        }
        else
          break;
      }
      return index - offset;
    }

    public static int ExpandEmojiUnicodeLeft(string str, int offset, out bool seenRegionalFlag)
    {
      int length = str.Length;
      int index = offset;
      seenRegionalFlag = false;
      if (index >= length)
        return index - length;
      Log.d("util", "expand emoji left | len={0}, offset={1}", (object) length, (object) offset);
      while (index > 0)
      {
        char lowSurrogate = str[index - 1];
        char ch = str[index];
        if (Utils.IsCombiningChracter(ch) || Emoji.IsVariantSelector((uint) ch))
          Log.d("util", "expand emoji left | error");
        if (Utils.IsTrailingSurrogate(ch) || ch == '\u200D' || lowSurrogate == '\u200D' || ch == '\u200B')
          --index;
        else if (!seenRegionalFlag && Emoji.IsRegionalIndicator(UTF32.ToUtf32(ch, lowSurrogate)))
        {
          seenRegionalFlag = true;
          --index;
        }
        else
          break;
      }
      return offset - index;
    }

    public static string TruncateAtIndex(string str, int idx)
    {
      if (idx >= str.Length)
        return string.Copy(str);
      while (idx > 0 && Utils.IsLeadingSurrogate(str[idx - 1]))
        --idx;
      return str.Substring(0, idx);
    }

    public static void Swap<T>(ref T a, ref T b)
    {
      T obj = a;
      a = b;
      b = obj;
    }

    public static int BinarySearch<T1, T2>(this T1[] array, T2 Key, Func<T2, T1, int> comparer)
    {
      int length = array.Length;
      int num1 = 0;
      int num2 = length;
      while (num1 < num2 || num1 == num2 && num1 < length)
      {
        int index = num1 + (num2 - num1) / 2;
        int num3 = comparer(Key, array[index]);
        if (num3 > 0)
        {
          num1 = index + 1;
        }
        else
        {
          if (num3 >= 0)
            return index;
          num2 = index - 1;
        }
      }
      return -1;
    }

    public static int BinarySearch<T>(this T[] array, T Key) where T : IComparable<T>
    {
      return array.BinarySearch<T, T>(Key, (Func<T, T, int>) ((key, item) => key.CompareTo(item)));
    }

    public static string FromUrlSafeBase64String(this string value)
    {
      StringBuilder stringBuilder = new StringBuilder(value.Length);
      for (int index = 0; index < value.Length; ++index)
      {
        char ch = value[index];
        switch (ch)
        {
          case '-':
            stringBuilder.Append('+');
            break;
          case '_':
            stringBuilder.Append('/');
            break;
          default:
            stringBuilder.Append(ch);
            break;
        }
      }
      return stringBuilder.ToString();
    }

    public static string ToUrlSafeBase64String(this string value)
    {
      StringBuilder stringBuilder = new StringBuilder(value.Length);
      for (int index = 0; index < value.Length; ++index)
      {
        char ch = value[index];
        switch (ch)
        {
          case '+':
            stringBuilder.Append('-');
            break;
          case '/':
            stringBuilder.Append('_');
            break;
          default:
            stringBuilder.Append(ch);
            break;
        }
      }
      return stringBuilder.ToString();
    }

    public static string ToUrlSafeBase64String(this byte[] value)
    {
      return Convert.ToBase64String(value).ToUrlSafeBase64String();
    }

    public static char? CalculateContactSections(string displayName)
    {
      char? contactSections = new char?();
      if (!string.IsNullOrEmpty(displayName))
      {
        contactSections = new char?(char.ToUpper(displayName[0]));
        if (!char.IsDigit(contactSections.Value))
        {
          char? nullable1 = contactSections;
          int? nullable2 = nullable1.HasValue ? new int?((int) nullable1.GetValueOrDefault()) : new int?();
          int num = 43;
          if ((nullable2.GetValueOrDefault() == num ? (nullable2.HasValue ? 1 : 0) : 0) == 0)
            goto label_4;
        }
        contactSections = new char?('#');
      }
label_4:
      return contactSections;
    }

    public static IEnumerable<Utils.FormatResult> Format(string str, params string[] args)
    {
      Regex regex = new Regex("\\{(\\d+)\\}");
      int startIndex = 0;
      Func<string, int?, Utils.FormatResult> process = (Func<string, int?, Utils.FormatResult>) ((text, matchIdx) => new Utils.FormatResult()
      {
        Value = text,
        Index = matchIdx
      });
      foreach (Match m in (IEnumerable<Match>) regex.Matches(str))
      {
        if (m.Index > startIndex)
          yield return process(str.Substring(startIndex, m.Index - startIndex), new int?());
        int index = int.Parse(m.Groups[1].Value);
        yield return process(args[index], new int?(index));
        startIndex = m.Index + m.Length;
      }
      if (startIndex != str.Length)
        yield return process(str.Substring(startIndex), new int?());
    }

    public static string CommaSeparate(IEnumerable<string> strs)
    {
      if (Utils.CommaSeparator == null)
      {
        CultureInfo that = new CultureInfo(AppResources.CultureString);
        string str1 = (string) null;
        string str2 = (string) null;
        ref string local1 = ref str1;
        ref string local2 = ref str2;
        that.GetLangAndLocale(out local1, out local2);
        Utils.CommaSeparator = !(str1 == "ar") ? ", " : "، ";
      }
      StringBuilder stringBuilder = new StringBuilder();
      int num = 0;
      foreach (string str in strs)
      {
        if (stringBuilder.Length != 0)
          stringBuilder.Append(Utils.CommaSeparator);
        stringBuilder.Append('{');
        stringBuilder.Append(num++.ToString());
        stringBuilder.Append('}');
      }
      return Bidi.Format(stringBuilder.ToString(), strs.ToArray<string>());
    }

    public static string DecodeQuotedPrintable(string input)
    {
      Encoding utF8 = Encoding.UTF8;
      StringBuilder stringBuilder = new StringBuilder();
      StringReader stringReader = new StringReader(input);
      Func<byte, bool> func = (Func<byte, bool>) (b => b > (byte) 32 && b < (byte) 127 || b == (byte) 9 || b == (byte) 32);
      string str1 = stringReader.ReadLine();
      while (str1 != null)
      {
        bool flag = true;
        byte[] bytes1 = utF8.GetBytes(str1.ToCharArray());
        int index = 0;
        while (index < bytes1.Length)
        {
          if (bytes1[index] == (byte) 61)
          {
            if (index == bytes1.Length - 1)
            {
              flag = false;
              break;
            }
            if (bytes1[index + 1] == (byte) 51 && bytes1[index + 2] == (byte) 68)
            {
              stringBuilder.Append("=");
              index += 3;
            }
            else
            {
              char ch = Convert.ToChar(bytes1[index + 1]);
              string str2 = ch.ToString();
              ch = Convert.ToChar(bytes1[index + 2]);
              string str3 = ch.ToString();
              byte num1 = Convert.ToByte(str2 + str3, 16);
              index += 3;
              if (func(num1))
              {
                stringBuilder.Append(Convert.ToChar(num1));
              }
              else
              {
                byte[] bytes2 = new byte[2]
                {
                  num1,
                  (byte) 0
                };
                byte[] numArray = bytes2;
                ch = Convert.ToChar(bytes1[index + 1]);
                string str4 = ch.ToString();
                ch = Convert.ToChar(bytes1[index + 2]);
                string str5 = ch.ToString();
                int num2 = (int) Convert.ToByte(str4 + str5, 16);
                numArray[1] = (byte) num2;
                index += 3;
                stringBuilder.Append(utF8.GetString(bytes2, 0, 2));
              }
            }
          }
          else if (func(bytes1[index]))
            stringBuilder.Append(Convert.ToChar(bytes1[index++]));
        }
        str1 = stringReader.ReadLine();
        if (str1 != null & flag)
          stringBuilder.Append(" ");
      }
      return stringBuilder.ToString();
    }

    public static void ClearTimeZoneCache()
    {
      TimeZoneInfo.ConvertTime(new DateTime(0L), TimeZoneInfo.Utc);
    }

    public static T TryGetExpression<T>(Func<T> getExpr, T @default)
    {
      try
      {
        return getExpr();
      }
      catch (Exception ex)
      {
        return @default;
      }
    }

    public static T TryGetExpression<T>(Func<T> f) => Utils.TryGetExpression<T>(f, default (T));

    public static string InferFileExtensionFromMimeType(string mimeType)
    {
      if (mimeType == null)
        return (string) null;
      switch (mimeType.ToLower())
      {
        case "application/msword":
          return "doc";
        case "application/pdf":
          return "pdf";
        case "application/vnd.ms-excel":
          return "xls";
        case "application/vnd.ms-powerpoint":
          return "ppt";
        case "application/vnd.openxmlformats-officedocument.presentationml.presentation":
          return "pptx";
        case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet":
          return "xlsx";
        case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
          return "docx";
        case "audio/3gpp":
          return "3gp";
        case "audio/amr":
          return "amr";
        case "audio/amr-wb":
          return "awb";
        case "audio/mpeg":
          return "mp3";
        case "audio/ogg":
          return "opus";
        case "audio/ogg; codecs=opus":
          return "opus";
        case "audio/x-caf":
          return "caf";
        case "image/gif":
          return "gif";
        case "image/jpeg":
          return "jpg";
        case "image/png":
          return "png";
        case "image/webp":
          return "webp";
        case "text/plain":
          return "txt";
        case "video/3gpp":
          return "3gp";
        case "video/mp4":
          return "mp4";
        case "video/quicktime":
          return "mov";
        default:
          return (string) null;
      }
    }

    public static void UpdateInPlace<TX, TY>(
      IList<TX> oldList,
      IList<TY> newList,
      Func<TX, string> keySelectorX,
      Func<TY, string> keySelectorY,
      Func<TY, TX> convertYToX,
      Action<TX> updateItemInPlace)
    {
      Set<string> set1 = new Set<string>(oldList.Select<TX, string>(keySelectorX));
      Set<string> set2 = new Set<string>(newList.Select<TY, string>(keySelectorY));
      int index1 = 0;
      int index2 = 0;
      bool flag1 = index2 < oldList.Count;
      for (bool flag2 = index1 < newList.Count; flag1 | flag2; flag2 = index1 < newList.Count)
      {
        if (flag1 && !set2.Contains(keySelectorX(oldList[index2])))
          oldList.RemoveAt(index2);
        else if (flag2 && !set1.Contains(keySelectorY(newList[index1])))
        {
          TX x = convertYToX(newList[index1++]);
          oldList.Insert(index2++, x);
        }
        else
        {
          if (flag2)
            ++index1;
          if (flag1)
          {
            if (updateItemInPlace != null)
              updateItemInPlace(oldList[index2]);
            ++index2;
          }
        }
        flag1 = index2 < oldList.Count;
      }
    }

    public static void UpdateInPlace<T>(
      IList<T> oldList,
      IList<T> newList,
      Func<T, string> keySelector,
      Action<T> updateInPlace)
    {
      Utils.UpdateInPlace<T, T>(oldList, newList, keySelector, keySelector, (Func<T, T>) (t => t), updateInPlace);
    }

    private static void InsertToDisambiguatedDictionary(
      string jid,
      Dictionary<string, string> namesToDisplay,
      bool insert)
    {
      string nameForContactJid1 = JidHelper.GetShortDisplayNameForContactJid(jid);
      string nameForContactJid2 = JidHelper.GetDisplayNameForContactJid(jid);
      string key1 = nameForContactJid1;
      if (namesToDisplay.ContainsKey(nameForContactJid1) && namesToDisplay[nameForContactJid1] != jid)
      {
        string jid1 = namesToDisplay[nameForContactJid1];
        string nameForContactJid3 = JidHelper.GetDisplayNameForContactJid(jid1);
        if (namesToDisplay.ContainsKey(nameForContactJid1))
          namesToDisplay.Remove(nameForContactJid1);
        namesToDisplay[nameForContactJid3] = jid1;
        if (insert)
          key1 = nameForContactJid2;
      }
      if (namesToDisplay.ContainsKey(nameForContactJid2) && namesToDisplay[nameForContactJid2] != jid)
      {
        string jid2 = namesToDisplay[nameForContactJid2];
        string key2 = nameForContactJid2 + " (" + JidHelper.GetPhoneNumber(jid2, true) + ")";
        if (namesToDisplay.ContainsKey(nameForContactJid2))
          namesToDisplay.Remove(nameForContactJid2);
        namesToDisplay[key2] = jid2;
        if (insert)
          key1 = nameForContactJid2 + " (" + JidHelper.GetPhoneNumber(jid, true) + ")";
      }
      if (!insert)
        return;
      namesToDisplay[key1] = jid;
    }

    public static string[] GetDisambiguatedNamesFromJids(string[] selection, string[] participants)
    {
      int num = 0;
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      switch (selection.Length)
      {
        case 0:
          while (num > 0)
            Utils.InsertToDisambiguatedDictionary(selection[--num], dictionary, true);
          foreach (string participant in participants)
            Utils.InsertToDisambiguatedDictionary(participant, dictionary, false);
          return dictionary.Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (kvp => kvp.Key)).ToArray<string>();
        case 1:
          num = 1;
          goto case 0;
        case 2:
          num = 2;
          goto case 0;
        default:
          num = 3;
          goto case 0;
      }
    }

    public static List<string> ParsePpsz(string str)
    {
      List<string> ppsz = new List<string>();
      int length;
      for (int startIndex = 0; startIndex < str.Length; startIndex += length + 1)
      {
        length = 0;
        while (str[startIndex + length] != char.MinValue)
          ++length;
        if (length != 0)
          ppsz.Add(str.Substring(startIndex, length));
        else
          break;
      }
      return ppsz;
    }

    public static string ToPpsz(IEnumerable<string> strs)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (string str in strs)
      {
        stringBuilder.Append(str);
        stringBuilder.Append(char.MinValue);
      }
      stringBuilder.Append(char.MinValue);
      return stringBuilder.ToString();
    }

    public static string BaseName(string str)
    {
      int num = str.LastIndexOfAny(new char[2]{ '\\', '/' });
      return num >= 0 ? str.Substring(num + 1) : str;
    }

    public static uint CalculateFibonacci(int iteration)
    {
      uint num1 = 1;
      uint fibonacci = 1;
      for (int index = 0; index < iteration; ++index)
      {
        int num2 = (int) num1 + (int) fibonacci;
        num1 = fibonacci;
        fibonacci = (uint) num2;
      }
      return fibonacci;
    }

    public static int CalculateLevenshteinDistance(string s1, string s2, int? threshold = null)
    {
      int length1 = s1 != null ? s1.Length : 0;
      int length2 = s2 != null ? s2.Length : 0;
      if (threshold.HasValue && Math.Abs(length1 - length2) > threshold.Value)
        return int.MaxValue;
      if (length1 == 0)
        return length2;
      if (length2 == 0)
        return length1;
      if (length1 > length2)
      {
        Utils.Swap<string>(ref s1, ref s2);
        Utils.Swap<int>(ref length1, ref length2);
      }
      int[] numArray = new int[length1 + 1];
      for (int index = 0; index <= length1; ++index)
        numArray[index] = index;
      for (int index1 = 1; index1 <= length2; ++index1)
      {
        int num1 = numArray[0];
        char ch = s2[index1 - 1];
        numArray[0] = index1;
        for (int index2 = 1; index2 <= length1; ++index2)
        {
          int num2 = numArray[index2];
          int num3 = (int) s1[index2 - 1] == (int) ch ? 0 : 1;
          numArray[index2] = Math.Min(Math.Min(numArray[index2 - 1] + 1, numArray[index2] + 1), num1 + num3);
          num1 = num2;
        }
      }
      int num = numArray[length1];
      return !threshold.HasValue || num <= threshold.Value ? num : int.MaxValue;
    }

    public class FileSizeFormatter
    {
      private static readonly string CONTENT_UNITS = "bkmgt";

      public static string Format(long bytes)
      {
        if (bytes <= 0L)
          return (string) null;
        if (bytes < 1024L)
          return string.Format("{0} bytes", (object) bytes);
        double amt1 = (double) bytes / 1024.0;
        if (amt1 < 1024.0)
          return Utils.FileSizeFormatter.ToUnitString(amt1, "KB");
        double amt2 = amt1 / 1024.0;
        return amt2 < 1024.0 ? Utils.FileSizeFormatter.ToUnitString(amt2, "MB") : Utils.FileSizeFormatter.ToUnitString(amt2 / 1024.0, "GB");
      }

      private static string ToUnitString(double amt, string unitType)
      {
        if (amt < 1.0)
          return string.Format("{0:0.00} {1}", (object) amt, (object) unitType);
        return amt < 10.0 ? string.Format("{0:0.0} {1}", (object) amt, (object) unitType) : string.Format("{0:0} {1}", (object) amt, (object) unitType);
      }

      public static long ConvertToLong(string sizeWithUnit)
      {
        try
        {
          if (string.IsNullOrEmpty(sizeWithUnit))
            return -1;
          string[] strArray = sizeWithUnit.Split(' ');
          if (strArray == null || strArray.Length < 1)
            return -1;
          long num1 = long.Parse(strArray[0]);
          string str = strArray.Length < 2 || string.IsNullOrEmpty(strArray[1]) ? " " : strArray[1].Substring(0, 1).ToLowerInvariant();
          int y = Utils.FileSizeFormatter.CONTENT_UNITS.IndexOf(str);
          double num2 = y < 1 ? 1.0 : Math.Pow(1024.0, (double) y);
          return (long) ((double) num1 * num2);
        }
        catch (Exception ex)
        {
          string context = "Exception converting size: " + sizeWithUnit;
          Log.LogException(ex, context);
          return -1;
        }
      }
    }

    public class FiboGenerator
    {
      private int a_;
      private int b_ = 1;

      public void Skip(int n)
      {
        for (int index = 0; index < n; ++index)
          this.Next();
      }

      public int Next()
      {
        int b = this.b_;
        this.b_ += this.a_;
        this.a_ = b;
        return b;
      }
    }

    public class FormatResult
    {
      public string Value;
      public int? Index;
    }

    public enum TimeSpentFieldStatsRecordOption
    {
      NoRecording = 0,
      BitArray = 1,
      SessionEvent = 2,
      SessionSummary = 4,
    }
  }
}
