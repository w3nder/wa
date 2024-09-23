// Decompiled with JetBrains decompiler
// Type: WhatsApp.Voip
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Networking.Voip;
using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using WhatsAppNative;
using WhatsAppNative.OutOfProc;
using Windows.Foundation;
using Windows.Phone.Media.Devices;
using Windows.Phone.Networking.Voip;

#nullable disable
namespace WhatsApp
{
  public class Voip
  {
    public const int RingTimeout = 45;
    public const int BusyTimeout = 30;
    public const int DefaultCallRatingIntervalInSeconds = 86400;
    private static object voipInitLock = new object();
    private static LinkedList<Action> outOfProcCleanupCallbacks = new LinkedList<Action>();
    private static Action disposeFGUserCallbacks = (Action) null;
    private static bool userCallbacksRegistered = false;
    private static Subject<Unit> reregisterSubject = new Subject<Unit>();
    private static WorkQueue worker = (WorkQueue) null;
    private const WorkQueue.StartFlags workerFlags = WorkQueue.StartFlags.Unpausable | WorkQueue.StartFlags.WatchdogExcempt;
    public static EventWaitHandle InCallEvent = new EventWaitHandle(false, EventResetMode.ManualReset, Constants.InCallEventName);
    public static EventWaitHandle BgSockRequestEvent = new EventWaitHandle(false, EventResetMode.AutoReset, Constants.BgSockRequestEventName);
    private static bool connectionEventsRegistered = false;
    private static Server outofProc;
    private static WhatsApp.Voip.ManagedVoipFactory managedFactory = (WhatsApp.Voip.ManagedVoipFactory) null;
    private static IDisposable outOfProcSub = (IDisposable) null;
    private static RefCountAction outOfProcAction = new RefCountAction((Action) (() =>
    {
      OutOfProcRegistration instance = NativeInterfaces.CreateInstance<OutOfProcRegistration>();
      EventWaitHandle eventWaitHandle = WhatsApp.Voip.EventForPid(NativeInterfaces.Misc.GetProcessId());
      eventWaitHandle.Set();
      WhatsApp.Voip.outOfProcSub = (IDisposable) new WhatsApp.Voip.OutOfProcReg()
      {
        Registration = instance,
        Event = eventWaitHandle
      };
    }), (Action) (() =>
    {
      WhatsApp.Voip.outOfProcSub.Dispose();
      WhatsApp.Voip.outOfProcSub = (IDisposable) null;
    }));
    private static CancellationTokenSource delayedCallCancelled = (CancellationTokenSource) null;
    private static Subject<bool> delayedCallExecuted = (Subject<bool>) null;
    private static int globalCallCancelCount = 0;

    public static WorkQueue Worker
    {
      get
      {
        return Utils.LazyInit<WorkQueue>(ref WhatsApp.Voip.worker, (Func<WorkQueue>) (() => new WorkQueue(flags: WorkQueue.StartFlags.Unpausable | WorkQueue.StartFlags.WatchdogExcempt)));
      }
    }

    public static bool IsInCall
    {
      get
      {
        if (!WhatsApp.Voip.InCallEvent.WaitOne(0))
          return false;
        bool asyncResult = false;
        ManualResetEvent ev = new ManualResetEvent(false);
        int @ref = 2;
        Action release = (Action) (() =>
        {
          if (Interlocked.Decrement(ref @ref) != 0)
            return;
          ev.Dispose();
        });
        WhatsApp.Voip.Worker.Enqueue((Action) (() =>
        {
          try
          {
            CallInfoStruct? callInfo = WhatsApp.Voip.Instance.GetCallInfo();
            asyncResult = callInfo.HasValue && callInfo.Value.CallState != 0;
          }
          catch (Exception ex)
          {
          }
          ev.Set();
          release();
        }));
        ev.WaitOne(TimeSpan.FromSeconds(2.0));
        release();
        return asyncResult;
      }
    }

    public static void UpdateNetworkType()
    {
      WhatsApp.Voip.Worker.Enqueue((Action) (() =>
      {
        VoipNetworkType NetType = VoipNetworkType.Unknown;
        switch (AppState.GetUserConnectionType())
        {
          case ConnectionType.Disconnected:
            NetType = VoipNetworkType.None;
            break;
          case ConnectionType.Wifi:
            NetType = VoipNetworkType.Wifi;
            break;
          case ConnectionType.Cellular_2G:
          case ConnectionType.Cellular_3G:
            NetType = VoipNetworkType.Cellular;
            break;
        }
        if (NetType == VoipNetworkType.Unknown)
          return;
        WhatsApp.Voip.Instance.SetNetworkType(NetType);
      }));
    }

    private static void OnVoipInstanceCreatedBG()
    {
      DeviceNetworkInformation.NetworkAvailabilityChanged += (EventHandler<NetworkNotificationEventArgs>) ((sender, args) => WhatsApp.Voip.Worker.Enqueue((Action) (() =>
      {
        Log.p("voip", "Network change: {0}, Interface: {1}", (object) args.NotificationType.ToString(), (object) args.NetworkInterface.InterfaceName);
        switch (args.NotificationType)
        {
          case NetworkNotificationType.InterfaceConnected:
          case NetworkNotificationType.InterfaceDisconnected:
            try
            {
              WhatsApp.Voip.Instance.NotifyNetworkChange();
              WhatsApp.Voip.UpdateNetworkType();
              break;
            }
            catch (Exception ex)
            {
              break;
            }
        }
      })));
      AudioRoutingManager audioRoutingManager = AudioRoutingManager.GetDefault();
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: method pointer
      WindowsRuntimeMarshal.AddEventHandler<TypedEventHandler<AudioRoutingManager, object>>(new Func<TypedEventHandler<AudioRoutingManager, object>, EventRegistrationToken>(audioRoutingManager.add_AudioEndpointChanged), new Action<EventRegistrationToken>(audioRoutingManager.remove_AudioEndpointChanged), WhatsApp.Voip.\u003C\u003Ec.\u003C\u003E9__17_2 ?? (WhatsApp.Voip.\u003C\u003Ec.\u003C\u003E9__17_2 = new TypedEventHandler<AudioRoutingManager, object>((object) WhatsApp.Voip.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003COnVoipInstanceCreatedBG\u003Eb__17_2))));
    }

