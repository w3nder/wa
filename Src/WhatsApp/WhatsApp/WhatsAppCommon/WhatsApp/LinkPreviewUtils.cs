// Decompiled with JetBrains decompiler
// Type: WhatsApp.LinkPreviewUtils
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using WhatsApp.RegularExpressions;
using WhatsApp.WaCollections;
using WhatsAppNative;


namespace WhatsApp
{
  public static class LinkPreviewUtils
  {
    private const string LogHeader = "link preview";
    private const int MIN_ICON_SIZE = 100;
    public const int MAX_ICON_SIZE = 140;
    private static KeyValueCache<string, WebPageMetadata> cachedData = (KeyValueCache<string, WebPageMetadata>) null;
    private static KeyValueCache<string, LinkPreviewUtils.FetchStatus> prevFetches = (KeyValueCache<string, LinkPreviewUtils.FetchStatus>) null;
    private static KeyValueCache<string, Subject<WebPageMetadata>> pendingNotifiers = (KeyValueCache<string, Subject<WebPageMetadata>>) null;
    private static Regex headRegex = new Regex("<head>([\\s\\S]*)<\\/head>", WhatsApp.RegularExpressions.RegexOptions.IgnoreCase);
    private static Regex metaRegex = new Regex("<meta([^>]+?)/?>", WhatsApp.RegularExpressions.RegexOptions.IgnoreCase);
    private static Regex linkRegex = new Regex("<link([^>]+?)/?>", WhatsApp.RegularExpressions.RegexOptions.IgnoreCase);
    private static Regex keyValRegex = new Regex("\\s*([^=]+)\\s*=\\s*(\"([^\"]+)\"|'([^']+)')", WhatsApp.RegularExpressions.RegexOptions.IgnoreCase);
    private static Regex titleRegex = new Regex("<title>([\\s\\S]*)<\\/title>", WhatsApp.RegularExpressions.RegexOptions.IgnoreCase);
    private static readonly string GOOD_IRI_CHAR = "a-zA-Z0-9 -\uD7FF豈-\uFDCFﷰ-\uFFEF";
    private static readonly string IRI = "[" + LinkPreviewUtils.GOOD_IRI_CHAR + "](?:[" + LinkPreviewUtils.GOOD_IRI_CHAR + "\\-]{0,61}[" + LinkPreviewUtils.GOOD_IRI_CHAR + "]){0,1}";
    private static readonly string GOOD_GTLD_CHAR = "a-zA-Z -\uD7FF豈-\uFDCFﷰ-\uFFEF";
    private static readonly string GTLD = "[" + LinkPreviewUtils.GOOD_GTLD_CHAR + "]{2,63}";
    private static readonly string HOST_NAME = "(?:" + LinkPreviewUtils.IRI + "\\.)+(" + LinkPreviewUtils.GTLD + ")";
    private static readonly string TOP_LEVEL_DOMAIN_STR_FOR_WEB_URL = "(?:(?:aero|arpa|asia|a[cdefgilmnoqrstuwxz])|(?:biz|b[abdefghijmnorstvwyz])|(?:cat|com|coop|c[acdfghiklmnoruvxyz])|d[ejkmoz]|(?:edu|e[cegrstu])|f[ijkmor]|(?:gov|g[abdefghilmnpqrstuwy])|h[kmnrtu]|(?:info|int|i[delmnoqrst])|(?:jobs|j[emop])|k[eghimnprwyz]|l[abcikrstuvy]|(?:mil|mobi|museum|m[acdeghklmnopqrstuvwxyz])|(?:name|net|n[acefgilopruz])|(?:org|om)|(?:pro|p[aefghklmnrstwy])|qa|r[eosuw]|s[abcdeghijklmnortuvyz]|(?:tel|travel|t[cdfghjklmnoprtvwz])|u[agksyz]|v[aceginu]|w[fs]|(?:δοκιμή|испытание|рф|срб|טעסט|آزمایشی|إختبار|الاردن|الجزائر|السعودية|المغرب|امارات|بھارت|تونس|سورية|فلسطين|قطر|مصر|परीक्षा|भारत|ভারত|ਭਾਰਤ|ભારત|இந்தியா|இலங்கை|சிங்கப்பூர்|பரிட்சை|భారత్|ලංකා|ไทย|テスト|中国|中國|台湾|台灣|新加坡|测试|測試|香港|테스트|한국|xn\\-\\-0zwm56d|xn\\-\\-11b5bs3a9aj6g|xn\\-\\-3e0b707e|xn\\-\\-45brj9c|xn\\-\\-80akhbyknj4f|xn\\-\\-90a3ac|xn\\-\\-9t4b11yi5a|xn\\-\\-clchc0ea0b2g2a9gcd|xn\\-\\-deba0ad|xn\\-\\-fiqs8s|xn\\-\\-fiqz9s|xn\\-\\-fpcrj9c3d|xn\\-\\-fzc2c9e2c|xn\\-\\-g6w251d|xn\\-\\-gecrj9c|xn\\-\\-h2brj9c|xn\\-\\-hgbk6aj7f53bba|xn\\-\\-hlcj6aya9esc7a|xn\\-\\-j6w193g|xn\\-\\-jxalpdlp|xn\\-\\-kgbechtv|xn\\-\\-kprw13d|xn\\-\\-kpry57d|xn\\-\\-lgbbat1ad8j|xn\\-\\-mgbaam7a8h|xn\\-\\-mgbayh7gpa|xn\\-\\-mgbbh1a71e|xn\\-\\-mgbc0a9azcg|xn\\-\\-mgberp4a5d4ar|xn\\-\\-o3cw4h|xn\\-\\-ogbpf8fl|xn\\-\\-p1ai|xn\\-\\-pgbs0dh|xn\\-\\-s9brj9c|xn\\-\\-wgbh1c|xn\\-\\-wgbl6a|xn\\-\\-xkc2al3hye2a|xn\\-\\-xkc2dl3a5ee0h|xn\\-\\-yfro4i67o|xn\\-\\-ygbi2ammx|xn\\-\\-zckzah|xxx)|y[et]|z[amw]))";
    private static readonly Regex IP_ADDRESS = new Regex("(((25[0-5]|2[0-4][0-9]|[0-1][0-9]{2}|[1-9][0-9]|[1-9])\\.(25[0-5]|2[0-4][0-9]|[0-1][0-9]{2}|[1-9][0-9]|[1-9]|0)\\.(25[0-5]|2[0-4][0-9]|[0-1][0-9]{2}|[1-9][0-9]|[1-9]|0)\\.(25[0-5]|2[0-4][0-9]|[0-1][0-9]{2}|[1-9][0-9]|[0-9]))|([0-9a-fA-F]{0,4}\u00AD:){2,7}(\u00AD:|[0-9a-fA-F]{1,4}))");
    private static readonly Regex DOMAIN_NAME = new Regex("(" + LinkPreviewUtils.HOST_NAME + "|" + (object) LinkPreviewUtils.IP_ADDRESS + ")");
    public static readonly string WEB_URL_STRING = "((?:(http|https|Http|Https|rtsp|Rtsp):\\/\\/(?:(?:[a-zA-Z0-9\\$\\-\\_\\.\\+\\!\\*\\'\\(\\)\\,\\;\\?\\&\\=]|(?:\\%[a-fA-F0-9]{2})){1,64}(?:\\:(?:[a-zA-Z0-9\\$\\-\\_\\.\\+\\!\\*\\'\\(\\)\\,\\;\\?\\&\\=]|(?:\\%[a-fA-F0-9]{2})){1,25})?\\@)?)?(" + LinkPreviewUtils.HOST_NAME + ")(?:\\:\\d{1,5})?)(\\/(?:(?:[" + LinkPreviewUtils.GOOD_IRI_CHAR + "\\;\\/\\?\\:\\@\\&\\=\\#\\~\\-\\.\\+\\!\\*\\'\\(\\)\\,\\_])|(?:\\%[a-fA-F0-9]{2}))*)?";
    private static readonly Regex WEB_URL = new Regex(LinkPreviewUtils.WEB_URL_STRING);
    private static readonly string HOST_NAME_STRICT = "((?:(?:[" + LinkPreviewUtils.GOOD_IRI_CHAR + "][" + LinkPreviewUtils.GOOD_IRI_CHAR + "\\-]{0,64}\\.)+" + LinkPreviewUtils.TOP_LEVEL_DOMAIN_STR_FOR_WEB_URL + ")";
    private static readonly Regex WEB_URL_STRICT = new Regex("^((?:(http|https|Http|Https|rtsp|Rtsp):\\/\\/(?:(?:[a-zA-Z0-9\\$\\-\\_\\.\\+\\!\\*\\'\\(\\)\\,\\;\\?\\&\\=]|(?:\\%[a-fA-F0-9]{2})){1,64}(?:\\:(?:[a-zA-Z0-9\\$\\-\\_\\.\\+\\!\\*\\'\\(\\)\\,\\;\\?\\&\\=]|(?:\\%[a-fA-F0-9]{2})){1,25})?\\@)?)?" + LinkPreviewUtils.HOST_NAME_STRICT + "(?:\\:\\d{1,5})?)(\\/(?:(?:[" + LinkPreviewUtils.GOOD_IRI_CHAR + "\\;\\/\\?\\:\\@\\&\\=\\#\\~\\-\\.\\+\\!\\*\\'\\(\\)\\,\\_])|(?:\\%[a-fA-F0-9]{2}))*)?(?:\\b|$)");

