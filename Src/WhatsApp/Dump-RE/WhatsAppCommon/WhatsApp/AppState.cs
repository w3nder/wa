// Decompiled with JetBrains decompiler
// Type: WhatsApp.AppState
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.BackgroundTransfer;
using Microsoft.Phone.Info;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Scheduler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Xml.Linq;
using WhatsAppNative;
using Windows.Devices.Enumeration;
using Windows.Phone.Devices.Power;
using Windows.Phone.System.Power;
using Windows.System;

#nullable disable
namespace WhatsApp
{
  public class AppState
  {
    public const string LogHeader = "app state";
    public static AppState.Client ClientInstance;
    public static EventWaitHandle ObscuredEvent = new EventWaitHandle(false, EventResetMode.ManualReset, Constants.ObscuredEventName);
    public static EventWaitHandle VoipEvent = new EventWaitHandle(false, EventResetMode.AutoReset, Constants.VoipEventName);
    public static MutexWithWatchdog BgMutex = new MutexWithWatchdog("WhatsAppBgMutex", false);
    public static bool IsBackgroundAgent;
    public static TimeSpan PingTimeout = Constants.ForegroundPingTimeout;
    private static WorkQueue worker = (WorkQueue) null;
    private static object workerLock = new object();
    private static WorkQueue imageWorker = (WorkQueue) null;
    public static Subject<Unit> DbResetSubject = new Subject<Unit>();
    public static TreeInForestSubject<Unit> ConnectionResetSubject = new TreeInForestSubject<Unit>();
    private static string cachedVersion = (string) null;
    private static string appGuid = (string) null;
    private static string appInstallDir_ = (string) null;
    private const int MAX_AUTO_DOWNLOAD = 32;
    public static QrPersistentAction QrPersistentAction = QrPersistentAction.Instance;
    private static bool uiUpdatesMuted = false;
    public static RefCountAction MuteUIUpdates = new RefCountAction((Action) (() => AppState.uiUpdatesMuted = true), (Action) (() => AppState.uiUpdatesMuted = false));
    private static object overrideLock = new object();
    private static bool? islowMem_ = new bool?();
    private static bool? isDecentMem_ = new bool?();
    private static object convoOpenLock = new object();
    private static LinkedList<string> openConvos = new LinkedList<string>();
    private static volatile bool isInvalidated = false;
    private static bool checkedRegLog = false;
    private static bool? useWindowsNotificationService;
    private static DateTime? buildTimeUtc = new DateTime?();
    private const int skewThresholdSeconds = 43200;

    public static WorkQueue Worker
    {
      get
      {
        return Utils.LazyInit<WorkQueue>(ref AppState.worker, (Func<WorkQueue>) (() => new WorkQueue(identifierString: nameof (Worker))), AppState.workerLock);
      }
    }

    public static WorkQueue ImageWorker
    {
      get
      {
        return Utils.LazyInit<WorkQueue>(ref AppState.imageWorker, (Func<WorkQueue>) (() => new WorkQueue(identifierString: nameof (ImageWorker))), AppState.workerLock);
      }
    }

    public static void ProcessResetActions()
    {
      PushSystem.Instance.OnAppReset();
      AppState.DbResetSubject.OnNext(new Unit());
    }

    public static FunXMPP.Connection GetConnection() => AppState.ClientInstance?.GetConnection();

    public static void InvokeWhenConnected(Action<FunXMPP.Connection> a)
    {
      FunXMPP.Connection conn = AppState.GetConnection();
      if (conn == null)
        return;
      conn.InvokeWhenConnected((Action) (() => a(conn)));
    }

    public static string GetAppVersion()
    {
      if (AppState.cachedVersion == null)
      {
        using (Stream stream = Application.GetResourceStream(new Uri("git-info", UriKind.Relative)).Stream)
        {
          MemoryStream destination = new MemoryStream();
          stream.CopyTo((Stream) destination);
          string str = Encoding.UTF8.GetString(destination.GetBuffer(), 1, (int) destination.Length - 1).Trim();
          int length = str.IndexOf('-');
          AppState.cachedVersion = length < 0 ? str : str.Substring(0, length);
        }
      }
      return AppState.cachedVersion;
    }

    public static string GetUserAgent()
    {
      string str1 = AppState.OSVersion.ToString();
      string str2 = string.Format("{0}-{1}-H{2}", (object) DeviceStatus.DeviceManufacturer, (object) DeviceStatus.DeviceName, (object) DeviceStatus.DeviceHardwareVersion).Replace(' ', '_');
      return string.Format("WhatsApp/{0} WP7{1}/{2} Device/{3}", (object) AppState.GetAppVersion(), AppState.IsBackgroundAgent ? (object) "B" : (object) "", (object) str1, (object) str2);
    }

    public static string GetUrlPreviewUserAgent()
    {
      return string.Format("WhatsApp/{0} W", (object) AppState.GetAppVersion());
    }

    public static string GetAssemblyName() => AppState.GetAssemblyName((Assembly) null);

    public static string GetAssemblyName(Assembly asm)
    {
      if (asm == null)
        asm = Assembly.GetExecutingAssembly();
      string assemblyName = asm.FullName;
      int length = assemblyName.IndexOf(',');
      if (length > 0)
        assemblyName = assemblyName.Substring(0, length);
      return assemblyName;
    }

    public static string GetAppGuid()
    {
      if (AppState.appGuid == null)
        AppState.appGuid = new string(XDocument.Load("WMAppManifest.xml").Root.Element((XName) "App").Attribute((XName) "ProductID").Value.Where<char>((Func<char, bool>) (c => c != '{' && c != '}')).ToArray<char>());
      return AppState.appGuid;
    }

