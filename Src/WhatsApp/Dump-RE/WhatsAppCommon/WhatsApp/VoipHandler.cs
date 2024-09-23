// Decompiled with JetBrains decompiler
// Type: WhatsApp.VoipHandler
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Windows;
using WhatsApp.Events;
using WhatsApp.WaCollections;
using WhatsAppNative;

#nullable disable
namespace WhatsApp
{
  public class VoipHandler : IUserVoipCallbacks
  {
    private const string LogHeader = "voiphandler";
    internal const string PrivateReasonPrefix = "\0";
    private static VoipHandler inst;
    public static Subject<WaCallEventArgs> IncomingCallSubject = new Subject<WaCallEventArgs>();
    public static Subject<Unit> CallStartedSubject = new Subject<Unit>();
    public static Subject<WaCallVideoStateChangedArgs> VideoStateChanged = new Subject<WaCallVideoStateChangedArgs>();
    public static Subject<WaCallVideoOrientationChangedArgs> VideoOrientationChanged = new Subject<WaCallVideoOrientationChangedArgs>();
    public static Subject<Unit> VideoRestarted = new Subject<Unit>();
    public static Subject<bool> CameraRestartSubject = new Subject<bool>();
    public static ReplaySubject<bool> PeerMutedSubject = new ReplaySubject<bool>();
    public static Subject<WaCallEndedEventArgs> CallEndedSubject = new Subject<WaCallEndedEventArgs>();
    public static Subject<bool> OfferAckReceivedSubject = new Subject<bool>();
    public static Subject<WaCallStateChangedArgs> CallStateChangedSubject = new Subject<WaCallStateChangedArgs>();
    public static Subject<WaCallBatteryLevelLowArgs> BatteryLevelChanged = new Subject<WaCallBatteryLevelLowArgs>();
    public static Subject<Unit> AudioFallbackSubject = new Subject<Unit>();
    public static Subject<WAGroupCallChangedArgs> GroupInfoChanged = new Subject<WAGroupCallChangedArgs>();
    public static Subject<Unit> GroupStateChanged = new Subject<Unit>();
    private static object incomingCallAgentLock = new object();
    private static LinkedList<RefCountAction> incomingCallAgentSubs = new LinkedList<RefCountAction>();
    private static LinkedList<IDisposable> incomingCallAgentDisposables = new LinkedList<IDisposable>();
    private static bool incomingCallUiUp = false;

    public static VoipHandler Instance
    {
      get
      {
        return Utils.LazyInit<VoipHandler>(ref VoipHandler.inst, (Func<VoipHandler>) (() => new VoipHandler()));
      }
    }

