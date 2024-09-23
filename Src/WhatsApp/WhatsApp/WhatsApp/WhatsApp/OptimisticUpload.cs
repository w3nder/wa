// Decompiled with JetBrains decompiler
// Type: WhatsApp.OptimisticUpload
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;


namespace WhatsApp
{
  public class OptimisticUpload : IDisposable
  {
    private const int SECONDS_DELAY_BEFORE_STARTING = 3;
    private string uniqueId;
    private OptimisticJpegUploadContext optMsgUploadContext;
    private static object lockObject = new object();
    private bool isUploadable = true;
    private DateTime? uploadStartedAt;
    private IDisposable uploadDisposable;
    private bool uploadCompleted;
    private bool uploadFailed;
    private bool removeUploadInitiated;

    public Message OptimisticMessage => this.optMsgUploadContext?.Message;

    public string UniqueId
    {
      get
      {
        if (this.uniqueId == null)
          this.uniqueId = this.PicInfo.GetOptimisticUniqueId();
        return this.uniqueId;
      }
    }

    public MediaSharingState.PicInfo PicInfo { get; private set; }

    private string LocalMediaUrl { get; set; }

    public DateTime ReadyTime { get; private set; } = DateTime.Now.AddSeconds(3.0);

    private OptimisticUpload(MediaSharingState.PicInfo picInfo, string localMediaUrl)
    {
      this.PicInfo = picInfo;
      this.LocalMediaUrl = localMediaUrl;
    }

    public static OptimisticUpload createOptimisticUpload(
      MediaSharingState.PicInfo picInfo,
      string localMediaUrl)
    {
      return picInfo.GetPathForOptimisticUpload() == null ? (OptimisticUpload) null : new OptimisticUpload(picInfo, localMediaUrl);
    }

    public bool IsActive()
    {
      lock (OptimisticUpload.lockObject)
        return this.isUploadable && !this.uploadCompleted && this.uploadDisposable != null;
    }

    public bool IsReady()
    {
      lock (OptimisticUpload.lockObject)
        return this.isUploadable && !this.uploadCompleted && this.uploadDisposable == null && this.ReadyTime < DateTime.Now;
    }

    public bool IsFinished()
    {
      lock (OptimisticUpload.lockObject)
      {
        if (this.isUploadable && !this.uploadCompleted)
        {
          if (!this.uploadFailed)
            goto label_7;
        }
        return true;
      }
label_7:
      return false;
    }

    public bool HasUsefulContent()
    {
      lock (OptimisticUpload.lockObject)
        return this.isUploadable && this.optMsgUploadContext.wasOptimisticallyUploaded();
    }

    public bool SetUseFileOnServer()
    {
      OptimisticJpegUploadContext msgUploadContext = this.optMsgUploadContext;
      if ((msgUploadContext != null ? (msgUploadContext.IsMms4Upload ? 1 : 0) : 0) == 0)
        return true;
      lock (OptimisticUpload.lockObject)
      {
        if (!this.removeUploadInitiated)
        {
          Log.l("OP", "Don't remove Upload for {0}", (object) this.UniqueId);
          this.removeUploadInitiated = false;
          return true;
        }
      }
      return false;
    }

    public OptimisticUpload.SendState GetSendState()
    {
      lock (OptimisticUpload.lockObject)
      {
        if (this.uploadCompleted)
          return OptimisticUpload.SendState.FINISHED_OK;
        if (this.uploadFailed)
          return OptimisticUpload.SendState.FINISHED_FAILED;
        return this.uploadStartedAt.HasValue ? OptimisticUpload.SendState.ACTIVE : OptimisticUpload.SendState.NOT_STARTED;
      }
    }

    public void Dispose()
    {
      this.uploadDisposable.SafeDispose();
      this.uploadDisposable = (IDisposable) null;
      this.RemoveFromServer();
      this.optMsgUploadContext.SafeDispose();
      this.optMsgUploadContext = (OptimisticJpegUploadContext) null;
    }

    public bool StartUpload()
    {
      lock (OptimisticUpload.lockObject)
      {
        if (this.uploadStartedAt.HasValue || !this.isUploadable)
          return false;
        Log.l("OP", "Attempting Upload for {0}", (object) this.UniqueId);
        this.uploadStartedAt = new DateTime?(DateTime.Now);
      }
      if (!AppState.ClientInstance.GetConnection().IsConnected)
      {
        Log.l("OP", "Ignoring Upload for {0} - no connectivity", (object) this.UniqueId);
        this.isUploadable = false;
      }
      try
      {
        IObservable<Unit> mediaUpload = this.SendToMediaUpload();
        if (mediaUpload != null)
        {
          this.uploadDisposable = mediaUpload.Subscribe<Unit>((Action<Unit>) (unit =>
          {
            Log.l("OP", "Upload onNext: {0}", (object) this.UniqueId);
            this.OptimisticUploadFinished(true);
          }), (Action<Exception>) (ex =>
          {
            Log.LogException(ex, string.Format("OP Exception sending Upload: {0}", (object) this.UniqueId));
            this.OptimisticUploadFinished(false);
          }), (Action) (() => Log.l("OP", "Upload onCompletion: {0}", (object) this.UniqueId)));
        }
        else
        {
          Log.l("OP", "Ignoring Upload for {0} - not appropriate", (object) this.UniqueId);
          this.isUploadable = false;
        }
      }
      catch (Exception ex)
      {
        this.isUploadable = false;
        string context = string.Format("OP Exception in sendToMediaUpload: {0}", (object) this.UniqueId);
        Log.LogException(ex, context);
        throw;
      }
      return this.isUploadable;
    }

