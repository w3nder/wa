// Decompiled with JetBrains decompiler
// Type: WhatsApp.NativeWeb
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WhatsApp.Resolvers;
using WhatsAppNative;


namespace WhatsApp
{
  public static class NativeWeb
  {
    private static IDisposable keepAliveNetworkSub;
    private static string LogHeaderAysnc = "Nw_Async";

    public static IObservable<Stream> SimpleGet(string url, string ua = null, NativeWeb.Options flags = NativeWeb.Options.Default)
    {
      return NativeWeb.Create<Stream>(flags, (Action<IWebRequest, IObserver<Stream>>) ((req, observer) =>
      {
        MemoryStream mem = new MemoryStream();
        IWebRequest req1 = req;
        string url1 = url;
        NativeWeb.Callback callbackObject = new NativeWeb.Callback();
        callbackObject.OnBytesIn = (Action<byte[]>) (b => mem.Write(b, 0, b.Length));
        callbackObject.OnEndResponse = (Action) (() =>
        {
          mem.Position = 0L;
          observer.OnNext((Stream) mem);
        });
        string userAgent = ua;
        req1.Open(url1, (IWebCallback) callbackObject, userAgent: userAgent);
      }));
    }

    public static IObservable<int> EmptyPost(string url, string ua = null, NativeWeb.Options flags = NativeWeb.Options.Default)
    {
      return NativeWeb.Create<int>(flags, (Action<IWebRequest, IObserver<int>>) ((req, observer) =>
      {
        IWebRequest req1 = req;
        string url1 = url;
        NativeWeb.Callback callbackObject = new NativeWeb.Callback();
        callbackObject.OnBeginResponse = (Action<int, string>) ((code, headers) => observer.OnNext(code));
        callbackObject.OnBytesIn = (Action<byte[]>) (b => { });
        callbackObject.OnEndResponse = (Action) (() => { });
        string userAgent = ua;
        req1.Open(url1, (IWebCallback) callbackObject, "POST", userAgent);
      }));
    }

    public static IObservable<int> Mms4AuthCheck(string url, string ip = null, NativeWeb.Options flags = NativeWeb.Options.Default)
    {
      return NativeWeb.Create<int>(flags, (Action<IWebRequest, IObserver<int>>) ((req, observer) =>
      {
        if (ip != null)
        {
          req.SetTryOriginalHost(false);
          req.SetResolver(new IpResolver() { Ip = ip }.ToNativeResolver());
        }
        req.Open(url, (IWebCallback) new NativeWeb.Callback()
        {
          OnBeginResponse = (Action<int, string>) ((code, headers) => observer.OnNext(code)),
          OnBytesIn = (Action<byte[]>) (b => { }),
          OnEndResponse = (Action) (() => { })
        }, "POST");
      }));
    }

    public static IObservable<MemoryStream> SimplePost(
      string url,
      Dictionary<string, string> postParms,
      NativeWeb.Options flags = NativeWeb.Options.Default)
    {
      return NativeWeb.Create<MemoryStream>(flags, (Action<IWebRequest, IObserver<MemoryStream>>) ((req, observer) =>
      {
        MemoryStream mem = new MemoryStream();
        StringBuilder stringBuilder1 = new StringBuilder();
        foreach (KeyValuePair<string, string> postParm in postParms)
        {
          if (stringBuilder1.Length > 0)
            stringBuilder1.Append("&");
          stringBuilder1.Append(string.Format("{0}={1}", (object) HttpUtility.UrlEncode(postParm.Key), (object) HttpUtility.UrlEncode(postParm.Value)));
        }
        byte[] postParams = Encoding.UTF8.GetBytes(stringBuilder1.ToString());
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        dictionary.Add("Content-Type", "application/x-www-form-urlencoded");
        dictionary.Add("Content-Length", postParams.Length.ToString());
        StringBuilder stringBuilder2 = new StringBuilder();
        foreach (KeyValuePair<string, string> keyValuePair in dictionary)
        {
          if (stringBuilder2.Length > 0)
            stringBuilder2.Append("\r\n");
          stringBuilder2.Append(string.Format("{0}: {1}", (object) keyValuePair.Key, (object) string.Join(" ", new string[1]
          {
            keyValuePair.Value
          })));
        }
        string str = stringBuilder2.ToString();
        IWebRequest req1 = req;
        string url1 = url;
        NativeWeb.Callback callbackObject = new NativeWeb.Callback();
        callbackObject.OnBeginResponse = (Action<int, string>) ((code, headers) =>
        {
          if (code == 200)
            return;
          observer.OnError(new Exception(string.Format("SimplePost Unexpected response {0}", (object) code)));
        });
        callbackObject.OnBytesIn = (Action<byte[]>) (b => mem.Write(b, 0, b.Length));
        callbackObject.OnWrite = (Action<Action<byte[], int, int>>) (a =>
        {
          if (postParams.Length == 0)
            return;
          a(postParams, 0, postParams.Length);
        });
        callbackObject.OnEndResponse = (Action) (() =>
        {
          mem.Position = 0L;
          observer.OnNext(mem);
        });
        string headers1 = str;
        req1.Open(url1, (IWebCallback) callbackObject, "POST", headers: headers1);
      }));
    }

