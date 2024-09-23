// Decompiled with JetBrains decompiler
// Type: WhatsApp.AsyncExtensions
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.BackgroundTransfer;
using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Device.Location;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using System.Windows.Threading;


namespace WhatsApp
{
  public static class AsyncExtensions
  {
    public static IObservable<Stream> GetRequestStreamAsync(this WebRequest that)
    {
      return Observable.FromAsyncPattern<Stream>(new Func<AsyncCallback, object, IAsyncResult>(that.BeginGetRequestStream), new Func<IAsyncResult, Stream>(that.EndGetRequestStream))();
    }

    public static IObservable<WebResponse> GetResponseAsync(this WebRequest that)
    {
      return Observable.FromAsyncPattern<WebResponse>(new Func<AsyncCallback, object, IAsyncResult>(that.BeginGetResponse), new Func<IAsyncResult, WebResponse>(that.EndGetResponse))().Take<WebResponse>(1);
    }

    public static IObservable<Unit> WriteAndCloseAsync(this Stream that, byte[] data)
    {
      return Observable.FromAsyncPattern<byte[]>((Func<byte[], AsyncCallback, object, IAsyncResult>) ((b, ac, s) => that.BeginWrite(b, 0, data.Length, ac, s)), (Action<IAsyncResult>) (ar =>
      {
        that.EndWrite(ar);
        that.Close();
      }))(data);
    }

    public static IObservable<Unit> WriteAsync(this Stream that, byte[] data)
    {
      return Observable.FromAsyncPattern<byte[]>((Func<byte[], AsyncCallback, object, IAsyncResult>) ((b, ac, s) => that.BeginWrite(b, 0, data.Length, ac, s)), (Action<IAsyncResult>) (ar => that.EndWrite(ar)))(data);
    }

    public static IObservable<byte[]> ReadStreamAsync(this Stream that)
    {
      byte[] buf = new byte[256];
      return Observable.FromAsyncPattern<byte[]>((Func<AsyncCallback, object, IAsyncResult>) ((ac, s) => that.BeginRead(buf, 0, buf.Length, ac, s)), (Func<IAsyncResult, byte[]>) (ar =>
      {
        int length = that.EndRead(ar);
        byte[] destinationArray = new byte[length];
        Array.Copy((Array) buf, (Array) destinationArray, length);
        return destinationArray;
      }))();
    }

    public static IObservable<byte[]> ReadStreamAsync(this Stream that, int maxBytes)
    {
      byte[] buf = new byte[256];
      return Observable.FromAsyncPattern<byte[]>((Func<AsyncCallback, object, IAsyncResult>) ((ac, s) => that.BeginRead(buf, 0, Math.Min(buf.Length, maxBytes), ac, s)), (Func<IAsyncResult, byte[]>) (ar =>
      {
        int length = that.EndRead(ar);
        byte[] destinationArray = new byte[length];
        Array.Copy((Array) buf, (Array) destinationArray, length);
        maxBytes -= length;
        return destinationArray;
      }))();
    }

    public static IObservable<IEvent<SocketAsyncEventArgs>> WriteAsync(
      this Socket that,
      byte[] data,
      int offset,
      int length)
    {
      return Observable.CreateWithDisposable<IEvent<SocketAsyncEventArgs>>((Func<IObserver<IEvent<SocketAsyncEventArgs>>, IDisposable>) (o =>
      {
        SocketAsyncEventArgs args = new SocketAsyncEventArgs();
        return Observable.FromEvent<SocketAsyncEventArgs>((Action<EventHandler<SocketAsyncEventArgs>>) (eh =>
        {
          args.Completed += eh;
          args.SetBuffer(data, offset, length);
          try
          {
            that.SendAsync(args);
          }
          catch (ObjectDisposedException ex)
          {
            o.OnCompleted();
          }
          catch (Exception ex)
          {
            o.OnError(ex);
          }
        }), (Action<EventHandler<SocketAsyncEventArgs>>) (eh => args.Completed -= eh)).Subscribe<IEvent<SocketAsyncEventArgs>>((Action<IEvent<SocketAsyncEventArgs>>) (ievent =>
        {
          o.OnNext(ievent);
          o.OnCompleted();
        }));
      }));
    }

