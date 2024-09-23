// Decompiled with JetBrains decompiler
// Type: WhatsApp.CallScreenHelper
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using WhatsAppNative;

#nullable disable
namespace WhatsApp
{
  public class CallScreenHelper : WaDisposable
  {
    private DateTime? lastButtonTapTime;
    private bool? presetMuteState;
    private IDisposable muteSub;
    private object muteSubLock = new object();

    private string LogHeader => "callscreen";

    public void EnqueueVoipWork(Action<IVoip> a, Action onComplete = null)
    {
      if (onComplete == null)
        onComplete = (Action) (() => { });
      this.PerformVoipWorkObservable(a).Take<bool>(1).Subscribe<bool>((Action<bool>) (_ => { }), onComplete);
    }

    public IObservable<bool> PerformVoipWorkObservable(Action<IVoip> a)
    {
      return Observable.Create<bool>((Func<IObserver<bool>, Action>) (observer =>
      {
        Voip.Worker.Enqueue((Action) (() =>
        {
          Subject<bool> delayedCallExecuted = Voip.DelayedCallExecuted;
          if (delayedCallExecuted == null)
            this.PerformVoipWorkObservableNoDelay(a).Subscribe(observer);
          else
            delayedCallExecuted.Subscribe<bool>((Action<bool>) (success =>
            {
              if (success)
              {
                this.PerformVoipWorkObservableNoDelay(a).Subscribe(observer);
              }
              else
              {
                observer.OnNext(false);
                observer.OnCompleted();
              }
            }));
        }));
        return (Action) (() => { });
      }));
    }

    private IObservable<bool> PerformVoipWorkObservableNoDelay(Action<IVoip> a)
    {
      return Observable.Create<bool>((Func<IObserver<bool>, Action>) (observer =>
      {
        IVoip instance = Voip.Instance;
        CallInfoStruct? callInfo = instance.GetCallInfo();
        if (callInfo.HasValue)
        {
          if (callInfo.Value.CallState != CallState.None)
          {
            try
            {
              a(instance);
              observer.OnNext(true);
              goto label_5;
            }
            catch (Exception ex)
            {
              Log.LogException(ex, "call screen page VOIP work");
              observer.OnNext(false);
              goto label_5;
            }
          }
        }
        Log.l(this.LogHeader, "voip call not available");
        observer.OnNext(false);
label_5:
        observer.OnCompleted();
        return (Action) (() => { });
      }));
    }

    public IObservable<int> GetDuration()
    {
      return Observable.Create<int>((Func<IObserver<int>, Action>) (observer =>
      {
        this.EnqueueVoipWork((Action<IVoip>) (voip =>
        {
          CallInfoStruct? callInfo = voip.GetCallInfo();
          if (!callInfo.HasValue || callInfo.Value.CallState != CallState.CallActive)
            return;
          observer.OnNext(callInfo.Value.CallDuration);
        }), (Action) (() => observer.OnCompleted()));
        return (Action) (() => { });
      }));
    }

    public IObservable<bool> GetMuteState(UiCallState currState)
    {
      return currState != UiCallState.Active ? Observable.Return<bool>(((int) this.presetMuteState ?? 0) != 0) : Observable.Create<bool>((Func<IObserver<bool>, Action>) (observer =>
      {
        this.EnqueueVoipWork((Action<IVoip>) (voip =>
        {
          try
          {
            observer.OnNext(voip.IsMuted());
          }
          catch (Exception ex)
          {
            Log.LogException(ex, "get mute state");
          }
        }), (Action) (() => observer.OnCompleted()));
        return (Action) (() => { });
      }));
    }

