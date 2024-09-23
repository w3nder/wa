// Decompiled with JetBrains decompiler
// Type: WhatsApp.WebServices
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Media.Imaging;

#nullable disable
namespace WhatsApp
{
  public class WebServices
  {
    private static readonly string LogHdr = nameof (WebServices);
    private static WebServices instance = new WebServices();
    private static IPlaceSearch[] PLACE_SERVICES;
    private static string[] LOG_PROVIDER_NAMES;
    private int availablePlaceServicesCount = 2;
    private static BingServices BING_SERVICES = new BingServices();
    private static GoogleServices GOOGLE_SERVICES = new GoogleServices();
    private static int CurrentProvider = WebServices.ValidateProvider(Settings.LocationProvider);
    private static bool skipGoogleMaps = false;
    public static bool ForceFbPlaceSearch = false;
    private static readonly int[] NextProviderArray = new int[4]
    {
      3,
      3,
      2,
      2
    };
    private static bool cacheChecked = false;

    private WebServices()
    {
      WebServices.PLACE_SERVICES = new IPlaceSearch[4];
      WebServices.PLACE_SERVICES[3] = (IPlaceSearch) new FacebookServices();
      WebServices.PLACE_SERVICES[2] = (IPlaceSearch) new FoursquareServices();
      this.availablePlaceServicesCount = 2;
      WebServices.LOG_PROVIDER_NAMES = new string[4];
      WebServices.LOG_PROVIDER_NAMES[3] = "Facebook";
      WebServices.LOG_PROVIDER_NAMES[2] = "Foursquare";
    }

    public static WebServices Instance => WebServices.instance;

    public IObservable<WriteableBitmap> GetMapThumbnail(
      double lat,
      double lon,
      int srcSize,
      int cropSize)
    {
      return this.GetMapThumbnail(lat, lon, srcSize, srcSize, cropSize, cropSize);
    }

    public IObservable<WriteableBitmap> GetMapThumbnail(
      double lat,
      double lon,
      int srcWidth,
      int srcHeight,
      int cropWidth,
      int cropHeight)
    {
      return Observable.Create<WriteableBitmap>((Func<IObserver<WriteableBitmap>, Action>) (observer =>
      {
        bool cancelled = false;
        IDisposable thumbSub = (IDisposable) null;
        Action<IObserver<WriteableBitmap>> getFromBing = (Action<IObserver<WriteableBitmap>>) (outerObserver => thumbSub = WebServices.BING_SERVICES.GetMapThumbnail(lat, lon, srcWidth, srcHeight, cropWidth, cropHeight).Subscribe<WriteableBitmap>((Action<WriteableBitmap>) (mapThumb =>
        {
          if (!cancelled)
          {
            outerObserver.OnNext(mapThumb);
            outerObserver.OnCompleted();
          }
          thumbSub.SafeDispose();
          thumbSub = (IDisposable) null;
        }), (Action<Exception>) (ex =>
        {
          if (!cancelled)
          {
            outerObserver.OnError(ex);
            outerObserver.OnCompleted();
          }
          thumbSub.SafeDispose();
          thumbSub = (IDisposable) null;
          Log.LogException(ex, "getting bing map thumbnail");
        })));
        if (this.ShouldSkipGoogleFlag())
          getFromBing(observer);
        else
          thumbSub = WebServices.GOOGLE_SERVICES.GetMapThumbnail(lat, lon, srcWidth, srcHeight, cropWidth, cropHeight).Subscribe<WriteableBitmap>((Action<WriteableBitmap>) (mapThumb =>
          {
            if (!cancelled)
            {
              observer.OnNext(mapThumb);
              observer.OnCompleted();
            }
            thumbSub.SafeDispose();
            thumbSub = (IDisposable) null;
          }), (Action<Exception>) (ex =>
          {
            thumbSub.SafeDispose();
            thumbSub = (IDisposable) null;
            if (ex is GoogleServices.OverQuota)
            {
              WebServices.skipGoogleMaps = true;
              Settings.LastGoogleMapsFailureUtc = new DateTime?(FunRunner.CurrentServerTimeUtc);
              Log.WriteLineDebug("google maps: hit quota");
            }
            else
              Log.LogException(ex, "getting google maps thumb");
            if (cancelled)
              return;
            getFromBing(observer);
          }));
        return (Action) (() =>
        {
          cancelled = true;
          thumbSub.SafeDispose();
          thumbSub = (IDisposable) null;
        });
      }));
    }

