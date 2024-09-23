// Decompiled with JetBrains decompiler
// Type: WhatsApp.MediaPlayerHelper
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Windows.Controls;
using System.Windows.Media;
using WhatsAppNative;


namespace WhatsApp
{
  internal static class MediaPlayerHelper
  {
    public static void SetMediaPlayerState(
      this MediaElement mediaElement,
      UiVideoState state,
      bool force = false,
      Uri newSource = null)
    {
      string str = "no action";
      MediaElementState currentState = mediaElement.CurrentState;
      switch (state)
      {
        case UiVideoState.None:
          mediaElement.Stop();
          str = "stopped +" + mediaElement.CurrentState.ToString();
          break;
        case UiVideoState.Playing:
          if (force || currentState != MediaElementState.Opening && currentState != MediaElementState.Playing)
          {
            if (newSource != (Uri) null)
              mediaElement.Source = newSource;
            mediaElement.Play();
            str = "started +" + mediaElement.CurrentState.ToString();
            break;
          }
          break;
      }
      Log.l("callscreen", "SetMediaPlayerState({0}, {1}, {2}, {3}) -> {4} (current state {5})", (object) mediaElement.Name, (object) state, (object) force, (object) newSource, (object) str, (object) currentState);
    }
  }
}
