// Decompiled with JetBrains decompiler
// Type: WhatsApp.TimeSpentSessionData
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WhatsApp.Events;
using WhatsApp.WaCollections;


namespace WhatsApp
{
  public class TimeSpentSessionData
  {
    private const int max_ints_in_bit_array_event = 20;
    private const long max_secs_in_bit_array = 640;
    private const long max_session_length_secs = 30;
    private const long max_secs_in_session_end = 180;
    private const int max_seconds_diff_not_considered_time_change = 3600;
    private const long max_secs_in_session_summary = 300;
    private const long max_period_in_session_summary = 3600;
    private long startTimeSec;
    private Utils.TimeSpentFieldStatsRecordOption recordOption;
    private long endTimeSec;
    private List<int> bitArray;
    private long lastActivityTimeSec;
    private long timeChangesDetected;
    private long foregroundCount;
    private long sessionSummaryTimeAccumulated;
    private long sessionSummaryStartTimeSec;
    private long sessionSummaryLastPersistTime;
    private long sessionStopTimeSec;
    private long eventsCounted;
    private const byte SESS_SUMM_FORMAT = 1;

    public TimeSpentSessionData(long startTime, Utils.TimeSpentFieldStatsRecordOption recordOption)
    {
      this.startTimeSec = startTime;
      this.recordOption = recordOption;
      this.bitArray = new List<int>();
      this.lastActivityTimeSec = startTime;
      this.eventsCounted = 1L;
      this.timeChangesDetected = 0L;
      this.foregroundCount = 1L;
      this.endTimeSec = -1L;
      switch (recordOption)
      {
        case Utils.TimeSpentFieldStatsRecordOption.BitArray:
          this.bitArray[this.EnsureRequiredEntriesinBitArray(0L)] |= 1;
          break;
        case Utils.TimeSpentFieldStatsRecordOption.SessionEvent:
          break;
        case Utils.TimeSpentFieldStatsRecordOption.SessionSummary:
          if (!this.RetrieveSessionSummary(startTime))
          {
            this.sessionSummaryTimeAccumulated = 1L;
            this.sessionSummaryStartTimeSec = this.startTimeSec;
          }
          else
            ++this.foregroundCount;
          this.sessionSummaryLastPersistTime = this.startTimeSec;
          this.sessionStopTimeSec = this.NextStopTimeSec(this.sessionSummaryStartTimeSec);
          break;
        default:
          throw new InvalidDataException("created TimeSpentSessionData with invalid recording option: " + (object) recordOption);
      }
    }

