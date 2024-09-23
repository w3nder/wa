// Decompiled with JetBrains decompiler
// Type: WhatsApp.PttPlaybackWrapper
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using WhatsApp.CommonOps;


namespace WhatsApp
{
  internal class PttPlaybackWrapper : IDisposable
  {
    private PlayAudioMessage audioPlayback_;
    private IDisposable audioRoutingSub;
    private IDisposable lcdSub;
    private IDisposable obscuredSub;
    private IDisposable playerEventSub;
    private PhoneApplicationPage page;
    private Popup proximityCover;
    public Action PlaybackStarted;
    public Action PlaybackStopped;

    public PlayAudioMessage Device => this.audioPlayback_;

    public PttPlaybackWrapper()
    {
      this.page = App.CurrentApp.CurrentPage;
      this.audioPlayback_ = PlayAudioMessage.GetInstance(true);
      this.audioPlayback_.PlaybackStarted += new EventHandler(this.OnPttPlaybackStarted);
      this.audioPlayback_.PlaybackStopped += new EventHandler(this.OnPttPlaybackStopped);
      this.playerEventSub = (IDisposable) new DisposableAction((Action) (() =>
      {
        this.audioPlayback_.PlaybackStarted -= new EventHandler(this.OnPttPlaybackStarted);
        this.audioPlayback_.PlaybackStopped -= new EventHandler(this.OnPttPlaybackStopped);
        this.PlaybackStarted = this.PlaybackStopped = (Action) null;
      }));
      this.audioRoutingSub = WaAudioRouting.GetEndpointChangedObservable().ObserveOnDispatcher<WaAudioRouting.Endpoint>().Subscribe<WaAudioRouting.Endpoint>((Action<WaAudioRouting.Endpoint>) (_ => this.UpdateForPttPlayback("endpoint changed")));
      TransitionFrame rootFrame = App.CurrentApp.RootFrame;
      rootFrame.Obscured += new EventHandler<ObscuredEventArgs>(this.OnObscured);
      this.obscuredSub = (IDisposable) new DisposableAction((Action) (() => rootFrame.Obscured -= new EventHandler<ObscuredEventArgs>(this.OnObscured)));
    }

    public void OnPttPlaybackStarted(object sender, EventArgs e)
    {
      Action playbackStarted = this.PlaybackStarted;
      if (playbackStarted != null)
        playbackStarted();
      this.UpdateForPttPlayback("playback started");
    }

    public void OnPttPlaybackStopped(object sender, EventArgs e)
    {
      Action playbackStopped = this.PlaybackStopped;
      if (playbackStopped != null)
        playbackStopped();
      this.UpdateForPttPlayback("playback stopped");
    }

    private void OnObscured(object sender, ObscuredEventArgs e)
    {
      AudioPlaybackManager player = this.audioPlayback_.Player;
      if (!player.IsPlaying)
        return;
      player.Pause();
    }

    public void DisposeLcd()
    {
      this.lcdSub.SafeDispose();
      this.lcdSub = (IDisposable) null;
    }

    public void Dispose()
    {
      this.obscuredSub.SafeDispose();
      this.obscuredSub = (IDisposable) null;
      this.audioRoutingSub.SafeDispose();
      this.audioRoutingSub = (IDisposable) null;
      this.DisposeLcd();
      PlayAudioMessage.DisposeInstance();
      this.playerEventSub.SafeDispose();
      this.playerEventSub = (IDisposable) null;
    }

    private void UpdateForPttPlayback(string context)
    {
      PlayAudioMessage audioPlayback = this.audioPlayback_;
      WaAudioRouting.Endpoint audioOutput = audioPlayback.Player.AudioOutput;
      bool toLock = audioPlayback.Player.IsActive && audioOutput == WaAudioRouting.Endpoint.Earpiece && !WaAudioRouting.IsBluetoothAvailable();
      Log.d("ptt", "{3} | {0} {1} -> {2}", (object) audioPlayback.Player.IsActive, (object) audioOutput, (object) toLock, (object) context);
      if (WaAudioRouting.ProximitySupported & toLock)
      {
        Action action = (Action) (() => { });
        if (this.lcdSub == null)
        {
          PopupManager popupManager = (PopupManager) null;
          if (this.proximityCover == null)
          {
            Popup popup = new Popup();
            Grid grid = new Grid();
            grid.Margin = new Thickness(-2.0);
            grid.CacheMode = (CacheMode) new BitmapCache();
            grid.Background = (Brush) UIUtils.BackgroundBrush;
            popup.Child = (UIElement) grid;
            this.proximityCover = popup;
            popupManager = new PopupManager(this.proximityCover, false);
          }
          this.page.IsEnabled = false;
          popupManager?.Show();
          this.proximityCover.IsOpen = true;
          IDisposable lcdOnOffSub = (IDisposable) null;
          if (AppState.IsWP10OrLater)
          {
            try
            {
              lcdOnOffSub = NativeInterfaces.Misc.ProximitySensorLcdSubscribe();
            }
            catch (Exception ex)
            {
            }
          }
          this.lcdSub = (IDisposable) new DisposableAction((Action) (() =>
          {
            this.page.IsEnabled = true;
            if (this.proximityCover != null)
              this.proximityCover.IsOpen = false;
            lcdOnOffSub.SafeDispose();
            lcdOnOffSub = (IDisposable) null;
          }));
        }
      }
      else
      {
        this.lcdSub.SafeDispose();
        this.lcdSub = (IDisposable) null;
      }
      UIUtils.LockOrientation(this.page, toLock);
    }
  }
}