    public IObservable<WriteableBitmap> GetBingMapThumbnail(
      double lat,
      double lon,
      int srcWidth,
      int srcHeight,
      int cropWidth,
      int cropHeight)
    {
      return WebServices.BING_SERVICES.GetMapThumbnail(lat, lon, srcWidth, srcHeight, cropWidth, cropHeight);
    }

    public IObservable<ImageSearchResult> ImageSearch(
      string query,
      string size = null,
      string aspect = null,
      int minimumSize = 0)
    {
      return WebServices.BING_SERVICES.ImageSearch(query, size, aspect, minimumSize);
    }

    public static string GetCachePath(
      string provider,
      double lat,
      double lon,
      double radius,
      string searchTerm)
    {
      return WebServices.GetCachePath(provider, lat, lon, radius, radius, searchTerm);
    }

    public static string GetCachePath(
      string provider,
      double lat,
      double lon,
      double width,
      double height,
      string searchTerm)
    {
      if (!WebServices.cacheChecked)
      {
        using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
        {
          string[] strArray = (string[]) null;
          try
          {
            strArray = storeForApplication.GetFileNames("locCache/*");
          }
          catch (IsolatedStorageException ex1)
          {
            Log.l("loccache", "Exception {0}", (object) ex1.GetFriendlyMessage());
            try
            {
              storeForApplication.CreateDirectory("locCache");
            }
            catch (Exception ex2)
            {
              Log.LogException(ex2, "Create directory exception creating local cache");
            }
          }
          catch (DirectoryNotFoundException ex3)
          {
            try
            {
              storeForApplication.CreateDirectory("locCache");
            }
            catch (Exception ex4)
            {
              Log.LogException(ex4, "Create directory exception creating local cache");
            }
          }
          DateTime dateTime = DateTime.Now.AddDays(-3.0);
          foreach (string str1 in strArray ?? new string[0])
          {
            try
            {
              string str2 = string.Format("{0}/{1}", (object) "locCache", (object) str1);
              if (storeForApplication.GetLastWriteTime(str2) < (DateTimeOffset) dateTime)
                storeForApplication.DeleteFile(str2);
            }
            catch (Exception ex)
            {
            }
          }
        }
        WebServices.cacheChecked = true;
      }
      return string.Format("{0}/{1}_{2:0.#####}_{3:0.#####}_{4}_{5}_{6}", (object) "locCache", (object) provider, (object) lat, (object) lon, (object) width, (object) height, (object) searchTerm);
    }

    private bool ShouldSkipGoogleFlag()
    {
      if (!WebServices.skipGoogleMaps)
      {
        DateTime? googleMapsFailureUtc = Settings.LastGoogleMapsFailureUtc;
        DateTime currentServerTimeUtc = FunRunner.CurrentServerTimeUtc;
        if (googleMapsFailureUtc.HasValue && googleMapsFailureUtc.Value > currentServerTimeUtc.AddHours(-6.0))
          WebServices.skipGoogleMaps = true;
      }
      return WebServices.skipGoogleMaps;
    }

