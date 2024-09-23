// Decompiled with JetBrains decompiler
// Type: WhatsApp.MediaDownloadManager
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using WhatsApp.Resolvers;
using WhatsAppNative;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.Web;


namespace WhatsApp
{
  public class MediaDownloadManager
  {
    private static MediaDownloadProgress paramError = new MediaDownloadProgress(MediaDownloadProgress.DownloadStatus.Completed)
    {
      HttpRespCode = -1
    };
    private static string LogHeader = "Md_Mgr";
    private static object createLock = new object();
    private static MediaDownloadManager _downloadManagerInstance;
    private object queueLock = new object();
    private List<MediaDownloadManager.DownloadQueueEntry> queuedEntries;
    private WorkQueue scheduler;
    private object schedulerLock = new object();
    private static List<BackgroundDownloadDetails> activeBackgroundDownloads = new List<BackgroundDownloadDetails>();
    private static List<DownloadOperation> runningBackgroundDownloads = new List<DownloadOperation>();

    public static IObservable<MediaDownloadProgress> DownloadEncryptedFile(
      string id,
      FunXMPP.FMessage.FunMediaType funMediaType,
      byte[] mediaHash,
      byte[] encryptedFileHash,
      byte[] encryptedKey,
      long downloadSize,
      string outputfilename,
      MediaHelper.DownloadContextOptions downloadContext)
    {
      return encryptedFileHash == null || encryptedFileHash.Length == 0 || encryptedKey == null || encryptedKey.Length == 0 ? Observable.Return<MediaDownloadProgress>(MediaDownloadManager.paramError) : MediaDownloadManager.DownloadMaybeEncryptedFile(id, funMediaType, mediaHash, encryptedFileHash, encryptedKey, downloadSize, outputfilename, downloadContext);
    }

    private static IObservable<MediaDownloadProgress> DownloadMaybeEncryptedFile(
      string id,
      FunXMPP.FMessage.FunMediaType funMediaType,
      byte[] mediaHash,
      byte[] encryptedFileHash,
      byte[] encryptedKey,
      long downloadSize,
      string outputfilename,
      MediaHelper.DownloadContextOptions downloadContext)
    {
      return string.IsNullOrEmpty(outputfilename) || mediaHash == null || mediaHash.Length == 0 ? Observable.Return<MediaDownloadProgress>(MediaDownloadManager.paramError) : Observable.Create<MediaDownloadProgress>((Func<IObserver<MediaDownloadProgress>, Action>) (observer =>
      {
        MediaDownloadManager.DownloadQueueEntry newEntry = new MediaDownloadManager.DownloadQueueEntry(new DownloadMedia(funMediaType, id, encryptedFileHash, encryptedKey, outputfilename, downloadSize, downloadContext), observer);
        MediaDownloadManager.DownloadQueueEntry queuedEntry = MediaDownloadManager.GetInstance().AddIfNewEntry(newEntry);
        MediaDownloadManager.PerformNextDownload();
        return Utils.IgnoreMultipleInvokes((Action) (() =>
        {
          if (queuedEntry.DownloadEntry.DownloadStatus == DownloadMedia.DownloadStatuses.Finished)
            return;
          MediaDownloadManager.CancelMediaDownload(queuedEntry);
        }));
      }));
    }

    private static void CancelMediaDownload(
      MediaDownloadManager.DownloadQueueEntry queuedEntry)
    {
      Log.d(MediaDownloadManager.LogHeader, "Cancelling entry {0}", (object) (queuedEntry?.DownloadEntry?.DownloadId ?? "null"));
      if (queuedEntry == null)
        return;
      MediaDownloadManager instance = MediaDownloadManager.GetInstance();
      if (queuedEntry.DownloadEntry.StatusChange(new DownloadMedia.DownloadStatuses[2]
      {
        DownloadMedia.DownloadStatuses.Unknown,
        DownloadMedia.DownloadStatuses.Queued
      }, DownloadMedia.DownloadStatuses.Cancelled).HasValue)
      {
        Log.l(MediaDownloadManager.LogHeader, "Removed unprocessed entry {0} {1}", (object) queuedEntry.DownloadEntry.DownloadType, (object) queuedEntry.DownloadEntry.DownloadId);
        instance.DelEntry(queuedEntry);
      }
      else
      {
        try
        {
          queuedEntry.CancelDownload();
          instance.DelEntry(queuedEntry);
        }
        catch (Exception ex)
        {
          Log.SendCrashLog(ex, "Cancelling donwload", logOnlyForRelease: true);
        }
      }
    }

    public static MediaDownloadManager GetInstance()
    {
      if (MediaDownloadManager._downloadManagerInstance == null)
      {
        lock (MediaDownloadManager.createLock)
        {
          if (MediaDownloadManager._downloadManagerInstance == null)
            MediaDownloadManager._downloadManagerInstance = new MediaDownloadManager();
        }
      }
      return MediaDownloadManager._downloadManagerInstance;
    }

    private List<MediaDownloadManager.DownloadQueueEntry> InitQueue()
    {
      if (this.queuedEntries == null)
        this.queuedEntries = new List<MediaDownloadManager.DownloadQueueEntry>();
      return this.queuedEntries;
    }

    private MediaDownloadManager.DownloadQueueEntry GetEntry(
      FunXMPP.FMessage.FunMediaType type,
      string id)
    {
      lock (this.queueLock)
      {
        if (this.queuedEntries == null)
          this.queuedEntries = this.InitQueue();
        foreach (MediaDownloadManager.DownloadQueueEntry queuedEntry in this.queuedEntries)
        {
          if (queuedEntry.DownloadEntry.DownloadId == id && queuedEntry.DownloadEntry.DownloadType == type)
            return queuedEntry;
        }
      }
      return (MediaDownloadManager.DownloadQueueEntry) null;
    }

    private MediaDownloadManager.DownloadQueueEntry AddIfNewEntry(
      MediaDownloadManager.DownloadQueueEntry newEntry)
    {
      lock (this.queueLock)
      {
        MediaDownloadManager.DownloadQueueEntry entry = this.GetEntry(newEntry.DownloadEntry.DownloadType, newEntry.DownloadEntry.DownloadId);
        if (entry != null)
          return entry;
        if (!newEntry.DownloadEntry.StatusChange(DownloadMedia.DownloadStatuses.Unknown, DownloadMedia.DownloadStatuses.Queued))
          throw new InvalidOperationException("Can't start download, when in state " + (object) newEntry.DownloadEntry.DownloadStatus);
        this.queuedEntries.Add(newEntry);
      }
      return newEntry;
    }

    private bool DelEntry(MediaDownloadManager.DownloadQueueEntry oldEntry)
    {
      lock (this.queueLock)
      {
        if (this.GetEntry(oldEntry.DownloadEntry.DownloadType, oldEntry.DownloadEntry.DownloadId) == null)
          return false;
        this.queuedEntries.Remove(oldEntry);
      }
      return true;
    }

    private MediaDownloadManager.DownloadQueueEntry GetNextEntryToProcess()
    {
      MediaDownloadManager.DownloadQueueEntry nextEntryToProcess = (MediaDownloadManager.DownloadQueueEntry) null;
      lock (this.queueLock)
      {
        if (this.queuedEntries == null)
          this.queuedEntries = this.InitQueue();
        Log.d(MediaDownloadManager.LogHeader, "Selecting from {0} entries", (object) this.queuedEntries.Count);
        foreach (MediaDownloadManager.DownloadQueueEntry queuedEntry in this.queuedEntries)
        {
          if (queuedEntry.DownloadEntry.StatusChange(DownloadMedia.DownloadStatuses.Queued, DownloadMedia.DownloadStatuses.Running))
          {
            if (nextEntryToProcess == null)
            {
              Log.l(MediaDownloadManager.LogHeader, "Selected entry {0}, {1}", (object) queuedEntry.DownloadEntry.DownloadType, (object) queuedEntry.DownloadEntry.DownloadId);
              nextEntryToProcess = queuedEntry;
            }
            if (MediaHelper.IsUserWaiting(queuedEntry.DownloadEntry.DownloadContext))
            {
              nextEntryToProcess = queuedEntry;
              break;
            }
            if (MediaHelper.IsUserRequest(queuedEntry.DownloadEntry.DownloadContext) && !MediaHelper.IsUserRequest(nextEntryToProcess.DownloadEntry.DownloadContext))
              nextEntryToProcess = queuedEntry;
          }
        }
        if (nextEntryToProcess == null)
        {
          if (this.queuedEntries.Count > 0)
          {
            Log.l(MediaDownloadManager.LogHeader, "Did not select from {0} entries", (object) this.queuedEntries.Count);
            foreach (MediaDownloadManager.DownloadQueueEntry queuedEntry in this.queuedEntries)
              Log.d(MediaDownloadManager.LogHeader, "Details {0} {1}", (object) queuedEntry.DownloadEntry.DownloadId, (object) queuedEntry.DownloadEntry.DownloadStatus);
          }
        }
      }
      return nextEntryToProcess;
    }