    private static void SetupUserCallbacks(IVoip instance)
    {
      IDisposable subscription = (IDisposable) null;
      if (WhatsApp.Voip.userCallbacksRegistered)
        return;
      WhatsApp.Voip.userCallbacksRegistered = true;
      try
      {
        subscription = instance.GetCallbacks().SubscribeUserCallbacks((IUserVoipCallbacks) VoipHandler.Instance, !AppState.IsBackgroundAgent);
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "Set voip user callbacks");
      }
      Action action = (Action) (() =>
      {
        lock (WhatsApp.Voip.voipInitLock)
        {
          try
          {
            subscription.SafeDispose();
            if (subscription != null)
            {
              if (Marshal.IsComObject((object) subscription))
                Marshal.ReleaseComObject((object) subscription);
            }
          }
          catch (Exception ex)
          {
          }
          subscription = (IDisposable) null;
          WhatsApp.Voip.userCallbacksRegistered = false;
        }
      });
      lock (WhatsApp.Voip.voipInitLock)
      {
        WhatsApp.Voip.outOfProcCleanupCallbacks.AddLast(action);
        if (AppState.IsBackgroundAgent)
          return;
        WhatsApp.Voip.disposeFGUserCallbacks = action;
      }
    }

    private static void OnVoipInstanceCreated(IVoip instance)
    {
      WhatsApp.Voip.SetupUserCallbacks(instance);
      if (WhatsApp.Voip.connectionEventsRegistered)
        return;
      WhatsApp.Voip.connectionEventsRegistered = true;
      if (AppState.IsBackgroundAgent)
        WhatsApp.Voip.OnVoipInstanceCreatedBG();
      FunXMPP.Connection conn = AppState.GetConnection();
      IDisposable previousSub = (IDisposable) null;
      Action dispose = (Action) (() =>
      {
        lock (WhatsApp.Voip.voipInitLock)
        {
          try
          {
            previousSub.SafeDispose();
            if (previousSub != null)
            {
              if (Marshal.IsComObject((object) previousSub))
                Marshal.ReleaseComObject((object) previousSub);
            }
          }
          catch (Exception ex)
          {
          }
          previousSub = (IDisposable) null;
        }
      });
      lock (WhatsApp.Voip.voipInitLock)
        WhatsApp.Voip.outOfProcCleanupCallbacks.AddLast(dispose);
      Action checkConn = (Action) null;
      checkConn = (Action) (() =>
      {
        if (conn == null)
          WAThreadPool.RunAfterDelay(TimeSpan.FromMilliseconds(500.0), (Action) (() =>
          {
            conn = AppState.GetConnection();
            checkConn();
          }));
        else
          conn.ConnectionStateObservable().Merge<bool>(WhatsApp.Voip.reregisterSubject.Select<Unit, bool>((Func<Unit, bool>) (_ => true))).ObserveOn<bool>((IScheduler) WhatsApp.Voip.Worker).Subscribe<bool>((Action<bool>) (connected =>
          {
            dispose();
            if (!connected)
              return;
            try
            {
              previousSub = WhatsApp.Voip.Instance.GetCallbacks().SubscribeSignalHandler((IVoipSignalingCallbacks) VoipSignaling.Instance);
            }
            catch (Exception ex)
            {
              Log.LogException(ex, "callback subscribe");
            }
          }));
      });
      checkConn();
    }

    public static void OnForegroundAppLeaving()
    {
      CallInfoStruct? nullable = new CallInfoStruct?();
      if (WhatsApp.Voip.IsInCall)
      {
        try
        {
          nullable = WhatsApp.Voip.Instance.GetCallInfo();
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "get call info on exit");
        }
      }
      if (WhatsApp.Voip.disposeFGUserCallbacks != null)
      {
        WhatsApp.Voip.disposeFGUserCallbacks();
        WhatsApp.Voip.disposeFGUserCallbacks = (Action) null;
      }
      if (nullable.HasValue && nullable.Value.CallState != CallState.None)
        WhatsApp.Voip.BgSockRequestEvent.Set();
      else
        WhatsApp.Voip.DisposeOutOfProc();
    }

    private static void DisposeOutOfProc()
    {
      lock (WhatsApp.Voip.voipInitLock)
      {
        foreach (Action action in WhatsApp.Voip.outOfProcCleanupCallbacks.AsRemoveSafeEnumerator<Action>())
          action();
        if (WhatsApp.Voip.outofProc != null)
          Marshal.ReleaseComObject((object) WhatsApp.Voip.outofProc);
        WhatsApp.Voip.outofProc = (Server) null;
      }
    }

    public static IVoip Instance
    {
      get
      {
        IVoip instance = (IVoip) null;
        if (AppState.IsBackgroundAgent)
        {
          instance = NativeInterfaces.Misc.GetVoipInstance();
        }
        else
        {
          bool flag = WhatsApp.Voip.outofProc == null;
          Server outOfProc = WhatsApp.Voip.OutOfProc;
          int num = 0;
          if (outOfProc != null)
          {
            object voipInstance;
            while (true)
            {
              try
              {
                voipInstance = outOfProc.GetVoipInstance();
                break;
              }
              catch (Exception ex)
              {
                if (num++ < 3)
                {
                  WhatsApp.Voip.DisposeOutOfProc();
                  outOfProc = WhatsApp.Voip.OutOfProc;
                }
                else
                  throw;
              }
            }
            instance = voipInstance as IVoip;
          }
          if (num != 0 && instance != null)
          {
            WhatsApp.Voip.reregisterSubject.OnNext(new Unit());
            flag = true;
          }
          if (flag)
            WhatsApp.Voip.userCallbacksRegistered = false;
          if (instance != null)
            WhatsApp.Voip.OnVoipInstanceCreated(instance);
        }
        return instance != null ? instance : throw new InvalidOperationException("Failed to initialize IVoip instance");
      }
    }

    public static UiCallState TranslateCallState(CallState state)
    {
      switch (state)
      {
        case CallState.Calling:
          return UiCallState.Calling;
        case CallState.PreacceptReceived:
          return UiCallState.Ringing;
        case CallState.ReceivedCall:
          return UiCallState.ReceivedCall;
        case CallState.AcceptSent:
        case CallState.AcceptReceived:
          return UiCallState.Connecting;
        case CallState.CallActive:
          return UiCallState.Active;
        default:
          return UiCallState.None;
      }
    }

    public static UiCallParticipantState TranslateCallParticipantState(CallParticipantDetail details)
    {
      switch (details.State)
      {
        case CallParticipantState.Connected:
        case CallParticipantState.Visible:
          return !details.RxConnecting ? UiCallParticipantState.Active : UiCallParticipantState.Reconnecting;
        case CallParticipantState.Incoming:
        case CallParticipantState.CreatingCall:
          return UiCallParticipantState.Calling;
        case CallParticipantState.Receipt:
          return !details.IsInvitedBySelf ? UiCallParticipantState.Calling : UiCallParticipantState.Ringing;
        case CallParticipantState.Rejected:
          return UiCallParticipantState.Busy;
        case CallParticipantState.Terminated:
        case CallParticipantState.Timedout:
        case CallParticipantState.CancelOffer:
          return UiCallParticipantState.Ending;
        default:
          return UiCallParticipantState.None;
      }
    }

    public static UiVideoState CallInfoToRemoteVideoState(
      CallInfoStruct info,
      CallParticipantDetail peerDetails)
    {
      if (!info.VideoEnabled)
        return UiVideoState.None;
      if (peerDetails.VideoStreamState == VideoState.Stopped)
        return UiVideoState.Stopped;
      if (peerDetails.VideoStreamState == VideoState.Paused)
        return UiVideoState.Paused;
      return !peerDetails.VideoDecodeStarted ? UiVideoState.Starting : UiVideoState.Playing;
    }

    public static UiVideoState CallInfoToLocalVideoState(
      CallInfoStruct info,
      CallParticipantDetail selfDetails)
    {
      if (!info.VideoEnabled)
        return UiVideoState.None;
      if (selfDetails.VideoStreamState == VideoState.Stopped)
        return UiVideoState.Stopped;
      return info.VideoPreviewStarted ? UiVideoState.Playing : UiVideoState.Starting;
    }

    public static UiUpgradeState CallInfoToUpgradeState(
      CallParticipantDetail selfDetails,
      CallParticipantDetail peerDetails)
    {
      UiUpgradeState upgradeState = UiUpgradeState.None;
      if (selfDetails.VideoStreamState == VideoState.UpgradeRequest)
        upgradeState = UiUpgradeState.RequestedBySelf;
      else if (peerDetails.VideoStreamState == VideoState.UpgradeRequest)
        upgradeState = UiUpgradeState.RequestedByPeer;
      return upgradeState;
    }

    public static EventWaitHandle EventForPid(uint pid)
    {
      return new EventWaitHandle(false, EventResetMode.ManualReset, "WhatsAppOutOfProc" + (object) pid);
    }

    public static Server OutOfProc
    {
      get
      {
        if (AppState.IsBackgroundAgent)
          throw new InvalidOperationException("This method is meant for the foreground app");
        return Utils.LazyInit<Server>(ref WhatsApp.Voip.outofProc, (Func<Server>) (() =>
        {
          Server outOfProc = (Server) WindowsRuntimeMarshal.GetActivationFactory(typeof (Server)).ActivateInstance();
          WhatsApp.Voip.LaunchBackgroundProc();
          return outOfProc;
        }), WhatsApp.Voip.voipInitLock, true);
      }
    }

    private static void LaunchBackgroundProc()
    {
      int backgroundProcessId;
      VoipBackgroundProcess.Launch(out backgroundProcessId);
      Log.WriteLineDebug("Voip pid is " + (object) backgroundProcessId);
      using (EventWaitHandle eventWaitHandle = WhatsApp.Voip.EventForPid((uint) backgroundProcessId))
        eventWaitHandle.WaitOne();
    }

    public static void InitBackgroundFactory()
    {
      Utils.LazyInit<WhatsApp.Voip.ManagedVoipFactory>(ref WhatsApp.Voip.managedFactory, (Func<WhatsApp.Voip.ManagedVoipFactory>) (() =>
      {
        WhatsApp.Voip.ManagedVoipFactory Fac = new WhatsApp.Voip.ManagedVoipFactory();
        NativeInterfaces.Misc.SetVoipFactory((IVoipFactory) Fac);
        return Fac;
      }));
    }

    public static IDisposable RegisterOutOfProc()
    {
      if (!AppState.IsBackgroundAgent)
        throw new InvalidOperationException("This method is meant for background agents");
      WhatsApp.Voip.InitBackgroundFactory();
      return WhatsApp.Voip.outOfProcAction.Subscribe();
    }

    public static Subject<bool> DelayedCallExecuted
    {
      get => WhatsApp.Voip.delayedCallCancelled == null ? (Subject<bool>) null : WhatsApp.Voip.delayedCallExecuted;
    }

    public static bool CancelDelayedCall()
    {
      Interlocked.Increment(ref WhatsApp.Voip.globalCallCancelCount);
      CancellationTokenSource delayedCallCancelled = WhatsApp.Voip.delayedCallCancelled;
      if (delayedCallCancelled == null)
        return false;
      Log.l("voip", "Cancellation request for delayed call");
      if (!delayedCallCancelled.IsCancellationRequested)
        delayedCallCancelled.Cancel();
      return true;
    }

    public static void StartCall(
      string selfJid,
      string peerJid,
      string callId,
      string displayName,
      bool useVideo,
      Action startCallScreen)
    {
      startCallScreen = new Action(new DisposableAction(startCallScreen).Dispose);
      Stopwatch elapsedTimer = new Stopwatch();
      elapsedTimer.Start();
      int initialCancelCount = WhatsApp.Voip.globalCallCancelCount;
      Action call = (Action) (() => WhatsApp.Voip.Worker.Enqueue((Action) (() =>
      {
        if (WhatsApp.Voip.delayedCallCancelled != null && !WhatsApp.Voip.delayedCallCancelled.IsCancellationRequested)
        {
          Log.l("voip", "Call button likely hit twice in a rapid period while call start was delayed; exiting");
        }
        else
        {
          try
          {
            CallInfoStruct? callInfo = WhatsApp.Voip.Instance.GetCallInfo();
            if (callInfo.HasValue)
            {
              if (callInfo.Value.CallState != CallState.None)
              {
                Log.l("voip", "Call button likely hit twice in a rapid period; exiting");
                return;
              }
            }
          }
          catch (Exception ex)
          {
          }
          startCallScreen();
          WhatsApp.Voip.Instance.GetCallbacks().SetPeerMetadata(displayName);
          elapsedTimer.Stop();
          int num = 0;
          try
          {
            num = Settings.CallStartDelay - (int) elapsedTimer.ElapsedMilliseconds;
          }
          catch (Exception ex)
          {
          }
          if (WhatsApp.Voip.globalCallCancelCount != initialCancelCount)
          {
            Log.l("voip", "Call was cancelled previously");
            WhatsApp.Voip.Instance.DelayedCallInterrupted((uint) elapsedTimer.ElapsedMilliseconds, peerJid);
          }
          else if (num > 0)
          {
            Log.l("voip", "Delaying call for {0} ms", (object) num);
            elapsedTimer.Start();
            Subject<bool> callExecuted = new Subject<bool>();
            CancellationTokenSource callCancelled = new CancellationTokenSource();
            CancellationToken token = callCancelled.Token;
            AutoResetEvent notexecuted = new AutoResetEvent(true);
            Action a = (Action) (() =>
            {
              if (WhatsApp.Voip.delayedCallCancelled == callCancelled)
              {
                WhatsApp.Voip.delayedCallCancelled = (CancellationTokenSource) null;
                WhatsApp.Voip.delayedCallExecuted = (Subject<bool>) null;
              }
              if (notexecuted.WaitOne(0))
              {
                elapsedTimer.Stop();
                try
                {
                  Settings.CallStartDelay = 0;
                }
                catch (Exception ex)
                {
                }
                try
                {
                  WhatsApp.Voip.Instance.StartCall(peerJid, callId, useVideo);
                  Log.l("voip", "Delayed call was executed");
                  callExecuted.OnNext(true);
                }
                catch (Exception ex)
                {
                  Log.l("voip", "Exception in delayed StartCall", (object) ex.ToString());
                  callExecuted.OnNext(false);
                }
                finally
                {
                  callExecuted.OnCompleted();
                }
              }
              else
                Log.l("voip", "Delayed call was processed before timeout");
            });
            token.Register((Action) (() =>
            {
              if (notexecuted.WaitOne(0))
              {
                Log.l("voip", "Delayed call was interrupted");
                elapsedTimer.Stop();
                callExecuted.OnNext(false);
                callExecuted.OnCompleted();
                uint elapsedMs = (uint) elapsedTimer.ElapsedMilliseconds;
                WhatsApp.Voip.Worker.Enqueue((Action) (() => WhatsApp.Voip.Instance.DelayedCallInterrupted(elapsedMs, peerJid)));
                WaCallEndedEventArgs callEndedEventArgs = new WaCallEndedEventArgs(peerJid, "")
                {
                  Reason = CallEndReason.Unknown,
                  ShouldRateCall = false,
                  RatingCookie = (byte[]) null,
                  DataUsage = new CallDataUsage()
                };
                VoipHandler.CallEndedSubject.OnNext(callEndedEventArgs);
              }
              else
                WhatsApp.Voip.Worker.Enqueue((Action) (() =>
                {
                  try
                  {
                    WhatsApp.Voip.Instance.EndCall(true);
                    Log.l("voip", "Delayed call was interrupted too late, ending call");
                  }
                  catch (Exception ex)
                  {
                    Log.l("voip", "Exception in delayed EndCall", (object) ex.ToString());
                  }
                }));
            }));
            WhatsApp.Voip.delayedCallExecuted = callExecuted;
            WhatsApp.Voip.delayedCallCancelled = callCancelled;
            WhatsApp.Voip.Worker.RunAfterDelay(TimeSpan.FromMilliseconds((double) num), a);
          }
          else
            WhatsApp.Voip.Instance.StartCall(peerJid, callId, useVideo);
        }
      })));
      AppState.GetConnection().Encryption.RecipientHasKeys(peerJid).Subscribe<bool>((Action<bool>) (k =>
      {
        if (!k)
          return;
        call();
      }));
    }

    private class OutOfProcReg : IDisposable
    {
      public OutOfProcRegistration Registration;
      public EventWaitHandle Event;
      public object @lock = new object();

      public void Dispose()
      {
        lock (this.@lock)
        {
          this.Event.Reset();
          this.Event.SafeDispose();
          this.Event = (EventWaitHandle) null;
          if (this.Registration == null)
            return;
          this.Registration.Dispose();
          this.Registration = (OutOfProcRegistration) null;
        }
      }
    }

    public class ManagedVoipFactory : IVoipFactory, IVoipCallbacks
    {
      private static IVoip instance;
      public const string LogMsft = "msft voip";
      public const string LogManaged = "voip";
      private VoipPhoneCall msftCallbacks;
      private string displayName;
      private byte[] ratingCookie;
      private string selfJid;
      private ManagedCallProperties? callProperties;
      private CallDataUsage dataUsage;
      private IDisposable callTickSub;
      private bool timedOut;
      private bool pendingAccept;
      private IUserVoipCallbacks fgUserCallbacks;
      private IUserVoipCallbacks bgUserCallbacks;
      private List<Action<IUserVoipCallbacks>> pendingUserCallbacks = new List<Action<IUserVoipCallbacks>>();
      private IVoipSignalingCallbacks signalingHandler;
      private List<Action<IVoipSignalingCallbacks>> pendingCallbacks = new List<Action<IVoipSignalingCallbacks>>();
      private const string AppName = "WHATSAPP";
      private VoipBatteryManagement batteryManagement = new VoipBatteryManagement();
      private VoipExtensions.CallLogEntry? groupCallEntry;
      private Dictionary<string, Action> errorDict = new Dictionary<string, Action>();
      private IDisposable rejectUpgradeTimerSub;
      private IDisposable cancelUpgradeTimerSub;
      private bool enableVideoPreviewStart;
      private UiCallState prevUiState;
      private DateTime? lastStartTime;
      private DateTime? lastConnectTime;
      private IDisposable onDismissSub;
      private object timerLock = new object();
      private IDisposable timerSub;
      private Action timerCallback;
      private string currentCallId;

      public IVoip Create()
      {
        return Utils.LazyInit<IVoip>(ref WhatsApp.Voip.ManagedVoipFactory.instance, (Func<IVoip>) (() =>
        {
          WhatsAppNative.Voip instance = NativeInterfaces.CreateInstance<WhatsAppNative.Voip>();
          ((IVoip) instance).SetCallbacks((IVoipCallbacks) this);
          WhatsApp.Voip.OnVoipInstanceCreated((IVoip) instance);
          return (IVoip) instance;
        }));
      }

      public void SetPeerMetadata(string displayName) => this.displayName = displayName;

      public void ClearManagedCallProperties()
      {
        this.selfJid = (string) null;
        this.callProperties = new ManagedCallProperties?();
        this.batteryManagement.Reset();
      }

      public void SetManagedCallProperties(string selfJid, ManagedCallProperties props)
      {
        this.selfJid = selfJid;
        this.callProperties = new ManagedCallProperties?(props);
        CallInfoStruct? callInfo = WhatsApp.Voip.Instance.GetCallInfo();
        if (!callInfo.HasValue || callInfo.Value.CallState != CallState.Calling || !this.lastStartTime.HasValue || props.CallerTimeout <= 0)
          return;
        double totalSeconds = (DateTime.UtcNow - this.lastStartTime.Value).TotalSeconds;
        double num = (double) props.CallerTimeout - totalSeconds;
        if (num <= 0.0 || num >= 120.0)
          return;
        this.ResetRingTimer(TimeSpan.FromSeconds(num));
      }

      private bool BadASN => this.callProperties.HasValue && this.callProperties.Value.BadASN;

      private bool ShouldRate
      {
        get => this.callProperties.HasValue && this.callProperties.Value.ShouldRateCall;
      }

      private bool ShouldUploadLogs
      {
        get => this.callProperties.HasValue && this.callProperties.Value.ShouldUploadLogs;
      }

      private bool AudioDriverErrorOccurred
      {
        get => this.callProperties.HasValue && this.callProperties.Value.AudioDriverErrorOccurred;
        set
        {
          if (!this.callProperties.HasValue)
            return;
          this.callProperties = new ManagedCallProperties?(this.callProperties.Value with
          {
            AudioDriverErrorOccurred = value
          });
        }
      }

      private int ElapsedOfferTime
      {
        get => !this.callProperties.HasValue ? 0 : this.callProperties.Value.ElapsedServerTime;
      }

      private int MsftCallActiveRetries
      {
        get => !this.callProperties.HasValue ? 0 : this.callProperties.Value.MsftCallActiveRetries;
        set
        {
          if (!this.callProperties.HasValue)
            return;
          this.callProperties = new ManagedCallProperties?(this.callProperties.Value with
          {
            MsftCallActiveRetries = value
          });
        }
      }

      public ManagedCallProperties GetManagedCallProperties(out bool hasValue)
      {
        if (this.callProperties.HasValue)
        {
          ManagedCallProperties managedCallProperties = this.callProperties.Value with
          {
            BatteryPercentChanged = this.batteryManagement.LastBatteryChangedValue ?? -1000
          };
          hasValue = true;
          return managedCallProperties;
        }
        hasValue = false;
        return new ManagedCallProperties();
      }

      public void OnRatingCookie(byte[] b) => this.ratingCookie = b;

      public void OnCallDataUsage(CallDataUsage data) => this.dataUsage = data;

      public void GetRandomBytes(out byte[] bytes) => bytes = Axolotl.GenerateRandomBytes(32);

      public void CallKeysFromCipherKeyV1(
        byte[] cipherKey,
        out byte[] callerSRTPBytes,
        out byte[] calleeSRTPBytes,
        out byte[] callerP2PBytes,
        out byte[] calleeP2PBytes)
      {
        Axolotl.CallKeysFromCipherKeyV1(cipherKey, out callerSRTPBytes, out calleeSRTPBytes, out callerP2PBytes, out calleeP2PBytes);
      }

      public void CallKeysFromCipherKeyV2(
        string jid,
        byte[] cipherKey,
        out byte[] srtpBytes,
        out byte[] p2pBytes)
      {
        if (string.IsNullOrEmpty(jid))
          jid = Settings.MyJid;
        Axolotl.CallKeysFromCipherKeyV2(jid, cipherKey, out srtpBytes, out p2pBytes);
      }

      public uint GetSecureSSRC(string callId, string jid, uint ssrcTag)
      {
        byte[] bytes1 = Encoding.UTF8.GetBytes(callId);
        byte[] bytes2 = Encoding.UTF8.GetBytes(jid);
        byte[] bytes3 = BitConverter.GetBytes(ssrcTag);
        return BitConverter.ToUInt32(HkdfSha256.Perform(4, bytes1, bytes3, bytes2), 0);
      }

      public void OnVideoFrameReceived(
        byte[] buffer,
        VideoCodec codec,
        int width,
        int height,
        long timestamp,
        bool keyframe,
        int orientation)
      {
        VideoOrientation? orientation1 = orientation > 0 ? new VideoOrientation?((VideoOrientation) orientation) : new VideoOrientation?();
        VoipVideoRenderer.OnFrameReceived(buffer, codec, width, height, timestamp, keyframe, orientation1);
      }

      public void OnCameraRestartBegin()
      {
        this.PerformUserEvent((Action<IUserVoipCallbacks>) (cb => cb.OnCameraRestartBegin()));
      }

      public void OnCameraRestartEnd()
      {
        this.PerformUserEvent((Action<IUserVoipCallbacks>) (cb => cb.OnCameraRestartEnd()));
      }

      public void OnVideoPlayerRestart()
      {
        try
        {
          this.PerformUserEvent((Action<IUserVoipCallbacks>) (cb => cb.OnVideoPlayerRestart()));
        }
        catch (Exception ex)
        {
        }
      }

      public CallApplicationSettings GetApplicationSettings()
      {
        CallApplicationSettings applicationSettings;
        applicationSettings.SelfJid = this.selfJid ?? Settings.MyJid;
        applicationSettings.DebugDirectory = Constants.IsoStorePath;
        applicationSettings.EnableAudioVideoSwitch = Settings.AudioVideoSwitchEnabled;
        return applicationSettings;
      }

      public IDisposable SubscribeUserCallbacks(IUserVoipCallbacks callbacks, bool foreground)
      {
        Action<IUserVoipCallbacks>[] array = this.pendingUserCallbacks.ToArray();
        this.pendingUserCallbacks.Clear();
        if (foreground)
        {
          this.ForegroundUserCallbacks = callbacks;
          VoipVideoRenderer.OnForegroundStarted();
          if (this.pendingAccept)
          {
            WhatsApp.Voip.Worker.Enqueue((Action) (() =>
            {
              try
              {
                WhatsApp.Voip.Instance.AcceptCall();
              }
              catch (Exception ex)
              {
                Log.LogException(ex, "AcceptCall in SubscribeUserCallbacks");
              }
            }));
            this.pendingAccept = false;
          }
        }
        else
          this.bgUserCallbacks = callbacks;
        foreach (Action<IUserVoipCallbacks> a in array)
          this.PerformUserEvent(a, false);
        return (IDisposable) new DisposableAction((Action) (() =>
        {
          if (foreground)
          {
            if (this.fgUserCallbacks == callbacks)
              this.ForegroundUserCallbacks = (IUserVoipCallbacks) null;
            VoipVideoRenderer.OnForegroundLeaving();
          }
          else
          {
            if (this.bgUserCallbacks != callbacks)
              return;
            this.bgUserCallbacks = (IUserVoipCallbacks) null;
          }
        }));
      }

      public IDisposable SubscribeSignalHandler(IVoipSignalingCallbacks callbacks)
      {
        Action<IVoipSignalingCallbacks>[] array = this.pendingCallbacks.ToArray();
        this.pendingCallbacks.Clear();
        foreach (Action<IVoipSignalingCallbacks> action in array)
          action(callbacks);
        this.signalingHandler = callbacks;
        return (IDisposable) new DisposableAction((Action) (() =>
        {
          if (this.signalingHandler != callbacks)
            return;
          this.signalingHandler = (IVoipSignalingCallbacks) null;
        }));
      }

      private IUserVoipCallbacks ForegroundUserCallbacks
      {
        get => this.fgUserCallbacks;
        set
        {
          int num = this.fgUserCallbacks == null != (value == null) ? 1 : 0;
          this.fgUserCallbacks = value;
          if (num == 0)
            return;
          this.TryStartVideoPreview();
        }
      }

      private bool ForegroundActive => this.fgUserCallbacks != null;

      private void PerformUserEvent(Action<IUserVoipCallbacks> a, bool allowDelayed = true)
      {
        if (this.fgUserCallbacks != null)
          a(this.fgUserCallbacks);
        else if (this.bgUserCallbacks != null)
          a(this.bgUserCallbacks);
        else if (allowDelayed)
        {
          Log.l("Voip callbacks", "No user callback handler available");
          this.pendingUserCallbacks.Add(a);
        }
        else
          Log.l("Voip callbacks", "No user callback, and event not delayed");
      }

      private void PerformSignalingEvent(Action<IVoipSignalingCallbacks> a)
      {
        if (this.signalingHandler != null)
          a(this.signalingHandler);
        else
          this.pendingCallbacks.Add(a);
      }

      private void NotifyCallActive()
      {
        if (this.msftCallbacks != null)
        {
          int num;
          for (num = 0; num < 3; ++num)
          {
            try
            {
              this.msftCallbacks.NotifyCallActive();
              Log.l("msft voip", "Notified call active");
              break;
            }
            catch (Exception ex)
            {
              Log.LogException(ex, nameof (NotifyCallActive));
            }
          }
          if (num <= 0)
            return;
          this.MsftCallActiveRetries = num;
        }
        else
          Log.l("msft voip", "Could not call NotifyCallActive because msft callbacks are null");
      }

      private void MsftCallback(Action<VoipPhoneCall> a, string desc)
      {
        if (this.msftCallbacks != null)
        {
          Log.l("msft voip", "Notifying event: {0}", (object) desc);
          try
          {
            a(this.msftCallbacks);
          }
          catch (Exception ex)
          {
            Log.LogException(ex, "msft callback");
          }
        }
        else
          Log.l("msft voip", "Could not notify event [{0}] because call is not active", (object) desc);
      }

      public void PlumbThroughError(
        string peerJid,
        string callId,
        CallEndReason reason,
        WhatsApp.Voip.ManagedVoipFactory.ErrorAction action)
      {
        Log.l("voip", "Publishing call end status: {0}, actions: {1}", (object) reason.ToString(), (object) action.ToString());
        bool rate = this.ShouldRate;
        bool upload = this.ShouldUploadLogs;
        byte[] cookie = this.ratingCookie ?? new byte[0];
        CallDataUsage dataUsage = this.dataUsage;
        this.ratingCookie = (byte[]) null;
        this.dataUsage = new CallDataUsage();
        if (this.prevUiState != UiCallState.Active && this.prevUiState != UiCallState.Reconnecting)
          rate = false;
        if (cookie != null && cookie.Length != 0 && (!rate || this.fgUserCallbacks == null && this.bgUserCallbacks == null))
        {
          byte[] fsCookie = cookie;
          FieldStatsRunner.FieldStatsAction((Action<IFieldStats>) (fs => fs.SubmitVoipNullRating(fsCookie)));
          cookie = new byte[0];
          rate = false;
        }
        int callRatingIntervalInSeconds = !this.callProperties.HasValue || this.callProperties.Value.CallRatingIntervalInSeconds < 0 ? 86400 : this.callProperties.Value.CallRatingIntervalInSeconds;
        Action action1 = (Action) (() =>
        {
          CallEndedNativeArgs @struct = new CallEndedNativeArgs();
          @struct.Reason = (int) reason;
          @struct.ShouldRate = rate;
          @struct.CallRatingIntervalInSeconds = callRatingIntervalInSeconds;
          @struct.ShouldUploadLogs = upload;
          @struct.AudioDriverErrorOccurred = this.AudioDriverErrorOccurred;
          @struct.DataUsage = dataUsage;
          this.PerformUserEvent((Action<IUserVoipCallbacks>) (callbacks =>
          {
            try
            {
              callbacks.OnCallEnded(peerJid, callId, cookie, @struct);
            }
            catch (Exception ex)
            {
              if (cookie != null && cookie.Length != 0)
                FieldStatsRunner.FieldStatsAction((Action<IFieldStats>) (fs => fs.SubmitVoipNullRating(cookie)));
              throw;
            }
          }));
        });
        if (reason == CallEndReason.Timeout)
          this.timedOut = true;
        if (reason == CallEndReason.PeerBusy && (action & WhatsApp.Voip.ManagedVoipFactory.ErrorAction.BusyTimeout) == WhatsApp.Voip.ManagedVoipFactory.ErrorAction.None)
        {
          this.OnUiCallState(peerJid, callId, UiCallState.Busy);
          this.BeginTimer(TimeSpan.FromSeconds(30.0), (Action) (() => this.PlumbThroughError(peerJid, callId, reason, action | WhatsApp.Voip.ManagedVoipFactory.ErrorAction.BusyTimeout)));
        }
        else
        {
          if ((action & WhatsApp.Voip.ManagedVoipFactory.ErrorAction.EndCall) != WhatsApp.Voip.ManagedVoipFactory.ErrorAction.None)
          {
            string key = CallCoalesceTable.KeyForCall(peerJid, callId);
            if (!this.errorDict.ContainsKey(key))
              this.errorDict[key] = action1;
            action1 = (Action) (() => { });
          }
          if ((action & WhatsApp.Voip.ManagedVoipFactory.ErrorAction.Reject) != WhatsApp.Voip.ManagedVoipFactory.ErrorAction.None)
          {
            try
            {
              WhatsApp.Voip.Instance.RejectCall();
            }
            catch (Exception ex)
            {
              Log.LogException(ex, "reject attempt");
            }
          }
          if ((action & WhatsApp.Voip.ManagedVoipFactory.ErrorAction.EndCall) != WhatsApp.Voip.ManagedVoipFactory.ErrorAction.None)
          {
            CallInfoStruct? callInfo = WhatsApp.Voip.Instance.GetCallInfo();
            if (!callInfo.HasValue || !callInfo.Value.IsGroupCall)
              WhatsApp.Voip.Worker.Enqueue((Action) (() =>
              {
                try
                {
                  WhatsApp.Voip.Instance.EndCall((action & WhatsApp.Voip.ManagedVoipFactory.ErrorAction.Terminate) != 0);
                }
                catch (Exception ex)
                {
                  Log.LogException(ex, "end attempt");
                }
              }));
          }
          action1();
        }
      }

      public void PlumbThroughError(
        string peerJid,
        string callId,
        string reasonStr,
        WhatsApp.Voip.ManagedVoipFactory.ErrorAction action)
      {
        this.PlumbThroughError(peerJid, callId, WhatsApp.Voip.ManagedVoipFactory.ParseCallEndReason(reasonStr), action);
      }

      public void PlumbThroughError(string peerJid, string callId, CallEndReason reason)
      {
        this.PlumbThroughError(peerJid, callId, reason, WhatsApp.Voip.ManagedVoipFactory.ErrorAction.EndCall);
      }

      public void PlumbThroughError(string peerJid, string callId, string reasonStr)
      {
        this.PlumbThroughError(peerJid, callId, WhatsApp.Voip.ManagedVoipFactory.ParseCallEndReason(reasonStr));
      }

      public void OnEvent(VoipEvent ev, string peerJid, string callId, object data)
      {
        UiCallState? nullable1 = new UiCallState?();
        Log.l("voip", "Got event: {0}", (object) ev.ToString());
        switch (ev)
        {
          case VoipEvent.CallOfferAckedWithRelayInfo:
            if (WhatsApp.Voip.Instance is ICallInfo instance1)
            {
              CallInfoStruct InfoStruct;
              instance1.GetCallInfo(out callId, out peerJid, out InfoStruct);
              this.OnOfferAckReceived(InfoStruct.EnableGroupCall);
              break;
            }
            break;
          case VoipEvent.CallAcceptFailed:
            CallInfoStruct info = WhatsApp.Voip.Instance.GetCallInfo() ?? new CallInfoStruct();
            this.PerformUserEvent((Action<IUserVoipCallbacks>) (handler => handler.OnMissedCall(peerJid ?? "", callId ?? "", 0L, this.ElapsedOfferTime, true, info.VideoEnabled)));
            this.PlumbThroughError(peerJid ?? "", callId ?? "", CallEndReason.InternalError, WhatsApp.Voip.ManagedVoipFactory.ErrorAction.EndCall | WhatsApp.Voip.ManagedVoipFactory.ErrorAction.Reject);
            break;
          case VoipEvent.CallRejectReceived:
            if (!(data is string str))
              str = "";
            string reasonStr = str;
            Log.l("voip", "reject reason: {0}", string.IsNullOrEmpty(reasonStr) ? (object) "n/a" : (object) reasonStr);
            if (string.IsNullOrEmpty(reasonStr))
            {
              this.PlumbThroughError(peerJid ?? "", callId ?? "", CallEndReason.Rejected);
              break;
            }
            this.PlumbThroughError(peerJid ?? "", callId ?? "", reasonStr);
            break;
          case VoipEvent.AudioStreamStarted:
          case VoipEvent.MuteStateChanged:
          case VoipEvent.InterruptionStateChanged:
          case VoipEvent.RxTrafficStateForPeerChanged:
          case VoipEvent.GroupParticipantLeft:
            if (WhatsApp.Voip.Instance is ICallInfo)
            {
              this.PerformUserEvent((Action<IUserVoipCallbacks>) (handler => handler.OnGroupStateChanged()));
              break;
            }
            break;
          case VoipEvent.CallStateChanged:
            if (data is ICallInfo callInfo1)
            {
              CallInfoStruct InfoStruct;
              callInfo1.GetCallInfo(out callId, out peerJid, out InfoStruct);
              this.OnCallStateChanged(peerJid, callId, InfoStruct);
              break;
            }
            break;
          case VoipEvent.MediaStreamError:
          case VoipEvent.RxTimeout:
          case VoipEvent.TxTimeout:
            this.PlumbThroughError(peerJid ?? "", callId ?? "", CallEndReason.InternalError, WhatsApp.Voip.ManagedVoipFactory.ErrorAction.EndCall | WhatsApp.Voip.ManagedVoipFactory.ErrorAction.Terminate);
            break;
          case VoipEvent.AudioInitError:
            bool flag = (bool) data && AppState.IsWP10OrLater;
            this.PlumbThroughError(peerJid ?? "", callId ?? "", flag ? CallEndReason.BadPrivacySettings : CallEndReason.InternalError, WhatsApp.Voip.ManagedVoipFactory.ErrorAction.EndCall | WhatsApp.Voip.ManagedVoipFactory.ErrorAction.Terminate);
            break;
          case VoipEvent.CallEnding:
            VoipExtensions.CallLogEntry? callLogEntry = WhatsApp.Voip.Instance.GetCallLogEntry();
            if (callLogEntry.HasValue)
            {
              this.groupCallEntry = new VoipExtensions.CallLogEntry?(callLogEntry.Value);
              Log.d("voip", "Call log entry: result = {0}, participants = {1}", (object) callLogEntry.Value.res.ToString(), (object) callLogEntry.Value.participants.Length);
              foreach (VoipExtensions.CallLogEntryParticipant participant in callLogEntry.Value.participants)
                Log.d("voip", "Call log participant: result = {0}, jid = {1}", (object) participant.res.ToString(), (object) participant.jid);
              break;
            }
            Log.d("voip", "Cannot get call log entry");
            break;
          case VoipEvent.TrafficStarted:
            if (this.prevUiState == UiCallState.Reconnecting)
            {
              try
              {
                CallInfoStruct? callInfo2 = WhatsApp.Voip.Instance.GetCallInfo();
                if (callInfo2.HasValue)
                {
                  nullable1 = new UiCallState?(WhatsApp.Voip.TranslateCallState(callInfo2.Value.CallState));
                  break;
                }
                break;
              }
              catch (Exception ex)
              {
                break;
              }
            }
            else
              break;
          case VoipEvent.TrafficStopped:
            try
            {
              ICallInfo instance2 = WhatsApp.Voip.Instance as ICallInfo;
              CallInfoStruct callInfoStruct = WhatsApp.Voip.Instance.GetCallInfo() ?? new CallInfoStruct();
              CallParticipantDetail? firstPeer = instance2.GetFirstPeer();
              if (callInfoStruct.CallState == CallState.CallActive)
              {
                if (!firstPeer.Value.RxTimedOut)
                {
                  nullable1 = new UiCallState?(UiCallState.Reconnecting);
                  break;
                }
                break;
              }
              break;
            }
            catch (Exception ex)
            {
              break;
            }
          case VoipEvent.RtcpByeReceived:
            WhatsApp.Voip.Worker.Enqueue((Action) (() => WhatsApp.Voip.Instance.EndCall(false)));
            break;
          case VoipEvent.RelayBindsFailed:
            this.PlumbThroughError(peerJid ?? "", callId ?? "", this.BadASN ? CallEndReason.YourASNBad : CallEndReason.RelayBindFailed, WhatsApp.Voip.ManagedVoipFactory.ErrorAction.EndCall | WhatsApp.Voip.ManagedVoipFactory.ErrorAction.Terminate);
            break;
          case VoipEvent.SoundPortCreated:
            try
            {
              CallInfoStruct? callInfo3 = WhatsApp.Voip.Instance.GetCallInfo();
              if (callInfo3.HasValue)
              {
                if (callInfo3.Value.VideoEnabled)
                {
                  AudioRoutingManager audioRoutingManager = AudioRoutingManager.GetDefault();
                  if (audioRoutingManager.GetAudioEndpoint() != 1)
                  {
                    if (audioRoutingManager.GetAudioEndpoint() == 3)
                    {
                      if ((audioRoutingManager.AvailableAudioEndpoints & 4) != null)
                        break;
                    }
                    else
                      break;
                  }
                  audioRoutingManager.SetAudioEndpoint((AudioRoutingEndpoint) 2);
                  break;
                }
                break;
              }
              break;
            }
            catch (Exception ex)
            {
              Log.l("voip", "Exception getting callinfo on sound port created {0}", (object) ex.ToString());
              break;
            }
          case VoipEvent.AudioDriverRestart:
            if ((AudioRestartReason) data == AudioRestartReason.RecordSilence)
            {
              this.AudioDriverErrorOccurred = true;
              break;
            }
            break;
          case VoipEvent.SelfVideoStateChanged:
          case VoipEvent.PeerVideoStateChanged:
          case VoipEvent.VideoDecodeStarted:
          case VoipEvent.VideoPreviewReady:
          case VoipEvent.VideoPreviewShouldMinimize:
          case VoipEvent.VideoRenderFormatChanged:
          case VoipEvent.VideoDecodeResumed:
            try
            {
              ICallInfo instance3 = WhatsApp.Voip.Instance as ICallInfo;
              this.HandleVideoStateChanged(callId, (WhatsApp.Voip.Instance.GetCallInfo() ?? throw new Exception("info")).Value, (instance3.GetSelf() ?? throw new Exception("self")).Value, (instance3.GetFirstPeer() ?? throw new Exception("peer")).Value, false);
              break;
            }
            catch (Exception ex)
            {
              Log.l("voip", "Exception getting callinfo on video event {0}: {1}", (object) ev.ToString(), (object) ex.ToString());
              break;
            }
          case VoipEvent.VideoCodecMismatch:
            this.PerformUserEvent((Action<IUserVoipCallbacks>) (handler => handler.OnAudioFallback(peerJid, callId)));
            break;
          case VoipEvent.VideoDecodePaused:
            try
            {
              ICallInfo instance4 = WhatsApp.Voip.Instance as ICallInfo;
              CallInfoStruct? callInfo4 = WhatsApp.Voip.Instance.GetCallInfo();
              string callId1 = callId;
              CallInfoStruct info1 = callInfo4.Value;
              CallParticipantDetail? nullable2 = instance4.GetSelf();
              CallParticipantDetail selfDetails = nullable2.Value;
              nullable2 = instance4.GetFirstPeer();
              CallParticipantDetail peerDetails = nullable2.Value;
              this.HandleVideoStateChanged(callId1, info1, selfDetails, peerDetails, true);
              break;
            }
            catch (Exception ex)
            {
              Log.l("voip", "Exception getting callinfo on video event {0}", (object) ex.ToString());
              break;
            }
          case VoipEvent.BatteryLevelLow:
            this.PerformUserEvent((Action<IUserVoipCallbacks>) (handler => handler.OnBatteryLevelLow(peerJid, callId, UiBatteryLevelSource.Self)));
            break;
          case VoipEvent.PeerBatteryLevelLow:
            this.PerformUserEvent((Action<IUserVoipCallbacks>) (handler => handler.OnBatteryLevelLow(peerJid, callId, UiBatteryLevelSource.Peer)));
            break;
          case VoipEvent.GroupInfoChanged:
            if (WhatsApp.Voip.Instance is ICallInfo)
            {
              this.PerformUserEvent((Action<IUserVoipCallbacks>) (handler => handler.OnGroupInfoChanged()));
              this.msftCallbacks.put_ContactName(WhatsApp.Voip.Instance.GetCallParticipantsDisplayName());
              break;
            }
            break;
        }
        if (!nullable1.HasValue)
          return;
        this.OnUiCallState(peerJid, callId, nullable1.Value);
      }

      private void HandleVideoStateChanged(
        string callId,
        CallInfoStruct info,
        CallParticipantDetail selfDetails,
        CallParticipantDetail peerDetails,
        bool decodePaused)
      {
        UiVideoState localVideoState = WhatsApp.Voip.CallInfoToLocalVideoState(info, selfDetails);
        UiVideoState remoteVideoState = decodePaused ? UiVideoState.Interrupted : WhatsApp.Voip.CallInfoToRemoteVideoState(info, peerDetails);
        UiUpgradeState upgradeState = WhatsApp.Voip.CallInfoToUpgradeState(selfDetails, peerDetails);
        if (peerDetails.VideoStreamState != VideoState.Disabled)
          VoipVideoRenderer.VideoCallStarted((VoipMediaStreamSource.OrientationChangedHandler) (orientation => this.HandleOrientationChanged(peerDetails.Jid, callId, orientation)));
        else
          VoipVideoRenderer.VideoCallEnded();
        if (upgradeState == UiUpgradeState.RequestedByPeer)
          this.rejectUpgradeTimerSub = PooledTimer.Instance.Schedule(TimeSpan.FromSeconds(30.0), (Action) (() => WhatsApp.Voip.Worker.Enqueue((Action) (() =>
          {
            try
            {
              WhatsApp.Voip.Instance.RejectVideoUpgrade(UpgradeRequestEndReason.Timeout);
            }
            catch (Exception ex)
            {
              Log.LogException(ex, "RejectVideoUpgrade timeout", false);
            }
          }))));
        else if (upgradeState == UiUpgradeState.RequestedBySelf)
        {
          this.cancelUpgradeTimerSub = PooledTimer.Instance.Schedule(TimeSpan.FromSeconds(45.0), (Action) (() => WhatsApp.Voip.Worker.Enqueue((Action) (() =>
          {
            try
            {
              WhatsApp.Voip.Instance.CancelVideoUpgrade(UpgradeRequestEndReason.Timeout);
            }
            catch (Exception ex)
            {
              Log.LogException(ex, "CancelUpgrade timeout", false);
            }
          }))));
        }
        else
        {
          this.rejectUpgradeTimerSub.SafeDispose();
          this.rejectUpgradeTimerSub = (IDisposable) null;
          this.cancelUpgradeTimerSub.SafeDispose();
          this.cancelUpgradeTimerSub = (IDisposable) null;
        }
        this.PerformUserEvent((Action<IUserVoipCallbacks>) (handler => handler.OnVideoStateChanged(peerDetails.Jid, callId, localVideoState, remoteVideoState, upgradeState, info.LocalCamera)));
      }

      private void HandleOrientationChanged(
        string peerJid,
        string callId,
        VideoOrientation peerOrientation)
      {
        this.PerformUserEvent((Action<IUserVoipCallbacks>) (handler => handler.OnVideoOrientationChanged(peerJid, callId, peerOrientation)));
      }

      private static CallEndReason ParseCallEndReason(string reasonStr)
      {
        CallEndReason callEndReason = CallEndReason.Unknown;
        string str = "\0";
        if (!string.IsNullOrEmpty(reasonStr))
        {
          if (reasonStr.StartsWith(str, StringComparison.Ordinal))
          {
            string s = reasonStr.Substring(str.Length);
            switch (s)
            {
              case "Error":
                callEndReason = CallEndReason.InternalError;
                break;
              case "Timeout":
                callEndReason = CallEndReason.Timeout;
                break;
              case "Reboot":
                callEndReason = CallEndReason.RebootRequired;
                break;
              case "PeerFailed":
                callEndReason = CallEndReason.PeerRelayBindFailed;
                break;
              default:
                int result;
                if (int.TryParse(s, out result))
                {
                  switch (result)
                  {
                    case 401:
                      callEndReason = CallEndReason.YourCountryNotAllowed;
                      break;
                    case 403:
                      callEndReason = CallEndReason.YourClientNotVoipCapable;
                      break;
                    case 405:
                      callEndReason = CallEndReason.PeerCountryNotAllowed;
                      break;
                    case 406:
                      callEndReason = CallEndReason.PeerBadPlatform;
                      break;
                    case 426:
                      callEndReason = CallEndReason.PeerAppTooOld;
                      break;
                    case 460:
                      callEndReason = CallEndReason.PeerOsTooOld;
                      break;
                    default:
                      if (result > 399 && result < 500)
                      {
                        callEndReason = CallEndReason.UnknownErrorCode;
                        break;
                      }
                      break;
                  }
                }
                else
                  break;
                break;
            }
          }
          else
          {
            switch (reasonStr)
            {
              case "busy":
                callEndReason = CallEndReason.PeerBusy;
                break;
              case "uncallable":
                callEndReason = CallEndReason.PeerUncallable;
                break;
              case "incompatible-srtp-key-exchange":
                callEndReason = CallEndReason.IncompatibleSrtpKeyExchange;
                break;
              case "srtp-key-generation-error":
                callEndReason = CallEndReason.SrtpKeyGenError;
                break;
              case "unsupported-audio-caps":
                callEndReason = CallEndReason.UnsupportedAudio;
                break;
              case "unavailable":
                callEndReason = CallEndReason.PeerUnavailable;
                break;
            }
          }
        }
        return callEndReason;
      }

      private bool EnableVideoPreviewStart
      {
        get => this.enableVideoPreviewStart;
        set
        {
          this.enableVideoPreviewStart = value;
          if (!value)
            return;
          this.TryStartVideoPreview();
        }
      }

      private void TryStartVideoPreview()
      {
        if (!this.EnableVideoPreviewStart || !this.ForegroundActive)
          return;
        this.EnableVideoPreviewStart = false;
        WhatsApp.Voip.Worker.Enqueue((Action) (() => WhatsApp.Voip.Instance.StartVideoPreview()));
      }

      private void OnOfferAckReceived(bool groupCallEnabled)
      {
        Log.l("voip", "Call offer ack received");
        this.PerformUserEvent((Action<IUserVoipCallbacks>) (handler => handler.OnOfferAckReceived(groupCallEnabled)));
      }

      private void OnCallStateChanged(string peerJid, string callId, CallInfoStruct callInfo)
      {
        CallState previousCallState = callInfo.PreviousCallState;
        CallState callState = callInfo.CallState;
        Log.l("voip", "call state changed: [{0}] -> [{1}]", (object) previousCallState, (object) callState);
        if (previousCallState != callState)
        {
          if (previousCallState == CallState.None)
          {
            this.lastStartTime = new DateTime?(DateTime.UtcNow);
            WhatsApp.Voip.UpdateNetworkType();
            this.batteryManagement.Start();
            if (callInfo.VideoEnabled && (callState == CallState.ReceivedCall || callState == CallState.Calling))
              this.EnableVideoPreviewStart = true;
          }
          else if (callState == CallState.CallActive)
          {
            this.lastConnectTime = new DateTime?(DateTime.UtcNow);
            this.timedOut = false;
            this.callTickSub = Observable.Interval(TimeSpan.FromMilliseconds(700.0), (IScheduler) WhatsApp.Voip.Worker).Subscribe<long>((Action<long>) (_ =>
            {
              try
              {
                WhatsApp.Voip.Instance.TimerTick();
              }
              catch (Exception ex)
              {
                Log.l("voip", "Exception from voip timer {0}", (object) ex);
              }
            }));
            if (callInfo.VideoEnabled)
              VoipVideoRenderer.VideoCallStarted((VoipMediaStreamSource.OrientationChangedHandler) (orientation => this.HandleOrientationChanged(peerJid, callId, orientation)));
          }
          switch (callState)
          {
            case CallState.None:
              try
              {
                this.EnableVideoPreviewStart = false;
                this.callTickSub.SafeDispose();
                this.callTickSub = (IDisposable) null;
                this.WriteCallRecord(peerJid, callId, this.timedOut, callInfo, this.dataUsage, this.ElapsedOfferTime, callInfo.VideoEnabled);
                VoipVideoRenderer.VideoCallEnded();
                this.lastStartTime = new DateTime?();
                this.lastConnectTime = new DateTime?();
                this.timedOut = false;
                string key = CallCoalesceTable.KeyForCall(peerJid, callId);
                Action action = (Action) null;
                this.errorDict.TryGetValue(key, out action);
                if (action != null)
                {
                  action();
                  this.errorDict.Remove(key);
                  break;
                }
                this.PlumbThroughError(peerJid, callId, CallEndReason.Unknown, WhatsApp.Voip.ManagedVoipFactory.ErrorAction.None);
                break;
              }
              catch (Exception ex)
              {
                throw;
              }
              finally
              {
                this.batteryManagement.Stop();
                ManualResetEvent statsEv = new ManualResetEvent(false);
                object l = new object();
                FieldStatsRunner.FieldStatsAction((Action) (() =>
                {
                  Log.WriteLineDebug("Confirmed field stats queue is flushed");
                  lock (l)
                    statsEv?.Set();
                }));
                this.PlayEndTone((Action) (() =>
                {
                  Log.WriteLineDebug("Checking status of field stats queue");
                  statsEv.WaitOne(TimeSpan.FromSeconds(10.0));
                  Log.WriteLineDebug("OK.");
                  this.MsftCallback((Action<VoipPhoneCall>) (m => m.NotifyCallEnded()), "Call ended");
                  this.msftCallbacks = (VoipPhoneCall) null;
                  this.onDismissSub.SafeDispose();
                  this.onDismissSub = (IDisposable) null;
                  lock (l)
                  {
                    statsEv.Dispose();
                    statsEv = (ManualResetEvent) null;
                  }
                }));
              }
            case CallState.Calling:
              this.NotifyCallStart(peerJid, callId, false, callInfo.VideoEnabled, this.displayName);
              break;
            case CallState.ReceivedCall:
              this.PerformUserEvent((Action<IUserVoipCallbacks>) (handler => handler.OnIncomingCall(peerJid, callId, callInfo.VideoEnabled)));
              break;
            case CallState.CallActive:
              this.PerformUserEvent((Action<IUserVoipCallbacks>) (handler => handler.OnCallStarted(peerJid, callId, callInfo.VideoEnabled)));
              this.NotifyCallActive();
              break;
          }
        }
        UiCallState uiState = WhatsApp.Voip.TranslateCallState(callState);
        this.OnUiCallState(peerJid, callId, uiState);
      }

      private void PlayEndTone(Action onComplete, bool onThread = false)
      {
        if (!onThread)
        {
          WhatsApp.Voip.Worker.Enqueue((Action) (() => this.PlayEndTone(onComplete, true)));
        }
        else
        {
          VoipAudioPlayer playback = (VoipAudioPlayer) null;
          Action inner = onComplete;
          onComplete = (Action) (() =>
          {
            inner();
            WhatsApp.Voip.Worker.Enqueue((Action) (() => playback.SafeDispose()));
          });
          try
          {
            playback = new VoipAudioPlayer((Func<Stream, ISoundSource>) (stream => (ISoundSource) new WavSource(stream)));
            playback.MediaEnded += (EventHandler) ((sender, args) => onComplete());
            playback.Play("file:" + NativeInterfaces.Misc.GetAppInstallDir() + "\\Sounds\\end_call.wav");
          }
          catch (Exception ex)
          {
            Log.LogException(ex, "play end tone");
            onComplete();
          }
        }
      }

      private void WriteCallRecord(
        string peerJid,
        string callId,
        bool timedOut,
        CallInfoStruct callInfo,
        CallDataUsage usage,
        int elapsedMs,
        bool hasVideo)
      {
        CallRecord rec = new CallRecord()
        {
          PeerJid = peerJid,
          CallId = callId,
          FromMe = callInfo.CallerStatus == CallerStatus.DeviceInitiated
        };
        rec.Result = callInfo.PreviousCallState != CallState.CallActive ? (!rec.FromMe ? (!callInfo.EndedByMe || timedOut ? CallRecord.CallResult.Missed : CallRecord.CallResult.Declined) : CallRecord.CallResult.Canceled) : CallRecord.CallResult.Connected;
        if (this.lastStartTime.HasValue)
          rec.StartTime = this.lastStartTime.Value;
        if (callInfo.CallActiveTime != 0L)
          rec.ConnectTime = new DateTime?(FunXMPP.UnixEpoch.AddSeconds((double) callInfo.CallActiveTime));
        else if (rec.Result == CallRecord.CallResult.Connected)
          rec.ConnectTime = new DateTime?(this.lastConnectTime.Value);
        rec.EndTime = rec.ConnectTime ?? rec.StartTime;
        if (callInfo.CallDuration > 0)
          rec.EndTime += TimeSpan.FromSeconds((double) callInfo.CallDuration);
        else if (rec.Result == CallRecord.CallResult.Connected)
          rec.EndTime = DateTime.UtcNow;
        rec.DataUsageTx = (long) this.dataUsage.BytesTransferred;
        rec.DataUsageRx = (long) this.dataUsage.BytesReceived;
        rec.VideoCall = new bool?(hasVideo);
        if (this.groupCallEntry.HasValue && ((IEnumerable<VoipExtensions.CallLogEntryParticipant>) this.groupCallEntry.Value.participants).Count<VoipExtensions.CallLogEntryParticipant>() > 0)
          rec.ParticipantEntries = ((IEnumerable<VoipExtensions.CallLogEntryParticipant>) this.groupCallEntry.Value.participants).Select<VoipExtensions.CallLogEntryParticipant, CallRecord.CallLogEntryParticipant>((Func<VoipExtensions.CallLogEntryParticipant, CallRecord.CallLogEntryParticipant>) (p => new CallRecord.CallLogEntryParticipant(p.jid, p.res))).ToList<CallRecord.CallLogEntryParticipant>();
        this.groupCallEntry = new VoipExtensions.CallLogEntry?();
        CallLog.Submit(rec);
        if (rec.Result != CallRecord.CallResult.Missed)
          return;
        this.PerformUserEvent((Action<IUserVoipCallbacks>) (handler => handler.OnMissedCall(peerJid, callId, rec.EndTime.ToFileTimeUtc(), elapsedMs, false, hasVideo)));
      }

      private void OnUiCallState(string peerJid, string callId, UiCallState uiState)
      {
        if (this.prevUiState == uiState)
          return;
        UiCallState snapPrev = this.prevUiState;
        this.PerformUserEvent((Action<IUserVoipCallbacks>) (handler => handler.OnCallStateChanged(peerJid, callId, snapPrev, uiState)));
        this.prevUiState = uiState;
      }

      public void NotifyCallStart(
        string peerJid,
        string callId,
        bool incoming,
        bool hasVideo,
        string displayName)
      {
        ((Action) (() => this.NotifyCallStartImpl(peerJid, callId, incoming, hasVideo, displayName)))();
      }

      private void NotifyCallStartImpl(
        string peerJid,
        string callId,
        bool incoming,
        bool hasVideo,
        string displayName)
      {
        // ISSUE: object of a compiler-generated type is created
        // ISSUE: variable of a compiler-generated type
        WhatsApp.Voip.ManagedVoipFactory.\u003C\u003Ec__DisplayClass88_0 cDisplayClass880 = new WhatsApp.Voip.ManagedVoipFactory.\u003C\u003Ec__DisplayClass88_0();
        // ISSUE: reference to a compiler-generated field
        cDisplayClass880.\u003C\u003E4__this = this;
        // ISSUE: reference to a compiler-generated field
        cDisplayClass880.hasVideo = hasVideo;
        // ISSUE: reference to a compiler-generated field
        cDisplayClass880.peerJid = peerJid;
        // ISSUE: reference to a compiler-generated field
        cDisplayClass880.callId = callId;
        bool flag = false;
        // ISSUE: reference to a compiler-generated field
        cDisplayClass880.useSystemUi = incoming;
        if (this.msftCallbacks != null)
        {
          Log.l("msft voip", "NotifyCallStartImpl: Call already created");
        }
        else
        {
          if (this.ForegroundActive)
          {
            if (incoming)
              flag = true;
            // ISSUE: reference to a compiler-generated field
            cDisplayClass880.useSystemUi = false;
          }
          // ISSUE: reference to a compiler-generated field
          WaUriParams uriParams = WaUriParams.ForCallScreen(cDisplayClass880.peerJid);
          uriParams.AddString("clr2", "ContactsPage");
          // ISSUE: reference to a compiler-generated field
          uriParams.AddBool("VideoCall", cDisplayClass880.hasVideo);
          string pageUriStr = UriUtils.CreatePageUriStr("CallScreenPage", uriParams);
          VoipCallCoordinator voipCallCoordinator1 = VoipCallCoordinator.GetDefault();
          // ISSUE: reference to a compiler-generated field
          if (cDisplayClass880.useSystemUi)
          {
            // ISSUE: object of a compiler-generated type is created
            // ISSUE: variable of a compiler-generated type
            WhatsApp.Voip.ManagedVoipFactory.\u003C\u003Ec__DisplayClass88_1 cDisplayClass881 = new WhatsApp.Voip.ManagedVoipFactory.\u003C\u003Ec__DisplayClass88_1();
            // ISSUE: reference to a compiler-generated field
            cDisplayClass881.CS\u0024\u003C\u003E8__locals1 = cDisplayClass880;
            VoipHandler.OnSystemIncomingCallUiRaised();
            string[] array = WhatsApp.Voip.Instance.GetCallPeers().Select<CallParticipantDetail, string>((Func<CallParticipantDetail, string>) (p => p.Jid)).ToArray<string>();
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            Uri uri1 = new Uri(array.Length > 1 ? VoipPictureStore.GetVoipContactsPhotoPath(array) : VoipPictureStore.GetVoipContactPhotoPath(cDisplayClass881.CS\u0024\u003C\u003E8__locals1.peerJid), UriKind.Absolute);
            if (array.Length > 1)
              VoipPictureStore.EnsureVoipContactsPhoto(array);
            Uri uri2 = AppState.IsWP10OrLater ? new Uri(NativeInterfaces.Misc.GetAppInstallDir() + "\\Icon1.png", UriKind.Absolute) : new Uri(NativeInterfaces.Misc.GetAppInstallDir() + "\\Icon0.png", UriKind.Absolute);
            Uri uri3 = (Uri) null;
            Uri uri4 = (Uri) null;
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            string ringtonePath = Ringtones.GetRingtonePath(cDisplayClass881.CS\u0024\u003C\u003E8__locals1.peerJid);
            if (ringtonePath != null)
              uri4 = new Uri(string.Format("{0}\\{1}", (object) NativeInterfaces.Misc.GetAppInstallDir(), (object) ringtonePath, (object) UriKind.Absolute));
            if (!AppState.IsWP10OrLater)
            {
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              displayName = UserCache.Get(cDisplayClass881.CS\u0024\u003C\u003E8__locals1.peerJid, false).GetDisplayName(true);
              displayName = displayName.Length <= 7 ? displayName : displayName.Substring(0, 7) + "...";
              if (array.Length > 1)
                displayName = displayName + " +" + (object) (array.Length - 1);
            }
            try
            {
              VoipCallCoordinator voipCallCoordinator2 = voipCallCoordinator1;
              string str1 = pageUriStr;
              string str2 = displayName;
              Uri uri5 = uri1;
              Uri uri6 = uri2;
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              string str3 = WhatsApp.Voip.Instance.GetCallPeers().Count > 1 ? AppResources.CallScreenLabelIncomingGroupWACall : (cDisplayClass881.CS\u0024\u003C\u003E8__locals1.hasVideo ? AppResources.CallScreenLabelIncomingCall : AppResources.CallScreenLabelIncomingWACall);
              Uri uri7 = uri4;
              if ((object) uri7 == null)
                uri7 = uri3;
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              int num = cDisplayClass881.CS\u0024\u003C\u003E8__locals1.hasVideo ? 3 : 1;
              TimeSpan timeSpan = TimeSpan.FromSeconds(45.0);
              ref VoipPhoneCall local = ref this.msftCallbacks;
              voipCallCoordinator2.RequestNewIncomingCall(str1, str2, "", uri5, "WHATSAPP", uri6, str3, uri7, (VoipCallMedia) num, timeSpan, ref local);
            }
            catch (Exception ex)
            {
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              this.OnBrokerException(ex, cDisplayClass881.CS\u0024\u003C\u003E8__locals1.peerJid, cDisplayClass881.CS\u0024\u003C\u003E8__locals1.callId, incoming);
              return;
            }
            // ISSUE: reference to a compiler-generated field
            cDisplayClass881.dt = DateTime.UtcNow;
            // ISSUE: reference to a compiler-generated field
            cDisplayClass881.dismissCall = new DisposableAction((Action) (() => VoipHandler.OnSystemIncomingCallUiDismissed()));
            this.onDismissSub.SafeDispose();
            // ISSUE: reference to a compiler-generated field
            this.onDismissSub = (IDisposable) cDisplayClass881.dismissCall;
            VoipPhoneCall msftCallbacks1 = this.msftCallbacks;
            // ISSUE: method pointer
            WindowsRuntimeMarshal.AddEventHandler<TypedEventHandler<VoipPhoneCall, CallAnswerEventArgs>>(new Func<TypedEventHandler<VoipPhoneCall, CallAnswerEventArgs>, EventRegistrationToken>(msftCallbacks1.add_AnswerRequested), new Action<EventRegistrationToken>(msftCallbacks1.remove_AnswerRequested), new TypedEventHandler<VoipPhoneCall, CallAnswerEventArgs>((object) cDisplayClass881, __methodptr(\u003CNotifyCallStartImpl\u003Eb__2)));
            VoipPhoneCall msftCallbacks2 = this.msftCallbacks;
            // ISSUE: method pointer
            WindowsRuntimeMarshal.AddEventHandler<TypedEventHandler<VoipPhoneCall, CallRejectEventArgs>>(new Func<TypedEventHandler<VoipPhoneCall, CallRejectEventArgs>, EventRegistrationToken>(msftCallbacks2.add_RejectRequested), new Action<EventRegistrationToken>(msftCallbacks2.remove_RejectRequested), new TypedEventHandler<VoipPhoneCall, CallRejectEventArgs>((object) cDisplayClass881, __methodptr(\u003CNotifyCallStartImpl\u003Eb__3)));
          }
          else
          {
            try
            {
              // ISSUE: reference to a compiler-generated field
              voipCallCoordinator1.RequestNewOutgoingCall(pageUriStr, displayName, "WHATSAPP", cDisplayClass880.hasVideo ? (VoipCallMedia) 3 : (VoipCallMedia) 1, ref this.msftCallbacks);
            }
            catch (Exception ex)
            {
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              this.OnBrokerException(ex, cDisplayClass880.peerJid, cDisplayClass880.callId, incoming);
              return;
            }
            // ISSUE: reference to a compiler-generated method
            this.BeginTimer(TimeSpan.FromSeconds(45.0), new Action(cDisplayClass880.\u003CNotifyCallStartImpl\u003Eb__5));
          }
          VoipPhoneCall msftCallbacks3 = this.msftCallbacks;
          // ISSUE: method pointer
          WindowsRuntimeMarshal.AddEventHandler<TypedEventHandler<VoipPhoneCall, CallStateChangeEventArgs>>(new Func<TypedEventHandler<VoipPhoneCall, CallStateChangeEventArgs>, EventRegistrationToken>(msftCallbacks3.add_EndRequested), new Action<EventRegistrationToken>(msftCallbacks3.remove_EndRequested), new TypedEventHandler<VoipPhoneCall, CallStateChangeEventArgs>((object) cDisplayClass880, __methodptr(\u003CNotifyCallStartImpl\u003Eb__6)));
          // ISSUE: reference to a compiler-generated field
          cDisplayClass880.setHold = (Action<bool, Action>) ((b, a) =>
          {
            Log.l("msft voip", "OnHold[{0}] requested", (object) b);
            try
            {
              WhatsApp.Voip.Instance.SetOnHold(b);
              a();
            }
            catch (Exception ex)
            {
              string context = string.Format("onHold[{0}]", (object) b);
              Log.LogException(ex, context);
            }
          });
          VoipPhoneCall msftCallbacks4 = this.msftCallbacks;
          // ISSUE: method pointer
          WindowsRuntimeMarshal.AddEventHandler<TypedEventHandler<VoipPhoneCall, CallStateChangeEventArgs>>(new Func<TypedEventHandler<VoipPhoneCall, CallStateChangeEventArgs>, EventRegistrationToken>(msftCallbacks4.add_HoldRequested), new Action<EventRegistrationToken>(msftCallbacks4.remove_HoldRequested), new TypedEventHandler<VoipPhoneCall, CallStateChangeEventArgs>((object) cDisplayClass880, __methodptr(\u003CNotifyCallStartImpl\u003Eb__8)));
          VoipPhoneCall msftCallbacks5 = this.msftCallbacks;
          // ISSUE: method pointer
          WindowsRuntimeMarshal.AddEventHandler<TypedEventHandler<VoipPhoneCall, CallStateChangeEventArgs>>(new Func<TypedEventHandler<VoipPhoneCall, CallStateChangeEventArgs>, EventRegistrationToken>(msftCallbacks5.add_ResumeRequested), new Action<EventRegistrationToken>(msftCallbacks5.remove_ResumeRequested), new TypedEventHandler<VoipPhoneCall, CallStateChangeEventArgs>((object) cDisplayClass880, __methodptr(\u003CNotifyCallStartImpl\u003Eb__9)));
          if (!flag)
            return;
          Log.l("msft voip", "Notifying call start early since we were inititiated from the UI app");
          this.NotifyCallActive();
        }
      }

      private void OnBrokerException(Exception ex, string peerJid, string callId, bool incoming)
      {
        Log.LogException(ex, "exception starting call on MSFT side");
        this.PlumbThroughError(peerJid, callId, CallEndReason.RebootRequired, (WhatsApp.Voip.ManagedVoipFactory.ErrorAction) ((incoming ? 4 : 8) | 1));
      }

      private void ResetRingTimer(TimeSpan timeout) => this.BeginTimer(timeout, this.timerCallback);

      private void BeginTimer(TimeSpan timeout, Action a)
      {
        lock (this.timerLock)
        {
          this.timerSub.SafeDispose();
          this.timerCallback = a;
          this.timerSub = PooledTimer.Instance.Schedule(timeout, (Action) (() =>
          {
            Action action = (Action) null;
            lock (this.timerLock)
            {
              action = this.timerCallback;
              this.timerCallback = (Action) null;
            }
            if (action == null)
              return;
            action();
          }));
        }
      }

      public void CancelRingTimers()
      {
        lock (this.timerLock)
        {
          this.timerSub.SafeDispose();
          this.timerSub = (IDisposable) null;
          this.timerCallback = (Action) null;
        }
      }

      public bool IsMsftCallbackRegistered() => this.msftCallbacks != null;

      public void OnSignalingData(byte[] buffer, SignalingDataArgs args)
      {
        this.PerformSignalingEvent((Action<IVoipSignalingCallbacks>) (handler => handler.OnSignalingData(buffer, args)));
      }

      public bool WasParticipantInvited
      {
        get
        {
          string callId;
          return WhatsApp.Voip.Instance.GetCallInfo(out callId, out string _, out CallInfoStruct _) && callId == this.currentCallId;
        }
        set
        {
          string callId;
          if (!value || !WhatsApp.Voip.Instance.GetCallInfo(out callId, out string _, out CallInfoStruct _))
            return;
          this.currentCallId = callId;
        }
      }

      [Flags]
      public enum ErrorAction
      {
        None = 0,
        EndCall = 1,
        Reject = 4,
        Terminate = 8,
        BusyTimeout = 16, // 0x00000010
      }
    }
  }
}
