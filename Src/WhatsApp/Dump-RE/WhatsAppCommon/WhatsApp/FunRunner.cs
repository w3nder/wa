// Decompiled with JetBrains decompiler
// Type: WhatsApp.FunRunner
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using WhatsAppNative;

#nullable disable
namespace WhatsApp
{
  public class FunRunner
  {
    private const string LogHeader = "funrunner";
    private static AutoResetEvent backoffEvent;
    private static IWASocket sock = (IWASocket) null;
    private static int backoffMs = 0;
    private static int backoffMsB = 0;
    private static bool invalidated = false;
    private static object invalidatedLock = new object();
    public static int BackoffMax = 3000;
    private static bool isClosedConnection = false;
    private static int[] Ports = new int[3]{ 443, 5222, 80 };
    private static bool CyclePorts = false;
    private static uint? currentPort = new uint?();
    private static IHost hosts_ = (IHost) null;
    private static object hostsLock = new object();
    public static FunRunner.SocketStates SocketState = FunRunner.SocketStates.Disconnected;
    public static DateTime? LastDisconnectedPhoneTimeUtc = new DateTime?();
    private static int? lastKnownTimeSkew = new int?();
    internal static object PreserveSideEffect;
    public static bool FailedLoginRetryTimerEnabled = true;
    public static Subject<Unit> FailedLoginRetrySucceededSubject = new Subject<Unit>();
    private const int validDayRangeForNtpResult = 1095;

    public static AutoResetEvent BackoffEvent
    {
      get
      {
        return Utils.LazyInit<AutoResetEvent>(ref FunRunner.backoffEvent, (Func<AutoResetEvent>) (() => new AutoResetEvent(false)));
      }
    }

    private static bool IsWifiChatConnection { get; set; } = false;

    private static IWASocket CreateTcpSocket(int port)
    {
      bool flag = false;
      int requestedFibBackoffState = Settings.ServerRequestedFibBackoffState;
      if (FunRunner.backoffMs != 0 || requestedFibBackoffState > 0 && FunRunner.backoffMsB != 0)
      {
        int millisecondsTimeout;
        if (FunRunner.backoffMs != 0)
        {
          millisecondsTimeout = FunRunner.backoffMs;
        }
        else
        {
          millisecondsTimeout = (int) Utils.CalculateFibonacci(requestedFibBackoffState - 1) * 1000;
          if (millisecondsTimeout < FunRunner.BackoffMax)
            Settings.ServerRequestedFibBackoffState = ++requestedFibBackoffState;
          else
            millisecondsTimeout = FunRunner.BackoffMax;
          Log.l("funrunner", "Server backoff: {0} - {1}", (object) requestedFibBackoffState, (object) millisecondsTimeout);
        }
        if (millisecondsTimeout < 0)
          millisecondsTimeout = FunRunner.BackoffMax;
        Log.l("funrunner", "back off for {0} ms", (object) millisecondsTimeout);
        if (FunRunner.BackoffEvent.WaitOne(millisecondsTimeout))
          flag = true;
      }
      else if (FunRunner.backoffMsB == 0)
        flag = true;
      if (flag)
      {
        FunRunner.backoffMs = 0;
        FunRunner.backoffMsB = 500;
      }
      else
      {
        int backoffMs = FunRunner.backoffMs;
        FunRunner.backoffMs = FunRunner.backoffMsB;
        FunRunner.backoffMsB = FunRunner.backoffMs + backoffMs;
        FunRunner.backoffMs = Math.Min(FunRunner.backoffMs, FunRunner.BackoffMax);
      }
      IWASocket tcpSocket = (IWASocket) null;
      lock (FunRunner.invalidatedLock)
      {
        if (!FunRunner.invalidated)
        {
          tcpSocket = port != 80 ? (IWASocket) NativeInterfaces.CreateInstance<WASocket>() : (IWASocket) new ChunkedHttpSocket();
          tcpSocket.SetHost(FunRunner.Hosts);
          tcpSocket.SetPort((ushort) port);
          tcpSocket.SetTimeoutMilliseconds((int) Constants.LoginTimeout.TotalMilliseconds, true);
          FieldStats.ReportChatConnection(port);
          Log.p("funrunner", "chat port: {0}", (object) port);
          FunRunner.IsWifiChatConnection = NetworkStateMonitor.IsWifiDataConnected();
        }
      }
      return tcpSocket;
    }

