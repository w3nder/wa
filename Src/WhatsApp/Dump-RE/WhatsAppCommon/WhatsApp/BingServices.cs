// Decompiled with JetBrains decompiler
// Type: WhatsApp.BingServices
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

#nullable disable
namespace WhatsApp
{
  public class BingServices : IImageSearch, IMapThumbnailFetch
  {
    private static readonly string BING_MKT_DEFAULT = "en-US";
    private readonly string[] BING_MKTS = new string[41]
    {
      "es-AR",
      "en-AU",
      "de-AT",
      "nl-BE",
      "fr-BE",
      "pt-BR",
      "en-CA",
      "fr-CA",
      "es-CL",
      "da-DK",
      "fi-FI",
      "fr-FR",
      "de-DE",
      "zh-HK",
      "en-IN",
      "en-ID",
      "en-IE",
      "it-IT",
      "ja-JP",
      "ko-KR",
      "en-MY",
      "es-MX",
      "nl-NL",
      "en-NZ",
      "no-NO",
      "zh-CN",
      "pl-PL",
      "pt-PT",
      "en-PH",
      "ru-RU",
      "ar-SA",
      "en-ZA",
      "es-ES",
      "sv-SE",
      "fr-CH",
      "de-CH",
      "zh-TW",
      "tr-TR",
      "en-GB",
      BingServices.BING_MKT_DEFAULT,
      "es-US"
    };
    private static readonly string BING_ASPECT_DEFAULT = "Square";
    private readonly string[] BING_ASPECTS = new string[4]
    {
      BingServices.BING_ASPECT_DEFAULT,
      "Wide",
      "Tall",
      "All"
    };
    private static readonly string BING_SIZE_DEFAULT = "Medium";
    private readonly string[] BING_SIZES = new string[5]
    {
      "Small",
      BingServices.BING_SIZE_DEFAULT,
      "Large",
      "Wallpaper",
      "All"
    };

    private static string KeyV6 => NativeInterfaces.Misc.GetString(7);

    public IObservable<ImageSearchResult> ImageSearch(
      string query,
      string size,
      string aspect,
      int minimumSize)
    {
      string lang;
      string locale;
      AppState.GetLangAndLocale(out lang, out locale);
      string str1 = locale == null || lang == null ? BingServices.BING_MKT_DEFAULT : lang + "-" + locale.ToUpperInvariant();
      string str2 = ((IEnumerable<string>) this.BING_MKTS).Contains<string>(str1) ? str1 : BingServices.BING_MKT_DEFAULT;
      size = size == null || !((IEnumerable<string>) this.BING_SIZES).Contains<string>(size) ? BingServices.BING_SIZE_DEFAULT : size;
      aspect = aspect == null || !((IEnumerable<string>) this.BING_ASPECTS).Contains<string>(aspect) ? BingServices.BING_ASPECT_DEFAULT : aspect;
      string uri = string.Format("https://www.bingapis.com/api/v6/images/search?q={0}&SafeSearch=Strict&appid={1}&mkt={2}&size={3}&aspect={4}&count={5}", (object) Uri.EscapeDataString(query), (object) BingServices.KeyV6, (object) str2, (object) size, (object) aspect, (object) 40);
      return Observable.Defer<ImageSearchResult>((Func<IObservable<ImageSearchResult>>) (() => new Uri(uri, UriKind.Absolute).ToGetRequest().GetResponseBytesAync().SelectMany<byte[], ImageSearchResult, ImageSearchResult>((Func<byte[], IObservable<ImageSearchResult>>) (bytes => this.InterpretImageResultsV6(bytes, minimumSize, uri)), (Func<byte[], ImageSearchResult, ImageSearchResult>) ((bytes, res) => res))));
    }

