// Decompiled with JetBrains decompiler
// Type: WhatsApp.EmojiSearch
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using WhatsAppNative;


namespace WhatsApp
{
  public class EmojiSearch
  {
    public static readonly string FB_TOP_50 = "fb-top-50";
    public static readonly string LANGUAGES_TAG = "languages";
    private static string LogHeader = nameof (EmojiSearch);
    private static string SERVER_ENDPOINT = "https://static.whatsapp.net/emoji?lgs=";
    private static string TOP_50 = "&top=1";
    private static string TOP_EMOJI_SEPARATOR = ",";
    public static long TIME_BETWEEN_FETCHES_TICKS = 36000000000;
    public static int STALENESS_THRESHOLD_DAYS = 7;
    public static long STALENESS_THRESHOLD_TICKS = (long) EmojiSearch.STALENESS_THRESHOLD_DAYS * 864000000000L;
    private static object createLock = new object();
    private static EmojiSearch _emojiSearchInstance;
    private Dictionary<string, SqliteEmojiSearch.EmojiSearchStatusInfo> emojiSearchDictionary = new Dictionary<string, SqliteEmojiSearch.EmojiSearchStatusInfo>();
    private static WorkQueue requestLanguageThread;
    private List<string> inProgressLanguages = new List<string>();

    public static EmojiSearch GetInstance()
    {
      if (EmojiSearch._emojiSearchInstance == null)
      {
        lock (EmojiSearch.createLock)
        {
          if (EmojiSearch._emojiSearchInstance == null)
          {
            string lang;
            CultureInfo.CurrentUICulture.GetLangAndLocale(out lang, out string _);
            EmojiSearch._emojiSearchInstance = EmojiSearch.CreateSearchInstance(lang);
          }
        }
      }
      return EmojiSearch._emojiSearchInstance;
    }

    public List<string> GetMatchingEmoji(string language, string searchTerm, bool exactMatch = false)
    {
      List<string> matchingEmoji = new List<string>();
      if (this.IsEmojiSearchSupportedForLanguage(language))
      {
        EmojiSearch.MaybeRemoveLocaleFromLanguageString(language);
        matchingEmoji = SqliteEmojiSearch.GetMatchingFromTable(searchTerm, 100, exactMatch);
      }
      return matchingEmoji;
    }

    public List<string> GetTopEmoji()
    {
      List<string> topEmoji = new List<string>();
      SqliteEmojiSearch.EmojiSearchStatusInfo searchStatusInfo = (SqliteEmojiSearch.EmojiSearchStatusInfo) null;
      if (this.emojiSearchDictionary.TryGetValue(EmojiSearch.FB_TOP_50, out searchStatusInfo) && !string.IsNullOrEmpty(searchStatusInfo.TopEmojiString))
      {
        foreach (string str in searchStatusInfo.TopEmojiString.Split(EmojiSearch.TOP_EMOJI_SEPARATOR.ToCharArray()))
          topEmoji.Add(str);
      }
      return topEmoji;
    }

    public bool IsEmojiSearchSupportedForLanguage(string inputLanguageString)
    {
      string key = EmojiSearch.MaybeRemoveLocaleFromLanguageString(inputLanguageString);
      Log.d(EmojiSearch.LogHeader, "checking emoji search for {0} {1}", (object) (inputLanguageString ?? "null"), (object) (key ?? "null"));
      if (string.IsNullOrEmpty(key) || !SqliteEmojiSearch.HasEmojiSearchDb())
        return false;
      if (this.inProgressLanguages.Count > 0 && this.inProgressLanguages.Contains(key))
        Log.l(EmojiSearch.LogHeader, "Processing a search while update in progress for {0}", (object) key);
      SqliteEmojiSearch.EmojiSearchStatusInfo statusInfo = (SqliteEmojiSearch.EmojiSearchStatusInfo) null;
      if (this.emojiSearchDictionary.TryGetValue(key, out statusInfo))
      {
        if (statusInfo.LanguageState == SqliteEmojiSearch.EmojiLanguageState.UPTO_DATE)
          return true;
        if (statusInfo.LanguageState == SqliteEmojiSearch.EmojiLanguageState.CACHED_STALE)
        {
          Log.d(EmojiSearch.LogHeader, "Requesting {0} because existing data is stale", (object) statusInfo.EmojiLanguage);
          this.RequestLanguage(statusInfo);
          return true;
        }
        if (statusInfo.ShouldRequestUpdate())
          this.RequestLanguage(statusInfo);
        Log.d(EmojiSearch.LogHeader, "Emoji search not available {0}", (object) statusInfo.LanguageState);
        return false;
      }
      Log.l(EmojiSearch.LogHeader, "Language not previously requested - requesting now {0}", (object) key);
      statusInfo = new SqliteEmojiSearch.EmojiSearchStatusInfo()
      {
        EmojiLanguage = key,
        FetchState = SqliteEmojiSearch.EmojiLanguageFetchState.FETCHING,
        LastUsedDate = DateTime.UtcNow,
        LastFetchedTimestamp = new DateTime?(),
        LastRequestETag = (string) null
      };
      this.emojiSearchDictionary[statusInfo.EmojiLanguage] = statusInfo;
      this.RequestLanguage(statusInfo);
      return false;
    }

