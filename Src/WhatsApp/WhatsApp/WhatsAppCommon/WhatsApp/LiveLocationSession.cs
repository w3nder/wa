// Decompiled with JetBrains decompiler
// Type: WhatsApp.LiveLocationSession
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using WhatsApp.Events;
using WhatsApp.WaCollections;


namespace WhatsApp
{
  internal class LiveLocationSession
  {
    private static LiveLocationSession instance = (LiveLocationSession) null;
    private static object instancelock = new object();
    private const string LogHdr = "lls";
    private const int DefaultSharingPeriodStartHour = 6;
    private long sharingTime;
    private DateTime sharingStartLocalDateTime;
    private long segmentNumber;
    private LiveLocationReportingSession reportingSessionEvent;
    private LiveLocationReportingSessionSegment reportingSessionSegmentEvent;
    private const int LiveLocationSessionVersion = 1;

    public static LiveLocationSession Instance
    {
      get
      {
        if (LiveLocationSession.instance == null)
        {
          lock (LiveLocationSession.instancelock)
          {
            if (LiveLocationSession.instance == null)
              LiveLocationSession.instance = new LiveLocationSession();
          }
        }
        return LiveLocationSession.instance;
      }
    }

    private LiveLocationSession()
    {
      byte[] liveLocationSession = Settings.LiveLocationSession;
      if (liveLocationSession != null)
      {
        try
        {
          BinaryData binaryData1 = new BinaryData(liveLocationSession);
          if (binaryData1.ReadByte(0) != (byte) 1)
          {
            Log.l("lls", "incorrect version");
            this.SetupDefaultLiveLocationSession();
          }
          else
          {
            this.sharingTime = binaryData1.ReadLong64(1);
            this.sharingStartLocalDateTime = DateTime.FromBinary(binaryData1.ReadLong64(9));
            this.segmentNumber = binaryData1.ReadLong64(17);
            int num1 = 25;
            BinaryData binaryData2 = binaryData1;
            int offset1 = num1;
            int offset2 = offset1 + 1;
            if (binaryData2.ReadByte(offset1) == (byte) 1)
            {
              this.reportingSessionEvent = new LiveLocationReportingSession()
              {
                sessionT = new long?(binaryData1.ReadLong64(offset2)),
                numberOfUpdates = new long?(binaryData1.ReadLong64(offset2 + 8)),
                batteryLevelChange = new double?((double) binaryData1.ReadLong64(offset2 + 16))
              };
              offset2 += 24;
            }
            BinaryData binaryData3 = binaryData1;
            int offset3 = offset2;
            int offset4 = offset3 + 1;
            if (binaryData3.ReadByte(offset3) == (byte) 1)
            {
              this.reportingSessionSegmentEvent = new LiveLocationReportingSessionSegment()
              {
                segmentT = new long?(binaryData1.ReadLong64(offset4)),
                segmentNumber = new long?(binaryData1.ReadLong64(offset4 + 8)),
                segmentNumberOfUpdates = new long?(binaryData1.ReadLong64(offset4 + 16)),
                segmentBatteryLevelChange = new double?((double) binaryData1.ReadLong64(offset4 + 24)),
                segmentBatteryCharging = new bool?(binaryData1.ReadByte(offset4 + 32) > (byte) 0),
                segmentBackoffStage = new wam_enum_live_location_backoff_stage?((wam_enum_live_location_backoff_stage) binaryData1.ReadInt32(offset4 + 33))
              };
              int num2 = offset4 + 37;
            }
            if (!(DateTime.Now > this.sharingStartLocalDateTime.AddHours(24.0)))
              return;
            this.SetupDefaultLiveLocationSession();
          }
        }
        catch (Exception ex)
        {
          Log.l(ex, "error when trying to restore LiveLocationSession");
          this.SetupDefaultLiveLocationSession();
        }
      }
      else
        this.SetupDefaultLiveLocationSession();
    }

    private void SetupDefaultLiveLocationSession()
    {
      this.sharingTime = 0L;
      this.sharingStartLocalDateTime = LiveLocationSession.GetCurrentSharingLocalDateTime();
    }

    private static DateTime GetCurrentSharingLocalDateTime()
    {
      DateTime now = DateTime.Now;
      return new DateTime(now.Year, now.Month, now.Hour < 6 ? now.Day - 1 : now.Day, 6, 0, 0);
    }

    public void InitSession(wam_enum_live_location_backoff_stage backOffStage)
    {
      long unixTime = FunRunner.CurrentServerTimeUtc.ToUnixTime();
      this.InitReportingSession(unixTime);
      this.InitReportingSegment(unixTime, backOffStage);
      this.Save();
    }