    private IObservable<ImageSearchResult> InterpretImageResultsV6(
      byte[] resultBytes,
      int minimumSize,
      string uri)
    {
      BingServices.RootElementV6 rootElementV6 = (BingServices.RootElementV6) null;
      using (MemoryStream memoryStream = new MemoryStream(resultBytes))
        rootElementV6 = new DataContractJsonSerializer(typeof (BingServices.RootElementV6)).ReadObject((Stream) memoryStream) as BingServices.RootElementV6;
      if (rootElementV6 == null)
        throw new Exception("json results came back as null");
      ImageSearchResult[] imageSearchResultArray = (ImageSearchResult[]) null;
      if (rootElementV6.type != null && rootElementV6.type == "Images" && rootElementV6.image != null && rootElementV6.image.Length != 0)
      {
        Log.d("Bing Image V6", "selecting from {0} results", (object) rootElementV6.image.Length);
        imageSearchResultArray = ((IEnumerable<BingServices.ImageV6>) rootElementV6.image).Where<BingServices.ImageV6>((Func<BingServices.ImageV6, bool>) (elem =>
        {
          if (elem.ThumbnailUrl != null && elem.MediaUrl != null && elem.Height.HasValue && elem.Width.HasValue && elem.FileSize > 0L)
          {
            if (((IEnumerable<int>) new int[2]
            {
              minimumSize != 0 ? minimumSize : 300,
              Math.Min(elem.Width.Value, elem.Height.Value)
            }).IsSorted<int>())
              return elem.FileSize < 512000L;
          }
          return false;
        })).Select(elem => new
        {
          Thumbnail = elem.ThumbnailUrl,
          MediaUrl = elem.MediaUrl,
          Url = elem.Url
        }).Select(item => new ImageSearchResult(BingServices.ImageFromUrl(item.Thumbnail), BingServices.ImageFromUrl(item.MediaUrl), item.Url)).ToArray<ImageSearchResult>();
      }
      return ((IEnumerable<ImageSearchResult>) (imageSearchResultArray ?? new ImageSearchResult[0])).ToObservable<ImageSearchResult>();
    }

    private static IObservable<WriteableBitmap> ImageFromUrl(string url)
    {
      return Observable.Defer<WriteableBitmap>((Func<IObservable<WriteableBitmap>>) (() => new Uri(url, UriKind.Absolute).ToGetRequest().GetResponseBytesAync().DecodeJpeg()));
    }

    public IObservable<WriteableBitmap> GetMapThumbnail(
      double latitude,
      double longitude,
      int sourceSize,
      int cropSize)
    {
      return this.GetMapThumbnail(latitude, longitude, sourceSize, sourceSize, cropSize, cropSize);
    }

    public IObservable<WriteableBitmap> GetMapThumbnail(
      double latitude,
      double longitude,
      int sourceWidth,
      int sourceHeight,
      int cropWidth,
      int cropHeight)
    {
      sourceHeight += 36;
      string uri = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "https://dev.virtualearth.net/REST/v1/Imagery/Map/{5}/{0},{1}/16?mapSize={2},{3}&pushpin={0},{1}&key={4}", (object) latitude, (object) longitude, (object) sourceWidth, (object) sourceHeight, (object) NativeInterfaces.Misc.GetString(4), Settings.MapCartographicModeRoad ? (object) "Road" : (object) "AerialWithLabels");
      return Observable.Defer<WriteableBitmap>((Func<IObservable<WriteableBitmap>>) (() => Observable.Defer<byte[]>((Func<IObservable<byte[]>>) (() => new Uri(uri, UriKind.Absolute).ToGetRequest().GetResponseBytesAync())).Cache(WebServices.GetCachePath("bingMap", latitude, longitude, (double) sourceWidth, (double) sourceHeight, (string) null)).DecodeJpeg().Select<WriteableBitmap, WriteableBitmap>((Func<WriteableBitmap, WriteableBitmap>) (src =>
      {
        if (sourceWidth > cropWidth || sourceHeight > cropHeight)
        {
          if (cropWidth > sourceWidth)
            cropWidth = sourceWidth;
          if (cropHeight > sourceHeight)
            cropHeight = sourceHeight;
          WriteableBitmap writeableBitmap = new WriteableBitmap(cropWidth, cropHeight);
          writeableBitmap.Render((UIElement) new Image()
          {
            Source = (ImageSource) src,
            Width = (double) src.PixelWidth,
            Height = (double) src.PixelHeight
          }, (Transform) new TranslateTransform()
          {
            X = (double) (-(sourceWidth - cropWidth) / 2),
            Y = (double) (-(sourceHeight - cropHeight) / 2)
          });
          writeableBitmap.Invalidate();
          src = writeableBitmap;
        }
        return src;
      }))));
    }

