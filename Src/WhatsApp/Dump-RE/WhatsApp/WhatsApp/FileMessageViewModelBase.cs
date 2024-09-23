// Decompiled with JetBrains decompiler
// Type: WhatsApp.FileMessageViewModelBase
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using WhatsApp.WaCollections;

#nullable disable
namespace WhatsApp
{
  public class FileMessageViewModelBase : MessageViewModel
  {
    protected bool largeThumbFetchAttempted;
    protected IDisposable largeThumbSub;
    private bool durationFetchAttempted;

    public virtual bool ShouldShowActionButton
    {
      get
      {
        if (this.Message.TransferInProgress)
          return false;
        return this.Message.MediaWaType == FunXMPP.FMessage.Type.Audio || this.Message.MediaWaType == FunXMPP.FMessage.Type.Document;
      }
    }

    public virtual BitmapSource ActionButtonIcon => (BitmapSource) null;

    public virtual BitmapSource ActionButtonIconReversed => (BitmapSource) null;

    public virtual bool ShouldShowAccessoryButton
    {
      get
      {
        return !this.Message.LocalFileExists() && this.Message.MediaWaType == FunXMPP.FMessage.Type.Video && this.Message.HasSidecar();
      }
    }

    public override bool ShouldShowText => this.Message.MediaCaption != null;

    public override bool ShouldUseFooterProtection
    {
      get
      {
        bool footerProtection = false;
        switch (this.Message.MediaWaType)
        {
          case FunXMPP.FMessage.Type.Image:
          case FunXMPP.FMessage.Type.Video:
          case FunXMPP.FMessage.Type.Gif:
            footerProtection = !this.ShouldShowText;
            break;
        }
        return footerProtection;
      }
    }

    public bool ShouldShowTransferStatusBar
    {
      get
      {
        return this.Message.TransferInProgress && (this.Message.KeyFromMe || string.IsNullOrEmpty(this.Message.LocalFileUri)) && (this.Message.MediaWaType != FunXMPP.FMessage.Type.Video || !this.Message.HasSidecar() || this.Message.KeyFromMe);
      }
    }

    public bool ShouldShowTransferProgressBar
    {
      get
      {
        bool flag = this.Message.HasSidecar() && !this.Message.KeyFromMe;
        if (!this.Message.TransferInProgress)
          return false;
        return this.Message.MediaWaType == FunXMPP.FMessage.Type.Video && !flag || this.Message.MediaWaType == FunXMPP.FMessage.Type.Document || this.Message.MediaWaType == FunXMPP.FMessage.Type.Sticker || this.Message.MediaWaType == FunXMPP.FMessage.Type.Gif;
      }
    }

    public double TransferProgressBarValue => this.Message.TransferValue;

    public virtual bool ShouldShowMediaInfo
    {
      get
      {
        return (this.Message.MediaWaType == FunXMPP.FMessage.Type.Video || this.Message.MediaWaType == FunXMPP.FMessage.Type.Audio) && !this.Message.TransferInProgress && !this.ShouldShowAccessoryButton;
      }
    }

    public virtual string MediaInfoStr
    {
      get
      {
        if (this.Message.MediaWaType != FunXMPP.FMessage.Type.Video && this.Message.MediaWaType != FunXMPP.FMessage.Type.Audio && this.Message.TransferInProgress)
          return (string) null;
        if (this.Message.LocalFileUri == null)
          return Utils.FileSizeFormatter.Format(this.Message.MediaSize);
        if (this.Message.MediaDurationSeconds > 0)
          return DateTimeUtils.FormatDuration(this.Message.MediaDurationSeconds);
        if (!this.durationFetchAttempted)
        {
          this.durationFetchAttempted = true;
          AppState.Worker.Enqueue((Action) (() =>
          {
            try
            {
              using (VideoFrameGrabber videoFrameGrabber = new VideoFrameGrabber(this.Message.LocalFileUri))
                this.OnMediaDurationRetrieved(videoFrameGrabber.DurationSeconds);
            }
            catch (Exception ex)
            {
              Log.l(this.LogHeader, "retrieve media duration failed");
              Log.LogException(ex, "retrieve media duration");
            }
          }));
        }
        return (string) null;
      }
    }