    private static KeyValueCache<string, WebPageMetadata> CachedData
    {
      get
      {
        if (LinkPreviewUtils.cachedData == null)
          LinkPreviewUtils.cachedData = new KeyValueCache<string, WebPageMetadata>(10, true, true);
        return LinkPreviewUtils.cachedData;
      }
    }

    private static KeyValueCache<string, LinkPreviewUtils.FetchStatus> PrevFetches
    {
      get
      {
        if (LinkPreviewUtils.prevFetches == null)
          LinkPreviewUtils.prevFetches = new KeyValueCache<string, LinkPreviewUtils.FetchStatus>(200, true, true);
        return LinkPreviewUtils.prevFetches;
      }
    }

    private static KeyValueCache<string, Subject<WebPageMetadata>> PendingNotifiers
    {
      get
      {
        return LinkPreviewUtils.pendingNotifiers ?? (LinkPreviewUtils.pendingNotifiers = new KeyValueCache<string, Subject<WebPageMetadata>>(20, false, true));
      }
    }

    public static IObservable<WebPageMetadata> FetchWebPageMetadata(string uri, bool skipCache = false)
    {
      if (string.IsNullOrEmpty(uri))
        return Observable.Empty<WebPageMetadata>();
      LinkPreviewUtils.FetchStatus val1 = LinkPreviewUtils.FetchStatus.None;
      if (!skipCache && LinkPreviewUtils.PrevFetches.TryGet(uri, out val1))
      {
        switch (val1)
        {
          case LinkPreviewUtils.FetchStatus.InProgress:
            Subject<WebPageMetadata> val2 = (Subject<WebPageMetadata>) null;
            if (!LinkPreviewUtils.PendingNotifiers.TryGet(uri, out val2) || val2 == null)
            {
              val2 = new Subject<WebPageMetadata>();
              LinkPreviewUtils.PendingNotifiers.Set(uri, val2);
            }
            Log.d("link preview", "defer | uri:{0}", (object) uri);
            return (IObservable<WebPageMetadata>) val2;
          case LinkPreviewUtils.FetchStatus.Success:
            WebPageMetadata val3 = (WebPageMetadata) null;
            if (LinkPreviewUtils.CachedData.TryGet(uri, out val3) && val3 != null)
            {
              Log.d("link preview", "return cached success | uri:{0}", (object) uri);
              return Observable.Return<WebPageMetadata>(val3);
            }
            break;
          case LinkPreviewUtils.FetchStatus.Failed:
            Log.d("link preview", "skip previous failed uri | uri:{0}", (object) uri);
            return Observable.Empty<WebPageMetadata>();
        }
      }
      return Observable.Create<WebPageMetadata>((Func<IObserver<WebPageMetadata>, Action>) (observer =>
      {
        Log.d("link preview", "fetching | uri:{0}", (object) uri);
        Action<WebPageMetadata> notify = (Action<WebPageMetadata>) (arg =>
        {
          if (arg != null)
            observer.OnNext(arg);
          observer.OnCompleted();
          Subject<WebPageMetadata> val6 = (Subject<WebPageMetadata>) null;
          if (!LinkPreviewUtils.PendingNotifiers.TryGet(uri, out val6) || val6 == null)
            return;
          if (arg != null)
            val6.OnNext(arg);
          val6.OnCompleted();
          LinkPreviewUtils.PendingNotifiers.Remove(uri);
        });
        Action onFail = (Action) (() =>
        {
          Log.d("link preview", "fetch failed | uri:{0}", (object) uri);
          LinkPreviewUtils.PrevFetches.Set(uri, LinkPreviewUtils.FetchStatus.Failed);
          notify((WebPageMetadata) null);
        });
        Action<WebPageMetadata> onSuccess = (Action<WebPageMetadata>) (arg =>
        {
          Log.d("link preview", "fetch success | uri:{0}", (object) uri);
          LinkPreviewUtils.PrevFetches.Set(uri, LinkPreviewUtils.FetchStatus.Success);
          LinkPreviewUtils.CachedData.Set(uri, arg);
          notify(arg);
        });
        IDisposable reqSub = (IDisposable) null;
        try
        {
          DateTime? timeStart = PerformanceTimer.Start(PerformanceTimer.Mode.DebugAndBeta);
          LinkPreviewUtils.PrevFetches.Set(uri, LinkPreviewUtils.FetchStatus.InProgress);
          reqSub = LinkPreviewUtils.ReadHeadContent(uri).Subscribe<string>((Action<string>) (headContent =>
          {
            PerformanceTimer.End("fetched head content", timeStart);
            WebPageMetadata head = LinkPreviewUtils.ParseHead(uri, headContent);
            PerformanceTimer.End("parsed head content", timeStart);
            if (head == null)
              onFail();
            else
              onSuccess(head);
          }), (Action<Exception>) (exInner => onFail()));
        }
        catch (WebException ex)
        {
          onFail();
        }
        return (Action) (() =>
        {
          Log.d("link preview", "fetch attempt end | uri:{0}", (object) uri);
          LinkPreviewUtils.FetchStatus val7 = LinkPreviewUtils.FetchStatus.None;
          if (LinkPreviewUtils.PrevFetches.TryGet(uri, out val7) && val7 == LinkPreviewUtils.FetchStatus.InProgress)
            LinkPreviewUtils.PrevFetches.Set(uri, LinkPreviewUtils.FetchStatus.None);
          reqSub.SafeDispose();
          reqSub = (IDisposable) null;
        });
      }));
    }