    public IObservable<bool> SyncPresetMuteState(UiCallState callState)
    {
      string logHeader = this.LogHeader;
      object[] objArray = new object[1];
      bool lockTaken;
      string str;
      if (!this.presetMuteState.HasValue)
      {
        str = "n/a";
      }
      else
      {
        lockTaken = this.presetMuteState.Value;
        str = lockTaken.ToString();
      }
      objArray[0] = (object) str;
      Log.l(logHeader, "sync preset mute state | {0}", objArray);
      if (!this.presetMuteState.HasValue)
        return callState == UiCallState.Active ? this.GetMuteState(callState) : Observable.Empty<bool>();
      bool toMute = this.presetMuteState.Value;
      this.presetMuteState = new bool?();
      object muteSubLock = this.muteSubLock;
      lockTaken = false;
      try
      {
        Monitor.Enter(muteSubLock, ref lockTaken);
        if (this.muteSub != null)
        {
          Log.l(this.LogHeader, "sync preset mute state | skipped");
          return Observable.Empty<bool>();
        }
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(muteSubLock);
      }
      return Observable.Create<bool>((Func<IObserver<bool>, Action>) (observer =>
      {
        this.EnqueueVoipWork((Action<IVoip>) (voip =>
        {
          try
          {
            voip.SetMute(toMute);
            observer.OnNext(toMute);
            Log.l(this.LogHeader, "mute state synced to {0}", (object) toMute);
          }
          catch (Exception ex3)
          {
            Log.LogException(ex3, "voip mute");
            Log.l(this.LogHeader, "mute state sync failed");
            try
            {
              observer.OnNext(voip.IsMuted());
            }
            catch (Exception ex4)
            {
              Log.LogException(ex4, "get voip mute state after mute fail");
            }
          }
        }), (Action) (() => observer.OnCompleted()));
        return (Action) (() => { });
      }));
    }

    public IObservable<bool> ToggleMute(UiCallState currState)
    {
      Log.d(this.LogHeader, "toggle mute | state:{0}", (object) currState);
      if (currState == UiCallState.Active)
        return Observable.Create<bool>((Func<IObserver<bool>, Action>) (observer =>
        {
          bool flag1 = false;
          lock (this.muteSubLock)
            flag1 = this.muteSub == null;
          if (flag1)
          {
            Log.d(this.LogHeader, "schedule toggle mute", (object) currState);
            IDisposable disposable = this.PerformVoipWorkObservable((Action<IVoip>) (voip =>
            {
              bool flag2 = false;
              bool Enabled = false;
              try
              {
                flag2 = voip.IsMuted();
                Enabled = !flag2;
                voip.SetMute(Enabled);
                Log.l(this.LogHeader, "togggle mute | {0}->{1}", (object) flag2, (object) Enabled);
                observer.OnNext(Enabled);
              }
              catch (Exception ex)
              {
                Log.LogException(ex, "voip mute");
                Log.l(this.LogHeader, "voip mute togggle failed | {0}->{1} | stay at {2}", (object) flag2, (object) Enabled, (object) flag2);
              }
            })).Subscribe<bool>((Action<bool>) (_ => { }), (Action) (() =>
            {
              lock (this.muteSubLock)
              {
                this.muteSub.SafeDispose();
                this.muteSub = (IDisposable) null;
              }
              observer.OnCompleted();
            }));
            lock (this.muteSubLock)
            {
              this.muteSub.SafeDispose();
              this.muteSub = disposable;
            }
          }
          else
          {
            Log.l(this.LogHeader, "togggle mute | skipped");
            observer.OnCompleted();
          }
          return (Action) (() =>
          {
            lock (this.muteSubLock)
            {
              this.muteSub.SafeDispose();
              this.muteSub = (IDisposable) null;
            }
          });
        }));
      this.presetMuteState = new bool?(!this.presetMuteState.HasValue || !this.presetMuteState.Value);
      Log.d(this.LogHeader, "toggle mute | preset | -> {0}", (object) this.presetMuteState.Value);
      return Observable.Return<bool>(this.presetMuteState.Value);
    }

