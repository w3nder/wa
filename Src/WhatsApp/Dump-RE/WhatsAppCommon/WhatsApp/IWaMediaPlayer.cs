// Decompiled with JetBrains decompiler
// Type: WhatsApp.IWaMediaPlayer
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Windows;

#nullable disable
namespace WhatsApp
{
  public interface IWaMediaPlayer
  {
    TimeSpan Position { get; set; }

    Duration Duration { get; }

    int Volume { get; set; }

    event EventHandler MediaStarted;

    event EventHandler MediaOpened;

    event EventHandler MediaEnded;

    event EventHandler<ErrorEventArgs> MediaFailed;

    void Play(string localPath);

    void Play();

    void Pause();

    void Stop();

    void Detach();
  }
}
