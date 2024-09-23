// Decompiled with JetBrains decompiler
// Type: WhatsApp.OptimisticUploadManager
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;


namespace WhatsApp
{
  public class OptimisticUploadManager : IDisposable
  {
    private bool optimisticUploadStopped = !MediaUpload.OptimisticUploadAllowed;
    private DateTime optimisticUploadStartTime = DateTime.Now;
    private wam_enum_opt_upload_context_type context;
    private object optimisticQueueLock = new object();
    private IDisposable timerSub;
    private List<OptimisticUpload> optimisticUploadList = new List<OptimisticUpload>();
    private bool fieldstatsSent;
    private int[] actionCounters = new int[12];
    private const int SUPPLIED_INDEX = 0;
    private const int DISABLED_INDEX = 1;
    private const int OPT_UPLOAD_STOPPED_INDEX = 2;
    private const int NOT_APPROPRIATE_INDEX = 3;
    private const int ADDED_INDEX = 4;
    private const int DELETED_B4_UPLOAD_INDEX = 5;
    private const int DELETED_UPLOAD_OK_INDEX = 6;
    private const int DELETED_UPLOAD_FAILED_INDEX = 7;
    private const int SENT_B4_UPLOAD_INDEX = 8;
    private const int SENT_UPLOAD_OK_INDEX = 9;
    private const int SENT_UPLOAD_FAILED_INDEX = 10;
    private const int SENT_USEFUL_CONTENT_INDEX = 11;

    public OptimisticUploadManager(wam_enum_opt_upload_context_type context)
    {
      this.optimisticUploadStopped = !MediaUpload.OptimisticUploadAllowed;
      this.context = context;
      for (int index = 0; index < this.actionCounters.Length; ++index)
        this.actionCounters[index] = 0;
    }

    public bool AddToOptimisticUploadQueue(MediaSharingState.PicInfo picInfo, string localMediaUrl)
    {
      ++this.actionCounters[0];
      if (!MediaUpload.OptimisticUploadAllowed)
      {
        ++this.actionCounters[1];
        return false;
      }
      if (this.optimisticUploadStopped)
      {
        ++this.actionCounters[2];
        return false;
      }
      foreach (OptimisticUpload optimisticUpload in this.optimisticUploadList)
      {
        if (optimisticUpload.PicInfo.Equals((object) picInfo))
        {
          Log.l("OP", "Re-adding existing item to optimistic upload list {0}", (object) picInfo.GetOptimisticUniqueId());
          return false;
        }
      }
      OptimisticUpload optimisticUpload1 = OptimisticUpload.createOptimisticUpload(picInfo, localMediaUrl);
      if (optimisticUpload1 == null)
      {
        ++this.actionCounters[3];
        return false;
      }
      ++this.actionCounters[4];
      this.optimisticUploadList.Add(optimisticUpload1);
      lock (this.optimisticQueueLock)
      {
        if (!this.optimisticUploadStopped)
        {
          if (this.timerSub == null)
            this.timerSub = Observable.Interval(TimeSpan.FromSeconds(1.0)).ObserveOn<long>(WAThreadPool.Scheduler).SubscribeOn<long>(WAThreadPool.Scheduler).Subscribe<long>((Action<long>) (tick =>
            {
              Log.d("OP", "Timer tick {0}", (object) this.optimisticUploadList.Count);
              if (this.optimisticUploadStopped)
                return;
              OptimisticUpload optimisticUpload2 = (OptimisticUpload) null;
              lock (this.optimisticQueueLock)
              {
                bool flag = true;
                foreach (OptimisticUpload optimisticUpload3 in this.optimisticUploadList)
                {
                  if (!optimisticUpload3.IsFinished())
                    flag = false;
                  if (optimisticUpload3.IsActive())
                  {
                    optimisticUpload2 = (OptimisticUpload) null;
                    break;
                  }
                  if (optimisticUpload3.IsReady())
                  {
                    if (optimisticUpload2 == null)
                      optimisticUpload2 = optimisticUpload3;
                    else if (optimisticUpload2.ReadyTime > optimisticUpload3.ReadyTime)
                      optimisticUpload2 = optimisticUpload3;
                  }
                }
                if (!flag)
                {
                  if (!this.optimisticUploadStopped)
                    goto label_24;
                }
                Log.l("OP", "Stopping optimistic upload timer, nothing to do");
                this.StopOptUploadTimer();
              }
label_24:
              if (optimisticUpload2 == null)
                return;
              if (this.optimisticUploadStopped)
                return;
              try
              {
                optimisticUpload2.StartUpload();
              }
              catch (Exception ex)
              {
                this.optimisticUploadStopped = true;
                Log.l(ex, "OP Stopping optimistic upload after exception");
              }
            }));
        }
      }
      return true;
    }