    public static void Invalidate()
    {
      lock (FunRunner.invalidatedLock)
        FunRunner.invalidated = true;
    }

    public static void RevertInvalidate()
    {
      lock (FunRunner.invalidatedLock)
        FunRunner.invalidated = false;
    }

    public static void CloseSocket()
    {
      FunRunner.IsWifiChatConnection = false;
      try
      {
        FunRunner.BackoffEvent.Set();
        FunRunner.CyclePorts = false;
        if (FunRunner.sock != null)
        {
          IWASocket sock = FunRunner.sock;
          FunRunner.sock = (IWASocket) null;
          sock.Close();
          sock.Dispose();
        }
        else
        {
          FunRunner.isClosedConnection = true;
          FunRunner.BackoffEvent.Set();
        }
      }
      catch (Exception ex)
      {
      }
    }

    public static void ResetSocket()
    {
      FunRunner.isClosedConnection = false;
      try
      {
        if (FunRunner.sock == null)
          return;
        IWASocket sock = FunRunner.sock;
        FunRunner.sock = (IWASocket) null;
        sock.Close();
        sock.Dispose();
      }
      catch (Exception ex)
      {
      }
    }

    private static uint CurrentPort
    {
      get
      {
        if (!FunRunner.currentPort.HasValue)
          FunRunner.currentPort = new uint?((uint) Settings.LastGoodPortIndex);
        return FunRunner.currentPort.Value;
      }
      set => FunRunner.currentPort = new uint?(value);
    }

    public static IHost Hosts
    {
      get
      {
        return Utils.LazyInit<IHost>(ref FunRunner.hosts_, (Func<IHost>) (() =>
        {
          NetworkStateMonitor.Instance.Observable.Subscribe<NetworkStateChange>((Action<NetworkStateChange>) (flags =>
          {
            lock (FunRunner.hostsLock)
              FunRunner.hosts_ = FunRunner.CreateHosts();
          }));
          return FunRunner.CreateHosts();
        }), FunRunner.hostsLock);
      }
      set => FunRunner.hosts_ = value;
    }

    public static IHost CreateHosts()
    {
      IHost instance = (IHost) NativeInterfaces.CreateInstance<HostBucket>();
      string inetAddress = (string) null;
      int portNumber = -1;
      bool force = false;
      if (FunRunner.TryFallbackIp(out inetAddress, out portNumber, out force))
      {
        Log.l("funrunner", "Using supplied Ip {0}", (object) force);
        instance.AddHost(inetAddress);
        if (force)
          instance.SetShuffleMode(HostShuffleMode.ShuffleSockaddrs);
      }
      HostCollection.EntryType type = NonDbSettings.ChatDnsDomain == "fb" ? HostCollection.EntryType.FBChat : HostCollection.EntryType.Chat;
      IEnumerable<string> first = HostCollection.Instance.GetHostsByType(type).Select<HostCollection.Entry, string>((Func<HostCollection.Entry, string>) (h => h.Name)).Shuffle<string>();
      if (type == HostCollection.EntryType.FBChat)
      {
        IEnumerable<string> second = HostCollection.Instance.GetHostsByType(HostCollection.EntryType.Chat).Select<HostCollection.Entry, string>((Func<HostCollection.Entry, string>) (h => h.Name)).Shuffle<string>();
        first = first.Concat<string>(second);
        instance.SetShuffleMode(HostShuffleMode.ShuffleSockaddrs);
      }
      foreach (string HostName in first)
        instance.AddHost(HostName);
      instance.SetResolver(Resolver.NativeInstance);
      return instance;
    }