    public void OnCallEnded(
      string peerJid,
      string callId,
      byte[] cookie,
      CallEndedNativeArgs args)
    {
      CallEndReason reason = (CallEndReason) args.Reason;
      bool flag = args.ShouldRate;
      DateTime? callRatingTimeUtc = Settings.LastShowCallRatingTimeUtc;
      if (callRatingTimeUtc.HasValue & flag)
      {
        double totalSeconds = (DateTime.UtcNow - callRatingTimeUtc.Value).TotalSeconds;
        flag = totalSeconds < 0.0 || totalSeconds >= (double) args.CallRatingIntervalInSeconds;
      }
      VoipHandler.PeerMutedSubject.OnNext(false);
      if (!flag)
        cookie = (byte[]) null;
      WaCallEndedEventArgs callEndedEventArgs = new WaCallEndedEventArgs(peerJid, callId)
      {
        Reason = reason,
        ShouldRateCall = flag,
        RatingCookie = cookie,
        DataUsage = args.DataUsage
      };
      VoipHandler.CallEndedSubject.OnNext(callEndedEventArgs);
      if (callEndedEventArgs.ShouldRateCall && AppState.IsBackgroundAgent)
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          db.InsertWaScheduledTaskOnSubmit(this.CreateCallRatingTask(peerJid, callId, cookie));
          db.SubmitChanges();
        }));
      if (args.AudioDriverErrorOccurred)
        Log.SendCrashLog((Exception) new ArgumentException("Call audio issue"), "No audio from driver");
      if (!args.ShouldUploadLogs)
        return;
      switch (reason)
      {
        case CallEndReason.Unknown:
        case CallEndReason.InternalError:
        case CallEndReason.UnknownErrorCode:
        case CallEndReason.IncompatibleSrtpKeyExchange:
        case CallEndReason.SrtpKeyGenError:
        case CallEndReason.UnsupportedAudio:
        case CallEndReason.RebootRequired:
        case CallEndReason.PeerRelayBindFailed:
        case CallEndReason.YourASNBad:
        case CallEndReason.RelayBindFailed:
        case CallEndReason.BadPrivacySettings:
          Log.SendSupportLog((string) null);
          break;
        case CallEndReason.Rejected:
          break;
        case CallEndReason.Timeout:
          break;
        case CallEndReason.YourCountryNotAllowed:
          break;
        case CallEndReason.PeerCountryNotAllowed:
          break;
        case CallEndReason.YourClientNotVoipCapable:
          break;
        case CallEndReason.PeerUnavailable:
          break;
        case CallEndReason.PeerAppTooOld:
          break;
        case CallEndReason.PeerOsTooOld:
          break;
        case CallEndReason.PeerBadPlatform:
          break;
        case CallEndReason.PeerBusy:
          break;
        case CallEndReason.PeerUncallable:
          break;
        default:
          Log.l("voiphandler", "unexpected call end reason: {0}", (object) reason);
          break;
      }
    }

    private WaScheduledTask CreateCallRatingTask(string peerJid, string callId, byte[] fsCookie)
    {
      BinaryData binaryData = new BinaryData();
      binaryData.AppendStrWithLengthPrefix(peerJid);
      binaryData.AppendStrWithLengthPrefix(callId);
      long unixTime = DateTime.UtcNow.AddMinutes(2.0).ToUnixTime();
      binaryData.AppendLong64(unixTime);
      binaryData.AppendBytesWithLengthPrefix(fsCookie);
      return new WaScheduledTask(WaScheduledTask.Types.RateCall, callId, binaryData.Get(), WaScheduledTask.Restrictions.FgOnly, new TimeSpan?(TimeSpan.FromDays(30.0)));
    }

    public static IObservable<Unit> PerformScheduledCallRating(
      WaScheduledTask task,
      bool skipRatingPrompt)
    {
      return task.TaskType != 4 ? Observable.Empty<Unit>() : Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        BinaryData binaryData = new BinaryData(task.BinaryData);
        int newOffset1 = 0;
        string peerJid = binaryData.ReadStrWithLengthPrefix(newOffset1, out newOffset1);
        string callId = binaryData.ReadStrWithLengthPrefix(newOffset1, out newOffset1);
        long seconds = binaryData.ReadLong64(newOffset1);
        int newOffset2 = newOffset1 + 8;
        byte[] fsCookie = binaryData.ReadBytesWithLengthPrefix(newOffset2, out newOffset2);
        DateTime dateTime = DateTimeUtils.FromUnixTime(seconds);
        if (skipRatingPrompt || dateTime < DateTime.UtcNow)
        {
          Log.l("voiphandler", "submit fs without rating | peer:{0},call:{1}", (object) peerJid, (object) callId);
          FieldStatsRunner.FieldStatsAction((Action<IFieldStats>) (fs =>
          {
            fs.SubmitVoipNullRating(fsCookie);
            Log.l("voiphandler", "submit fs | complete | peer:{0},call:{1}", (object) peerJid, (object) callId);
          }));
        }
        else
        {
          Log.l("voiphandler", "prompt call rating | peer:{0},call:{1}", (object) peerJid, (object) callId);
          AppState.ClientInstance.PromptRateCall(peerJid, fsCookie);
        }
        observer.OnNext(new Unit());
        observer.OnCompleted();
        return (Action) (() => { });
      }));
    }

    public void OnMissedCall(
      string jid,
      string callId,
      long time,
      int elapsedMs,
      bool newRecord,
      bool hasVideo)
    {
      DateTime callTime = time != 0L ? DateTime.FromFileTimeUtc(time) - TimeSpan.FromSeconds((double) FunRunner.LastKnownTimeSkew) : FunRunner.CurrentServerTimeUtc;
      if (string.IsNullOrEmpty(jid) || string.IsNullOrEmpty(callId))
        return;
      if (newRecord)
        CallLog.Submit(new CallRecord()
        {
          PeerJid = jid,
          CallId = callId,
          FromMe = false,
          StartTime = callTime,
          EndTime = time != 0L ? callTime + TimeSpan.FromSeconds(45.0) : callTime,
          Result = CallRecord.CallResult.Missed,
          VideoCall = new bool?(hasVideo)
        });
      if (AppState.IsBackgroundAgent)
      {
        UserStatus us = (UserStatus) null;
        ContactsContext.Instance((Action<ContactsContext>) (cdb =>
        {
          us = cdb.GetUserStatus(jid);
          ContactsContext.Reset(true);
        }));
        string displayName = us.GetDisplayName();
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          TileHelper.IncrementTiles(db, TileDataSource.CreateForMissedCall(jid, displayName));
          MessagesContext.Reset(true);
        }));
        PushSystem.Instance.ShellToastEx(new string[2]
        {
          AppResources.MissedCallNotification,
          displayName
        }, "calls", "/PageSelect", false);
      }
      new Call()
      {
        callSide = new wam_enum_call_side?(wam_enum_call_side.CALLEE),
        callResult = new wam_enum_call_result_type?(wam_enum_call_result_type.MISSED),
        callOfferElapsedT = new long?((long) elapsedMs)
      }.SaveEvent();
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        Message missedCall = SystemMessageUtils.CreateMissedCall((SqliteMessagesContext) db, jid, callTime, hasVideo);
        db.InsertMessageOnSubmit(missedCall);
        db.SubmitChanges();
      }));
    }

    public void OnCallStarted(string peerJid, string callId, bool videoEnabled)
    {
      VoipHandler.CallStartedSubject.OnNext(new Unit());
    }

    public void OnVideoStateChanged(
      string peerJid,
      string callId,
      UiVideoState localVideoState,
      UiVideoState remoteVideoState,
      UiUpgradeState upgradeState,
      CameraInformation localCamera)
    {
      VoipHandler.VideoStateChanged.OnNext(new WaCallVideoStateChangedArgs(peerJid, callId, localVideoState, remoteVideoState, upgradeState, localCamera));
    }

    public void OnVideoOrientationChanged(
      string peerJid,
      string callid,
      VideoOrientation remoteOrientation)
    {
      VoipHandler.VideoOrientationChanged.OnNext(new WaCallVideoOrientationChangedArgs(peerJid, callid, remoteOrientation));
    }

    public void OnVideoPlayerRestart() => VoipHandler.VideoRestarted.OnNext(new Unit());

    public void OnCameraRestartBegin() => VoipHandler.CameraRestartSubject.OnNext(false);

    public void OnCameraRestartEnd() => VoipHandler.CameraRestartSubject.OnNext(true);

    public void OnIncomingCall(string jid, string callId, bool hasVideo)
    {
      Log.d("voiphandler", nameof (OnIncomingCall));
      WAThreadPool.QueueUserWorkItem((Action) (() =>
      {
        ContactsContext.Instance<UserStatus>((Func<ContactsContext, UserStatus>) (db => db.GetUserStatus(jid)));
        Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() =>
        {
          Log.d("voiphandler", "OnIncomingCall - EnsureVoipContactPhoto");
          VoipPictureStore.EnsureVoipContactPhoto(jid);
        }));
        string displayName = Voip.Instance.GetCallParticipantsDisplayName(true);
        Voip.Worker.Enqueue((Action) (() =>
        {
          Log.d("voiphandler", "OnIncomingCall - NotifyCallStart");
          Voip.Instance.GetCallbacks().NotifyCallStart(jid, callId, true, hasVideo, displayName);
        }), WorkQueue.Priority.Interrupt);
        VoipHandler.IncomingCallSubject.OnNext(new WaCallEventArgs(jid, callId));
      }));
    }

    public void OnOfferAckReceived(bool groupCallEnabled)
    {
      VoipHandler.OfferAckReceivedSubject.OnNext(groupCallEnabled);
    }

    public void OnCallStateChanged(
      string peerJid,
      string callId,
      UiCallState oldState,
      UiCallState newState)
    {
      VoipHandler.CallStateChangedSubject.OnNext(new WaCallStateChangedArgs(peerJid, callId, oldState, newState));
    }

    public void OnBatteryLevelLow(string peerJid, string callId, UiBatteryLevelSource Source)
    {
      if (AppState.IsBackgroundAgent)
        return;
      VoipHandler.BatteryLevelChanged.OnNext(new WaCallBatteryLevelLowArgs(peerJid, callId, Source));
    }

    public void OnAudioFallback(string peerJid, string callId)
    {
      if (AppState.IsBackgroundAgent)
        return;
      VoipHandler.AudioFallbackSubject.OnNext(new Unit());
    }

    public void OnGroupInfoChanged()
    {
      if (AppState.IsBackgroundAgent)
        return;
      Voip.Worker.Enqueue((Action) (() =>
      {
        try
        {
          List<CallParticipantDetail> callPeers = Voip.Instance.GetCallPeers();
          VoipHandler.GroupInfoChanged.OnNext(new WAGroupCallChangedArgs(callPeers));
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "group info changed.");
        }
      }));
    }

    public void OnGroupStateChanged()
    {
      if (AppState.IsBackgroundAgent)
        return;
      Voip.Worker.Enqueue((Action) (() =>
      {
        try
        {
          VoipHandler.GroupStateChanged.OnNext(new Unit());
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "group state changed.");
        }
      }));
    }

    public static IDisposable AddIncomingCallSub(RefCountAction sub)
    {
      LinkedListNode<RefCountAction> node = (LinkedListNode<RefCountAction>) null;
      LinkedListNode<IDisposable> dispNode = (LinkedListNode<IDisposable>) null;
      lock (VoipHandler.incomingCallAgentLock)
      {
        node = VoipHandler.incomingCallAgentSubs.AddLast(sub);
        if (VoipHandler.incomingCallUiUp)
          dispNode = VoipHandler.incomingCallAgentDisposables.AddLast(sub.Subscribe());
      }
      return (IDisposable) new DisposableAction((Action) (() =>
      {
        lock (VoipHandler.incomingCallAgentLock)
        {
          node?.List.Remove(node);
          dispNode?.List.Remove(dispNode);
        }
      }));
    }

    public static void OnSystemIncomingCallUiRaised()
    {
      lock (VoipHandler.incomingCallAgentLock)
      {
        VoipHandler.incomingCallUiUp = true;
        foreach (RefCountAction refCountAction in VoipHandler.incomingCallAgentSubs.AsRemoveSafeEnumerator<RefCountAction>())
          VoipHandler.incomingCallAgentDisposables.AddLast(refCountAction.Subscribe());
      }
    }

    public static void OnSystemIncomingCallUiDismissed()
    {
      lock (VoipHandler.incomingCallAgentLock)
      {
        foreach (IDisposable disposable in VoipHandler.incomingCallAgentDisposables.AsRemoveSafeEnumerator<IDisposable>())
          disposable.Dispose();
        VoipHandler.incomingCallAgentDisposables.Clear();
        VoipHandler.incomingCallUiUp = false;
      }
    }
  }
}
