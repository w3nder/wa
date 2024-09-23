// Decompiled with JetBrains decompiler
// Type: WhatsApp.VoipVideoRenderer
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Media;
using System;
using System.Windows;
using System.Windows.Media;
using WhatsAppNative;


namespace WhatsApp
{
  public static class VoipVideoRenderer
  {
    private static bool _inForeground;
    private static bool _playerRestarting;
    private static bool _inVideoCall;
    private static bool _streamPaused;
    private static MediaStreamer _streamer;
    private static VoipMediaStreamSource _mss;
    private static VoipMediaStreamSource.OrientationChangedHandler _orientationHandler;

    public static void OnForegroundStarted()
    {
      Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() =>
      {
        VoipVideoRenderer._inForeground = true;
        VoipVideoRenderer.CreateVideo();
      }));
    }

    public static void OnForegroundLeaving()
    {
      Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() =>
      {
        VoipVideoRenderer._inForeground = false;
        VoipVideoRenderer.DestroyVideo(true);
      }));
    }

    public static void VideoCallStarted(
      VoipMediaStreamSource.OrientationChangedHandler orientationHandler)
    {
      Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() =>
      {
        VoipVideoRenderer._inVideoCall = true;
        VoipVideoRenderer._orientationHandler = orientationHandler;
        VoipVideoRenderer.CreateVideo();
      }));
    }

    public static void VideoCallEnded()
    {
      Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() =>
      {
        VoipVideoRenderer._streamPaused = false;
        VoipVideoRenderer.DestroyVideo(false);
        VoipVideoRenderer._inVideoCall = false;
        VoipVideoRenderer._orientationHandler = (VoipMediaStreamSource.OrientationChangedHandler) null;
      }));
    }

    public static void OnFrameReceived(
      byte[] buffer,
      VideoCodec codec,
      int width,
      int height,
      long timestamp,
      bool keyframe,
      VideoOrientation? orientation)
    {
      if (VoipVideoRenderer._mss == null || VoipVideoRenderer._mss.AddSample(buffer, codec, width, height, timestamp, keyframe, orientation) || VoipVideoRenderer._playerRestarting)
        return;
      VoipVideoRenderer._playerRestarting = true;
      Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() =>
      {
        VoipVideoRenderer.DestroyVideo(false);
        Voip.Worker.Enqueue((Action) (() =>
        {
          Voip.Instance.GetCallbacks().OnVideoPlayerRestart();
          Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() =>
          {
            VoipVideoRenderer.CreateVideo();
            VoipVideoRenderer._playerRestarting = false;
          }));
        }));
      }));
    }

    private static void CreateVideo()
    {
      if (!VoipVideoRenderer._inForeground || !VoipVideoRenderer._inVideoCall || VoipVideoRenderer._streamer != null)
        return;
      if (VoipVideoRenderer._streamPaused)
      {
        VoipVideoRenderer._streamPaused = false;
        Voip.Worker.Enqueue((Action) (() =>
        {
          try
          {
            Voip.Instance.ResumeVideoStream();
          }
          catch (Exception ex)
          {
            Log.LogException(ex, "ResumeVideoStream in CreateVideo");
          }
        }));
      }
      VoipVideoRenderer._streamer = MediaStreamerFactory.CreateMediaStreamer(867);
      VoipVideoRenderer._streamer.MediaOpened += new EventHandler<MediaStreamerOpenedEventArgs>(VoipVideoRenderer._streamer_MediaOpened);
      VoipVideoRenderer._streamer.MediaFailed += new EventHandler<MediaStreamerFailedEventArgs>(VoipVideoRenderer._streamer_MediaFailed);
      VoipVideoRenderer._streamer.MediaEnded += new EventHandler<MediaStreamerEndedEventArgs>(VoipVideoRenderer._streamer_MediaEnded);
      VoipVideoRenderer._mss = new VoipMediaStreamSource();
      if (VoipVideoRenderer._orientationHandler != null)
        VoipVideoRenderer._mss.OrientationChanged += VoipVideoRenderer._orientationHandler;
      VoipVideoRenderer._streamer.SetSource((MediaStreamSource) VoipVideoRenderer._mss);
    }

    private static void _streamer_MediaEnded(object sender, MediaStreamerEndedEventArgs e)
    {
      Log.l("Voip MediaStreamer", "media ended");
    }

    private static void _streamer_MediaFailed(object sender, MediaStreamerFailedEventArgs e)
    {
      Log.SendCrashLog(e.ErrorException, "Voip MediaStreamer");
    }

    private static void _streamer_MediaOpened(object sender, MediaStreamerOpenedEventArgs e)
    {
      Log.l("Voip MediaStreamer", "media opened");
    }

    private static void DestroyVideo(bool pauseStream)
    {
      if (!VoipVideoRenderer._inVideoCall)
        return;
      if (pauseStream)
      {
        Voip.Worker.Enqueue((Action) (() =>
        {
          try
          {
            Voip.Instance.PauseVideoStream();
          }
          catch (Exception ex)
          {
            Log.LogException(ex, "PauseVideoStream in DestroyVideo");
          }
        }));
        VoipVideoRenderer._streamPaused = true;
      }
      if (VoipVideoRenderer._streamer == null)
        return;
      VoipVideoRenderer._streamer.SafeDispose();
      VoipVideoRenderer._streamer = (MediaStreamer) null;
      if (VoipVideoRenderer._mss != null && VoipVideoRenderer._orientationHandler != null)
        VoipVideoRenderer._mss.OrientationChanged -= VoipVideoRenderer._orientationHandler;
      VoipVideoRenderer._mss = (VoipMediaStreamSource) null;
    }
  }
}