    private void InitReportingSession(long currentTime)
    {
      if (this.reportingSessionEvent != null)
        return;
      this.reportingSessionEvent = new LiveLocationReportingSession()
      {
        sessionT = new long?(currentTime),
        numberOfUpdates = new long?(0L),
        batteryLevelChange = new double?((double) AppState.BatteryPercentage)
      };
    }

    private void InitReportingSegment(
      long currentTime,
      wam_enum_live_location_backoff_stage backOffStage)
    {
      if (this.reportingSessionSegmentEvent != null)
      {
        wam_enum_live_location_backoff_stage? segmentBackoffStage = this.reportingSessionSegmentEvent.segmentBackoffStage;
        wam_enum_live_location_backoff_stage locationBackoffStage = backOffStage;
        if ((segmentBackoffStage.GetValueOrDefault() == locationBackoffStage ? (!segmentBackoffStage.HasValue ? 1 : 0) : 1) != 0)
          this.PostSegmentEvent(currentTime);
      }
      if (this.reportingSessionSegmentEvent != null)
        return;
      this.reportingSessionSegmentEvent = new LiveLocationReportingSessionSegment()
      {
        segmentT = new long?(currentTime),
        segmentNumber = new long?(this.segmentNumber++),
        segmentNumberOfUpdates = new long?(0L),
        segmentBatteryLevelChange = new double?((double) AppState.BatteryPercentage),
        segmentBatteryCharging = new bool?(AppState.PowerSourceConnected),
        segmentBackoffStage = new wam_enum_live_location_backoff_stage?(backOffStage)
      };
    }

    public long GetSharingTime()
    {
      if (AppState.PowerSourceConnected && AppState.BatteryPercentage > 70)
        this.sharingTime = 0L;
      return this.reportingSessionSegmentEvent != null && !AppState.PowerSourceConnected ? this.sharingTime + (FunRunner.CurrentServerTimeUtc.ToUnixTime() - this.reportingSessionSegmentEvent.segmentT.Value) : this.sharingTime;
    }

    public void LocationUpdateSent(wam_enum_live_location_backoff_stage backOffStage)
    {
      long unixTime = FunRunner.CurrentServerTimeUtc.ToUnixTime();
      if (this.reportingSessionEvent != null)
      {
        LiveLocationReportingSession reportingSessionEvent = this.reportingSessionEvent;
        long? numberOfUpdates = reportingSessionEvent.numberOfUpdates;
        long num = 1;
        reportingSessionEvent.numberOfUpdates = numberOfUpdates.HasValue ? new long?(numberOfUpdates.GetValueOrDefault() + num) : new long?();
      }
      if (this.reportingSessionSegmentEvent != null)
      {
        LiveLocationReportingSessionSegment sessionSegmentEvent = this.reportingSessionSegmentEvent;
        long? segmentNumberOfUpdates = sessionSegmentEvent.segmentNumberOfUpdates;
        long num = 1;
        sessionSegmentEvent.segmentNumberOfUpdates = segmentNumberOfUpdates.HasValue ? new long?(segmentNumberOfUpdates.GetValueOrDefault() + num) : new long?();
        if ((double) (unixTime - this.reportingSessionSegmentEvent.segmentT.Value) > TimeSpan.FromMinutes(5.0).TotalSeconds)
        {
          this.PostSegmentEvent(unixTime);
          this.InitReportingSegment(unixTime, backOffStage);
        }
      }
      this.Save();
    }

    public void PostSession()
    {
      long unixTime = FunRunner.CurrentServerTimeUtc.ToUnixTime();
      this.PostSessionEvent(unixTime);
      this.PostSegmentEvent(unixTime);
      this.segmentNumber = 0L;
      this.Save();
    }

    private void PostSessionEvent(long currentTime)
    {
      if (this.reportingSessionEvent == null)
      {
        Log.l("lls", "trying to reportingSessionEvent that does not exist");
      }
      else
      {
        LiveLocationReportingSession reportingSessionEvent1 = this.reportingSessionEvent;
        long num1 = currentTime;
        long? sessionT = this.reportingSessionEvent.sessionT;
        long? nullable = sessionT.HasValue ? new long?(num1 - sessionT.GetValueOrDefault()) : new long?();
        reportingSessionEvent1.sessionT = nullable;
        LiveLocationReportingSession reportingSessionEvent2 = this.reportingSessionEvent;
        double? batteryLevelChange1 = reportingSessionEvent2.batteryLevelChange;
        double batteryPercentage = (double) AppState.BatteryPercentage;
        reportingSessionEvent2.batteryLevelChange = batteryLevelChange1.HasValue ? new double?(batteryLevelChange1.GetValueOrDefault() - batteryPercentage) : new double?();
        sessionT = this.reportingSessionEvent.sessionT;
        long num2 = 0;
        if ((sessionT.GetValueOrDefault() > num2 ? (sessionT.HasValue ? 1 : 0) : 0) != 0)
        {
          double? batteryLevelChange2 = this.reportingSessionEvent.batteryLevelChange;
          double num3 = 0.0;
          if ((batteryLevelChange2.GetValueOrDefault() > num3 ? (batteryLevelChange2.HasValue ? 1 : 0) : 0) != 0)
          {
            this.reportingSessionEvent.SaveEvent();
            goto label_6;
          }
        }
        Log.l("lls", "dropping reportingSessionEvent");
label_6:
        this.reportingSessionEvent = (LiveLocationReportingSession) null;
      }
    }