    private static string MaybeRemoveLocaleFromLanguageString(string languageString)
    {
      string str = languageString;
      if (string.IsNullOrEmpty(str))
        return (string) null;
      int length1 = str.IndexOf("-");
      if (length1 > -1)
      {
        if (length1 != 2 && length1 != 3)
          return (string) null;
        str = str.Substring(0, length1);
      }
      int length2 = str.IndexOf("_");
      if (length2 > -1)
      {
        if (length2 != 2 && length2 != 3)
          return (string) null;
        str = str.Substring(0, length2);
      }
      return str.Length == 2 || str.Length == 3 ? str : (string) null;
    }

    private static EmojiSearch CreateSearchInstance(string systemLang)
    {
      Log.d(EmojiSearch.LogHeader, "Creating a new emoji search instance for lang={0}", (object) systemLang);
      EmojiSearch searchInstance = new EmojiSearch();
      searchInstance.emojiSearchDictionary = new Dictionary<string, SqliteEmojiSearch.EmojiSearchStatusInfo>();
      try
      {
        foreach (SqliteEmojiSearch.EmojiSearchStatusInfo statusInfo in SqliteEmojiSearch.GetEmojiSearchStatusInfo())
        {
          if (statusInfo.ShouldRequestUpdate())
            searchInstance.RequestLanguage(statusInfo);
          searchInstance.emojiSearchDictionary[statusInfo.EmojiLanguage] = statusInfo;
        }
        if (searchInstance.emojiSearchDictionary.ContainsKey(systemLang))
        {
          if (searchInstance.emojiSearchDictionary.ContainsKey(EmojiSearch.FB_TOP_50))
            goto label_12;
        }
        SqliteEmojiSearch.EmojiSearchStatusInfo statusInfo1 = new SqliteEmojiSearch.EmojiSearchStatusInfo()
        {
          EmojiLanguage = systemLang,
          LastUsedDate = FunXMPP.UnixEpoch,
          LastFetchedTimestamp = new DateTime?(),
          LastRequestETag = (string) null
        };
        searchInstance.RequestLanguage(statusInfo1);
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "Exception retrieving Languages");
      }
label_12:
      return searchInstance;
    }

    private void AddLanguage(
      SqliteEmojiSearch.EmojiSearchStatusInfo statusInfo,
      Dictionary<string, string[]> emojiDictionary)
    {
      SqliteEmojiSearch.AddLanguageToDatabase(statusInfo, emojiDictionary);
      lock (EmojiSearch.createLock)
        this.emojiSearchDictionary[statusInfo.EmojiLanguage] = statusInfo;
    }

    private static WorkQueue RequestLanguageThread
    {
      get
      {
        return Utils.LazyInit<WorkQueue>(ref EmojiSearch.requestLanguageThread, (Func<WorkQueue>) (() => new WorkQueue()));
      }
    }