    private static WebPageMetadata ParseHead(string uriStr, string headStr)
    {
      WebPageMetadata head = (WebPageMetadata) null;
      if (string.IsNullOrEmpty(headStr))
        return head;
      Dictionary<string, string> metadata = new Dictionary<string, string>();
      foreach (Match match in (IEnumerable<Match>) LinkPreviewUtils.metaRegex.Matches(headStr))
        LinkPreviewUtils.ParseMetaTag(match.Groups[1].Value, ref metadata);
      string str1 = LinkPreviewUtils.GetValueInOrder(metadata, "og:image", "twitter:image", "image", "thumbnail");
      if (string.IsNullOrEmpty(str1))
      {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        string str2 = (string) null;
        string str3 = (string) null;
        string str4 = (string) null;
        foreach (Match match in (IEnumerable<Match>) LinkPreviewUtils.linkRegex.Matches(headStr))
        {
          string str5 = match.Groups[1].Value;
          Dictionary<string, string> attributes = LinkPreviewUtils.ParseAttributes(str5);
          string str6 = (string) null;
          if (attributes.TryGetValue("rel", out str6) && !string.IsNullOrEmpty(str6))
          {
            if ("apple-touch-icon-precomposed".Equals(str6) || "apple-touch-icon".Equals(str6))
            {
              attributes.TryGetValue("href", out str2);
              if (str2 != null && LinkPreviewUtils.isSizeAttributeAcceptable(attributes, str5))
              {
                str1 = str2;
                break;
              }
            }
            if ("image_src".Equals(str6))
              attributes.TryGetValue("href", out str4);
            if ("icon".Equals(str6))
            {
              attributes.TryGetValue("href", out str3);
              if (LinkPreviewUtils.isSizeAttributeAcceptable(attributes, str5))
              {
                str1 = str3;
                break;
              }
            }
          }
        }
        if (string.IsNullOrEmpty(str1))
          str1 = str3 == null ? (str4 == null ? str2 : str4) : str3;
      }
      if (!string.IsNullOrEmpty(str1))
        str1 = HttpUtility.HtmlDecode(str1);
      if (!string.IsNullOrEmpty(str1) && !str1.StartsWith("http"))
      {
        Uri uri = new Uri(uriStr);
        str1 = !str1.StartsWith("//") ? string.Format("{0}://{1}{2}", (object) uri.Scheme, (object) uri.Host, (object) str1) : string.Format("{0}:{1}", (object) uri.Scheme, (object) str1);
      }
      if (!string.IsNullOrEmpty(str1))
      {
        try
        {
          Uri uri = new Uri(str1);
        }
        catch (Exception ex)
        {
          string context = "Exception creating Uri from thumb url: " + str1;
          Log.LogException(ex, context);
          str1 = (string) null;
        }
      }
      string html1 = LinkPreviewUtils.GetValueInOrder(metadata, "og:title", "twitter:title", "title");
      if (string.IsNullOrEmpty(html1))
      {
        Match match = LinkPreviewUtils.titleRegex.Match(headStr);
        if (match.Success)
          html1 = match.Groups[1].Value.Trim();
      }
      if (!string.IsNullOrEmpty(html1))
        html1 = HttpUtility.HtmlDecode(html1);
      string html2 = LinkPreviewUtils.GetValueInOrder(metadata, "og:description", "twitter:description", "description");
      if (!string.IsNullOrEmpty(html2))
        html2 = HttpUtility.HtmlDecode(html2);
      string valueInOrder = LinkPreviewUtils.GetValueInOrder(metadata, "og:url", "twitter:url");
      if (html1 != null || html2 != null)
        head = new WebPageMetadata()
        {
          Description = html2,
          OriginalUrl = uriStr,
          Title = html1,
          CanonicalUrl = valueInOrder,
          ThumbnailUrl = str1
        };
      return head;
    }

