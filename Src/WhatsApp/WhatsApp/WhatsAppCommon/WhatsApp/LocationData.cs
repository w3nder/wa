// Decompiled with JetBrains decompiler
// Type: WhatsApp.LocationData
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Device.Location;


namespace WhatsApp
{
  public class LocationData
  {
    public float? DurationInSeconds;
    public uint? AccuracyInMeters;
    public float? SpeedInMps;
    public uint? DegreesClockwiseFromMagneticNorth;
    public DateTime Timestamp;

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public string Caption { get; set; }

    public string PlaceUrl { get; set; }

    public string PlaceDetails { get; set; }

    public byte[] ThumbnailBytes { get; set; }

    public byte[] LargeThumbnailBytes { get; set; }

    public static LocationData CreateFromMessage(Message msg)
    {
      LocationData fromMessage = (LocationData) null;
      if (msg.MediaWaType == FunXMPP.FMessage.Type.Location || msg.MediaWaType == FunXMPP.FMessage.Type.LiveLocation)
        fromMessage = new LocationData()
        {
          Latitude = new double?(msg.Latitude),
          Longitude = new double?(msg.Longitude),
          PlaceUrl = msg.LocationUrl,
          PlaceDetails = msg.LocationDetails,
          ThumbnailBytes = msg.BinaryData
        };
      if (msg.MediaWaType == FunXMPP.FMessage.Type.LiveLocation)
      {
        fromMessage.DurationInSeconds = new float?((float) msg.MediaDurationSeconds);
        MessageProperties.LiveLocationProperties locationPropertiesField = msg.InternalProperties?.LiveLocationPropertiesField;
        if (locationPropertiesField != null)
        {
          fromMessage.AccuracyInMeters = locationPropertiesField.AccuracyInMeters;
          fromMessage.Caption = locationPropertiesField.Caption;
          fromMessage.DegreesClockwiseFromMagneticNorth = locationPropertiesField.DegreesClockwiseFromMagneticNorth;
          fromMessage.SpeedInMps = locationPropertiesField.SpeedInMps;
        }
      }
      return fromMessage;
    }

    public static LocationData CreateFromGeoCoordinate(GeoCoordinate coord, DateTime timestamp)
    {
      return new LocationData()
      {
        AccuracyInMeters = new uint?((uint) coord.HorizontalAccuracy),
        DegreesClockwiseFromMagneticNorth = new uint?((uint) coord.Course),
        Latitude = new double?(coord.Latitude),
        Longitude = new double?(coord.Longitude),
        SpeedInMps = new float?((float) coord.Speed),
        Timestamp = timestamp
      };
    }

    public static LocationData CreateFromLiveLocationMessage(
      WhatsApp.ProtoBuf.Message.LiveLocationMessage message,
      DateTime timestamp)
    {
      return new LocationData()
      {
        AccuracyInMeters = message.AccuracyInMeters,
        DegreesClockwiseFromMagneticNorth = message.DegreesClockwiseFromMagneticNorth,
        Latitude = message.DegreesLatitude,
        Longitude = message.DegreesLongitude,
        SpeedInMps = message.SpeedInMps,
        Timestamp = timestamp
      };
    }

    public void PopulateMessage(Message msg)
    {
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        db.GetMessageById(msg.MessageID);
        if (msg.Status.IsDeliveredToServer())
          return;
        msg.Latitude = this.Latitude.Value;
        msg.Longitude = this.Longitude.Value;
        msg.LocationDetails = this.PlaceDetails;
        msg.LocationUrl = this.PlaceUrl;
        msg.BinaryData = this.ThumbnailBytes;
        msg.MediaName = (string) null;
        if (this.LargeThumbnailBytes != null && msg.SaveBinaryDataFile(this.LargeThumbnailBytes))
          db.LocalFileAddRef(msg.DataFileName, LocalFileType.Thumbnail);
        msg.Status = FunXMPP.FMessage.Status.Unsent;
        db.SubmitChanges();
      }));
      AppState.SendMessage(AppState.ClientInstance.GetConnection(), msg);
      AppState.QrPersistentAction.NotifyMessage(msg, QrMessageForwardType.Update);
    }
  }
}