    public void SetBit(long activityTimeSec)
    {
      if (this.lastActivityTimeSec == activityTimeSec)
        return;
      if (this.endTimeSec != -1L)
        throw new InvalidDataException("Adding activity to completed Data");
      lock (this)
      {
        if (activityTimeSec < this.lastActivityTimeSec)
        {
          Log.l(TimeSpentManager.LogHeader, "Possible time rewind! act:{0} last:{1} start:{2} now:{3}", (object) activityTimeSec, (object) this.lastActivityTimeSec, (object) this.startTimeSec, (object) DateTime.UtcNow.ToUnixTime());
          Log.SendCrashLog((Exception) new InvalidDataException("Time rewind detected"), TimeSpentManager.LogHeader = "Time rewind detected", logOnlyForRelease: true);
        }
        ++this.eventsCounted;
        if (activityTimeSec < this.lastActivityTimeSec || activityTimeSec > this.lastActivityTimeSec + 3600L)
          ++this.timeChangesDetected;
        if (this.recordOption == Utils.TimeSpentFieldStatsRecordOption.SessionSummary)
        {
          if (activityTimeSec < this.startTimeSec)
          {
            this.sessionSummaryTimeAccumulated += (long) this.CalculateSessionLength();
            this.lastActivityTimeSec = activityTimeSec;
          }
          else if (activityTimeSec - this.lastActivityTimeSec > 30L)
          {
            long summaryTimeAccumulated = this.sessionSummaryTimeAccumulated;
            this.sessionSummaryTimeAccumulated += (long) this.CalculateSessionLength();
            this.lastActivityTimeSec = activityTimeSec;
            this.PersistSessionSummary(this.lastActivityTimeSec);
          }
          else
          {
            this.lastActivityTimeSec = activityTimeSec;
            long summaryTimeAccumulated = this.sessionSummaryTimeAccumulated;
            this.sessionSummaryTimeAccumulated += (long) (this.CalculateSessionLength() - 1);
          }
          this.startTimeSec = activityTimeSec;
          if (activityTimeSec >= this.sessionStopTimeSec)
          {
            this.endTimeSec = activityTimeSec - 1L;
            this.SendSessionSummaryEventToFieldStats();
            this.clearSessionSummaryCollection(activityTimeSec);
          }
          else
          {
            if (activityTimeSec < this.sessionSummaryLastPersistTime + 180L)
              return;
            this.PersistSessionSummary(activityTimeSec);
            this.sessionSummaryLastPersistTime += 180L;
          }
        }
        else if (this.recordOption == Utils.TimeSpentFieldStatsRecordOption.SessionEvent)
        {
          if (activityTimeSec - this.lastActivityTimeSec > 30L || activityTimeSec < this.startTimeSec)
          {
            this.SendSessionEndEventToFieldStats();
            this.startTimeSec = activityTimeSec;
          }
          else if (activityTimeSec >= this.startTimeSec + 180L)
          {
            this.lastActivityTimeSec = this.startTimeSec + 180L - 1L;
            this.SendSessionEndEventToFieldStats();
            this.startTimeSec += 180L;
          }
          this.lastActivityTimeSec = activityTimeSec;
        }
        else
        {
          long secsOffset = activityTimeSec - this.startTimeSec;
          if (secsOffset < 0L)
          {
            this.SendBitArrayEventToFieldStats();
            this.bitArray.Clear();
            this.startTimeSec = activityTimeSec;
            secsOffset = activityTimeSec - this.startTimeSec;
            this.lastActivityTimeSec = activityTimeSec;
          }
          else
          {
            this.lastActivityTimeSec = activityTimeSec;
            if (secsOffset > 639L)
            {
              this.SendBitArrayEventToFieldStats();
              this.bitArray.Clear();
              this.startTimeSec = activityTimeSec;
              secsOffset = activityTimeSec - this.startTimeSec;
            }
          }
          this.bitArray[this.EnsureRequiredEntriesinBitArray(secsOffset)] |= 1 << (int) (secsOffset & 31L);
        }
      }
    }

    private void clearSessionSummaryCollection(long newStartTime)
    {
      this.sessionSummaryStartTimeSec = newStartTime;
      this.sessionStopTimeSec = this.NextStopTimeSec(this.sessionSummaryStartTimeSec);
      this.timeChangesDetected = 0L;
      this.sessionSummaryTimeAccumulated = 0L;
      this.foregroundCount = 0L;
      this.endTimeSec = -1L;
    }

    public bool SetBackgroundTime(long bgTimeSecs)
    {
      if (bgTimeSecs < this.lastActivityTimeSec)
      {
        Log.l(TimeSpentManager.LogHeader, "Possible time rewind! end:{0} last:{1} start:{2} now:{3}", (object) bgTimeSecs, (object) this.lastActivityTimeSec, (object) this.startTimeSec, (object) DateTime.UtcNow.ToUnixTime());
        Log.SendCrashLog((Exception) new InvalidDataException("Time rewind detected"), TimeSpentManager.LogHeader = "Time rewind detected", logOnlyForRelease: true);
        bgTimeSecs = this.lastActivityTimeSec + 1L;
      }
      this.endTimeSec = bgTimeSecs;
      if (this.recordOption != Utils.TimeSpentFieldStatsRecordOption.SessionSummary)
        return true;
      this.PersistSessionSummary(this.endTimeSec);
      return false;
    }

    public void CreateAndSendEvent()
    {
      lock (this)
      {
        if (this.recordOption == Utils.TimeSpentFieldStatsRecordOption.SessionSummary)
          this.SendSessionSummaryEventToFieldStats();
        else if (this.recordOption == Utils.TimeSpentFieldStatsRecordOption.SessionEvent)
          this.SendSessionEndEventToFieldStats();
        else
          this.SendBitArrayEventToFieldStats();
      }
    }