    public static DateTime CurrentServerTimeUtc
    {
      get => DateTime.UtcNow - TimeSpan.FromSeconds(Settings.LastLocalServerTimeDiff);
    }

    public static int LastKnownTimeSkew
    {
      get
      {
        return FunRunner.CurrentTimeSkew ?? FunRunner.lastKnownTimeSkew ?? (FunRunner.lastKnownTimeSkew = new int?((int) Settings.LastLocalServerTimeDiff)).Value;
      }
    }

    public static int? CurrentTimeSkew { get; set; }

    public static void ClearSkewValues(string context)
    {
      Log.l("funrunner", "Resetting skew variables: {0}", (object) context);
      Settings.LastLocalServerTimeDiff = 0.0;
      FunRunner.CurrentTimeSkew = new int?(0);
    }

    public static void OnServerTime(DateTime serverTime)
    {
      Settings.LastKnownServerTimeUtc = new DateTime?(serverTime);
      DateTime utcNow = DateTime.UtcNow;
      double totalSeconds = (utcNow - serverTime).TotalSeconds;
      Settings.LastLocalServerTimeDiff = totalSeconds;
      FunRunner.lastKnownTimeSkew = new int?();
      FunRunner.CurrentTimeSkew = new int?((int) totalSeconds);
      Log.l("funrunner", "local phone utc:{0},server utc:{1},time skew:{2} seconds", (object) utcNow, (object) serverTime, (object) (int) totalSeconds);
    }

    private static IDisposable ConnectAndLogin(
      FunXMPP.Connection conn,
      FunRunner.SocketEventHandler handler,
      Action onLogin)
    {
      if (FunRunner.CyclePorts)
        ++FunRunner.CurrentPort;
      else
        FunRunner.CyclePorts = true;
      int port = FunRunner.Ports[(long) FunRunner.CurrentPort % (long) FunRunner.Ports.Length];
      IWASocket socket = (IWASocket) null;
      if (!FunRunner.isClosedConnection)
        socket = FunRunner.sock = FunRunner.CreateTcpSocket(port);
      if (socket == null || FunRunner.isClosedConnection)
      {
        if (FunRunner.sock != null)
          Log.d("funrunner", "socket closed during CreateTcpSocket");
        FunRunner.ResetSocket();
        return (IDisposable) new DisposableAction((Action) (() => { }));
      }
      socket.SetHandler((IWASocketHandler) handler);
      WAProtocol protocol = new WAProtocol(Settings.ClientStaticPrivateKey, Settings.ClientStaticPublicKey, Settings.ServerStaticPublicKey, conn, socket, handler, Settings.ChatID);
      FunXMPP.BinTreeNodeReader binTreeNodeReader = new FunXMPP.BinTreeNodeReader((FunXMPP.StanzaProvider) protocol, conn, FunXMPP.Dictionary);
      FunXMPP.BinTreeNodeWriter binTreeNodeWriter = new FunXMPP.BinTreeNodeWriter((FunXMPP.StanzaWriter) protocol, FunXMPP.Dictionary);
      protocol.LoggedIn += (EventHandler<FunXMPP.LoginEventArgs>) ((sender, args) =>
      {
        FunRunner.SocketState = FunRunner.SocketStates.Connected;
        Log.l("funrunner", string.Format("Logged in | {0}", FunRunner.IsWifiChatConnection ? (object) "Wifi" : (object) "Cellular"));
        Settings.LastGoodPortIndex = (int) ((long) FunRunner.CurrentPort % (long) FunRunner.Ports.Length);
        FunRunner.CurrentTimeSkew = new int?();
        if (args.ServerTime.HasValue)
          FunRunner.OnServerTime(args.ServerTime.Value);
        Settings.SuccessfulLoginUtc = new DateTime?(FunRunner.CurrentServerTimeUtc);
        FunRunner.backoffMs = 0;
        Settings.ServerRequestedFibBackoffStateSaved = Settings.ServerRequestedFibBackoffState;
        Settings.ServerRequestedFibBackoffState = 0;
        conn.Encryption.OnLoggedIn();
        conn.SetWAProtocol(protocol);
        FunRunner.OnLogin(conn);
        onLogin();
        socket.SetTimeoutMilliseconds((int) AppState.PingTimeout.TotalMilliseconds, false);
        conn.SendDanglingAcksAndExitPassive();
      });
      protocol.Connect();
      FunRunner.SocketState = FunRunner.SocketStates.Connecting;
      socket.Connect((IWAScheduler) null);
      IDisposable swapToWiFi = (IDisposable) null;
      if (!FunRunner.IsWifiChatConnection)
      {
        int MIN_SECS_CONNECTION_BEFORE_SWAPPING_TO_WIFI = 15;
        DateTime connectionStartTime = DateTime.Now;
        swapToWiFi = NetworkStateMonitor.Instance.Observable.Subscribe<NetworkStateChange>((Action<NetworkStateChange>) (flags =>
        {
          if ((flags & NetworkStateChange.WifiInternetConnected) != NetworkStateChange.WifiInternetConnected)
            return;
          if (FunRunner.IsWifiChatConnection)
          {
            Log.l("funrunner", "Not disconnecting chatd to swap to WiFi - already WiFi");
            swapToWiFi.SafeDispose();
          }
          else
          {
            if (!(DateTime.Now > connectionStartTime.AddSeconds((double) MIN_SECS_CONNECTION_BEFORE_SWAPPING_TO_WIFI)))
              return;
            Log.l("funrunner", "Disconnecting chatd to swap to WiFi");
            swapToWiFi.SafeDispose();
            AppState.ConnectionResetSubject.OnNext(new Unit());
          }
        }));
      }
      return (IDisposable) new DisposableAction((Action) (() =>
      {
        swapToWiFi.SafeDispose();
        socket?.Close();
        socket = (IWASocket) null;
        FunRunner.sock = (IWASocket) null;
      }));
    }

