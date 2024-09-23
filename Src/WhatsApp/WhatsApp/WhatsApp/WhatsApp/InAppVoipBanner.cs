// Decompiled with JetBrains decompiler
// Type: WhatsApp.InAppVoipBanner
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Windows;
using WhatsAppNative;


namespace WhatsApp
{
  public class InAppVoipBanner : InAppFloatingBanner
  {
    private static InAppVoipBanner instance;
    private bool closed;
    private bool callEnded;
    private IDisposable callEndSub;
    private IDisposable callStateSub;
    private IDisposable videoStateSub;
    private IDisposable participantsSub;
    private InAppVoipBannerView voipView;
    private string peerJid;
    private UiUpgradeState upgradeState;

    protected InAppVoipBanner(InAppVoipBannerView voipBannerView)
    {
      this.view = (IInAppFloatingBannerView) (this.voipView = voipBannerView);
      this.callStateSub = VoipHandler.CallStateChangedSubject.ObserveOnDispatcher<WaCallStateChangedArgs>().Subscribe<WaCallStateChangedArgs>((Action<WaCallStateChangedArgs>) (args => this.OnCallStateChanged(args.CurrState, new UiCallState?(args.PrevState))));
      this.videoStateSub = VoipHandler.VideoStateChanged.ObserveOnDispatcher<WaCallVideoStateChangedArgs>().Subscribe<WaCallVideoStateChangedArgs>((Action<WaCallVideoStateChangedArgs>) (args => this.OnVideoStateChanged(args.UpgradeState)));
      this.callEndSub = VoipHandler.CallEndedSubject.ObserveOnDispatcher<WaCallEndedEventArgs>().Subscribe<WaCallEndedEventArgs>(new Action<WaCallEndedEventArgs>(this.OnCallEnded));
      this.participantsSub = VoipHandler.GroupInfoChanged.ObserveOnDispatcher<WAGroupCallChangedArgs>().Subscribe<WAGroupCallChangedArgs>(new Action<WAGroupCallChangedArgs>(this.OnGroupInfoChanged));
    }

    public static void ShowAsync()
    {
      if (InAppVoipBanner.instance != null)
        return;
      InAppVoipBanner.instance = new InAppVoipBanner((InAppVoipBannerView) null);
      Voip.Worker.Enqueue((Action) (() =>
      {
        string peerJid = (string) null;
        string callId = (string) null;
        CallInfoStruct callInfo;
        if (Voip.Instance.GetCallInfo(out callId, out peerJid, out callInfo) && (callInfo.CallState == CallState.CallActive || callInfo.CallState == CallState.Calling))
          Deployment.Current.Dispatcher.BeginInvoke((Action) (() =>
          {
            if (App.CurrentApp.CurrentPage is CallScreenPage)
            {
              InAppVoipBanner.CloseInstance();
            }
            else
            {
              if (InAppVoipBanner.instance == null || InAppVoipBanner.instance.callEnded)
                return;
              UiCallState uiState = Voip.TranslateCallState(callInfo.CallState);
              InAppVoipBanner.instance.peerJid = peerJid;
              string participantsDisplayName = Voip.Instance.GetCallParticipantsDisplayName();
              InAppVoipBanner.instance.SetVoipBannerView(InAppVoipBannerView.Create(participantsDisplayName, uiState));
              InAppVoipBanner.instance.onClick = (Action) (() => CallScreenPage.Launch(peerJid, new UiCallState?(uiState), hasVideo: callInfo.VideoEnabled));
              InAppVoipBanner.instance.Show();
              InAppVoipBanner.instance.voipView.SetDuration(callInfo.CallState == CallState.CallActive ? new int?(callInfo.CallDuration / 1000) : new int?());
            }
          }));
        else
          Deployment.Current.Dispatcher.BeginInvoke((Action) (() => InAppVoipBanner.CloseInstance()));
      }));
    }

    public static void CloseInstance()
    {
      if (InAppVoipBanner.instance == null)
        return;
      InAppVoipBanner.instance.Close();
      InAppVoipBanner.instance = (InAppVoipBanner) null;
    }

    public override void Close()
    {
      if (this.closed)
        return;
      this.closed = true;
      this.videoStateSub.SafeDispose();
      this.videoStateSub = (IDisposable) null;
      this.callStateSub.SafeDispose();
      this.callStateSub = (IDisposable) null;
      this.callEndSub.SafeDispose();
      this.callEndSub = (IDisposable) null;
      this.participantsSub.SafeDispose();
      this.participantsSub = (IDisposable) null;
      if (this.voipView != null)
        this.voipView.Dispose();
      base.Close();
      InAppVoipBanner.instance = (InAppVoipBanner) null;
    }

    protected override bool ShouldPreserveBehindNewBanner() => true;

    protected override bool ShouldShiftPageContentDownInPortrait() => true;

    protected void SetVoipBannerView(InAppVoipBannerView voipBannerView)
    {
      this.view = (IInAppFloatingBannerView) (this.voipView = voipBannerView);
    }

    private void OnCallEnded(WaCallEndedEventArgs args)
    {
      this.callEnded = true;
      this.Close();
      if (!args.ShouldRateCall)
        return;
      CallRatingPage.Start(this.peerJid, args.RatingCookie, false, false);
    }

    private void OnCallStateChanged(UiCallState newState, UiCallState? prevState)
    {
      if (newState == UiCallState.None)
      {
        this.Close();
      }
      else
      {
        if (this.voipView == null)
          return;
        this.voipView.SetCallState(newState);
        if (newState != UiCallState.Active)
          return;
        Voip.Worker.Enqueue((Action) (() =>
        {
          string peerJid = (string) null;
          string callId = (string) null;
          CallInfoStruct callInfo;
          if (!Voip.Instance.GetCallInfo(out callId, out peerJid, out callInfo) || callInfo.CallState != CallState.CallActive)
            return;
          Deployment.Current.Dispatcher.BeginInvoke((Action) (() => this.voipView.SetDuration(new int?(callInfo.CallDuration / 1000))));
        }));
      }
    }

    private void OnGroupInfoChanged(WAGroupCallChangedArgs args)
    {
      try
      {
        this.voipView.UpdateParticipants(Voip.Instance.GetCallParticipantsDisplayName());
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "group info changed.");
      }
    }

    private void OnVideoStateChanged(UiUpgradeState state)
    {
      if (state == this.upgradeState)
        return;
      this.upgradeState = state;
      if (state != UiUpgradeState.RequestedByPeer)
        return;
      this.View_Tap();
    }

    protected override void OnClosed()
    {
      base.OnClosed();
      SysTrayHelper.SysTrayKeeper.Instance.ExtraVertialPageMarginAdjustment = 0.0;
    }
  }
}
