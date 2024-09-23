// Decompiled with JetBrains decompiler
// Type: WhatsApp.GifProviderGiphy
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Windows.Media.Imaging;


namespace WhatsApp
{
  public class GifProviderGiphy : IGifProvider
  {
    private const string TRENDING_QUERY = "trending";
    private string CurrentQuery = "trending";
    private int CurrentOffset;
    private static byte[] cachedFirstTrendingResult = (byte[]) null;
    private int cachedFirstTrendingCurrentOffset;
    private static long cachedTrendingResultValidTillTicks = 0;
    private static WebServices.Attribution attribution = new WebServices.Attribution()
    {
      Text = "Powered by Giphy",
      Logo = (BitmapImage) null
    };
    private static string langParm = (string) null;
    private static string[] SUPPORTED_LANGUAGES = new string[31]
    {
      "es",
      "pt",
      "id",
      "fr",
      "ar",
      "tr",
      "th",
      "vi",
      "de",
      "it",
      "ja",
      "zh-CN",
      "zh-TW",
      "ru",
      "ko",
      "pl",
      "nl",
      "ro",
      "hu",
      "sv",
      "cs",
      "hi",
      "bn",
      "da",
      "fa",
      "tl",
      "fi",
      "iw",
      "ms",
      "no",
      "uk"
    };

    private static string Key => "AEvTMDR8cqzEk";

    public IObservable<GifSearchResult> GifSearch(string query)
    {
      this.CurrentOffset = 0;
      return this.GifSearch(query, this.CurrentOffset);
    }

    private IObservable<GifSearchResult> GifSearch(string query, int offset)
    {
      string uri = string.Format("https://api.giphy.com/v1/gifs/search?q={0}&api_key={1}&limit={2}&offset={3}{4}&rating=pg-13", (object) query, (object) GifProviderGiphy.Key, (object) 12, (object) offset, (object) GifProviderGiphy.LangParam());
      this.CurrentQuery = query;
      return Observable.Defer<GifSearchResult>((Func<IObservable<GifSearchResult>>) (() => Observable.Defer<byte[]>((Func<IObservable<byte[]>>) (() => new Uri(uri, UriKind.Absolute).ToGetRequest().GetResponseBytesAync())).SelectMany<byte[], GifSearchResult, GifSearchResult>((Func<byte[], IObservable<GifSearchResult>>) (bytes => this.InterpretGifResults(bytes)), (Func<byte[], GifSearchResult, GifSearchResult>) ((bytes, res) => res))));
    }

    public IObservable<GifSearchResult> TrendingGifs()
    {
      this.CurrentOffset = 0;
      return this.TrendingGifs(this.CurrentOffset);
    }

    private IObservable<GifSearchResult> TrendingGifs(int offset)
    {
      this.CurrentQuery = "trending";
      if (offset == 0)
      {
        if (GifProviderGiphy.cachedTrendingResultValidTillTicks > DateTime.UtcNow.Ticks && GifProviderGiphy.cachedFirstTrendingResult != null)
        {
          Log.d("GifProviderTenor", "Returning cached result");
          this.CurrentOffset = this.cachedFirstTrendingCurrentOffset;
          return this.InterpretGifResults(GifProviderGiphy.cachedFirstTrendingResult);
        }
        GifProviderGiphy.cachedFirstTrendingResult = (byte[]) null;
        GifProviderGiphy.cachedTrendingResultValidTillTicks = 0L;
      }
      string uri = string.Format("https://api.giphy.com/v1/gifs/trending?api_key={0}&limit={1}&offset={2} ", (object) GifProviderGiphy.Key, (object) 12, (object) offset);
      return Observable.Defer<GifSearchResult>((Func<IObservable<GifSearchResult>>) (() => Observable.Defer<byte[]>((Func<IObservable<byte[]>>) (() => new Uri(uri, UriKind.Absolute).ToGetRequest().GetResponseBytesAync())).SelectMany<byte[], GifSearchResult, GifSearchResult>((Func<byte[], IObservable<GifSearchResult>>) (bytes => this.InterpretGifResults(bytes)), (Func<byte[], GifSearchResult, GifSearchResult>) ((bytes, res) => res))));
    }

