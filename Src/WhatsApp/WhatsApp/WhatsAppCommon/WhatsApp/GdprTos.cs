// Decompiled with JetBrains decompiler
// Type: WhatsApp.GdprTos
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;


namespace WhatsApp
{
  public static class GdprTos
  {
    public const string LogHeader = "tos2";
    public const string ResetServerProp = "0";
    public const int NoneStage = 0;
    public static readonly Color LinkColor = Color.FromArgb(byte.MaxValue, (byte) 71, (byte) 165, (byte) 254);

    public static void ProcessServerProp(string val)
    {
      Log.l("tos2", "process incoming server prop:[{0}]", (object) (val ?? ""));
      val = val?.Trim();
      if (string.IsNullOrEmpty(val))
        return;
      if (val == "0")
      {
        Log.l("tos2", "reset request from server");
        GdprTos.LocalReset("reset request from server", false);
        Settings.GdprTosServerProperty = "0";
        GdprTos.ScheduleAckGdprTosReset();
      }
      else if (Settings.GdprTosAcceptedUtc.HasValue)
        GdprTos.Accept("process incoming server prop");
      else if (val == Settings.GdprTosServerProperty)
      {
        Log.l("tos2", "Server prop has not changed");
      }
      else
      {
        GdprTos.LocalReset("new server prop", true);
        Settings.GdprTosServerProperty = val;
      }
    }

    public static void LocalReset(string context, bool preserveStartTimes)
    {
      Log.l("tos2", "reset local state | {0}", (object) context);
      Settings.GdprTosServerProperty = (string) null;
      Settings.GdprTosCurrentStage = 0;
      Settings.GdprTosLastShownUtc = new DateTime?();
      Settings.GdprTosUserDismissedUtc = new DateTime?();
      Settings.GdprTosAcceptedUtc = new DateTime?();
      Settings.GdprTosFirstSeenSecondPageUtc = new DateTime?();
      if (!preserveStartTimes)
        return;
      Settings.GdprTosStage1StartUtc = new DateTime?();
      Settings.GdprTosStage2StartUtc = new DateTime?();
      Settings.GdprTosStage3StartUtc = new DateTime?();
    }

    public static void Accept(string context)
    {
      Log.l("tos2", "accept | {0}", (object) context);
      PersistentAction pa = (PersistentAction) null;
      bool flag = false;
      if (Settings.GdprTosAcceptedUtc.HasValue)
      {
        Log.l("tos2", "accept | pending from {0} (utc)", (object) Settings.GdprTosAcceptedUtc.Value);
        MessagesContext.Run((MessagesContext.MessagesCallback) (db => pa = ((IEnumerable<PersistentAction>) db.GetPersistentActions(PersistentAction.Types.SendGdprAccept)).FirstOrDefault<PersistentAction>()));
        if (pa == null)
        {
          Log.l("tos2", "accept | pending but no persist action found");
          pa = GdprTos.SendGdprAcceptAction();
        }
        else
          flag = true;
      }
      else
      {
        Settings.GdprTosAcceptedUtc = new DateTime?(FunRunner.CurrentServerTimeUtc);
        Log.l("tos2", "accept | {0} (utc)", (object) Settings.GdprTosAcceptedUtc.Value);
        pa = GdprTos.SendGdprAcceptAction();
      }
      if (flag)
        AppState.AttemptPersistentAction(pa);
      else
        AppState.SchedulePersistentAction(pa);
    }

    public static bool ShouldBlockNotifications()
    {
      return !Settings.GdprTosAcceptedUtc.HasValue && Settings.GdprTosCurrentStage > 2;
    }

    public static bool ShouldRejectCalls()
    {
      return !Settings.GdprTosAcceptedUtc.HasValue && Settings.GdprTosCurrentStage > 1;
    }

