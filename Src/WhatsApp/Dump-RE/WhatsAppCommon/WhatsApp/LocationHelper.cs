// Decompiled with JetBrains decompiler
// Type: WhatsApp.LocationHelper
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Device.Location;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;

#nullable disable
namespace WhatsApp
{
  public static class LocationHelper
  {
    private static Geolocator _geoLocator;

    private static Geolocator Locator
    {
      get
      {
        if (LocationHelper._geoLocator == null)
          LocationHelper._geoLocator = new Geolocator();
        return LocationHelper._geoLocator;
      }
    }

    public static IObservable<GeoCoordinate> ObserveAccurateGeoCoordinate()
    {
      return Observable.Create<GeoCoordinate>((Func<IObserver<GeoCoordinate>, Action>) (observer =>
      {
        // ISSUE: object of a compiler-generated type is created
        // ISSUE: variable of a compiler-generated type
        LocationHelper.\u003C\u003Ec__DisplayClass3_0 cDisplayClass30 = new LocationHelper.\u003C\u003Ec__DisplayClass3_0()
        {
          observer = observer,
          geoLocator = LocationHelper.Locator,
          handler = (TypedEventHandler<Geolocator, PositionChangedEventArgs>) null
        };
        // ISSUE: reference to a compiler-generated field
        // ISSUE: method pointer
        cDisplayClass30.handler = new TypedEventHandler<Geolocator, PositionChangedEventArgs>((object) cDisplayClass30, __methodptr(\u003CObserveAccurateGeoCoordinate\u003Eb__1));
        try
        {
          // ISSUE: reference to a compiler-generated field
          cDisplayClass30.geoLocator.put_DesiredAccuracy((PositionAccuracy) 0);
          // ISSUE: reference to a compiler-generated field
          cDisplayClass30.geoLocator.put_MovementThreshold(10.0);
          // ISSUE: reference to a compiler-generated field
          cDisplayClass30.geoLocator.put_ReportInterval(0U);
          // ISSUE: reference to a compiler-generated field
          Geolocator geoLocator = cDisplayClass30.geoLocator;
          // ISSUE: reference to a compiler-generated field
          WindowsRuntimeMarshal.AddEventHandler<TypedEventHandler<Geolocator, PositionChangedEventArgs>>(new Func<TypedEventHandler<Geolocator, PositionChangedEventArgs>, EventRegistrationToken>(geoLocator.add_PositionChanged), new Action<EventRegistrationToken>(geoLocator.remove_PositionChanged), cDisplayClass30.handler);
        }
        catch (Exception ex)
        {
          // ISSUE: reference to a compiler-generated field
          cDisplayClass30.observer.OnError(ex);
        }
        // ISSUE: reference to a compiler-generated method
        return new Action(cDisplayClass30.\u003CObserveAccurateGeoCoordinate\u003Eb__2);
      }));
    }

    public static async Task<GeoCoordinate> GetInaccurateGeoCoordinate()
    {
      Geolocator locator = LocationHelper.Locator;
      locator.put_DesiredAccuracyInMeters(new uint?(1000U));
      locator.put_MovementThreshold(10.0);
      locator.put_ReportInterval(0U);
      Geoposition geopositionAsync = await locator.GetGeopositionAsync(TimeSpan.FromMinutes(5.0), TimeSpan.FromSeconds(10.0));
      return new GeoCoordinate(geopositionAsync.Coordinate.Latitude, geopositionAsync.Coordinate.Longitude, 0.0, geopositionAsync.Coordinate.Accuracy, geopositionAsync.Coordinate.Accuracy, 0.0, 0.0);
    }

    public static void CompletePendingLocationDataMessage(Message msg, bool failureRetried)
    {
      LocationData locData = LocationData.CreateFromMessage(msg);
      if (locData.Latitude.Value == double.MinValue || locData.Longitude.Value == double.MinValue)
      {
        IDisposable locationSubscription = (IDisposable) null;
        locationSubscription = LocationHelper.ObserveAccurateGeoCoordinate().Subscribe<GeoCoordinate>((Action<GeoCoordinate>) (coord =>
        {
          locData.Latitude = new double?(coord.Latitude);
          locData.Longitude = new double?(coord.Longitude);
          WebServices.GetCompleteLocationData(locData).Subscribe<LocationData>((Action<LocationData>) (newLocData => newLocData.PopulateMessage(msg)));
          locationSubscription?.Dispose();
        }), (Action<Exception>) (e =>
        {
          if (failureRetried)
            return;
          Log.l("LocMsg", "CompletePendingLocationDataMessageFailed: " + e.Message);
          locationSubscription?.Dispose();
          WAThreadPool.RunAfterDelay(TimeSpan.FromSeconds(1.0), (Action) (() => LocationHelper.CompletePendingLocationDataMessage(msg, true)));
        }));
      }
      else
        WebServices.GetCompleteLocationData(locData).Subscribe<LocationData>((Action<LocationData>) (newLocData => newLocData.PopulateMessage(msg)));
    }
  }
}
