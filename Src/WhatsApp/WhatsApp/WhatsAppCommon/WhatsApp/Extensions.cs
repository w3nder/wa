// Decompiled with JetBrains decompiler
// Type: WhatsApp.Extensions
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using ICSharpCode.SharpZipLib.Silverlight.GZip;
using ICSharpCode.SharpZipLib.Silverlight.Zip;
using Microsoft.Phone.Reactive;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using WhatsApp.RegularExpressions;
using WhatsAppNative;


namespace WhatsApp
{
  public static class Extensions
  {
    public static char[] nlChars = new char[2]{ '\r', '\n' };
    private static Regex nlRegex = new Regex("\\r(\\n|)");

    public static IWAScheduler AsWAScheduler(this IScheduler sched)
    {
      return (IWAScheduler) new Extensions.WAScheduler(sched);
    }

    public static IAction AsComAction(this Action a) => (IAction) new Extensions.WAAction(a);

    public static T TryPeek<T>(this Queue<T> queue)
    {
      T obj = default (T);
      if (queue.Count > 0)
        obj = queue.Peek();
      return obj;
    }

    public static MemoryStream Gzip(this Stream input)
    {
      MemoryStream baseOutputStream = new MemoryStream();
      GZipOutputStream destination = new GZipOutputStream((Stream) baseOutputStream);
      input.CopyTo((Stream) destination);
      destination.Finish();
      baseOutputStream.Position = 0L;
      return baseOutputStream;
    }

    public static Stream CreateGzipDecompressionStream(this Stream compressedStream)
    {
      return (Stream) new GZipInputStream(compressedStream);
    }

    public static MemoryStream Gunzip(this Stream input)
    {
      MemoryStream destination = new MemoryStream();
      new GZipInputStream(input).CopyTo((Stream) destination);
      destination.Position = 0L;
      return destination;
    }

    public static MemoryStream ExtractZipFile(
      Stream archiveFileStream,
      string desiredFile1,
      string desiredFile2 = null)
    {
      ZipFile zipFile = (ZipFile) null;
      MemoryStream destination = (MemoryStream) null;
      try
      {
        zipFile = new ZipFile(archiveFileStream);
        foreach (ZipEntry entry in zipFile)
        {
          if (entry.IsFile)
          {
            string name = entry.Name;
            if (desiredFile1 == name || desiredFile2 != null && desiredFile2 == name)
            {
              destination = new MemoryStream();
              using (Stream inputStream = zipFile.GetInputStream(entry))
                inputStream.CopyTo((Stream) destination);
              Log.d("zip", "length out {0}", (object) destination.Length);
              destination.Position = 0L;
              break;
            }
          }
        }
      }
      finally
      {
        zipFile?.Close();
      }
      return destination;
    }

    public static int getXlsxSheetCount(Stream archiveFileStream)
    {
      ZipFile zipFile = (ZipFile) null;
      int val1 = -1;
      try
      {
        zipFile = new ZipFile(archiveFileStream);
        string str1 = "xl/worksheets/sheet";
        string str2 = ".xml";
        foreach (ZipEntry zipEntry in zipFile)
        {
          if (zipEntry.IsFile)
          {
            string name = zipEntry.Name;
            if (name.StartsWith(str1) && name.EndsWith(str2))
            {
              int val2 = int.Parse(name.Substring(str1.Length, name.Length - (str1.Length + str2.Length)));
              val1 = Math.Max(val1, val2);
            }
          }
        }
      }
      finally
      {
        zipFile?.Close();
      }
      return val1;
    }

    public static string extractItemCountFromXmlPropsStream(
      Stream xmlPropStream,
      string extractionRegex,
      string refineRegex)
    {
      try
      {
        Match match1 = new Regex(extractionRegex).Match(new StreamReader(xmlPropStream).ReadToEnd());
        if (match1.Success)
        {
          Match match2 = new Regex(refineRegex).Match(match1.Value);
          if (match2.Success)
            return match2.Value;
        }
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "Exception attempting to extract count from open docs file");
      }
      return (string) null;
    }

