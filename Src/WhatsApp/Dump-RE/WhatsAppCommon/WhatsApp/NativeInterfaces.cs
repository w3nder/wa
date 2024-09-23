// Decompiled with JetBrains decompiler
// Type: WhatsApp.NativeInterfaces
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using WhatsAppNative;

#nullable disable
namespace WhatsApp
{
  public static class NativeInterfaces
  {
    private static IMisc misc = (IMisc) null;
    private static object miscLock = new object();
    private static IMp4Utils mp4Utils;
    private static IMediaLibrary mediaLib;
    private static IWebp webp;

    public static T CreateInstance<T>() where T : new()
    {
      try
      {
        return new T();
      }
      catch (Exception ex)
      {
        if (ex.HResult == -2147417842)
        {
          T instance = default (T);
          Dispatcher dispatcher = Deployment.Current.Dispatcher;
          if (!NativeInterfaces.ScheduleWithTimeout<T>((Func<T>) (() => new T()), (Action<Action>) (a => new Thread((ThreadStart) (() => a())).Start()), new TimeSpan?(), out instance) && !dispatcher.CheckAccess())
            NativeInterfaces.ScheduleWithTimeout<T>((Func<T>) (() => new T()), (Action<Action>) (a => dispatcher.BeginInvoke(a)), new TimeSpan?(TimeSpan.FromSeconds(5.0)), out instance);
          if ((object) instance != null)
          {
            Log.SendCrashLog(new Exception("beta only: CreateInstance hack worked!"), nameof (CreateInstance));
            return instance;
          }
          Thread.Sleep(250);
        }
        else
          throw;
      }
      return new T();
    }

    public static bool ScheduleWithTimeout<T>(
      Func<T> ctor,
      Action<Action> scheduler,
      TimeSpan? timeout,
      out T value)
      where T : new()
    {
      ManualResetEvent ev = new ManualResetEvent(false);
      int refCount = 2;
      Action unref = (Action) (() =>
      {
        if (Interlocked.Decrement(ref refCount) != 0)
          return;
        ev.Dispose();
      });
      T r = default (T);
      try
      {
        scheduler((Action) (() =>
        {
          try
          {
            r = ctor();
          }
          catch (Exception ex)
          {
            Log.LogException(ex, "scheduler thread");
          }
          finally
          {
            ev.Set();
            unref();
          }
        }));
      }
      catch (Exception ex)
      {
        ev.Dispose();
        throw;
      }
      if (timeout.HasValue)
        ev.WaitOne(timeout.Value);
      else
        ev.WaitOne();
      unref();
      value = r;
      return (object) value != null;
    }

    public static void InitLogSink()
    {
      if (!Debugger.IsAttached)
        return;
      NativeInterfaces.Misc.SetLogSink((ILogSink) new NativeInterfaces.NativeLogSink());
    }

    public static IMisc Misc
    {
      get
      {
        return Utils.LazyInit<IMisc>(ref NativeInterfaces.misc, (Func<IMisc>) (() => (IMisc) NativeInterfaces.CreateInstance<WhatsAppNative.Misc>()), NativeInterfaces.miscLock);
      }
    }

    public static IMediaMisc MediaMisc => (IMediaMisc) NativeInterfaces.CreateInstance<WhatsAppNative.MediaMisc>();

    public static IMp4Utils Mp4Utils
    {
      get
      {
        return Utils.LazyInit<IMp4Utils>(ref NativeInterfaces.mp4Utils, (Func<IMp4Utils>) (() => (IMp4Utils) NativeInterfaces.CreateInstance<WhatsAppNative.Mp4Utils>()));
      }
    }

    public static IMediaLibrary MediaLib
    {
      get
      {
        return Utils.LazyInit<IMediaLibrary>(ref NativeInterfaces.mediaLib, (Func<IMediaLibrary>) (() => (IMediaLibrary) NativeInterfaces.CreateInstance<MediaLibraryWrapper>()));
      }
    }

    public static IWebp WebP
    {
      get
      {
        return Utils.LazyInit<IWebp>(ref NativeInterfaces.webp, (Func<IWebp>) (() => (IWebp) NativeInterfaces.CreateInstance<NativeWebp>()));
      }
    }

    private class NativeLogSink : ILogSink
    {
      public void OnLogMessage(string msg)
      {
        if (msg.Length == 0 || msg[msg.Length - 1] != '\n')
          return;
        msg = msg.Substring(0, msg.Length - 1);
      }
    }
  }
}
