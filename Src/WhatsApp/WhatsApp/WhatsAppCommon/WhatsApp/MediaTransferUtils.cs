// Decompiled with JetBrains decompiler
// Type: WhatsApp.MediaTransferUtils
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;


namespace WhatsApp
{
  public static class MediaTransferUtils
  {
    private static PendingTransfers Instance = new PendingTransfers();
    private static object lockObject = new object();
    private static Dictionary<int, PendingMediaTransfer> stalledTransfers = new Dictionary<int, PendingMediaTransfer>();
    private static IDisposable voipCompletionSub = (IDisposable) null;
    private static IDisposable voipCallStartedSub = (IDisposable) null;
    private static object voipSubLock = new object();

    public static bool AddPendingTransfer(
      Message m,
      PendingMediaTransfer.TransferTypes tType,
      Action<Unit> onNext,
      Action<Exception> onError,
      Action onCompleted)
    {
      bool flag;
      if (tType == PendingMediaTransfer.TransferTypes.Transcode)
      {
        flag = true;
      }
      else
      {
        lock (MediaTransferUtils.voipSubLock)
        {
          PendingMediaTransfer pTransfer = new PendingMediaTransfer(m, tType, onNext, onError, onCompleted);
          lock (MediaTransferUtils.lockObject)
          {
            flag = !Voip.IsInCall && MediaTransferUtils.voipCompletionSub == null || tType == PendingMediaTransfer.TransferTypes.Upload_NotWeb || tType == PendingMediaTransfer.TransferTypes.Upload_Web || tType == PendingMediaTransfer.TransferTypes.Download_Foreground_Interactive || tType == PendingMediaTransfer.TransferTypes.Download_Foreground_Interactive_Streaming;
            Log.d("media transfer", "media transfer add {0} {1} {2} {3}", (object) flag, (object) Voip.IsInCall, (object) (MediaTransferUtils.voipCompletionSub == null), (object) tType);
            if (flag)
            {
              MediaTransferUtils.Instance.AddTransfer(pTransfer);
              MediaTransferUtils.StopTransfersOnVoipCall();
            }
            else
            {
              MediaTransferUtils.stalledTransfers[pTransfer.GetHashCode()] = pTransfer;
              MediaTransferUtils.RestartDownloadsOnVoipCallCompletion();
            }
          }
        }
      }
      Log.d("media transfer", "adding transfer {0} {1} {2}", (object) MediaTransferUtils.stalledTransfers.Count, (object) tType, (object) flag);
      return flag;
    }

    public static void RemovePendingTransfer(Message m)
    {
      bool flag = false;
      PendingMediaTransfer pTransfer = new PendingMediaTransfer(m, PendingMediaTransfer.TransferTypes.Transcode, (Action<Unit>) null, (Action<Exception>) null, (Action) null);
      lock (MediaTransferUtils.lockObject)
      {
        flag = MediaTransferUtils.Instance.RemoveTransfer(pTransfer);
        if (!flag)
        {
          int hashCode = pTransfer.GetHashCode();
          flag = MediaTransferUtils.stalledTransfers.Remove(hashCode);
        }
      }
      Log.d("media transfer", "removing transfer {0} {1}", (object) MediaTransferUtils.stalledTransfers.Count, (object) flag);
    }

    private static void StallPendingTransfers()
    {
      Dictionary<int, PendingMediaTransfer> pendingTransfers = MediaTransferUtils.Instance.ExtractPendingTransfers();
      Log.d("media transfer", "Found {0} to stop", (object) pendingTransfers.Count);
      int num = 0;
      lock (MediaTransferUtils.lockObject)
      {
        foreach (PendingMediaTransfer pendingMediaTransfer in pendingTransfers.Values)
        {
          PendingMediaTransfer pTransfer = pendingMediaTransfer;
          if (pTransfer != null)
          {
            Message msg = (Message) null;
            MessagesContext.Run((MessagesContext.MessagesCallback) (db => msg = db.GetMessage(pTransfer.Jid, pTransfer.Id, pTransfer.FromMe)));
            if (msg != null && msg.CancelPendingMedia())
            {
              MediaTransferUtils.stalledTransfers[pTransfer.GetHashCode()] = pTransfer;
              ++num;
            }
          }
        }
      }
      Log.l("media transfer", "Found {0} to stop and stopped {1}", (object) pendingTransfers.Count, (object) num);
    }