    public void SendTextReply(string jid, string text)
    {
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        Message m = new Message(true)
        {
          KeyFromMe = true,
          KeyRemoteJid = jid,
          KeyId = FunXMPP.GenerateMessageId(),
          Data = text,
          Status = FunXMPP.FMessage.Status.Unsent,
          MediaWaType = FunXMPP.FMessage.Type.Undefined
        };
        db.InsertMessageOnSubmit(m);
        db.SubmitChanges();
      }));
    }

    public bool IsTapTooFrequent()
    {
      DateTime utcNow = DateTime.UtcNow;
      int num = !this.lastButtonTapTime.HasValue ? 0 : ((utcNow - this.lastButtonTapTime.Value).Milliseconds < 100 ? 1 : 0);
      this.lastButtonTapTime = new DateTime?(utcNow);
      return num != 0;
    }

    public static void ProcessCallEndReason(
      string peerJid,
      CallEndReason reason,
      bool hasVideo,
      out bool hasError)
    {
      hasError = false;
      string errMsg = (string) null;
      string errTitle = AppResources.PlaceCallErrorTitle;
      string displayName = UserCache.Get(peerJid, true).GetDisplayName(true);
      switch (reason)
      {
        case CallEndReason.YourCountryNotAllowed:
          errMsg = AppResources.CallErrorNAInSelfCountry;
          break;
        case CallEndReason.PeerCountryNotAllowed:
          errMsg = string.Format(AppResources.CallErrorNAInPeerCountry, (object) displayName);
          break;
        case CallEndReason.PeerAppTooOld:
          errMsg = !hasVideo ? string.Format(AppResources.CallErrorPeerAppTooOld, (object) displayName) : string.Format(AppResources.CallErrorPeerAppTooOldVideo, (object) displayName);
          break;
        case CallEndReason.PeerOsTooOld:
        case CallEndReason.PeerBadPlatform:
          errMsg = !hasVideo ? string.Format(AppResources.CallErrorPeerBadPlatform, (object) displayName) : string.Format(AppResources.CallErrorPeerBadPlatformVideo, (object) displayName);
          break;
        case CallEndReason.UnknownErrorCode:
        case CallEndReason.PeerUncallable:
          errMsg = !hasVideo ? string.Format(AppResources.CallErrorUncalable, (object) displayName) : string.Format(AppResources.CallErrorVideoUncallable, (object) displayName);
          break;
        case CallEndReason.RebootRequired:
          errMsg = AppResources.CallErrorRebootRequired;
          break;
        case CallEndReason.PeerRelayBindFailed:
          errMsg = string.Format(AppResources.CallErrorPeerConnectionFailure, (object) displayName);
          break;
        case CallEndReason.YourASNBad:
          errMsg = !DeviceNetworkInformation.IsWiFiEnabled ? AppResources.CallErrorNotConnectedCellular : AppResources.CallErrorNotConnectedWifi;
          break;
        case CallEndReason.RelayBindFailed:
          errMsg = !DeviceNetworkInformation.IsWiFiEnabled ? AppResources.CallErrorIncompatibleCellular : AppResources.CallErrorIncompatibleWifi;
          break;
        case CallEndReason.BadPrivacySettings:
          hasError = true;
          Deployment.Current.Dispatcher.BeginInvoke((Action) (() => UIUtils.MessageBox(AppResources.MicrophoneAccessTitle, AppResources.MicrophoneAccess, (IEnumerable<string>) new string[2]
          {
            AppResources.DismissButton,
            AppResources.Settings
          }, (Action<int>) (idx =>
          {
            if (idx != 1)
              return;
            NavUtils.NavigateExternal("ms-settings:privacy-microphone");
          }))));
          break;
      }
      if (errMsg == null)
        return;
      hasError = true;
      Deployment.Current.Dispatcher.BeginInvoke((Action) (() =>
      {
        if (errTitle == null)
        {
          int num1 = (int) MessageBox.Show(errMsg);
        }
        else
        {
          int num2 = (int) MessageBox.Show(errMsg, errTitle, MessageBoxButton.OK);
        }
      }));
    }
  }
}
