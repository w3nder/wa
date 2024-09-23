// Decompiled with JetBrains decompiler
// Type: WhatsApp.LocationSharingService
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Device.Location;
using System.Runtime.InteropServices.WindowsRuntime;
using WhatsApp.WaCollections;
using Windows.Devices.Geolocation;
using Windows.Foundation;


namespace WhatsApp
{
  public class LocationSharingService
  {
    private const string LogHdr = "llss";
    public const int DefaultDuration = 40;
    private const int HighAccuracyUpdateIntervalMS = 5000;
    private const int MediumAccuracyUpdateIntervalMS = 10000;
    private const int LowAccuracyUpdateIntervalMS = 30000;
    private const int HighAccuracyDuration = 1800;
    private const int MediumAccuracyDuration = 7200;
    private const int HighAccuracyBatteryPercentage = 30;
    private const int LowAccuracyBatteryPercentage = 15;
    private TypedEventHandler<Geolocator, PositionChangedEventArgs> positionChangedHandler;
    private TypedEventHandler<Geolocator, StatusChangedEventArgs> statusChangedHandler;
    private LocationSharingService.LastReportedLocationInfo lastReportedLocationInfo;
    private Geolocator geolocator;
    private long expireTime;
    private const int LastReportedLocationInfoVersion = 2;

    public LocationSharingService()
    {
      this.geolocator = new Geolocator();
      this.lastReportedLocationInfo = LocationSharingService.LastReportedLocationInfo.Restore();
    }