    public static IEnumerable<string> GetSupportedCultures()
    {
      string assemblyName = AppState.GetAssemblyName();
      yield return "en-US";
      string fileSought = string.Format("/{0}.resources.dll", (object) assemblyName);
      XElement root = XDocument.Load("AppManifest.xaml").Root;
      string str1 = "{http://schemas.microsoft.com/client/2007/deployment}";
      XName name = (XName) (str1 + "Deployment.Parts");
      foreach (XElement element in root.Element(name).Elements((XName) (str1 + "AssemblyPart")))
      {
        XAttribute xattribute = element.Attribute((XName) "Source");
        if (xattribute != null && xattribute.Value != null)
        {
          string str2 = xattribute.Value;
          int length = str2.IndexOf('/');
          if (length > 0 && str2.EndsWith(fileSought, StringComparison.InvariantCultureIgnoreCase))
            yield return str2.Substring(0, length);
        }
      }
    }

    public static void OnDatabaseCorruption()
    {
      AppState.Client clientInstance = AppState.ClientInstance;
      if (clientInstance == null)
        return;
      clientInstance.ResetConnection();
      clientInstance.OnDatabaseCorruption();
    }

    public static void OnLoginException(FunXMPP.LoginFailureException ex)
    {
      AppState.ClientInstance?.OnLoginException(ex);
    }

    public static AppResources GetAppResources() => new AppResources();

    public static string GetString(string key) => AppResources.ResourceManager.GetString(key);

    public static string GetNeutralString(string key)
    {
      return key == null ? (string) null : AppResources.ResourceManager.GetString(key, new CultureInfo("en-US"));
    }

    public static void UpdateLogConnectionInfo()
    {
      DeviceNetworkInformation.NetworkAvailabilityChanged += (EventHandler<NetworkNotificationEventArgs>) ((sender, args) =>
      {
        if (args.NotificationType == NetworkNotificationType.InterfaceDisconnected)
          NativeInterfaces.Misc.SetLogUserConnection(ConnectionType.Disconnected);
        else
          NativeInterfaces.Misc.SetLogUserConnection(AppState.GetUserConnectionType());
        FunRunner.BackoffEvent.Set();
      });
    }

    public static ConnectionType GetUserConnectionType()
    {
      if (!DeviceNetworkInformation.IsNetworkAvailable)
        return ConnectionType.Disconnected;
      if (NetworkStateMonitor.IsWifiDataConnected())
        return ConnectionType.Wifi;
      if (NetworkStateMonitor.Is2GConnection())
        return ConnectionType.Cellular_2G;
      return NetworkStateMonitor.Is3GOrBetter() ? ConnectionType.Cellular_3G : ConnectionType.Unknown;
    }

    public static string FormatPhoneNumber(string number)
    {
      if (!number.Where<char>((Func<char, bool>) (c => char.IsDigit(c))).Any<char>())
        return number;
      try
      {
        string str = PhoneNumberFormatter.FormatInternationalNumber(number);
        if (!string.IsNullOrEmpty(str))
          return str;
      }
      catch (Exception ex)
      {
        string context = string.Format("Phone number [{0}] threw exception", (object) number);
        Log.SendCrashLog(ex, context);
      }
      Log.d(nameof (FormatPhoneNumber), "Failure to convert number: {0}", (object) number);
      return "+" + number;
    }

    public static string AppInstallDir
    {
      get
      {
        return AppState.appInstallDir_ ?? (AppState.appInstallDir_ = NativeInterfaces.Misc.GetAppInstallDir());
      }
    }

    public static Stream OpenFromXAP(string path)
    {
      using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
        return nativeMediaStorage.OpenFile(AppState.AppInstallDir + "\\" + path, FileMode.Open, FileAccess.Read);
    }

    public static void SendPushConfig(
      Uri uri,
      IEnumerable<FunXMPP.Connection.GroupSetting> groupSettings,
      Action onCompleted = null,
      Action<int> onError = null)
    {
      Log.d("app state", "send push config | uri:{0}", (object) uri);
      Dictionary<string, string> clientConfig = PushSystem.Instance.ClientConfig;
      Action oldOnComplete = onCompleted;
      onCompleted = (Action) (() =>
      {
        if (oldOnComplete != null)
          oldOnComplete();
        PushSystem.Instance.OnPushRegistered();
      });
      AppState.GetConnection().SendClientConfig(uri, clientConfig, groupSettings, onCompleted, onError);
    }