    public static T MaxOfFunc<T, T2>(this IEnumerable<T> seq, Func<T, T2> selector) where T2 : IComparable
    {
      T obj1 = default (T);
      T2 obj2 = default (T2);
      bool flag = false;
      foreach (T obj3 in seq)
      {
        T2 obj4 = selector(obj3);
        if (!flag || obj4.CompareTo((object) obj2) > 0)
        {
          obj1 = obj3;
          obj2 = obj4;
          flag = true;
        }
      }
      return obj1;
    }

    public static T MinOfFunc<T, T2>(this IEnumerable<T> seq, Func<T, T2> selector) where T2 : IComparable
    {
      T obj1 = default (T);
      T2 obj2 = default (T2);
      bool flag = false;
      foreach (T obj3 in seq)
      {
        T2 obj4 = selector(obj3);
        if (!flag || obj4.CompareTo((object) obj2) < 0)
        {
          obj1 = obj3;
          obj2 = obj4;
          flag = true;
        }
      }
      return obj1;
    }

    public static bool IsPsaJid(this string str) => JidHelper.IsPsaJid(str);

    public static bool IsGroupJid(this string str) => JidHelper.IsGroupJid(str);

    public static bool IsBroadcastJid(this string str) => JidHelper.IsBroadcastJid(str);

    public static bool IsUserJid(this string str) => JidHelper.IsUserJid(str);

    public static string GetFirstLine(this string s)
    {
      if (!string.IsNullOrEmpty(s))
      {
        int length = s.IndexOfAny(Extensions.nlChars);
        if (length >= 0)
          return s.Substring(0, length);
      }
      return s;
    }

    public static IList<U> ToGoodGrouping<T, U>(this IGrouping<T, U> that)
    {
      return (IList<U>) new Extensions.Grouping<T, U>(that.Key, (IEnumerable<U>) that);
    }

    public static void InsertInOrder<T>(
      this IList<T> that,
      T newItem,
      Func<T, T, bool> compareFunc)
    {
      int count = that.Count;
      for (int index = 0; index < count; ++index)
      {
        T obj = that.ElementAt<T>(index);
        if (compareFunc(newItem, obj))
        {
          that.Insert(index, newItem);
          return;
        }
      }
      that.Add(newItem);
    }

    public static LinkedListNode<T> InsertInOrder<T>(
      this LinkedList<T> list,
      T newItem,
      Func<T, T, bool> compareFunc)
    {
      LinkedListNode<T> node = (LinkedListNode<T>) null;
      for (LinkedListNode<T> linkedListNode = list.First; linkedListNode != null && !compareFunc(linkedListNode.Value, newItem); linkedListNode = linkedListNode.Next)
        node = linkedListNode;
      return node != null ? list.AddAfter(node, newItem) : list.AddFirst(newItem);
    }

    public static IEnumerable<T> AsRemoveSafeEnumerator<T>(this LinkedList<T> list)
    {
      return (IEnumerable<T>) list.ToArray<T>();
    }

    public static string ConvertLineEndings(this string that)
    {
      return Extensions.nlRegex.Replace(that, (Func<Match, string>) (match => "\n"));
    }