    private void SendSessionSummaryEventToFieldStats()
    {
      this.LogCollectedDataBrief();
      UserActivitySessionSummary activitySessionSummary = new UserActivitySessionSummary();
      activitySessionSummary.userActivityStartTime = new long?(this.sessionSummaryStartTimeSec);
      activitySessionSummary.userActivityDuration = new long?(this.endTimeSec - this.sessionSummaryStartTimeSec + 1L);
      activitySessionSummary.userActivitySessionsLength = new long?(this.sessionSummaryTimeAccumulated);
      activitySessionSummary.userActivityTimeChange = new long?(this.timeChangesDetected);
      activitySessionSummary.userActivityForeground = new long?(this.foregroundCount);
      activitySessionSummary.userSessionSummarySequence = new long?(NonDbSettings.IncrementSessionSummaryCount);
      activitySessionSummary.SaveEvent();
      this.ClearPersistedSessionSummary();
      Log.d(TimeSpentManager.LogHeader, "Recording summary session event: {0} {1} {2} {3}", (object) activitySessionSummary.userActivityStartTime, (object) activitySessionSummary.userActivitySessionsLength, (object) activitySessionSummary.userActivityForeground, (object) activitySessionSummary.userActivityTimeChange);
    }

    private void SendSessionEndEventToFieldStats()
    {
      this.LogCollectedDataBrief();
      if (this.lastActivityTimeSec < this.startTimeSec)
      {
        Log.l(TimeSpentManager.LogHeader, "Ignoring session event: {0} {1} {2}", (object) this.startTimeSec, (object) this.lastActivityTimeSec, (object) this.endTimeSec);
        Log.SendCrashLog((Exception) new InvalidDataException("Creating session end with -ve time"), TimeSpentManager.LogHeader = "Creating session end with -ve time", logOnlyForRelease: true);
      }
      else
      {
        UserActivitySessionEnd activitySessionEnd = new UserActivitySessionEnd();
        activitySessionEnd.userActivityStartTime = new long?((long) (int) this.startTimeSec);
        activitySessionEnd.userActivitySessionLength = new long?((long) this.CalculateSessionLength());
        activitySessionEnd.SaveEvent();
        Log.d(TimeSpentManager.LogHeader, "Recording session event: {0} {1}", (object) activitySessionEnd.userActivityStartTime, (object) activitySessionEnd.userActivitySessionLength);
      }
    }

    private int CalculateSessionLength()
    {
      return (int) (this.lastActivityTimeSec - this.startTimeSec) + 1;
    }

    private void SendBitArrayEventToFieldStats()
    {
      this.LogCollectedDataBrief();
      UserActivityBitArray activityBitArray1 = new UserActivityBitArray();
      activityBitArray1.userActivityStartTime = new long?((long) (int) this.startTimeSec);
      int num1 = Math.Min(20, this.bitArray.Count);
      for (int index = 0; index < num1; ++index)
      {
        if (this.bitArray[index] != 0)
        {
          long bit = (long) this.bitArray[index];
          switch (index)
          {
            case 0:
              activityBitArray1.userActivityBitmap0 = new long?(bit);
              continue;
            case 1:
              activityBitArray1.userActivityBitmap1 = new long?(bit);
              continue;
            case 2:
              activityBitArray1.userActivityBitmap2 = new long?(bit);
              continue;
            case 3:
              activityBitArray1.userActivityBitmap3 = new long?(bit);
              continue;
            case 4:
              activityBitArray1.userActivityBitmap4 = new long?(bit);
              continue;
            case 5:
              activityBitArray1.userActivityBitmap5 = new long?(bit);
              continue;
            case 6:
              activityBitArray1.userActivityBitmap6 = new long?(bit);
              continue;
            case 7:
              activityBitArray1.userActivityBitmap7 = new long?(bit);
              continue;
            case 8:
              activityBitArray1.userActivityBitmap8 = new long?(bit);
              continue;
            case 9:
              activityBitArray1.userActivityBitmap9 = new long?(bit);
              continue;
            case 10:
              activityBitArray1.userActivityBitmap10 = new long?(bit);
              continue;
            case 11:
              activityBitArray1.userActivityBitmap11 = new long?(bit);
              continue;
            case 12:
              activityBitArray1.userActivityBitmap12 = new long?(bit);
              continue;
            case 13:
              activityBitArray1.userActivityBitmap13 = new long?(bit);
              continue;
            case 14:
              activityBitArray1.userActivityBitmap14 = new long?(bit);
              continue;
            case 15:
              activityBitArray1.userActivityBitmap15 = new long?(bit);
              continue;
            case 16:
              activityBitArray1.userActivityBitmap16 = new long?(bit);
              continue;
            case 17:
              activityBitArray1.userActivityBitmap17 = new long?(bit);
              continue;
            case 18:
              activityBitArray1.userActivityBitmap18 = new long?(bit);
              continue;
            case 19:
              activityBitArray1.userActivityBitmap19 = new long?(bit);
              continue;
            default:
              continue;
          }
        }
      }
      long num2 = this.endTimeSec > 0L ? this.endTimeSec : this.lastActivityTimeSec;
      activityBitArray1.userActivityBitmapLen = new long?((long) (int) Math.Min(num2 - this.startTimeSec + 1L, 640L));
      if (num2 == this.endTimeSec && num2 > this.lastActivityTimeSec)
      {
        long? activityBitmapLen = activityBitArray1.userActivityBitmapLen;
        long num3 = 1;
        if ((activityBitmapLen.GetValueOrDefault() > num3 ? (activityBitmapLen.HasValue ? 1 : 0) : 0) != 0)
        {
          activityBitmapLen = activityBitArray1.userActivityBitmapLen;
          long num4 = 640;
          if ((activityBitmapLen.GetValueOrDefault() < num4 ? (activityBitmapLen.HasValue ? 1 : 0) : 0) != 0)
          {
            UserActivityBitArray activityBitArray2 = activityBitArray1;
            activityBitmapLen = activityBitArray2.userActivityBitmapLen;
            long num5 = 1;
            activityBitArray2.userActivityBitmapLen = activityBitmapLen.HasValue ? new long?(activityBitmapLen.GetValueOrDefault() - num5) : new long?();
          }
        }
      }
      activityBitArray1.SaveEvent();
      Log.d(TimeSpentManager.LogHeader, "Recording bit array event: {0} {1}", (object) activityBitArray1.userActivityStartTime, (object) activityBitArray1.userActivityBitmapLen);
    }