    public static void SendMessage(FunXMPP.Connection conn, Message msg, bool mms_retry = false)
    {
      if (conn == null || msg == null)
        return;
      if (msg.Status.GetOverrideWeight() >= FunXMPP.FMessage.Status.ReceivedByServer.GetOverrideWeight())
        Log.l("app state", "send msg | skip | already sent");
      else if (!msg.ShouldSend())
      {
        Log.d("app state", "send msg | skip | shouldn't send");
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          msg.Status = FunXMPP.FMessage.Status.Error;
          db.SubmitChanges();
        }));
      }
      else if (JidHelper.IsSelfJid(msg.KeyRemoteJid))
      {
        Log.d("app state", "send msg | skip | shouldn't send to self");
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          msg.Status = FunXMPP.FMessage.Status.ReadByTarget;
          db.SubmitChanges();
        }));
      }
      else if (!JidChecker.CheckJidProtocolString(msg.KeyRemoteJid))
      {
        Log.l("app state", "send msg | skip | shouldn't send to invalid jid {0}", (object) msg.KeyRemoteJid);
        JidChecker.SendJidErrorClb("Sending to invalid jid", msg.KeyRemoteJid);
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          msg.Status = FunXMPP.FMessage.Status.ReadByTarget;
          db.SubmitChanges();
        }));
      }
      else if (msg.IsStatusWithNoSentRecipient())
      {
        Log.l("statusv3", "zero recipients | skip sending");
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          msg.Status = FunXMPP.FMessage.Status.ReadByTarget;
          db.SubmitChanges();
        }));
      }
      else
      {
        Action retryAction = (Action) (() => AppState.SendMessage(conn, msg, mms_retry));
        FunXMPP.FMessage fmsg = conn.Encryption.EncryptMessage(msg, retryAction);
        if (fmsg == null)
          return;
        fmsg.mms_retry = mms_retry;
        string listName = (string) null;
        if (JidHelper.IsBroadcastJid(msg.KeyRemoteJid))
          MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
          {
            Conversation conversation = db.GetConversation(msg.KeyRemoteJid, CreateOptions.None);
            if (conversation == null)
              return;
            listName = conversation.GroupSubject;
          }));
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          fmsg.qcount = msg.GetMiscInfo((SqliteMessagesContext) db, CreateOptions.CreateToDbIfNotFound).ClientRetryCount++;
          db.SubmitChanges();
        }));
        if (fmsg.participants != null)
          conn.SendMultiParticipantMessage(fmsg, "participants", fmsg.participants, listName);
        else
          conn.SendMessage(fmsg);
      }
    }

    public static void ProcessSyncPendingNetworkTasks()
    {
      PersistentAction[] persistActions = (PersistentAction[]) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db => persistActions = db.GetQrPersistentActions()));
      if (persistActions == null || persistActions.Length == 0)
        return;
      PersistentActionRetryState state = new PersistentActionRetryState()
      {
        MaxAutoDownload = new int?(32)
      };
      foreach (PersistentAction a in (IEnumerable<PersistentAction>) ((IEnumerable<PersistentAction>) persistActions).OrderBy<PersistentAction, int>((Func<PersistentAction, int>) (id => id.ActionID)))
        AppState.AttemptPersistentAction(a, AppState.ClientInstance.GetConnection(), state);
    }

    public static void ProcessPendingNetworkTasks()
    {
      foreach (Message message in MessagesContext.Select<Message[]>((Func<MessagesContext, Message[]>) (db => db.GetUnsentMessages())))
      {
        switch (message.Status)
        {
          case FunXMPP.FMessage.Status.Uploading:
          case FunXMPP.FMessage.Status.UploadingCustomHash:
            if (AppState.ShouldRetry(message) && !message.UploadContext.isActiveStreamingUpload())
            {
              message.SetPendingMediaSubscription("Should retry", PendingMediaTransfer.TransferTypes.Upload_NotWeb, MediaUpload.SendMediaObservable(message));
              break;
            }
            break;
          case FunXMPP.FMessage.Status.Unsent:
          case FunXMPP.FMessage.Status.Relay:
            if (AppState.ShouldRetry(message))
            {
              AppState.SendMessage(AppState.ClientInstance.GetConnection(), message);
              break;
            }
            break;
          case FunXMPP.FMessage.Status.Pending:
            if (AppState.ShouldRetry(message))
            {
              AppState.ProcessPendingMessage(message);
              break;
            }
            break;
          default:
            Log.d("Unsent message query - unexpected value " + (object) message.Status);
            break;
        }
      }
      PersistentAction[] persistActions = (PersistentAction[]) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db => persistActions = db.GetPersistentActions()));
      if (persistActions == null || !((IEnumerable<PersistentAction>) persistActions).Any<PersistentAction>())
        return;
      PersistentActionRetryState state = new PersistentActionRetryState()
      {
        MaxAutoDownload = new int?(32)
      };
      foreach (PersistentAction a in persistActions)
        AppState.AttemptPersistentAction(a, AppState.ClientInstance.GetConnection(), state);
    }

    public static bool ShouldRetry(Message msg)
    {
      bool flag = false;
      string str = (string) null;
      if (msg.KeyRemoteJid == null)
      {
        Log.SendCrashLog((Exception) new NullReferenceException("KeyRemoteJid is null!"), nameof (ShouldRetry));
        flag = true;
        str = "null jid";
      }
      else
      {
        DateTime? funTimestamp = msg.FunTimestamp;
        if (funTimestamp.HasValue)
        {
          DateTime universalTime = DateTime.Now.ToUniversalTime();
          funTimestamp = msg.FunTimestamp;
          DateTime dateTime = funTimestamp.Value;
          if ((universalTime - dateTime).TotalHours >= 24.0)
          {
            flag = true;
            str = "over 24h";
            goto label_7;
          }
        }
        if (msg.ContainsMediaContent() && !msg.LocalFileExists())
        {
          flag = true;
          str = string.Format("file missing {0}", (object) msg.LocalFileUri);
        }
      }
label_7:
      if (flag)
      {
        Log.d("app state", "aborting message send for id={0}, type={1}, reason={2}", (object) msg.MessageID, (object) msg.MediaWaType, (object) str);
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          msg.Status = FunXMPP.FMessage.Status.Error;
          db.SubmitChanges();
        }));
        if (AppState.GetConnection().EventHandler.Qr.Session.Active)
          AppState.GetConnection().SendQrReceived(new FunXMPP.FMessage.Key(msg.KeyRemoteJid, msg.KeyFromMe, msg.KeyId), FunXMPP.FMessage.Status.Error);
      }
      return !flag;
    }

    public static void SchedulePersistentAction(PersistentAction a, bool synchronous = false, bool attempt = true)
    {
      Action a1 = (Action) (() =>
      {
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          db.StorePersistentAction(a);
          db.SubmitChanges();
        }));
        if (!attempt)
          return;
        FunXMPP.Connection conn = AppState.GetConnection();
        if (conn == null)
          return;
        conn.InvokeIfConnected((Action) (() => AppState.AttemptPersistentAction(a, conn)));
      });
      if (synchronous)
        a1();
      else
        AppState.Worker.Enqueue(a1);
    }

    public static IDisposable AttemptPersistentAction(
      PersistentAction a,
      FunXMPP.Connection conn = null,
      PersistentActionRetryState state = null)
    {
      if (a == null)
        return (IDisposable) null;
      Action<Exception> onError = (Action<Exception>) (ex =>
      {
        try
        {
          MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
          {
            object[] objArray = new object[5]
            {
              (object) a.ActionID,
              (object) a.Jid,
              (object) a.Attempts,
              null,
              null
            };
            int? attemptsLimit = a.AttemptsLimit;
            // ISSUE: variable of a boxed type
            __Boxed<int> local = (ValueType) ((attemptsLimit ?? 100) - a.Attempts);
            attemptsLimit = a.AttemptsLimit;
            string str = attemptsLimit.HasValue ? "" : "(default)";
            objArray[3] = (object) string.Format("{0}{1}", (object) local, (object) str);
            objArray[4] = a.ExpirationTime.HasValue ? (object) a.ExpirationTime.ToString() : (object) "n/a";
            Log.l("persist act", "failed | id:{0},jid:{1},attempts:{2},remaining attempts:{3},expiration:{4}", objArray);
            if (!a.IsAttemptsLimitReached)
              return;
            Log.l("persit act", "deleted after too many attempts | id:{0}", (object) a.ActionID);
            db.DeletePersistentActionOnSubmit(a);
            db.SubmitChanges();
          }));
        }
        catch (DatabaseInvalidatedException ex1)
        {
        }
      });
      Action onDone = (Action) (() =>
      {
        try
        {
          MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
          {
            Log.l("persist act", "finished | id:{0},attempts:{1},type:{2},data:{3}", (object) a.ActionID, (object) a.Attempts, (object) (PersistentAction.Types) a.ActionType, (object) a.ActionDataStringForLog);
            db.DeletePersistentActionOnSubmit(a);
            db.SubmitChanges();
          }));
        }
        catch (Exception ex)
        {
          if (ex is DatabaseInvalidatedException)
            return;
          onError(ex);
        }
      });
      bool shouldAttempt = true;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        Log.l("persist act", "attempt | id:{0},jid:{1},attempts:{2},type:{3},data:{4},expiration:{5}", (object) a.ActionID, (object) a.Jid, (object) a.Attempts, (object) (PersistentAction.Types) a.ActionType, (object) a.ActionDataStringForLog, a.ExpirationTime.HasValue ? (object) a.ExpirationTime.ToString() : (object) "n/a");
        if (a.IsAttemptsLimitReached || a.IsExpired)
        {
          shouldAttempt = false;
          Log.l("persist act", "dropped | id:{0},jid:{1}", (object) a.ActionID, (object) a.Jid);
        }
        else
        {
          ++a.Attempts;
          db.SubmitChanges();
        }
      }));
      IDisposable disposable = (IDisposable) null;
      if (shouldAttempt)
      {
        IObservable<Unit> source = a.Perform(conn ?? AppState.GetConnection(), state);
        if (a.ActionType != 27)
          source = source.ObserveOn<Unit>(WAThreadPool.Scheduler);
        disposable = source.Take<Unit>(1).Subscribe<Unit>((Action<Unit>) (_ => onDone()), onError, (Action) (() => Log.d("persist act", "attempted | id:{0},jid:{1},attempts:{2},type:{3}", (object) a.ActionID, (object) a.Jid, (object) a.Attempts, (object) (PersistentAction.Types) a.ActionType)));
      }
      else
        onDone();
      return disposable;
    }

    public static void ScheduleFileRehash(bool submit = true)
    {
      PersistentAction pa = (PersistentAction) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        PersistentAction[] persistentActions = db.GetPersistentActions(PersistentAction.Types.ReHashLocalFiles);
        if ((persistentActions != null ? (persistentActions.Length != 0 ? 1 : 0) : 0) != 0)
          return;
        pa = new PersistentAction()
        {
          ActionType = 27,
          ExpirationTime = new DateTime?()
        };
        db.StorePersistentAction(pa);
        if (!submit)
          return;
        db.SubmitChanges();
      }));
      if (pa == null)
        return;
      AppState.Worker.Enqueue((Action) (() => AppState.SchedulePersistentAction(pa)));
    }

    public static void ProcessPendingMessage(Message m)
    {
      if (m.Status != FunXMPP.FMessage.Status.Pending)
        return;
      switch (m.MediaWaType)
      {
        case FunXMPP.FMessage.Type.Audio:
        case FunXMPP.FMessage.Type.Video:
        case FunXMPP.FMessage.Type.Gif:
        case FunXMPP.FMessage.Type.Sticker:
          TranscodeWrapper.ProcessPendingTranscode(m);
          break;
        case FunXMPP.FMessage.Type.Contact:
          Message pendingMsg = (Message) null;
          MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
          {
            pendingMsg = db.GetMessage(m.KeyRemoteJid, m.KeyId, m.KeyFromMe);
            if (pendingMsg == null || pendingMsg.Status != FunXMPP.FMessage.Status.Pending)
              return;
            IEnumerable<ContactVCard> contactCards = pendingMsg.GetContactCards();
            if (contactCards.Count<ContactVCard>() > 1)
            {
              IEnumerable<string> strings = contactCards.Select<ContactVCard, string>((Func<ContactVCard, string>) (c => c.ToVCardData(true)));
              MessageProperties forMessage = MessageProperties.GetForMessage(pendingMsg);
              forMessage.Contacts = strings;
              forMessage.Save();
            }
            else
            {
              ContactVCard contactVcard = ContactVCard.Create(pendingMsg.Data);
              if (contactVcard != null)
                pendingMsg.Data = contactVcard.ToVCardData(true);
            }
            pendingMsg.Status = FunXMPP.FMessage.Status.Unsent;
            db.SubmitChanges();
          }));
          if (pendingMsg.Status != FunXMPP.FMessage.Status.Unsent)
            break;
          AppState.SendMessage(AppState.GetConnection(), pendingMsg);
          break;
        case FunXMPP.FMessage.Type.Location:
          LocationHelper.CompletePendingLocationDataMessage(m, false);
          break;
        case FunXMPP.FMessage.Type.LiveLocation:
          LiveLocationManager.Instance.CompletePendingLiveLocationDataMessage(m);
          break;
      }
    }

    public static bool UIUpdatesMuted => AppState.uiUpdatesMuted;

    public static void PatchLocale()
    {
      if (!(AppResources.CultureString == "en-US"))
        return;
      CultureInfo currentUiCulture = CultureInfo.CurrentUICulture;
      CultureInfo cult = (CultureInfo) null;
      string lang = (string) null;
      string locale = (string) null;
      try
      {
        currentUiCulture.GetLangAndLocale(out lang, out locale);
      }
      catch (Exception ex)
      {
      }
      if (lang == "zh" && locale == "HK")
        cult = new CultureInfo("zh-TW");
      else if (lang != null && lang != "en" && lang != "pt")
      {
        string str = lang + "-";
        foreach (string supportedCulture in AppState.GetSupportedCultures())
        {
          if (supportedCulture.StartsWith(str, StringComparison.Ordinal))
          {
            cult = new CultureInfo(supportedCulture);
            break;
          }
        }
      }
      if (cult == null)
        return;
      AppState.SetLocale(cult);
    }

    public static void SetLocale(CultureInfo cult)
    {
      Thread currentThread1 = Thread.CurrentThread;
      Thread currentThread2 = Thread.CurrentThread;
      CultureInfo cultureInfo1;
      CultureInfo.DefaultThreadCurrentUICulture = cultureInfo1 = cult;
      CultureInfo.DefaultThreadCurrentCulture = cultureInfo1;
      CultureInfo cultureInfo2 = cultureInfo1;
      currentThread2.CurrentUICulture = cultureInfo1;
      CultureInfo cultureInfo3 = cultureInfo2;
      currentThread1.CurrentCulture = cultureInfo3;
      Plurals.ResetInstance();
      Utils.CommaSeparator = (string) null;
      AppState.ClientInstance?.OnUILanguageChanged();
    }

    public static bool IsVoipScheduled()
    {
      if (!AppState.IsBackgroundAgent)
        return true;
      try
      {
        ScheduledAction scheduledAction = ScheduledActionService.Find("WhatsApp.IncomingCallAgent");
        return scheduledAction != null && scheduledAction.IsScheduled;
      }
      catch (Exception ex)
      {
        Log.SendCrashLog(ex, "voip scheduled check", logOnlyForRelease: true);
      }
      return false;
    }

    public static bool IsLowMemoryDevice
    {
      get
      {
        return AppState.islowMem_ ?? (AppState.islowMem_ = new bool?(DeviceStatus.DeviceTotalMemory <= 268435456L)).Value;
      }
    }

    public static bool IsDecentMemoryDevice
    {
      get
      {
        return AppState.isDecentMem_ ?? (AppState.isDecentMem_ = new bool?(DeviceStatus.DeviceTotalMemory > 805306368L)).Value;
      }
    }

    public static Version OSVersion
    {
      get
      {
        Version osVersion = Environment.OSVersion.Version;
        if (osVersion.Major >= 10)
        {
          Assembly assembly = IntrospectionExtensions.GetTypeInfo(typeof (Launcher)).Assembly;
          System.Type type1 = assembly.GetType("Windows.System.Profile.AnalyticsInfo");
          System.Type type2 = assembly.GetType("Windows.System.Profile.AnalyticsVersionInfo");
          if (type1 != null && type2 != null)
          {
            object obj = type1.GetRuntimeProperty("VersionInfo").GetValue((object) null);
            long result;
            if (long.TryParse(type2.GetRuntimeProperty("DeviceFamilyVersion").GetValue(obj).ToString(), out result))
              osVersion = new Version((int) (ushort) (result >> 48), (int) (ushort) (result >> 32), (int) (ushort) (result >> 16), (int) (ushort) result);
          }
        }
        return osVersion;
      }
    }

    public static int OSBuildNumber => AppState.OSVersion.Build;

    public static bool IsWP81Gdr1OrLater
    {
      get => AppState.IsWP10OrLater || AppState.OSBuildNumber >= 12397;
    }

    public static bool IsWP10OrLater => AppState.OSVersion.Major >= 10;

    public static bool IsWP10CreatorOrLater
    {
      get => AppState.OSVersion.Major >= 10 && AppState.OSBuildNumber >= 15063;
    }

    public static IRegHelper RegHelper => (IRegHelper) NativeInterfaces.CreateInstance<WhatsAppNative.RegHelper>();

    public static bool BatterySaverEnabled
    {
      get
      {
        try
        {
          return PowerManager.PowerSavingMode == 1;
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "battery savery query");
          return false;
        }
      }
    }

    public static int BatteryPercentage
    {
      get
      {
        int batteryPercentage = -1;
        try
        {
          batteryPercentage = Battery.GetDefault().RemainingChargePercent;
        }
        catch (Exception ex)
        {
        }
        return batteryPercentage;
      }
    }

    public static bool PowerSourceConnected
    {
      get
      {
        bool powerSourceConnected = false;
        try
        {
          powerSourceConnected = DeviceStatus.PowerSource == PowerSource.External;
        }
        catch (Exception ex)
        {
        }
        return powerSourceConnected;
      }
    }

    public static string AppUniqueSuffix => "beta";

    public static string GetStackTrace()
    {
      string stackTrace = new StackTrace().ToString();
      if (stackTrace != null && stackTrace.Length == 0)
        stackTrace = (string) null;
      int num;
      if (stackTrace != null && (num = stackTrace.IndexOf('\n')) >= 0)
        stackTrace = stackTrace.Substring(num + 1);
      return stackTrace;
    }

    public static void PerformIfBeforeDate(Action a, DateTime? dt)
    {
      if (!dt.HasValue || !(DateTime.Now < dt.Value))
        return;
      a();
    }

    public static IDisposable PerformWhenLeavingFg(Action a)
    {
      IDisposable disposable = (IDisposable) null;
      if (!AppState.IsBackgroundAgent)
        disposable = AppState.ClientInstance.PerformWhenLeavingFg(a);
      return disposable;
    }

    public static IDisposable MarkConverationAsOpen(string jid)
    {
      LinkedListNode<string> node = (LinkedListNode<string>) null;
      if (!string.IsNullOrEmpty(jid))
      {
        lock (AppState.convoOpenLock)
        {
          node = AppState.openConvos.AddLast(jid);
          Log.d("appstate", "mark chat as open | jid:{0}", (object) jid);
        }
      }
      return (IDisposable) new DisposableAction((Action) (() =>
      {
        Log.d("appstate", "marking chat as closed | jid:{0}", (object) (jid ?? "n/a"));
        lock (AppState.convoOpenLock)
        {
          if (node == null)
            return;
          AppState.openConvos.Remove(node);
          node = (LinkedListNode<string>) null;
          Log.d("appstate", "mark chat as closed | jid:{0}", (object) jid);
        }
      }));
    }

    public static bool IsConversationOpen(string jid)
    {
      if (AppState.IsBackgroundAgent || string.IsNullOrEmpty(jid))
        return false;
      lock (AppState.convoOpenLock)
        return AppState.openConvos.Any<string>((Func<string, bool>) (str => string.CompareOrdinal(str, jid) == 0));
    }

    public static void GetLangAndLocale(out string lang, out string locale)
    {
      CultureInfo.CurrentUICulture.GetLangAndLocale(out lang, out locale);
    }

    public static bool IsMilitaryTimeDisplayed()
    {
      return CultureInfo.CurrentUICulture.DateTimeFormat.ShortTimePattern.Contains("H");
    }

    public static void CheckDbValid()
    {
      if (!AppState.isInvalidated)
        return;
      AppState.ThrowInvalidated();
    }

    public static void ThrowInvalidated(string str = null)
    {
      if (str == null)
        str = "Told to exit while acquiring lock. Logging exception.";
      Log.d("app state", str);
      throw new DatabaseInvalidatedException(str);
    }

    public static void ThrowInvalidated(string fmt, params object[] args)
    {
      AppState.ThrowInvalidated(string.Format(fmt, args));
    }

    public static void InvalidateDatabases() => AppState.isInvalidated = true;

    public static void RevertInvalidateDatabases() => AppState.isInvalidated = false;

    public static IDisposable PauseWorkerThreads(bool discardAtResume = false)
    {
      Log.d("pausing worker threads...");
      PausedThread state = new PausedThread(new PausedThread[3]
      {
        AppState.BlockDebugStacks(),
        WorkQueue.Pause(discardAtResume),
        WAThreadPool.Pause(discardAtResume)
      });
      state.WaitForPauseCompleted();
      Log.d("OK");
      if (!AppState.IsBackgroundAgent)
        FieldStatsRunner.Shutdown();
      else
        FieldStatsRunner.Sync();
      return (IDisposable) new DisposableAction((Action) (() => state.Unpause()));
    }

    private static PausedThread BlockDebugStacks()
    {
      object @lock = new object();
      bool paused = false;
      IDisposable disp = DebugStack.ApplyFilter((Func<Action, Action>) (a => (Action) (() =>
      {
        lock (@lock)
        {
          if (paused)
            return;
          a();
        }
      })));
      return new PausedThread((Action) (() =>
      {
        lock (@lock)
          paused = true;
      }), (Action) (() =>
      {
        lock (@lock)
        {
          disp.SafeDispose();
          disp = (IDisposable) null;
          paused = false;
        }
      }));
    }

    public static void OnSafeToTouchDatabase()
    {
      Log.SetCrashLogInfo();
      if (AppState.checkedRegLog)
        return;
      switch (Settings.PhoneNumberVerificationState)
      {
        case PhoneNumberVerificationState.VerifiedPendingBackupCheck:
        case PhoneNumberVerificationState.VerifiedPendingHistoryRestore:
        case PhoneNumberVerificationState.Verified:
          AppState.checkedRegLog = true;
          break;
        default:
          NativeInterfaces.Misc.OpenRegLog();
          goto case PhoneNumberVerificationState.VerifiedPendingBackupCheck;
      }
    }

    public static RingerVibrateState RingerVibrateState
    {
      get
      {
        uint Key = 2147483650;
        string Subkey = "Software\\Microsoft\\EventSounds\\Sounds";
        uint ringerVibrateState = 3;
        try
        {
          ringerVibrateState &= AppState.RegHelper.ReadDWord(Key, Subkey, nameof (RingerVibrateState));
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "ringer/vibrate check");
        }
        return (RingerVibrateState) ringerVibrateState;
      }
    }

    public static void CheckDevicePermissions()
    {
      if (!AppState.IsWP10OrLater)
        return;
      try
      {
        DeviceAccessInformation fromDeviceClass = DeviceAccessInformation.CreateFromDeviceClass((DeviceClass) 1);
        if (Settings.DevicesAccessPermissions == fromDeviceClass.CurrentStatus)
          return;
        Log.l("app state", "Permssion has changed for audio. Previous: {0}, Current: {1}", (object) (DeviceAccessStatus) Settings.DevicesAccessPermissions, (object) fromDeviceClass.CurrentStatus);
        Settings.DevicesAccessPermissions = (int) fromDeviceClass.CurrentStatus;
      }
      catch (Exception ex)
      {
        Log.l("app state", "Failed to get audio permission: {0}", (object) ex.Message);
      }
    }

    public static bool UseWindowsNotificationService
    {
      get
      {
        if (!AppState.useWindowsNotificationService.HasValue)
        {
          using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
            AppState.useWindowsNotificationService = new bool?(nativeMediaStorage.FileExists(AppState.AppInstallDir + "\\UseWNS.txt"));
        }
        return AppState.useWindowsNotificationService.GetValueOrDefault();
      }
    }

    public static DateTime GetBuildTime()
    {
      if (AppState.buildTimeUtc.HasValue)
        return AppState.buildTimeUtc.Value;
      BuildHash.AdditionalDetails details = new BuildHash.AdditionalDetails();
      using (Stream file = AppState.OpenFromXAP("WhatsApp.dll"))
        BuildHash.ParsePeImage(file, details);
      return (AppState.buildTimeUtc = new DateTime?(DateTimeUtils.FromUnixTime((long) details.Timestamp))).Value;
    }

    public static TimeSpan GetTimeSinceBuild(DateTime? now = null)
    {
      return (now ?? FunRunner.CurrentServerTimeUtc) - AppState.GetBuildTime().ToUniversalTime();
    }

    public static bool IsExpired
    {
      get => AppState.DaysUntilExpiration() <= 0.0 || AppState.AppUpgradeRequired;
    }

    public static bool IsPhoneTimeValidForBuild()
    {
      TimeSpan timeSinceBuild = AppState.GetTimeSinceBuild(new DateTime?(DateTime.UtcNow));
      bool flag = timeSinceBuild.TotalHours > -1.0 && timeSinceBuild.TotalDays < 366.0;
      if (!flag)
      {
        Log.l("app state", "Phone time not valid for this app build {0}", (object) timeSinceBuild);
        FunRunner.ClearSkewValues("Invalid for Build");
      }
      return flag;
    }

    public static bool IsPhoneTimeBadlySkewed()
    {
      bool flag = !AppState.IsPhoneTimeValidForBuild();
      if (!flag)
      {
        int? currentTimeSkew = FunRunner.CurrentTimeSkew;
        if (currentTimeSkew.HasValue)
          flag = Math.Abs(currentTimeSkew.Value) > 43200;
      }
      return flag;
    }

    public static double DaysUntilExpiration(DateTime? now = null)
    {
      if (AppState.IsFinalRelease())
      {
        Log.l("app state", "Final build expiry being calculated");
        return (Settings.DeprecationDateActual.Value - DateTime.Now).TotalDays;
      }
      DateTime? expirationOverride = Settings.ServerExpirationOverride;
      if (!now.HasValue)
        now = new DateTime?(FunRunner.CurrentServerTimeUtc);
      if (expirationOverride.HasValue)
        return (expirationOverride.Value - now.Value).TotalDays + (double) Settings.Jitter;
      double val2 = (double) (45 + Settings.Jitter) - AppState.GetTimeSinceBuild(now).TotalDays;
      DateTime deprecationDate = AppState.GetDeprecationDate();
      DateTime? nullable = now;
      return Math.Min((double) (nullable.HasValue ? new TimeSpan?(deprecationDate - nullable.GetValueOrDefault()) : new TimeSpan?()).Value.Days, val2);
    }

    public static void UpdateServerExpirationOverride(DateTime expiration)
    {
      DateTime? expirationOverride = Settings.ServerExpirationOverride;
      if (expirationOverride.HasValue)
      {
        DateTime? nullable = expirationOverride;
        DateTime dateTime1 = expiration;
        if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() == dateTime1 ? 1 : 0) : 1) : 0) != 0)
          return;
        nullable = expirationOverride;
        DateTime dateTime2 = expiration;
        if ((nullable.HasValue ? (nullable.GetValueOrDefault() < dateTime2 ? 1 : 0) : 0) != 0)
        {
          Settings.ServerExpirationOverride = new DateTime?(expiration);
        }
        else
        {
          DateTime dateTime3 = DateTime.UtcNow + TimeSpan.FromDays(3.0);
          if (expiration < dateTime3)
            expiration = dateTime3;
          Settings.ServerExpirationOverride = new DateTime?(expiration);
        }
      }
      else
      {
        DateTime dateTime = DateTime.UtcNow + TimeSpan.FromDays(3.0);
        if (expiration < dateTime)
          expiration = dateTime;
        Settings.ServerExpirationOverride = new DateTime?(expiration);
      }
    }

    public static bool AppUpgradeRequired
    {
      get
      {
        string upgradeRequiredVersion = Settings.LoginFailedUpgradeRequiredVersion;
        if (upgradeRequiredVersion != null)
        {
          if (upgradeRequiredVersion == AppState.GetAppVersion())
            return true;
          Settings.LoginFailedUpgradeRequiredVersion = (string) null;
        }
        return false;
      }
      set => Settings.LoginFailedUpgradeRequiredVersion = AppState.GetAppVersion();
    }

    public static DateTime GetDeprecationDate()
    {
      return !(DateTime.Now >= Settings.DeprecationDateOfficial.Value.AddDays(-2.0)) ? Settings.DeprecationDateOfficial.Value : Settings.DeprecationDateActual.Value;
    }

    public static string GetDeprecationDateString()
    {
      return !(DateTime.Now >= Settings.DeprecationDateOfficial.Value.AddDays(-2.0)) ? AppResources.WPDeprecationDateDec312019 : AppResources.WpDeprecationDateJan142020;
    }

    public static double DaysUntilDeprecation(DateTime? now = null)
    {
      return (double) (AppState.GetDeprecationDate() - DateTime.Now).Days;
    }

    public static bool ShowDeprecationMessaging()
    {
      DateTime now = DateTime.Now;
      DateTime? deprecationDateMessaging = Settings.DeprecationDateMessaging;
      return deprecationDateMessaging.HasValue && now > deprecationDateMessaging.GetValueOrDefault();
    }

    public static bool IsFinalRelease() => Settings.DeprecationFinalRelease == 1;

    public static bool IsTimeForDeprecationNag()
    {
      double num1 = AppState.DeprecationNagFrequency();
      DateTime? deprecationNagTime = Settings.LastDeprecationNagTime;
      if (!deprecationNagTime.HasValue)
        return AppState.ShowDeprecationMessaging();
      DateTime? nullable1 = deprecationNagTime;
      DateTime utcNow1 = DateTime.UtcNow;
      bool flag;
      if ((nullable1.HasValue ? (nullable1.GetValueOrDefault() > utcNow1 ? 1 : 0) : 0) != 0)
      {
        flag = true;
      }
      else
      {
        int num2;
        if (num1 > 0.0)
        {
          DateTime utcNow2 = DateTime.UtcNow;
          DateTime? nullable2 = deprecationNagTime;
          TimeSpan? nullable3 = nullable2.HasValue ? new TimeSpan?(utcNow2 - nullable2.GetValueOrDefault()) : new TimeSpan?();
          TimeSpan timeSpan = TimeSpan.FromDays(1.0 * num1);
          num2 = nullable3.HasValue ? (nullable3.GetValueOrDefault() > timeSpan ? 1 : 0) : 0;
        }
        else
          num2 = 0;
        flag = num2 != 0;
      }
      return flag;
    }

    public static double DeprecationNagFrequency()
    {
      double num = AppState.DaysUntilDeprecation();
      Log.d("app state", "days till deprecation:{0}", (object) num);
      return AppState.ShowDeprecationMessaging() ? (num < 31.0 ? (num < 15.0 ? (num < 8.0 ? (num < 6.0 ? 0.1 : 0.5) : 1.0) : 2.0) : 5.0) : 0.0;
    }

    public static void SetDeprecationNagDisplayed()
    {
      Settings.LastDeprecationNagTime = new DateTime?(DateTime.UtcNow);
    }

    public static IObservable<Unit> MonitorSettingsConditionObservable(
      Func<bool> check,
      Settings.Key key)
    {
      return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        IDisposable disp = (IDisposable) null;
        DisposableAction safeDisp = new DisposableAction((Action) (() =>
        {
          disp.SafeDispose();
          disp = (IDisposable) null;
        }));
        Func<bool> performCheck = (Func<bool>) (() =>
        {
          bool flag;
          if (flag = check())
          {
            try
            {
              observer.OnNext(new Unit());
            }
            finally
            {
              observer.OnCompleted();
              safeDisp.Dispose();
            }
          }
          return flag;
        });
        if (!performCheck())
        {
          int num1;
          disp = Settings.GetSettingsChangedObservable(new Settings.Key[1]
          {
            key
          }).Subscribe<Settings.Key>((Action<Settings.Key>) (_ => num1 = performCheck() ? 1 : 0));
          int num2 = performCheck() ? 1 : 0;
        }
        return new Action(safeDisp.Dispose);
      })).Take<Unit>(1);
    }

    public interface Client
    {
      FunXMPP.Connection GetConnection();

      void OnLoginException(FunXMPP.LoginFailureException ex);

      void OnDatabaseCorruption();

      void Add(BackgroundTransferRequest req);

      void ShowWebTask(Uri uri);

      IDisposable LockScreenSubscription();

      void ResetConnection();

      void ShowToast(string str, string destUri);

      void ShowMessageBox(string str);

      void ShowErrorMessage(string errMsg, bool useMsgBox);

      string GetBuildHash();

      Subject<ApplicationUnhandledExceptionEventArgs> GetUnhandledExceptionSubject();

      void CheckPushName();

      IDisposable PerformWhenLeavingFg(Action a);

      IObservable<bool> Decision(
        IObservable<bool> src,
        string prompt,
        string positive = null,
        string negative = null,
        string title = null);

      bool IsActive();

      void PromptRateCall(string peerJid, byte[] fsCookie);

      void OnUILanguageChanged();
    }
  }
}