    public static IDisposable ConnectAndRead(
      FunXMPP.Connection c,
      IObservable<Unit> reset,
      Func<IObservable<Unit>, IObservable<Unit>> repeat = null)
    {
      if (repeat == null)
        repeat = (Func<IObservable<Unit>, IObservable<Unit>>) (obs => obs);
      IObservable<Unit> source = Observable.CreateWithDisposable<Unit>((Func<IObserver<Unit>, IDisposable>) (observer =>
      {
        try
        {
          ++FieldStatsRunner.LoginRetryCount;
          return FunRunner.ConnectAndRead(c, reset, (Action) (() => observer.OnCompleted()));
        }
        catch (Exception ex)
        {
          observer.OnError(ex);
          observer.OnCompleted();
          return (IDisposable) new DisposableAction((Action) (() => { }));
        }
      })).Catch<Unit, DatabaseInvalidatedException>((Func<DatabaseInvalidatedException, IObservable<Unit>>) (ex => Observable.Empty<Unit>()));
      return repeat(source.SimpleSubscribeOn<Unit>(WAThreadPool.Scheduler)).Subscribe<Unit>();
    }

    private static bool ShouldAttemptLoginOnFailure()
    {
      if (!FunRunner.FailedLoginRetryTimerEnabled)
        return false;
      DateTime? loginFailedRetryUtc = Settings.LoginFailedRetryUtc;
      if (!loginFailedRetryUtc.HasValue)
        return false;
      DateTime? nullable = loginFailedRetryUtc;
      DateTime currentServerTimeUtc = FunRunner.CurrentServerTimeUtc;
      return nullable.HasValue && nullable.GetValueOrDefault() < currentServerTimeUtc;
    }