    public static string ToRegexString(this IEnumerable<string> values, bool capture = false)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append('(');
      if (!capture)
        stringBuilder.Append("?:");
      int length = stringBuilder.Length;
      foreach (string str in values)
      {
        if (stringBuilder.Length != length)
          stringBuilder.Append('|');
        stringBuilder.Append(str);
      }
      stringBuilder.Append(')');
      return stringBuilder.ToString();
    }

    public static string ToLocalizedString(this EmailAddressKind emailAddressKind)
    {
      string localizedString;
      switch (emailAddressKind)
      {
        case EmailAddressKind.Personal:
          localizedString = AppResources.EmailKindPersonal;
          break;
        case EmailAddressKind.Work:
          localizedString = AppResources.EmailKindWork;
          break;
        default:
          localizedString = AppResources.EmailKindOther;
          break;
      }
      return localizedString;
    }

    public static string ToLocalizedString(this PhoneNumberKind phoneNumberKind)
    {
      string localizedString;
      switch (phoneNumberKind)
      {
        case PhoneNumberKind.Mobile:
          localizedString = AppResources.PhoneKindMobile;
          break;
        case PhoneNumberKind.Work:
          localizedString = AppResources.EmailKindWork;
          break;
        case PhoneNumberKind.Company:
          localizedString = AppResources.PhoneKindCompany;
          break;
        case PhoneNumberKind.Pager:
          localizedString = AppResources.PhoneKindPager;
          break;
        case PhoneNumberKind.HomeFax:
          localizedString = AppResources.PhoneKindHomeFax;
          break;
        case PhoneNumberKind.WorkFax:
          localizedString = AppResources.PhoneKindWorkFax;
          break;
        default:
          localizedString = AppResources.PhoneKindHome;
          break;
      }
      return localizedString;
    }

    private static byte ForDigit(int b) => b < 10 ? (byte) (48 + b) : (byte) (97 + b - 10);

    public static string ToHexString(this byte[] bytes)
    {
      byte[] bytes1 = new byte[bytes.Length * 2];
      int index1 = 0;
      for (int index2 = 0; index2 < bytes.Length; ++index2)
      {
        int num = (int) bytes[index2];
        if (num < 0)
          num += 256;
        bytes1[index1] = Extensions.ForDigit(num >> 4);
        int index3 = index1 + 1;
        bytes1[index3] = Extensions.ForDigit(num % 16);
        index1 = index3 + 1;
      }
      return Encoding.UTF8.GetString(bytes1, 0, bytes1.Length);
    }

    public static string ToHexString(this byte[] bytes, int start, int len)
    {
      if (bytes == null || start < 0 || len < 0 || bytes.Length < start + len)
      {
        Log.l(nameof (ToHexString), "Invalid arguments: {0} {1} {2}", bytes != null ? (object) bytes.Length.ToString() : (object) "null", (object) start, (object) len);
        throw new ArgumentOutOfRangeException(nameof (ToHexString));
      }
      if (len == 0)
        return "";
      byte[] bytes1 = new byte[len * 2];
      int index1 = 0;
      for (int index2 = start; index2 < start + len; ++index2)
      {
        int num = (int) bytes[index2];
        if (num < 0)
          num += 256;
        bytes1[index1] = Extensions.ForDigit(num >> 4);
        int index3 = index1 + 1;
        bytes1[index3] = Extensions.ForDigit(num % 16);
        index1 = index3 + 1;
      }
      return Encoding.UTF8.GetString(bytes1, 0, bytes1.Length);
    }

    public static byte[] FromHexString(this string hexString)
    {
      if (hexString == null)
        return (byte[]) null;
      return hexString.Length == 0 ? new byte[0] : Enumerable.Range(0, hexString.Length).Where<int>((Func<int, bool>) (x => x % 2 == 0)).Select<int, byte>((Func<int, byte>) (x => Convert.ToByte(hexString.Substring(x, 2), 16))).ToArray<byte>();
    }

    public static IObservable<T> ToObservable<T>(this IEnumerable<T> enumerable)
    {
      return Observable.Create<T>((Func<IObserver<T>, Action>) (observer =>
      {
        foreach (T obj in enumerable)
          observer.OnNext(obj);
        observer.OnCompleted();
        return (Action) (() => { });
      }));
    }

    public static IObservable<T> Cache<T>(
      this IObservable<T> source,
      Func<T> load,
      Action<T> store)
    {
      return Observable.CreateWithDisposable<T>((Func<IObserver<T>, IDisposable>) (observer =>
      {
        T obj = load();
        return (object) obj != null ? Observable.Return<T>(obj).Subscribe(observer) : source.Do<T>((Action<T>) (res => store(res))).Subscribe(observer);
      }));
    }

    public static IObservable<byte[]> Cache(
      this IObservable<byte[]> source,
      string filename,
      DateTime? minDate)
    {
      return source.Cache<byte[]>((Func<byte[]>) (() =>
      {
        try
        {
          using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
          {
            if (storeForApplication.FileExists(filename))
            {
              if (minDate.HasValue)
              {
                if (!(storeForApplication.GetLastWriteTime(filename) >= (DateTimeOffset) minDate.Value))
                  goto label_13;
              }
              using (IsolatedStorageFileStream storageFileStream = storeForApplication.OpenFile(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
              {
                MemoryStream destination = new MemoryStream();
                storageFileStream.CopyTo((Stream) destination);
                return destination.ToArray();
              }
            }
          }
        }
        catch (Exception ex)
        {
        }
label_13:
        return (byte[]) null;
      }), (Action<byte[]>) (bytes =>
      {
        try
        {
          using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
          {
            using (IsolatedStorageFileStream storageFileStream = storeForApplication.OpenFile(filename, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete))
              storageFileStream.Write(bytes, 0, bytes.Length);
          }
        }
        catch (Exception ex)
        {
        }
      }));
    }

    public static IObservable<byte[]> Cache(this IObservable<byte[]> source, string filename)
    {
      return source.Cache(filename, new DateTime?());
    }

    public static V GetValueOrDefault<K, V>(this Dictionary<K, V> dict, K key)
    {
      V valueOrDefault = default (V);
      dict.TryGetValue(key, out valueOrDefault);
      return valueOrDefault;
    }

    public static bool IsSorted<T>(this IEnumerable<T> that)
    {
      T y = default (T);
      bool flag = false;
      foreach (T x in that)
      {
        if (flag && Comparer<T>.Default.Compare(x, y) < 0)
          return false;
        y = x;
        flag = true;
      }
      return true;
    }

    public static string[] ParseLocArray(this string str)
    {
      string[] strArray = (string[]) null;
      if (!string.IsNullOrEmpty(str))
        strArray = str.Substring(1).Split(str[0]);
      return strArray ?? new string[0];
    }

    public static void Shuffle<T>(this T[] arr)
    {
      Random random = new Random();
      for (int index1 = arr.Length - 1; index1 > 0; --index1)
      {
        int index2 = random.Next(index1 + 1);
        T obj = arr[index1];
        arr[index1] = arr[index2];
        arr[index2] = obj;
      }
    }

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> items)
    {
      T[] array = items.ToArray<T>();
      array.Shuffle<T>();
      return (IEnumerable<T>) array;
    }

    public static void BeginInvokeIfNeeded(this Dispatcher dispatcher, Action a)
    {
      if (dispatcher.CheckAccess())
        a();
      else
        dispatcher.BeginInvoke(a);
    }

    public static void InvokeSynchronous(Action<Action> perform, Action action)
    {
      Action throwAction = (Action) null;
      using (ManualResetEvent ev = new ManualResetEvent(false))
      {
        perform((Action) (() =>
        {
          try
          {
            action();
          }
          catch (Exception ex)
          {
            throwAction = ex.GetRethrowAction();
            Log.LogException(ex, "invoke synchronous");
          }
          finally
          {
            ev.Set();
          }
        }));
        ev.WaitOne();
      }
      if (throwAction == null)
        return;
      throwAction();
    }

    public static void InvokeSynchronous(this Dispatcher dispatcher, Action action)
    {
      if (dispatcher.CheckAccess())
        action();
      else
        Extensions.InvokeSynchronous((Action<Action>) (a => dispatcher.BeginInvoke(a)), action);
    }

    public static void InvokeSynchronous(this IScheduler sched, Action action)
    {
      Extensions.InvokeSynchronous((Action<Action>) (a => sched.Schedule(a)), action);
    }

    public static void RunAfterDelay(this Dispatcher dispatcher, TimeSpan delay, Action a)
    {
      PooledTimer.Instance.Schedule(delay, (Action) (() => dispatcher.BeginInvoke(a)));
    }

    public static IEnumerable<uint> ToUtf32(this string chars) => chars.ToUtf32(0, chars.Length);

    public static IEnumerable<uint> ToUtf32(this string chars, int startIndex, int length)
    {
      int i = startIndex;
      while (i < startIndex + length)
      {
        char ch = chars[i++];
        if (ch < '\uD800' || ch >= '\uE000')
        {
          yield return (uint) ch;
        }
        else
        {
          uint num1 = (uint) ch;
          if (((int) num1 & 64512) != 55296)
            throw new InvalidOperationException("1st char in surrogate pair - Unexpected char " + num1.ToString("x"));
          uint num2 = (uint) (((int) num1 & 1023) << 10);
          uint num3 = (uint) chars[i++];
          if (((int) num3 & 64512) != 56320)
            throw new InvalidCastException("2nd char in surrogate pair - unexpected char " + num3.ToString("x"));
          yield return (uint) (((int) num2 | (int) num3 & 1023) + 65536);
        }
      }
    }

    public static int GetUtf32Length(this string str) => str.GetUtf32Length(0, str.Length);

    public static int GetUtf32Length(this string str, int startIndex, int length)
    {
      try
      {
        return str.ToUtf32(startIndex, length).Count<uint>();
      }
      catch (Exception ex)
      {
        return str.Length;
      }
    }

    public static int GetRealCharLength(this string str)
    {
      if (string.IsNullOrEmpty(str))
        return 0;
      str = NativeInterfaces.Misc.NormalizeUnicodeString(str, false);
      IEnumerable<LinkDetector.Result> emojiMatches = LinkDetector.GetEmojiMatches(str);
      int startIndex = 0;
      int realCharLength = 0;
      foreach (LinkDetector.Result result in emojiMatches)
      {
        if (result.Index + result.Length <= str.Length)
        {
          if (Emoji.Mappings(result.Value) != null)
          {
            ++realCharLength;
            if (result.Index - startIndex > 0)
              realCharLength += str.GetUtf32Length(startIndex, result.Index - startIndex);
            startIndex = result.Index + result.Length;
          }
        }
        else
          break;
      }
      if (str.Length - startIndex > 0)
        realCharLength += str.GetUtf32Length(startIndex, str.Length - startIndex);
      return realCharLength;
    }

    public static string MaybeTruncateToMaxRealCharLength(this string inputStr, int maxLength)
    {
      try
      {
        if (string.IsNullOrEmpty(inputStr) || inputStr.Length <= maxLength)
          return inputStr;
        string str = NativeInterfaces.Misc.NormalizeUnicodeString(inputStr, false);
        if (str.Length <= maxLength)
          return inputStr;
        IEnumerable<LinkDetector.Result> emojiMatches = LinkDetector.GetEmojiMatches(str);
        int startIndex = 0;
        int num = 0;
        foreach (LinkDetector.Result result in emojiMatches)
        {
          if (result.Index + result.Length <= str.Length)
          {
            if (Emoji.Mappings(result.Value) != null)
            {
              ++num;
              if (result.Index - startIndex > 0)
                num += str.GetUtf32Length(startIndex, result.Index - startIndex);
              startIndex = result.Index + result.Length;
            }
            if (num >= maxLength)
              break;
          }
          else
            break;
        }
        if (num < maxLength && str.Length - startIndex > 0)
        {
          num += str.GetUtf32Length(startIndex, str.Length - startIndex);
          startIndex = str.Length;
        }
        if (num >= maxLength)
        {
          string maxRealCharLength = str.Substring(0, startIndex - (num - maxLength));
          Log.l("Truncated", "Input {0}, Normalized {1}, Max {2}", (object) inputStr.Length, (object) str.Length, (object) maxLength);
          return maxRealCharLength;
        }
      }
      catch (Exception ex)
      {
        string context = "Exception truncating string " + (inputStr ?? "").Length.ToString() + " " + maxLength.ToString();
        Log.LogException(ex, context);
      }
      return inputStr;
    }

    public static string ExtractDigits(this string str)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (str != null)
      {
        foreach (char c in str)
        {
          if (c >= '0' && c <= '9')
            stringBuilder.Append(c);
          else if (c >= '٠' && c <= '٩')
            stringBuilder.Append((char) (48 + ((int) c - 1632)));
          else if (c >= '۰' && c <= '۹')
            stringBuilder.Append((char) (48 + ((int) c - 1776)));
          else if (char.IsDigit(c))
            stringBuilder.Append(c);
        }
      }
      return stringBuilder.ToString();
    }

    public static string ToLangFriendlyLower(this string str)
    {
      if (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "de")
        return str;
      return str?.ToLower();
    }

    public static string ExtractFileExtension(this string str)
    {
      string fileExtension = (string) null;
      int num;
      if (!string.IsNullOrEmpty(str) && (num = str.LastIndexOf('.')) > 0)
        fileExtension = str.Substring(num + 1);
      return fileExtension;
    }

    public static bool TryGetValue(
      this WebHeaderCollection headers,
      string key,
      out string valueOut)
    {
      bool flag = false;
      string str = (string) null;
      try
      {
        str = headers[key];
        flag = str != null;
      }
      catch (Exception ex)
      {
      }
      valueOut = str;
      return flag;
    }

    public static Visibility ToVisibility(this bool value)
    {
      return !value ? Visibility.Collapsed : Visibility.Visible;
    }

    public static IEnumerable<T> MakeUnique<T>(this IEnumerable<T> source)
    {
      return source.MakeUnique<T, T>((Func<T, T>) (key => key));
    }

    public static IEnumerable<T> MakeUnique<T, U>(this IEnumerable<T> source, Func<T, U> makeKey)
    {
      Dictionary<U, bool> dict = new Dictionary<U, bool>();
      return source.Where<T>((Func<T, bool>) (item =>
      {
        U key = makeKey(item);
        int num = dict.ContainsKey(key) ? 1 : 0;
        if (num == 0)
          dict.Add(key, true);
        return num == 0;
      }));
    }

    public static void RemoveFirst<T>(this IList<T> list, Func<T, bool> pred)
    {
      if (list == null || pred == null)
        return;
      int index = 0;
      for (int count = list.Count; index < count; ++index)
      {
        if (pred(list[index]))
        {
          list.RemoveAt(index);
          break;
        }
      }
    }

    public static void RemoveWhere<T>(this IList<T> list, Func<T, bool> pred)
    {
      if (list == null || pred == null)
        return;
      int index = 0;
      for (int count = list.Count; index < count; ++index)
      {
        if (pred(list[index]))
        {
          list.RemoveAt(index);
          --index;
          --count;
        }
      }
    }

    public static System.Net.WebRequest ToGetRequest(this Uri that)
    {
      System.Net.WebRequest getRequest = System.Net.WebRequest.Create(that.IdnToAsciiAbsoluteUriString());
      getRequest.Headers[HttpRequestHeader.UserAgent] = AppState.GetUserAgent();
      return getRequest;
    }

    public static string IdnToAsciiAbsoluteUriString(this Uri that)
    {
      if (that == (Uri) null)
        return (string) null;
      string host = that.Host;
      string absoluteUri = that.AbsoluteUri;
      try
      {
        string ascii = NativeInterfaces.Misc.IdnToAscii(0U, host);
        if (!string.IsNullOrEmpty(ascii))
        {
          if (ascii != host)
          {
            int length = absoluteUri.IndexOf(host);
            string absoluteUriString = absoluteUri.Substring(0, length) + ascii;
            if (absoluteUri.Length > length + host.Length)
              absoluteUriString += absoluteUri.Substring(length + host.Length);
            Log.d("punycode", "converted {0} to {1}", (object) host, (object) ascii);
            return absoluteUriString;
          }
        }
      }
      catch (Exception ex)
      {
        string context = "Exception converting to punycode " + absoluteUri;
        Log.LogException(ex, context);
      }
      return absoluteUri;
    }

    public static string IdnToAsciiAbsoluteUriString(this string uriString)
    {
      if (uriString == null)
        return (string) null;
      try
      {
        return new Uri(uriString, UriKind.Absolute).IdnToAsciiAbsoluteUriString();
      }
      catch (Exception ex)
      {
        string context = "Exception converting to punycode " + uriString;
        Log.LogException(ex, context);
      }
      return (string) null;
    }

    public static string IdnToUnicodeAbsoluteUriString(this Uri that)
    {
      if (that == (Uri) null)
        return (string) null;
      string host = that.Host;
      string absoluteUri = that.AbsoluteUri;
      try
      {
        string unicode = NativeInterfaces.Misc.IdnToUnicode(0U, host);
        if (!string.IsNullOrEmpty(unicode))
        {
          if (unicode != host)
          {
            int length = absoluteUri.IndexOf(host);
            string absoluteUriString = absoluteUri.Substring(0, length) + unicode;
            if (absoluteUri.Length > length + host.Length)
              absoluteUriString += absoluteUri.Substring(length + host.Length);
            Log.d("punycode", "converted from {0} to {1}", (object) host, (object) unicode);
            return absoluteUriString;
          }
        }
      }
      catch (Exception ex)
      {
        string context = "Exception converting from punycode " + absoluteUri;
        Log.LogException(ex, context);
      }
      return absoluteUri;
    }

    public static string IdnToUnicodeAbsoluteUriString(this string uriString)
    {
      if (string.IsNullOrEmpty(uriString))
        return (string) null;
      try
      {
        return new Uri(uriString, UriKind.Absolute).IdnToUnicodeAbsoluteUriString();
      }
      catch (Exception ex)
      {
        string context = "Exception converting from punycode " + uriString;
        Log.LogException(ex, context);
      }
      return (string) null;
    }

    public static void SafeDispose(this IDisposable d)
    {
      if (d == null)
        return;
      try
      {
        d.Dispose();
      }
      catch (Exception ex)
      {
        Log.SendCrashLog(ex, nameof (SafeDispose));
      }
    }

    public static string ToFriendlyString(this TimeSpan delta)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (delta >= TimeSpan.FromHours(1.0))
      {
        stringBuilder.Append((int) delta.TotalHours);
        stringBuilder.Append(':');
      }
      int num = (int) delta.TotalMinutes % 60;
      if (stringBuilder.Length != 0)
        stringBuilder.Append(num.ToString().PadLeft(2, '0'));
      else
        stringBuilder.Append(num.ToString());
      stringBuilder.Append(':');
      stringBuilder.Append(((int) delta.TotalSeconds % 60).ToString().PadLeft(2, '0'));
      return stringBuilder.ToString();
    }

    public static IEnumerable<T> Cast<T>(this IEnumerable nonGenericSource)
    {
      foreach (T obj in nonGenericSource)
        yield return obj;
    }

    public static void WaitOne(this ManualResetEventSlim ev) => ev.Wait();

    public static IObservable<T> DistinctUntilChanged<T>(
      this IObservable<T> source,
      Func<T, T, bool> eq)
    {
      T cached = default (T);
      bool hasCached = false;
      object gate = new object();
      return source.Synchronize<T>(gate).Where<T>((Func<T, bool>) (value =>
      {
        int num = !hasCached ? 1 : (!eq(cached, value) ? 1 : 0);
        cached = value;
        hasCached = true;
        return num != 0;
      }));
    }

    public static bool IsEqualBytes(byte[] a, int aoff, int alen, byte[] b, int boff, int blen)
    {
      if (alen != blen)
        return false;
      bool flag = true;
      int num = 0;
      for (int index = Math.Min(alen, blen); num < index; ++num)
      {
        if ((int) a[num + aoff] != (int) b[num + boff])
          flag = false;
      }
      return flag;
    }

    public static bool IsEqualBytes(this byte[] a, byte[] b)
    {
      return a == null || b == null ? a == b : Extensions.IsEqualBytes(a, 0, a.Length, b, 0, b.Length);
    }

    public static string ToWebClientBool(this bool b) => !b ? "false" : "true";

    public static string ToWebClientBool(this bool? b) => b.GetValueOrDefault().ToWebClientBool();

    public static string ReplaceLineBreaksWithSpaces(this string s)
    {
      return s?.Replace(Environment.NewLine, " ").Replace("\n", " ").Replace("\r", " ");
    }

    private class WAScheduler : IWAScheduler
    {
      private IScheduler sched;

      public WAScheduler(IScheduler sched) => this.sched = sched;

      public void Schedule(IAction waAction)
      {
        this.sched.Schedule((Action) (() => this.Perform(waAction)));
      }

      private void Perform(IAction waAction)
      {
        try
        {
          waAction.Perform();
        }
        finally
        {
          if (Marshal.IsComObject((object) waAction))
            Marshal.ReleaseComObject((object) waAction);
        }
      }
    }

    private class WAAction : IAction
    {
      private Action a;

      public WAAction(Action a) => this.a = a;

      public void Perform() => this.a();
    }

    public class Grouping<T, U> : List<U>
    {
      private T t;
      private IEnumerable<U> uEnumerable;

      public Grouping(T t, IEnumerable<U> u)
        : base(u)
      {
        this.t = t;
        this.uEnumerable = u;
      }

      public T Key => this.t;

      public override bool Equals(object obj)
      {
        return obj is Extensions.Grouping<T, U> grouping && this.t.Equals((object) grouping.t);
      }
    }
  }
}