    private WorkQueue Scheduler
    {
      get
      {
        return Utils.LazyInit<WorkQueue>(ref this.scheduler, (Func<WorkQueue>) (() => new WorkQueue(identifierString: "MediaDownloadManager.Scheduler")), this.schedulerLock);
      }
    }

    private static void PerformNextDownload()
    {
      MediaDownloadManager.GetInstance().GetNextEntryToProcess()?.Download();
    }

    private static IObservable<MediaDownloadProgress> DownloadFromBackground(
      string url,
      string ipHint,
      string outputFileName,
      int downloadMaxBytes,
      long downloadSize,
      WhatsApp.Events.MediaDownload fsEvent)
    {
      return MediaDownloadManager.InAppDownload(url, ipHint, outputFileName, true, downloadMaxBytes, downloadSize, fsEvent);
    }

    public static IObservable<MediaDownloadProgress> DownloadFromForeground(
      string url,
      string ipHint,
      string outputFileName,
      int downloadMaxBytes,
      long downloadSize,
      WhatsApp.Events.MediaDownload fsEvent,
      bool interactive)
    {
      Log.d(MediaDownloadManager.LogHeader, "Media size: {0}", (object) downloadSize);
      if (interactive && NetworkStateMonitor.Is2GConnection() || downloadSize > 0L && downloadSize < 1048576L)
        return MediaDownloadManager.InAppDownload(url, ipHint, outputFileName, true, 0, downloadSize, fsEvent);
      if (downloadMaxBytes > 0)
      {
        Assert.IsTrue(Settings.VideoPrefetchBytes < 1048576, string.Format("Cannot prefetch more than {0} bytes of a video while using InAppDownload", (object) 1048576L));
        return MediaDownloadManager.InAppDownload(url, ipHint, outputFileName, true, downloadMaxBytes, downloadSize, fsEvent);
      }
      return !interactive ? MediaDownloadManager.BackgroundTransfer(url, outputFileName) : Observable.Create<MediaDownloadProgress>((Func<IObserver<MediaDownloadProgress>, Action>) (observer =>
      {
        IDisposable btsSub = (IDisposable) null;
        IDisposable inAppSub = (IDisposable) null;
        object releaseLock = new object();
        Action release = Utils.IgnoreMultipleInvokes((Action) (() =>
        {
          Log.d(MediaDownloadManager.LogHeader, "Invoking release");
          btsSub.SafeDispose();
          btsSub = (IDisposable) null;
          inAppSub.SafeDispose();
          inAppSub = (IDisposable) null;
          observer.OnCompleted();
        }));
        btsSub = MediaDownloadManager.BackgroundTransfer(url, outputFileName).Subscribe<MediaDownloadProgress>((Action<MediaDownloadProgress>) (dlState =>
        {
          if (dlState.BackgroundDownloadFailedTryForeground)
          {
            inAppSub = MediaDownloadManager.InAppDownload(url, ipHint, outputFileName, true, downloadMaxBytes, downloadSize, fsEvent).Subscribe<MediaDownloadProgress>((Action<MediaDownloadProgress>) (ev2 =>
            {
              observer.OnNext(ev2);
              if (ev2.DownloadState != MediaDownloadProgress.DownloadStatus.Completed)
                return;
              release();
            }), (Action<Exception>) (err => observer.OnError(err)), (Action) (() => MediaDownloadManager.SafeRelease(releaseLock, ref inAppSub, ref btsSub, release)));
          }
          else
          {
            observer.OnNext(dlState);
            if (dlState.DownloadState != MediaDownloadProgress.DownloadStatus.Completed)
              return;
            release();
          }
        }), (Action<Exception>) (err => observer.OnError(err)), (Action) (() => MediaDownloadManager.SafeRelease(releaseLock, ref btsSub, ref inAppSub, release)));
        return release;
      }));
    }

    private static void SafeRelease(
      object releaseLock,
      ref IDisposable mySub,
      ref IDisposable otherSub,
      Action release)
    {
      bool flag = false;
      lock (releaseLock)
      {
        mySub.SafeDispose();
        mySub = (IDisposable) null;
        flag = otherSub == null;
      }
      if (!flag)
        return;
      release();
    }

    private static IObservable<MediaDownloadProgress> InAppDownload(
      string url,
      string ipHint,
      string outputFileName,
      bool allowResume,
      int downloadMaxBytes,
      long downloadSize,
      WhatsApp.Events.MediaDownload fsEvent)
    {
      fsEvent.routeIp = ipHint;
      Log.d(MediaDownloadManager.LogHeader, "processing {0}", (object) outputFileName);
      IObservable<MediaDownloadProgress> observable = Observable.Create<MediaDownloadProgress>((Func<IObserver<MediaDownloadProgress>, Action>) (observer =>
      {
        CancellationTokenSource src = new CancellationTokenSource();
        CancellationToken cancellationToken = src.Token;
        WAThreadPool.QueueUserWorkItem((Action) (async () =>
        {
          Action<DownloadProgress> downloadProgress = (Action<DownloadProgress>) (progress =>
          {
            Log.l(MediaDownloadManager.LogHeader, "Progess {0} {1}", (object) progress.DownloadedSoFar, (object) progress.TotalToDownload);
            observer.OnNext(new MediaDownloadProgress(MediaDownloadProgress.DownloadStatus.Active)
            {
              DownloadedSoFar = progress.DownloadedSoFar,
              TotalToDownload = progress.TotalToDownload
            });
          });
          FileDownloadResult downloadResult = (FileDownloadResult) null;
          bool retry = true;
          bool resumeAllowed = allowResume;
          Exception downloadException = (Exception) null;
          while (retry && !cancellationToken.IsCancellationRequested)
          {
            retry = false;
            try
            {
              Log.l(MediaDownloadManager.LogHeader, "InApp download {0}", (object) url);
              downloadResult = await MediaDownloadManager.InAppDownloadImplAsync(url, ipHint, outputFileName, resumeAllowed, 0, downloadProgress, fsEvent, cancellationToken);
              Log.l(MediaDownloadManager.LogHeader, "result {0}", (object) downloadResult.HttpResponseCode);
              downloadException = downloadResult.RequestException;
            }
            catch (OperationCanceledException ex)
            {
              Log.l(MediaDownloadManager.LogHeader, "cancelled");
              if (ex is MediaDownloadException downloadException2)
                downloadException = (Exception) downloadException2;
            }
            catch (Exception ex)
            {
              Log.LogException(ex, nameof (InAppDownload));
              downloadException = ex;
              HttpStatusCode? nullable3 = MediaHelper.StatusCodeFromException(ex);
              if (!nullable3.HasValue && ex.InnerException != null)
                nullable3 = MediaHelper.StatusCodeFromException(ex.InnerException);
              HttpStatusCode? nullable4 = nullable3;
              HttpStatusCode httpStatusCode = HttpStatusCode.RequestedRangeNotSatisfiable;
              if ((nullable4.GetValueOrDefault() == httpStatusCode ? (nullable4.HasValue ? 1 : 0) : 0) != 0)
              {
                if (resumeAllowed)
                {
                  resumeAllowed = false;
                  retry = true;
                  downloadException = (Exception) null;
                }
              }
            }
            finally
            {
              Log.l(MediaDownloadManager.LogHeader, "InAppDownload finally {0} {1}", (object) resumeAllowed, (object) retry);
            }
          }
          if (cancellationToken.IsCancellationRequested)
            return;
          if (downloadException != null)
            observer.OnError(downloadException);
          else if (downloadResult != null)
          {
            if (downloadResult.RequestException == null)
              observer.OnNext(new MediaDownloadProgress(MediaDownloadProgress.DownloadStatus.Completed)
              {
                HttpRespCode = downloadResult.HttpResponseCode
              });
            else
              observer.OnError(downloadResult.RequestException);
          }
          else
          {
            Log.l(MediaDownloadManager.LogHeader, "Unexpected combination");
            observer.OnError((Exception) new MediaDownloadException(MediaDownloadException.DownloadErrorCode.Unknown, "Unexpected combination"));
          }
          observer.OnCompleted();
        }));
        return (Action) (() => src.Cancel());
      }));
      Log.d(MediaDownloadManager.LogHeader, "created observable for {0}", (object) outputFileName);
      return observable;
    }

