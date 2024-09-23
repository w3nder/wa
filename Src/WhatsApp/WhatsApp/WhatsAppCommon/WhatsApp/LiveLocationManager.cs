// Decompiled with JetBrains decompiler
// Type: WhatsApp.LiveLocationManager
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;


namespace WhatsApp
{
  public class LiveLocationManager
  {
    public static readonly string LogHdr = "llman";
    private static LiveLocationManager instance = (LiveLocationManager) null;
    private static object instancelock = new object();
    private LiveLocationManager.LocationSharingData locShareCache;
    private object locShareDataLock = new object();
    private object sequenceidlock = new object();
    private const int FastRatchetScale = 3;
    private LocationSharingService LocationService = new LocationSharingService();
    public static readonly string LocationGroupJid = "location@broadcast";
    private Dictionary<string, long> currentSubscriptions = new Dictionary<string, long>();
    private static HashSet<string> deniersCache;
    private static object denierslock = new object();

    public static LiveLocationManager Instance
    {
      get
      {
        if (LiveLocationManager.instance == null)
        {
          lock (LiveLocationManager.instancelock)
          {
            if (LiveLocationManager.instance == null)
              LiveLocationManager.instance = new LiveLocationManager();
          }
        }
        return LiveLocationManager.instance;
      }
    }

    private LiveLocationManager.LocationSharingData LocShareData
    {
      get
      {
        if (this.locShareCache == null)
        {
          lock (this.locShareDataLock)
          {
            if (this.locShareCache == null)
            {
              byte[] liveLocationData = Settings.LiveLocationData;
              try
              {
                if (liveLocationData != null)
                {
                  this.locShareCache = JsonConvert.DeserializeObject<LiveLocationManager.LocationSharingData>(Encoding.UTF8.GetString(liveLocationData, 0, liveLocationData.Length), new JsonSerializerSettings()
                  {
                    TypeNameHandling = TypeNameHandling.Auto
                  });
                  this.ClearExpiredLocations();
                }
              }
              catch (Exception ex)
              {
                Log.LogException(ex, "Exception extracted saved locations");
                Settings.LiveLocationData = (byte[]) null;
                this.locShareCache = (LiveLocationManager.LocationSharingData) null;
              }
              if (this.locShareCache == null)
                this.locShareCache = new LiveLocationManager.LocationSharingData()
                {
                  SendingLocationRecord = new Dictionary<string, Tuple<double, List<string>>>(),
                  ReceivingLocationRecord = new Dictionary<string, Dictionary<string, int?>>(),
                  UserLocations = new Dictionary<string, LocationData>()
                };
            }
          }
        }
        return this.locShareCache;
      }
    }

    public long getNextSequenceNumber()
    {
      lock (this.sequenceidlock)
      {
        long num = Settings.LiveLocationSequenceNumber;
        if (num == 0L)
          num = FunRunner.CurrentServerTimeUtc.ToUnixTime() * 1000000L;
        long nextSequenceNumber = num + 1L;
        Settings.LiveLocationSequenceNumber = nextSequenceNumber;
        return nextSequenceNumber;
      }
    }

    public LiveLocationManager() => this.locShareCache = this.LocShareData;

    public void DeleteLocationCache()
    {
      Settings.LiveLocationData = (byte[]) null;
      lock (this.locShareDataLock)
        this.locShareCache = (LiveLocationManager.LocationSharingData) null;
    }