    private static void OnLoginResultAfterRetry(bool ok)
    {
      if (ok)
      {
        Settings.DeleteMany((IEnumerable<Settings.Key>) new Settings.Key[5]
        {
          Settings.Key.LoginFailed,
          Settings.Key.LoginFailedReason,
          Settings.Key.LoginFailedRetryUtc,
          Settings.Key.LoginFailedExpirationUtc,
          Settings.Key.LoginFailedExpirationTotalSeconds
        });
        FunRunner.FailedLoginRetrySucceededSubject.OnNext(new Unit());
      }
      else
        FunRunner.FailedLoginRetryTimerEnabled = false;
    }

    private static IDisposable ConnectAndRead(
      FunXMPP.Connection c,
      IObservable<Unit> reset,
      Action onDisconnect)
    {
      FunRunner.ResetSocket();
      bool loginFailedButAttemptingAnyway = false;
      DiskSpace diskSpace = NativeInterfaces.Misc.GetDiskSpace(Constants.IsoStorePath);
      if (Settings.PhoneNumberVerificationState != PhoneNumberVerificationState.Verified)
      {
        if (!AppState.IsBackgroundAgent)
        {
          if (!((IEnumerable<PhoneNumberVerificationState>) new PhoneNumberVerificationState[2]
          {
            PhoneNumberVerificationState.VerifiedPendingBackupCheck,
            PhoneNumberVerificationState.VerifiedPendingHistoryRestore
          }).Contains<PhoneNumberVerificationState>(Settings.PhoneNumberVerificationState))
            goto label_5;
        }
        else
          goto label_5;
      }
      if (diskSpace.FreeBytes / 1024UL / 1024UL > 15UL && (!Settings.LoginFailed || (loginFailedButAttemptingAnyway = FunRunner.ShouldAttemptLoginOnFailure())) && !AppState.IsExpired && AppState.IsPhoneTimeValidForBuild() && !Settings.CorruptDb)
      {
        Log.l("funrunner", "Creating a new socket.");
        FunRunner.SocketEventHandler handler = new FunRunner.SocketEventHandler();
        object disconnectLock = new object();
        Action raiseOnDisconnect = (Action) (() =>
        {
          lock (disconnectLock)
          {
            Action action = onDisconnect;
            if (action == null)
              return;
            Log.l("funrunner", "raising OnDisconnect");
            onDisconnect = (Action) null;
            action();
          }
        });
        handler.OnError += (EventHandler<FunRunner.SocketEventHandler.ErrorEventArgs>) ((sender, args) =>
        {
          Exception exception = args.Exception;
          FunXMPP.LoginFailureException ex = exception as FunXMPP.LoginFailureException;
          FieldStats.ReportLogin(wam_enum_login_result_type.ERROR_UNKNOWN);
          if (ex != null)
          {
            if (loginFailedButAttemptingAnyway)
            {
              FunRunner.OnLoginResultAfterRetry(false);
            }
            else
            {
              Settings.SuccessfulLoginUtc = new DateTime?();
              AppState.OnLoginException(ex);
            }
          }
          else
          {
            if (!FunRunner.IsInteresting(exception))
              return;
            Log.SendCrashLog(exception, "readLoop");
          }
        });
        handler.OnDisconnect += (EventHandler<FunRunner.SocketEventHandler.DisconnectEventArgs>) ((sender, args) =>
        {
          c.OnConnectionLost();
          raiseOnDisconnect();
        });
        Action onLogin = (Action) (() =>
        {
          FieldStats.ReportLogin(wam_enum_login_result_type.OK);
          if (!loginFailedButAttemptingAnyway)
            return;
          FunRunner.OnLoginResultAfterRetry(true);
        });
        IDisposable connSub = FunRunner.ConnectAndLogin(c, handler, onLogin);
        Subject<Unit> localReset = new Subject<Unit>();
        reset.Do<Unit>((Action<Unit>) (_ => FunRunner.BackoffEvent.Set())).Merge<Unit>((IObservable<Unit>) localReset).Take<Unit>(1).Subscribe<Unit>((Action<Unit>) (_ =>
        {
          if (connSub == null)
            return;
          connSub.Dispose();
          connSub = (IDisposable) null;
        }));
        return (IDisposable) new DisposableAction((Action) (() =>
        {
          localReset.OnNext(new Unit());
          Log.l("funrunner", "Socket disposed");
        }));
      }
label_5:
      FieldStats.ReportLogin(wam_enum_login_result_type.ERROR_UNKNOWN);
      FunRunner.TryTimeFromNtp();
      Log.l("funrunner", "client is not yet registered; waiting for reset event before connecting | state:{0}{1}", (object) Settings.PhoneNumberVerificationState.ToString(), Settings.LoginFailed ? (object) " | login failed" : (object) "");
      return reset.Merge<Unit>(Settings.GetSettingsChangedObservable(new Settings.Key[1]
      {
        Settings.Key.PhoneNumberVerificationState
      }).Select<Settings.Key, Unit>((Func<Settings.Key, Unit>) (_ => new Unit()))).Take<Unit>(1).Concat<Unit>(Observable.CreateWithDisposable<Unit>((Func<IObserver<Unit>, IDisposable>) (observer => FunRunner.ConnectAndRead(c, reset, onDisconnect)))).Subscribe<Unit>();
    }