    private async Task<MediaDownloadProgress> RunInAppDownloadAsync(
      CancellationTokenSource cancelSource,
      string url,
      string ipHint,
      string outputFileName,
      bool allowResume,
      int downloadMaxBytes,
      long downloadSize,
      WhatsApp.Events.MediaDownload fsEvent,
      IObserver<MediaDownloadProgress> progressObserver)
    {
      Action<DownloadProgress> downloadProgress = (Action<DownloadProgress>) (progress =>
      {
        Log.l(MediaDownloadManager.LogHeader, "Progess {0} {1}", (object) progress.DownloadedSoFar, (object) progress.TotalToDownload);
        MediaDownloadProgress downloadProgress1 = new MediaDownloadProgress(MediaDownloadProgress.DownloadStatus.Active)
        {
          DownloadedSoFar = progress.DownloadedSoFar,
          TotalToDownload = progress.TotalToDownload
        };
        progressObserver?.OnNext(downloadProgress1);
      });
      FileDownloadResult downloadResult = (FileDownloadResult) null;
      bool retry = true;
      bool resumeAllowed = allowResume;
      Exception downloadException = (Exception) null;
      while (retry && !cancelSource.Token.IsCancellationRequested)
      {
        retry = false;
        try
        {
          Log.l(MediaDownloadManager.LogHeader, "InApp download {0}", (object) url);
          downloadResult = await MediaDownloadManager.InAppDownloadImplAsync(url, ipHint, outputFileName, resumeAllowed, 0, downloadProgress, fsEvent, cancelSource.Token);
          Log.l(MediaDownloadManager.LogHeader, "result {0}", (object) downloadResult.HttpResponseCode);
          downloadException = downloadResult.RequestException;
        }
        catch (OperationCanceledException ex)
        {
          Log.l(MediaDownloadManager.LogHeader, "cancelled");
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "InAppDownload");
          downloadException = ex;
          HttpStatusCode? nullable1 = MediaHelper.StatusCodeFromException(ex);
          if (!nullable1.HasValue && ex.InnerException != null)
            nullable1 = MediaHelper.StatusCodeFromException(ex.InnerException);
          HttpStatusCode? nullable2 = nullable1;
          HttpStatusCode httpStatusCode = HttpStatusCode.RequestedRangeNotSatisfiable;
          if ((nullable2.GetValueOrDefault() == httpStatusCode ? (nullable2.HasValue ? 1 : 0) : 0) != 0)
          {
            if (resumeAllowed)
            {
              resumeAllowed = false;
              retry = true;
              downloadException = (Exception) null;
            }
          }
        }
        finally
        {
          Log.l(MediaDownloadManager.LogHeader, "InAppDownload finally {0} {1}", (object) resumeAllowed, (object) retry);
        }
      }
      if (cancelSource.Token.IsCancellationRequested)
        throw new OperationCanceledException();
      if (downloadException != null)
      {
        Log.l(MediaDownloadManager.LogHeader, "Throwing local exception");
        throw downloadException;
      }
      if (downloadResult == null)
        throw new OperationCanceledException();
      if (downloadResult.RequestException == null)
      {
        MediaDownloadProgress downloadProgress2 = new MediaDownloadProgress(MediaDownloadProgress.DownloadStatus.Completed)
        {
          HttpRespCode = downloadResult.HttpResponseCode
        };
        progressObserver?.OnNext(downloadProgress2);
        return downloadProgress2;
      }
      Log.l(MediaDownloadManager.LogHeader, "Throwing async detected exception");
      throw downloadResult.RequestException;
    }

    private static void ApplyIpHint(
      List<MediaDownloadManager.FallbackOption> r,
      string ip,
      Func<string, string> urlMap = null)
    {
      MediaDownloadManager.FallbackOption fallbackOption1;
      if (ip != null)
      {
        List<MediaDownloadManager.FallbackOption> fallbackOptionList = r;
        fallbackOption1 = new MediaDownloadManager.FallbackOption();
        fallbackOption1.SetResolver = (Action<IWebRequest>) (req => req.SetResolver(new IpResolver()
        {
          Ip = ip
        }.ToNativeResolver()));
        fallbackOption1.UrlMap = urlMap;
        MediaDownloadManager.FallbackOption fallbackOption2 = fallbackOption1;
        fallbackOptionList.Add(fallbackOption2);
      }
      List<MediaDownloadManager.FallbackOption> fallbackOptionList1 = r;
      fallbackOption1 = new MediaDownloadManager.FallbackOption();
      fallbackOption1.UrlMap = urlMap;
      MediaDownloadManager.FallbackOption fallbackOption3 = fallbackOption1;
      fallbackOptionList1.Add(fallbackOption3);
    }