    private static bool isSizeAttributeAcceptable(
      Dictionary<string, string> attributes,
      string linkAttr)
    {
      string str = (string) null;
      attributes.TryGetValue("sizes", out str);
      if (str != null)
      {
        try
        {
          string[] strArray = str.Split('x');
          if (strArray.Length == 2)
          {
            if (Math.Min(Convert.ToInt32(strArray[0]), Convert.ToInt32(strArray[1])) >= 100)
              return true;
          }
        }
        catch (Exception ex)
        {
          string context = "Exception processing sizes attribute: " + linkAttr;
          Log.LogException(ex, context);
        }
      }
      return false;
    }

    private static string GetValueInOrder(Dictionary<string, string> dict, params string[] keys)
    {
      string str = (string) null;
      foreach (string key in keys)
      {
        if (dict.TryGetValue(key, out str) && !string.IsNullOrEmpty(str))
          break;
      }
      return str?.Trim();
    }

    private static void ParseMetaTag(string s, ref Dictionary<string, string> metadata)
    {
      Dictionary<string, string> attributes = LinkPreviewUtils.ParseAttributes(s);
      if (attributes == null || !attributes.Any<KeyValuePair<string, string>>() || metadata == null)
        return;
      string str1 = (string) null;
      if (attributes.TryGetValue("class", out str1))
        return;
      string key = (string) null;
      if (!attributes.TryGetValue("property", out key) && !attributes.TryGetValue("name", out key) || string.IsNullOrEmpty(key))
        return;
      key = key.ToLower();
      if (!("og:image" == key) && !("image" == key) && !("thumbnail" == key) && !("twitter:image" == key) && !("og:title" == key) && !("title" == key) && !("twitter:title" == key) && !("og:description" == key) && !("description" == key) && !("twitter:description" == key) && !("og:url" == key) && !("twitter:url" == key))
        return;
      string str2 = (string) null;
      if (!attributes.TryGetValue("content", out str2) || string.IsNullOrEmpty(str2))
        return;
      metadata[key] = str2.Trim();
    }