    public IObservable<PlaceSearchResult> ReverseGeocode(double lat, double lon)
    {
      return Observable.Create<PlaceSearchResult>((Func<IObserver<PlaceSearchResult>, Action>) (observer =>
      {
        IDisposable subscription = WebServices.BING_SERVICES.ReverseGeocode(lat, lon).Subscribe<PlaceSearchResult>((Action<PlaceSearchResult>) (res => observer.OnNext(res)), (Action<Exception>) (ex =>
        {
          Log.LogException(ex, "bing reverse geocoding");
          observer.OnError(ex);
        }), (Action) (() => observer.OnCompleted()));
        return (Action) (() =>
        {
          if (subscription == null)
            return;
          subscription.Dispose();
          subscription = (IDisposable) null;
        });
      }));
    }

    public IObservable<PlaceSearchResult> PlaceSearch(
      double lat,
      double lon,
      double radius,
      string name)
    {
      return this.PlaceSearchImpl(lat, lon, radius, false, name);
    }

    public IObservable<PlaceSearchResult> LoadAdditionalPlacesFromSearch()
    {
      return WebServices.PLACE_SERVICES[WebServices.CurrentProvider].LoadAdditionalPlacesFromSearch();
    }

    public IObservable<PlaceSearchResult> AutoCompleteSearch(
      double lat,
      double lon,
      double radius,
      string name)
    {
      return this.PlaceSearchImpl(lat, lon, radius, true, name);
    }

    private IObservable<PlaceSearchResult> TryCurrentProvider(
      double lat,
      double lon,
      double radius,
      bool autocomplete,
      string name = null)
    {
      return Observable.Create<PlaceSearchResult>((Func<IObserver<PlaceSearchResult>, Action>) (observer =>
      {
        IDisposable subscription = (IDisposable) null;
        bool cancel = false;
        if (!cancel)
        {
          int n = 0;
          subscription = WebServices.PLACE_SERVICES[WebServices.CurrentProvider].PlaceSearch(lat, lon, radius, autocomplete, name).Timeout<PlaceSearchResult>(TimeSpan.FromSeconds(6.0)).Subscribe<PlaceSearchResult>((Action<PlaceSearchResult>) (res =>
          {
            observer.OnNext(res);
            ++n;
          }), (Action<Exception>) (ex =>
          {
            if (ex is TimeoutException)
              Log.l(WebServices.LogHdr, "Provider: " + WebServices.LOG_PROVIDER_NAMES[WebServices.CurrentProvider] + " places timeout");
            else
              Log.l(WebServices.LogHdr, "Provider: " + WebServices.LOG_PROVIDER_NAMES[WebServices.CurrentProvider] + " places search error");
            observer.OnError(ex);
          }), (Action) (() =>
          {
            if (n == 0)
            {
              observer.OnError(new Exception());
              Log.l(WebServices.LogHdr, WebServices.LOG_PROVIDER_NAMES[WebServices.CurrentProvider] + " no places found");
            }
            else
              observer.OnCompleted();
          }));
        }
        return (Action) (() =>
        {
          cancel = true;
          if (subscription == null)
            return;
          subscription.Dispose();
          subscription = (IDisposable) null;
        });
      }));
    }

    private static int ValidateProvider(int currentProvider)
    {
      if (currentProvider != 2)
      {
        if (currentProvider == 3)
        {
          Log.l(WebServices.LogHdr, "Server Recommended Facebook");
          return currentProvider;
        }
        Log.l(WebServices.LogHdr, "Server Recommended {0}, using Foursquare", (object) currentProvider);
        return 2;
      }
      Log.l(WebServices.LogHdr, "Server Recommended Foursquare");
      return currentProvider;
    }

