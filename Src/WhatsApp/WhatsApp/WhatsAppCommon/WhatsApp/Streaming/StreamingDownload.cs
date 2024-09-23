// Decompiled with JetBrains decompiler
// Type: WhatsApp.Streaming.StreamingDownload
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using WhatsAppNative;


namespace WhatsApp.Streaming
{
  public class StreamingDownload : IDisposable
  {
    private Message message;
    private AxolotlMediaCipher mediaCipher;
    private SidecarVerifier sidecar;
    private WriteToReadPipe decryptPipe = new WriteToReadPipe();
    private IDisposable transferSub;
    private StreamingDownloadFileSource fileSource;
    private object threadStartSentinel;
    private string outputFilename;
    private TreeInForestSubject<Unit> mediaTransferObs = new TreeInForestSubject<Unit>();
    private WhatsApp.Events.MediaDownload streamingDownloadEvent;
    private DateTime streamStartTime;

    public IStreamingFileSource FileSource => (IStreamingFileSource) this.fileSource;

    public StreamingDownload(Message message)
    {
      this.message = message;
      this.fileSource = new StreamingDownloadFileSource(this, message.MediaSize);
    }

    private void wrapExn(Action inner, bool rethrow, Action<Exception> onError = null)
    {
      if (onError == null)
        onError = new Action<Exception>(this.OnError);
      try
      {
        inner();
      }
      catch (Exception ex)
      {
        WhatsApp.MediaDownload.SetMessageErrorFromException(this.message, ex);
        onError(ex);
        if (!rethrow)
          return;
        throw;
      }
    }

    public void Start()
    {
      this.streamingDownloadEvent = FieldStats.GetFsMediaDownloadEvent(this.message);
      this.streamStartTime = DateTime.Now;
      this.wrapExn((Action) (() =>
      {
        if (this.message == null || !this.message.HasSidecar())
          throw new Mp4CannotStreamException("Sidecar missing.");
        if (WhatsApp.MediaDownload.FindDuplicateImpl(this.message) != null)
        {
          this.streamingDownloadEvent.mediaDownloadResult = new wam_enum_media_download_result_type?(wam_enum_media_download_result_type.DEDUPED);
          throw new Mp4CannotStreamException("Duplicate message exists.");
        }
        this.message.SetPendingMediaSubscription("streaming", PendingMediaTransfer.TransferTypes.Download_Foreground_Interactive_Streaming, (IObservable<Unit>) this.mediaTransferObs, disposeExisting: true);
        this.mediaCipher = AxolotlMediaCipher.CreateDownloadCipher(this.message);
        this.sidecar = this.mediaCipher.SidecarVerifier;
        this.sidecar.OnBytes = new Action<byte[], int, int>(this.decryptPipe.Write);
        string inAppPath;
        WhatsApp.MediaDownload.GetFileNames(this.message, out string _, out inAppPath);
        this.outputFilename = inAppPath;
        this.transferSub = this.GetTransferObservableMaybeMms4(MediaStorage.OpenFile(this.outputFilename, FileMode.OpenOrCreate, FileAccess.ReadWrite), this.message).Subscribe<Unit>((Action<Unit>) (unit => { }), new Action<Exception>(this.OnError));
      }), false);
    }

    private IObservable<Unit> GetTransferObservableMaybeMms4(Stream encFile, Message msg)
    {
      if (!Mms4Helper.IsMms4DownloadMessage(msg))
        return this.GetTransferObservable(encFile, msg.MediaUrl, (string) null, -1L);
      FunXMPP.FMessage.FunMediaType funType = msg.GetFunMediaType();
      byte[] cipherHash = msg.GetCipherMediaHash();
      long routeStartTimeUtcTicks = DateTime.UtcNow.Ticks;
      return Mms4HostSelector.GetInstance().GetSelectedHostObservable(true, msg.GetFunMediaType(), false).SelectMany<Mms4HostSelector.Mms4HostSelection, Unit, Unit>((Func<Mms4HostSelector.Mms4HostSelection, IObservable<Unit>>) (hostSelection => this.GetTransferObservable(encFile, Mms4Helper.CreateUrlStringForDownload(hostSelection.HostName, funType, cipherHash, false, msg.GetDirectPath()), (string) null, routeStartTimeUtcTicks)), (Func<Mms4HostSelector.Mms4HostSelection, Unit, Unit>) ((hostSelection, obs) => obs));
    }