    public bool RemoveFromOptimisticUploadQueue(MediaSharingState.PicInfo picInfo)
    {
      OptimisticUpload removedItem = (OptimisticUpload) null;
      lock (this.optimisticQueueLock)
      {
        foreach (OptimisticUpload optimisticUpload in this.optimisticUploadList)
        {
          if (optimisticUpload.PicInfo.Equals((object) picInfo))
          {
            this.optimisticUploadList.Remove(optimisticUpload);
            removedItem = optimisticUpload;
            break;
          }
        }
      }
      if (removedItem == null)
        return false;
      this.setDeleteAction(removedItem);
      removedItem.StopUpload(true);
      removedItem.Dispose();
      return true;
    }

    public Dictionary<string, OptimisticUpload> RetrieveOptimisticUploads()
    {
      Log.l("OP", "retrieve uploads | Count: {0}", (object) this.optimisticUploadList.Count);
      this.StopOptUploadTimer();
      Dictionary<string, OptimisticUpload> dictionary = new Dictionary<string, OptimisticUpload>();
      lock (this.optimisticQueueLock)
      {
        foreach (OptimisticUpload optimisticUpload in this.optimisticUploadList)
        {
          this.setSentAction(optimisticUpload);
          if (optimisticUpload.HasUsefulContent())
          {
            ++this.actionCounters[11];
            dictionary[optimisticUpload.PicInfo.GetOptimisticUniqueId()] = optimisticUpload;
            optimisticUpload.StopUpload(false);
          }
          else
            optimisticUpload.Dispose();
        }
        this.optimisticUploadList.Clear();
      }
      Log.l("OP", "retrieve uploads | useful Count: {0}", (object) dictionary.Count);
      this.maybeSendFsEvent();
      return dictionary;
    }

    private void StopOptUploadTimer()
    {
      this.timerSub.SafeDispose();
      this.timerSub = (IDisposable) null;
    }

    public void Dispose()
    {
      Log.l("OP", "disposing optimistic uploads | Count: {0}", (object) this.optimisticUploadList.Count);
      this.StopOptUploadTimer();
      lock (this.optimisticQueueLock)
      {
        foreach (OptimisticUpload optimisticUpload in this.optimisticUploadList)
        {
          try
          {
            optimisticUpload.StopUpload(true);
          }
          catch (Exception ex)
          {
            string context = "OP exception stopping upload " + optimisticUpload.UniqueId;
            Log.LogException(ex, context);
          }
          this.setDeleteAction(optimisticUpload);
          optimisticUpload.Dispose();
        }
        this.optimisticUploadList.Clear();
      }
      this.maybeSendFsEvent();
    }

    private void setDeleteAction(OptimisticUpload removedItem)
    {
      if (removedItem == null)
        return;
      int index = 7;
      switch (removedItem.GetSendState())
      {
        case OptimisticUpload.SendState.NOT_STARTED:
          index = 5;
          break;
        case OptimisticUpload.SendState.FINISHED_OK:
          index = 6;
          break;
      }
      ++this.actionCounters[index];
    }

    private void setSentAction(OptimisticUpload optimisticUpload)
    {
      if (optimisticUpload == null)
        return;
      int index = 10;
      switch (optimisticUpload.GetSendState())
      {
        case OptimisticUpload.SendState.NOT_STARTED:
          index = 8;
          break;
        case OptimisticUpload.SendState.FINISHED_OK:
          index = 9;
          break;
      }
      ++this.actionCounters[index];
    }

    private void maybeSendFsEvent()
    {
      if (this.fieldstatsSent)
        return;
      this.fieldstatsSent = true;
      new WhatsApp.Events.OptimisticUpload()
      {
        mediaType = new wam_enum_media_type?(wam_enum_media_type.PHOTO),
        optSupplied = new long?((long) this.actionCounters[0]),
        optIgnoredDisabled = new long?((long) this.actionCounters[1]),
        optIgnoredStopped = new long?((long) this.actionCounters[2]),
        optIgnoredNotAppropriate = new long?((long) this.actionCounters[3]),
        optAdded = new long?((long) this.actionCounters[4]),
        optDeletedB4Upload = new long?((long) this.actionCounters[5]),
        optDeletedUploadOk = new long?((long) this.actionCounters[6]),
        optDeletedUploadNotOk = new long?((long) this.actionCounters[7]),
        optSentB4Upload = new long?((long) this.actionCounters[8]),
        optSentUploadOk = new long?((long) this.actionCounters[9]),
        optSentUploadNotOk = new long?((long) this.actionCounters[10]),
        optSentUploadUseful = new long?((long) this.actionCounters[11]),
        optUploadContext = new wam_enum_opt_upload_context_type?(this.context),
        optUploadAvailableT = new long?((long) (DateTime.Now - this.optimisticUploadStartTime).Milliseconds)
      }.SaveEvent();
    }
  }
}