    public static IObservable<int> EmptyDelete(string url, string ua = null, NativeWeb.Options flags = NativeWeb.Options.Default)
    {
      return NativeWeb.Create<int>(flags, (Action<IWebRequest, IObserver<int>>) ((req, observer) =>
      {
        IWebRequest req1 = req;
        string url1 = url;
        NativeWeb.Callback callbackObject = new NativeWeb.Callback();
        callbackObject.OnBeginResponse = (Action<int, string>) ((code, headers) => observer.OnNext(code));
        callbackObject.OnBytesIn = (Action<byte[]>) (b => { });
        callbackObject.OnEndResponse = (Action) (() => { });
        string userAgent = ua;
        req1.Open(url1, (IWebCallback) callbackObject, "DELETE", userAgent);
      }));
    }

    public static IObservable<T> Create<T>(Action<IWebRequest, IObserver<T>> callback)
    {
      return NativeWeb.Create<T>(NativeWeb.Options.Default, callback);
    }

    public static IObservable<T> Create<T>(
      NativeWeb.Options flags,
      Action<IWebRequest, IObserver<T>> callback)
    {
      return Observable.Create<T>((Func<IObserver<T>, Action>) (observer =>
      {
        IWebRequest req = (IWebRequest) null;
        object cancelLock = new object();
        bool cancelled = false;
        ThreadPool.QueueUserWorkItem((WaitCallback) (_ =>
        {
          lock (cancelLock)
          {
            if (cancelled)
              return;
          }
          try
          {
            req = (IWebRequest) NativeInterfaces.CreateInstance<WhatsAppNative.WebRequest>();
            req.SetResolver((flags & NativeWeb.Options.CacheDns) == NativeWeb.Options.None ? Resolver.UncachedNativeInstance : Resolver.NativeInstance);
            req.SetCertificatePinning((flags & NativeWeb.Options.PinCertificate) != 0);
            if ((flags & NativeWeb.Options.KeepAlive) != NativeWeb.Options.None)
            {
              req.SetKeepAlive(true);
              Utils.LazyInit<IDisposable>(ref NativeWeb.keepAliveNetworkSub, (Func<IDisposable>) (() => NetworkStateMonitor.Instance.Observable.Subscribe<NetworkStateChange>((Action<NetworkStateChange>) (nsc =>
              {
                WhatsAppNative.WebRequest instance = NativeInterfaces.CreateInstance<WhatsAppNative.WebRequest>();
                ((IWebRequest) instance).ClearIpCache();
                Marshal.ReleaseComObject((object) instance);
              }))));
            }
            callback(req, observer);
          }
          catch (Exception ex)
          {
            if (ex is OutOfMemoryException)
              Log.SendCrashLog(ex, "web request OOM");
            observer.OnError(ex);
          }
          finally
          {
            observer.OnCompleted();
            lock (cancelLock)
            {
              if (req != null)
              {
                req.Cancel();
                req.Dispose();
                Marshal.ReleaseComObject((object) req);
                req = (IWebRequest) null;
              }
            }
          }
        }));
        return (Action) (() =>
        {
          lock (cancelLock)
          {
            req?.Cancel();
            cancelled = true;
          }
          observer.OnCompleted();
        });
      })).ObserveOn<T>(WAThreadPool.Scheduler);
    }