    private IObservable<Unit> GetTransferObservable(
      Stream encFile,
      string downloadUrl,
      string mms4IpHint,
      long mms4RouteStartUtcTicks)
    {
      bool isMms4 = mms4RouteStartUtcTicks > 0L;
      if (isMms4 && this.streamingDownloadEvent != null)
        this.streamingDownloadEvent.routeSelectionDelayT = new long?((DateTime.UtcNow.Ticks - mms4RouteStartUtcTicks) / 10000L);
      long bytesSentToCrypto = 0;
      IObservable<Unit> request = NativeWeb.Create<Unit>(isMms4 ? NativeWeb.Options.Default | NativeWeb.Options.KeepAlive : NativeWeb.Options.Default, (Action<IWebRequest, IObserver<Unit>>) ((req, observer) =>
      {
        string str1 = (string) null;
        long fileStart = encFile.Length;
        long skip = 0;
        if (fileStart > 1L)
        {
          string str2 = string.Format("bytes={0}-", (object) (fileStart - 1L));
          Log.WriteLineDebug("using Range: {0}", (object) str2);
          str1 = string.Format("Range: {0}\r\n", (object) str2);
        }
        else
          fileStart = 0L;
        IWebRequest req1 = req;
        string url = downloadUrl;
        NativeWeb.Callback callbackObject = new NativeWeb.Callback();
        callbackObject.OnBeginResponse = (Action<int, string>) ((code, headersString) => this.wrapExn((Action) (() =>
        {
          if (isMms4)
            Mms4RouteSelector.GetInstance().OnMediaTransferErrorOrResponseCode(code);
          if (code < 200 || code > 299)
            throw new HttpStatusException((HttpStatusCode) code);
          Dictionary<string, string> dictionary = NativeWeb.ParseHeaders(headersString).ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (kv => kv.Key.ToLowerInvariant()), (Func<KeyValuePair<string, string>, string>) (kv => kv.Value));
          string range;
          if (dictionary.TryGetValue("content-range", out range))
          {
            WhatsApp.MediaDownload.ParsedContentRange contentRange = WhatsApp.MediaDownload.ParseContentRange(range);
            fileStart = contentRange.Start;
            this.OnContentLengthKnown(contentRange.FileSize ?? contentRange.End + 1L);
          }
          else
          {
            string s;
            long result;
            if (!dictionary.TryGetValue("content-length", out s) || !long.TryParse(s, out result))
              throw new InvalidOperationException("Content length missing");
            this.OnContentLengthKnown(result);
          }
          encFile.Position = bytesSentToCrypto;
          if (fileStart < bytesSentToCrypto)
          {
            skip = bytesSentToCrypto - fileStart;
          }
          else
          {
            long val2 = fileStart - bytesSentToCrypto;
            byte[] buffer = (byte[]) null;
            int len;
            for (; val2 != 0L; val2 -= (long) len)
            {
              if (buffer == null)
                buffer = new byte[4096];
              int count = (int) Math.Min((long) buffer.Length, val2);
              len = encFile.Read(buffer, 0, count);
              if (len != count)
                throw new IOException("Short read");
              this.OnBytesFromSocket(buffer, 0, len);
              bytesSentToCrypto += (long) len;
            }
          }
        }), true));
        callbackObject.OnBytesIn = (Action<byte[]>) (b => this.wrapExn((Action) (() =>
        {
          int length = b.Length;
          int offset = (int) Math.Min((long) length, skip);
          int num = length - offset;
          encFile.Write(b, offset, num);
          this.OnBytesFromSocket(b, offset, num);
          bytesSentToCrypto += (long) num;
        }), true));
        callbackObject.OnEndResponse = (Action) (() => this.wrapExn((Action) (() => this.OnEndOfSocket()), true));
        string headers = str1;
        req1.Open(url, (IWebCallback) callbackObject, headers: headers);
      }));
      return request.Catch<Unit, Exception>((Func<Exception, IObservable<Unit>>) (ex => ((IEnumerable<uint>) new uint[3]
      {
        2147954402U,
        2147954429U,
        2147954431U
      }).Contains<uint>(ex.GetHResult()) ? request : Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        observer.OnError(ex);
        return (Action) (() => { });
      })))).Take<Unit>(1).Finally<Unit>(new Action(encFile.Dispose));
    }

    private void OnContentLengthKnown(long contentLength)
    {
      if (this.streamingDownloadEvent == null)
        this.streamingDownloadEvent = FieldStats.GetFsMediaDownloadEvent(this.message);
      this.streamingDownloadEvent.connectT = new long?((long) DateTime.Now.Subtract(this.streamStartTime).TotalMilliseconds);
      Utils.LazyInit<object>(ref this.threadStartSentinel, (Func<object>) (() =>
      {
        new Thread((ThreadStart) (() => this.wrapExn((Action) (() => this.DecryptThread(contentLength, this.streamingDownloadEvent)), false))).Start();
        return new object();
      }));
    }

    private void OnBytesFromSocket(byte[] buffer, int offset, int len)
    {
      this.sidecar.OnBytesIn(buffer, offset, len);
    }

    private void OnEndOfSocket()
    {
      this.ReportStreamingDownloadEvent();
      this.sidecar.Final();
      this.decryptPipe.OnEndOfFile();
    }

    private void OnError(Exception ex)
    {
      this.ReportStreamingDownloadEvent(ex);
      this.fileSource.OnError(ex.GetRethrowAction());
      this.mediaTransferObs.OnError(ex);
      this.mediaTransferObs.OnCompleted();
    }

    private void ReportStreamingDownloadEvent(Exception ex = null)
    {
      DateTime streamStartTime = this.streamStartTime;
      if (this.streamingDownloadEvent == null)
        return;
      this.streamingDownloadEvent.isStreamingMedia = new bool?(true);
      this.streamingDownloadEvent.mediaDownloadT = new long?((long) DateTime.Now.Subtract(this.streamStartTime).TotalMilliseconds);
      if (ex == null)
      {
        this.streamingDownloadEvent.mediaDownloadResult = new wam_enum_media_download_result_type?(wam_enum_media_download_result_type.OK);
        this.streamingDownloadEvent.networkDownloadT = this.streamingDownloadEvent.mediaDownloadT;
      }
      else
      {
        if (ex is Mp4CannotStreamException && !this.streamingDownloadEvent.mediaDownloadResult.HasValue)
        {
          Log.d("Not reporting streaming download event as the video was not streamable.");
          return;
        }
        if (ex is HttpStatusException)
        {
          switch ((ex as HttpStatusException).StatusCode)
          {
            case HttpStatusCode.Unauthorized:
              this.streamingDownloadEvent.mediaDownloadResult = new wam_enum_media_download_result_type?(wam_enum_media_download_result_type.ERROR_INVALID_URL);
              break;
            case HttpStatusCode.NotFound:
            case HttpStatusCode.Gone:
              this.streamingDownloadEvent.mediaDownloadResult = new wam_enum_media_download_result_type?(wam_enum_media_download_result_type.ERROR_TOO_OLD);
              break;
            case HttpStatusCode.RequestTimeout:
              this.streamingDownloadEvent.mediaDownloadResult = new wam_enum_media_download_result_type?(wam_enum_media_download_result_type.ERROR_TIMEOUT);
              break;
            case HttpStatusCode.RequestedRangeNotSatisfiable:
              this.streamingDownloadEvent.mediaDownloadResult = new wam_enum_media_download_result_type?(wam_enum_media_download_result_type.ERROR_CANNOT_RESUME);
              break;
            case HttpStatusCode.GatewayTimeout:
              this.streamingDownloadEvent.mediaDownloadResult = new wam_enum_media_download_result_type?(wam_enum_media_download_result_type.ERROR_DNS);
              break;
            default:
              this.streamingDownloadEvent.mediaDownloadResult = new wam_enum_media_download_result_type?(wam_enum_media_download_result_type.ERROR_UNKNOWN);
              break;
          }
        }
        else if (!this.streamingDownloadEvent.mediaDownloadResult.HasValue)
          this.streamingDownloadEvent.mediaDownloadResult = new wam_enum_media_download_result_type?(wam_enum_media_download_result_type.ERROR_UNKNOWN);
      }
      this.streamingDownloadEvent.SaveEvent();
    }

    private void OnBytesFromDecrypt(byte[] buffer, int offset, int len)
    {
      this.fileSource.Write(buffer, offset, len);
    }

    private void OnEndOfDecrypt() => this.fileSource.OnEndOfFile();

    private void DecryptThread(long contentLength, WhatsApp.Events.MediaDownload fsEvent)
    {
      AxolotlMediaCipher mediaCipher = Interlocked.Exchange<AxolotlMediaCipher>(ref this.mediaCipher, (AxolotlMediaCipher) null);
      if (mediaCipher == null)
        throw new OperationCanceledException();
      try
      {
        mediaCipher.DecryptMedia(this.message, new Func<byte[], int, int, int>(this.decryptPipe.Read), (Func<long>) (() => this.decryptPipe.TotalBytesRead), contentLength, new Action<byte[], int, int>(this.OnBytesFromDecrypt), fsEvent);
        this.OnEndOfDecrypt();
      }
      catch (Exception ex)
      {
        mediaCipher.SafeDispose();
        throw;
      }
      WAThreadPool.QueueUserWorkItem((Action) (() =>
      {
        using (mediaCipher)
        {
          bool callback = false;
          WhatsApp.MediaDownload.StoreMediaPathToDb(this.message, this.fileSource.PlaintextFilename, new List<Action<Message>>(), new List<string>()
          {
            this.outputFilename
          }, (WhatsApp.MediaDownload.MediaProgress) null, mediaCipher.CipherMediaHash, mediaCipher.GenerateMediaRef(Settings.MyJid), ref callback);
        }
        this.mediaTransferObs.OnCompleted();
      }));
    }

    public void Dispose()
    {
      this.transferSub.SafeDispose();
      this.transferSub = (IDisposable) null;
      Interlocked.Exchange<AxolotlMediaCipher>(ref this.mediaCipher, (AxolotlMediaCipher) null).SafeDispose();
      this.decryptPipe.OnEndOfFile();
      this.mediaTransferObs.OnCompleted();
      if (this.message == null || this.message.IsStatus() || this.message.LocalFileUri != null)
        return;
      WhatsApp.Events.MediaDownload mediaDownloadEvent = FieldStats.GetFsMediaDownloadEvent(this.message);
      this.message.SetPendingMediaSubscription("post-streaming download", PendingMediaTransfer.TransferTypes.Download_Foreground_NotInteractive, WhatsApp.MediaDownload.TransferForMessageObservable(this.message, WhatsApp.MediaDownload.TransferFromForeground(this.message, mediaDownloadEvent, false), mediaDownloadEvent));
    }
  }
}