    public static bool ShouldShowOnAppEntry(bool enforce24hRule)
    {
      string str = Settings.GdprTosServerProperty?.Trim();
      bool flag;
      if (string.IsNullOrEmpty(Settings.PushName) || Settings.ShowPushNameScreen)
      {
        Log.d("tos2", "skip showing | no pushname set yet:[{0}]", (object) (str ?? ""));
        flag = false;
      }
      else if (string.IsNullOrEmpty(str) || str == "0")
      {
        flag = false;
        Log.d("tos2", "skip showing | no-op server prop:[{0}]", (object) (str ?? ""));
      }
      else
      {
        DateTime? nullable = Settings.GdprTosAcceptedUtc;
        if (nullable.HasValue)
        {
          flag = false;
          Log.d("tos2", "skip showing | accept pending");
          GdprTos.Accept("app entry");
        }
        else
        {
          Log.l("tos2", "determine showing gdpr tos | prop:[{0}]", (object) str);
          int targetStage = GdprTos.GetTargetStage();
          if (!GdprTos.IsValidStage(targetStage))
          {
            flag = false;
            Log.l("tos2", "skip showing | invalid target stage:{0}", (object) targetStage);
          }
          else if (targetStage == 1)
          {
            if (enforce24hRule)
            {
              nullable = Settings.GdprTosLastShownUtc;
              if (nullable.HasValue)
              {
                nullable = Settings.GdprTosUserDismissedUtc;
                if (nullable.HasValue)
                {
                  DateTime currentServerTimeUtc = FunRunner.CurrentServerTimeUtc;
                  nullable = Settings.GdprTosLastShownUtc;
                  DateTime dateTime = nullable.Value;
                  if ((currentServerTimeUtc - dateTime).TotalHours < 24.0)
                  {
                    Log.l("tos2", "skip showing | 24 hours rule");
                    flag = false;
                    goto label_16;
                  }
                }
              }
            }
            flag = true;
          }
          else
            flag = true;
label_16:
          Log.d("tos2", "{0} gdpr tos | stage:{0}", flag ? (object) "to show" : (object) "skip showing", (object) Settings.GdprTosCurrentStage);
        }
      }
      return flag;
    }

    public static bool IsValidStage(int stage) => stage == 1 || stage == 2 || stage == 3;

    public static int GetTargetStage()
    {
      int? nullable = new int?();
      int stage1Hours = 0;
      int stage2Hours = 0;
      string str1 = Settings.GdprTosServerProperty?.Trim();
      Log.d("tos2", "get target state | server prop:[{0}]", (object) (str1 ?? ""));
      if (string.IsNullOrEmpty(str1) || str1 == "0")
      {
        nullable = new int?(0);
      }
      else
      {
        if (!GdprTos.GetStageDurations(out stage1Hours, out stage2Hours))
        {
          InvalidOperationException e = new InvalidOperationException("couldn't parse stage durations");
          Log.SendCrashLog((Exception) e, string.Format("parse stage durations | prop:{0}", (object) str1));
          throw e;
        }
        DateTime currentServerTimeUtc = FunRunner.CurrentServerTimeUtc;
        DateTime? tosStage1StartUtc = Settings.GdprTosStage1StartUtc;
        DateTime? tosStage2StartUtc = Settings.GdprTosStage2StartUtc;
        object[] objArray = new object[3]
        {
          (object) str1,
          null,
          null
        };
        DateTime valueOrDefault;
        string str2;
        if (!tosStage1StartUtc.HasValue)
        {
          str2 = (string) null;
        }
        else
        {
          valueOrDefault = tosStage1StartUtc.GetValueOrDefault();
          str2 = valueOrDefault.ToString();
        }
        if (str2 == null)
          str2 = "n/a";
        objArray[1] = (object) str2;
        string str3;
        if (!tosStage2StartUtc.HasValue)
        {
          str3 = (string) null;
        }
        else
        {
          valueOrDefault = tosStage2StartUtc.GetValueOrDefault();
          str3 = valueOrDefault.ToString();
        }
        if (str3 == null)
          str3 = "n/a";
        objArray[2] = (object) str3;
        Log.l("tos2", "get target stage | prop:{0},s1 start:{1},s2 start:{2}", objArray);
        TimeSpan timeSpan;
        if (tosStage1StartUtc.HasValue)
        {
          timeSpan = currentServerTimeUtc - tosStage1StartUtc.Value;
          if (timeSpan.TotalHours > (double) stage1Hours)
          {
            Log.l("tos2", "get stage | stage 1 expired");
          }
          else
          {
            nullable = new int?(1);
            Log.l("tos2", "get stage | stage 1 in effect");
          }
        }
        else if (stage1Hours > 0)
        {
          nullable = new int?(1);
          Log.l("tos2", "get stage | start stage 1 for {0}h", (object) stage1Hours);
        }
        else
          Log.l("tos2", "get stage | skip stage 1");
        if (!nullable.HasValue)
        {
          if (tosStage2StartUtc.HasValue)
          {
            timeSpan = currentServerTimeUtc - tosStage2StartUtc.Value;
            if (timeSpan.TotalHours > (double) stage2Hours)
            {
              nullable = new int?(3);
              Log.l("tos2", "get stage | stage 2 expired");
            }
            else
            {
              nullable = new int?(2);
              Log.l("tos2", "get stage | stage 2 in effect");
            }
          }
          else if (stage2Hours > 0)
          {
            nullable = new int?(2);
            Log.l("tos2", "get stage | start stage 2 for {0}h", (object) stage2Hours);
          }
          else
          {
            nullable = new int?(3);
            Log.l("tos2", "get stage | skip stage 2");
          }
        }
      }
      if (!nullable.HasValue || !GdprTos.IsValidStage(nullable.Value) && nullable.Value != 0)
      {
        ArgumentException e = new ArgumentException("invalid target stage");
        Log.SendCrashLog((Exception) e, "get target stage");
        throw e;
      }
      Log.l("tos2", "get target stage | {0}", (object) (nullable?.ToString() ?? "n/a"));
      return nullable ?? 0;
    }