    private static void TryTimeFromNtp()
    {
      if (!Settings.LastKnownServerTimeUtc.HasValue && !AppState.IsExpired || !AppState.IsPhoneTimeValidForBuild() || !AppState.IsPhoneTimeBadlySkewed() && !AppState.IsExpired)
        return;
      DateTime utcNow = DateTime.UtcNow;
      DateTime? lastNtpCheck = Settings.LastNtpCheck;
      TimeSpan? nullable1 = lastNtpCheck.HasValue ? new TimeSpan?(utcNow - lastNtpCheck.GetValueOrDefault()) : new TimeSpan?();
      ref TimeSpan? local = ref nullable1;
      double? nullable2 = local.HasValue ? new double?(local.GetValueOrDefault().TotalHours) : new double?();
      if (nullable2.HasValue && Math.Abs(nullable2.Value) < 12.0)
        return;
      ThreadPool.QueueUserWorkItem((WaitCallback) (_ => new NtpClient("2.android.pool.ntp.org").GetCurrentTime().ObserveOn<DateTime>((IScheduler) AppState.Worker).Subscribe<DateTime>((Action<DateTime>) (dt =>
      {
        if (Math.Abs(AppState.GetTimeSinceBuild(new DateTime?(dt)).TotalDays) > 1095.0)
        {
          Log.l("funrunner", "Ignoring odd ntp supplied time {0}", (object) dt);
        }
        else
        {
          FunRunner.OnServerTime(dt);
          Settings.LastNtpCheck = new DateTime?(DateTime.UtcNow);
        }
      }), (Action<Exception>) (ex => Log.LogException(ex, "ntp")))));
    }

    private static bool IsInteresting(Exception ex)
    {
      switch (ex)
      {
        case DatabaseInvalidatedException _:
        case ThreadAbortException _:
          return false;
        case FunXMPP.LoginFailureException _:
        case FunXMPP.StreamEndException _:
          return false;
        default:
          return true;
      }
    }

    private static void OnLogin(FunXMPP.Connection conn) => conn.SendAvailableForChat(false);

    public static void SaveFallbackIp(string fbIpString)
    {
      if (string.IsNullOrEmpty(fbIpString) || fbIpString.ToLowerInvariant() == "clear")
        NonDbSettings.FallbackIp = (string) null;
      else
        NonDbSettings.FallbackIp = fbIpString;
    }