    public static IEnumerable<KeyValuePair<string, string>> ParseHeaders(string headers)
    {
      string[] strArray = headers.Split(new string[1]
      {
        "\r\n"
      }, StringSplitOptions.None);
      for (int index = 0; index < strArray.Length; ++index)
      {
        string str = strArray[index];
        int length = str.IndexOf(':');
        if (length > 0)
        {
          int num = length + 1;
          while (num < str.Length && str[num] == ' ')
            ++num;
          yield return new KeyValuePair<string, string>(str.Substring(0, length), str.Substring(num));
        }
      }
      strArray = (string[]) null;
    }

    public static void Open(
      this IWebRequest req,
      string url,
      IWebCallback callbackObject,
      string method = null,
      string userAgent = null,
      string headers = null)
    {
      req.OpenImpl(url.IdnToAsciiAbsoluteUriString(), method ?? "GET", userAgent ?? AppState.GetUserAgent(), headers ?? "", callbackObject);
    }

    public static async Task<T> RunRequestAysnc<T>(
      CancellationToken cancellationToken,
      NativeWeb.Options flags,
      Action<DownloadProgress> downloadProgress,
      Action<IWebRequest, Action<T>, Action<DownloadProgress>, CancellationToken> callBack)
      where T : NativeWeb.FileDownloadResultBase
    {
      TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();
      IWebRequest req = (IWebRequest) null;
      object cancelLock = new object();
      if (cancellationToken.IsCancellationRequested)
        throw new OperationCanceledException();
      Action action1 = (Action) (() =>
      {
        Log.d(NativeWeb.LogHeaderAysnc, "onCompleted");
        lock (cancelLock)
        {
          if (req != null)
          {
            req.Cancel();
            req.Dispose();
            Marshal.ReleaseComObject((object) req);
            req = (IWebRequest) null;
          }
        }
        if (!tcs.TrySetCanceled())
          return;
        Log.l(NativeWeb.LogHeaderAysnc, "Request was auto cancelled");
      });
      try
      {
        Utils.IgnoreMultipleInvokes((Action) (() => { }));
        Action<T> action2 = Utils.IgnoreMultipleInvokes<T>((Action<T>) (value =>
        {
          bool cancellationRequested = cancellationToken.IsCancellationRequested;
          bool flag = value.RequestException != null;
          Log.d(NativeWeb.LogHeaderAysnc, "Finished invoked {0} exception? {1}", (object) cancellationRequested, (object) flag);
          if (cancellationRequested || (value.RequestException == null ? tcs.TrySetResult(value) : tcs.TrySetException(value.RequestException)))
            return;
          Log.SendCrashLog((Exception) new InvalidOperationException("Finished processing invoked repeatedly"), "Finished processing invoked repeatedly", logOnlyForRelease: true);
        }));
        req = (IWebRequest) NativeInterfaces.CreateInstance<WhatsAppNative.WebRequest>();
        req.SetResolver((flags & NativeWeb.Options.CacheDns) == NativeWeb.Options.None ? Resolver.UncachedNativeInstance : Resolver.NativeInstance);
        req.SetCertificatePinning((flags & NativeWeb.Options.PinCertificate) != 0);
        if ((flags & NativeWeb.Options.KeepAlive) != NativeWeb.Options.None)
        {
          req.SetKeepAlive(true);
          Utils.LazyInit<IDisposable>(ref NativeWeb.keepAliveNetworkSub, (Func<IDisposable>) (() => NetworkStateMonitor.Instance.Observable.Subscribe<NetworkStateChange>((Action<NetworkStateChange>) (nsc =>
          {
            WhatsAppNative.WebRequest instance = NativeInterfaces.CreateInstance<WhatsAppNative.WebRequest>();
            ((IWebRequest) instance).ClearIpCache();
            Marshal.ReleaseComObject((object) instance);
          }))));
        }
        callBack(req, action2, downloadProgress, cancellationToken);
      }
      catch (Exception ex)
      {
        if (ex is OutOfMemoryException)
          Log.SendCrashLog(ex, "web request OOM");
        tcs.SetException(ex);
      }
      finally
      {
        action1();
      }
      return await tcs.Task;
    }