    private static async Task<FileDownloadResult> InAppDownloadImplAsync(
      string url,
      string ipHint,
      string outputFileName,
      bool allowResume,
      int downloadMaxBytes,
      Action<DownloadProgress> progressObserver,
      WhatsApp.Events.MediaDownload fsEvent,
      CancellationToken cancellationToken)
    {
      fsEvent.routeIp = ipHint;
      Log.d(MediaDownloadManager.LogHeader, "processing {0}", (object) outputFileName);
      Stream output = (Stream) null;
      IMediaStorage mediaStorage;
      FileDownloadResult fileDownloadResult1;
      using (mediaStorage = MediaStorage.Create(outputFileName))
      {
        Log.l(MediaDownloadManager.LogHeader, "Using in-app HTTP APIs to download {0} from {1}", (object) outputFileName, (object) MediaDownload.RedactUrl(url));
        try
        {
          output = mediaStorage.OpenFile(outputFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        }
        catch (Exception ex1)
        {
          Log.l(MediaDownloadManager.LogHeader, "Unexpected exception creating {0}, {1}", (object) outputFileName, (object) ex1.GetFriendlyMessage());
          try
          {
            mediaStorage.DeleteFile(outputFileName);
            Log.l(MediaDownloadManager.LogHeader, "Deleted output file");
            output = mediaStorage.OpenFile(outputFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
          }
          catch (Exception ex2)
          {
            Log.l(MediaDownloadManager.LogHeader, "Further unexpected exception creating {0}, {1}", (object) outputFileName, (object) ex2.GetFriendlyMessage());
            Log.SendCrashLog((Exception) new InvalidOperationException("Download file issues"), "Further exception", logOnlyForRelease: true);
            throw ex1;
          }
          Log.SendCrashLog((Exception) new InvalidOperationException("Download file issues"), "Initial exception", logOnlyForRelease: true);
        }
        Log.d(MediaDownloadManager.LogHeader, "Downloading to {0}", (object) outputFileName);
        Dictionary<string, string> headers = new Dictionary<string, string>();
        if (output.Length > 1L & allowResume || downloadMaxBytes > 0)
        {
          long num = output.Length > 1L & allowResume ? output.Length - 1L : 0L;
          string str1 = downloadMaxBytes > 0 ? downloadMaxBytes.ToString() : (string) null;
          string str2 = string.Format("bytes={0}-{1}", (object) num, (object) str1);
          Log.l(MediaDownloadManager.LogHeader, string.Format("using Range: {0}", (object) str2));
          headers["Range"] = str2;
          fsEvent.downloadResumePoint = new long?((long) (int) num);
        }
        else
          fsEvent.downloadResumePoint = new long?(0L);
        Action<DownloadProgress> progressObserver1 = (Action<DownloadProgress>) (progress =>
        {
          Log.d(MediaDownloadManager.LogHeader, "Progess {0} {1}", (object) progress.DownloadedSoFar, (object) progress.TotalToDownload);
          try
          {
            progressObserver(progress);
          }
          catch (Exception ex)
          {
            Log.l(MediaDownloadManager.LogHeader, "Exception processing progress " + ex.GetFriendlyMessage());
          }
        });
        try
        {
          FileDownloadResult fileDownloadResult2 = await MediaDownloadManager.InAppDownloadStreamAsync(output, url, headers, progressObserver1, fsEvent, cancellationToken);
          Log.l(MediaDownloadManager.LogHeader, "result {0} {1}", (object) fileDownloadResult2.HttpResponseCode, (object) output.Length);
          output.Flush();
          output.Close();
          output = (Stream) null;
          fileDownloadResult1 = fileDownloadResult2;
        }
        finally
        {
          Log.d(MediaDownloadManager.LogHeader, "running finally {0}", (object) outputFileName);
          if (output != null)
          {
            try
            {
              output.Close();
            }
            catch (Exception ex)
            {
            }
          }
          output.SafeDispose();
          output = (Stream) null;
        }
      }
      return fileDownloadResult1;
    }

    private static async Task<FileDownloadResult> InAppDownloadStreamAsync(
      Stream output,
      string url,
      Dictionary<string, string> headers,
      Action<DownloadProgress> progressObserver,
      WhatsApp.Events.MediaDownload fsEvent,
      CancellationToken cancellationToken)
    {
      Action<IWebRequest> setResolver = (Action<IWebRequest>) null;
      return await NativeWeb.RunRequestAysnc<FileDownloadResult>(cancellationToken, NativeWeb.Options.Default | NativeWeb.Options.KeepAlive, progressObserver, (Action<IWebRequest, Action<FileDownloadResult>, Action<DownloadProgress>, CancellationToken>) ((req, onFinished, progressReporter, cancelToken) =>
      {
        long len = 0;
        long totalRead = 0;
        fsEvent.mediaDownloadResult = new wam_enum_media_download_result_type?(wam_enum_media_download_result_type.ERROR_UNKNOWN);
        Stopwatch timer = new Stopwatch();
        Action tidyUp = (Action) (() =>
        {
          Log.l(MediaDownloadManager.LogHeader, "tidyUp start");
          timer?.Stop();
          timer = (Stopwatch) null;
          req?.Cancel();
          req = (IWebRequest) null;
          Log.l(MediaDownloadManager.LogHeader, "tidyUp done");
        });
        Action cancelDownload = (Action) (() =>
        {
          Log.l(MediaDownloadManager.LogHeader, "Cancellation detected");
          tidyUp();
        });
        int responseCode = -1;
        NativeWeb.Callback callbackObject = (NativeWeb.Callback) null;
        callbackObject = new NativeWeb.Callback()
        {
          OnBeginResponse = (Action<int, string>) ((code, headerStrings) =>
          {
            Log.d(MediaDownloadManager.LogHeader, "OnBeginResponse: code {0}", (object) code);
            responseCode = code;
            try
            {
              if (cancelToken.IsCancellationRequested)
              {
                cancelDownload();
              }
              else
              {
                if (code < 200 || code > 299)
                {
                  Mms4RouteSelector.GetInstance().OnMediaTransferErrorOrResponseCode(code);
                  throw new MediaDownloadException(MediaDownloadException.DownloadErrorCode.UnexpectedHttpCode, code, (string) null);
                }
                headers.Clear();
                foreach (KeyValuePair<string, string> header in NativeWeb.ParseHeaders(headerStrings))
                  headers[header.Key.ToLowerInvariant()] = header.Value;
                string valueOrDefault3 = headers.GetValueOrDefault<string, string>("content-length");
                if (valueOrDefault3 == null || !long.TryParse(valueOrDefault3, out len))
                  throw new WebException("Could not parse content-length");
                string valueOrDefault4 = headers.GetValueOrDefault<string, string>("content-range");
                long num = 0;
                if (valueOrDefault4 != null)
                  num = MediaDownload.ParseContentRange(valueOrDefault4).Start;
                Log.l(MediaDownloadManager.LogHeader, "Starting download at byte {0}", (object) num);
                output.Position = num;
                progressReporter(new DownloadProgress()
                {
                  DownloadedSoFar = output.Position,
                  TotalToDownload = len
                });
              }
            }
            catch (Exception ex)
            {
              tidyUp();
              onFinished(new FileDownloadResult()
              {
                HttpResponseCode = responseCode,
                RequestException = ex
              });
            }
          }),
          OnBytesIn = (Action<byte[]>) (buf =>
          {
            int length = buf.Length;
            Log.d(MediaDownloadManager.LogHeader, "OnBytesIn: length {0}", (object) length);
            try
            {
              if (cancelToken.IsCancellationRequested)
              {
                cancelDownload();
              }
              else
              {
                try
                {
                  output.Write(buf, 0, length);
                }
                catch (Exception ex)
                {
                  Log.l(MediaDownloadManager.LogHeader, "Exception writing stream {0}", (object) ex.GetFriendlyMessage());
                  fsEvent.mediaDownloadResult = new wam_enum_media_download_result_type?(wam_enum_media_download_result_type.ERROR_INSUFFICIENT_SPACE);
                  throw;
                }
                if (totalRead == 0L && callbackObject != null && callbackObject.writeStartTimeUtcTicks > 0L)
                  fsEvent.connectT = new long?((callbackObject.writeStartTimeUtcTicks - callbackObject.createTimeUtcTicks) / 10000L);
                totalRead += (long) length;
                progressReporter(new DownloadProgress()
                {
                  DownloadedSoFar = totalRead,
                  TotalToDownload = len
                });
              }
            }
            catch (Exception ex)
            {
              tidyUp();
              onFinished(new FileDownloadResult()
              {
                RequestException = ex
              });
            }
          }),
          OnEndResponse = (Action) (() =>
          {
            Log.d(MediaDownloadManager.LogHeader, "OnEndResponse: length {0}", (object) len);
            try
            {
              if (cancelToken.IsCancellationRequested)
              {
                cancelDownload();
              }
              else
              {
                timer.Stop();
                fsEvent.mediaDownloadT = new long?(timer.ElapsedMilliseconds);
                if (callbackObject != null && callbackObject.writeStartTimeUtcTicks > 0L)
                {
                  fsEvent.connectT = new long?((callbackObject.writeStartTimeUtcTicks - callbackObject.createTimeUtcTicks) / 10000L);
                  fsEvent.networkDownloadT = new long?((DateTime.UtcNow.Ticks - callbackObject.createTimeUtcTicks) / 10000L);
                }
                FileDownloadResult fileDownloadResult = new FileDownloadResult()
                {
                  HttpResponseCode = responseCode
                };
                if (len == totalRead)
                  fsEvent.mediaDownloadResult = new wam_enum_media_download_result_type?(wam_enum_media_download_result_type.OK);
                onFinished(fileDownloadResult);
              }
            }
            catch (Exception ex)
            {
              tidyUp();
              onFinished(new FileDownloadResult()
              {
                RequestException = ex
              });
            }
          })
        };
        try
        {
          timer.Start();
          Action<IWebRequest> action = setResolver;
          if (action != null)
            action(req);
          FieldStats.SetHostDetailsInDownloadEvent(fsEvent, url);
          req.Open(url, (IWebCallback) callbackObject, headers: string.Join("\r\n", headers.Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (kv => string.Format("{0}: {1}", (object) kv.Key, (object) kv.Value)))));
        }
        catch (Exception ex)
        {
          tidyUp();
          onFinished(new FileDownloadResult()
          {
            RequestException = ex
          });
        }
        finally
        {
          Log.l(MediaDownloadManager.LogHeader, "Download ended");
        }
      }));
    }

    private static IObservable<MediaDownloadProgress> BackgroundTransfer(
      string url,
      string filename)
    {
      return Observable.Create<MediaDownloadProgress>((Func<IObserver<MediaDownloadProgress>, Action>) (observer =>
      {
        CancellationTokenSource cancelSource = new CancellationTokenSource();
        Action action = Utils.IgnoreMultipleInvokes((Action) (() =>
        {
          if (cancelSource != null && !cancelSource.IsCancellationRequested)
            cancelSource.Cancel();
          observer.OnCompleted();
        }));
        WAThreadPool.QueueUserWorkItem((Action) (async () =>
        {
          Log.l(MediaDownloadManager.LogHeader, "Using BT to download {0}", (object) MediaDownload.RedactUrl(url));
          using (IMediaStorage mediaStorage = MediaStorage.Create(filename))
          {
            try
            {
              mediaStorage.OpenFile(filename, FileMode.OpenOrCreate).SafeDispose();
            }
            catch (Exception ex)
            {
              Log.l(MediaDownloadManager.LogHeader, "Exception generating file {0}", (object) filename);
            }
          }
          Uri downloadUri = (Uri) null;
          StorageFile destinationFile;
          try
          {
            destinationFile = await StorageFile.GetFileFromPathAsync(filename);
            downloadUri = new Uri(url);
          }
          catch (Exception ex)
          {
            observer.OnError(ex);
            return;
          }
          try
          {
            DownloadOperation download = new BackgroundDownloader().CreateDownload(downloadUri, (IStorageFile) destinationFile);
            Progress<DownloadOperation> progressCallback = new Progress<DownloadOperation>(new Action<DownloadOperation>(MediaDownloadManager.BackgroundDownloadProgress));
            BackgroundDownloadDetails details = new BackgroundDownloadDetails(cancelSource, download, observer);
            MediaDownloadManager.activeBackgroundDownloads.Add(details);
            Task<MediaDownloadProgress> t = MediaDownloadManager.HandleBackgroundDownloadAsync(cancelSource.Token, progressCallback, download, true);
            details.BackgroundDownloadTask = (Task) t;
            Log.l(MediaDownloadManager.LogHeader, "Downloading {0} to {1}, guid, {2}", (object) downloadUri.AbsoluteUri, (object) destinationFile.Path, (object) download.Guid);
            MediaDownloadProgress downloadProgress = await t;
            Log.l(MediaDownloadManager.LogHeader, "Downloaded? {0}, task {1}", (object) download.Guid, (object) t.Status);
            MediaDownloadManager.activeBackgroundDownloads.Remove(details);
            observer.OnNext(new MediaDownloadProgress(MediaDownloadProgress.DownloadStatus.Completed)
            {
              HttpRespCode = t.Result.HttpRespCode
            });
            download = (DownloadOperation) null;
            details = (BackgroundDownloadDetails) null;
            t = (Task<MediaDownloadProgress>) null;
          }
          catch (Exception ex)
          {
            Log.l(MediaDownloadManager.LogHeader, "BackgroundTransfer exception");
            if (cancelSource.Token.IsCancellationRequested)
              return;
            observer.OnError(ex);
          }
          finally
          {
            observer.OnCompleted();
          }
        }));
        return action;
      }));
    }

    private static async Task<MediaDownloadProgress> RunBackgroundDownloadAsync(
      CancellationTokenSource cancelSource,
      string url,
      string filename,
      IObserver<MediaDownloadProgress> progressObserver)
    {
      Log.l(MediaDownloadManager.LogHeader, "Using BT direct to download {0}", (object) MediaDownload.RedactUrl(url));
      using (IMediaStorage mediaStorage = MediaStorage.Create(filename))
      {
        try
        {
          mediaStorage.OpenFile(filename, FileMode.OpenOrCreate).SafeDispose();
        }
        catch (Exception ex)
        {
          Log.l(MediaDownloadManager.LogHeader, "BT Exception generating file {0}", (object) filename);
          throw;
        }
      }
      Uri downloadUri = (Uri) null;
      FileRoot? nullable = MediaStorage.DetermineRoot(filename);
      string destinationFilename = filename.Replace("/", "\\");
      Uri uri = (Uri) null;
      string message = (string) null;
      if (!nullable.HasValue)
      {
        message = "file location has no root";
      }
      else
      {
        nullable = new FileRoot?(FileRoot.PhoneStorage);
        switch (nullable.Value)
        {
          case FileRoot.IsoStore:
            if (filename.StartsWith(Constants.IsoStorePath, StringComparison.OrdinalIgnoreCase))
            {
              destinationFilename = "isostore:\\\\" + filename.Substring(Constants.IsoStorePath.Length);
              uri = new Uri(destinationFilename);
              break;
            }
            message = "IsoStore file does not start with correct prefix";
            break;
          case FileRoot.PhoneStorage:
            destinationFilename = filename;
            break;
          case FileRoot.SdCard:
            Log.l(MediaDownloadManager.LogHeader, "Sd Card might not work");
            break;
          default:
            message = string.Format("Unsupported root {0}", (object) nullable.Value);
            break;
        }
      }
      if (message != null)
      {
        Log.l(MediaDownloadManager.LogHeader, "Exception being thrown for {0}", (object) filename);
        InvalidDataException e = new InvalidDataException(message);
        Log.SendCrashLog((Exception) e, "Processing BT media download", logOnlyForRelease: true);
        throw e;
      }
      StorageFile destinationFile;
      try
      {
        if (uri != (Uri) null)
        {
          destinationFile = await StorageFile.GetFileFromApplicationUriAsync(uri);
        }
        else
        {
          Log.d(MediaDownloadManager.LogHeader, "BT creating destination file {0} for {1}", (object) destinationFilename, (object) filename);
          destinationFile = await StorageFile.GetFileFromPathAsync(destinationFilename);
        }
        downloadUri = new Uri(url);
      }
      catch (Exception ex)
      {
        Log.l(MediaDownloadManager.LogHeader, "BT Exception creating destination file {0} for {1}", (object) destinationFilename, (object) filename);
        throw;
      }
      DownloadOperation download = new BackgroundDownloader().CreateDownload(downloadUri, (IStorageFile) destinationFile);
      Progress<DownloadOperation> progressCallback = new Progress<DownloadOperation>(new Action<DownloadOperation>(MediaDownloadManager.BackgroundDownloadProgress));
      BackgroundDownloadDetails details = new BackgroundDownloadDetails(cancelSource, download, progressObserver);
      MediaDownloadProgress result;
      try
      {
        MediaDownloadManager.activeBackgroundDownloads.Add(details);
        Task<MediaDownloadProgress> t = MediaDownloadManager.HandleBackgroundDownloadAsync(cancelSource.Token, progressCallback, download, true);
        details.BackgroundDownloadTask = (Task) t;
        Log.l(MediaDownloadManager.LogHeader, "BT Downloading {0} to {1}, guid, {2}", (object) downloadUri.AbsoluteUri, (object) destinationFile.Path, (object) download.Guid);
        MediaDownloadProgress downloadProgress = await t;
        Log.l(MediaDownloadManager.LogHeader, "BT Downloaded? {0}, task {1}", (object) download.Guid, (object) t.Status);
        result = t.Result;
      }
      catch (Exception ex)
      {
        Log.l(MediaDownloadManager.LogHeader, "BT exception {0}", (object) download.Guid);
        throw;
      }
      finally
      {
        MediaDownloadManager.activeBackgroundDownloads.Remove(details);
      }
      return result;
    }

    private static async Task<MediaDownloadProgress> HandleBackgroundDownloadAsync(
      CancellationToken cancelToken,
      Progress<DownloadOperation> progressCallback,
      DownloadOperation download,
      bool start)
    {
      MediaDownloadProgress downloadProgress;
      try
      {
        Log.l(MediaDownloadManager.LogHeader, "Running: " + (object) download.Guid);
        MediaDownloadManager.runningBackgroundDownloads.Add(download);
        if (start)
        {
          DownloadOperation downloadOperation1 = await download.StartAsync().AsTask<DownloadOperation, DownloadOperation>(cancelToken, (IProgress<DownloadOperation>) progressCallback);
        }
        else
        {
          DownloadOperation downloadOperation2 = await download.AttachAsync().AsTask<DownloadOperation, DownloadOperation>(cancelToken, (IProgress<DownloadOperation>) progressCallback);
        }
        ResponseInformation responseInformation = download.GetResponseInformation();
        string str = responseInformation != null ? responseInformation.StatusCode.ToString() : string.Empty;
        Log.l(MediaDownloadManager.LogHeader, "Completed: {0}, Status Code: {1}, Bytes left: {2}", (object) download.Guid, (object) str, (object) (ulong) ((long) download.Progress.TotalBytesToReceive - (long) download.Progress.BytesReceived));
        downloadProgress = new MediaDownloadProgress(MediaDownloadProgress.DownloadStatus.Completed)
        {
          HttpRespCode = (int) responseInformation.StatusCode
        };
      }
      catch (TaskCanceledException ex)
      {
        Log.l(MediaDownloadManager.LogHeader, "Cancelled: " + (object) download.Guid);
        throw new OperationCanceledException();
      }
      catch (Exception ex)
      {
        WebErrorStatus status = BackgroundTransferError.GetStatus(ex.HResult);
        Log.l(MediaDownloadManager.LogHeader, "Error: {0}: {1}", (object) download.Guid, (object) status);
        throw;
      }
      finally
      {
        Log.l(MediaDownloadManager.LogHeader, "Stopped: " + (object) download.Guid);
        MediaDownloadManager.runningBackgroundDownloads.Remove(download);
      }
      return downloadProgress;
    }

    private static void BackgroundDownloadProgress(DownloadOperation download)
    {
      Windows.Networking.BackgroundTransfer.BackgroundDownloadProgress progress = download.Progress;
      Log.l(MediaDownloadManager.LogHeader, "Progress: {0}, Status: {1}", (object) download.Guid, (object) progress.Status);
      double num = 100.0;
      if (progress.TotalBytesToReceive > 0UL)
        num = (double) (progress.BytesReceived * 100UL / progress.TotalBytesToReceive);
      Log.l(MediaDownloadManager.LogHeader, " - Transferred bytes: {0} of {1}, {2}%", (object) progress.BytesReceived, (object) progress.TotalBytesToReceive, (object) num);
      IObserver<MediaDownloadProgress> observerForDownload = MediaDownloadManager.GetObserverForDownload(download);
      if (observerForDownload != null)
        observerForDownload.OnNext(new MediaDownloadProgress(MediaDownloadProgress.DownloadStatus.Active)
        {
          TotalToDownload = (long) progress.TotalBytesToReceive,
          DownloadedSoFar = (long) progress.BytesReceived
        });
      if (progress.HasRestarted)
        Log.l(MediaDownloadManager.LogHeader, " - Download restarted");
      if (!progress.HasResponseChanged)
        return;
      ResponseInformation responseInformation = download.GetResponseInformation();
      int count = responseInformation != null ? responseInformation.Headers.Count : 0;
      Log.l(MediaDownloadManager.LogHeader, " - Response updated; Header count: " + (object) count);
    }

    private static IObserver<MediaDownloadProgress> GetObserverForDownload(
      DownloadOperation download)
    {
      foreach (BackgroundDownloadDetails backgroundDownload in MediaDownloadManager.activeBackgroundDownloads)
      {
        if (backgroundDownload.BackgroundDownload.Guid == download.Guid)
          return backgroundDownload.BackgroundDownloadObserver;
      }
      return (IObserver<MediaDownloadProgress>) null;
    }

    public static void ReportActiveDownloads()
    {
      AppState.Worker.Enqueue((Action) (() => MediaDownloadManager.DiscoverActiveDownloadsAsync().Wait()));
    }

    private static async Task<List<DownloadOperation>> DiscoverActiveDownloadsAsync()
    {
      List<DownloadOperation> activeDownloads = new List<DownloadOperation>();
      IReadOnlyList<DownloadOperation> downloads = (IReadOnlyList<DownloadOperation>) null;
      try
      {
        downloads = await BackgroundDownloader.GetCurrentDownloadsAsync();
      }
      catch (Exception ex)
      {
        Log.l(MediaDownloadManager.LogHeader, "Exception getting list of downloads: {0}", (object) ex.GetFriendlyMessage());
        return (List<DownloadOperation>) null;
      }
      foreach (DownloadOperation downloadOperation in (IEnumerable<DownloadOperation>) downloads)
      {
        Log.l(MediaDownloadManager.LogHeader, "Active BT Download: {0} - status {1}", (object) downloadOperation.Guid, (object) downloadOperation.Progress.Status);
        if (downloadOperation.Progress.Status == 2)
        {
          downloadOperation.Resume();
          Log.l(MediaDownloadManager.LogHeader, "BT Download: {0} resumed", (object) downloadOperation.Guid);
        }
        else if (downloadOperation.Progress.Status == 6 || downloadOperation.Progress.Status == 5 || downloadOperation.Progress.Status == 7)
          Log.l(MediaDownloadManager.LogHeader, "BT Download: {0} found {1}", (object) downloadOperation.Guid, (object) downloadOperation.Progress.Status);
      }
      return activeDownloads;
    }

    private static void DecryptFileDownload(
      FunXMPP.FMessage.FunMediaType funMediaType,
      byte[] cipherMediaKey,
      string fileName,
      string plaintextName,
      byte[] plainTextHash,
      WhatsApp.Events.MediaDownload fsEvent)
    {
      try
      {
        using (AxolotlMediaCipher downloadCipher = AxolotlMediaCipher.CreateDownloadCipher(FunXMPP.FMessage.TypeFromFunMediaType(funMediaType), cipherMediaKey))
        {
          Log.l(MediaDownloadManager.LogHeader, "Decrypting [{0}] -> [{1}]", (object) fileName, (object) plaintextName);
          using (IMediaStorage mediaStorage = MediaStorage.Create(fileName))
          {
            Stream src;
            try
            {
              src = mediaStorage.OpenFile(fileName);
            }
            catch (Exception ex)
            {
              Log.l(MediaDownloadManager.LogHeader, "Exception opening file");
              throw;
            }
            using (src)
            {
              using (Stream dest = mediaStorage.OpenFile(plaintextName, FileMode.Create, FileAccess.ReadWrite))
                downloadCipher.DecryptMedia(src, dest, plainTextHash, fsEvent);
            }
          }
        }
      }
      catch (Exception ex)
      {
        switch (ex)
        {
          case CryptographicException _:
          case Axolotl.AxolotlMediaDecryptException _:
          case CheckAndRepairException _:
            Log.SendCrashLog(ex, "decrypt exception", logOnlyForRelease: true);
            break;
          default:
            Log.SendCrashLog(ex, MediaDownloadManager.LogHeader);
            break;
        }
        throw ex;
      }
    }

    private static async Task<Mms4HostSelector.Mms4HostSelection> GetSelectedRouteAsync(
      FunXMPP.FMessage.FunMediaType mediaType,
      bool waitForPrewarm,
      CancellationToken cancelToken)
    {
      IObservable<Mms4HostSelector.Mms4HostSelection> selectedHostObservable = Mms4HostSelector.GetInstance().GetSelectedHostObservable(true, mediaType, waitForPrewarm);
      TaskCompletionSource<Mms4HostSelector.Mms4HostSelection> tcs = new TaskCompletionSource<Mms4HostSelector.Mms4HostSelection>();
      Action<Mms4HostSelector.Mms4HostSelection> onNext = (Action<Mms4HostSelector.Mms4HostSelection>) (hostSelection =>
      {
        if (cancelToken.IsCancellationRequested)
          tcs.SetCanceled();
        else
          tcs.SetResult(hostSelection);
      });
      Action<Exception> onError = (Action<Exception>) (ex =>
      {
        if (cancelToken.IsCancellationRequested)
          tcs.SetCanceled();
        else
          tcs.SetException(ex);
      });
      Action onCompleted = (Action) (() =>
      {
        if (!tcs.TrySetCanceled())
          return;
        Log.l(MediaDownloadManager.LogHeader, "Needed to set cancelled for GetSelectedRouteAsync");
      });
      selectedHostObservable.Subscribe<Mms4HostSelector.Mms4HostSelection>(onNext, onError, onCompleted);
      Mms4HostSelector.Mms4HostSelection task = await tcs.Task;
      if (tcs.Task.IsCanceled)
        throw new OperationCanceledException("GetSelectedRouteAsync cancelled");
      return !tcs.Task.IsFaulted ? tcs.Task.Result : (Mms4HostSelector.Mms4HostSelection) null;
    }

    private static async Task<MediaDownloadProgress> TurnDownloadObservableIntoAsync(
      IObserver<MediaDownloadProgress> actualObserver,
      IObservable<MediaDownloadProgress> obs,
      string observableDescription,
      CancellationToken cancelToken)
    {
      TaskCompletionSource<MediaDownloadProgress> tcs = new TaskCompletionSource<MediaDownloadProgress>();
      obs.Subscribe<MediaDownloadProgress>((Action<MediaDownloadProgress>) (result =>
      {
        Log.d(MediaDownloadManager.LogHeader, "OnNext {0} {1} {2}", (object) observableDescription, (object) result, (object) tcs.Task.IsCompleted);
        if (cancelToken.IsCancellationRequested)
          tcs.SetCanceled();
        else if (result.DownloadState == MediaDownloadProgress.DownloadStatus.Completed)
          tcs.SetResult(result);
        else
          actualObserver?.OnNext(result);
      }), (Action<Exception>) (ex =>
      {
        if (cancelToken.IsCancellationRequested)
        {
          tcs.SetCanceled();
        }
        else
        {
          if (tcs.TrySetException(ex))
            return;
          Log.l(MediaDownloadManager.LogHeader, "Can't set excption {0}", (object) ex.GetFriendlyMessage());
        }
      }), (Action) (() =>
      {
        if (!tcs.TrySetCanceled())
          return;
        Log.l(MediaDownloadManager.LogHeader, "Needed to set cancelled for " + observableDescription);
      }));
      MediaDownloadProgress task = await tcs.Task;
      if (tcs.Task.IsCanceled)
        throw new OperationCanceledException(observableDescription + " cancelled");
      if (tcs.Task.IsFaulted)
        throw tcs.Task.Exception;
      return tcs.Task.Result;
    }

    private class DownloadQueueEntry
    {
      private IObserver<MediaDownloadProgress> obsForDownload;
      private CancellationTokenSource ctsForDownload;
      private WhatsApp.Events.MediaDownload fsEventForDownload;
      private object processStepLock = new object();

      public DownloadMedia DownloadEntry { get; private set; }

      public MediaDownloadManager.DownloadQueueEntry.ProcessingSteps ProcessStep { get; private set; }

      public DownloadQueueEntry(DownloadMedia entry, IObserver<MediaDownloadProgress> obs)
      {
        this.DownloadEntry = entry;
        this.obsForDownload = obs;
      }

      public async void Download()
      {
        lock (this.processStepLock)
        {
          if (this.ProcessStep == MediaDownloadManager.DownloadQueueEntry.ProcessingSteps.Cancelled)
            return;
          if (this.ProcessStep != MediaDownloadManager.DownloadQueueEntry.ProcessingSteps.TryAgainLater && this.ProcessStep != MediaDownloadManager.DownloadQueueEntry.ProcessingSteps.Unknown)
          {
            Log.l(MediaDownloadManager.LogHeader, "Unexpected initial processing state {0}, {1}", (object) this.ProcessStep, (object) this.DownloadEntry.DownloadId);
            this.obsForDownload?.OnError((Exception) new MediaDownloadException(MediaDownloadException.DownloadErrorCode.UnexpectedProcessingState, "Unexpected Processing State"));
            this.obsForDownload?.OnCompleted();
            return;
          }
          this.ProcessStep = MediaDownloadManager.DownloadQueueEntry.ProcessingSteps.Started;
          if (this.ctsForDownload != null && !this.ctsForDownload.IsCancellationRequested)
            Log.l(MediaDownloadManager.LogHeader, "Unexpected uncancelled cancellation token found - ignoring");
          this.ctsForDownload = new CancellationTokenSource();
        }
        this.fsEventForDownload = new WhatsApp.Events.MediaDownload();
        this.fsEventForDownload.mediaDownloadResult = new wam_enum_media_download_result_type?(wam_enum_media_download_result_type.ERROR_UNKNOWN);
        if (MediaHelper.IsUserWaiting(this.DownloadEntry.DownloadContext) && AppState.GetConnection() != null && !AppState.GetConnection().IsConnected)
        {
          this.obsForDownload?.OnError((Exception) new MediaDownloadException(MediaDownloadException.DownloadErrorCode.NotConnected, "Not Connected"));
          this.obsForDownload?.OnCompleted();
        }
        else
        {
          try
          {
            string downloadUrl = this.DownloadEntry.DownloadUrl;
            string ipHint = (string) null;
            if (this.DownloadEntry.EncryptedHash != null)
            {
              DateTime routeRequestStart = DateTime.UtcNow;
              Mms4HostSelector.Mms4HostSelection selectedRouteAsync = await MediaDownloadManager.GetSelectedRouteAsync(this.DownloadEntry.DownloadType, true, this.ctsForDownload.Token);
              if (selectedRouteAsync == null)
              {
                this.StoppedDownload("No route", false, true, wam_enum_media_download_result_type.ERROR_DNS);
                this.obsForDownload?.OnError((Exception) new MediaDownloadException(MediaDownloadException.DownloadErrorCode.NoAccessToServer, "No route"));
                this.obsForDownload?.OnCompleted();
                return;
              }
              this.fsEventForDownload.routeHostname = selectedRouteAsync.HostName;
              this.fsEventForDownload.routeIp = (string) null;
              this.fsEventForDownload.routeResponseTtl = new long?((DateTime.UtcNow.Ticks - routeRequestStart.Ticks) / 10000L);
              this.fsEventForDownload.routeSelectionDelayT = new long?((long) (DateTime.UtcNow - routeRequestStart).TotalMilliseconds);
              downloadUrl = Mms4Helper.CreateUrlStringForUpload(selectedRouteAsync.HostName, this.DownloadEntry.DownloadType, selectedRouteAsync.AuthToken, this.DownloadEntry.EncryptedHash, false, false, false);
              ipHint = (string) null;
            }
            lock (this.processStepLock)
            {
              if (this.ProcessStep == MediaDownloadManager.DownloadQueueEntry.ProcessingSteps.Cancelled)
                return;
              if (this.ProcessStep != MediaDownloadManager.DownloadQueueEntry.ProcessingSteps.Started)
              {
                Log.l(MediaDownloadManager.LogHeader, "Unexpected processing state for download {0}, {1}", (object) this.ProcessStep, (object) this.DownloadEntry.DownloadId);
                this.obsForDownload?.OnError((Exception) new MediaDownloadException(MediaDownloadException.DownloadErrorCode.UnexpectedProcessingState, "Unexpected state"));
                this.obsForDownload?.OnCompleted();
                return;
              }
              this.ProcessStep = MediaDownloadManager.DownloadQueueEntry.ProcessingSteps.Downloading;
            }
            IObserver<MediaDownloadProgress> obsForDownload1 = this.obsForDownload;
            if (obsForDownload1 != null)
              obsForDownload1.OnNext(new MediaDownloadProgress(MediaDownloadProgress.DownloadStatus.Active)
              {
                TotalToDownload = this.DownloadEntry.DownloadSize,
                DownloadedSoFar = 0L
              });
            string outputFileUrl = this.DownloadEntry.LocalUrl;
            if (this.DownloadEntry.DecryptionKey != null)
            {
              using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
              {
                string str = "";
                try
                {
                  if (!storeForApplication.DirectoryExists("tmp"))
                    storeForApplication.CreateDirectory("tmp");
                  str = this.DownloadEntry.PlainTextHash == null || this.DownloadEntry.PlainTextHash.Length == 0 ? (this.DownloadEntry.EncryptedHash == null || this.DownloadEntry.EncryptedHash.Length == 0 ? DateTime.UtcNow.Ticks.ToString() : this.DownloadEntry.EncryptedHash.ToHexString()) : this.DownloadEntry.PlainTextHash.ToHexString();
                  str += ".tmp";
                  using (storeForApplication.OpenFile(str, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete))
                    outputFileUrl = Constants.IsoStorePath + "\\" + str;
                  Log.l(MediaDownloadManager.LogHeader, "Using temporary location for download {0} {1}", (object) this.DownloadEntry.DownloadId, (object) outputFileUrl);
                }
                catch (Exception ex1)
                {
                  Log.l(MediaDownloadManager.LogHeader, "Unexpected exception creating {0}, {1}", (object) str, (object) ex1.GetFriendlyMessage());
                  try
                  {
                    storeForApplication.DeleteFile(str);
                    Log.l(MediaDownloadManager.LogHeader, "Deleted work file");
                    using (storeForApplication.OpenFile(str, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete))
                      outputFileUrl = Constants.IsoStorePath + "\\" + str;
                  }
                  catch (Exception ex2)
                  {
                    Log.l(MediaDownloadManager.LogHeader, "Further unexpected exception creating {0}, {1}", (object) outputFileUrl, (object) ex2.GetFriendlyMessage());
                    Log.SendCrashLog((Exception) new InvalidOperationException("Download Isostore issues"), "Further exception", logOnlyForRelease: true);
                    this.StoppedDownload("temp file creation exception", false, false, wam_enum_media_download_result_type.ERROR_INSUFFICIENT_SPACE);
                    this.obsForDownload?.OnError(ex1);
                    this.obsForDownload?.OnCompleted();
                    return;
                  }
                  Log.SendCrashLog((Exception) new InvalidOperationException("Download Isostore issues"), "Initial exception", logOnlyForRelease: true);
                }
              }
              Log.d(MediaDownloadManager.LogHeader, "Downloading to {0}", (object) outputFileUrl);
            }
            lock (this.processStepLock)
            {
              if (this.ProcessStep == MediaDownloadManager.DownloadQueueEntry.ProcessingSteps.Cancelled)
              {
                this.fsEventForDownload.mediaDownloadResult = new wam_enum_media_download_result_type?(wam_enum_media_download_result_type.ERROR_CANCEL);
                return;
              }
              this.ProcessStep = MediaDownloadManager.DownloadQueueEntry.ProcessingSteps.Downloading;
            }
            if (MediaHelper.IsUserWaiting(this.DownloadEntry.DownloadContext) && !NetworkStateMonitor.IsDataConnected())
            {
              this.obsForDownload?.OnError((Exception) new MediaDownloadException(MediaDownloadException.DownloadErrorCode.NotConnected, "Not Data Connected"));
              this.obsForDownload?.OnCompleted();
            }
            else
            {
              MediaDownloadProgress downloadResult = (MediaDownloadProgress) null;
              MediaHelper.DownloadMethodOptions downloadMethod = MediaHelper.DetermineDownloadOptions(this.DownloadEntry.DownloadSize, this.DownloadEntry.DownloadMaxSize, this.DownloadEntry.DownloadContext);
              if (downloadMethod == MediaHelper.DownloadMethodOptions.BTandInApp || downloadMethod == MediaHelper.DownloadMethodOptions.BTOnly)
              {
                try
                {
                  downloadResult = await MediaDownloadManager.RunBackgroundDownloadAsync(this.ctsForDownload, downloadUrl, outputFileUrl, this.obsForDownload);
                  if (downloadResult.DownloadState == MediaDownloadProgress.DownloadStatus.Completed)
                  {
                    IObserver<MediaDownloadProgress> obsForDownload2 = this.obsForDownload;
                    if (obsForDownload2 != null)
                      obsForDownload2.OnNext(new MediaDownloadProgress(MediaDownloadProgress.DownloadStatus.Completed)
                      {
                        HttpRespCode = downloadResult.HttpRespCode
                      });
                    downloadMethod = MediaHelper.DownloadMethodOptions.BTOnly;
                  }
                }
                catch (Exception ex)
                {
                  if (downloadMethod == MediaHelper.DownloadMethodOptions.BTandInApp)
                  {
                    if (!MediaHelper.BTExceptionSuggestsinAppMightWork(ex))
                    {
                      Log.l(MediaDownloadManager.LogHeader, "Suppressing attempt at InApp download after BT failure");
                      downloadMethod = MediaHelper.DownloadMethodOptions.BTOnly;
                    }
                  }
                }
              }
              if (downloadMethod == MediaHelper.DownloadMethodOptions.BTandInApp || downloadMethod == MediaHelper.DownloadMethodOptions.InAppOnly)
              {
                IObservable<MediaDownloadProgress> observable = MediaDownloadManager.InAppDownload(downloadUrl, ipHint, outputFileUrl, true, -1, this.DownloadEntry.DownloadSize, this.fsEventForDownload);
                observable.Do<MediaDownloadProgress>((Action<MediaDownloadProgress>) (progress => this.obsForDownload?.OnNext(progress)));
                Log.l(MediaDownloadManager.LogHeader, "starting In App download for {0}", (object) this.DownloadEntry.DownloadId);
                downloadResult = await MediaDownloadManager.TurnDownloadObservableIntoAsync(this.obsForDownload, observable, "InApp Download", this.ctsForDownload.Token);
              }
              if (downloadResult == null || downloadResult.DownloadState != MediaDownloadProgress.DownloadStatus.Completed)
              {
                string logHeader = MediaDownloadManager.LogHeader;
                object[] objArray = new object[2];
                MediaDownloadProgress downloadProgress = downloadResult;
                objArray[0] = (object) (MediaDownloadProgress.DownloadStatus) (downloadProgress != null ? (int) downloadProgress.DownloadState : 0);
                objArray[1] = (object) this.DownloadEntry.DownloadId;
                Log.l(logHeader, "Unexpected download status: {0}, for {1}", objArray);
                this.StoppedDownload("Unexpected status", false, false, wam_enum_media_download_result_type.ERROR_UNKNOWN);
                this.obsForDownload?.OnError((Exception) new MediaDownloadException(MediaDownloadException.DownloadErrorCode.UnexpectedProcessingState, "Unexpected status"));
                this.obsForDownload?.OnCompleted();
              }
              else
              {
                Log.l(MediaDownloadManager.LogHeader, "processed download for {0}", (object) this.DownloadEntry.DownloadId);
                lock (this.processStepLock)
                {
                  if (this.ProcessStep == MediaDownloadManager.DownloadQueueEntry.ProcessingSteps.Cancelled)
                  {
                    this.StoppedDownload("Cancelled", false, false, wam_enum_media_download_result_type.ERROR_CANCEL);
                    this.obsForDownload?.OnError((Exception) new MediaDownloadException(MediaDownloadException.DownloadErrorCode.CancelledByUser, "Cancelled by user"));
                    this.obsForDownload?.OnCompleted();
                    return;
                  }
                  this.ProcessStep = MediaDownloadManager.DownloadQueueEntry.ProcessingSteps.Decrypting;
                }
                if (outputFileUrl != this.DownloadEntry.LocalUrl)
                {
                  lock (this.processStepLock)
                    this.ProcessStep = MediaDownloadManager.DownloadQueueEntry.ProcessingSteps.Decrypting;
                  MediaDownloadManager.DecryptFileDownload(this.DownloadEntry.DownloadType, this.DownloadEntry.DecryptionKey, outputFileUrl, this.DownloadEntry.LocalUrl, this.DownloadEntry.PlainTextHash, this.fsEventForDownload);
                }
                lock (this.processStepLock)
                  this.ProcessStep = MediaDownloadManager.DownloadQueueEntry.ProcessingSteps.CompletedOK;
                this.StoppedDownload("Completed", true, false, wam_enum_media_download_result_type.OK);
                this.DownloadEntry.StatusChange(DownloadMedia.DownloadStatuses.Running, DownloadMedia.DownloadStatuses.Finished);
                this.obsForDownload?.OnNext(new MediaDownloadProgress(MediaDownloadProgress.DownloadStatus.Completed));
                downloadUrl = (string) null;
                ipHint = (string) null;
                outputFileUrl = (string) null;
                downloadResult = (MediaDownloadProgress) null;
              }
            }
          }
          catch (Exception ex)
          {
            Log.l(MediaDownloadManager.LogHeader, "Exception processing step {0} for {1}", (object) this.ProcessStep, (object) this.DownloadEntry.DownloadId);
            wam_enum_media_download_result_type fsResult = wam_enum_media_download_result_type.ERROR_UNKNOWN;
            bool canRetry = false;
            if (ex is MediaDownloadException)
            {
              switch ((ex as MediaDownloadException).HttpResponseCode)
              {
                case 401:
                  fsResult = wam_enum_media_download_result_type.ERROR_INVALID_URL;
                  break;
                case 404:
                case 410:
                  fsResult = wam_enum_media_download_result_type.ERROR_TOO_OLD;
                  break;
                case 408:
                  fsResult = wam_enum_media_download_result_type.ERROR_TIMEOUT;
                  canRetry = true;
                  break;
                case 416:
                  fsResult = wam_enum_media_download_result_type.ERROR_CANNOT_RESUME;
                  break;
                case 504:
                  fsResult = wam_enum_media_download_result_type.ERROR_DNS;
                  canRetry = true;
                  break;
                default:
                  fsResult = wam_enum_media_download_result_type.ERROR_UNKNOWN;
                  break;
              }
            }
            if (fsResult == wam_enum_media_download_result_type.ERROR_UNKNOWN)
              Log.SendCrashLog(ex, MediaDownloadManager.LogHeader + " Exception", logOnlyForRelease: true);
            this.StoppedDownload("Exception", false, canRetry, fsResult);
            this.DownloadEntry.StatusChange(DownloadMedia.DownloadStatuses.Running, DownloadMedia.DownloadStatuses.Cancelled);
            this.obsForDownload?.OnError(ex);
            this.obsForDownload?.OnCompleted();
          }
          finally
          {
            this.obsForDownload?.OnCompleted();
            MediaDownloadManager.PerformNextDownload();
          }
        }
      }

      public void CancelDownload()
      {
        Log.d(MediaDownloadManager.LogHeader, "Stopping entry {0} {1}", (object) this.ProcessStep, (object) (this.DownloadEntry?.DownloadId ?? "null"));
        MediaDownloadManager.GetInstance();
        lock (this.processStepLock)
        {
          if (this.ProcessStepChange(new MediaDownloadManager.DownloadQueueEntry.ProcessingSteps[3]
          {
            MediaDownloadManager.DownloadQueueEntry.ProcessingSteps.Unknown,
            MediaDownloadManager.DownloadQueueEntry.ProcessingSteps.Cancelled,
            MediaDownloadManager.DownloadQueueEntry.ProcessingSteps.TryAgainLater
          }, MediaDownloadManager.DownloadQueueEntry.ProcessingSteps.Cancelled).HasValue)
            Log.l(MediaDownloadManager.LogHeader, "Stopped unprocessed entry {0} {1}", (object) this.DownloadEntry.DownloadType, (object) this.DownloadEntry.DownloadId);
          else if (this.ProcessStep == MediaDownloadManager.DownloadQueueEntry.ProcessingSteps.Started || this.ProcessStep == MediaDownloadManager.DownloadQueueEntry.ProcessingSteps.Downloading || this.ProcessStep == MediaDownloadManager.DownloadQueueEntry.ProcessingSteps.Decrypting || this.ProcessStep == MediaDownloadManager.DownloadQueueEntry.ProcessingSteps.MediaCheck)
          {
            this.ctsForDownload.Cancel();
            this.fsEventForDownload.mediaDownloadResult = new wam_enum_media_download_result_type?(wam_enum_media_download_result_type.ERROR_CANCEL);
            this.ProcessStep = MediaDownloadManager.DownloadQueueEntry.ProcessingSteps.Cancelling;
          }
          else
          {
            Log.l(MediaDownloadManager.LogHeader, "Cancelling while in state {0}", (object) this.ProcessStep);
            Log.SendCrashLog((Exception) new InvalidOperationException("cancelling unexpected state download"), "Cancel in unexpected state", logOnlyForRelease: true);
            this.ctsForDownload.Cancel();
            this.fsEventForDownload.mediaDownloadResult = new wam_enum_media_download_result_type?(wam_enum_media_download_result_type.ERROR_CANCEL);
            this.ProcessStep = MediaDownloadManager.DownloadQueueEntry.ProcessingSteps.Cancelling;
          }
        }
      }

      private void StoppedDownload(
        string context,
        bool completed,
        bool canRetry,
        wam_enum_media_download_result_type fsResult)
      {
        Log.d(MediaDownloadManager.LogHeader, "Stopping entry for {0} at {1} - {2}", (object) context, (object) this.ProcessStep, (object) (this.DownloadEntry?.DownloadId ?? "null"));
        lock (this.processStepLock)
        {
          this.fsEventForDownload.mediaDownloadResult = new wam_enum_media_download_result_type?(fsResult);
          if (completed)
            this.ProcessStep = MediaDownloadManager.DownloadQueueEntry.ProcessingSteps.CompletedOK;
          else if (this.ProcessStep == MediaDownloadManager.DownloadQueueEntry.ProcessingSteps.Cancelling)
          {
            this.ProcessStep = MediaDownloadManager.DownloadQueueEntry.ProcessingSteps.Cancelled;
          }
          else
          {
            Log.l(MediaDownloadManager.LogHeader, "Unexpected process state {0}, for {1}", (object) this.ProcessStep, (object) this.DownloadEntry.DownloadId);
            if (canRetry)
              this.ProcessStep = MediaDownloadManager.DownloadQueueEntry.ProcessingSteps.TryAgainLater;
            else
              this.ProcessStep = MediaDownloadManager.DownloadQueueEntry.ProcessingSteps.Cancelled;
          }
        }
      }

      private bool StatusChange(
        MediaDownloadManager.DownloadQueueEntry.ProcessingSteps from,
        MediaDownloadManager.DownloadQueueEntry.ProcessingSteps to)
      {
        return this.ProcessStepChange(new MediaDownloadManager.DownloadQueueEntry.ProcessingSteps[1]
        {
          from
        }, to).HasValue;
      }

      private MediaDownloadManager.DownloadQueueEntry.ProcessingSteps? ProcessStepChange(
        MediaDownloadManager.DownloadQueueEntry.ProcessingSteps[] from,
        MediaDownloadManager.DownloadQueueEntry.ProcessingSteps to)
      {
        lock (this.processStepLock)
        {
          MediaDownloadManager.DownloadQueueEntry.ProcessingSteps processStep = this.ProcessStep;
          if (((IEnumerable<MediaDownloadManager.DownloadQueueEntry.ProcessingSteps>) from).Contains<MediaDownloadManager.DownloadQueueEntry.ProcessingSteps>(this.ProcessStep))
          {
            this.ProcessStep = to;
            return new MediaDownloadManager.DownloadQueueEntry.ProcessingSteps?(processStep);
          }
        }
        return new MediaDownloadManager.DownloadQueueEntry.ProcessingSteps?();
      }

      public enum ProcessingSteps
      {
        Unknown = 0,
        Started = 1,
        Downloading = 2,
        Decrypting = 3,
        MediaCheck = 4,
        TryAgainLater = 55, // 0x00000037
        Cancelling = 98, // 0x00000062
        Cancelled = 99, // 0x00000063
        CompletedOK = 100, // 0x00000064
      }
    }

    private struct FallbackOption
    {
      public Action<IWebRequest> SetResolver;
      public Func<string, string> UrlMap;
    }
  }
}
