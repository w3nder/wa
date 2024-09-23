// Decompiled with JetBrains decompiler
// Type: WhatsApp.MpnsForeground
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Notification;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Shell;
using System;

#nullable disable
namespace WhatsApp
{
  public class MpnsForeground : IPushSystemForeground
  {
    private MutexWithWatchdog PushLock = PushSystem.PushLock;

    public IObservable<Uri> UriObservable
    {
      get
      {
        return Observable.CreateWithDisposable<Uri>((Func<IObserver<Uri>, IDisposable>) (observer =>
        {
          HttpNotificationChannel channel = (HttpNotificationChannel) null;
          bool abort = false;
          this.PushLock.PerformWithLock((Action) (() =>
          {
            try
            {
              channel = HttpNotificationChannel.Find("message");
            }
            catch (Exception ex)
            {
              this.LogPushException(ex, "find channel");
            }
            if (channel == null)
            {
              BackgroundAgentHelper.DescheduleVoip(false);
              try
              {
                channel = new HttpNotificationChannel("message", "mpns.whatsapp.net");
                channel.Open();
              }
              catch (Exception ex)
              {
                this.LogPushException(ex, "push open");
                abort = true;
              }
            }
            else
              Log.WriteLineDebug("Using existing push channel");
          }));
          if (abort)
          {
            observer.OnCompleted();
            return (IDisposable) new DisposableAction((Action) (() => { }));
          }
          IDisposable uriObservable = Observable.Return<Uri>(channel.TryGetUri()).Concat<Uri>(channel.GetChannelUriUpdatedAsync().Select<IEvent<NotificationChannelUriEventArgs>, Uri>((Func<IEvent<NotificationChannelUriEventArgs>, Uri>) (ev => ev.EventArgs.ChannelUri))).Do<Uri>((Action<Uri>) (_ =>
          {
            if (!(_ == (Uri) null))
              return;
            Log.d("mpns", "null push uri!");
          })).Where<Uri>((Func<Uri, bool>) (uri => uri != (Uri) null)).Subscribe(observer);
          this.BindPush(channel);
          return uriObservable;
        }));
      }
    }

    private void BindPush(HttpNotificationChannel channel)
    {
      ChannelConnectionStatus? nullable1 = new ChannelConnectionStatus?();
      bool[] flagArray = new bool[2];
      ChannelConnectionStatus? connectionStatus1;
      ChannelConnectionStatus? nullable2 = connectionStatus1 = channel.TryGetConnectionStatus();
      ChannelConnectionStatus connectionStatus2 = ChannelConnectionStatus.Connected;
      flagArray[0] = (nullable2.GetValueOrDefault() == connectionStatus2 ? (!nullable2.HasValue ? 1 : 0) : 1) != 0;
      Uri uri;
      flagArray[1] = (uri = channel.TryGetUri()) == (Uri) null;
      if (Utils.NonShortCircuitOr(flagArray))
        Log.WriteLineDebug("Got unexpected push connection status: {0}; batsaver={1}; wifi={2}; edge={3}; 3g={4}; nulluri={5}", connectionStatus1.HasValue ? (object) connectionStatus1.Value.ToString() : (object) "(null)", (object) AppState.BatterySaverEnabled, (object) NetworkStateMonitor.IsWifiDataConnected(), (object) NetworkStateMonitor.Is2GConnection(), (object) NetworkStateMonitor.Is3GOrBetter(), (object) (uri == (Uri) null));
      try
      {
        if (!channel.IsShellTileBound)
          channel.BindToShellTile();
      }
      catch (Exception ex)
      {
        this.LogPushException(ex, "bind to shell tile");
      }
      try
      {
        if (!channel.IsShellToastBound)
          channel.BindToShellToast();
      }
      catch (Exception ex)
      {
        this.LogPushException(ex, "bind to shell toast");
      }
      PushSystem.PushBoundSubject.OnNext(new Unit());
    }

    public void BindPush()
    {
      HttpNotificationChannel channel = (HttpNotificationChannel) null;
      try
      {
        channel = HttpNotificationChannel.Find("message");
      }
      catch (Exception ex)
      {
        this.LogPushException(ex, "find channel");
      }
      if (channel == null)
        return;
      this.BindPush(channel);
    }

    public void RequestNewUri()
    {
      try
      {
        HttpNotificationChannel.Find("message")?.Close();
        Settings.LastChannelReopenUtc = new DateTime?(FunRunner.CurrentServerTimeUtc);
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "push reopen");
      }
    }

    public string PushState
    {
      get
      {
        string pushState = "unknown";
        try
        {
          HttpNotificationChannel notificationChannel = HttpNotificationChannel.Find("message");
          if (notificationChannel != null)
          {
            pushState = notificationChannel.ConnectionStatus.ToString();
            if (notificationChannel.ChannelUri == (Uri) null)
              pushState += "-nouri";
          }
        }
        catch (Exception ex)
        {
          pushState = ex.GetSynopsis();
        }
        return pushState;
      }
    }

    public bool IsHealthy
    {
      get
      {
        HttpNotificationChannel channel = (HttpNotificationChannel) null;
        try
        {
          channel = HttpNotificationChannel.Find("message");
        }
        catch (Exception ex)
        {
        }
        return channel != null && ((int) channel.TryGetConnectionStatus() ?? 0) == 0 && channel.TryGetUri() != (Uri) null;
      }
    }

    private void CreateSecondaryTile(Uri uri, StandardTileData data, bool supportsWide = false)
    {
      ShellTile.Create(uri, (ShellTileData) data, supportsWide);
    }

    public void CreateTile(
      string key,
      string title,
      int initialCount,
      string initialContent,
      Uri uri,
      Uri backgroundImage,
      Uri smallBackgroundImage)
    {
      title = title.EscapeForTile();
      FlipTileData data = new FlipTileData();
      data.Title = data.BackTitle = title;
      data.BackContent = initialContent;
      data.Count = new int?(initialCount);
      data.BackgroundImage = backgroundImage;
      data.SmallBackgroundImage = smallBackgroundImage;
      this.CreateSecondaryTile(uri, (StandardTileData) data);
    }

    private void LogPushException(Exception ex, string descr) => Log.LogException(ex, descr);
  }
}