    public static async Task<NativeWeb.StreamPlusException> SimpleGetAsync(
      string url,
      string ua = null,
      NativeWeb.Options flags = NativeWeb.Options.Default)
    {
      CancellationToken cancellationToken = new CancellationToken();
      Action<DownloadProgress> action = (Action<DownloadProgress>) (progress => Log.l("nw", "Progess {0} {1}", (object) progress.DownloadedSoFar, (object) progress.TotalToDownload));
      int flags1 = (int) flags;
      Action<DownloadProgress> downloadProgress = action;
      Action<IWebRequest, Action<NativeWeb.StreamPlusException>, Action<DownloadProgress>, CancellationToken> callBack = (Action<IWebRequest, Action<NativeWeb.StreamPlusException>, Action<DownloadProgress>, CancellationToken>) ((req, onNext, progressReport, cancelToken) =>
      {
        MemoryStream mem = new MemoryStream();
        try
        {
          IWebRequest req1 = req;
          string url1 = url;
          NativeWeb.Callback callbackObject = new NativeWeb.Callback();
          callbackObject.OnBytesIn = (Action<byte[]>) (b => mem.Write(b, 0, b.Length));
          callbackObject.OnEndResponse = (Action) (() =>
          {
            mem.Position = 0L;
            NativeWeb.StreamPlusException streamPlusException = new NativeWeb.StreamPlusException();
            streamPlusException.RespStream = (Stream) mem;
            Thread.Sleep(20000);
            onNext(streamPlusException);
          });
          string userAgent = ua;
          req1.Open(url1, (IWebCallback) callbackObject, userAgent: userAgent);
        }
        catch (Exception ex)
        {
          onNext(new NativeWeb.StreamPlusException()
          {
            RequestException = ex
          });
        }
      });
      return await NativeWeb.RunRequestAysnc<NativeWeb.StreamPlusException>(cancellationToken, (NativeWeb.Options) flags1, downloadProgress, callBack);
    }

    public enum Options
    {
      None,
      PinCertificate,
      CacheDns,
      Default,
      KeepAlive,
    }

    public class Callback : IWebCallback
    {
      public Action<Action<byte[], int, int>> OnWrite;
      public Action<byte[]> OnBytesIn;
      public Action OnEndResponse;
      public Action<int, string> OnBeginResponse;
      private int? ResponseCode;
      private string Headers;
      public long createTimeUtcTicks = DateTime.UtcNow.Ticks;
      public long writeStartTimeUtcTicks = -1;

      public void OnResponseCode(int code)
      {
        this.ResponseCode = new int?(code);
        this.TryOnBeginResponse();
      }

      public void OnHeaders(string headers)
      {
        this.Headers = headers;
        this.TryOnBeginResponse();
      }

      private void TryOnBeginResponse()
      {
        if (this.OnBeginResponse == null)
        {
          this.Headers = (string) null;
        }
        else
        {
          if (!this.ResponseCode.HasValue || this.Headers == null)
            return;
          this.OnBeginResponse(this.ResponseCode.Value, this.Headers);
        }
      }

      public void ResponseBytesIn(IByteBuffer bb)
      {
        Action<byte[]> onBytesIn = this.OnBytesIn;
        if (onBytesIn == null)
          return;
        onBytesIn(bb.Get());
      }

      public void EndResponse()
      {
        Action onEndResponse = this.OnEndResponse;
        if (onEndResponse == null)
          return;
        onEndResponse();
      }

      public void Write(IWebWriter writer)
      {
        if (this.writeStartTimeUtcTicks < 0L)
          this.writeStartTimeUtcTicks = DateTime.UtcNow.Ticks;
        if (this.OnWrite == null)
          return;
        IByteBuffer bb = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
        this.OnWrite((Action<byte[], int, int>) ((b, off, len) =>
        {
          bb.Put(b, off, len);
          writer.Write(bb);
          bb.Reset();
        }));
      }
    }

    public class FileDownloadResultBase
    {
      public Exception RequestException;
    }

    public class StreamPlusException : NativeWeb.FileDownloadResultBase
    {
      public Exception Exc { get; private set; }

      public Stream RespStream { get; set; }
    }
  }
}