    private void SaveLocationSharingData()
    {
      if (this.locShareCache.UserLocations.Count == 0 && this.locShareCache.ReceivingLocationRecord.Count == 0 && this.locShareCache.SendingLocationRecord.Count == 0)
      {
        Settings.LiveLocationData = (byte[]) null;
      }
      else
      {
        try
        {
          Settings.LiveLocationData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject((object) this.LocShareData, new JsonSerializerSettings()
          {
            TypeNameHandling = TypeNameHandling.All
          }));
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "Exception saving location data");
          Settings.LiveLocationData = (byte[]) null;
        }
      }
    }

    public void ClearExpiredLocationsAndSave()
    {
      lock (this.locShareDataLock)
      {
        this.ClearExpiredLocations();
        this.SaveLocationSharingData();
      }
    }

    private void ClearExpiredLocations()
    {
      this.ClearExpiredUserLocations();
      this.ClearExpiredReceivingLocationRecords();
      this.ClearExpiredSendingLocationRecords();
    }

    private void ClearExpiredUserLocations()
    {
      List<string> stringList = new List<string>();
      foreach (string key in this.LocShareData.UserLocations.Keys)
      {
        if (this.LocShareData.UserLocations[key].Timestamp + TimeSpan.FromHours(4.0) < FunRunner.CurrentServerTimeUtc)
          stringList.Add(key);
      }
      foreach (string key in stringList)
        this.LocShareData.UserLocations.Remove(key);
    }

    private void ClearExpiredReceivingLocationRecords()
    {
      HashSet<string> stringSet1 = new HashSet<string>();
      foreach (string key1 in this.LocShareData.ReceivingLocationRecord.Keys)
      {
        long unixTime = FunRunner.CurrentServerTimeUtc.ToUnixTime();
        HashSet<string> stringSet2 = new HashSet<string>();
        foreach (string key2 in this.LocShareData.ReceivingLocationRecord[key1].Keys)
        {
          int? nullable = this.LocShareData.ReceivingLocationRecord[key1][key2];
          if (nullable.HasValue)
          {
            nullable = this.LocShareData.ReceivingLocationRecord[key1][key2];
            if ((long) nullable.Value >= unixTime)
              continue;
          }
          stringSet2.Add(key2);
        }
        foreach (string str in stringSet2)
        {
          Log.d(LiveLocationManager.LogHdr, "ClearExpiredReceivingLocationRecords for jid {0} participantJid {1}", (object) key1, (object) str);
          this.LocShareData.ReceivingLocationRecord[key1].Remove(str);
          this.RemoveReceiverLocationIfNeeded(str);
          stringSet1.Add(key1);
        }
      }
      foreach (string key in stringSet1)
      {
        Dictionary<string, int?> dictionary = (Dictionary<string, int?>) null;
        if (this.LocShareData.ReceivingLocationRecord.TryGetValue(key, out dictionary))
        {
          if (dictionary.Count == 0)
            this.LocShareData.ReceivingLocationRecord.Remove(key);
        }
        else
        {
          Log.l(LiveLocationManager.LogHdr, string.Format("Participant removed from group that does not exist, jid {0} ", (object) key));
          Log.SendCrashLog(new Exception("Participant removed from group that does not exist"), "Participant removed from group that does not exist");
        }
      }
    }

    private void ClearExpiredSendingLocationRecords()
    {
      long unixTime = FunRunner.CurrentServerTimeUtc.ToUnixTime();
      double num1 = (double) unixTime + TimeSpan.FromHours(8.0).TotalSeconds;
      double num2 = (double) unixTime + TimeSpan.FromDays(2.0).TotalSeconds;
      List<string> stringList = new List<string>();
      foreach (string key in this.LocShareData.SendingLocationRecord.Keys)
      {
        double num3 = this.LocShareData.SendingLocationRecord[key].Item1;
        if ((double) unixTime > num3)
        {
          stringList.Add(key);
          this.LogLiveLocationExpired(key);
        }
        if (num3 > num2)
        {
          stringList.Add(key);
          Log.SendCrashLog(new Exception("force removing sender record"), string.Format("jid {0} now {1} expiry {2}", (object) Settings.MyJid, (object) unixTime, (object) num3));
        }
        else if (num3 > num1)
          Log.l(LiveLocationManager.LogHdr, "sender record found that is > 8 hours for jid {0} now {1} expiry {2}", (object) Settings.MyJid, (object) unixTime, (object) num3);
      }
      foreach (string key in stringList)
      {
        Log.d(LiveLocationManager.LogHdr, "ClearExpiredSendingLocationRecords for jid {0}", (object) key);
        this.LocShareData.SendingLocationRecord.Remove(key);
      }
      if (this.LocShareData.SendingLocationRecord.Keys.Count != 0)
        return;
      this.LocShareData.UserLocations.Remove(Settings.MyJid);
    }

    private void LogLiveLocationExpired(string jid)
    {
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        Message liveLocationMessage = db.GetLatestLiveLocationMessage(jid, Settings.MyJid);
        if (liveLocationMessage == null)
          return;
        FieldStats.ReportLiveLocationSharingSession(new long?((long) liveLocationMessage.MediaDurationSeconds), wam_enum_live_location_sharing_session_ended_reason.EXPIRED);
      }));
    }

    private void RemoveReceiverLocationIfNeeded(string participantJid)
    {
      bool flag = false;
      foreach (string key in this.LocShareData.ReceivingLocationRecord.Keys)
      {
        if (this.LocShareData.ReceivingLocationRecord[key].ContainsKey(participantJid))
          flag = true;
      }
      if (flag)
        return;
      this.LocShareData.UserLocations.Remove(participantJid);
    }

    public void NewIncomingLocation(
      string participantJid,
      int elapsed,
      WhatsApp.ProtoBuf.Message.LiveLocationMessage locationMessage)
    {
      DateTime timestamp = FunRunner.CurrentServerTimeUtc.Subtract(TimeSpan.FromSeconds((double) elapsed));
      LocationData liveLocationMessage = LocationData.CreateFromLiveLocationMessage(locationMessage, timestamp);
      this.NewIncomingLocation(participantJid, liveLocationMessage);
    }

    public void NewIncomingLocation(string participantJid, GeoCoordinate coord)
    {
      LocationData fromGeoCoordinate = LocationData.CreateFromGeoCoordinate(coord, FunRunner.CurrentServerTimeUtc);
      this.NewIncomingLocation(participantJid, fromGeoCoordinate);
    }

    private void NewIncomingLocation(string participantJid, LocationData data)
    {
      lock (this.locShareDataLock)
      {
        this.LocShareData.UserLocations[participantJid] = data;
        this.SaveLocationSharingData();
      }
    }

    public void UpdateReceiverRecord(string jid, string participantJid, int? expiration)
    {
      if (!expiration.HasValue || expiration.Value == 0)
        return;
      lock (this.locShareDataLock)
      {
        Log.d(LiveLocationManager.LogHdr, "UpdateReceiverRecord for jid {0} participantJid {1}", (object) jid, (object) participantJid);
        if (this.IsReceivingLocationFromJid(jid))
          this.LocShareData.ReceivingLocationRecord[jid][participantJid] = expiration;
        else
          this.LocShareData.ReceivingLocationRecord[jid] = new Dictionary<string, int?>()
          {
            {
              participantJid,
              expiration
            }
          };
        this.SaveLocationSharingData();
      }
    }

    private void RemoveReceiverRecord(string jid, string participantJid)
    {
      lock (this.locShareDataLock)
      {
        if (!this.IsReceivingLocationFromJid(jid) || !this.LocShareData.ReceivingLocationRecord[jid].Keys.Contains<string>(participantJid))
          return;
        Log.d(LiveLocationManager.LogHdr, "RemoveSenderRecord for jid {0} participantJid {1}", (object) jid, (object) participantJid);
        this.LocShareData.ReceivingLocationRecord[jid].Remove(participantJid);
        if (this.LocShareData.ReceivingLocationRecord[jid].Count == 0)
          this.LocShareData.ReceivingLocationRecord.Remove(jid);
        this.RemoveReceiverLocationIfNeeded(participantJid);
        this.SaveLocationSharingData();
      }
    }

    public void UpdateSenderRecord(string jid, long endtime, List<string> participants)
    {
      lock (this.locShareDataLock)
      {
        if (this.LocShareData.SendingLocationRecord.ContainsKey(jid))
          this.LocShareData.SendingLocationRecord.Remove(jid);
        Log.d(LiveLocationManager.LogHdr, "UpdateSenderRecord for jid {0}", (object) jid);
        this.LocShareData.SendingLocationRecord[jid] = new Tuple<double, List<string>>((double) endtime, participants);
        this.SaveLocationSharingData();
      }
    }

    private void RemoveSenderRecord(string jid)
    {
      lock (this.locShareDataLock)
      {
        if (!this.LocShareData.SendingLocationRecord.ContainsKey(jid))
          return;
        Log.d(LiveLocationManager.LogHdr, "RemoveSenderRecord for jid {0}", (object) jid);
        this.LocShareData.SendingLocationRecord.Remove(jid);
        if (this.LocShareData.SendingLocationRecord.Keys.Count == 0)
          this.LocShareData.UserLocations.Remove(Settings.MyJid);
        this.SaveLocationSharingData();
      }
    }

    public int GetJidsReceivingForGroupCount(string gjid)
    {
      return this.GetJidsReceivingForGroup(gjid).Keys.Count;
    }

    public int GetJidsSendingToCount() => this.GetJidsSendingTo().Keys.Count;

    public Dictionary<string, int?> GetJidsSharingForGroup(string gjid)
    {
      Dictionary<string, int?> jidsSharingForGroup = new Dictionary<string, int?>();
      if (this.IsSharingLocationWithJid(gjid))
        jidsSharingForGroup.Add(Settings.MyJid, new int?((int) this.LocShareData.SendingLocationRecord[gjid].Item1));
      foreach (KeyValuePair<string, int?> keyValuePair in this.GetJidsReceivingForGroup(gjid))
        jidsSharingForGroup.Add(keyValuePair.Key, keyValuePair.Value);
      return jidsSharingForGroup;
    }

    private Dictionary<string, int?> GetJidsReceivingForGroup(string gjid)
    {
      lock (this.locShareDataLock)
        return this.IsReceivingLocationFromJid(gjid) ? new Dictionary<string, int?>((IDictionary<string, int?>) this.LocShareData.ReceivingLocationRecord[gjid]) : new Dictionary<string, int?>();
    }

    public Dictionary<string, Dictionary<string, int?>> GetJidsReceivingFrom()
    {
      lock (this.locShareDataLock)
        return new Dictionary<string, Dictionary<string, int?>>((IDictionary<string, Dictionary<string, int?>>) this.LocShareData.ReceivingLocationRecord);
    }

    public Dictionary<string, Tuple<double, List<string>>> GetJidsSendingTo()
    {
      lock (this.locShareDataLock)
        return new Dictionary<string, Tuple<double, List<string>>>((IDictionary<string, Tuple<double, List<string>>>) this.LocShareData.SendingLocationRecord);
    }

    public LocationData GetLocationDataForJid(string jid)
    {
      lock (this.locShareDataLock)
        return this.LocShareData.UserLocations.ContainsKey(jid) ? this.LocShareData.UserLocations[jid] : (LocationData) null;
    }

    public bool IsSharingLocationWithJid(string jid)
    {
      return this.LocShareData.SendingLocationRecord.ContainsKey(jid);
    }

    public bool IsSharingLocationWithParticipant(string participantJid)
    {
      foreach (string key in this.LocShareData.SendingLocationRecord.Keys)
      {
        if (this.LocShareData.SendingLocationRecord[key].Item2.Contains(participantJid))
          return true;
      }
      return false;
    }

    public bool IsReceivingLocationFromJid(string jid)
    {
      return this.LocShareData.ReceivingLocationRecord.ContainsKey(jid);
    }

    public void ProcessUserBlock(string participantJid)
    {
      this.ReceiveLocationDisabled(participantJid);
      this.DisableLocationSharing(participantJid, wam_enum_live_location_sharing_session_ended_reason.OTHER);
    }

    public void ProcessUserRemovedFromGroup(string jid, string participantJid)
    {
      if (participantJid != Settings.MyJid)
      {
        this.ReceiveLocationDisabled(jid, participantJid);
      }
      else
      {
        try
        {
          foreach (string key in LiveLocationManager.Instance.GetJidsReceivingForGroup(jid).Keys)
            this.ReceiveLocationDisabled(jid, key);
          this.DisableLocationSharing(jid, wam_enum_live_location_sharing_session_ended_reason.OTHER);
        }
        catch (Exception ex)
        {
          string context = LiveLocationManager.LogHdr + " in ProcessUserRemovedFromGroup";
          Log.LogException(ex, context);
        }
      }
    }

    public void ReceiveLocationDisabled(string participantJid)
    {
      lock (this.locShareDataLock)
      {
        List<string> stringList = new List<string>();
        foreach (string key in this.LocShareData.ReceivingLocationRecord.Keys)
        {
          if (this.LocShareData.ReceivingLocationRecord[key].ContainsKey(participantJid))
          {
            this.LocShareData.ReceivingLocationRecord[key].Remove(participantJid);
            this.StopLatestLiveLocationMessage(key, participantJid);
            stringList.Add(key);
          }
        }
        foreach (string key in stringList)
        {
          if (this.LocShareData.ReceivingLocationRecord[key].Count == 0)
            this.LocShareData.ReceivingLocationRecord.Remove(key);
        }
        this.RemoveReceiverLocationIfNeeded(participantJid);
      }
    }

    public void ReceiveLocationDisabled(string jid, string participantJid)
    {
      try
      {
        this.RemoveReceiverRecord(jid, participantJid);
        this.StopLatestLiveLocationMessage(jid, participantJid);
      }
      catch (Exception ex)
      {
        string context = LiveLocationManager.LogHdr + " in ReceiveGroupLocationDisabled";
        Log.LogException(ex, context);
      }
    }

    public void LiveLocationMessageUpdated(Message m)
    {
      string keyRemoteJid = m.KeyRemoteJid;
      string senderJid = m.GetSenderJid();
      int num = (int) (m.TimestampLong + (long) m.MediaDurationSeconds);
      LocationData fromMessage = LocationData.CreateFromMessage(m);
      fromMessage.Timestamp = FunRunner.CurrentServerTimeUtc;
      this.NewIncomingLocation(senderJid, fromMessage);
      this.UpdateReceiverRecord(keyRemoteJid, senderJid, new int?(num));
    }

    private long? StopLatestLiveLocationMessage(string jid, string participantJid)
    {
      long? effectiveDuration = new long?();
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        effectiveDuration = (long?) db.GetLatestLiveLocationMessage(jid, participantJid)?.EndLiveLocation();
        db.SubmitChanges();
      }));
      return effectiveDuration;
    }

    public void CompletePendingLiveLocationDataMessage(Message msg)
    {
      this.EnableLocationSharing(msg.KeyRemoteJid, msg.MediaDurationSeconds);
      IDisposable locationSubscription = (IDisposable) null;
      locationSubscription = LocationHelper.ObserveAccurateGeoCoordinate().Subscribe<GeoCoordinate>((Action<GeoCoordinate>) (coord =>
      {
        this.NewIncomingLocation(msg.GetSenderJid(), coord);
        LocationData fromGeoCoordinate = LocationData.CreateFromGeoCoordinate(coord, FunRunner.CurrentServerTimeUtc);
        this.SendLatestLocation(fromGeoCoordinate);
        WebServices.GetCompleteLocationData(fromGeoCoordinate).Subscribe<LocationData>((Action<LocationData>) (newLocData =>
        {
          newLocData.PopulateMessage(msg);
          FieldStats.ReportLiveLocationMessageStatus(msg.FunTimestamp.Value, wam_enum_live_location_message_status_result.OK);
        }));
        locationSubscription?.Dispose();
      }), (Action<Exception>) (e =>
      {
        Log.l(LiveLocationManager.LogHdr, "CompletePendingLiveLocationDataMessage: " + e.Message);
        FieldStats.ReportLiveLocationMessageStatus(msg.FunTimestamp.Value, wam_enum_live_location_message_status_result.FAILED);
        locationSubscription?.Dispose();
      }));
    }

    public void EnableLocationSharing(string jid, int durationInSeconds)
    {
      List<string> participantJids = (List<string>) null;
      if (JidHelper.IsGroupJid(jid))
        MessagesContext.Run((MessagesContext.MessagesCallback) (db => participantJids = db.GetParticipants(jid, true).Keys.ToList<string>()));
      else
        participantJids = new List<string>() { jid };
      if (participantJids == null)
        return;
      bool flag = this.IsSharingLocationWithJid(jid);
      long unixTime = FunRunner.CurrentServerTimeUtc.AddSeconds((double) durationInSeconds).ToUnixTime();
      this.UpdateSenderRecord(jid, unixTime, participantJids);
      if (flag || participantJids.Count == 0)
        Log.d(LiveLocationManager.LogHdr, "Not sending key distribution notification: {0} count: {1}", (object) flag, (object) participantJids.Count);
      else
        this.SendEnableLocationSharing(participantJids);
    }

    public void SendEnableLocationSharing(List<string> participantJids)
    {
      try
      {
        Action retryAction = (Action) (() => this.SendEnableLocationSharing(participantJids));
        IEnumerable<FunXMPP.FMessage.Participant> participants = AppState.GetConnection().Encryption.EncryptLiveLocationKeys((IEnumerable<string>) participantJids, retryAction);
        if (participants == null)
          return;
        AppState.SchedulePersistentAction(PersistentAction.SendEnableLocationSharing(participants));
      }
      catch (Exception ex)
      {
        string context = LiveLocationManager.LogHdr + " in SendEnableLocationSharing";
        Log.SendCrashLog(ex, context, logOnlyForRelease: true);
      }
    }

    public void DisableAllLocationSharing()
    {
      foreach (string key in this.GetJidsSendingTo().Keys)
        this.DisableLocationSharing(key, wam_enum_live_location_sharing_session_ended_reason.USER_CANCELED);
    }

    public void DisableLocationSharing(
      string jid,
      wam_enum_live_location_sharing_session_ended_reason reason)
    {
      this.DisableLocationSharing(jid, (string) null, reason);
    }

    public void DisableLocationSharing(
      string jid,
      string id,
      wam_enum_live_location_sharing_session_ended_reason reason)
    {
      try
      {
        this.RemoveSenderRecord(jid);
        FieldStats.ReportLiveLocationSharingSession(this.StopLatestLiveLocationMessage(jid, Settings.MyJid), reason);
      }
      catch (Exception ex)
      {
        string context = LiveLocationManager.LogHdr + " in DisableLocationSharing";
        Log.SendCrashLog(ex, context, logOnlyForRelease: true);
      }
      AppState.GetConnection().Encryption.OnLocationSharingDisabled(LiveLocationManager.LocationGroupJid);
      AppState.SchedulePersistentAction(PersistentAction.SendDisableLocationSharing(jid, id, this.getNextSequenceNumber().ToString()));
    }

    private void SendLatestLocation(LocationData locData)
    {
      FunXMPP.Connection connection = AppState.GetConnection();
      if (connection != null)
      {
        WhatsApp.ProtoBuf.Message liveLocationMessage = this.CreateLiveLocationMessage(locData);
        byte[] encAttributes = connection.Encryption.EncryptFastRatchetPayload(liveLocationMessage.ToPlainText(), LiveLocationManager.LocationGroupJid);
        connection.SendReportLocation(encAttributes);
      }
      else
        Log.l(LiveLocationManager.LogHdr, "Bypass send lastest, no connection");
    }

    public WhatsApp.ProtoBuf.Message CreateLiveLocationMessage(LocationData locData)
    {
      return new WhatsApp.ProtoBuf.Message()
      {
        LiveLocationMessageField = new WhatsApp.ProtoBuf.Message.LiveLocationMessage()
        {
          AccuracyInMeters = locData.AccuracyInMeters,
          DegreesClockwiseFromMagneticNorth = locData.DegreesClockwiseFromMagneticNorth,
          DegreesLatitude = locData.Latitude,
          DegreesLongitude = locData.Longitude,
          SequenceNumber = new long?(this.getNextSequenceNumber()),
          SpeedInMps = locData.SpeedInMps
        }
      };
    }

    public void SubscribeToLocationUpdates(string gjid, bool hasParticipants)
    {
      this.ClearExpiredSubscriptions();
      if (!this.currentSubscriptions.ContainsKey(gjid))
      {
        int num = Settings.LiveLocationSubscriptionDuration - 5;
        if (num < 0)
        {
          Log.l(LiveLocationManager.LogHdr, "Duration was less than 5 seconds");
          num = 0;
        }
        long unixTime = FunRunner.CurrentServerTimeUtc.AddSeconds((double) num).ToUnixTime();
        this.currentSubscriptions.Add(gjid, unixTime);
        AppState.SchedulePersistentAction(PersistentAction.SendSubscribeToLocationUpdates(gjid, hasParticipants));
      }
      else
        Log.l(LiveLocationManager.LogHdr, "Skip sending subscription notification");
    }

    public void UnsubscribeToLocationUpdates(string gjid, string id)
    {
      this.currentSubscriptions.Remove(gjid);
      AppState.SchedulePersistentAction(PersistentAction.SendUnsubscribeToLocationUpdates(gjid, id));
    }

    private void ClearExpiredSubscriptions()
    {
      long unixTime = FunRunner.CurrentServerTimeUtc.ToUnixTime();
      List<string> stringList = new List<string>();
      foreach (string key in this.currentSubscriptions.Keys)
      {
        if (this.currentSubscriptions[key] < unixTime)
          stringList.Add(key);
      }
      foreach (string key in stringList)
        this.currentSubscriptions.Remove(key);
    }

    public void StartLocationReporting(string gjid, string id, int? duration)
    {
      bool successful = this.IsSharingLocationWithJid(gjid);
      Log.l(LiveLocationManager.LogHdr, "StartLocationReporting: gjid {0} duration: {1} sharing: {2}.", (object) gjid, (object) duration, (object) successful);
      AppState.GetConnection()?.SendStartLocationReportingResponse(gjid, id, successful);
      if (!successful)
        return;
      this.LocationService.BeginLocationSharing(duration);
    }

    public void StopLocationReporting(string id)
    {
      Log.l(LiveLocationManager.LogHdr, nameof (StopLocationReporting));
      AppState.GetConnection()?.SendStopLocationReportingResponse(id);
      this.LocationService.StopLocationSharingAndPostSession();
    }

    private static HashSet<string> Deniers
    {
      get
      {
        if (LiveLocationManager.deniersCache == null)
        {
          lock (LiveLocationManager.denierslock)
          {
            if (LiveLocationManager.deniersCache == null)
            {
              DataContractJsonSerializer contractJsonSerializer = new DataContractJsonSerializer(typeof (HashSet<string>));
              byte[] liveLocationDeniers = Settings.LiveLocationDeniers;
              if (liveLocationDeniers != null)
              {
                using (MemoryStream memoryStream = new MemoryStream(liveLocationDeniers, 0, liveLocationDeniers.Length, false))
                  LiveLocationManager.deniersCache = contractJsonSerializer.ReadObject((Stream) memoryStream) as HashSet<string>;
              }
              else
                LiveLocationManager.deniersCache = new HashSet<string>();
            }
          }
        }
        return LiveLocationManager.deniersCache;
      }
    }

    public static bool HasLocationRetryDenied(string jid)
    {
      lock (LiveLocationManager.denierslock)
        return LiveLocationManager.Deniers.Contains(jid);
    }

    public static void SetLocationRetryAllowed(string jid, bool allowed)
    {
      lock (LiveLocationManager.denierslock)
      {
        if (allowed)
        {
          if (LiveLocationManager.Deniers.Contains(jid))
            LiveLocationManager.Deniers.Remove(jid);
        }
        else
          LiveLocationManager.Deniers.Add(jid);
        using (MemoryStream memoryStream = new MemoryStream())
        {
          new DataContractJsonSerializer(typeof (HashSet<string>)).WriteObject((Stream) memoryStream, (object) LiveLocationManager.Deniers);
          Settings.LiveLocationDeniers = memoryStream.ToArray();
        }
      }
    }

    private class LocationSharingData
    {
      public Dictionary<string, Tuple<double, List<string>>> SendingLocationRecord;
      public Dictionary<string, LocationData> UserLocations;
      public Dictionary<string, Dictionary<string, int?>> ReceivingLocationRecord;
    }
  }
}