    private IObservable<PlaceSearchResult> PlaceSearchImpl(
      double lat,
      double lon,
      double radius,
      bool autocomplete,
      string name = null)
    {
      return Observable.Create<PlaceSearchResult>((Func<IObserver<PlaceSearchResult>, Action>) (observer =>
      {
        IDisposable subscription = (IDisposable) null;
        if (WebServices.ForceFbPlaceSearch)
        {
          WebServices.CurrentProvider = 3;
          Log.l(WebServices.LogHdr, "Forcing Facebook");
        }
        IObservable<PlaceSearchResult> recurse = (IObservable<PlaceSearchResult>) null;
        int attemptsMade = 0;
        recurse = this.TryCurrentProvider(lat, lon, radius, autocomplete, name).Catch<PlaceSearchResult>(Observable.Defer<PlaceSearchResult>((Func<IObservable<PlaceSearchResult>>) (() =>
        {
          Log.l(WebServices.LogHdr, WebServices.LOG_PROVIDER_NAMES[WebServices.CurrentProvider] + " failed");
          if (++attemptsMade > this.availablePlaceServicesCount)
          {
            Log.p(WebServices.LogHdr, "all providers failed, returning no results");
            return Observable.Empty<PlaceSearchResult>();
          }
          WebServices.CurrentProvider = WebServices.GetNextProvider(WebServices.CurrentProvider);
          return recurse;
        })));
        subscription = recurse.Subscribe<PlaceSearchResult>((Action<PlaceSearchResult>) (res => observer.OnNext(res)), (Action) (() => observer.OnCompleted()));
        return (Action) (() =>
        {
          subscription.SafeDispose();
          subscription = (IDisposable) null;
        });
      }));
    }

    private static int GetNextProvider(int currentProvider)
    {
      int nextProvider = currentProvider <= 0 || currentProvider > WebServices.NextProviderArray.Length ? 3 : WebServices.NextProviderArray[currentProvider - 1];
      Log.l(WebServices.LogHdr, "Trying: {0}", (object) WebServices.LOG_PROVIDER_NAMES[nextProvider]);
      return nextProvider;
    }

    public IObservable<WriteableBitmap> GetPlaceIcon(string uri)
    {
      return Observable.Defer<WriteableBitmap>((Func<IObservable<WriteableBitmap>>) (() => Observable.Defer<WriteableBitmap>((Func<IObservable<WriteableBitmap>>) (() => new Uri(uri, UriKind.Absolute).ToGetRequest().GetResponseBytesAync().DecodeJpeg()))));
    }

    public static IObservable<LocationData> GetCompleteLocationData(LocationData locData)
    {
      return Observable.Create<LocationData>((Func<IObserver<LocationData>, Action>) (observer =>
      {
        IDisposable placeDetailsSub = (IDisposable) null;
        IDisposable mapThumbSub = (IDisposable) null;
        WebServices instance = WebServices.Instance;
        double? nullable = locData.Latitude;
        double valueOrDefault1 = nullable.GetValueOrDefault();
        nullable = locData.Longitude;
        double valueOrDefault2 = nullable.GetValueOrDefault();
        mapThumbSub = instance.GetMapThumbnail(valueOrDefault1, valueOrDefault2, 680, 320, 680, 320).Subscribe<WriteableBitmap>((Action<WriteableBitmap>) (largeThumb =>
        {
          WriteableBitmap bitmap = (WriteableBitmap) null;
          if (largeThumb != null)
          {
            int num = 300;
            System.Windows.Point cropPos = new System.Windows.Point((double) ((largeThumb.PixelWidth - num) / 2), (double) ((largeThumb.PixelHeight - num) / 2));
            bitmap = largeThumb.Crop(cropPos, new Size((double) num, (double) num));
          }
          locData.ThumbnailBytes = bitmap.ToJpegByteArray(48128);
          locData.LargeThumbnailBytes = largeThumb.ToJpegByteArray(48128);
          observer.OnNext(locData);
          observer.OnCompleted();
        }), (Action<Exception>) (ex => observer.OnCompleted()), (Action) (() =>
        {
          if (placeDetailsSub != null)
            return;
          observer.OnCompleted();
        }));
        return (Action) (() =>
        {
          mapThumbSub.SafeDispose();
          placeDetailsSub.SafeDispose();
          mapThumbSub = placeDetailsSub = (IDisposable) null;
        });
      }));
    }

    public class Attribution
    {
      public string Text;
      public BitmapImage Logo;
      public double LogoScaleFactor = 0.5;
    }
  }
}
