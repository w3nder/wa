// Decompiled with JetBrains decompiler
// Type: WhatsApp.VideoMessageViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.IO;
using System.Windows.Media.Imaging;


namespace WhatsApp
{
  public class VideoMessageViewModel : ImageMessageViewModel
  {
    public override BitmapSource ActionButtonIcon
    {
      get
      {
        BitmapSource actionButtonIcon = (BitmapSource) null;
        if (this.Message.KeyFromMe)
        {
          switch (MessageExtensions.GetUploadActionState(this.Message))
          {
            case MessageExtensions.MediaUploadActionState.Retryable:
              actionButtonIcon = (BitmapSource) ImageStore.UploadIcon;
              break;
            case MessageExtensions.MediaUploadActionState.Cancellable:
              actionButtonIcon = AssetStore.DismissIconWhite;
              break;
          }
        }
        else if (this.Message.LocalFileUri == null)
          actionButtonIcon = !this.Message.HasSidecar() ? (BitmapSource) ImageStore.DownloadIcon : (BitmapSource) ImageStore.PlayIcon;
        if (actionButtonIcon == null)
          actionButtonIcon = this.Message.PlaybackInProgress ? (BitmapSource) ImageStore.PauseIcon : (BitmapSource) ImageStore.PlayIcon;
        return actionButtonIcon;
      }
    }

    public override BitmapSource ActionButtonIconReversed
    {
      get
      {
        BitmapSource buttonIconReversed = (BitmapSource) null;
        if (this.Message.KeyFromMe)
        {
          switch (MessageExtensions.GetUploadActionState(this.Message))
          {
            case MessageExtensions.MediaUploadActionState.Retryable:
              buttonIconReversed = (BitmapSource) ImageStore.UploadIconActive;
              break;
            case MessageExtensions.MediaUploadActionState.Cancellable:
              buttonIconReversed = (BitmapSource) ImageStore.CancelIconActive;
              break;
          }
        }
        else if (this.Message.LocalFileUri == null)
          buttonIconReversed = !this.Message.HasSidecar() ? (BitmapSource) ImageStore.DownloadIconActive : (BitmapSource) ImageStore.PlayIconActive;
        if (buttonIconReversed == null)
          buttonIconReversed = this.Message.PlaybackInProgress ? (BitmapSource) ImageStore.PauseIconActive : (BitmapSource) ImageStore.PlayIconActive;
        return buttonIconReversed;
      }
    }

    public override bool ShouldShowActionButton => true;

    public VideoMessageViewModel(Message m)
      : base(m)
    {
    }

    protected override IObservable<WriteableBitmap> GenerateLargeThumbnailObservable(int targetWidth)
    {
      Message m = this.Message;
      return m == null ? Observable.Empty<WriteableBitmap>() : Observable.Create<WriteableBitmap>((Func<IObserver<WriteableBitmap>, Action>) (observer =>
      {
        IDisposable videoFrameSub = (IDisposable) null;
        videoFrameSub = this.GetVideoFrameBitmapObservable(m).SubscribeOn<WriteableBitmap>((IScheduler) AppState.ImageWorker).Take<WriteableBitmap>(1).ObserveOnDispatcher<WriteableBitmap>().Subscribe<WriteableBitmap>((Action<WriteableBitmap>) (bitmap =>
        {
          if (bitmap != null)
            observer.OnNext(this.CreateThumbnail(bitmap));
          observer.OnCompleted();
          videoFrameSub.SafeDispose();
          videoFrameSub = (IDisposable) null;
        }), (Action<Exception>) (ex => Log.l("Exception generating large thumb for Video " + m.LocalFileUri)), (Action) (() => observer.OnCompleted()));
        return (Action) (() =>
        {
          videoFrameSub.SafeDispose();
          videoFrameSub = (IDisposable) null;
        });
      }));
    }

    private IObservable<WriteableBitmap> GetVideoFrameBitmapObservable(Message m)
    {
      if (!Assert.IsTrue(m != null))
        return Observable.Empty<WriteableBitmap>();
      if (m.LocalFileExists())
        return !Assert.IsTrue(m.LocalFileUri != null) ? Observable.Empty<WriteableBitmap>() : ImageStore.GetVideoFrameBitmap(-1, 0L, m.LocalFileUri);
      Stream prefetchedStream = m.GetPrefetchedVideoStream();
      if (prefetchedStream == null)
        return Observable.Empty<WriteableBitmap>();
      if (prefetchedStream.Length < 16L)
        return Observable.Empty<WriteableBitmap>();
      MemoryStream clonedStream = new MemoryStream();
      AxolotlMediaCipher downloadCipher = AxolotlMediaCipher.CreateDownloadCipher(m);
      long maximumReadBytes = Math.Min((long) Settings.VideoPrefetchBytes, prefetchedStream.Length - prefetchedStream.Length % 16L);
      Func<byte[], int, int, int> srcRead = (Func<byte[], int, int, int>) ((buffer, offset, count) =>
      {
        if (prefetchedStream.Position >= maximumReadBytes)
          throw new VideoMessageViewModel.ReadPastPrefetchedBytesException();
        return prefetchedStream.Read(buffer, offset, count);
      });
      try
      {
        long srcLength = prefetchedStream.Length + 16L;
        Log.d(this.LogHeader, string.Format("Decrypting thumbnail from the first {0} bytes (faking length as {1})", (object) maximumReadBytes, (object) srcLength));
        downloadCipher.DecryptMedia(m, srcRead, (Func<long>) (() => prefetchedStream.Position), srcLength, new Action<byte[], int, int>(((Stream) clonedStream).Write), (WhatsApp.Events.MediaDownload) null);
      }
      catch (VideoMessageViewModel.ReadPastPrefetchedBytesException ex)
      {
        Log.d(this.LogHeader, "Successfully decrypted prefetched file");
      }
      finally
      {
        prefetchedStream.SafeDispose();
      }
      if (clonedStream == null)
        return Observable.Empty<WriteableBitmap>();
      return ImageStore.GetVideoFrameBitmap(-1, 0L, (string) null, (Stream) clonedStream);
    }

    protected virtual WriteableBitmap CreateThumbnail(WriteableBitmap bitmap)
    {
      return ImageStore.CreateThumbnail((BitmapSource) bitmap, MessageViewModel.LargeThumbPixelWidth);
    }

    private class ReadPastPrefetchedBytesException : Exception
    {
    }
  }
}
