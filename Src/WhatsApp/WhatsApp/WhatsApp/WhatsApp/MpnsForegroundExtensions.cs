// Decompiled with JetBrains decompiler
// Type: WhatsApp.MpnsForegroundExtensions
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Notification;
using Microsoft.Phone.Reactive;
using System;


namespace WhatsApp
{
  public static class MpnsForegroundExtensions
  {
    public static IObservable<IEvent<NotificationChannelUriEventArgs>> GetChannelUriUpdatedAsync(
      this HttpNotificationChannel that)
    {
      return Observable.FromEvent<NotificationChannelUriEventArgs>((Action<EventHandler<NotificationChannelUriEventArgs>>) (eh => that.ChannelUriUpdated += eh), (Action<EventHandler<NotificationChannelUriEventArgs>>) (eh => that.ChannelUriUpdated -= eh));
    }

    public static ChannelConnectionStatus? TryGetConnectionStatus(
      this HttpNotificationChannel channel)
    {
      try
      {
        return new ChannelConnectionStatus?(channel.ConnectionStatus);
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "get channel status");
      }
      return new ChannelConnectionStatus?();
    }

    public static Uri TryGetUri(this HttpNotificationChannel channel)
    {
      try
      {
        return channel.ChannelUri;
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "get channel uri");
      }
      return (Uri) null;
    }
  }
}