    private void PostSegmentEvent(long currentTime)
    {
      if (this.reportingSessionSegmentEvent == null)
      {
        Log.l("lls", "trying to reportingSessionSegmentEvent that does not exist");
      }
      else
      {
        LiveLocationReportingSessionSegment sessionSegmentEvent1 = this.reportingSessionSegmentEvent;
        long num1 = currentTime;
        long? segmentT = this.reportingSessionSegmentEvent.segmentT;
        long? nullable = segmentT.HasValue ? new long?(num1 - segmentT.GetValueOrDefault()) : new long?();
        sessionSegmentEvent1.segmentT = nullable;
        LiveLocationReportingSessionSegment sessionSegmentEvent2 = this.reportingSessionSegmentEvent;
        double? batteryLevelChange1 = sessionSegmentEvent2.segmentBatteryLevelChange;
        double batteryPercentage = (double) AppState.BatteryPercentage;
        sessionSegmentEvent2.segmentBatteryLevelChange = batteryLevelChange1.HasValue ? new double?(batteryLevelChange1.GetValueOrDefault() - batteryPercentage) : new double?();
        segmentT = this.reportingSessionSegmentEvent.segmentT;
        long num2 = 0;
        if ((segmentT.GetValueOrDefault() > num2 ? (segmentT.HasValue ? 1 : 0) : 0) != 0)
        {
          double? batteryLevelChange2 = this.reportingSessionSegmentEvent.segmentBatteryLevelChange;
          double num3 = 0.0;
          if ((batteryLevelChange2.GetValueOrDefault() > num3 ? (batteryLevelChange2.HasValue ? 1 : 0) : 0) != 0)
          {
            this.reportingSessionSegmentEvent.SaveEvent();
            if (!this.reportingSessionSegmentEvent.segmentBatteryCharging.Value)
            {
              this.sharingTime += this.reportingSessionSegmentEvent.segmentT.Value;
              goto label_7;
            }
            else
              goto label_7;
          }
        }
        Log.l("lls", "dropping reportingSessionSegmentEvent");
label_7:
        this.reportingSessionSegmentEvent = (LiveLocationReportingSessionSegment) null;
      }
    }

    private void Save()
    {
      BinaryData binaryData = new BinaryData();
      binaryData.AppendByte((byte) 1);
      binaryData.AppendLong64(this.sharingTime);
      binaryData.AppendLong64(this.sharingStartLocalDateTime.ToBinary());
      binaryData.AppendLong64(this.segmentNumber);
      if (this.reportingSessionEvent == null)
      {
        binaryData.AppendByte((byte) 0);
      }
      else
      {
        binaryData.AppendByte((byte) 1);
        binaryData.AppendLong64(this.reportingSessionEvent.sessionT.Value);
        binaryData.AppendLong64(this.reportingSessionEvent.numberOfUpdates.Value);
        binaryData.AppendLong64((long) this.reportingSessionEvent.batteryLevelChange.Value);
      }
      if (this.reportingSessionSegmentEvent == null)
      {
        binaryData.AppendByte((byte) 0);
      }
      else
      {
        binaryData.AppendByte((byte) 1);
        binaryData.AppendLong64(this.reportingSessionSegmentEvent.segmentT.Value);
        binaryData.AppendLong64(this.reportingSessionSegmentEvent.segmentNumber.Value);
        binaryData.AppendLong64(this.reportingSessionSegmentEvent.segmentNumberOfUpdates.Value);
        binaryData.AppendLong64((long) this.reportingSessionSegmentEvent.segmentBatteryLevelChange.Value);
        binaryData.AppendByte(this.reportingSessionSegmentEvent.segmentBatteryCharging.Value ? (byte) 1 : (byte) 0);
        binaryData.AppendInt32((int) this.reportingSessionSegmentEvent.segmentBackoffStage.Value);
      }
      Settings.LiveLocationSession = binaryData.Get();
    }
  }
}
