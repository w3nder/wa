// Decompiled with JetBrains decompiler
// Type: WhatsApp.PttDispatchMediaPlayer
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Collections.Generic;
using System.Windows;
using WhatsAppNative;

#nullable disable
namespace WhatsApp
{
  public class PttDispatchMediaPlayer : IWaMediaPlayer
  {
    private IWaMediaPlayer currentPlayer;

    public void Play(string localPath)
    {
      if (this.currentPlayer != null)
      {
        this.currentPlayer.Detach();
        this.currentPlayer = (IWaMediaPlayer) null;
      }
      this.currentPlayer = (IWaMediaPlayer) new VoipAudioPlayer((IEnumerable<SoundPlaybackCodec>) CodecDetector.DetectAudioCodec(localPath).Codecs);
      this.AttachEvents();
      this.currentPlayer.Play(localPath);
    }

    public event EventHandler MediaStarted;

    public event EventHandler MediaOpened;

    public event EventHandler MediaEnded;

    public event EventHandler<ErrorEventArgs> MediaFailed;

    private void AttachEvents()
    {
      this.currentPlayer.MediaEnded += (EventHandler) ((sender, args) =>
      {
        EventHandler mediaEnded = this.MediaEnded;
        if (mediaEnded != null)
          mediaEnded((object) this, args);
        this.Detach();
      });
      this.currentPlayer.MediaFailed += (EventHandler<ErrorEventArgs>) ((sender, args) =>
      {
        EventHandler<ErrorEventArgs> mediaFailed = this.MediaFailed;
        if (mediaFailed == null)
          return;
        mediaFailed((object) this, args);
      });
      this.currentPlayer.MediaOpened += (EventHandler) ((sender, args) =>
      {
        EventHandler mediaOpened = this.MediaOpened;
        if (mediaOpened == null)
          return;
        mediaOpened((object) this, args);
      });
      this.currentPlayer.MediaStarted += (EventHandler) ((sender, args) =>
      {
        EventHandler mediaStarted = this.MediaStarted;
        if (mediaStarted == null)
          return;
        mediaStarted((object) this, args);
      });
    }

    public Duration Duration
    {
      get
      {
        return this.currentPlayer == null ? new Duration(TimeSpan.FromMilliseconds(0.0)) : this.currentPlayer.Duration;
      }
    }

    public TimeSpan Position
    {
      get => this.currentPlayer.Position;
      set => this.currentPlayer.Position = value;
    }

    public int Volume
    {
      get => this.currentPlayer.Volume;
      set => this.currentPlayer.Volume = value;
    }

    public void Detach()
    {
      if (this.currentPlayer == null)
        return;
      this.currentPlayer.Detach();
      this.currentPlayer = (IWaMediaPlayer) null;
    }

    public void Pause()
    {
      if (this.currentPlayer == null)
        return;
      this.currentPlayer.Pause();
    }

    public void Play()
    {
      if (this.currentPlayer == null)
        return;
      this.currentPlayer.Play();
    }

    public void Stop()
    {
      if (this.currentPlayer == null)
        return;
      try
      {
        this.currentPlayer.Stop();
      }
      catch (Exception ex)
      {
        Log.SendCrashLog(ex, "exception during AudioPlayBack.Stop", logOnlyForRelease: true);
      }
    }
  }
}