    public static void CheckStageUpgrade()
    {
      int gdprTosCurrentStage = Settings.GdprTosCurrentStage;
      int targetStage = GdprTos.GetTargetStage();
      object[] objArray = new object[6]
      {
        (object) (Settings.GdprTosServerProperty ?? ""),
        (object) gdprTosCurrentStage,
        (object) targetStage,
        null,
        null,
        null
      };
      DateTime? nullable = Settings.GdprTosStage1StartUtc;
      ref DateTime? local1 = ref nullable;
      objArray[3] = (object) ((local1.HasValue ? local1.GetValueOrDefault().ToString() : (string) null) ?? "n/a");
      nullable = Settings.GdprTosStage2StartUtc;
      ref DateTime? local2 = ref nullable;
      objArray[4] = (object) ((local2.HasValue ? local2.GetValueOrDefault().ToString() : (string) null) ?? "n/a");
      nullable = Settings.GdprTosStage3StartUtc;
      ref DateTime? local3 = ref nullable;
      objArray[5] = (object) ((local3.HasValue ? local3.GetValueOrDefault().ToString() : (string) null) ?? "n/a");
      Log.l("tos2", "check stage upgrade | prop:[{0}],stage:{1}->{2},s1 start:{3},s2 start:{4},s3 start:{5}", objArray);
      if (!GdprTos.IsValidStage(targetStage))
        Log.l("tos2", "check stage upgrade | invalid stage");
      else if (targetStage < gdprTosCurrentStage)
        Log.l("tos2", "check stage upgrade | do not allow going backwards in stages");
      else if (targetStage == gdprTosCurrentStage)
      {
        Log.l("tos2", "check stage upgrade | stay in same stage");
      }
      else
      {
        Settings.GdprTosCurrentStage = targetStage;
        DateTime currentServerTimeUtc = FunRunner.CurrentServerTimeUtc;
        switch (targetStage)
        {
          case 1:
            Settings.GdprTosStage1StartUtc = new DateTime?(currentServerTimeUtc);
            break;
          case 2:
            Settings.GdprTosStage2StartUtc = new DateTime?(currentServerTimeUtc);
            break;
          case 3:
            Settings.GdprTosStage3StartUtc = new DateTime?(currentServerTimeUtc);
            break;
        }
        GdprTos.ScheduleSendGdprStage(targetStage);
        Log.l("tos2", "stage upgraded | {0}->{1},t:{2}", (object) gdprTosCurrentStage, (object) targetStage, (object) currentServerTimeUtc);
      }
    }

    public static bool GetStageDurations(out int stage1Hours, out int stage2Hours)
    {
      stage1Hours = stage2Hours = 0;
      bool stageDurations = false;
      string tosServerProperty = Settings.GdprTosServerProperty;
      Log.d("tos2", "get state hours | server prop:[{0}]", (object) (tosServerProperty ?? ""));
      if (!string.IsNullOrEmpty(tosServerProperty) && !(tosServerProperty == "0"))
      {
        int length = tosServerProperty.IndexOf('-');
        if (length > 0)
        {
          string s1 = tosServerProperty.Substring(0, length);
          string s2 = tosServerProperty.Substring(length + 1);
          ref int local = ref stage1Hours;
          if (int.TryParse(s1, out local) && stage1Hours >= 0 && int.TryParse(s2, out stage2Hours) && stage2Hours >= 0)
            stageDurations = true;
        }
      }
      return stageDurations;
    }

    public static bool IsEEA(string countryCode = null)
    {
      if (string.IsNullOrEmpty(countryCode))
        countryCode = Settings.CountryCode;
      CountryInfoItem infoForCountryCode = CountryInfo.Instance.GetCountryInfoForCountryCode(countryCode);
      return infoForCountryCode != null && infoForCountryCode.IsTosRegionEu();
    }