    private IObservable<GifSearchResult> InterpretGifResults(byte[] resultBytes)
    {
      GifProviderGiphy.ResultRoot resultRoot = (GifProviderGiphy.ResultRoot) null;
      if (resultBytes != null && resultBytes.Length != 0)
      {
        using (MemoryStream memoryStream = new MemoryStream(resultBytes))
          resultRoot = new DataContractJsonSerializer(typeof (GifProviderGiphy.ResultRoot)).ReadObject((Stream) memoryStream) as GifProviderGiphy.ResultRoot;
      }
      IEnumerable<GifSearchResult> results = (IEnumerable<GifSearchResult>) new List<GifSearchResult>();
      if (resultRoot == null)
      {
        Log.l(nameof (GifProviderGiphy), "null Json returned!");
      }
      else
      {
        GifProviderGiphy.Data[] source = resultRoot.Data ?? new GifProviderGiphy.Data[0];
        Log.l(nameof (GifProviderGiphy), "Found {0} for {1}", (object) ((IEnumerable<GifProviderGiphy.Data>) source).Count<GifProviderGiphy.Data>(), (object) this.CurrentQuery.Length);
        if (((IEnumerable<GifProviderGiphy.Data>) source).Count<GifProviderGiphy.Data>() > 0)
        {
          if (this.CurrentQuery == "trending" && GifProviderGiphy.cachedTrendingResultValidTillTicks == 0L)
          {
            GifProviderGiphy.cachedFirstTrendingResult = resultBytes;
            this.cachedFirstTrendingCurrentOffset = this.CurrentOffset;
            DateTime dateTime = DateTime.UtcNow;
            dateTime = dateTime.AddMinutes(30.0);
            GifProviderGiphy.cachedTrendingResultValidTillTicks = dateTime.Ticks;
          }
          results = ((IEnumerable<GifProviderGiphy.Data>) (source ?? new GifProviderGiphy.Data[0])).Where<GifProviderGiphy.Data>((Func<GifProviderGiphy.Data, bool>) (gif => gif.Images != null && gif.Images.DownsizedSmall != null && !string.IsNullOrEmpty(gif.Images.DownsizedSmall.Mp4) && gif.Images.DownsizedStill != null)).Select<GifProviderGiphy.Data, GifSearchResult>((Func<GifProviderGiphy.Data, GifSearchResult>) (gif => new GifSearchResult()
          {
            GifPath = gif.Images.DownsizedStill.PreviewUrl,
            GifPreviewPath = gif.Images.DownsizedStill.PreviewUrl,
            Mp4Path = gif.Images.DownsizedSmall.Mp4,
            Mp4PreviewPath = gif.Images.DownsizedStill.PreviewUrl,
            Attribution = MessageProperties.MediaProperties.Attribution.GIPHY
          }));
        }
      }
      return Observable.Create<GifSearchResult>((Func<IObserver<GifSearchResult>, Action>) (observer =>
      {
        foreach (GifSearchResult gifSearchResult in results)
          observer.OnNext(gifSearchResult);
        observer.OnCompleted();
        return (Action) (() => { });
      }));
    }

    public IObservable<GifSearchResult> LoadAdditionalGifs()
    {
      this.CurrentOffset += 12;
      return this.CurrentQuery == "trending" ? this.TrendingGifs(this.CurrentOffset) : this.GifSearch(this.CurrentQuery, this.CurrentOffset);
    }

    private static string LangParam()
    {
      if (GifProviderGiphy.langParm == null)
        GifProviderGiphy.langParm = GifProviderGiphy.LangParamImpl();
      return GifProviderGiphy.langParm;
    }

    private static string LangParamImpl()
    {
      string lang;
      string locale;
      AppState.GetLangAndLocale(out lang, out locale);
      if (!string.IsNullOrEmpty(lang))
      {
        foreach (string str in GifProviderGiphy.SUPPORTED_LANGUAGES)
        {
          if (str.StartsWith(lang, StringComparison.InvariantCultureIgnoreCase) && (lang.Length == str.Length || lang.Length < str.Length && str.EndsWith(locale)))
            return str;
        }
      }
      return "";
    }

    public string GetName() => "GIPHY";

    public string GetSearchTooltip() => AppResources.SearchGiphy;

    [DataContract]
    public class ResultRoot
    {
      [DataMember(Name = "data")]
      public GifProviderGiphy.Data[] Data { get; set; }
    }

    [DataContract]
    public class Data
    {
      [DataMember(Name = "url")]
      public string WebsiteUrl { get; set; }

      [DataMember(Name = "images")]
      public GifProviderGiphy.Images Images { get; set; }
    }

    [DataContract]
    public class Images
    {
      [DataMember(Name = "downsized_small")]
      public GifProviderGiphy.DownsizedMp4 DownsizedSmall { get; set; }

      [DataMember(Name = "fixed_height_still")]
      public GifProviderGiphy.DownsizedStill DownsizedStill { get; set; }
    }

    [DataContract]
    public class DownsizedMp4
    {
      [DataMember(Name = "mp4")]
      public string Mp4 { get; set; }
    }

    [DataContract]
    public class DownsizedStill
    {
      [DataMember(Name = "url")]
      public string PreviewUrl { get; set; }
    }
  }
}