    private void RequestLanguage(SqliteEmojiSearch.EmojiSearchStatusInfo statusInfo)
    {
      Log.d(EmojiSearch.LogHeader, "Requesting {0}", (object) statusInfo.EmojiLanguage);
      if (statusInfo.EmojiLanguage == EmojiSearch.FB_TOP_50)
        return;
      if (this.inProgressLanguages.Contains(statusInfo.EmojiLanguage))
        Log.l(EmojiSearch.LogHeader, "Update for {0} skipped - in progress", (object) statusInfo.EmojiLanguage);
      else if (!NetworkStateMonitor.IsDataConnected())
      {
        Log.l(EmojiSearch.LogHeader, "Update for {0} skipped - no network", (object) statusInfo.EmojiLanguage);
      }
      else
      {
        this.inProgressLanguages.Add(statusInfo.EmojiLanguage);
        EmojiSearch.RequestLanguageThread.Enqueue((Action) (() =>
        {
          Dictionary<string, string> dictionary1 = new Dictionary<string, string>();
          dictionary1.Add("Content-Type", "application/json");
          dictionary1.Add("Accept-Encoding", "gzip");
          if (!string.IsNullOrEmpty(statusInfo.LastRequestETag))
            dictionary1.Add("If-None-Match", statusInfo.LastRequestETag);
          StringBuilder stringBuilder = new StringBuilder();
          foreach (KeyValuePair<string, string> keyValuePair in dictionary1)
          {
            if (stringBuilder.Length > 0)
              stringBuilder.Append("\r\n");
            stringBuilder.Append(string.Format("{0}: {1}", (object) keyValuePair.Key, (object) string.Join(" ", new string[1]
            {
              keyValuePair.Value
            })));
          }
          string headers = stringBuilder.ToString();
          EmojiSearch.GetEmojiSearchLanguagePack(EmojiSearch.SERVER_ENDPOINT + statusInfo.EmojiLanguage + EmojiSearch.TOP_50, headers).Subscribe<EmojiSearch.EmojiDictDownloadDetails>((Action<EmojiSearch.EmojiDictDownloadDetails>) (downloadDetails =>
          {
            int responseCode = downloadDetails.ResponseCode;
            string responseEncoding = downloadDetails.ResponseEncoding;
            string etag = downloadDetails.ETag;
            Stream stream = (Stream) downloadDetails.EmojiDetailsStream;
            Log.l(EmojiSearch.LogHeader, "processing response {0}, {1}, {2} {3}", (object) responseCode, (object) (responseEncoding ?? "null"), (object) (etag ?? "null"), (object) (stream != null));
            switch (responseCode)
            {
              case 200:
                if (stream != null)
                {
                  if (stream.Length != 0L)
                  {
                    try
                    {
                      stream.Position = 0L;
                      if (responseEncoding == "gzip")
                        stream = (Stream) stream.Gunzip();
                      JObject fromJsonStream = EmojiSearch.CreateFromJsonStream<JObject>(stream);
                      Dictionary<string, JObject> dictionary2 = fromJsonStream.GetValue(EmojiSearch.LANGUAGES_TAG)?.ToObject<Dictionary<string, JObject>>();
                      string[] strArray = fromJsonStream.GetValue(EmojiSearch.FB_TOP_50)?.ToObject<string[]>();
                      if (dictionary2 == null || strArray == null)
                      {
                        ArgumentException e = new ArgumentException("Unexpected null data");
                        Log.SendCrashLog((Exception) e, string.Format("no useful data returned from emoji request {0}", (object) statusInfo.EmojiLanguage), logOnlyForRelease: true);
                        throw e;
                      }
                      foreach (string key in dictionary2.Keys)
                      {
                        Dictionary<string, string[]> emojiDictionary = dictionary2[key].ToObject<Dictionary<string, string[]>>();
                        if (key == statusInfo.EmojiLanguage)
                        {
                          statusInfo.FetchState = SqliteEmojiSearch.EmojiLanguageFetchState.LAST_FETCH_OK;
                          statusInfo.LastFetchedTimestamp = new DateTime?(DateTime.UtcNow);
                          statusInfo.LastRequestETag = etag;
                          statusInfo.TopEmojiString = strArray != null ? string.Join(EmojiSearch.TOP_EMOJI_SEPARATOR, strArray) : (string) null;
                          this.AddLanguage(statusInfo, emojiDictionary);
                        }
                        else
                        {
                          Log.l(EmojiSearch.LogHeader, "Unexpected language in response {0}", (object) key);
                          this.AddLanguage(new SqliteEmojiSearch.EmojiSearchStatusInfo()
                          {
                            EmojiLanguage = key,
                            FetchState = SqliteEmojiSearch.EmojiLanguageFetchState.LAST_FETCH_OK,
                            LastUsedDate = DateTime.UtcNow,
                            LastFetchedTimestamp = new DateTime?(DateTime.UtcNow),
                            LastRequestETag = etag,
                            TopEmojiString = (string) null
                          }, emojiDictionary);
                        }
                      }
                      this.AddLanguage(new SqliteEmojiSearch.EmojiSearchStatusInfo()
                      {
                        EmojiLanguage = EmojiSearch.FB_TOP_50,
                        FetchState = SqliteEmojiSearch.EmojiLanguageFetchState.LAST_FETCH_OK,
                        LastUsedDate = DateTime.UtcNow,
                        LastFetchedTimestamp = new DateTime?(DateTime.UtcNow),
                        LastRequestETag = etag,
                        TopEmojiString = strArray != null ? string.Join(EmojiSearch.TOP_EMOJI_SEPARATOR, strArray) : (string) null
                      }, (Dictionary<string, string[]>) null);
                      return;
                    }
                    catch (Exception ex)
                    {
                      string context = string.Format("Exception processing language {0}", (object) statusInfo.EmojiLanguage);
                      Log.LogException(ex, context);
                      EmojiSearch.SaveFetchError(statusInfo, SqliteEmojiSearch.EmojiLanguageFetchState.NETWORK_ERROR_RETRY);
                      return;
                    }
                  }
                  else
                    break;
                }
                else
                  break;
              case 304:
                statusInfo.LastFetchedTimestamp = new DateTime?(DateTime.UtcNow);
                statusInfo.Store();
                return;
              case 404:
                EmojiSearch.SaveFetchError(statusInfo, SqliteEmojiSearch.EmojiLanguageFetchState.NOT_FOUND);
                return;
            }
            EmojiSearch.SaveFetchError(statusInfo, SqliteEmojiSearch.EmojiLanguageFetchState.NETWORK_ERROR_NO_RETRY);
          }), (Action<Exception>) (ex =>
          {
            Log.LogException(ex, string.Format("Exception getting language {0}", (object) statusInfo.EmojiLanguage));
            EmojiSearch.SaveFetchError(statusInfo, SqliteEmojiSearch.EmojiLanguageFetchState.NETWORK_ERROR_NO_RETRY);
            this.inProgressLanguages.Remove(statusInfo.EmojiLanguage);
          }), (Action) (() => this.inProgressLanguages.Remove(statusInfo.EmojiLanguage)));
        }));
      }
    }

