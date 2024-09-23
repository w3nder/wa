// Decompiled with JetBrains decompiler
// Type: WhatsApp.TimeSpentManager
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;


namespace WhatsApp
{
  public class TimeSpentManager
  {
    public static string LogHeader = "T_S";
    private static object initLock = new object();
    private static TimeSpentManager _instance = (TimeSpentManager) null;
    private List<Utils.TimeSpentFieldStatsRecordOption> recordOptions;
    private List<TimeSpentSessionData> currentData;
    private object processLock = new object();
    private bool started;
    private bool listening;
    private bool userActionClbThrottleFlag;
    private long lastUserActionEventSecs = -1;
    private static bool throttleNextCheckTime = false;
    private static bool throttleCheckExceptions = false;

    public static TimeSpentManager GetInstance()
    {
      if (TimeSpentManager._instance == null)
      {
        lock (TimeSpentManager.initLock)
        {
          if (TimeSpentManager._instance == null)
            TimeSpentManager._instance = new TimeSpentManager();
        }
      }
      return TimeSpentManager._instance;
    }

    private TimeSpentManager()
    {
      int spentRecordOption = NonDbSettings.TimeSpentRecordOption;
      this.recordOptions = new List<Utils.TimeSpentFieldStatsRecordOption>();
      switch (spentRecordOption)
      {
        case 3:
          this.recordOptions.Add(Utils.TimeSpentFieldStatsRecordOption.BitArray);
          this.recordOptions.Add(Utils.TimeSpentFieldStatsRecordOption.SessionEvent);
          break;
        case 5:
          this.recordOptions.Add(Utils.TimeSpentFieldStatsRecordOption.BitArray);
          this.recordOptions.Add(Utils.TimeSpentFieldStatsRecordOption.SessionSummary);
          break;
        case 6:
          this.recordOptions.Add(Utils.TimeSpentFieldStatsRecordOption.SessionEvent);
          this.recordOptions.Add(Utils.TimeSpentFieldStatsRecordOption.SessionSummary);
          break;
        case 7:
          this.recordOptions.Add(Utils.TimeSpentFieldStatsRecordOption.SessionEvent);
          this.recordOptions.Add(Utils.TimeSpentFieldStatsRecordOption.BitArray);
          this.recordOptions.Add(Utils.TimeSpentFieldStatsRecordOption.SessionSummary);
          break;
        default:
          if ((spentRecordOption <= 0 || spentRecordOption >= 3) && spentRecordOption != 4)
            break;
          this.recordOptions.Add((Utils.TimeSpentFieldStatsRecordOption) spentRecordOption);
          break;
      }
    }

    public Action AppForegrounded()
    {
      try
      {
        lock (this.processLock)
        {
          Log.d(TimeSpentManager.LogHeader, "TimeSpent foreground: started {0}, listening {1}, recording {2}", (object) this.started, (object) this.listening, this.recordOptions == null ? (object) "null" : (object) this.recordOptions.Count.ToString());
          if (this.started)
            return (Action) null;
          this.started = true;
          if (this.recordOptions == null || this.recordOptions.Count == 0)
            return (Action) (() =>
            {
              Log.d(TimeSpentManager.LogHeader, "TimeSpent collection not started");
              this.started = false;
            });
          Action a = (Action) (() => this.AppBackgrounded());
          this.RecordEventNow(TimeSpentManager.AppEvent.FOREGROUNDED);
          if (this.listening)
            return a;
          this.listening = true;
          Touch.FrameReported += new TouchFrameEventHandler(this.Touch_FrameReported);
          AppState.PerformWhenLeavingFg(a);
          return a;
        }
      }
      catch (Exception ex)
      {
        string context = TimeSpentManager.LogHeader + " AppForegrounded";
        Log.SendCrashLog(ex, context, logOnlyForRelease: true);
        return (Action) null;
      }
    }

    public void AppBackgrounded()
    {
      try
      {
        lock (this.processLock)
        {
          Log.d(TimeSpentManager.LogHeader, "TimeSpent background: started {0}, listening {1}", (object) this.started, (object) this.listening);
          if (!this.started)
            return;
          this.started = false;
          this.RecordEventNow(TimeSpentManager.AppEvent.BACKGROUNDED);
          if (!this.listening)
            return;
          this.listening = false;
          Touch.FrameReported -= new TouchFrameEventHandler(this.Touch_FrameReported);
        }
      }
      catch (Exception ex)
      {
        string context = TimeSpentManager.LogHeader + " AppBackgrounded";
        Log.SendCrashLog(ex, context, logOnlyForRelease: true);
      }
    }

    private void Touch_FrameReported(object sender, TouchFrameEventArgs e) => this.UserAction();