    public bool StopUpload(bool removeUploadFromServer)
    {
      if (!this.isUploadable)
        return true;
      if (this.uploadStartedAt.HasValue)
      {
        if (!this.uploadFailed && !this.uploadCompleted)
        {
          Log.l("OP", "Upload - stop in progress upload: {0}", (object) this.UniqueId);
          this.uploadDisposable.SafeDispose();
          this.uploadDisposable = (IDisposable) null;
        }
        else if (!this.uploadFailed)
          Log.l("OP", "Upload - stop completed upload: {0}", (object) this.UniqueId);
        else
          Log.l("OP", "Upload - stop failed upload: {0}", (object) this.UniqueId);
        this.uploadCompleted = this.uploadFailed = true;
        if (removeUploadFromServer)
          this.RemoveFromServer();
        return true;
      }
      Log.l("OP", "Optimistic Upload Cancel ignored - not started upload: {0}", (object) this.UniqueId);
      this.uploadCompleted = this.uploadFailed = true;
      return false;
    }

    private bool RemoveFromServer()
    {
      lock (OptimisticUpload.lockObject)
      {
        if (this.removeUploadInitiated)
          return false;
        Log.l("OP", "Removing Upload for {0}", (object) this.UniqueId);
        this.removeUploadInitiated = true;
      }
      if (this.OptimisticMessage != null && this.OptimisticMessage.MediaUrl != null)
      {
        if (this.optMsgUploadContext.IsMms4Upload)
          MediaUploadMms4.DeleteMedia(this.OptimisticMessage.MediaUploadUrl);
        else if (this.optMsgUploadContext.PersonalRef != null)
          MediaDownload.AckMedia(this.OptimisticMessage.MediaUrl + "?ack=" + this.optMsgUploadContext.PersonalRef, true);
      }
      return true;
    }

    private void OptimisticUploadFinished(bool uploadOK)
    {
      Log.l("OP", "Completed Optimistic Upload for {0} - {1}", (object) this.UniqueId, (object) uploadOK);
      this.uploadFailed = !uploadOK;
      this.uploadCompleted = true;
      this.uploadDisposable.SafeDispose();
      this.uploadDisposable = (IDisposable) null;
    }

    private IObservable<Unit> SendToMediaUpload()
    {
      if (this.PicInfo.GetImageStream() == null)
      {
        Log.l("OP", "input stream null - don't do optimistic upload: {0}", (object) this.UniqueId);
        return (IObservable<Unit>) null;
      }
      if (this.PicInfo.isChangedByUser())
      {
        Log.l("OP", "image changed by user - don't do optimistic upload: {0}", (object) this.UniqueId);
        return (IObservable<Unit>) null;
      }
      WriteableBitmap bitmap = (WriteableBitmap) null;
      Stream jpegStream = (Stream) null;
      DateTime now = DateTime.Now;
      long jpegStreamDispStartTimeTicks = now.Ticks;
      Deployment.Current.Dispatcher.InvokeSynchronous((Action) (() =>
      {
        jpegStreamDispStartTimeTicks = DateTime.Now.Ticks;
        try
        {
          Stream imageStream = this.PicInfo.GetImageStream();
          imageStream.Position = 0L;
          jpegStream = MediaUpload.CreateImageStreamAndPicture(imageStream, ref bitmap);
          imageStream.Position = 0L;
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "Exception creating stream for Optimistic upload");
          bitmap = (WriteableBitmap) null;
        }
      }));
      object[] objArray = new object[2];
      now = DateTime.Now;
      objArray[0] = (object) ((now.Ticks - jpegStreamDispStartTimeTicks) / 10000L);
      objArray[1] = (object) ((jpegStreamDispStartTimeTicks - jpegStreamDispStartTimeTicks) / 10000L);
      Log.l("OP", "jpeg stream creation held dispatcher for {0}ms after waiting {1}ms", objArray);
      if (bitmap == null || jpegStream == null)
      {
        Log.l("OP", "jpeg stream null or bitmap null - don't do optimistic upload: {0}", (object) this.UniqueId);
        return (IObservable<Unit>) null;
      }
      bitmap = (WriteableBitmap) null;
      this.optMsgUploadContext = MediaUpload.CreateOptimisticUploadWithPicture(this.UniqueId, jpegStream, this.LocalMediaUrl);
      return this.optMsgUploadContext == null ? (IObservable<Unit>) null : MediaUpload.SendOptimisticMediaObservable(this.optMsgUploadContext);
    }

    public enum SendState
    {
      NOT_STARTED,
      ACTIVE,
      FINISHED_OK,
      FINISHED_FAILED,
    }
  }
}