    private static void SaveFetchError(
      SqliteEmojiSearch.EmojiSearchStatusInfo statusInfo,
      SqliteEmojiSearch.EmojiLanguageFetchState newFetchState)
    {
      try
      {
        statusInfo.FetchState = newFetchState;
        statusInfo.LastFetchedTimestamp = new DateTime?(DateTime.UtcNow);
        statusInfo.LastRequestETag = (string) null;
        statusInfo.Store();
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "Exception saving search emoji state info");
      }
    }

    public static T CreateFromJsonStream<T>(Stream stream)
    {
      JsonSerializer jsonSerializer = new JsonSerializer();
      using (stream)
      {
        using (StreamReader reader = new StreamReader(stream))
          return (T) jsonSerializer.Deserialize((TextReader) reader, typeof (T));
      }
    }

    private static IObservable<EmojiSearch.EmojiDictDownloadDetails> GetEmojiSearchLanguagePack(
      string url,
      string headers)
    {
      return NativeWeb.Create<EmojiSearch.EmojiDictDownloadDetails>(NativeWeb.Options.Default | NativeWeb.Options.KeepAlive, (Action<IWebRequest, IObserver<EmojiSearch.EmojiDictDownloadDetails>>) ((req, observer) =>
      {
        MemoryStream mem = new MemoryStream();
        int responseCode = -1;
        bool onCompletedFired = false;
        string responseETag = (string) null;
        string responseEncoding = (string) null;
        IWebRequest req1 = req;
        string url1 = url;
        NativeWeb.Callback callbackObject = new NativeWeb.Callback();
        callbackObject.OnBeginResponse = (Action<int, string>) ((code, returnedHeaders) =>
        {
          responseCode = code;
          foreach (KeyValuePair<string, string> header in NativeWeb.ParseHeaders(headers))
          {
            string key = header.Key;
            if (key == "ETag")
              responseETag = header.Value;
            if (key == "Accept-Encoding")
              responseEncoding = header.Value;
          }
          switch (code)
          {
            case 200:
              break;
            case 304:
            case 404:
              observer.OnNext(new EmojiSearch.EmojiDictDownloadDetails()
              {
                ResponseCode = responseCode,
                ResponseEncoding = (string) null,
                ETag = responseETag,
                EmojiDetailsStream = (MemoryStream) null
              });
              observer.OnCompleted();
              onCompletedFired = true;
              break;
            default:
              observer.OnError(new Exception(string.Format("GetEmojiSearchLanguagePack Unexpected response {0}", (object) code)));
              observer.OnCompleted();
              onCompletedFired = true;
              break;
          }
        });
        callbackObject.OnBytesIn = (Action<byte[]>) (b => mem.Write(b, 0, b.Length));
        callbackObject.OnEndResponse = (Action) (() =>
        {
          if (!onCompletedFired)
          {
            mem.Position = 0L;
            observer.OnNext(new EmojiSearch.EmojiDictDownloadDetails()
            {
              ResponseCode = responseCode,
              ResponseEncoding = responseEncoding,
              ETag = responseETag,
              EmojiDetailsStream = mem
            });
            observer.OnCompleted();
          }
          else
            mem.SafeDispose();
        });
        string headers1 = headers;
        req1.Open(url1, (IWebCallback) callbackObject, headers: headers1);
      }));
    }

    private class EmojiDictDownloadDetails
    {
      public int ResponseCode = -1;
      public string ETag;
      public string ResponseEncoding;
      public MemoryStream EmojiDetailsStream;
    }
  }
}