    private int EnsureRequiredEntriesinBitArray(long secsOffset)
    {
      int num = (int) secsOffset >> 5;
      lock (this)
      {
        for (int index = this.bitArray.Count - 1; index < num; ++index)
          this.bitArray.Add(0);
      }
      return num;
    }

    private void LogCollectedDataBrief()
    {
      long num1 = this.startTimeSec;
      string str1;
      if (this.recordOption == Utils.TimeSpentFieldStatsRecordOption.SessionSummary)
      {
        str1 = this.sessionSummaryTimeAccumulated.ToString();
        num1 = this.sessionSummaryStartTimeSec;
      }
      else if (this.recordOption == Utils.TimeSpentFieldStatsRecordOption.BitArray)
      {
        int bit = this.bitArray[0];
        StringBuilder stringBuilder = new StringBuilder(64);
        for (int index = 0; index < 32; ++index)
        {
          long num2 = (long) bit & 1L;
          bit >>= 1;
          string str2 = num2 == 1L ? "1" : "0";
          stringBuilder.Append(str2);
        }
        str1 = stringBuilder.ToString();
      }
      else
        str1 = (this.lastActivityTimeSec - this.startTimeSec).ToString();
      long num3 = this.endTimeSec - num1;
      if (num3 < 0L)
        num3 = -1L;
      Log.d(TimeSpentManager.LogHeader, "Start {0}, duration {1}, last {2}, events {3}, usage {4}", (object) num1, (object) num3, (object) this.lastActivityTimeSec, (object) this.eventsCounted, (object) str1);
    }

    private void PersistSessionSummary(long persistTime)
    {
      byte[] numArray;
      try
      {
        BinaryData binaryData = new BinaryData();
        binaryData.AppendByte((byte) 1);
        binaryData.AppendLong64(this.sessionSummaryStartTimeSec);
        binaryData.AppendLong64(this.sessionSummaryTimeAccumulated);
        binaryData.AppendLong64(this.timeChangesDetected);
        binaryData.AppendLong64(this.foregroundCount);
        binaryData.AppendLong64(persistTime);
        numArray = binaryData.Get();
        Log.d(TimeSpentManager.LogHeader, "saving summary data {0} {1} {2} {3}", (object) this.sessionSummaryStartTimeSec, (object) this.sessionSummaryTimeAccumulated, (object) this.foregroundCount, (object) this.timeChangesDetected);
      }
      catch (Exception ex)
      {
        string context = TimeSpentManager.LogHeader + " Exception serializing session summary details";
        Log.LogException(ex, context);
        numArray = (byte[]) null;
      }
      NonDbSettings.TimeSpentSummaryData = numArray;
    }