    public static WaRichText.Chunk[] GetRichTextFormattings(
      string text,
      KeyValuePair<WaRichText.Formats, string>[] formattings)
    {
      WaRichText.Chunk[] richTextFormattings = (WaRichText.Chunk[]) null;
      if (formattings != null)
      {
        int length = formattings.Length;
        richTextFormattings = WaRichText.GetHtmlLinkChunks(text);
        if (richTextFormattings.Length != length)
        {
          ArgumentException e = new ArgumentException("invalid gdpr tos screen 1 body");
          Log.SendCrashLog((Exception) e, text);
          throw e;
        }
        for (int index = 0; index < length; ++index)
        {
          WaRichText.Chunk chunk = richTextFormattings[index];
          WaRichText.Formats key = formattings[index].Key;
          chunk.Format = key;
          chunk.AuxiliaryInfo = formattings[index].Value;
          if (key == WaRichText.Formats.Link)
          {
            chunk.ForegroundColor = new Color?(GdprTos.LinkColor);
            chunk.LinkUnderscore = false;
          }
        }
      }
      return richTextFormattings;
    }

    public static void ScheduleSendGdprStage(int stage)
    {
      AppState.SchedulePersistentAction(new PersistentAction()
      {
        ActionType = 43,
        ActionDataString = stage.ToString()
      }, true);
    }

    public static IObservable<Unit> PerformSendGdprStage(string stageStr)
    {
      int stage = 0;
      if (!int.TryParse(stageStr, out stage) || !GdprTos.IsValidStage(stage))
      {
        ArgumentException e = new ArgumentException("invalid stage to send");
        Log.SendCrashLog((Exception) e, string.Format("stage: {0}", (object) stageStr));
        throw e;
      }
      return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        Log.l("tos2", "send stage | {0}", (object) stage);
        FunXMPP.Connection connection = AppState.GetConnection();
        if (connection == null)
          observer.OnCompleted();
        else
          connection.SendSetGdprStage(stage, (Action) (() =>
          {
            Log.l("tos2", "send stage | success");
            observer.OnNext(new Unit());
            observer.OnCompleted();
          }), (Action<int>) (err =>
          {
            Log.l("tos2", "send stage | error:{0}", (object) err);
            observer.OnCompleted();
          }));
        return (Action) (() => { });
      }));
    }

    public static PersistentAction SendGdprAcceptAction()
    {
      return new PersistentAction() { ActionType = 44 };
    }

    public static IObservable<Unit> PerformSendGdprAccept()
    {
      return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        Log.l("tos2", "attempt sending accept iq");
        FunXMPP.Connection connection = AppState.GetConnection();
        if (connection == null)
          observer.OnCompleted();
        else
          connection.SendSetGdprAccept((Action) (() =>
          {
            Log.l("tos2", "accept | success");
            Settings.GdprTosServerProperty = (string) null;
            Settings.GdprTosCurrentStage = 0;
            observer.OnNext(new Unit());
            observer.OnCompleted();
          }), (Action<int>) (err =>
          {
            Log.l("tos2", "accept | error:{0}", (object) err);
            observer.OnCompleted();
          }));
        return (Action) (() => { });
      }));
    }

    public static void ScheduleAckGdprTosReset()
    {
      AppState.SchedulePersistentAction(new PersistentAction()
      {
        ActionType = 45
      }, true);
    }

    public static IObservable<Unit> PerformAckGdprTosReset()
    {
      return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        Log.l("tos2", "ack reset");
        FunXMPP.Connection connection = AppState.GetConnection();
        if (connection == null)
          observer.OnCompleted();
        else
          connection.SendAckGdprReset((Action) (() =>
          {
            Log.l("tos2", "ack reset | success");
            observer.OnNext(new Unit());
            observer.OnCompleted();
          }), (Action<int>) (err =>
          {
            Log.l("tos2", "ack reset | error:{0}", (object) err);
            observer.OnCompleted();
          }));
        return (Action) (() => { });
      }));
    }

    public static void ScheduleSendGdprTosPage(int page)
    {
      AppState.SchedulePersistentAction(new PersistentAction()
      {
        ActionType = 46,
        ActionDataString = page.ToString()
      }, true);
    }

    public static IObservable<Unit> PerformSendGdprTosPage(string page)
    {
      return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        Log.l("tos2", "send tos page: {0}", (object) page);
        FunXMPP.Connection connection = AppState.GetConnection();
        if (connection == null)
          observer.OnCompleted();
        else
          connection.SendSetGdprPage(page, (Action) (() =>
          {
            Log.l("tos2", "send tos page | success");
            observer.OnNext(new Unit());
            observer.OnCompleted();
          }), (Action<int>) (err =>
          {
            Log.l("tos2", "send tos page | error:{0}", (object) err);
            observer.OnCompleted();
          }));
        return (Action) (() => { });
      }));
    }
  }
}