    public static IObservable<IEvent<SocketAsyncEventArgs>> WriteAsync(
      this Socket that,
      byte[] data)
    {
      return that.WriteAsync(data, 0, data.Length);
    }

    public static IObservable<byte[]> ReadAsync(this Socket that, byte[] buf)
    {
      return Observable.CreateWithDisposable<byte[]>((Func<IObserver<byte[]>, IDisposable>) (o =>
      {
        SocketAsyncEventArgs args = new SocketAsyncEventArgs();
        IDisposable disposable = Observable.FromEvent<SocketAsyncEventArgs>((Action<EventHandler<SocketAsyncEventArgs>>) (eh => args.Completed += eh), (Action<EventHandler<SocketAsyncEventArgs>>) (eh => args.Completed -= eh)).Subscribe<IEvent<SocketAsyncEventArgs>>((Action<IEvent<SocketAsyncEventArgs>>) (ievent =>
        {
          if (args.SocketError != SocketError.Success)
          {
            o.OnError((Exception) new SocketException((int) args.SocketError));
          }
          else
          {
            int count = args.Count - args.BytesTransferred;
            int offset = args.Offset + args.BytesTransferred;
            if (count > 0)
            {
              args.SetBuffer(buf, offset, count);
              AsyncExtensions.ReceiveAsync(that, args);
            }
            else
            {
              o.OnNext(buf);
              o.OnCompleted();
            }
          }
        }), (Action<Exception>) (ex =>
        {
          if (ex is ObjectDisposedException)
            o.OnCompleted();
          else
            o.OnError(ex);
        }), (Action) (() => o.OnCompleted()));
        args.SetBuffer(buf, 0, buf.Length);
        try
        {
          AsyncExtensions.ReceiveAsync(that, args);
        }
        catch (ObjectDisposedException ex)
        {
          o.OnCompleted();
        }
        catch (Exception ex)
        {
          o.OnError(ex);
        }
        return disposable;
      }));
    }

    private static bool ReceiveAsync(Socket s, SocketAsyncEventArgs a) => s.ReceiveAsync(a);

    public static IObservable<IEvent<SocketAsyncEventArgs>> GetConnectAsync(
      this Socket that,
      EndPoint endPoint)
    {
      that.NoDelay = true;
      SocketAsyncEventArgs args = new SocketAsyncEventArgs()
      {
        RemoteEndPoint = endPoint
      };
      return Observable.FromEvent<SocketAsyncEventArgs>((Action<EventHandler<SocketAsyncEventArgs>>) (eh =>
      {
        args.Completed += eh;
        that.ConnectAsync(args);
      }), (Action<EventHandler<SocketAsyncEventArgs>>) (eh => args.Completed -= eh)).Take<IEvent<SocketAsyncEventArgs>>(1);
    }

    public static IObservable<IEvent<GeoPositionChangedEventArgs<GeoCoordinate>>> GetGeoPositionAsync(
      this GeoCoordinateWatcher that)
    {
      return Observable.FromEvent<GeoPositionChangedEventArgs<GeoCoordinate>>((Action<EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>>) (eh => that.PositionChanged += eh), (Action<EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>>) (eh => that.PositionChanged -= eh));
    }

    public static IObservable<IEvent<BackgroundTransferEventArgs>> GetTransferProgressChangedAsync(
      this BackgroundTransferRequest that)
    {
      return Observable.FromEvent<BackgroundTransferEventArgs>((Action<EventHandler<BackgroundTransferEventArgs>>) (eh => that.TransferProgressChanged += eh), (Action<EventHandler<BackgroundTransferEventArgs>>) (eh => that.TransferProgressChanged -= eh));
    }

