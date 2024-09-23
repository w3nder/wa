// Decompiled with JetBrains decompiler
// Type: WhatsApp.InAppRinger
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Devices;
using Microsoft.Phone.Reactive;
using System;

#nullable disable
namespace WhatsApp
{
  public static class InAppRinger
  {
    public static IObservable<Unit> GetRingObservable(string jid)
    {
      IObservable<Unit> leftSource = Observable.Never<Unit>();
      int ringerVibrateState = (int) AppState.RingerVibrateState;
      string tone = (string) null;
      if ((ringerVibrateState & 2) != 0 && !string.IsNullOrEmpty(tone = Ringtones.GetRingtonePath(jid)))
        leftSource = leftSource.Merge<Unit>(Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
        {
          Action onComplete = (Action) (() => { });
          WaAudioRouting.Endpoint old = WaAudioRouting.GetCurrentAudioEndpoint();
          if (old != WaAudioRouting.Endpoint.Bluetooth)
          {
            WaAudioRouting.SetAudioEndpoint(WaAudioRouting.Endpoint.Speaker);
            onComplete = (Action) (() => WaAudioRouting.SetAudioEndpoint(old));
          }
          IDisposable disp = InAppRinger.MediaPlaybackObservable(tone).Repeat<Unit>().ObserveOnDispatcher<Unit>().Do<Unit>((Action<Unit>) (_ => { }), onComplete).Subscribe(observer);
          return (Action) (() =>
          {
            onComplete();
            disp.SafeDispose();
            disp = (IDisposable) null;
          });
        })));
      if ((ringerVibrateState & 1) != 0)
      {
        IObservable<Unit>[] observableArray = new IObservable<Unit>[8]
        {
          InAppRinger.Vibrate(250),
          InAppRinger.Delay(200),
          InAppRinger.Vibrate(500),
          InAppRinger.Delay(200),
          InAppRinger.Vibrate(250),
          InAppRinger.Delay(200),
          InAppRinger.Vibrate(250),
          InAppRinger.Delay(2000)
        };
        leftSource = leftSource.Merge<Unit>(Observable.Concat<Unit>(observableArray).Repeat<Unit>());
      }
      return leftSource;
    }

    public static IObservable<Unit> MediaPlaybackObservable(string path)
    {
      return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        object @lock = new object();
        PttDispatchMediaPlayer playback = new PttDispatchMediaPlayer();
        playback.MediaEnded += (EventHandler) ((sender, args) =>
        {
          lock (@lock)
          {
            if (playback != null)
            {
              playback.Detach();
              playback = (PttDispatchMediaPlayer) null;
            }
          }
          observer.OnNext(new Unit());
          observer.OnCompleted();
        });
        playback.MediaOpened += (EventHandler) ((sender, args) =>
        {
          try
          {
            int num = (int) ((double) NativeInterfaces.Misc.GetVolume() * 100.0);
            playback.Volume = Math.Max(0, Math.Min(77, 100));
          }
          catch (Exception ex)
          {
            Log.LogException(ex, "volume set");
          }
        });
        string localPath = NativeMediaStorage.MakeUri(NativeInterfaces.Misc.GetAppInstallDir() + "\\" + path);
        playback.Play(localPath);
        return (Action) (() =>
        {
          lock (@lock)
          {
            if (playback == null)
              return;
            playback.Stop();
            playback.Detach();
            playback = (PttDispatchMediaPlayer) null;
          }
        });
      }));
    }

    public static IObservable<Unit> Vibrate(int millis)
    {
      return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        VibrateController vibra = VibrateController.Default;
        TimeSpan timeSpan = TimeSpan.FromMilliseconds((double) millis);
        vibra.Start(timeSpan);
        IDisposable disp = PooledTimer.Instance.Schedule(timeSpan, (Action) (() =>
        {
          observer.OnNext(new Unit());
          observer.OnCompleted();
        }));
        return (Action) (() =>
        {
          if (disp == null)
            return;
          vibra.Stop();
          disp.SafeDispose();
          disp = (IDisposable) null;
        });
      }));
    }

    public static IObservable<Unit> Delay(int millis)
    {
      return Observable.CreateWithDisposable<Unit>((Func<IObserver<Unit>, IDisposable>) (observer => PooledTimer.Instance.Schedule(TimeSpan.FromMilliseconds((double) millis), (Action) (() =>
      {
        observer.OnNext(new Unit());
        observer.OnCompleted();
      }))));
    }
  }
}