    public void BeginLocationSharing(int? durationSeconds)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      LocationSharingService.\u003C\u003Ec__DisplayClass15_0 cDisplayClass150 = new LocationSharingService.\u003C\u003Ec__DisplayClass15_0();
      // ISSUE: reference to a compiler-generated field
      cDisplayClass150.\u003C\u003E4__this = this;
      this.expireTime = FunRunner.CurrentServerTimeUtc.AddSeconds(durationSeconds.HasValue ? (double) durationSeconds.Value : 40.0).ToUnixTime();
      if (this.positionChangedHandler != null)
      {
        if (this.lastReportedLocationInfo.SecondsSinceLastShare() <= 120.0)
          return;
        this.StopLocationSharing();
      }
      else
      {
        bool batterySaverEnabled = AppState.BatterySaverEnabled;
        bool powerSourceConnected = AppState.PowerSourceConnected;
        int batteryPercentage = AppState.BatteryPercentage;
        long sharingTime = LiveLocationSession.Instance.GetSharingTime();
        int num;
        PositionAccuracy? nullable;
        if (batterySaverEnabled)
        {
          // ISSUE: reference to a compiler-generated field
          cDisplayClass150.backOffStage = wam_enum_live_location_backoff_stage.MORE_THAN_TWO_HOURS;
          num = 30000;
          nullable = new PositionAccuracy?();
        }
        else if (powerSourceConnected && batteryPercentage > 30)
        {
          // ISSUE: reference to a compiler-generated field
          cDisplayClass150.backOffStage = wam_enum_live_location_backoff_stage.LESS_THAN_THIRTY_MINUTES;
          num = 5000;
          nullable = new PositionAccuracy?((PositionAccuracy) 1);
        }
        else if (sharingTime > 7200L || !powerSourceConnected && batteryPercentage <= 15)
        {
          // ISSUE: reference to a compiler-generated field
          cDisplayClass150.backOffStage = wam_enum_live_location_backoff_stage.MORE_THAN_TWO_HOURS;
          num = 30000;
          nullable = new PositionAccuracy?();
        }
        else if (sharingTime > 1800L || !powerSourceConnected && batteryPercentage <= 30)
        {
          // ISSUE: reference to a compiler-generated field
          cDisplayClass150.backOffStage = wam_enum_live_location_backoff_stage.THIRTY_MINUTES_TO_TWO_HOURS;
          num = 10000;
          nullable = new PositionAccuracy?((PositionAccuracy) 0);
        }
        else
        {
          // ISSUE: reference to a compiler-generated field
          cDisplayClass150.backOffStage = wam_enum_live_location_backoff_stage.LESS_THAN_THIRTY_MINUTES;
          num = 30000;
          nullable = new PositionAccuracy?((PositionAccuracy) 1);
        }
        Log.l("llss", string.Format("batterySaverEnabled {0} powerSourceConnected {1} batteryPercentage {2} sharingDuration {3} reportInterval {4} desiredAccuracy {5}", (object) batterySaverEnabled, (object) powerSourceConnected, (object) batteryPercentage, (object) sharingTime, (object) num, (object) nullable));
        // ISSUE: reference to a compiler-generated field
        LiveLocationSession.Instance.InitSession(cDisplayClass150.backOffStage);
        try
        {
          if (!nullable.HasValue)
          {
            // ISSUE: reference to a compiler-generated method
            WAThreadPool.QueueUserWorkItem(new Action(cDisplayClass150.\u003CBeginLocationSharing\u003Eb__0));
          }
          else
          {
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            // ISSUE: method pointer
            this.statusChangedHandler = LocationSharingService.\u003C\u003Ec.\u003C\u003E9__15_1 ?? (LocationSharingService.\u003C\u003Ec.\u003C\u003E9__15_1 = new TypedEventHandler<Geolocator, StatusChangedEventArgs>((object) LocationSharingService.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CBeginLocationSharing\u003Eb__15_1)));
            // ISSUE: method pointer
            this.positionChangedHandler = new TypedEventHandler<Geolocator, PositionChangedEventArgs>((object) cDisplayClass150, __methodptr(\u003CBeginLocationSharing\u003Eb__2));
            this.geolocator.put_DesiredAccuracy(nullable.Value);
            this.geolocator.put_MovementThreshold(0.0);
            this.geolocator.put_ReportInterval((uint) num);
            Geolocator geolocator1 = this.geolocator;
            WindowsRuntimeMarshal.AddEventHandler<TypedEventHandler<Geolocator, StatusChangedEventArgs>>(new Func<TypedEventHandler<Geolocator, StatusChangedEventArgs>, EventRegistrationToken>(geolocator1.add_StatusChanged), new Action<EventRegistrationToken>(geolocator1.remove_StatusChanged), this.statusChangedHandler);
            Geolocator geolocator2 = this.geolocator;
            WindowsRuntimeMarshal.AddEventHandler<TypedEventHandler<Geolocator, PositionChangedEventArgs>>(new Func<TypedEventHandler<Geolocator, PositionChangedEventArgs>, EventRegistrationToken>(geolocator2.add_PositionChanged), new Action<EventRegistrationToken>(geolocator2.remove_PositionChanged), this.positionChangedHandler);
          }
        }
        catch (Exception ex)
        {
          Log.l(ex, "Exception when sharing location");
        }
      }
    }

    public void StopLocationSharingAndPostSession()
    {
      this.StopLocationSharing();
      LiveLocationSession.Instance.PostSession();
    }

    private void StopLocationSharing()
    {
      WindowsRuntimeMarshal.RemoveEventHandler<TypedEventHandler<Geolocator, StatusChangedEventArgs>>(new Action<EventRegistrationToken>(this.geolocator.remove_StatusChanged), this.statusChangedHandler);
      WindowsRuntimeMarshal.RemoveEventHandler<TypedEventHandler<Geolocator, PositionChangedEventArgs>>(new Action<EventRegistrationToken>(this.geolocator.remove_PositionChanged), this.positionChangedHandler);
      this.statusChangedHandler = (TypedEventHandler<Geolocator, StatusChangedEventArgs>) null;
      this.positionChangedHandler = (TypedEventHandler<Geolocator, PositionChangedEventArgs>) null;
      this.expireTime = 0L;
    }

    private void OnNewGeoposition(
      Geoposition gpos,
      wam_enum_live_location_backoff_stage backOffStage)
    {
      FunXMPP.Connection connection = AppState.GetConnection();
      if (connection == null)
      {
        Log.l("llss", "Bypassed OnNewGeoposition, no connection");
      }
      else
      {
        if (gpos == null || gpos.Coordinate == null || !this.lastReportedLocationInfo.isBetterLocation(gpos.Coordinate))
          return;
        Geocoordinate coordinate = gpos.Coordinate;
        double? nullable1 = coordinate.Speed;
        nullable1.GetValueOrDefault();
        WhatsApp.ProtoBuf.Message msg = new WhatsApp.ProtoBuf.Message()
        {
          LiveLocationMessageField = new WhatsApp.ProtoBuf.Message.LiveLocationMessage()
          {
            AccuracyInMeters = new uint?((uint) coordinate.Accuracy),
            DegreesLatitude = new double?(coordinate.Point.Position.Latitude),
            DegreesLongitude = new double?(coordinate.Point.Position.Longitude),
            SequenceNumber = new long?(LiveLocationManager.Instance.getNextSequenceNumber())
          }
        };
        nullable1 = coordinate.Heading;
        if (nullable1.HasValue)
        {
          nullable1 = coordinate.Heading;
          if (!double.IsNaN(nullable1.Value))
          {
            WhatsApp.ProtoBuf.Message.LiveLocationMessage locationMessageField = msg.LiveLocationMessageField;
            nullable1 = coordinate.Heading;
            uint? nullable2 = new uint?((uint) nullable1.Value);
            locationMessageField.DegreesClockwiseFromMagneticNorth = nullable2;
          }
        }
        nullable1 = coordinate.Speed;
        if (nullable1.HasValue)
        {
          nullable1 = coordinate.Speed;
          if (!double.IsNaN(nullable1.Value))
          {
            WhatsApp.ProtoBuf.Message.LiveLocationMessage locationMessageField = msg.LiveLocationMessageField;
            nullable1 = coordinate.Speed;
            float? nullable3 = new float?((float) nullable1.Value);
            locationMessageField.SpeedInMps = nullable3;
          }
        }
        byte[] encAttributes = connection.Encryption.EncryptFastRatchetPayload(msg.ToPlainText(), LiveLocationManager.LocationGroupJid);
        int totalSeconds = (int) DateTimeOffset.Now.Subtract(coordinate.Timestamp).TotalSeconds;
        connection.SendReportLocation(encAttributes, totalSeconds);
        if (AppState.GetConnection().EventHandler.Qr.Session.Active)
          AppState.GetConnection().SendQrLocationUpdate(Settings.MyJid, totalSeconds, msg);
        LiveLocationSession.Instance.LocationUpdateSent(backOffStage);
        this.lastReportedLocationInfo.OnNewLocation(coordinate);
      }
    }

    private class LastReportedLocationInfo
    {
      public const int TimeIntervalBetweenLocationUpdates = 120;
      private const int FastestTimeIntervalBetweenLocationUpdates = 40;
      private const int LocationAccuracyForFastReports = 80;
      private DateTime Timestamp = DateTime.MinValue;
      private int Accuracy;
      private long Latitude;
      private long Longitude;
      private DateTime LastReportTime = DateTime.MinValue;

      private LastReportedLocationInfo()
      {
      }

      public static LocationSharingService.LastReportedLocationInfo Restore()
      {
        LocationSharingService.LastReportedLocationInfo reportedLocationInfo1 = new LocationSharingService.LastReportedLocationInfo();
        byte[] reportedLocationInfo2 = Settings.LastReportedLocationInfo;
        if (reportedLocationInfo2 != null)
        {
          try
          {
            BinaryData binaryData = new BinaryData(reportedLocationInfo2);
            if (binaryData.ReadByte(0) != (byte) 2)
            {
              Log.l("llss", "incorrect version");
            }
            else
            {
              reportedLocationInfo1.Timestamp = DateTime.FromBinary(binaryData.ReadLong64(1));
              reportedLocationInfo1.Accuracy = binaryData.ReadInt32(9);
              reportedLocationInfo1.Latitude = binaryData.ReadLong64(13);
              reportedLocationInfo1.Longitude = binaryData.ReadLong64(21);
              reportedLocationInfo1.LastReportTime = DateTime.FromBinary(binaryData.ReadLong64(29));
            }
          }
          catch (Exception ex)
          {
            Log.l(ex, "error when trying to restore LastReportedLocationInfo");
          }
        }
        return reportedLocationInfo1;
      }

      public void OnNewLocation(Geocoordinate gcoord)
      {
        this.Timestamp = gcoord.Timestamp.UtcDateTime;
        this.Accuracy = (int) gcoord.Accuracy;
        this.Latitude = (long) gcoord.Point.Position.Latitude;
        this.Longitude = (long) gcoord.Point.Position.Longitude;
        this.LastReportTime = FunRunner.CurrentServerTimeUtc;
        BinaryData binaryData = new BinaryData();
        binaryData.AppendByte((byte) 2);
        binaryData.AppendLong64(this.Timestamp.ToBinary());
        binaryData.AppendInt32(this.Accuracy);
        binaryData.AppendLong64(this.Latitude);
        binaryData.AppendLong64(this.Longitude);
        binaryData.AppendLong64(this.LastReportTime.ToBinary());
        Settings.LastReportedLocationInfo = binaryData.Get();
      }

      public bool isBetterLocation(Geocoordinate gcoord)
      {
        double num = this.SecondsSinceLastShare();
        if ((double) this.Accuracy > gcoord.Accuracy || num > 120.0 || this.DistanceBetweenLastLocation(gcoord) > Math.Max(10.0, gcoord.Accuracy))
          return true;
        return num > 40.0 && gcoord.Accuracy < 80.0;
      }

      public double SecondsSinceLastShare()
      {
        return FunRunner.CurrentServerTimeUtc.Subtract(this.LastReportTime).TotalSeconds;
      }

      public TimeSpan GetTimeSinceLastLocationTimestamp()
      {
        return DateTime.UtcNow.Subtract(this.Timestamp);
      }

      private double DistanceBetweenLastLocation(Geocoordinate gcoord)
      {
        return new GeoCoordinate((double) this.Latitude, (double) this.Longitude).GetDistanceTo(new GeoCoordinate(gcoord.Point.Position.Latitude, gcoord.Point.Position.Longitude));
      }
    }
  }
}