    public static IObservable<IEvent<BackgroundTransferEventArgs>> GetTransferStatusChangedAsync(
      this BackgroundTransferRequest that)
    {
      return Observable.FromEvent<BackgroundTransferEventArgs>((Action<EventHandler<BackgroundTransferEventArgs>>) (eh => that.TransferStatusChanged += eh), (Action<EventHandler<BackgroundTransferEventArgs>>) (eh => that.TransferStatusChanged -= eh));
    }

    public static IObservable<T> LogTime<T>(this IObservable<T> that, string msg) => that;

    public static IObservable<T> LogTime<T>(this IObservable<T> that)
    {
      return that.LogTime<T>((string) null);
    }

    public static IObservable<byte[]> GetResponseBytesAync(
      this WebRequest that,
      Action<long, long> onProgress)
    {
      return Observable.Create<byte[]>((Func<IObserver<byte[]>, Action>) (observer =>
      {
        bool shouldAbort = false;
        IDisposable subscription = AsyncExtensions.GetResponseAsync(that).Subscribe<WebResponse>((Action<WebResponse>) (resp =>
        {
          byte[] numArray = (byte[]) null;
          if (resp is HttpWebResponse httpWebResponse2 && httpWebResponse2.StatusCode < HttpStatusCode.OK || httpWebResponse2.StatusCode > (HttpStatusCode) 299)
            observer.OnError(new Exception("unexpected status code " + (object) httpWebResponse2.StatusCode));
          long contentLength = resp.ContentLength;
          long num = 0;
          if (onProgress != null)
            onProgress(num, contentLength);
          using (Stream responseStream = resp.GetResponseStream())
          {
            using (MemoryStream memoryStream = new MemoryStream())
            {
              byte[] buffer = new byte[4096];
              int count = 0;
              while (!shouldAbort && (count = responseStream.Read(buffer, 0, buffer.Length)) > 0)
              {
                memoryStream.Write(buffer, 0, count);
                num += (long) count;
                if (onProgress != null)
                  onProgress(num, contentLength);
              }
              if (count < 0)
                observer.OnError(new Exception("read returned unexpected value " + (object) count));
              else if (num < contentLength)
                observer.OnError(new Exception(string.Format("expected {0} bytes, got {1}", (object) contentLength, (object) num)));
              else if (!shouldAbort)
                numArray = memoryStream.ToArray();
            }
          }
          if (numArray != null)
            observer.OnNext(numArray);
          if (!shouldAbort)
            return;
          Log.l(nameof (GetResponseBytesAync), "cancelled {0}", (object) that.RequestUri.ToString());
        }), (Action<Exception>) (e => observer.OnError(e)), (Action) (() => observer.OnCompleted()));
        return (Action) (() =>
        {
          if (subscription != null)
          {
            subscription.Dispose();
            subscription = (IDisposable) null;
          }
          shouldAbort = true;
        });
      }));
    }

    public static IObservable<byte[]> GetResponseBytesAync(this WebRequest that)
    {
      return that.GetResponseBytesAync((Action<long, long>) null);
    }

    public static IObservable<PropertyChangedEventArgs> GetPropertyChangedAsync(
      this INotifyPropertyChanged that)
    {
      return Observable.Create<PropertyChangedEventArgs>((Func<IObserver<PropertyChangedEventArgs>, Action>) (observer =>
      {
        PropertyChangedEventHandler eh = (PropertyChangedEventHandler) ((sender, args) => observer.OnNext(args));
        that.PropertyChanged += eh;
        return (Action) (() => that.PropertyChanged -= eh);
      }));
    }