    private bool RetrieveSessionSummary(long startTime)
    {
      bool flag = false;
      try
      {
        byte[] spentSummaryData = NonDbSettings.TimeSpentSummaryData;
        if (spentSummaryData != null)
        {
          int offset1 = 0;
          BinaryData binaryData = new BinaryData(spentSummaryData);
          byte num1 = binaryData.ReadByte(offset1);
          if (num1 != (byte) 1)
          {
            Log.l(TimeSpentManager.LogHeader, "Deserialize expected {0} found {1} at offset {2}", (object) (byte) 1, (object) num1, (object) offset1);
            flag = false;
          }
          else
          {
            int offset2 = offset1 + 1;
            long num2 = binaryData.ReadLong64(offset2);
            long unixTime = DateTime.UtcNow.ToUnixTime();
            if (num2 > unixTime || (double) num2 < (double) unixTime - TimeSpan.FromHours(24.0).TotalSeconds)
            {
              Log.l(TimeSpentManager.LogHeader, "Deserialize time looks wrong {0}, now {1}", (object) num2, (object) unixTime);
              flag = false;
            }
            else
            {
              this.sessionSummaryStartTimeSec = num2;
              int offset3 = offset2 + 8;
              this.sessionSummaryTimeAccumulated = binaryData.ReadLong64(offset3);
              int offset4 = offset3 + 8;
              this.timeChangesDetected = binaryData.ReadLong64(offset4);
              int offset5 = offset4 + 8;
              this.foregroundCount = binaryData.ReadLong64(offset5);
              int offset6 = offset5 + 8;
              if (binaryData.Length() >= offset6 + 8 && binaryData.ReadLong64(offset6) < startTime)
                ++this.sessionSummaryTimeAccumulated;
              Log.d(TimeSpentManager.LogHeader, "retrieved summary data {0} {1} {2} {3}", (object) this.sessionSummaryStartTimeSec, (object) this.sessionSummaryTimeAccumulated, (object) this.foregroundCount, (object) this.timeChangesDetected);
              this.sessionStopTimeSec = this.NextStopTimeSec(this.sessionSummaryStartTimeSec);
              if (startTime >= this.sessionStopTimeSec)
              {
                this.endTimeSec = startTime - 1L;
                this.SendSessionSummaryEventToFieldStats();
                this.clearSessionSummaryCollection(startTime);
              }
              flag = true;
            }
          }
        }
      }
      catch (Exception ex)
      {
        string context = TimeSpentManager.LogHeader + " Exception deserializing session summary details";
        Log.LogException(ex, context);
        flag = false;
      }
      return flag;
    }

    private bool ClearPersistedSessionSummary()
    {
      bool flag = false;
      try
      {
        NonDbSettings.TimeSpentSummaryData = (byte[]) null;
        Log.d(TimeSpentManager.LogHeader, "clearing summary data");
      }
      catch (Exception ex)
      {
        string context = TimeSpentManager.LogHeader + " Exception deserializing session summary details";
        Log.LogException(ex, context);
        flag = false;
      }
      return flag;
    }

    private long NextStopTimeSec(long sessionStartUtcSecs)
    {
      DateTime dateTime = FunXMPP.UnixEpoch.AddSeconds((double) sessionStartUtcSecs);
      long unixTime1 = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, DateTimeKind.Utc).AddHours(24.0).ToUnixTime();
      DateTime localTime = dateTime.ToLocalTime();
      long unixTime2 = new DateTime(localTime.Year, localTime.Month, localTime.Day, 0, 0, 0, DateTimeKind.Local).AddHours(24.0).ToUnixTime();
      long num = sessionStartUtcSecs + 3600L;
      Log.d(TimeSpentManager.LogHeader, "seconds left today: Utc={0}, Local={1}. Max Dur={2}. Start Utc {3}:{4}:{5}", (object) (unixTime1 - sessionStartUtcSecs), (object) (unixTime2 - sessionStartUtcSecs), (object) (num - sessionStartUtcSecs), (object) dateTime.Hour, (object) dateTime.Minute, (object) dateTime.Second);
      if (num < unixTime2 && num < unixTime1)
        return num;
      return unixTime2 > sessionStartUtcSecs && unixTime2 < unixTime1 ? unixTime2 : unixTime1;
    }
  }
}