    public void UserAction()
    {
      if (this.recordOptions == null)
        return;
      if (this.recordOptions.Count == 0)
        return;
      try
      {
        this.RecordEventNow(TimeSpentManager.AppEvent.USER_ACTION);
      }
      catch (Exception ex)
      {
        if (!this.userActionClbThrottleFlag)
        {
          this.userActionClbThrottleFlag = true;
          Log.SendCrashLog(ex, TimeSpentManager.LogHeader + " UserAction", logOnlyForRelease: true);
        }
        else
          Log.l(TimeSpentManager.LogHeader, "UserAction {0}", (object) ex.ToString());
      }
    }

    private void RecordEventNow(TimeSpentManager.AppEvent eventType)
    {
      long unixTime = DateTime.UtcNow.ToUnixTime();
      switch (eventType)
      {
        case TimeSpentManager.AppEvent.FOREGROUNDED:
          if (unixTime == this.lastUserActionEventSecs)
            goto case TimeSpentManager.AppEvent.USER_ACTION;
          else
            goto default;
        case TimeSpentManager.AppEvent.USER_ACTION:
          if (eventType != TimeSpentManager.AppEvent.USER_ACTION && eventType != TimeSpentManager.AppEvent.FOREGROUNDED || unixTime == this.lastUserActionEventSecs)
            break;
          lock (this.processLock)
          {
            if (unixTime == this.lastUserActionEventSecs)
              break;
            this.lastUserActionEventSecs = unixTime;
            if (this.currentData == null)
            {
              this.currentData = new List<TimeSpentSessionData>();
              foreach (Utils.TimeSpentFieldStatsRecordOption recordOption in this.recordOptions)
              {
                Log.d(TimeSpentManager.LogHeader, "Starting collection: {0}, count {1}, now {2}", (object) eventType, (object) recordOption, (object) unixTime);
                this.currentData.Add(new TimeSpentSessionData(unixTime, recordOption));
              }
              try
              {
                long timeSpentCheckTime = NonDbSettings.TimeSpentCheckTime;
                if (!TimeSpentManager.throttleNextCheckTime && timeSpentCheckTime != 0L)
                {
                  if (unixTime % 20L == 0L)
                    Log.SendCrashLog((Exception) new InvalidDataException("Missing time spent event detected"), "Missing time spent event", logOnlyForRelease: true);
                  else
                    Log.l(TimeSpentManager.LogHeader, "Missing time spent event detected");
                  TimeSpentManager.throttleNextCheckTime = true;
                }
                NonDbSettings.TimeSpentCheckTime = unixTime;
                break;
              }
              catch (Exception ex)
              {
                if (TimeSpentManager.throttleCheckExceptions)
                {
                  Log.l(TimeSpentManager.LogHeader, "Exception checking for missing events: {0}", (object) ex.GetFriendlyMessage());
                  break;
                }
                TimeSpentManager.throttleCheckExceptions = true;
                Log.SendCrashLog(ex, "Exception checking for missing events", logOnlyForRelease: true);
                break;
              }
            }
            else
            {
              using (List<TimeSpentSessionData>.Enumerator enumerator = this.currentData.GetEnumerator())
              {
                while (enumerator.MoveNext())
                  enumerator.Current.SetBit(unixTime);
                break;
              }
            }
          }
        default:
          List<TimeSpentSessionData> spentSessionDataList = new List<TimeSpentSessionData>();
          lock (this.processLock)
          {
            if (this.currentData != null)
            {
              Log.d(TimeSpentManager.LogHeader, "Maybe terminating collection: {0}", (object) eventType);
              foreach (TimeSpentSessionData spentSessionData in this.currentData)
              {
                if (spentSessionData.SetBackgroundTime(unixTime))
                {
                  this.lastUserActionEventSecs = -1L;
                  spentSessionDataList.Add(spentSessionData);
                }
              }
              this.currentData = (List<TimeSpentSessionData>) null;
            }
          }
          if (spentSessionDataList.Count > 0)
          {
            foreach (TimeSpentSessionData spentSessionData in spentSessionDataList)
              spentSessionData.CreateAndSendEvent();
            try
            {
              NonDbSettings.TimeSpentCheckTime = 0L;
              goto case TimeSpentManager.AppEvent.USER_ACTION;
            }
            catch (Exception ex)
            {
              if (TimeSpentManager.throttleCheckExceptions)
              {
                Log.l(TimeSpentManager.LogHeader, "Exception setting missing event check to 0: {0}", (object) ex.GetFriendlyMessage());
                goto case TimeSpentManager.AppEvent.USER_ACTION;
              }
              else
              {
                TimeSpentManager.throttleCheckExceptions = true;
                Log.SendCrashLog(ex, "Exception setting missing event check to 0", logOnlyForRelease: true);
                goto case TimeSpentManager.AppEvent.USER_ACTION;
              }
            }
          }
          else
            goto case TimeSpentManager.AppEvent.USER_ACTION;
      }
    }

    public enum AppEvent
    {
      FOREGROUNDED = 1,
      USER_ACTION = 2,
      BACKGROUNDED = 3,
      CLOCK_CHANGE = 4,
    }
  }
}