    public virtual Thickness MediaInfoMargin => new Thickness(0.0);

    protected override bool ShouldAddFooterPlaceHolder => this.Message.MediaCaption != null;

    public FileMessageViewModelBase(Message m)
      : base(m)
    {
    }

    public override void Cleanup()
    {
      this.largeThumbSub.SafeDispose();
      this.largeThumbSub = (IDisposable) null;
      base.Cleanup();
    }

    public override Set<string> GetTrackedProperties()
    {
      Set<string> trackedProperties = base.GetTrackedProperties();
      if (string.IsNullOrEmpty(this.Message.LocalFileUri))
      {
        trackedProperties.Add("LocalFileUri");
        trackedProperties.Add("Status");
        trackedProperties.Add("TransferValue");
        trackedProperties.Add("TransferInProgress");
      }
      else if (this.Message.KeyFromMe && !this.Message.IsDeliveredToServer())
      {
        trackedProperties.Add("TransferValue");
        trackedProperties.Add("TransferInProgress");
      }
      return trackedProperties;
    }

    protected virtual IObservable<WriteableBitmap> GenerateLargeThumbnailObservable(int targetWidth)
    {
      return Observable.Return<WriteableBitmap>((WriteableBitmap) null);
    }

    protected override IObservable<MessageViewModel.ThumbnailState> GetThumbnailObservableImpl(
      MessageViewModel.ThumbnailOptions thumbOptions = MessageViewModel.ThumbnailOptions.Standard)
    {
      Message msg = this.Message;
      return Observable.Create<MessageViewModel.ThumbnailState>((Func<IObserver<MessageViewModel.ThumbnailState>, Action>) (observer =>
      {
        bool disposed = false;
        bool preferSmallSize = (thumbOptions & MessageViewModel.ThumbnailOptions.GetSmall) != (MessageViewModel.ThumbnailOptions) 0 && (thumbOptions & MessageViewModel.ThumbnailOptions.GetLarge) == (MessageViewModel.ThumbnailOptions) 0;
        bool flag1 = (thumbOptions & MessageViewModel.ThumbnailOptions.GetSmall) == (MessageViewModel.ThumbnailOptions) 0 && (thumbOptions & MessageViewModel.ThumbnailOptions.GetLarge) != 0;
        bool isLargeSize = false;
        MemoryStream thumbStream = msg.GetThumbnailStream(preferSmallSize, out isLargeSize);
        bool flag2 = true;
        if (preferSmallSize & isLargeSize)
          flag2 = false;
        else if (flag1 && !isLargeSize)
          flag2 = false;
        bool shouldGenerateLargeThumb = !preferSmallSize && (thumbStream == null || !isLargeSize) && (msg.MediaWaType == FunXMPP.FMessage.Type.Image || msg.MediaWaType == FunXMPP.FMessage.Type.Video || msg.MediaWaType == FunXMPP.FMessage.Type.Document || msg.MediaWaType == FunXMPP.FMessage.Type.Gif);
        Log.l(this.LogHeader, "got thumb stream | large:{0}, stream:{1}, use:{2}, generateLarge:{3}", (object) isLargeSize, (object) (thumbStream != null), (object) flag2, (object) shouldGenerateLargeThumb);
        if (flag2)
        {
          if (thumbStream == null)
          {
            observer.OnNext(new MessageViewModel.ThumbnailState((System.Windows.Media.ImageSource) null, msg.KeyId, false));
            if (!shouldGenerateLargeThumb)
              observer.OnCompleted();
          }
          else if (!isLargeSize && (msg.MediaWaType == FunXMPP.FMessage.Type.Image || msg.MediaWaType == FunXMPP.FMessage.Type.Video || msg.MediaWaType == FunXMPP.FMessage.Type.Gif))
            Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() =>
            {
              if (!disposed)
              {
                BitmapSource thumb = (BitmapSource) null;
                using (thumbStream)
                  thumb = this.CreateBlurryThumb((BitmapSource) BitmapUtils.CreateBitmap((Stream) thumbStream, 100, 100));
                thumbStream = (MemoryStream) null;
                observer.OnNext(new MessageViewModel.ThumbnailState((System.Windows.Media.ImageSource) thumb, msg.KeyId, false));
              }
              if (shouldGenerateLargeThumb)
                return;
              observer.OnCompleted();
            }));
          else
            Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() =>
            {
              if (!disposed)
              {
                BitmapSource thumb = (BitmapSource) null;
                using (thumbStream)
                {
                  if ((thumbOptions & MessageViewModel.ThumbnailOptions.DecodeInBackground) == (MessageViewModel.ThumbnailOptions) 0)
                  {
                    thumb = (BitmapSource) BitmapUtils.CreateBitmap((Stream) thumbStream);
                  }
                  else
                  {
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.CreateOptions = BitmapCreateOptions.BackgroundCreation;
                    bitmapImage.SetSource((Stream) thumbStream);
                    thumb = (BitmapSource) bitmapImage;
                  }
                }
                thumbStream = (MemoryStream) null;
                observer.OnNext(new MessageViewModel.ThumbnailState((System.Windows.Media.ImageSource) thumb, msg.KeyId, true));
              }
              if (shouldGenerateLargeThumb)
                return;
              observer.OnCompleted();
            }));
        }
        IDisposable genLargeThumbSub = (IDisposable) null;
        if (shouldGenerateLargeThumb)
          genLargeThumbSub = this.GenerateLargeThumbnailObservable(MessageViewModel.LargeThumbPixelWidth).SubscribeOn<WriteableBitmap>((IScheduler) AppState.ImageWorker).ObserveOnDispatcher<WriteableBitmap>().Subscribe<WriteableBitmap>((Action<WriteableBitmap>) (largeThumb =>
          {
            if (largeThumb != null && !disposed)
            {
              observer.OnNext(new MessageViewModel.ThumbnailState((System.Windows.Media.ImageSource) largeThumb, msg.KeyId, true));
              this.SaveLargeThumbnail(largeThumb);
            }
            genLargeThumbSub.SafeDispose();
            genLargeThumbSub = (IDisposable) null;
            observer.OnCompleted();
          }), (Action) (() => observer.OnCompleted()));
        return (Action) (() =>
        {
          disposed = true;
          thumbStream.SafeDispose();
          thumbStream = (MemoryStream) null;
          genLargeThumbSub.SafeDispose();
          genLargeThumbSub = (IDisposable) null;
        });
      }));
    }

    protected override bool OnMessagePropertyChanged(string prop)
    {
      if (base.OnMessagePropertyChanged(prop))
        return true;
      bool flag = false;
      switch (prop)
      {
        case "LocalFileUri":
          this.OnMessageLocalFileUriChanged();
          flag = true;
          break;
        case "TransferValue":
          this.OnMessageTransferValueChanged();
          flag = true;
          break;
        case "TransferInProgress":
          this.OnMessageTransferStatusChanged();
          flag = true;
          break;
      }
      return flag;
    }

    private void OnMediaDurationRetrieved(double duration)
    {
      Log.l(this.LogHeader, "retrieved media duration | duration={0}", (object) duration);
      if (duration <= 0.0)
        return;
      AppState.Worker.Enqueue((Action) (() =>
      {
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          if (this.Message == null)
            return;
          Message message = db.GetMessage(this.Message.KeyRemoteJid, this.Message.KeyId, this.Message.KeyFromMe);
          if (message == null)
            return;
          this.Message.MediaDurationSeconds = message.MediaDurationSeconds = (int) duration;
          db.SubmitChanges();
        }));
        this.Notify("MediaDurationChanged");
      }));
    }

    private void OnMessageTransferValueChanged() => this.Notify("TransferProgressChanged");

    protected virtual void OnMessageLocalFileUriChanged() => this.Notify("LocalFileUriChanged");

    protected virtual void OnMessageTransferStatusChanged() => this.Notify("TransferStateChanged");
  }
}
