// Decompiled with JetBrains decompiler
// Type: WhatsApp.CommonOps.PlayAudioMessage7
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;


namespace WhatsApp.CommonOps
{
  internal class PlayAudioMessage7 : PlayAudioMessage
  {
    private AudioEndpointSwitchPanel audioEndpointSwitchPanel_;
    private IDisposable popupCloseSub_;

    protected override void Init()
    {
      base.Init();
      this.audioEndpointSwitchPanel_ = new AudioEndpointSwitchPanel();
      this.audioEndpointSwitchPanel_.AudioEndpointChanged += (EventHandler) ((sender, e) =>
      {
        WaAudioRouting.Endpoint defaultAudioEndpoint = (WaAudioRouting.Endpoint) Settings.DefaultAudioEndpoint;
        this.player_.RelativeSeek(-1000.0);
        this.player_.SwitchAudioOutput(defaultAudioEndpoint);
      });
    }

    private void OpenAudioEndpointSwitchBar()
    {
      this.popupCloseSub_.SafeDispose();
      this.popupCloseSub_ = (IDisposable) null;
      if (this.audioEndpointSwitchPanel_ == null || this.audioEndpointSwitchPanel_.IsPopupOpen)
        return;
      PopupManager popupManager1 = new PopupManager(this.audioEndpointSwitchPanel_.AudioEndpointSwitchPopup, false);
      popupManager1.OrientationChanged += (EventHandler<EventArgs>) ((sender, e) =>
      {
        if (!(sender is PopupManager popupManager3))
          return;
        this.audioEndpointSwitchPanel_.Orientation = popupManager3.Orientation;
      });
      IDisposable sysTrayHideSub = SysTrayHelper.SysTrayKeeper.Instance.RequestHide();
      popupManager1.Closed += (EventHandler<EventArgs>) ((sender, e) => sysTrayHideSub.Dispose());
      popupManager1.Show();
    }

    private void CloseAudioEndpointSwitchBar()
    {
      if (this.audioEndpointSwitchPanel_ == null)
        return;
      this.popupCloseSub_ = this.audioEndpointSwitchPanel_.ClosePopup();
    }

    protected override void OnPlaybackStarted()
    {
      base.OnPlaybackStarted();
      if (this.player_.AudioOutput != WaAudioRouting.Endpoint.Speaker && this.player_.AudioOutput != WaAudioRouting.Endpoint.Earpiece)
        return;
      this.OpenAudioEndpointSwitchBar();
    }

    protected override void OnPlaybackStopped()
    {
      base.OnPlaybackStopped();
      this.CloseAudioEndpointSwitchBar();
    }

    protected override void OnAudioOutputChanged(WaAudioRouting.Endpoint newEndpoint)
    {
      base.OnAudioOutputChanged(newEndpoint);
      if (newEndpoint == WaAudioRouting.Endpoint.Speaker || newEndpoint == WaAudioRouting.Endpoint.Earpiece)
        return;
      this.CloseAudioEndpointSwitchBar();
    }
  }
}