    public static IObservable<T> ObserveOnDispatcherIfNeeded<T>(this IObservable<T> source)
    {
      Dispatcher dispatcher = Deployment.Current.Dispatcher;
      return Observable.CreateWithDisposable<T>((Func<IObserver<T>, IDisposable>) (observer => source.Subscribe<T>((Action<T>) (_ => dispatcher.BeginInvokeIfNeeded((Action) (() =>
      {
        try
        {
          observer.OnNext(_);
        }
        catch (Exception ex)
        {
          observer.OnError(ex);
        }
      }))), (Action<Exception>) (ex => dispatcher.BeginInvokeIfNeeded((Action) (() => observer.OnError(ex)))), (Action) (() => dispatcher.BeginInvokeIfNeeded(new Action(observer.OnCompleted))))));
    }

    public static IObservable<T> ObserveOffDispatcher<T>(this IObservable<T> source)
    {
      return source.ObserveOffDispatcher<T>((IScheduler) AppState.Worker);
    }

    public static IObservable<T> ObserveOffDispatcher<T>(
      this IObservable<T> source,
      IScheduler sched)
    {
      return Observable.CreateWithDisposable<T>((Func<IObserver<T>, IDisposable>) (observer =>
      {
        Dispatcher Dispatcher = Deployment.Current.Dispatcher;
        Action<Action> runOffDispatcher = (Action<Action>) (a =>
        {
          if (Dispatcher.CheckAccess())
            sched.Schedule((Action) (() =>
            {
              try
              {
                a();
              }
              catch (Exception ex)
              {
                observer.OnError(ex);
              }
            }));
          else
            a();
        });
        return source.Subscribe<T>((Action<T>) (item => runOffDispatcher((Action) (() => observer.OnNext(item)))), (Action<Exception>) (ex => observer.OnError(ex)), (Action) (() => observer.OnCompleted()));
      }));
    }

    public static IObservable<T> SynchronizeWithMutex<T>(this IObservable<T> source, Mutex m)
    {
      return source.SynchronizeWithMutex<T>((Action) (() => m.WaitOne()), (Action) (() => m.ReleaseMutex()));
    }

    public static IObservable<T> SynchronizeWithMutex<T>(
      this IObservable<T> source,
      MutexWithWatchdog m)
    {
      return source.SynchronizeWithMutex<T>((Action) (() => m.WaitOne()), (Action) (() => m.ReleaseMutex()));
    }

    public static IObservable<T> SynchronizeWithMutex<T>(
      this IObservable<T> source,
      Action acquire,
      Action release)
    {
      return Observable.CreateWithDisposable<T>((Func<IObserver<T>, IDisposable>) (obs => source.Subscribe<T>((Action<T>) (_ => AsyncExtensions.PerformWithLock((Action) (() => obs.OnNext(_)), acquire, release)), (Action<Exception>) (_ => AsyncExtensions.PerformWithLock((Action) (() => obs.OnError(_)), acquire, release)), (Action) (() => AsyncExtensions.PerformWithLock((Action) (() => obs.OnCompleted()), acquire, release)))));
    }

    public static void PerformWithLock(this MutexWithWatchdog m, Action a)
    {
      AsyncExtensions.PerformWithLock(a, (Action) (() => m.WaitOne()), (Action) (() => m.ReleaseMutex()));
    }

    public static void Finally(this Action action, Action cleanup)
    {
      try
      {
        action();
      }
      finally
      {
        cleanup();
      }
    }

    public static void Using<T>(this T d, Action<T> a) where T : IDisposable
    {
      ((Action) (() => a(d))).Finally(new Action(((IDisposable) d).Dispose));
    }

    public static void PerformWithLock(Action action, Action acquire, Action release)
    {
      acquire();
      action.Finally(release);
    }