    private static Dictionary<string, string> ParseAttributes(string s)
    {
      Dictionary<string, string> attributes = new Dictionary<string, string>();
      if (string.IsNullOrEmpty(s))
        return attributes;
      foreach (Match match in (IEnumerable<Match>) LinkPreviewUtils.keyValRegex.Matches(s))
      {
        string key = match.Groups[1].Value;
        if (!string.IsNullOrEmpty(key))
        {
          string str = match.Groups[3].Value;
          if (string.IsNullOrEmpty(str))
            str = match.Groups[4].Value;
          attributes[key] = str;
        }
      }
      return attributes;
    }

    public static IObservable<string> ReadHeadContent(string url)
    {
      return NativeWeb.Create<string>(NativeWeb.Options.CacheDns, (Action<IWebRequest, IObserver<string>>) ((req, observer) =>
      {
        StringBuilder sb = new StringBuilder();
        string head = (string) null;
        req.SetIgnoreCertificateRevocations(true);
        try
        {
          IWebRequest req1 = req;
          string url1 = url;
          NativeWeb.Callback callbackObject = new NativeWeb.Callback();
          callbackObject.OnBytesIn = (Action<byte[]>) (bytes =>
          {
            sb.Append(Encoding.UTF8.GetString(bytes, 0, bytes.Length));
            Match match = LinkPreviewUtils.headRegex.Match(sb.ToString());
            if (!match.Success)
              return;
            head = match.Groups[1].Value;
            observer.OnNext(head);
            req.Cancel();
          });
          callbackObject.OnBeginResponse = (Action<int, string>) ((code, headers) =>
          {
            if (code == 200)
              return;
            observer.OnError(new Exception("Unexpected link preview response code: " + (object) code));
            req.Cancel();
          });
          callbackObject.OnEndResponse = (Action) (() =>
          {
            if (head != null)
              return;
            observer.OnNext(sb.ToString());
          });
          string previewUserAgent = AppState.GetUrlPreviewUserAgent();
          req1.Open(url1, (IWebCallback) callbackObject, userAgent: previewUserAgent);
        }
        catch (Exception ex)
        {
          Log.l("link preview", "Exception attempting to get link preview data: {0}", (object) ex.ToString());
          observer.OnError(new Exception("Unexpected link preview request exception"));
          req.Cancel();
        }
      }));
    }

    public static string GetLink(string text) => LinkPreviewUtils.GetLinkMatch(text);

    private static string GetLinkMatch(string text)
    {
      if (string.IsNullOrEmpty(text))
        return (string) null;
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      foreach (Match match in (IEnumerable<Match>) LinkPreviewUtils.WEB_URL.Matches(text))
      {
        int index = match.Index;
        if (index <= 0 || text[index - 1] != '@')
          return match.Groups[0].Value;
      }
      return (string) null;
    }

    private enum FetchStatus
    {
      None,
      InProgress,
      Success,
      Failed,
    }
  }
}