    private static void RestartStalledTransfers()
    {
      List<PendingMediaTransfer> source1 = new List<PendingMediaTransfer>();
      lock (MediaTransferUtils.lockObject)
      {
        foreach (PendingMediaTransfer pendingMediaTransfer in MediaTransferUtils.stalledTransfers.Values)
          source1.Add(pendingMediaTransfer);
        MediaTransferUtils.stalledTransfers = new Dictionary<int, PendingMediaTransfer>();
      }
      Log.d("media transfer", "Found {0} to restart", (object) source1.Count);
      if (AppState.IsBackgroundAgent)
      {
        Log.l("media transfer", "Not restarting {0} transfers - running in BG", (object) source1.Count);
      }
      else
      {
        int num1 = 0;
        int num2 = 0;
        List<PendingMediaTransfer> list = source1.OrderBy<PendingMediaTransfer, DateTime>((Func<PendingMediaTransfer, DateTime>) (o => o.CreatedTime)).ToList<PendingMediaTransfer>();
        foreach (PendingMediaTransfer pTransfer in list)
        {
          if (pTransfer.TransferType != PendingMediaTransfer.TransferTypes.Transcode)
          {
            Message forPendingTransfer = MediaTransferUtils.getMessageForPendingTransfer(pTransfer);
            if (forPendingTransfer != null)
            {
              bool flag1 = false;
              bool flag2 = false;
              bool interactive = false;
              bool webRetry = false;
              switch (pTransfer.TransferType)
              {
                case PendingMediaTransfer.TransferTypes.Upload_NotWeb:
                  flag1 = true;
                  break;
                case PendingMediaTransfer.TransferTypes.Upload_Web:
                  flag1 = true;
                  webRetry = true;
                  break;
                case PendingMediaTransfer.TransferTypes.Download_Foreground_Interactive:
                  flag2 = true;
                  interactive = true;
                  break;
                case PendingMediaTransfer.TransferTypes.Download_Foreground_NotInteractive:
                  flag2 = true;
                  break;
                case PendingMediaTransfer.TransferTypes.Download_Foreground_Interactive_Streaming:
                  continue;
              }
              if (flag1)
              {
                ++num1;
                forPendingTransfer.SetPendingMediaSubscription("Media Voip resume", pTransfer.TransferType, MediaUpload.SendMediaObservable(forPendingTransfer, webRetry));
              }
              else
              {
                ++num2;
                WhatsApp.Events.MediaDownload mediaDownloadEvent = FieldStats.GetFsMediaDownloadEvent(forPendingTransfer);
                IObservable<MediaDownload.MediaProgress> source2 = !flag2 ? MediaDownload.TransferFromBackground(forPendingTransfer, mediaDownloadEvent) : MediaDownload.TransferFromForeground(forPendingTransfer, mediaDownloadEvent, interactive);
                forPendingTransfer.SetPendingMediaSubscription("Media Voip resume", pTransfer.TransferType, MediaDownload.TransferForMessageObservable(forPendingTransfer, source2, mediaDownloadEvent), pTransfer.OnNext, pTransfer.OnError, pTransfer.OnCompleted);
              }
            }
          }
        }
        Log.l("media transfer", "Found {0} to restart, restarted {1} upload(s) and {2} download(s)", (object) list.Count, (object) num1, (object) num2);
      }
    }

    internal static Message getMessageForPendingTransfer(PendingMediaTransfer pTransfer)
    {
      Message msg = (Message) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db => msg = db.GetMessage(pTransfer.Jid, pTransfer.Id, pTransfer.FromMe)));
      return msg;
    }

    public static void RestartDownloadsOnVoipCallCompletion()
    {
      if (MediaTransferUtils.voipCompletionSub != null)
        return;
      MediaTransferUtils.voipCompletionSub = VoipHandler.CallEndedSubject.ObserveOnDispatcher<WaCallEndedEventArgs>().Subscribe<WaCallEndedEventArgs>((Action<WaCallEndedEventArgs>) (args =>
      {
        Log.d("media transfer", "restarting transfers now Voip call is over");
        lock (MediaTransferUtils.voipSubLock)
        {
          MediaTransferUtils.voipCompletionSub.SafeDispose();
          MediaTransferUtils.voipCompletionSub = (IDisposable) null;
        }
        MediaTransferUtils.RestartStalledTransfers();
      }));
    }

    public static void StopTransfersOnVoipCall()
    {
      if (MediaTransferUtils.voipCallStartedSub != null)
        return;
      MediaTransferUtils.voipCallStartedSub = VoipHandler.CallStartedSubject.Subscribe<Unit>((Action<Unit>) (u =>
      {
        Log.d("media transfer", "stopping transfers while in Voip call");
        lock (MediaTransferUtils.voipSubLock)
        {
          MediaTransferUtils.voipCallStartedSub.SafeDispose();
          MediaTransferUtils.voipCallStartedSub = (IDisposable) null;
          MediaTransferUtils.RestartDownloadsOnVoipCallCompletion();
        }
        MediaTransferUtils.StallPendingTransfers();
      }));
    }
  }
}
