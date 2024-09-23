// Decompiled with JetBrains decompiler
// Type: WhatsApp.GifProviderTenor
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
  public class GifProviderTenor : IGifProvider
  {
    private const string TRENDING_QUERY = "trending";
    private string CurrentQuery = "trending";
    private string CurrentNextPos;
    private static byte[] cachedFirstTrendingResult = (byte[]) null;
    private static string cachedFirstTrendingNextPos = (string) null;
    private static long cachedTrendingResultValidTillTicks = 0;
    private static WebServices.Attribution attribution = new WebServices.Attribution()
    {
      Text = "Powered by Tenor",
      Logo = (BitmapImage) null
    };
    private static string localParm = (string) null;

    private static string Key => "8Y1MY7JASQTM";

    public IObservable<GifSearchResult> GifSearch(string query)
    {
      this.CurrentNextPos = (string) null;
      return this.GifSearch(query, this.CurrentNextPos);
    }

    private IObservable<GifSearchResult> GifSearch(string query, string position)
    {
      this.CurrentQuery = query;
      string uri = string.Format("https://wa.tenor.co/v1/search?tag={0}&key={1}&limit={2}{3}{4}&safesearch=strict&media_filter=basic", (object) query, (object) GifProviderTenor.Key, (object) 12, position == null ? (object) "" : (object) ("&pos=" + position), (object) GifProviderTenor.LocaleParam());
      return Observable.Defer<GifSearchResult>((Func<IObservable<GifSearchResult>>) (() => Observable.Defer<byte[]>((Func<IObservable<byte[]>>) (() => new Uri(uri, UriKind.Absolute).ToGetRequest().GetResponseBytesAync())).SelectMany<byte[], GifSearchResult, GifSearchResult>((Func<byte[], IObservable<GifSearchResult>>) (bytes => this.InterpretGifResults(bytes)), (Func<byte[], GifSearchResult, GifSearchResult>) ((bytes, res) => res))));
    }

    public IObservable<GifSearchResult> TrendingGifs()
    {
      this.CurrentNextPos = (string) null;
      return this.TrendingGifs(this.CurrentNextPos);
    }

    private IObservable<GifSearchResult> TrendingGifs(string position)
    {
      this.CurrentQuery = "trending";
      if (position == null)
      {
        if (GifProviderTenor.cachedTrendingResultValidTillTicks > DateTime.UtcNow.Ticks && GifProviderTenor.cachedFirstTrendingResult != null)
        {
          Log.d(nameof (GifProviderTenor), "Returning cached result");
          this.CurrentNextPos = GifProviderTenor.cachedFirstTrendingNextPos;
          return this.InterpretGifResults(GifProviderTenor.cachedFirstTrendingResult);
        }
        GifProviderTenor.cachedFirstTrendingResult = (byte[]) null;
        GifProviderTenor.cachedTrendingResultValidTillTicks = 0L;
      }
      string uri = string.Format("https://wa.tenor.co/v1/trending?key={0}&media_filter=basic&limit={1}{2}", (object) GifProviderTenor.Key, (object) 12, position == null ? (object) "" : (object) ("&pos=" + position));
      return Observable.Defer<GifSearchResult>((Func<IObservable<GifSearchResult>>) (() => Observable.Defer<byte[]>((Func<IObservable<byte[]>>) (() => new Uri(uri, UriKind.Absolute).ToGetRequest().GetResponseBytesAync())).SelectMany<byte[], GifSearchResult, GifSearchResult>((Func<byte[], IObservable<GifSearchResult>>) (bytes => this.InterpretGifResults(bytes)), (Func<byte[], GifSearchResult, GifSearchResult>) ((bytes, res) => res))));
    }

    private IObservable<GifSearchResult> InterpretGifResults(byte[] resultBytes)
    {
      GifProviderTenor.ResultRoot resultRoot = (GifProviderTenor.ResultRoot) null;
      if (resultBytes != null && resultBytes.Length != 0)
      {
        using (MemoryStream memoryStream = new MemoryStream(resultBytes))
          resultRoot = new DataContractJsonSerializer(typeof (GifProviderTenor.ResultRoot)).ReadObject((Stream) memoryStream) as GifProviderTenor.ResultRoot;
      }
      IEnumerable<GifSearchResult> results = (IEnumerable<GifSearchResult>) new List<GifSearchResult>();
      if (resultRoot == null)
      {
        Log.l(nameof (GifProviderTenor), "null Json returned!");
      }
      else
      {
        this.CurrentNextPos = resultRoot.Next;
        GifProviderTenor.Results[] source = resultRoot.Results ?? new GifProviderTenor.Results[0];
        Log.l(nameof (GifProviderTenor), "Found {0} for {1}", (object) ((IEnumerable<GifProviderTenor.Results>) source).Count<GifProviderTenor.Results>(), (object) this.CurrentQuery.Length);
        if (((IEnumerable<GifProviderTenor.Results>) source).Count<GifProviderTenor.Results>() > 0)
        {
          if (this.CurrentQuery == "trending" && GifProviderTenor.cachedTrendingResultValidTillTicks == 0L)
          {
            GifProviderTenor.cachedFirstTrendingResult = resultBytes;
            GifProviderTenor.cachedFirstTrendingNextPos = this.CurrentNextPos;
            GifProviderTenor.cachedTrendingResultValidTillTicks = DateTime.UtcNow.AddMinutes(30.0).Ticks;
          }
          results = ((IEnumerable<GifProviderTenor.Results>) source).SelectMany((Func<GifProviderTenor.Results, IEnumerable<GifProviderTenor.Media>>) (media => (IEnumerable<GifProviderTenor.Media>) media.Media ?? (IEnumerable<GifProviderTenor.Media>) new GifProviderTenor.Media[0]), (media, gif) => new
          {
            media = media,
            gif = gif
          }).Where(_param1 => _param1.gif.TinyGif != null && _param1.gif.TinyMp4 != null && !string.IsNullOrEmpty(_param1.gif.TinyMp4.VideoUrl)).Select(_param1 => new GifSearchResult()
          {
            GifPath = _param1.gif.TinyGif.GifUrl,
            GifPreviewPath = _param1.gif.TinyGif.Preview,
            Mp4Path = _param1.gif.TinyMp4.VideoUrl,
            Mp4PreviewPath = _param1.gif.TinyMp4.Preview,
            Attribution = MessageProperties.MediaProperties.Attribution.TENOR
          });
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
      return this.CurrentQuery == "trending" ? this.TrendingGifs(this.CurrentNextPos) : this.GifSearch(this.CurrentQuery, this.CurrentNextPos);
    }

    private static string LocaleParam()
    {
      if (GifProviderTenor.localParm == null)
        GifProviderTenor.localParm = GifProviderTenor.LocaleParamImpl();
      return GifProviderTenor.localParm;
    }

    private static string LocaleParamImpl()
    {
      string str = "";
      string lang;
      string locale;
      AppState.GetLangAndLocale(out lang, out locale);
      if (!string.IsNullOrEmpty(lang))
      {
        str = "&locale=" + lang;
        if (!string.IsNullOrEmpty(locale))
          str = str + "_" + locale;
      }
      return str;
    }

    public string GetName() => "Tenor";

    public string GetSearchTooltip() => AppResources.SearchTenor;

    [DataContract]
    public class ResultRoot
    {
      [DataMember(Name = "results")]
      public GifProviderTenor.Results[] Results { get; set; }

      [DataMember(Name = "next")]
      public string Next { get; set; }
    }

    [DataContract]
    public class Results
    {
      [DataMember(Name = "url")]
      public string WebsiteUrl { get; set; }

      [DataMember(Name = "media")]
      public GifProviderTenor.Media[] Media { get; set; }
    }

    [DataContract]
    public class Media
    {
      [DataMember(Name = "tinygif")]
      public GifProviderTenor.TinyGif TinyGif { get; set; }

      [DataMember(Name = "tinymp4")]
      public GifProviderTenor.TinyMp4 TinyMp4 { get; set; }
    }

    [DataContract]
    public class TinyGif
    {
      [DataMember(Name = "url")]
      public string GifUrl { get; set; }

      [DataMember(Name = "preview")]
      public string Preview { get; set; }
    }

    [DataContract]
    public class TinyMp4
    {
      [DataMember(Name = "url")]
      public string VideoUrl { get; set; }

      [DataMember(Name = "preview")]
      public string Preview { get; set; }
    }
  }
}
