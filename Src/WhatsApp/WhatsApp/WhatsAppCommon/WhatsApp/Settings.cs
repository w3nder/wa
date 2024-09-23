// Decompiled with JetBrains decompiler
// Type: WhatsApp.Settings
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Devices;
using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;
using WhatsAppNative;


namespace WhatsApp
{
  public class Settings
  {
    private static int CountOfKeys = Enum.GetValues(typeof (Settings.Key)).Length;
    public static Settings.Key[] DeprecatedKeys = new Settings.Key[4]
    {
      Settings.Key.BBFR,
      Settings.Key.MyJid,
      Settings.Key.NextChallenge,
      Settings.Key.RotatedPassword
    };
    private static object settingsLock = new object();
    private object[] settingsCache;
    private SettingsStorage settingsStorage;
    public static volatile Settings settingsInstance = (Settings) null;
    private static Settings.KeyObserverList[] subscribedKeys = new Settings.KeyObserverList[Settings.CountOfKeys];
    private byte[] recoveryToken;
    private IRecToken pwdToken;
    private string facebookPhoneId;
    private static double? sysFontSize = new double?();
    public static Func<bool> AccessSafeFromBgAgent = (Func<bool>) (() => true);
    private static object danglingAcksLock = new object();
    public static int ServerRequestedFibBackoffStateSaved = Settings.ServerRequestedFibBackoffState;
    private static readonly DateTime DefaultDateActual = new DateTime(2020, 1, 14, 23, 59, 59);
    private static readonly DateTime DefaultDateOfficial = new DateTime(2019, 12, 31, 23, 59, 59);
    private static readonly DateTime DefaultDateMessaging = new DateTime(2019, 5, 28, 23, 59, 59);

    private static Settings Instance
    {
      get
      {
        Settings instanceCapture = Settings.settingsInstance;
        if (instanceCapture == null)
          Settings.PerformWithWriteLock((Action) (() =>
          {
            if (Settings.settingsInstance == null)
            {
              Log.l("settings", "loading instance");
              Settings.settingsInstance = new Settings();
            }
            instanceCapture = Settings.settingsInstance;
          }));
        return instanceCapture;
      }
    }

    private Settings()
    {
      this.settingsStorage = new SettingsStorage();
      this.settingsCache = new object[Settings.CountOfKeys];
    }

    public static IObservable<Settings.Key> GetSettingsChangedObservable(
      Settings.Key[] keysToSubscribe)
    {
      return keysToSubscribe != null && ((IEnumerable<Settings.Key>) keysToSubscribe).Any<Settings.Key>() ? Observable.Create<Settings.Key>((Func<IObserver<Settings.Key>, Action>) (observer =>
      {
        foreach (Settings.Key index in keysToSubscribe)
          Utils.LazyInit<Settings.KeyObserverList>(ref Settings.subscribedKeys[(int) index], (Func<Settings.KeyObserverList>) (() => new Settings.KeyObserverList())).AddObserver(observer);
        return (Action) (() =>
        {
          foreach (Settings.Key index in keysToSubscribe)
            Settings.subscribedKeys[(int) index].RemoveObserver(observer);
        });
      })) : Observable.Never<Settings.Key>();
    }

    private static void NotifyChanged(Settings.Key k)
    {
      Settings.subscribedKeys[(int) k]?.OnNext(k);
    }

    private static void NotifyChanged(Settings.Key[] keys)
    {
      foreach (Settings.Key key in keys)
        Settings.NotifyChanged(key);
    }

    public static void PerformWithWriteLock(Action a)
    {
      ((Action<Action>) (inner =>
      {
        lock (Settings.settingsLock)
          inner();
      }))(a);
    }

    private T HelperGet<T>(Settings.Key key, T defValue)
    {
      object obj1 = this.settingsCache[(int) key];
      if (obj1 != null)
      {
        try
        {
          return (T) obj1;
        }
        catch (Exception ex)
        {
          Log.l(nameof (Settings), "Exception processing {0} to {1}", (object) obj1.GetType().ToString(), (object) defValue.GetType().ToString());
          throw;
        }
      }
      else
      {
        T obj2 = this.Lookup<T>(key, defValue);
        this.settingsCache[(int) key] = (object) obj2;
        return obj2;
      }
    }

    private void HelperSet(Settings.Key key, byte[] value, bool bypassCache = false)
    {
      byte[] a = bypassCache ? (byte[]) null : (byte[]) this.settingsCache[(int) key];
      if (value == null && !bypassCache)
      {
        this.DeleteKey(key);
      }
      else
      {
        if (a != null && a.IsEqualBytes(value))
          return;
        this.Set<byte[]>(key, value, bypassCache, false);
        this.settingsCache[(int) key] = (object) value;
        Settings.NotifyChanged(key);
      }
    }

    private void HelperSet<T>(Settings.Key key, T value, bool bypassCache = false) where T : IComparable
    {
      object obj = bypassCache ? (object) null : this.settingsCache[(int) key];
      if ((object) value == null && !bypassCache)
      {
        this.DeleteKey(key);
      }
      else
      {
        if (obj != null && ((T) obj).CompareTo((object) value) == 0)
          return;
        this.Set<T>(key, value, bypassCache, false);
        this.settingsCache[(int) key] = (object) value;
        Settings.NotifyChanged(key);
      }
    }

    private void HelperSet<T>(Settings.Key key, T? value, bool bypassCache = false) where T : struct, IComparable
    {
      T? nullable = bypassCache ? new T?() : (T?) this.settingsCache[(int) key];
      if (!value.HasValue && !bypassCache)
      {
        this.DeleteKey(key);
      }
      else
      {
        if (nullable.HasValue && nullable.GetValueOrDefault().CompareTo((object) value) == 0)
          return;
        this.Set<T?>(key, value, bypassCache, false);
        this.settingsCache[(int) key] = (object) value;
        Settings.NotifyChanged(key);
      }
    }