    public static IObservable<T> RepeatWhile<T>(this IObservable<T> source, Func<bool> condition)
    {
      return Observable.Create<T>((Func<IObserver<T>, Action>) (observer =>
      {
        Action subscribe = (Action) null;
        object @lock = new object();
        IDisposable currentSub = (IDisposable) null;
        bool cancelled = false;
        subscribe = (Action) (() =>
        {
          lock (@lock)
          {
            currentSub.SafeDispose();
            currentSub = (IDisposable) null;
          }
          if (cancelled || !condition())
            return;
          IDisposable disposable = source.Subscribe<T>((Action<T>) (_ => observer.OnNext(_)), (Action<Exception>) (_ =>
          {
            try
            {
              observer.OnError(_);
            }
            finally
            {
              subscribe();
            }
          }), (Action) (() => subscribe()));
          lock (@lock)
          {
            if (currentSub != null | cancelled || !condition())
              disposable.Dispose();
            else
              currentSub = disposable;
          }
        });
        subscribe();
        return (Action) (() =>
        {
          lock (@lock)
          {
            cancelled = true;
            currentSub.SafeDispose();
            currentSub = (IDisposable) null;
          }
        });
      }));
    }

    public static IObservable<T> SimpleObserveOn<T>(
      this IObservable<T> source,
      IScheduler scheduler)
    {
      if (!(scheduler is ThreadPoolScheduler) && !(scheduler is WAThreadPool.SchedulerImpl))
        return Observable.Create<T>((Func<IObserver<T>, Action>) (observer =>
        {
          object @lock = new object();
          LinkedList<IDisposable> subscriptions = new LinkedList<IDisposable>();
          bool cancel = false;
          Action<Action> schedule = (Action<Action>) (a =>
          {
            LinkedListNode<IDisposable> node = (LinkedListNode<IDisposable>) null;
            IDisposable disposable = scheduler.Schedule((Action) (() =>
            {
              try
              {
                if (cancel)
                  return;
                a();
              }
              finally
              {
                lock (@lock)
                {
                  if (node != null)
                  {
                    if (node.List != null)
                    {
                      node.List.Remove(node);
                      node = (LinkedListNode<IDisposable>) null;
                    }
                  }
                }
              }
            }));
            lock (@lock)
            {
              if (cancel)
                disposable.Dispose();
              else
                node = subscriptions.AddLast(disposable);
            }
          });
          IDisposable sourceSub = source.Subscribe<T>((Action<T>) (_ => schedule((Action) (() => observer.OnNext(_)))), (Action<Exception>) (ex => schedule((Action) (() => observer.OnError(ex)))), (Action) (() => schedule(new Action(observer.OnCompleted))));
          return (Action) (() =>
          {
            lock (@lock)
            {
              cancel = true;
              sourceSub.SafeDispose();
              sourceSub = (IDisposable) null;
              schedule = (Action<Action>) (a => { });
              subscriptions.ToList<IDisposable>().ForEach((Action<IDisposable>) (d => d.SafeDispose()));
              subscriptions.Clear();
            }
          });
        }));
      Log.WriteLineDebug("Warning! SimpleObserveOn() used on thread pool");
      return source.ObserveOn<T>(scheduler);
    }

    public static IObservable<T> SimpleSubscribeOn<T>(
      this IObservable<T> source,
      IScheduler scheduler)
    {
      return Observable.Create<T>((Func<IObserver<T>, Action>) (observer =>
      {
        IDisposable schedSub = (IDisposable) null;
        IDisposable sourceSub = (IDisposable) null;
        object @lock = new object();
        bool cancel = false;
        schedSub = scheduler.Schedule((Action) (() =>
        {
          IDisposable d = source.Subscribe(observer);
          lock (@lock)
          {
            if (cancel)
            {
              d.SafeDispose();
              d = (IDisposable) null;
            }
            sourceSub = d;
          }
        }));
        return (Action) (() =>
        {
          lock (@lock)
          {
            cancel = true;
            sourceSub.SafeDispose();
            sourceSub = (IDisposable) null;
          }
          schedSub.SafeDispose();
          schedSub = (IDisposable) null;
        });
      }));
    }
  }
}
