// Decompiled with JetBrains decompiler
// Type: WhatsApp.MediaElementWrapper
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;


namespace WhatsApp
{
  public class MediaElementWrapper : IWaMediaPlayer
  {
    private MediaElement player_;

    public event EventHandler MediaStarted;

    public event EventHandler MediaOpened;

    public event EventHandler MediaEnded;

    public event EventHandler<ErrorEventArgs> MediaFailed;

    public MediaElementWrapper(MediaElement player)
    {
      this.player_ = player;
      this.player_.MediaOpened += new RoutedEventHandler(this.Player_MediaOpened);
      this.player_.MediaEnded += new RoutedEventHandler(this.Player_MediaEnded);
      this.player_.MediaFailed += new EventHandler<ExceptionRoutedEventArgs>(this.Player_MediaFailed);
    }

    public TimeSpan Position
    {
      get => this.player_.Position;
      set => this.player_.Position = value;
    }

    public Duration Duration => this.player_.NaturalDuration;

    public int Volume
    {
      get => 100;
      set
      {
      }
    }

    private void Player_MediaOpened(object sender, RoutedEventArgs e)
    {
      EventHandler mediaOpened = this.MediaOpened;
      if (mediaOpened == null)
        return;
      mediaOpened(sender, (EventArgs) e);
    }

    private void Player_MediaEnded(object sender, RoutedEventArgs e)
    {
      EventHandler mediaEnded = this.MediaEnded;
      if (mediaEnded == null)
        return;
      mediaEnded(sender, (EventArgs) e);
    }

    private void Player_MediaFailed(object sender, ExceptionRoutedEventArgs e)
    {
      EventHandler<ErrorEventArgs> mediaFailed = this.MediaFailed;
      if (mediaFailed == null)
        return;
      mediaFailed(sender, new ErrorEventArgs()
      {
        ErrorException = e.ErrorException
      });
    }

    public void Play(string localPath)
    {
      this.SetSource(localPath);
      this.Play();
    }

    public void Play(Uri localUri)
    {
      this.SetSource(localUri);
      this.Play();
    }

    public void SetSource(string localPath)
    {
      this.Pause();
      using (IMediaStorage mediaStorage = MediaStorage.Create(localPath))
      {
        using (Stream stream = mediaStorage.OpenFile(localPath))
          this.player_.SetSource(stream);
      }
    }

    public void SetSource(Uri path)
    {
      Log.l("Media Element", "Using aource uri");
      this.Pause();
      this.player_.Source = path;
    }

    public void Play() => this.player_.Play();

    private void Play(Stream stream)
    {
      this.Stop();
      this.player_.SetSource(stream);
      this.player_.Position = TimeSpan.Zero;
      this.player_.Play();
    }

    public void Pause() => this.player_.Pause();

    public void Stop() => this.player_.Stop();

    public void Detach()
    {
      this.player_.MediaOpened -= new RoutedEventHandler(this.Player_MediaOpened);
      this.player_.MediaEnded -= new RoutedEventHandler(this.Player_MediaEnded);
      this.player_.MediaFailed -= new EventHandler<ExceptionRoutedEventArgs>(this.Player_MediaFailed);
      this.player_.ClearValue(MediaElement.SourceProperty);
      this.player_.Source = (Uri) null;
    }
  }
}