    public static int LocationProvider
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.LocationProvider, -1);
      set => Settings.Instance.HelperSet<int>(Settings.Key.LocationProvider, value);
    }

    public static int GifSearchProvider
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.GifSearchProvider, -1);
      set => Settings.Instance.HelperSet<int>(Settings.Key.GifSearchProvider, value);
    }

    public static int WaAdminForceGifProvider
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.WaAdminForceGifProvider, -1);
      set => Settings.Instance.HelperSet<int>(Settings.Key.WaAdminForceGifProvider, value);
    }

    public static string PhoneNumber
    {
      get => Settings.Instance.HelperGet<string>(Settings.Key.PhoneNumber, string.Empty);
      set => Settings.Instance.HelperSet<string>(Settings.Key.PhoneNumber, value);
    }

    public static string CountryCode
    {
      get => Settings.Instance.HelperGet<string>(Settings.Key.CountryCode, string.Empty);
      set => Settings.Instance.HelperSet<string>(Settings.Key.CountryCode, value);
    }

    public static DateTime PhoneNumberVerificationTimeoutUtc
    {
      get
      {
        return Settings.Instance.HelperGet<DateTime>(Settings.Key.PhoneNumberVerificationTimeoutUtc, new DateTime(0L));
      }
      set
      {
        Settings.Instance.HelperSet<DateTime>(Settings.Key.PhoneNumberVerificationTimeoutUtc, value);
      }
    }

    public static DateTime PhoneNumberVerificationRetryUtc
    {
      get
      {
        return Settings.Instance.HelperGet<DateTime>(Settings.Key.PhoneNumberVerificationRetryUtc, new DateTime(0L));
      }
      set
      {
        Settings instance = Settings.Instance;
        instance.HelperSet<DateTime>(Settings.Key.PhoneNumberVerificationRetryUtc, value);
        Settings.RegTimerToastShown = false;
        instance.DeleteKey(Settings.Key.RegProgressbarStartUtc);
      }
    }

    public static DateTime CodeEntryWaitToRetryUtc
    {
      get
      {
        return Settings.Instance.HelperGet<DateTime>(Settings.Key.CodeEntryWaitToRetryUtc, new DateTime(0L));
      }
      set
      {
        Settings instance = Settings.Instance;
        instance.HelperSet<DateTime>(Settings.Key.CodeEntryWaitToRetryUtc, value);
        Settings.RegTimerToastShown = false;
        instance.DeleteKey(Settings.Key.RegProgressbarStartUtc);
      }
    }

    public static bool RegTimerToastShown
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.RegTimerToastShown, false);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.RegTimerToastShown, value, true);
    }

    public static DateTime RegProgressbarStartUtc
    {
      get
      {
        return Settings.Instance.HelperGet<DateTime>(Settings.Key.RegProgressbarStartUtc, new DateTime(0L));
      }
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.RegProgressbarStartUtc, value);
    }

    public static PhoneNumberVerificationState PhoneNumberVerificationState
    {
      get
      {
        return Settings.Instance.HelperGet<PhoneNumberVerificationState>(Settings.Key.PhoneNumberVerificationState, PhoneNumberVerificationState.NewlyEntered);
      }
      set
      {
        Settings.Instance.HelperSet<PhoneNumberVerificationState>(Settings.Key.PhoneNumberVerificationState, value);
        switch (value)
        {
          case PhoneNumberVerificationState.NewlyEntered:
            Settings.PhoneNumberAccountCreationType = PhoneNumberAccountCreationType.Unknown;
            break;
          case PhoneNumberVerificationState.VerifiedPendingBackupCheck:
          case PhoneNumberVerificationState.VerifiedPendingHistoryRestore:
          case PhoneNumberVerificationState.Verified:
            NativeInterfaces.Misc.CloseRegLog();
            break;
        }
      }
    }

    public static PhoneNumberAccountCreationType PhoneNumberAccountCreationType
    {
      get
      {
        return Settings.Instance.HelperGet<PhoneNumberAccountCreationType>(Settings.Key.PhoneNumberAccountCreationType, PhoneNumberAccountCreationType.Unknown);
      }
      set
      {
        Settings.Instance.HelperSet<PhoneNumberAccountCreationType>(Settings.Key.PhoneNumberAccountCreationType, value);
      }
    }

    public static int MaxPreKeyBatchSize
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.MaxPreKeyBatchSize, 812);
      set => Settings.Instance.HelperSet<int>(Settings.Key.MaxPreKeyBatchSize, value);
    }

    public static string MyJid
    {
      get
      {
        Settings instance = Settings.Instance;
        if (instance.settingsCache[8] != null)
          return (string) instance.settingsCache[8];
        string myJid = Settings.ChatID;
        if (myJid != null)
          myJid = string.Format("{0}@s.whatsapp.net", (object) myJid);
        instance.settingsCache[8] = (object) myJid;
        return myJid;
      }
    }

    public static byte[] NextChallenge
    {
      get => Settings.Instance.HelperGet<byte[]>(Settings.Key.NextChallenge2, (byte[]) null);
      set => Settings.Instance.HelperSet(Settings.Key.NextChallenge2, value);
    }

    public static string ChatID
    {
      get => Settings.Instance.HelperGet<string>(Settings.Key.ChatID, (string) null);
      set
      {
        Settings instance = Settings.Instance;
        instance.HelperSet<string>(Settings.Key.ChatID, value);
        instance.settingsCache[8] = (object) null;
      }
    }

    public static string OldChatID
    {
      get => Settings.Instance.HelperGet<string>(Settings.Key.OldChatID, (string) null);
      set => Settings.Instance.HelperSet<string>(Settings.Key.OldChatID, value);
    }

    public static string OldPhoneNumber
    {
      get => Settings.Instance.HelperGet<string>(Settings.Key.OldPhoneNumber, (string) null);
      set => Settings.Instance.HelperSet<string>(Settings.Key.OldPhoneNumber, value);
    }

    public static string OldCountryCode
    {
      get => Settings.Instance.HelperGet<string>(Settings.Key.OldCountryCode, (string) null);
      set => Settings.Instance.HelperSet<string>(Settings.Key.OldCountryCode, value);
    }

    public static string PushName
    {
      get => Settings.Instance.HelperGet<string>(Settings.Key.PushName, string.Empty);
      set => Settings.Instance.HelperSet<string>(Settings.Key.PushName, value);
    }

    public static bool PreviewEnabled
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.PreviewEnabled, true);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.PreviewEnabled, value);
    }

    public static DateTime? LastKnownServerTimeUtc
    {
      get
      {
        return Settings.Instance.HelperGet<DateTime?>(Settings.Key.LastKnownServerTimeUtc, new DateTime?());
      }
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.LastKnownServerTimeUtc, value);
    }

    public static double LastLocalServerTimeDiff
    {
      get => Settings.Instance.HelperGet<double>(Settings.Key.LastLocalServerTimeDiff, 0.0);
      set => Settings.Instance.HelperSet<double>(Settings.Key.LastLocalServerTimeDiff, value);
    }

    public static DateTime? EULAAcceptedUtc
    {
      get => Settings.Instance.HelperGet<DateTime?>(Settings.Key.EULAAcceptedUtc, new DateTime?());
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.EULAAcceptedUtc, value);
    }

    public static DateTime? RegistrationCompleteUtc
    {
      get
      {
        return Settings.Instance.HelperGet<DateTime?>(Settings.Key.RegistrationCompleteUtc, new DateTime?());
      }
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.RegistrationCompleteUtc, value);
    }

    public static DateTime? LastFullSyncUtc
    {
      get => Settings.Instance.HelperGet<DateTime?>(Settings.Key.LastFullSyncUtc, new DateTime?());
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.LastFullSyncUtc, value);
    }

    public static DateTime? NextFullSyncUtc
    {
      get => Settings.Instance.HelperGet<DateTime?>(Settings.Key.NextFullSyncUtc, new DateTime?());
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.NextFullSyncUtc, value);
    }

    public static DateTime? SyncBackoffUtc
    {
      get => Settings.Instance.HelperGet<DateTime?>(Settings.Key.SyncBackoff, new DateTime?());
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.SyncBackoff, value);
    }

    public static byte[] SyncHistory
    {
      get => Settings.Instance.HelperGet<byte[]>(Settings.Key.SyncHistory, (byte[]) null);
      set => Settings.Instance.HelperSet(Settings.Key.SyncHistory, value);
    }

    public static byte[] LastAddressbookSize
    {
      get => Settings.Instance.HelperGet<byte[]>(Settings.Key.LastAddressbookSize, (byte[]) null);
      set => Settings.Instance.HelperSet(Settings.Key.LastAddressbookSize, value);
    }

    public static DateTime? LastGroupsUpdatedUtc
    {
      get
      {
        return Settings.Instance.HelperGet<DateTime?>(Settings.Key.LastGroupsUpdatedUtc, new DateTime?());
      }
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.LastGroupsUpdatedUtc, value);
    }

    public static bool FirstRegistrationUIShown
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.FirstRegistrationUIShown, false);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.FirstRegistrationUIShown, value);
    }

    public static DateTime? LastGoogleMapsFailureUtc
    {
      get
      {
        return Settings.Instance.HelperGet<DateTime?>(Settings.Key.LastGoogleMapsFailureUtc, new DateTime?());
      }
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.LastGoogleMapsFailureUtc, value);
    }

    public static int MaxGroupParticipants
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.MaxGroupParticipants, 9999);
      set => Settings.Instance.HelperSet<int>(Settings.Key.MaxGroupParticipants, value);
    }

    public static int MaxGroupSubject
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.MaxGroupSubject, 25);
      set => Settings.Instance.HelperSet<int>(Settings.Key.MaxGroupSubject, value);
    }

    public static int MaxListRecipients
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.MaxListRecipients, 256);
      set => Settings.Instance.HelperSet<int>(Settings.Key.MaxListRecipients, value);
    }

    public static bool ShouldQueryBroadcastLists
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.ShouldQueryBroadcastLists, true);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.ShouldQueryBroadcastLists, value);
    }

    public static DateTime? LastPropertiesQueryUtc
    {
      get
      {
        return Settings.Instance.HelperGet<DateTime?>(Settings.Key.LastPropertiesQueryUtc, new DateTime?());
      }
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.LastPropertiesQueryUtc, value);
    }

    public static int LastGoodPortIndex
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.LastGoodPortIndex, 0);
      set => Settings.Instance.HelperSet<int>(Settings.Key.LastGoodPortIndex, value);
    }

    public static int LocalTileCount
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.LocalTileCount, 0);
      set => Settings.Instance.HelperSet<int>(Settings.Key.LocalTileCount, value, true);
    }

    public static DateTime? SuccessfulLoginUtc
    {
      get
      {
        return Settings.Instance.HelperGet<DateTime?>(Settings.Key.SuccessfulLoginUtc, new DateTime?());
      }
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.SuccessfulLoginUtc, value);
    }

    public static DateTime? LastChannelReopenUtc
    {
      get
      {
        return Settings.Instance.HelperGet<DateTime?>(Settings.Key.LastChannelReopenUtc, new DateTime?());
      }
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.LastChannelReopenUtc, value);
    }

    public static byte[] RecoveryToken
    {
      get
      {
        lock (Settings.settingsLock)
        {
          Settings instance1 = Settings.Instance;
          if (instance1.recoveryToken != null)
            return instance1.recoveryToken;
          IRecToken instance2 = (IRecToken) NativeInterfaces.CreateInstance<RecToken>();
          try
          {
            return instance1.recoveryToken = instance2.GetToken(0).Get();
          }
          finally
          {
          }
        }
      }
    }

    private IRecToken PwdToken
    {
      get
      {
        lock (Settings.settingsLock)
        {
          if (this.pwdToken == null)
            this.pwdToken = (IRecToken) NativeInterfaces.CreateInstance<RecToken>();
        }
        return this.pwdToken;
      }
    }

    public static int CodeLength
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.CodeLength, 6);
      set => Settings.Instance.HelperSet<int>(Settings.Key.CodeLength, value);
    }

    public static bool RecoveryTokenSet
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.RecoveryTokenSet, false);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.RecoveryTokenSet, value);
    }

    public static string FacebookPhoneId
    {
      get
      {
        string facebookPhoneId = (string) null;
        Settings instance1 = Settings.Instance;
        if (instance1.facebookPhoneId == null)
        {
          IRecToken instance2 = (IRecToken) NativeInterfaces.CreateInstance<RecToken>();
          try
          {
            facebookPhoneId = instance1.facebookPhoneId = Convert.ToBase64String(instance2.GetToken(1).Get());
          }
          catch (Exception ex)
          {
            Log.LogException(ex, "phone id");
            return "";
          }
          finally
          {
          }
        }
        else
          facebookPhoneId = instance1.facebookPhoneId;
        return facebookPhoneId;
      }
    }

    public static bool EnterKeyIsSend
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.EnterKeyIsSend, true);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.EnterKeyIsSend, value);
    }

    public static int LastEmojiTab
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.LastEmojiTab, 1);
      set => Settings.Instance.HelperSet<int>(Settings.Key.LastEmojiTab, value);
    }

    public static string LastEmojiPagePositions
    {
      get => Settings.Instance.HelperGet<string>(Settings.Key.LastEmojiPagePositions, "0,0,0,0,0");
      set => Settings.Instance.HelperSet<string>(Settings.Key.LastEmojiPagePositions, value);
    }

    public static DateTime? LastBatterySaverAlertedUtc
    {
      get
      {
        return Settings.Instance.HelperGet<DateTime?>(Settings.Key.LastBatterySaverAlertedUtc, new DateTime?());
      }
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.LastBatterySaverAlertedUtc, value);
    }

    public static int DefaultAudioEndpoint
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.DefaultAudioEndpoint, 0);
      set => Settings.Instance.HelperSet<int>(Settings.Key.DefaultAudioEndpoint, value);
    }

    public static byte[] LastPushUriHash
    {
      get => Settings.Instance.HelperGet<byte[]>(Settings.Key.LastPushUriHash, new byte[0]);
      set => Settings.Instance.HelperSet(Settings.Key.LastPushUriHash, value);
    }

    public static int PushUriCount
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.PushUriCount, 0);
      set => Settings.Instance.HelperSet<int>(Settings.Key.PushUriCount, value);
    }

    public static int VoipPushUriCount
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.VoipPushUriCount, 0);
      set => Settings.Instance.HelperSet<int>(Settings.Key.VoipPushUriCount, value);
    }

    public static bool LoginFailed
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.LoginFailed, false);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.LoginFailed, value);
    }

    public static string LoginFailedReason
    {
      get => Settings.Instance.HelperGet<string>(Settings.Key.LoginFailedReason, (string) null);
      set => Settings.Instance.HelperSet<string>(Settings.Key.LoginFailedReason, value);
    }

    public static string LoginFailedReasonCode
    {
      get => Settings.Instance.HelperGet<string>(Settings.Key.LoginFailedReasonCode, (string) null);
      set => Settings.Instance.HelperSet<string>(Settings.Key.LoginFailedReasonCode, value);
    }

    public static DateTime? LoginFailedExpirationUtc
    {
      get
      {
        return Settings.Instance.HelperGet<DateTime?>(Settings.Key.LoginFailedExpirationUtc, new DateTime?());
      }
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.LoginFailedExpirationUtc, value);
    }

    public static long? LoginFailedExpirationTotalSeconds
    {
      get
      {
        return Settings.Instance.HelperGet<long?>(Settings.Key.LoginFailedExpirationTotalSeconds, new long?());
      }
      set
      {
        Settings.Instance.HelperSet<long>(Settings.Key.LoginFailedExpirationTotalSeconds, value);
      }
    }

    public static DateTime? LoginFailedRetryUtc
    {
      get
      {
        return Settings.Instance.HelperGet<DateTime?>(Settings.Key.LoginFailedRetryUtc, new DateTime?());
      }
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.LoginFailedRetryUtc, value);
    }

    public static string LoginFailedUpgradeRequiredVersion
    {
      get
      {
        return Settings.Instance.HelperGet<string>(Settings.Key.LoginFailedUpgradeRequiredVersion, (string) null);
      }
      set
      {
        Settings.Instance.HelperSet<string>(Settings.Key.LoginFailedUpgradeRequiredVersion, value);
      }
    }

    public static int VoipExitReason
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.VoipLastExitReason, 0);
      set => Settings.Instance.HelperSet<int>(Settings.Key.VoipLastExitReason, value);
    }

    public static bool CorruptDb
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.DbCorrupt, false);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.DbCorrupt, value);
    }

    public static bool LockOrientationInPortrait
    {
      get => false;
      set => Settings.Instance.HelperSet<bool>(Settings.Key.LockOrientationInPortrait, value);
    }

    public static string VoipExceptionMessage
    {
      get => Settings.Instance.HelperGet<string>(Settings.Key.VoipExceptionMessage, (string) null);
      set => Settings.Instance.HelperSet<string>(Settings.Key.VoipExceptionMessage, value);
    }

    public static int MaxMediaSize
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.MaxMediaSize, 16777216);
      set => Settings.Instance.HelperSet<int>(Settings.Key.MaxMediaSize, value);
    }

    public static int MaxAutodownloadSize
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.MaxAutodownloadSize, 33554432);
      set => Settings.Instance.HelperSet<int>(Settings.Key.MaxAutodownloadSize, value);
    }

    public static int MaxFileSize
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.MaxFileSize, 67108864);
      set => Settings.Instance.HelperSet<int>(Settings.Key.MaxFileSize, value);
    }

    public static int StatusVideoMaxBitrate
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.StatusVideoMaxBitrate, 2000);
      set => Settings.Instance.HelperSet<int>(Settings.Key.StatusVideoMaxBitrate, value);
    }

    public static int VideoMaxBitrate
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.VideoMaxBitrate, 2000);
      set => Settings.Instance.HelperSet<int>(Settings.Key.VideoMaxBitrate, value);
    }

    public static int StatusVideoMaxDuration
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.StatusVideoMaxDuration, 30);
      set => Settings.Instance.HelperSet<int>(Settings.Key.StatusVideoMaxDuration, value);
    }

    public static int StatusImageMaxEdge
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.StatusImageMaxEdge, 1280);
      set => Settings.Instance.HelperSet<int>(Settings.Key.StatusImageMaxEdge, value);
    }

    public static int StatusImageQuality
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.StatusImageQuality, 50);
      set => Settings.Instance.HelperSet<int>(Settings.Key.StatusImageQuality, value);
    }

    public static int ImageMaxEdge
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.ImageMaxEdge, 1600);
      set => Settings.Instance.HelperSet<int>(Settings.Key.ImageMaxEdge, value);
    }

    public static int ImageMaxKbytes
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.ImageMaxKbytes, 1024);
      set => Settings.Instance.HelperSet<int>(Settings.Key.ImageMaxKbytes, value);
    }

    public static int JpegQuality
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.JpegQuality, 80);
      set => Settings.Instance.HelperSet<int>(Settings.Key.JpegQuality, value);
    }

    public static int VideoPrefetchBytes
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.VideoPrefetchBytes, 262144);
      set => Settings.Instance.HelperSet<int>(Settings.Key.VideoPrefetchBytes, value);
    }

    public static DateTime? EverstoreBackoffUtc
    {
      get
      {
        return Settings.Instance.HelperGet<DateTime?>(Settings.Key.EverstoreBackoffTime, new DateTime?());
      }
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.EverstoreBackoffTime, value);
    }

    public static int EverstoreBackoffAttempt
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.EverstoreBackoffAttempt, 0);
      set => Settings.Instance.HelperSet<int>(Settings.Key.EverstoreBackoffAttempt, value);
    }

    public static double SystemFontSize
    {
      get
      {
        return Settings.sysFontSize ?? (Settings.sysFontSize = new double?(PhoneAccessibilitySettings.GetTextSize())).Value;
      }
      set => Settings.sysFontSize = new double?(value);
    }

    public static void ClearCachedSysFontSize() => Settings.sysFontSize = new double?();

    public static Settings.ScreenResolutionKind ScreenResolution
    {
      get
      {
        return Settings.Instance.HelperGet<Settings.ScreenResolutionKind>(Settings.Key.ScreenResolution, Settings.ScreenResolutionKind.Undefined);
      }
      set
      {
        Settings.Instance.HelperSet<Settings.ScreenResolutionKind>(Settings.Key.ScreenResolution, value);
      }
    }

    public static int ScreenRenderWidth
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.ScreenRenderWidth, 0);
      set => Settings.Instance.HelperSet<int>(Settings.Key.ScreenRenderWidth, value);
    }

    public static int ScreenRenderHeight
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.ScreenRenderHeight, 0);
      set => Settings.Instance.HelperSet<int>(Settings.Key.ScreenRenderHeight, value);
    }

    public static AutoDownloadSetting AutoDownloadImage
    {
      get
      {
        return Settings.Instance.HelperGet<AutoDownloadSetting>(Settings.Key.AutoDownloadSettingImage, AutoDownloadSetting.Enabled);
      }
      set
      {
        Settings.Instance.HelperSet<AutoDownloadSetting>(Settings.Key.AutoDownloadSettingImage, value);
      }
    }

    public static AutoDownloadSetting AutoDownloadAudio
    {
      get
      {
        return Settings.Instance.HelperGet<AutoDownloadSetting>(Settings.Key.AutoDownloadSettingAudio, AutoDownloadSetting.EnabledOnWifi);
      }
      set
      {
        Settings.Instance.HelperSet<AutoDownloadSetting>(Settings.Key.AutoDownloadSettingAudio, value);
      }
    }

    public static AutoDownloadSetting AutoDownloadVideo
    {
      get
      {
        return Settings.Instance.HelperGet<AutoDownloadSetting>(Settings.Key.AutoDownloadSettingVideo, AutoDownloadSetting.EnabledOnWifi);
      }
      set
      {
        Settings.Instance.HelperSet<AutoDownloadSetting>(Settings.Key.AutoDownloadSettingVideo, value);
      }
    }

    public static AutoDownloadSetting AutoDownloadDocument
    {
      get
      {
        return Settings.Instance.HelperGet<AutoDownloadSetting>(Settings.Key.AutoDownloadSettingDocument, Settings.AutoDownloadImage != AutoDownloadSetting.Disabled || Settings.AutoDownloadAudio != AutoDownloadSetting.Disabled || Settings.AutoDownloadVideo != AutoDownloadSetting.Disabled ? AutoDownloadSetting.EnabledOnWifi : AutoDownloadSetting.Disabled);
      }
      set
      {
        Settings.Instance.HelperSet<AutoDownloadSetting>(Settings.Key.AutoDownloadSettingDocument, value);
      }
    }

    public static string IndividualTone
    {
      get => Settings.Instance.HelperGet<string>(Settings.Key.IndividualTone, (string) null);
      set => Settings.Instance.HelperSet<string>(Settings.Key.IndividualTone, value);
    }

    public static string GroupTone
    {
      get => Settings.Instance.HelperGet<string>(Settings.Key.GroupTone, (string) null);
      set => Settings.Instance.HelperSet<string>(Settings.Key.GroupTone, value);
    }

    public static string VoipRingtone
    {
      get => Settings.Instance.HelperGet<string>(Settings.Key.VoipRingtone, (string) null);
      set => Settings.Instance.HelperSet<string>(Settings.Key.VoipRingtone, value);
    }

    public static PrivacyVisibility LastSeenVisibility
    {
      get
      {
        return Settings.Instance.HelperGet<PrivacyVisibility>(Settings.Key.LastSeenVisibility, PrivacyVisibility.Everyone);
      }
      set => Settings.Instance.HelperSet<PrivacyVisibility>(Settings.Key.LastSeenVisibility, value);
    }

    public static PrivacyVisibility StatusVisibility
    {
      get
      {
        return Settings.Instance.HelperGet<PrivacyVisibility>(Settings.Key.StatusVisibility, PrivacyVisibility.Everyone);
      }
      set => Settings.Instance.HelperSet<PrivacyVisibility>(Settings.Key.StatusVisibility, value);
    }

    public static PrivacyVisibility ProfilePhotoVisibility
    {
      get
      {
        return Settings.Instance.HelperGet<PrivacyVisibility>(Settings.Key.ProfilePhotoVisibility, PrivacyVisibility.Everyone);
      }
      set
      {
        Settings.Instance.HelperSet<PrivacyVisibility>(Settings.Key.ProfilePhotoVisibility, value);
      }
    }

    public static DateTime? LastPrivacyCheckUtc
    {
      get
      {
        return Settings.Instance.HelperGet<DateTime?>(Settings.Key.LastPrivacyCheckUtc, new DateTime?());
      }
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.LastPrivacyCheckUtc, value);
    }

    public static DateTime? LastBlockListCheckUtc
    {
      get
      {
        return Settings.Instance.HelperGet<DateTime?>(Settings.Key.LastBlockListCheckUtc, new DateTime?());
      }
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.LastBlockListCheckUtc, value);
    }

    public static bool LastSeenVisibilityInDoubt
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.LastSeenVisibilityInDoubt, true);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.LastSeenVisibilityInDoubt, value);
    }

    public static bool StatusVisibilityInDoubt
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.StatusVisibilityInDoubt, true);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.StatusVisibilityInDoubt, value);
    }

    public static bool ProfilePhotoVisibilityInDoubt
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.ProfilePhotoVisibilityInDoubt, true);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.ProfilePhotoVisibilityInDoubt, value);
    }

    public static bool EnableReadReceipts
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.EnableReadReceipts, true);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.EnableReadReceipts, value);
    }

    public static bool EnableReadReceiptInDoubt
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.EnableReadReceiptInDoubt, true);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.EnableReadReceiptInDoubt, value);
    }

    public static bool VacationModeEnabled
    {
      get
      {
        return Settings.IsWaAdmin && Settings.Instance.HelperGet<bool>(Settings.Key.VacationModeEnabled, false);
      }
      set => Settings.Instance.HelperSet<bool>(Settings.Key.VacationModeEnabled, value);
    }

    public static int? StatusV3Override
    {
      get => Settings.Instance.HelperGet<int?>(Settings.Key.StatusV3Override, new int?());
      set => Settings.Instance.HelperSet<int>(Settings.Key.StatusV3Override, value);
    }

    public static bool StatusRecipientsStateDirty
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.StatusRecipientsStateDirty, true);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.StatusRecipientsStateDirty, value);
    }

    public static WaStatusHelper.StatusPrivacySettings StatusV3PrivacySetting
    {
      get
      {
        return (WaStatusHelper.StatusPrivacySettings) Settings.Instance.HelperGet<int>(Settings.Key.StatusV3PrivacySetting, 99);
      }
      set
      {
        Settings.Instance.HelperSet<int>(Settings.Key.StatusV3PrivacySetting, (int) value);
        Settings.StatusRecipientsStateDirty = true;
      }
    }

    public static DateTime? LastEnableReadReceiptsTimeUtc
    {
      get
      {
        return Settings.Instance.HelperGet<DateTime?>(Settings.Key.LastEnableReadReceiptsTimeUtc, new DateTime?());
      }
      set
      {
        Settings.Instance.HelperSet<DateTime>(Settings.Key.LastEnableReadReceiptsTimeUtc, value);
      }
    }

    public static DateTime? LastSeenMissedCallTimeUtc
    {
      get
      {
        return Settings.Instance.HelperGet<DateTime?>(Settings.Key.LastSeenMissedCallTimeUtc, new DateTime?());
      }
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.LastSeenMissedCallTimeUtc, value);
    }

    public static DateTime? LastSeenStatusListTimeUtc
    {
      get
      {
        return Settings.Instance.HelperGet<DateTime?>(Settings.Key.LastSeenStatusListTimeUtc, new DateTime?());
      }
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.LastSeenStatusListTimeUtc, value);
    }

    public static int VideoCallPreviewLocation
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.VideoCallPreviewLocation, 0);
      set => Settings.Instance.HelperSet<int>(Settings.Key.VideoCallPreviewLocation, value);
    }

    public static string GlobalWallpaper
    {
      get => Settings.Instance.HelperGet<string>(Settings.Key.GlobalWallpaper, (string) null);
      set => Settings.Instance.HelperSet<string>(Settings.Key.GlobalWallpaper, value);
    }

    public static bool IsUpdateAvailable
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.IsUpdateAvailable, false);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.IsUpdateAvailable, value);
    }

    public static DateTime? LastCheckForUpdatesUtc
    {
      get
      {
        return Settings.Instance.HelperGet<DateTime?>(Settings.Key.LastCheckForUpdatesUtc, new DateTime?());
      }
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.LastCheckForUpdatesUtc, value);
    }

    public static bool MapCartographicModeRoad
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.MapCartographicModeRoad, true);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.MapCartographicModeRoad, value);
    }

    public static double LastKnownLatitude
    {
      get => Settings.Instance.HelperGet<double>(Settings.Key.LastKnownLatitude, 37.391441);
      set => Settings.Instance.HelperSet<double>(Settings.Key.LastKnownLatitude, value);
    }

    public static double LastKnownLongitude
    {
      get => Settings.Instance.HelperGet<double>(Settings.Key.LastKnownLongitude, -122.080161);
      set => Settings.Instance.HelperSet<double>(Settings.Key.LastKnownLongitude, value);
    }

    public static bool SuppressRestoreFromBackupAtReg
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.SuppressRestoreFromBackupAtReg, false);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.SuppressRestoreFromBackupAtReg, value);
    }

    public static int Jitter
    {
      get
      {
        Settings instance = Settings.Instance;
        int? nullable = instance.HelperGet<int?>(Settings.Key.Jitter, new int?());
        if (!nullable.HasValue)
        {
          int r = DateTime.Now.Second % 8;
          instance.settingsCache[100] = (object) r;
          WAThreadPool.QueueUserWorkItem((Action) (() => Settings.Jitter = r));
          nullable = new int?(r);
        }
        return nullable.Value;
      }
      set => Settings.Instance.HelperSet<int>(Settings.Key.Jitter, value);
    }

    public static int LastWarningTime
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.LastWarningTime, 30);
      set => Settings.Instance.HelperSet<int>(Settings.Key.LastWarningTime, value);
    }

    public static int NumberOfGroups
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.NumberOfGroups, -1);
      set => Settings.Instance.HelperSet<int>(Settings.Key.NumberOfGroups, value);
    }

    public static DateTime? LastDailyStatsCollectedUtc
    {
      get
      {
        return Settings.Instance.HelperGet<DateTime?>(Settings.Key.LastDailyStatsCollectedUtc, new DateTime?());
      }
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.LastDailyStatsCollectedUtc, value);
    }

    public static DateTime? LastWeeklyStatsCollectedUtc
    {
      get
      {
        return Settings.Instance.HelperGet<DateTime?>(Settings.Key.LastWeeklyStatsCollectedUtc, new DateTime?());
      }
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.LastWeeklyStatsCollectedUtc, value);
    }

    public static DateTime? LastMonthlyStatsCollectUtc
    {
      get
      {
        return Settings.Instance.HelperGet<DateTime?>(Settings.Key.LastMonthlyStatsCollectUtc, new DateTime?());
      }
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.LastMonthlyStatsCollectUtc, value);
    }

    public static bool ForceServerPropsReload
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.ForceServerPropsReload, false);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.ForceServerPropsReload, value);
    }

    public static byte[] QrBlob
    {
      get => Settings.Instance.HelperGet<byte[]>(Settings.Key.QrBlob, (byte[]) null);
      set => Settings.Instance.HelperSet(Settings.Key.QrBlob, value);
    }

    public static byte[] QrActionsBlob
    {
      get => Settings.Instance.HelperGet<byte[]>(Settings.Key.QrActionsBlob, (byte[]) null);
      set => Settings.Instance.HelperSet(Settings.Key.QrActionsBlob, value);
    }

    public static byte[] StatusesBlob
    {
      get => Settings.Instance.HelperGet<byte[]>(Settings.Key.StatusesBlob, (byte[]) null);
      set => Settings.Instance.HelperSet(Settings.Key.StatusesBlob, value);
    }

    public static void UpdateContactsChecksum()
    {
      int num = new Random((int) DateTime.UtcNow.ToUnixTime()).Next(1, 999998);
      if (num >= Settings.ContactsChecksum)
        ++num;
      Settings.Instance.HelperSet<int>(Settings.Key.ContactsChecksum, num);
    }

    public static int ContactsChecksum
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.ContactsChecksum, 0);
    }

    public static int ServerPropsVersion
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.ServerPropsVersion, 4);
      set => Settings.Instance.HelperSet<int>(Settings.Key.ServerPropsVersion, value);
    }

    public static bool WnsRegistered
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.WnsRegistered, false);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.WnsRegistered, value);
    }

    public static void ModifyDanglingAcks(Action<List<FunXMPP.Connection.Ack>> operation)
    {
      lock (Settings.danglingAcksLock)
      {
        List<FunXMPP.Connection.Ack> danglingAcks = Settings.DanglingAcks;
        operation(danglingAcks);
        Settings.DanglingAcks = danglingAcks;
      }
    }

    public static List<FunXMPP.Connection.Ack> DanglingAcks
    {
      get
      {
        lock (Settings.danglingAcksLock)
          return ((IEnumerable<string>) Settings.Instance.HelperGet<string>(Settings.Key.DanglingAcks, "").Split(new char[1], StringSplitOptions.RemoveEmptyEntries)).Select<string, FunXMPP.Connection.Ack>((Func<string, int, FunXMPP.Connection.Ack>) ((ackString, _) => FunXMPP.Connection.Ack.FromString(ackString))).ToList<FunXMPP.Connection.Ack>();
      }
      set
      {
        lock (Settings.danglingAcksLock)
          Settings.Instance.HelperSet<string>(Settings.Key.DanglingAcks, string.Join("\0", value.Select<FunXMPP.Connection.Ack, string>((Func<FunXMPP.Connection.Ack, int, string>) ((ack, _) => ack.ToString()))));
      }
    }

    public static string[] RemovedJids
    {
      get
      {
        return Utils.ParsePpsz(Settings.Instance.HelperGet<string>(Settings.Key.RemovedJids, "")).ToArray();
      }
      set
      {
        if (value == null || value.Length == 0)
          Settings.Instance.DeleteKey(Settings.Key.RemovedJids);
        else
          Settings.Instance.HelperSet<string>(Settings.Key.RemovedJids, Utils.ToPpsz((IEnumerable<string>) value));
      }
    }

    public static bool EnableInAppNotificationToast
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.EnableInAppNotificationToast, true);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.EnableInAppNotificationToast, value);
    }

    public static bool EnableInAppNotificationSound
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.EnableInAppNotificationSound, true);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.EnableInAppNotificationSound, value);
    }

    public static bool EnableInAppNotificationVibrate
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.EnableInAppNotificationVibrate, true);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.EnableInAppNotificationVibrate, value);
    }

    public static bool EnableGroupAlerts
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.EnableGroupAlerts, true);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.EnableGroupAlerts, value);
    }

    public static bool EnableIndividualAlerts
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.EnableIndividualAlerts, true);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.EnableIndividualAlerts, value);
    }

    public static bool SaveIncomingMedia
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.SaveIncomingMedia, true);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.SaveIncomingMedia, value);
    }

    public static bool ShowStatusPresendTooltip
    {
      get
      {
        return Settings.StatusV3PrivacySetting != WaStatusHelper.StatusPrivacySettings.WhiteList && Settings.StatusV3PrivacySetting != WaStatusHelper.StatusPrivacySettings.BlackList && Settings.Instance.HelperGet<bool>(Settings.Key.ShowStatusPresendTooltip, true);
      }
      set => Settings.Instance.HelperSet<bool>(Settings.Key.ShowStatusPresendTooltip, value);
    }

    public static bool ShowStatusRecipientPickerTooltip
    {
      get
      {
        return Settings.StatusV3PrivacySetting != WaStatusHelper.StatusPrivacySettings.WhiteList && Settings.StatusV3PrivacySetting != WaStatusHelper.StatusPrivacySettings.BlackList && Settings.Instance.HelperGet<bool>(Settings.Key.ShowStatusRecipientPickerTooltip, true);
      }
      set
      {
        Settings.Instance.HelperSet<bool>(Settings.Key.ShowStatusRecipientPickerTooltip, value);
      }
    }

    public static bool ShowStatusTabTooltipBanner
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.ShowStatusTabTooltipBanner, true);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.ShowStatusTabTooltipBanner, value);
    }

    public static bool IsStatusPSAUnseen
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.IsStatusPSAUnseen, false);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.IsStatusPSAUnseen, value);
    }

    public static bool IsWaAdmin
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.IsWaAdmin, false);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.IsWaAdmin, value);
    }

    public static bool LowBandwidthVoip
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.LowBandwidthVoip, Settings.IsWaAdmin);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.LowBandwidthVoip, value);
    }

    public static bool AudioVideoSwitchEnabled
    {
      get
      {
        return Settings.Instance.HelperGet<bool>(Settings.Key.AudioVideoSwitchEnabled, Settings.IsWaAdmin);
      }
      set => Settings.Instance.HelperSet<bool>(Settings.Key.AudioVideoSwitchEnabled, value);
    }

    public static GdprReport.States GdprReportState
    {
      get
      {
        return Settings.Instance.HelperGet<GdprReport.States>(Settings.Key.GdprReportState, GdprReport.States.Init);
      }
      set => Settings.Instance.HelperSet<GdprReport.States>(Settings.Key.GdprReportState, value);
    }

    public static DateTime? GdprReportReadyTimeUtc
    {
      get
      {
        return Settings.Instance.HelperGet<DateTime?>(Settings.Key.GdprReportReadyTimeUtc, new DateTime?());
      }
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.GdprReportReadyTimeUtc, value);
    }

    public static DateTime? GdprReportCreationTimeUtc
    {
      get
      {
        return Settings.Instance.HelperGet<DateTime?>(Settings.Key.GdprReportCreationTimeUtc, new DateTime?());
      }
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.GdprReportCreationTimeUtc, value);
    }

    public static DateTime? GdprReportExpirationTimeUtc
    {
      get
      {
        return Settings.Instance.HelperGet<DateTime?>(Settings.Key.GdprReportExpirationTimeUtc, new DateTime?());
      }
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.GdprReportExpirationTimeUtc, value);
    }

    public static string GdprReportFilepath
    {
      get => Settings.Instance.HelperGet<string>(Settings.Key.GdprReportFilepath, (string) null);
      set => Settings.Instance.HelperSet<string>(Settings.Key.GdprReportFilepath, value);
    }

    public static byte[] GdprReportInfo
    {
      get => Settings.Instance.HelperGet<byte[]>(Settings.Key.GdprReportInfo, (byte[]) null);
      set => Settings.Instance.HelperSet(Settings.Key.GdprReportInfo, value);
    }

    public static bool GdprReportEnabled
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.GdprReportEnabled, Settings.IsWaAdmin);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.GdprReportEnabled, value);
    }

    public static byte[] UploadUniqueBytes
    {
      get
      {
        byte[] uploadUniqueBytes = Settings.Instance.HelperGet<byte[]>(Settings.Key.UploadRandomBytes, (byte[]) null);
        if (uploadUniqueBytes == null)
        {
          Random random = new Random((int) (DateTime.UtcNow.Ticks & (long) uint.MaxValue));
          uploadUniqueBytes = new byte[32];
          byte[] buffer = uploadUniqueBytes;
          random.NextBytes(buffer);
          Settings.Instance.HelperSet(Settings.Key.UploadRandomBytes, uploadUniqueBytes);
        }
        return uploadUniqueBytes;
      }
    }

    public static bool E2EVerificationEnabled
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.E2EVerificationEnabled, false);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.E2EVerificationEnabled, value);
    }

    public static bool E2EVerificationCleanup
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.E2EVerificationCleanup, false);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.E2EVerificationCleanup, value);
    }

    public static string RegistrationSmsDetails
    {
      get => Settings.Instance.HelperGet<string>(Settings.Key.RegistrationSMSCode, (string) null);
      set => Settings.Instance.HelperSet<string>(Settings.Key.RegistrationSMSCode, value);
    }

    public static string DeprecationDetails
    {
      get => Settings.Instance.HelperGet<string>(Settings.Key.DeprecationDetails, (string) null);
      set => Settings.Instance.HelperSet<string>(Settings.Key.DeprecationDetails, value);
    }

    public static int AxolotlRegistrationRetries
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.AxolotlRegistrationRetries, 0);
      set => Settings.Instance.HelperSet<int>(Settings.Key.AxolotlRegistrationRetries, value);
    }

    public static bool MPNSChatTileExists
    {
      get
      {
        return AppState.UseWindowsNotificationService ? Settings.Instance.HelperGet<bool>(Settings.Key.ChatTileExists, false) : Settings.Instance.HelperGet<bool>(Settings.Key.ChatTileExists, true);
      }
      set => Settings.Instance.HelperSet<bool>(Settings.Key.ChatTileExists, value);
    }

    public static string LastContactException
    {
      get => Settings.Instance.HelperGet<string>(Settings.Key.LastContactException, (string) null);
      set => Settings.Instance.HelperSet<string>(Settings.Key.LastContactException, value);
    }

    public static FlashMode? UserFlashSetting
    {
      get
      {
        return Settings.Instance.HelperGet<FlashMode?>(Settings.Key.UserFlashSetting, new FlashMode?());
      }
      set => Settings.Instance.HelperSet<FlashMode>(Settings.Key.UserFlashSetting, value);
    }

    public static CameraType? UserCameraTypeSetting
    {
      get
      {
        return Settings.Instance.HelperGet<CameraType?>(Settings.Key.UserCameraTypeSetting, new CameraType?());
      }
      set => Settings.Instance.HelperSet<CameraType>(Settings.Key.UserCameraTypeSetting, value);
    }

    public static bool ShowPushNameScreen
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.ShowPushNameScreen, false);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.ShowPushNameScreen, value);
    }

    public static bool LiveLocationIsNewUser
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.LiveLocationIsNewUser, true);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.LiveLocationIsNewUser, value);
    }

    public static byte[] ClientStaticPrivateKey
    {
      get
      {
        Settings instance1 = Settings.Instance;
        byte[] bytes = instance1.Lookup<byte[]>(Settings.Key.ClientStaticPrivateKey, (byte[]) null);
        if (bytes == null)
        {
          WAProtocol.GenerateClientStaticKeyPair();
          bytes = instance1.Lookup<byte[]>(Settings.Key.ClientStaticPrivateKey, (byte[]) null);
        }
        IRecToken pwdToken = instance1.PwdToken;
        try
        {
          IByteBuffer instance2 = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
          instance2.Put(bytes);
          pwdToken.Decode(instance2);
          return instance2.Get();
        }
        catch (Exception ex)
        {
          Log.SendCrashLog(ex, "decode client static private key");
          return new byte[0];
        }
      }
      set
      {
        Settings instance1 = Settings.Instance;
        byte[] bytes = value;
        IRecToken pwdToken = instance1.PwdToken;
        IByteBuffer instance2 = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
        instance2.Put(bytes);
        IByteBuffer Bytes = instance2;
        pwdToken.Encode(Bytes);
        instance1.Set<byte[]>(Settings.Key.ClientStaticPrivateKey, instance2.Get());
      }
    }

    public static byte[] ClientStaticPublicKey
    {
      get
      {
        Settings instance1 = Settings.Instance;
        byte[] bytes = instance1.Lookup<byte[]>(Settings.Key.ClientStaticPublicKey, (byte[]) null);
        if (bytes == null)
        {
          WAProtocol.GenerateClientStaticKeyPair();
          bytes = instance1.Lookup<byte[]>(Settings.Key.ClientStaticPublicKey, (byte[]) null);
        }
        IRecToken pwdToken = instance1.PwdToken;
        try
        {
          IByteBuffer instance2 = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
          instance2.Put(bytes);
          pwdToken.Decode(instance2);
          return instance2.Get();
        }
        catch (Exception ex)
        {
          Log.SendCrashLog(ex, "decode client static public key");
          return new byte[0];
        }
      }
      set
      {
        Settings instance1 = Settings.Instance;
        byte[] bytes = value;
        IRecToken pwdToken = instance1.PwdToken;
        IByteBuffer instance2 = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
        instance2.Put(bytes);
        IByteBuffer Bytes = instance2;
        pwdToken.Encode(Bytes);
        instance1.Set<byte[]>(Settings.Key.ClientStaticPublicKey, instance2.Get());
      }
    }

    public static byte[] ServerStaticPublicKey
    {
      get => Settings.Instance.HelperGet<byte[]>(Settings.Key.ServerStaticPublicKey, (byte[]) null);
      set => Settings.Instance.HelperSet(Settings.Key.ServerStaticPublicKey, value);
    }

    public static int MaxVideoEdge
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.MaxVideoEdge, 960);
      set => Settings.Instance.HelperSet<int>(Settings.Key.MaxVideoEdge, value);
    }

    public static bool DisablePushOnDebug
    {
      get => false;
      set
      {
      }
    }

    public static DateTime? LastSignedPreKeySent
    {
      get
      {
        return Settings.Instance.HelperGet<DateTime?>(Settings.Key.LastSignedPreKeySent, new DateTime?());
      }
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.LastSignedPreKeySent, value);
    }

    public static OneDriveBackupFrequency OneDriveBackupFrequency
    {
      get
      {
        return Settings.Instance.HelperGet<OneDriveBackupFrequency>(Settings.Key.OneDriveBackupFrequency, OneDriveBackupFrequency.Off);
      }
      set
      {
        Settings.Instance.HelperSet<OneDriveBackupFrequency>(Settings.Key.OneDriveBackupFrequency, value);
      }
    }

    public static AutoDownloadSetting OneDriveBackupNetwork
    {
      get
      {
        return Settings.Instance.HelperGet<AutoDownloadSetting>(Settings.Key.OneDriveBackupNetwork, AutoDownloadSetting.EnabledOnWifi);
      }
      set
      {
        Settings.Instance.HelperSet<AutoDownloadSetting>(Settings.Key.OneDriveBackupNetwork, value);
      }
    }

    public static AutoDownloadSetting OneDriveRestoreNetwork
    {
      get
      {
        return Settings.Instance.HelperGet<AutoDownloadSetting>(Settings.Key.OneDriveRestoreNetwork, AutoDownloadSetting.Enabled);
      }
      set
      {
        Settings.Instance.HelperSet<AutoDownloadSetting>(Settings.Key.OneDriveRestoreNetwork, value);
      }
    }

    public static bool OneDriveIncludeVideos
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.OneDriveIncludeVideos, false);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.OneDriveIncludeVideos, value);
    }

    public static string OneDriveUserId
    {
      get => Settings.Instance.HelperGet<string>(Settings.Key.OneDriveUserId, string.Empty);
      set => Settings.Instance.HelperSet<string>(Settings.Key.OneDriveUserId, value);
    }

    public static string OneDriveUserDisplayName
    {
      get
      {
        return Settings.Instance.HelperGet<string>(Settings.Key.OneDriveUserDisplayName, string.Empty);
      }
      set => Settings.Instance.HelperSet<string>(Settings.Key.OneDriveUserDisplayName, value);
    }

    public static string OneDriveUserAccountEmail
    {
      get
      {
        return Settings.Instance.HelperGet<string>(Settings.Key.OneDriveUserAccountEmail, string.Empty);
      }
      set => Settings.Instance.HelperSet<string>(Settings.Key.OneDriveUserAccountEmail, value);
    }

    public static string OneDriveUserAccountId
    {
      get => Settings.Instance.HelperGet<string>(Settings.Key.OneDriveUserAccountId, string.Empty);
      set => Settings.Instance.HelperSet<string>(Settings.Key.OneDriveUserAccountId, value);
    }

    public static string OneDriveUserAccountProviderId
    {
      get
      {
        return Settings.Instance.HelperGet<string>(Settings.Key.OneDriveUserAccountProviderId, "https://login.microsoft.com");
      }
      set => Settings.Instance.HelperSet<string>(Settings.Key.OneDriveUserAccountProviderId, value);
    }

    public static string OneDriveUserAccountProviderAuthority
    {
      get
      {
        return Settings.Instance.HelperGet<string>(Settings.Key.OneDriveUserAccountProviderAuthority, "consumers");
      }
      set
      {
        Settings.Instance.HelperSet<string>(Settings.Key.OneDriveUserAccountProviderAuthority, value);
      }
    }

    public static bool OneDriveUserReauthenticate
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.OneDriveUserReauthenticate, false);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.OneDriveUserReauthenticate, value);
    }

    public static string OneDriveWP10AuthInterfaceError
    {
      get
      {
        return Settings.Instance.HelperGet<string>(Settings.Key.OneDriveWP10AuthInterfaceError, string.Empty);
      }
      set
      {
        Settings.Instance.HelperSet<string>(Settings.Key.OneDriveWP10AuthInterfaceError, value);
      }
    }

    public static int OneDriveBackupFailedCount
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.OneDriveBackupFailedCount, 0);
      set => Settings.Instance.HelperSet<int>(Settings.Key.OneDriveBackupFailedCount, value);
    }

    public static int OneDriveBackupRequestCount
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.OneDriveBackupRequestCount, 0);
      set => Settings.Instance.HelperSet<int>(Settings.Key.OneDriveBackupRequestCount, value);
    }

    public static Dictionary<string, string> OneDriveLargeUploadUrls
    {
      get
      {
        List<string> ppsz = Utils.ParsePpsz(Settings.Instance.HelperGet<string>(Settings.Key.OneDriveLargeUploadUrls, ""));
        int count = ppsz.Count;
        Dictionary<string, string> driveLargeUploadUrls = new Dictionary<string, string>(count / 2);
        for (int index = 0; index < count; index += 2)
          driveLargeUploadUrls[ppsz[index]] = ppsz[index + 1];
        return driveLargeUploadUrls;
      }
      set
      {
        if (value == null || value.Count == 0)
        {
          Settings.Instance.DeleteKey(Settings.Key.OneDriveLargeUploadUrls);
        }
        else
        {
          string[] strs = new string[value.Count * 2];
          int index = 0;
          foreach (KeyValuePair<string, string> keyValuePair in value)
          {
            strs[index] = keyValuePair.Key;
            strs[index + 1] = keyValuePair.Value;
            index += 2;
          }
          Settings.Instance.HelperSet<string>(Settings.Key.OneDriveLargeUploadUrls, Utils.ToPpsz((IEnumerable<string>) strs));
        }
      }
    }

    public static DateTime? LastMediaSweepTime
    {
      get
      {
        return Settings.Instance.HelperGet<DateTime?>(Settings.Key.LastMediaSweepTime, new DateTime?());
      }
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.LastMediaSweepTime, value);
    }

    public static DateTime? UsyncBackoffUtc
    {
      get => Settings.Instance.HelperGet<DateTime?>(Settings.Key.UsyncBackoff, new DateTime?());
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.UsyncBackoff, value);
    }

    public static DateTime? NextUsyncStatusRefreshUtc
    {
      get
      {
        return Settings.Instance.HelperGet<DateTime?>(Settings.Key.NextUsyncStatusRefresh, new DateTime?());
      }
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.NextUsyncStatusRefresh, value);
    }

    public static DateTime? UsyncStatusBackoffUtc
    {
      get
      {
        return Settings.Instance.HelperGet<DateTime?>(Settings.Key.UsyncStatusBackoff, new DateTime?());
      }
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.UsyncStatusBackoff, value);
    }

    public static DateTime? NextUsyncPictureRefreshUtc
    {
      get
      {
        return Settings.Instance.HelperGet<DateTime?>(Settings.Key.NextUsyncPictureRefresh, new DateTime?());
      }
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.NextUsyncPictureRefresh, value);
    }

    public static DateTime? UsyncPictureBackoffUtc
    {
      get
      {
        return Settings.Instance.HelperGet<DateTime?>(Settings.Key.UsyncPictureBackoff, new DateTime?());
      }
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.UsyncPictureBackoff, value);
    }

    public static DateTime? NextUsyncFeatureRefreshUtc
    {
      get
      {
        return Settings.Instance.HelperGet<DateTime?>(Settings.Key.NextUsyncFeatureRefresh, new DateTime?());
      }
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.NextUsyncFeatureRefresh, value);
    }

    public static DateTime? UsyncFeatureBackoffUtc
    {
      get
      {
        return Settings.Instance.HelperGet<DateTime?>(Settings.Key.UsyncFeatureBackoff, new DateTime?());
      }
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.UsyncFeatureBackoff, value);
    }

    public static DateTime? NextUsyncBusinessRefreshUtc
    {
      get
      {
        return Settings.Instance.HelperGet<DateTime?>(Settings.Key.NextUsyncBusinessRefresh, new DateTime?());
      }
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.NextUsyncBusinessRefresh, value);
    }

    public static DateTime? UsyncBusinessBackoffUtc
    {
      get
      {
        return Settings.Instance.HelperGet<DateTime?>(Settings.Key.UsyncBusinessBackoff, new DateTime?());
      }
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.UsyncBusinessBackoff, value);
    }

    public static bool UsyncSidelist
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.UsyncSidelist, false);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.UsyncSidelist, value);
    }

    public static DateTime? UsyncSidelistBackoffUtc
    {
      get
      {
        return Settings.Instance.HelperGet<DateTime?>(Settings.Key.UsyncSidelistBackoff, new DateTime?());
      }
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.UsyncSidelistBackoff, value);
    }

    public static DateTime? NextUsyncSidelistRefreshUtc
    {
      get
      {
        return Settings.Instance.HelperGet<DateTime?>(Settings.Key.NextUsyncSidelistRefresh, new DateTime?());
      }
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.NextUsyncSidelistRefresh, value);
    }

    public static SqliteRepair.SqliteRepairState MessagesDbRepairState
    {
      get
      {
        return Settings.Instance.HelperGet<SqliteRepair.SqliteRepairState>(Settings.Key.MessagesDbRepairState, SqliteRepair.SqliteRepairState.Unstarted);
      }
      set
      {
        Settings.Instance.HelperSet<SqliteRepair.SqliteRepairState>(Settings.Key.MessagesDbRepairState, value);
      }
    }

    public static bool DebugMarkDbAsCorrupt
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.MarkDbAsCorrupt, false);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.MarkDbAsCorrupt, value);
    }

    public static bool TwoFactorAuthEnabled
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.TwoFactorAuthEnabled, false);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.TwoFactorAuthEnabled, value);
    }

    public static bool TwoFactorPromptReset
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.TwoFactorPromptReset, false);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.TwoFactorPromptReset, value);
    }

    public static string TwoFactorAuthEmail
    {
      get => Settings.Instance.HelperGet<string>(Settings.Key.TwoFactorEmailAddress, string.Empty);
      set => Settings.Instance.HelperSet<string>(Settings.Key.TwoFactorEmailAddress, value);
    }

    public static string TwoFactorAuthCodeLocal
    {
      get => Settings.Instance.HelperGet<string>(Settings.Key.TwoFactorAuthCodeLocal, string.Empty);
      set => Settings.Instance.HelperSet<string>(Settings.Key.TwoFactorAuthCodeLocal, value);
    }

    public static DateTime? TwoFactorNextPrompt
    {
      get
      {
        return Settings.Instance.HelperGet<DateTime?>(Settings.Key.TwoFactorNextPrompt, new DateTime?());
      }
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.TwoFactorNextPrompt, value);
    }

    public static string TwoFactorWipeToken
    {
      get => Settings.Instance.HelperGet<string>(Settings.Key.TwoFactorWipeToken, string.Empty);
      set => Settings.Instance.HelperSet<string>(Settings.Key.TwoFactorWipeToken, value);
    }

    public static string TwoFactorWipeType
    {
      get => Settings.Instance.HelperGet<string>(Settings.Key.TwoFactorWipeType, string.Empty);
      set => Settings.Instance.HelperSet<string>(Settings.Key.TwoFactorWipeType, value);
    }

    public static DateTime? TwoFactorEmailWipePollExpiryTime
    {
      get
      {
        return new DateTime?(Settings.Instance.HelperGet<DateTime>(Settings.Key.TwoFactorWipeExpiryTime, DateTime.Now));
      }
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.TwoFactorWipeExpiryTime, value);
    }

    public static int TwoFactorWipePollInterval
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.TwoFactorWipePollInterval, 5);
      set => Settings.Instance.HelperSet<int>(Settings.Key.TwoFactorWipePollInterval, value);
    }

    public static DateTime? TwoFactorWipeTimeUntilTokenValid
    {
      get
      {
        return Settings.Instance.HelperGet<DateTime?>(Settings.Key.TwoFactorWipeWait, new DateTime?());
      }
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.TwoFactorWipeWait, value);
    }

    public static DateTime? LastNtpCheck
    {
      get => Settings.Instance.HelperGet<DateTime?>(Settings.Key.LastNtpCheck, new DateTime?());
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.LastNtpCheck, value);
    }

    public static bool EmojiSearchEnabled
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.EmojiSearch, false);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.EmojiSearch, value);
    }

    public static int TimeSpentRecordOption
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.TimeSpentRecordOption, 0);
      set
      {
        Settings.Instance.HelperSet<int>(Settings.Key.TimeSpentRecordOption, value);
        NonDbSettings.TimeSpentRecordOption = value;
      }
    }

    public static int FieldStatsSendIntervalSecs
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.FieldStatsSendIntervalSecs, 0);
      set
      {
        Settings.Instance.HelperSet<int>(Settings.Key.FieldStatsSendIntervalSecs, value);
        NonDbSettings.FieldStatsSendIntervalSecs = value;
      }
    }

    public static bool LiveLocationEnabled
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.LiveLocationProp, false);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.LiveLocationProp, value);
    }

    public static byte[] LiveLocationData
    {
      get => Settings.Instance.HelperGet<byte[]>(Settings.Key.LiveLocationData, (byte[]) null);
      set => Settings.Instance.HelperSet(Settings.Key.LiveLocationData, value);
    }

    public static byte[] LiveLocationDeniers
    {
      get => Settings.Instance.HelperGet<byte[]>(Settings.Key.LiveLocationDeniers, (byte[]) null);
      set => Settings.Instance.HelperSet(Settings.Key.LiveLocationDeniers, value);
    }

    public static byte[] BackupKey
    {
      get => Settings.Instance.HelperGet<byte[]>(Settings.Key.BackupKey, (byte[]) null);
      set => Settings.Instance.HelperSet(Settings.Key.BackupKey, value);
    }

    public static DateTime? BackupKeySetUtc
    {
      get => Settings.Instance.HelperGet<DateTime?>(Settings.Key.BackupKeySetUtc, new DateTime?());
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.BackupKeySetUtc, value);
    }

    public static byte[] RestoreKeys
    {
      get => Settings.Instance.HelperGet<byte[]>(Settings.Key.RestoreKeys, (byte[]) null);
      set => Settings.Instance.HelperSet(Settings.Key.RestoreKeys, value);
    }

    public static int CipherTextPlaceholderShown
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.CipherTextPlaceholderShown, 0);
      set => Settings.Instance.HelperSet<int>(Settings.Key.CipherTextPlaceholderShown, value);
    }

    public static int TwoFactorPromptBackoff
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.TwoFactorPromptBackoff, 0);
      set => Settings.Instance.HelperSet<int>(Settings.Key.TwoFactorPromptBackoff, value);
    }

    public static DateTime? KeydBackoffUtc
    {
      get => Settings.Instance.HelperGet<DateTime?>(Settings.Key.KeydBackoffTime, new DateTime?());
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.KeydBackoffTime, value);
    }

    public static int KeydBackoffAttempt
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.KeydBackoffAttempt, 0);
      set => Settings.Instance.HelperSet<int>(Settings.Key.KeydBackoffAttempt, value);
    }

    public static bool UseFBCrashlogUploadObsolete
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.UseFBCrashlogUpload, false);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.UseFBCrashlogUpload, value);
    }

    public static byte[] OneDriveBackupStatus
    {
      get => Settings.Instance.HelperGet<byte[]>(Settings.Key.OneDriveBackupStatus, (byte[]) null);
      set => Settings.Instance.HelperSet(Settings.Key.OneDriveBackupStatus, value);
    }

    public static bool ContactArrayEnabled
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.ContactArrayEnabled, false);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.ContactArrayEnabled, value);
    }

    public static int LastPSAReceived
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.LastPSAReceived, 0);
      set => Settings.Instance.HelperSet<int>(Settings.Key.LastPSAReceived, value);
    }

    public static int CallStartDelay
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.CallStartDelay, 0);
      set => Settings.Instance.HelperSet<int>(Settings.Key.CallStartDelay, value);
    }

    public static bool RevokeFirstUseMessageDisplayed
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.RevokeFirstUseMessageDisplayed, false);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.RevokeFirstUseMessageDisplayed, value);
    }

    public static bool StickersEnabled
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.StickersEnabled, false);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.StickersEnabled, value);
    }

    public static bool StickerAnimationEnabled
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.StickerAnimationEnabled, false);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.StickerAnimationEnabled, value);
    }

    public static bool StickerPickerEnabled
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.StickerPickerEnabled, false);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.StickerPickerEnabled, value);
    }

    public static long LiveLocationSequenceNumber
    {
      get => Settings.Instance.HelperGet<long>(Settings.Key.LiveLocationSequenceNumber, 0L);
      set => Settings.Instance.HelperSet<long>(Settings.Key.LiveLocationSequenceNumber, value);
    }

    public static byte[] PaymentsSettings
    {
      get => Settings.Instance.HelperGet<byte[]>(Settings.Key.PaymentsSettings, (byte[]) null);
      set => Settings.Instance.HelperSet(Settings.Key.PaymentsSettings, value);
    }

    public static byte[] PaymentsWaAdminOverrides
    {
      get
      {
        return Settings.Instance.HelperGet<byte[]>(Settings.Key.PaymentsWaAdminOverrides, (byte[]) null);
      }
      set => Settings.Instance.HelperSet(Settings.Key.PaymentsWaAdminOverrides, value);
    }

    public static DateTime? ServerExpirationOverride
    {
      get
      {
        return Settings.Instance.HelperGet<DateTime?>(Settings.Key.ServerExpirationOverride, new DateTime?());
      }
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.ServerExpirationOverride, value);
    }

    public static byte[] EdgeRoutingInfo
    {
      get => Settings.Instance.HelperGet<byte[]>(Settings.Key.EdgeRoutingInfo, (byte[]) null);
      set => Settings.Instance.HelperSet(Settings.Key.EdgeRoutingInfo, value);
    }

    public static bool ChangeNumberNotifyEnabled
    {
      get
      {
        return Settings.Instance.HelperGet<bool>(Settings.Key.ChangeNumberNotifyEnabled, Settings.IsWaAdmin);
      }
      set => Settings.Instance.HelperSet<bool>(Settings.Key.ChangeNumberNotifyEnabled, value);
    }

    public static List<string> ChangeNumberNotifyJids
    {
      get
      {
        return !Settings.ChangeNumberNotifyEnabled ? new List<string>() : Utils.ParsePpsz(Settings.Instance.HelperGet<string>(Settings.Key.ChangeNumberNotifyJids, "") ?? "");
      }
      set
      {
        string str = "";
        if (value != null && value.Any<string>())
          str = Utils.ToPpsz((IEnumerable<string>) value);
        Settings.Instance.HelperSet<string>(Settings.Key.ChangeNumberNotifyJids, str);
      }
    }

    public static bool P2pPaymentEnabled
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.P2pPaymentEnabled, false);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.P2pPaymentEnabled, value);
    }

    public static long Mms4QueryBackoffUntilTimeUtcTicks
    {
      get => Settings.Instance.HelperGet<long>(Settings.Key.Mms4QueryBackoffUntilTimeUtcTicks, 0L);
      set
      {
        Settings.Instance.HelperSet<long>(Settings.Key.Mms4QueryBackoffUntilTimeUtcTicks, value);
      }
    }

    public static string Mms4CurrentRoute
    {
      get => Settings.Instance.HelperGet<string>(Settings.Key.Mms4CurrentRoute, (string) null);
      set => Settings.Instance.HelperSet<string>(Settings.Key.Mms4CurrentRoute, value);
    }

    public static string Mms4FibBackoffState
    {
      get => Settings.Instance.HelperGet<string>(Settings.Key.Mms4FibBackoffState, (string) null);
      set => Settings.Instance.HelperSet<string>(Settings.Key.Mms4FibBackoffState, value);
    }

    public static bool Mms4ServerImage
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.Mms4ServerImage, false);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.Mms4ServerImage, value);
    }

    public static bool Mms4ServerAudio
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.Mms4ServerAudio, false);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.Mms4ServerAudio, value);
    }

    public static bool Mms4ServerPtt
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.Mms4ServerPtt, false);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.Mms4ServerPtt, value);
    }

    public static bool MmsServerVideo
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.MmsServerVideo, false);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.MmsServerVideo, value);
    }

    public static bool MmsServerGif
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.MmsServerGif, false);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.MmsServerGif, value);
    }

    public static bool MmsServerDoc
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.MmsServerDoc, false);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.MmsServerDoc, value);
    }

    public static int GroupDescriptionLength
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.GroupDescriptionLength, 0);
      set => Settings.Instance.HelperSet<int>(Settings.Key.GroupDescriptionLength, value);
    }

    public static bool RestrictGroups
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.RestrictGroups, false);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.RestrictGroups, value);
    }

    public static int AnnouncementGroupSize
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.AnnouncementGroupSize, 0);
      set => Settings.Instance.HelperSet<int>(Settings.Key.AnnouncementGroupSize, value);
    }

    public static int AnnouncementGroupToggleTimeHours
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.AnnouncementGroupToggleTimeHours, 72);
      set => Settings.Instance.HelperSet<int>(Settings.Key.AnnouncementGroupToggleTimeHours, value);
    }

    public static bool GroupsV3
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.GroupsV3, false);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.GroupsV3, value);
    }

    public static DateTime? GdprTosAcceptedUtc
    {
      get
      {
        return Settings.Instance.HelperGet<DateTime?>(Settings.Key.GdprTosAcceptedUtc, new DateTime?());
      }
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.GdprTosAcceptedUtc, value);
    }

    public static DateTime? GdprTosStage1StartUtc
    {
      get
      {
        return Settings.Instance.HelperGet<DateTime?>(Settings.Key.GdprTosStage1StartUtc, new DateTime?());
      }
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.GdprTosStage1StartUtc, value);
    }

    public static DateTime? GdprTosStage2StartUtc
    {
      get
      {
        return Settings.Instance.HelperGet<DateTime?>(Settings.Key.GdprTosStage2StartUtc, new DateTime?());
      }
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.GdprTosStage2StartUtc, value);
    }

    public static DateTime? GdprTosStage3StartUtc
    {
      get
      {
        return Settings.Instance.HelperGet<DateTime?>(Settings.Key.GdprTosStage3StartUtc, new DateTime?());
      }
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.GdprTosStage3StartUtc, value);
    }

    public static DateTime? GdprTosLastShownUtc
    {
      get
      {
        return Settings.Instance.HelperGet<DateTime?>(Settings.Key.GdprTosLastShownUtc, new DateTime?());
      }
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.GdprTosLastShownUtc, value);
    }

    public static int GdprTosCurrentStage
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.GdprTosCurrentStage, 0);
      set => Settings.Instance.HelperSet<int>(Settings.Key.GdprTosCurrentStage, value);
    }

    public static DateTime? GdprTosUserDismissedUtc
    {
      get
      {
        return Settings.Instance.HelperGet<DateTime?>(Settings.Key.GdprTosUserDismissedUtc, new DateTime?());
      }
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.GdprTosUserDismissedUtc, value);
    }

    public static DateTime? GdprTosFirstSeenSecondPageUtc
    {
      get
      {
        return Settings.Instance.HelperGet<DateTime?>(Settings.Key.GdprTosFirstSeenSecondPageUtc, new DateTime?());
      }
      set
      {
        Settings.Instance.HelperSet<DateTime>(Settings.Key.GdprTosFirstSeenSecondPageUtc, value);
      }
    }

    public static string GdprTosServerProperty
    {
      get => Settings.Instance.HelperGet<string>(Settings.Key.GdprTosServerProperty, (string) null);
      set => Settings.Instance.HelperSet<string>(Settings.Key.GdprTosServerProperty, value);
    }

    public static int DevicesAccessPermissions
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.DevicesAccessPermissions, 0);
      set => Settings.Instance.HelperSet<int>(Settings.Key.DevicesAccessPermissions, value);
    }

    public static int ServerRequestedFibBackoffState
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.ServerRequestBackoffState, 0);
      set => Settings.Instance.HelperSet<int>(Settings.Key.ServerRequestBackoffState, value);
    }

    public static DateTime? LastShowCallRatingTimeUtc
    {
      get
      {
        return Settings.Instance.HelperGet<DateTime?>(Settings.Key.LastShowCallRatingTimeUtc, new DateTime?());
      }
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.LastShowCallRatingTimeUtc, value);
    }

    public static string CreateSupportLogIntervalHours
    {
      set => Settings.Instance.HelperSet<string>(Settings.Key.CreateSupportLogIntervalHours, value);
    }

    public static long ForwardedMsgUiTs
    {
      get => Settings.Instance.HelperGet<long>(Settings.Key.ForwardedMsgUiTs, 0L);
      set => Settings.Instance.HelperSet<long>(Settings.Key.ForwardedMsgUiTs, value);
    }

    public static byte[] LiveLocationSession
    {
      get => Settings.Instance.HelperGet<byte[]>(Settings.Key.LiveLocationSession, (byte[]) null);
      set => Settings.Instance.HelperSet(Settings.Key.LiveLocationSession, value);
    }

    public static byte[] LastReportedLocationInfo
    {
      get
      {
        return Settings.Instance.HelperGet<byte[]>(Settings.Key.LastReportedLocationInfo, (byte[]) null);
      }
      set => Settings.Instance.HelperSet(Settings.Key.LastReportedLocationInfo, value);
    }

    public static long CmcStartTs
    {
      get => Settings.Instance.HelperGet<long>(Settings.Key.CmcStartTs, -1L);
      set => Settings.Instance.HelperSet<long>(Settings.Key.CmcStartTs, value);
    }

    public static byte[] CmcPendingDetails
    {
      get => Settings.Instance.HelperGet<byte[]>(Settings.Key.CmcPendingDetails, (byte[]) null);
      set => Settings.Instance.HelperSet(Settings.Key.CmcPendingDetails, value);
    }

    public static byte[] CmcDailyDetails
    {
      get => Settings.Instance.HelperGet<byte[]>(Settings.Key.CmcDailyDetails, (byte[]) null);
      set => Settings.Instance.HelperSet(Settings.Key.CmcDailyDetails, value);
    }

    public static int LiveLocationSubscriptionDuration
    {
      get
      {
        return Settings.Instance.HelperGet<int>(Settings.Key.LiveLocationSubscriptionDuration, Constants.LiveLocationDefaultSubscriptionDurationSeconds);
      }
      set => Settings.Instance.HelperSet<int>(Settings.Key.LiveLocationSubscriptionDuration, value);
    }

    public static int MulticastLimitRestricted
    {
      get
      {
        return Settings.Instance.HelperGet<int>(Settings.Key.MulticastLimitRestricted, Constants.MaxMessageRecipients);
      }
      set => Settings.Instance.HelperSet<int>(Settings.Key.MulticastLimitRestricted, value);
    }

    public static bool MSUpdateLinkEnabled
    {
      get => Settings.Instance.HelperGet<bool>(Settings.Key.MSUpdateLinkEnabled, false);
      set => Settings.Instance.HelperSet<bool>(Settings.Key.MSUpdateLinkEnabled, value);
    }

    public static DateTime? BizChat2TierOneTimeSysMsgAdded
    {
      get
      {
        return Settings.Instance.HelperGet<DateTime?>(Settings.Key.BizChat2TierOneTimeSysMsgAdded, new DateTime?());
      }
      set
      {
        Settings.Instance.HelperSet<DateTime>(Settings.Key.BizChat2TierOneTimeSysMsgAdded, value);
      }
    }

    public static int MulticastLimitGlobal
    {
      get
      {
        return Settings.Instance.HelperGet<int>(Settings.Key.MulticastLimitGlobal, Constants.MaxMessageRecipients);
      }
      set => Settings.Instance.HelperSet<int>(Settings.Key.MulticastLimitGlobal, value);
    }

    public static int WebMsgMaxSizeKb
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.WebMsgMaxSizeKb, 100);
      set => Settings.Instance.HelperSet<int>(Settings.Key.WebMsgMaxSizeKb, value);
    }

    public static int KeepStackTrace
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.KeepStackTrace, 0);
      set
      {
        Settings.Instance.HelperSet<int>(Settings.Key.KeepStackTrace, value);
        NonDbSettings.KeepStackTrace = value;
      }
    }

    public static int InvalidJidSampleRate
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.InvalidJidSampleRate, 10);
      set => Settings.Instance.HelperSet<int>(Settings.Key.InvalidJidSampleRate, value);
    }

    public static int InvalidJidThrottleSeconds
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.InvalidJidThrottleSeconds, 900);
      set => Settings.Instance.HelperSet<int>(Settings.Key.InvalidJidThrottleSeconds, value);
    }

    public static DateTime? LastDeprecationNagTime
    {
      get
      {
        return Settings.Instance.HelperGet<DateTime?>(Settings.Key.LastDeprecationNagTime, new DateTime?());
      }
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.LastDeprecationNagTime, value);
    }

    public static DateTime? DeprecationDateActual
    {
      get
      {
        return new DateTime?(Settings.Instance.HelperGet<DateTime>(Settings.Key.DeprecationDateActual, Settings.DefaultDateActual));
      }
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.DeprecationDateActual, value);
    }

    public static DateTime? DeprecationDateOfficial
    {
      get
      {
        return new DateTime?(Settings.Instance.HelperGet<DateTime>(Settings.Key.DeprecationDateOfficial, Settings.DefaultDateOfficial));
      }
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.DeprecationDateOfficial, value);
    }

    public static DateTime? DeprecationDateMessaging
    {
      get
      {
        return new DateTime?(Settings.Instance.HelperGet<DateTime>(Settings.Key.DeprecationDateMessaging, Settings.DefaultDateMessaging));
      }
      set => Settings.Instance.HelperSet<DateTime>(Settings.Key.DeprecationDateMessaging, value);
    }

    public static int DeprecationFinalRelease
    {
      get => Settings.Instance.HelperGet<int>(Settings.Key.DeprecationFinalRelease, 1);
      set => Settings.Instance.HelperSet<int>(Settings.Key.DeprecationFinalRelease, value);
    }

    public static void Invalidate()
    {
      Log.l("settings", "invalidate");
      lock (Settings.settingsLock)
        Settings.settingsInstance = (Settings) null;
    }

    public static void Reset()
    {
      Log.l("settings", "reset");
      lock (Settings.settingsLock)
      {
        Settings.settingsInstance.settingsStorage.Reset();
        Settings.settingsInstance.settingsStorage.Save();
        Settings.settingsInstance = (Settings) null;
      }
    }

    public static bool TryGet<T>(Settings.Key k, out T v)
    {
      return Settings.Instance.TryGetImpl<T>(k, out v);
    }

    private bool TryGetImpl<T>(Settings.Key k, out T v)
    {
      lock (Settings.settingsLock)
        return this.settingsStorage.TryGet<T>(k, out v);
    }

    private T Lookup<T>(Settings.Key name, T defValue)
    {
      T obj = default (T);
      lock (Settings.settingsLock)
        return this.settingsStorage.Get<T>(name, defValue);
    }

    private void Set<T>(Settings.Key key, T val, bool bypassCache = false, bool notify = true)
    {
      Log.l("settings", "set {0}", (object) key.ToString());
      Settings.PerformWithWriteLock((Action) (() =>
      {
        this.settingsStorage.Set<T>(key, val, bypassCache);
        this.settingsStorage.Save();
      }));
      if (!notify)
        return;
      Settings.NotifyChanged(key);
    }

    public static void Delete(Settings.Key key) => Settings.Instance.DeleteKey(key);

    public void DeleteKey(Settings.Key key)
    {
      Log.l("settings", "delete {0}", (object) key.ToString());
      Settings.PerformWithWriteLock((Action) (() =>
      {
        this.settingsStorage.Delete(key);
        this.settingsStorage.Save();
        this.settingsCache[(int) key] = (object) null;
      }));
      Settings.NotifyChanged(key);
    }

    public static void DeleteMany(IEnumerable<Settings.Key> names)
    {
      Settings instance = Settings.Instance;
      Settings.Key[] keys = names.ToArray<Settings.Key>();
      Settings.PerformWithWriteLock((Action) (() =>
      {
        foreach (Settings.Key Key in keys)
        {
          Log.l("settings", "delete {0}", (object) Key.ToString());
          instance.settingsStorage.Delete(Key);
          instance.settingsCache[(int) Key] = (object) null;
        }
        instance.settingsStorage.Save();
      }));
      Settings.NotifyChanged(keys);
    }

    public enum Key
    {
      Invalid = -1, // 0xFFFFFFFF
      BBFR = 0,
      ChatID = 1,
      CountryCode = 2,
      EULAAcceptedUtc = 3,
      FirstRegistrationUIShown = 4,
      LastFullSyncUtc = 5,
      LastGroupsUpdatedUtc = 6,
      MediaAutoSaveEnabled = 7,
      MyJid = 8,
      NextChallenge = 9,
      PhoneNumber = 10, // 0x0000000A
      PhoneNumberVerificationRetryUtc = 11, // 0x0000000B
      PhoneNumberVerificationState = 12, // 0x0000000C
      PhoneNumberVerificationTimeoutUtc = 13, // 0x0000000D
      PushEnabled = 14, // 0x0000000E
      PushName = 15, // 0x0000000F
      NetworkPreference = 16, // 0x00000010
      LastGoogleMapsFailureUtc = 17, // 0x00000011
      MaxGroupParticipants = 18, // 0x00000012
      MaxGroupSubject = 19, // 0x00000013
      LastPropertiesQueryUtc = 20, // 0x00000014
      NextChallenge2 = 21, // 0x00000015
      LastGoodPortIndex = 22, // 0x00000016
      PreviewEnabled = 23, // 0x00000017
      LocalTileCount = 24, // 0x00000018
      SuccessfulLoginUtc = 25, // 0x00000019
      RegV2Password = 26, // 0x0000001A
      CodeLength = 27, // 0x0000001B
      RotatedPassword = 28, // 0x0000001C
      RecoveryTokenSet = 29, // 0x0000001D
      RegV2LegacyWP8 = 30, // 0x0000001E
      MaxBroadcastRecipients = 31, // 0x0000001F
      EnterKeyIsSend = 32, // 0x00000020
      PttEnabled = 33, // 0x00000021
      PSTNCallWarningShown = 34, // 0x00000022
      LastEmojiTab = 35, // 0x00000023
      LastEmojiPagePositions = 36, // 0x00000024
      LastBatterySaverAlertedUtc = 37, // 0x00000025
      DefaultAudioEndpoint = 38, // 0x00000026
      LastChannelReopenUtc = 39, // 0x00000027
      LastPushUriHash = 40, // 0x00000028
      PushUriCount = 41, // 0x00000029
      VoipPushUriCount = 42, // 0x0000002A
      LoginFailed = 43, // 0x0000002B
      VoipLastExitReason = 44, // 0x0000002C
      VoipExceptionMessage = 45, // 0x0000002D
      ServicePrice = 46, // 0x0000002E
      AccountExpiration = 47, // 0x0000002F
      DbCorrupt = 48, // 0x00000030
      CodeEntryWaitToRetryUtc = 49, // 0x00000031
      LockOrientationInPortrait = 50, // 0x00000032
      NextFullSyncUtc = 51, // 0x00000033
      SyncHistory = 52, // 0x00000034
      SyncBackoff = 53, // 0x00000035
      MaxMediaSize = 54, // 0x00000036
      ImageMaxEdge = 55, // 0x00000037
      JpegQuality = 56, // 0x00000038
      SystemFontSize = 57, // 0x00000039
      TrackWorkQueueDateTimeUtc = 58, // 0x0000003A
      TrackDeadlockDateTimeUtc = 59, // 0x0000003B
      QrToken = 60, // 0x0000003C
      OldChatID = 61, // 0x0000003D
      OldPhoneNumber = 62, // 0x0000003E
      OldCountryCode = 63, // 0x0000003F
      LastKnownServerTimeUtc = 64, // 0x00000040
      AutoDownloadSettingImage = 65, // 0x00000041
      AutoDownloadSettingAudio = 66, // 0x00000042
      AutoDownloadSettingVideo = 67, // 0x00000043
      QrEnabled = 68, // 0x00000044
      IndividualTone = 69, // 0x00000045
      GroupTone = 70, // 0x00000046
      StatusVisibility = 71, // 0x00000047
      LastSeenVisibility = 72, // 0x00000048
      ProfilePhotoVisibility = 73, // 0x00000049
      LastPrivacyCheckUtc = 74, // 0x0000004A
      LastBlockListCheckUtc = 75, // 0x0000004B
      LastSeenVisibilityInDoubt = 76, // 0x0000004C
      StatusVisibilityInDoubt = 77, // 0x0000004D
      ProfilePhotoVisibilityInDoubt = 78, // 0x0000004E
      MaxListRecipients = 79, // 0x0000004F
      ShouldQueryBroadcastLists = 80, // 0x00000050
      ScreenRenderWidth = 81, // 0x00000051
      ScreenRenderHeight = 82, // 0x00000052
      IsGlobalWallpaperSet = 83, // 0x00000053
      IsUpdateAvailable = 84, // 0x00000054
      LastCheckForUpdatesUtc = 85, // 0x00000055
      ScreenResolution = 86, // 0x00000056
      PrereleaseOsNagged = 87, // 0x00000057
      ImageMaxKbytes = 88, // 0x00000058
      RegTimerToastShown = 89, // 0x00000059
      RegProgressbarStartUtc = 90, // 0x0000005A
      MapCartographicModeRoad = 91, // 0x0000005B
      LastKnownLatitude = 92, // 0x0000005C
      LastKnownLongitude = 93, // 0x0000005D
      SuppressRestoreFromBackupAtReg = 94, // 0x0000005E
      ReadReceiptTimestamp = 95, // 0x0000005F
      LoginFailedReason = 96, // 0x00000060
      LoginFailedExpirationUtc = 97, // 0x00000061
      LoginFailedRetryUtc = 98, // 0x00000062
      LoginFailedExpirationTotalSeconds = 99, // 0x00000063
      Jitter = 100, // 0x00000064
      LastWarningTime = 101, // 0x00000065
      WAAddressBookSize = 102, // 0x00000066
      NumberOfGroups = 103, // 0x00000067
      LastDailyStatsCollectedUtc = 104, // 0x00000068
      LastWeeklyStatsCollectedUtc = 105, // 0x00000069
      LastMonthlyStatsCollectUtc = 106, // 0x0000006A
      LastFieldStatsSentToServer = 107, // 0x0000006B
      LastFieldStatsSentToServerBA = 108, // 0x0000006C
      GroupsV2 = 109, // 0x0000006D
      ForceServerPropsReload = 110, // 0x0000006E
      GlobalWallpaper = 111, // 0x0000006F
      NextChatSortKey = 112, // 0x00000070
      QrBlob = 113, // 0x00000071
      ServerPropsVersion = 114, // 0x00000072
      LastLocalServerTimeDiff = 115, // 0x00000073
      WnsRegistered = 116, // 0x00000074
      BetaPushBroken = 117, // 0x00000075
      RemovedJids = 118, // 0x00000076
      EnableReadReceipts = 119, // 0x00000077
      LastEnableReadReceiptsTimeUtc = 120, // 0x00000078
      VoipEnabled = 121, // 0x00000079
      E2EMessageWaitPending = 122, // 0x0000007A
      E2EPlainTextDisabled = 123, // 0x0000007B
      E2EPlainTextReEnableThreshold = 124, // 0x0000007C
      IsWaAdmin = 125, // 0x0000007D
      LastSeenMissedCallTimeUtc = 126, // 0x0000007E
      EnableInAppNotificationToast = 127, // 0x0000007F
      EnableInAppNotificationSound = 128, // 0x00000080
      EnableInAppNotificationVibrate = 129, // 0x00000081
      EnableGroupAlerts = 130, // 0x00000082
      EnableIndividualAlerts = 131, // 0x00000083
      LowBandwidthVoip = 132, // 0x00000084
      E2EGroups = 133, // 0x00000085
      DanglingAcks = 134, // 0x00000086
      QrActionsBlob = 135, // 0x00000087
      VoipRingtone = 136, // 0x00000088
      StatusesBlob = 137, // 0x00000089
      ContactsChecksum = 138, // 0x0000008A
      TosUpdateStageStartUtc = 139, // 0x0000008B
      TosUpdateStages = 140, // 0x0000008C
      TosUpdateAccepted = 141, // 0x0000008D
      TosUpdateCurrentStage = 142, // 0x0000008E
      LocationProvider = 143, // 0x0000008F
      OpusPttEnabled = 144, // 0x00000090
      E2EImages = 145, // 0x00000091
      E2EContact = 146, // 0x00000092
      E2ELocation = 147, // 0x00000093
      E2EBroadcast = 148, // 0x00000094
      E2EAudio = 149, // 0x00000095
      E2EVerificationEnabled = 150, // 0x00000096
      E2EVerificationCapable = 151, // 0x00000097
      AxolotlRegistrationRetries = 152, // 0x00000098
      TosUpdateOptOut = 153, // 0x00000099
      TosUpdateLastSeenUtc = 154, // 0x0000009A
      ChatTileExists = 155, // 0x0000009B
      E2EVoip = 156, // 0x0000009C
      LastContactException = 157, // 0x0000009D
      UserFlashSetting = 158, // 0x0000009E
      UserCameraTypeSetting = 159, // 0x0000009F
      ShowPushNameScreen = 160, // 0x000000A0
      ClientStaticPrivateKey = 161, // 0x000000A1
      ClientStaticPublicKey = 162, // 0x000000A2
      ServerStaticPublicKey = 163, // 0x000000A3
      LoginFailedReasonCode = 164, // 0x000000A4
      MaxVideoEdge = 165, // 0x000000A5
      FacebookPhoneId = 166, // 0x000000A6
      AutoDownloadSettingDocument = 167, // 0x000000A7
      SupportedDocumentTypes = 168, // 0x000000A8
      MaxFileSize = 169, // 0x000000A9
      E2EEncryptedAnnouncementSent = 170, // 0x000000AA
      SaveIncomingMedia = 171, // 0x000000AB
      MaxAutodownloadSize = 172, // 0x000000AC
      DisablePushOnDebug = 173, // 0x000000AD
      EnableReadReceiptInDoubt = 174, // 0x000000AE
      LastSignedPreKeySent = 175, // 0x000000AF
      OneDriveBackupFrequency = 176, // 0x000000B0
      OneDriveBackupNetwork = 177, // 0x000000B1
      OneDriveIncludeVideos = 178, // 0x000000B2
      VoipVideo = 179, // 0x000000B3
      EverstoreBackoffTime = 180, // 0x000000B4
      EverstoreBackoffAttempt = 181, // 0x000000B5
      UseEverstoreForProfilePictures = 182, // 0x000000B6
      UseUsyncProtocol = 183, // 0x000000B7
      LastMediaSweepTimeObsolete = 184, // 0x000000B8
      GroupInviteLinksOn = 185, // 0x000000B9
      OneDriveUserId = 186, // 0x000000BA
      OneDriveUserDisplayName = 187, // 0x000000BB
      OneDriveUserReauthenticate = 188, // 0x000000BC
      UsyncBackoff = 189, // 0x000000BD
      UsyncStatusBackoff = 190, // 0x000000BE
      NextUsyncStatusRefresh = 191, // 0x000000BF
      UsyncPictureBackoff = 192, // 0x000000C0
      NextUsyncPictureRefresh = 193, // 0x000000C1
      UsyncFeatureBackoff = 194, // 0x000000C2
      NextUsyncFeatureRefresh = 195, // 0x000000C3
      MessagesDbRepairState = 196, // 0x000000C4
      MarkDbAsCorrupt = 197, // 0x000000C5
      LastNtpCheck = 198, // 0x000000C6
      PhoneNumberAccountCreationType = 199, // 0x000000C7
      MaxPreKeyBatchSize = 200, // 0x000000C8
      GifSending = 201, // 0x000000C9
      EnableMentionsSending = 202, // 0x000000CA
      E2EVerificationCleanup = 203, // 0x000000CB
      RegistrationSMSCode = 204, // 0x000000CC
      TwoFactorEmailAddress = 205, // 0x000000CD
      TwoFactorNextPrompt = 206, // 0x000000CE
      TwoFactorPromptReset = 207, // 0x000000CF
      TwoFactorAuthEnabled = 208, // 0x000000D0
      TwoFactorAuthAllowed = 209, // 0x000000D1
      VideoCallPreviewLocation = 210, // 0x000000D2
      OneDriveBackupFailedCount = 211, // 0x000000D3
      OneDriveBackupRequestCount = 212, // 0x000000D4
      DeprecationDetails = 213, // 0x000000D5
      OneDriveLargeUploadUrls = 214, // 0x000000D6
      TwoFactorAuthCodeLocal = 215, // 0x000000D7
      TwoFactorWipeToken = 216, // 0x000000D8
      TwoFactorWipeType = 217, // 0x000000D9
      TwoFactorWipeExpiryTime = 218, // 0x000000DA
      TwoFactorWipePollInterval = 219, // 0x000000DB
      OneDriveUserAccountEmail = 220, // 0x000000DC
      BackupKey = 221, // 0x000000DD
      RestoreKeys = 222, // 0x000000DE
      OneDriveUserAccountId = 223, // 0x000000DF
      OneDriveUserAccountProviderId = 224, // 0x000000E0
      OneDriveUserAccountProviderAuthority = 225, // 0x000000E1
      CipherTextPlaceholderShown = 226, // 0x000000E2
      TwoFactorPromptBackoff = 227, // 0x000000E3
      KeydBackoffTime = 228, // 0x000000E4
      KeydBackoffAttempt = 229, // 0x000000E5
      TwoFactorWipeWait = 230, // 0x000000E6
      StatusV3Setting = 231, // 0x000000E7
      VacationModeEnabled = 232, // 0x000000E8
      UseFBCrashlogUpload = 233, // 0x000000E9
      OneDriveBackupStatus = 234, // 0x000000EA
      StatusRecipientsStateDirty = 235, // 0x000000EB
      StatusV3PrivacySetting = 236, // 0x000000EC
      GifSearchProvider = 237, // 0x000000ED
      OneDriveRestoreNetwork = 238, // 0x000000EE
      LastMediaSweepTime = 239, // 0x000000EF
      ContactArrayEnabled = 240, // 0x000000F0
      OneDriveWP10AuthInterfaceError = 241, // 0x000000F1
      LastPSAReceived = 242, // 0x000000F2
      StatusImageMaxEdge = 243, // 0x000000F3
      StatusImageQuality = 244, // 0x000000F4
      IsStatusPSAUnseen = 245, // 0x000000F5
      GifSearch = 246, // 0x000000F6
      StatusVideoMaxBitrate = 247, // 0x000000F7
      StatusVideoMaxDuration = 248, // 0x000000F8
      ShowStatusTabTooltipBanner = 249, // 0x000000F9
      StatusV3Override = 250, // 0x000000FA
      CallStartDelay = 251, // 0x000000FB
      LastSeenStatusListTimeUtc = 252, // 0x000000FC
      ShowStatusRecipientPickerTooltip = 253, // 0x000000FD
      WaAdminForceGifProvider = 254, // 0x000000FE
      ShowStatusPresendTooltip = 255, // 0x000000FF
      UseNewFaqUrl = 256, // 0x00000100
      ReviveStatusClassic = 257, // 0x00000101
      MessageRevokeEnabled = 258, // 0x00000102
      ChangeNumberNotifyTarget = 259, // 0x00000103
      PaymentsSettings = 260, // 0x00000104
      ServerExpirationOverride = 261, // 0x00000105
      EdgeRoutingInfo = 262, // 0x00000106
      ChangeNumberNotifyEnabled = 263, // 0x00000107
      ChangeNumberNotifyJids = 264, // 0x00000108
      LiveLocationProp = 265, // 0x00000109
      LiveLocationData = 266, // 0x0000010A
      LiveLocationDeniers = 267, // 0x0000010B
      P2pPaymentEnabled = 268, // 0x0000010C
      VideoPrefetchBytes = 269, // 0x0000010D
      PaymentsWaAdminOverrides = 270, // 0x0000010E
      ChatDnsDomain = 271, // 0x0000010F
      Mms4QueryBackoffUntilTimeUtcTicks = 272, // 0x00000110
      Mms4CurrentRoute = 273, // 0x00000111
      Mms4FibBackoffState = 274, // 0x00000112
      Mms4ServerImage = 275, // 0x00000113
      Mms4ServerAudio = 276, // 0x00000114
      Mms4ServerPtt = 277, // 0x00000115
      MmsServerVideo = 278, // 0x00000116
      MmsServerGif = 279, // 0x00000117
      MmsServerDoc = 280, // 0x00000118
      VideoMaxBitrate = 281, // 0x00000119
      RegistrationCompleteUtc = 282, // 0x0000011A
      EmojiSearch = 283, // 0x0000011B
      TimeSpentRecordOption = 284, // 0x0000011C
      GroupDescriptionLength = 285, // 0x0000011D
      FieldStatsSendIntervalSecs = 286, // 0x0000011E
      UsyncBusinessBackoff = 287, // 0x0000011F
      NextUsyncBusinessRefresh = 288, // 0x00000120
      RevokeFirstUseMessageDisplayed = 289, // 0x00000121
      RestrictGroups = 290, // 0x00000122
      AnnouncementGroupSize = 291, // 0x00000123
      GroupsV3 = 292, // 0x00000124
      UsyncSidelist = 293, // 0x00000125
      UsyncSidelistBackoff = 294, // 0x00000126
      NextUsyncSidelistRefresh = 295, // 0x00000127
      AnnouncementGroupToggleTimeHours = 296, // 0x00000128
      DevicesAccessPermissions = 297, // 0x00000129
      AudioVideoSwitchEnabled = 298, // 0x0000012A
      StickersEnabled = 299, // 0x0000012B
      StickerAnimationEnabled = 300, // 0x0000012C
      LoginFailedUpgradeRequiredVersion = 301, // 0x0000012D
      ServerRequestBackoffState = 302, // 0x0000012E
      LiveLocationSequenceNumber = 303, // 0x0000012F
      GdprTosLastShownUtc = 304, // 0x00000130
      GdprTosCurrentStage = 305, // 0x00000131
      GdprTosStage1StartUtc = 306, // 0x00000132
      GdprTosAcceptedUtc = 307, // 0x00000133
      GdprReportState = 308, // 0x00000134
      GdprReportReadyTimeUtc = 309, // 0x00000135
      GdprReportCreationTimeUtc = 310, // 0x00000136
      GdprReportFilepath = 311, // 0x00000137
      GdprReportInfo = 312, // 0x00000138
      GdprReportEnabled = 313, // 0x00000139
      GdprTosServerProperty = 314, // 0x0000013A
      GdprTosFirstSeenSecondPageUtc = 315, // 0x0000013B
      UploadRandomBytes = 316, // 0x0000013C
      LiveLocationIsNewUser = 317, // 0x0000013D
      LastShowCallRatingTimeUtc = 318, // 0x0000013E
      GdprReportExpirationTimeUtc = 319, // 0x0000013F
      BackupKeySetUtc = 320, // 0x00000140
      GdprTosStage2StartUtc = 321, // 0x00000141
      GdprTosStage3StartUtc = 322, // 0x00000142
      IsEEA = 323, // 0x00000143
      GdprTosUserDismissedUtc = 324, // 0x00000144
      CreateSupportLogIntervalHours = 325, // 0x00000145
      ForwardedMsgUi = 326, // 0x00000146
      LastAddressbookSize = 327, // 0x00000147
      LiveLocationSession = 328, // 0x00000148
      LastReportedLocationInfo = 329, // 0x00000149
      ForwardedMsgUiTs = 330, // 0x0000014A
      CmcStartTs = 331, // 0x0000014B
      CmcPendingDetails = 332, // 0x0000014C
      CmcDailyDetails = 333, // 0x0000014D
      LiveLocationSubscriptionDuration = 334, // 0x0000014E
      StickerPickerEnabled = 335, // 0x0000014F
      MulticastLimitRestricted = 336, // 0x00000150
      MSUpdateLinkEnabled = 337, // 0x00000151
      WaAdminMemoryLeakDetection = 338, // 0x00000152
      FieldStatsBeaconChance = 339, // 0x00000153
      BizChat2TierOneTimeSysMsgAdded = 340, // 0x00000154
      MulticastLimitGlobal = 341, // 0x00000155
      WebMsgMaxSizeKb = 342, // 0x00000156
      KeepStackTrace = 343, // 0x00000157
      InvalidJidSampleRate = 344, // 0x00000158
      InvalidJidThrottleSeconds = 345, // 0x00000159
      LastDeprecationNagTime = 346, // 0x0000015A
      DeprecationDateOfficial = 347, // 0x0000015B
      DeprecationDateActual = 348, // 0x0000015C
      DeprecationDateMessaging = 349, // 0x0000015D
      DeprecationFinalRelease = 350, // 0x0000015E
    }

    private class KeyObserverList
    {
      private object @lock = new object();
      private List<IObserver<Settings.Key>> observers = new List<IObserver<Settings.Key>>();

      public void OnNext(Settings.Key k)
      {
        lock (this.@lock)
        {
          foreach (IObserver<Settings.Key> observer in this.observers.ToArray())
          {
            try
            {
              observer.OnNext(k);
            }
            catch (Exception ex)
            {
              observer.OnError(ex);
            }
          }
        }
      }

      public void AddObserver(IObserver<Settings.Key> obs)
      {
        lock (this.@lock)
          this.observers.Add(obs);
      }

      public void RemoveObserver(IObserver<Settings.Key> obs)
      {
        lock (this.@lock)
          this.observers.Remove(obs);
      }
    }

    public enum ScreenResolutionKind
    {
      Undefined,
      WVGA,
      WXGA,
      HD720p,
      HD1080p,
    }
  }
}