    public IObservable<PlaceSearchResult> ReverseGeocode(double latitude, double longitude)
    {
      string uri = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "https://dev.virtualearth.net/REST/v1/Locations/{0},{1}?key={2}", (object) latitude, (object) longitude, (object) NativeInterfaces.Misc.GetString(4));
      return Observable.Defer<PlaceSearchResult>((Func<IObservable<PlaceSearchResult>>) (() => Observable.Defer<byte[]>((Func<IObservable<byte[]>>) (() => new Uri(uri, UriKind.Absolute).ToGetRequest().GetResponseBytesAync())).SelectMany<byte[], PlaceSearchResult, PlaceSearchResult>((Func<byte[], IObservable<PlaceSearchResult>>) (bytes => this.InterpretPlaceResults(bytes)), (Func<byte[], PlaceSearchResult, PlaceSearchResult>) ((bytes, res) => res))));
    }

    private IObservable<PlaceSearchResult> InterpretPlaceResults(byte[] resultBytes)
    {
      BingServices.SearchResult resp = (BingServices.SearchResult) null;
      if (resultBytes != null && resultBytes.Length != 0)
      {
        using (MemoryStream memoryStream = new MemoryStream(resultBytes))
          resp = new DataContractJsonSerializer(typeof (BingServices.SearchResult)).ReadObject((Stream) memoryStream) as BingServices.SearchResult;
      }
      if (resp == null)
        throw new Exception("json results came back as null");
      if (resp.resourceSets == null || resp.resourceSets.Length == 0)
        return ((IEnumerable<PlaceSearchResult>) new PlaceSearchResult[0]).ToObservable<PlaceSearchResult>();
      Dispatcher dispatcher = Deployment.Current.Dispatcher;
      IObservable<PlaceSearchResult> r = (IObservable<PlaceSearchResult>) null;
      Action action = (Action) (() => r = ((IEnumerable<PlaceSearchResult>) ((IEnumerable<BingServices.ResourceSet>) resp.resourceSets).Where<BingServices.ResourceSet>((Func<BingServices.ResourceSet, bool>) (elem => elem.resources != null && ((IEnumerable<BingServices.Location>) elem.resources).Count<BingServices.Location>() > 0 && elem.resources[0].address != null)).Select<BingServices.ResourceSet, PlaceSearchResult>((Func<BingServices.ResourceSet, PlaceSearchResult>) (elem => new PlaceSearchResult()
      {
        Address = elem.resources[0].address.addressLine,
        Locality = elem.resources[0].address.locality,
        AdminDistrict = elem.resources[0].address.adminDistrict
      })).ToArray<PlaceSearchResult>()).ToObservable<PlaceSearchResult>());
      dispatcher.InvokeSynchronous(action);
      return r;
    }

    [DataContract]
    public class RootElementV6
    {
      [DataMember(Name = "_type")]
      public string type { get; set; }

      [DataMember(Name = "value")]
      public BingServices.ImageV6[] image { get; set; }
    }

    [DataContract]
    public class ImageV6
    {
      [DataMember(Name = "thumbnailUrl")]
      public string ThumbnailUrl { get; set; }

      [DataMember(Name = "contentUrl")]
      public string MediaUrl { get; set; }

      [DataMember(Name = "hostPageUrl")]
      public string Url { get; set; }

      [DataMember(Name = "height")]
      public int? Height { get; set; }

      [DataMember(Name = "width")]
      public int? Width { get; set; }

      [DataMember(Name = "contentSize")]
      public string ContentSize { get; set; }

      public long FileSize => Utils.FileSizeFormatter.ConvertToLong(this.ContentSize);
    }

    [DataContract]
    public class SearchResult
    {
      [DataMember]
      public string authenticationResultCode { get; set; }

      [DataMember]
      public string brandLogoUri { get; set; }

      [DataMember]
      public string copyright { get; set; }

      [DataMember]
      public BingServices.ResourceSet[] resourceSets { get; set; }
    }

    [DataContract]
    public class ResourceSet
    {
      [DataMember]
      public int estimatedTotal { get; set; }

      [DataMember]
      public BingServices.Location[] resources { get; set; }
    }

    [DataContract(Name = "Location", Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
    public class Location
    {
      [DataMember]
      public string name { get; set; }

      [DataMember]
      public BingServices.Point point { get; set; }

      [DataMember]
      public BingServices.Address address { get; set; }
    }

    [DataContract]
    public class Point
    {
      [DataMember]
      public string type { get; set; }

      [DataMember]
      public string[] coordinates { get; set; }
    }

    [DataContract]
    public class Address
    {
      [DataMember]
      public string addressLine { get; set; }

      [DataMember]
      public string adminDistrict { get; set; }

      [DataMember]
      public string adminDistrict2 { get; set; }

      [DataMember]
      public string countryRegion { get; set; }

      [DataMember]
      public string formattedAddress { get; set; }

      [DataMember]
      public string locality { get; set; }

      [DataMember]
      public string postalCode { get; set; }
    }
  }
}