    private static bool TryFallbackIp(out string inetAddress, out int portNumber, out bool force)
    {
      bool flag = false;
      inetAddress = (string) null;
      portNumber = -1;
      force = true;
      string fallbackIp = NonDbSettings.FallbackIp;
      if (!string.IsNullOrEmpty(fallbackIp))
      {
        try
        {
          string[] strArray = fallbackIp.Split('|');
          long result1 = 0;
          long.TryParse(strArray[2], out result1);
          long result2 = 0;
          long.TryParse(strArray[3], out result2);
          if (FunRunner.CurrentServerTimeUtc.ToUnixTime() < result1 + result2)
          {
            inetAddress = strArray[0];
            int.TryParse(strArray[1], out portNumber);
            if (strArray.Length >= 6)
              force = strArray[5] == "true";
            flag = true;
          }
          else
            NonDbSettings.FallbackIp = (string) null;
        }
        catch (Exception ex)
        {
          Log.l("funrunner", "Exception extracting from {0}", (object) (fallbackIp ?? "null"));
          Log.l(ex, "Exception extracting from fbIps value");
          NonDbSettings.FallbackIp = (string) null;
        }
      }
      return flag;
    }

    public enum SocketStates
    {
      Disconnected,
      Connecting,
      LoggingIn,
      Connected,
    }

    public class SocketEventHandler : IWASocketHandler
    {
      private MemoryStream memory = new MemoryStream();
      private int offset;

      public event FunRunner.SocketEventHandler.BytesHandler BytesAvailable;

      public event EventHandler<FunRunner.SocketEventHandler.ErrorEventArgs> OnError;

      public event EventHandler<FunRunner.SocketEventHandler.DisconnectEventArgs> OnDisconnect;

      public void Connected() => FunRunner.SocketState = FunRunner.SocketStates.LoggingIn;

      public void Disconnected(uint hr)
      {
        if (this.OnDisconnect != null)
          this.OnDisconnect((object) this, new FunRunner.SocketEventHandler.DisconnectEventArgs()
          {
            HResult = hr
          });
        if (FunRunner.SocketState == FunRunner.SocketStates.LoggingIn || FunRunner.SocketState == FunRunner.SocketStates.Connected)
          FunRunner.LastDisconnectedPhoneTimeUtc = new DateTime?(DateTime.UtcNow);
        FunRunner.SocketState = FunRunner.SocketStates.Disconnected;
        FunRunner.CurrentTimeSkew = new int?();
      }

      public void BytesIn(IByteBuffer buffer)
      {
        byte[] numArray = buffer.Get();
        buffer = (IByteBuffer) null;
        if (this.memory.Length == 0L)
        {
          int length1 = numArray.Length;
          int length2 = length1;
          int localOffset = 0;
          this.OnReaderBytesIn(numArray, ref localOffset, ref length2);
          if (localOffset >= length1)
            return;
          this.memory.Write(numArray, localOffset, length2);
        }
        else
        {
          this.memory.Write(numArray, 0, numArray.Length);
          int length = (int) this.memory.Length - this.offset;
          this.OnReaderBytesIn(this.memory.GetBuffer(), ref this.offset, ref length);
          if ((long) this.offset < this.memory.Length)
            return;
          this.memory.Position = 0L;
          this.memory.SetLength(0L);
          this.offset = 0;
        }
      }

      public void OnReaderBytesIn(byte[] buf, ref int localOffset, ref int length)
      {
        try
        {
          int num = 0;
          FunRunner.SocketEventHandler.BytesHandler bytesAvailable = this.BytesAvailable;
          if (bytesAvailable != null)
            num = bytesAvailable(buf, localOffset, length);
          localOffset += num;
          length -= num;
        }
        catch (Exception ex)
        {
          if (FunRunner.IsInteresting(ex))
            Log.SendCrashLog(ex, "read loop");
          if (this.OnError != null)
            this.OnError((object) this, new FunRunner.SocketEventHandler.ErrorEventArgs()
            {
              Exception = ex
            });
          throw;
        }
      }

      public void WriteBufferDrained()
      {
      }

      public delegate int BytesHandler(byte[] buffer, int offset, int length);

      public class ErrorEventArgs : EventArgs
      {
        public Exception Exception;
      }

      public class DisconnectEventArgs : EventArgs
      {
        public uint HResult;
      }
    }
  }
}
