// Decompiled with JetBrains decompiler
// Type: WhatsApp.ImageMessageViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

#nullable disable
namespace WhatsApp
{
  public class ImageMessageViewModel : FileMessageViewModelBase
  {
    public override Thickness FooterMargin
    {
      get
      {
        return !this.ShouldShowText ? new Thickness(0.0, 0.0, 15.0 * this.zoomMultiplier, 15.0 * this.zoomMultiplier) : base.FooterMargin;
      }
    }

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
          actionButtonIcon = !this.Message.TransferInProgress ? (BitmapSource) ImageStore.DownloadIcon : AssetStore.DismissIconWhite;
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
              buttonIconReversed = (BitmapSource) ImageStore.UploadIcon;
              break;
            case MessageExtensions.MediaUploadActionState.Cancellable:
              buttonIconReversed = AssetStore.DismissIconWhite;
              break;
          }
        }
        else if (this.Message.LocalFileUri == null)
          buttonIconReversed = !this.Message.TransferInProgress ? (BitmapSource) ImageStore.DownloadIcon : AssetStore.DismissIconWhite;
        return buttonIconReversed;
      }
    }

    public override bool ShouldShowActionButton
    {
      get
      {
        if (!this.Message.KeyFromMe)
          return this.Message.LocalFileUri == null;
        MessageExtensions.MediaUploadActionState uploadActionState = MessageExtensions.GetUploadActionState(this.Message);
        return uploadActionState == MessageExtensions.MediaUploadActionState.Cancellable || uploadActionState == MessageExtensions.MediaUploadActionState.Retryable;
      }
    }

    public override Thickness ForwardedRowMargin
    {
      get
      {
        return base.ForwardedRowMargin with
        {
          Bottom = this.Message.KeyFromMe ? -4.0 : 6.0
        };
      }
    }

    public ImageMessageViewModel(Message m)
      : base(m)
    {
    }

    public override void RefreshTextFontSize()
    {
      if (this.Message.MediaCaption == null)
        return;
      this.Notify("TextFontSizeChanged");
    }

    protected double TargetThumbnailRatio => this.Message.GetThumbnailRatio();

    protected override Size GetTargetThumbnailSizeImpl()
    {
      double targetThumbnailRatio = this.TargetThumbnailRatio;
      double defaultContentWidth = MessageViewModel.DefaultContentWidth;
      double height = targetThumbnailRatio >= 1.0 ? (targetThumbnailRatio <= 2.4 ? defaultContentWidth / targetThumbnailRatio : defaultContentWidth / 2.4) : defaultContentWidth;
      return new Size(defaultContentWidth, height);
    }

    protected override IObservable<WriteableBitmap> GenerateLargeThumbnailObservable(int targetWidth)
    {
      Message m = this.Message;
      return Observable.Create<WriteableBitmap>((Func<IObserver<WriteableBitmap>, Action>) (observer =>
      {
        Stream imgStream = m.GetImageStream();
        if (imgStream == null)
        {
          observer.OnNext((WriteableBitmap) null);
          observer.OnCompleted();
        }
        else
          Deployment.Current.Dispatcher.BeginInvoke((Action) (() =>
          {
            WriteableBitmap writeableBitmap = (WriteableBitmap) null;
            using (imgStream)
            {
              WriteableBitmap bitmap = BitmapUtils.CreateBitmap(imgStream);
              if (bitmap != null)
                writeableBitmap = ImageStore.CreateMessageLargeThumbnail(imgStream, bitmap.PixelWidth, bitmap.PixelHeight, targetWidth);
            }
            observer.OnNext(writeableBitmap);
            observer.OnCompleted();
          }));
        return (Action) (() => { });
      }));
    }

    public override void GenerateLargeThumbnailAsync()
    {
      if (this.largeThumbFetchAttempted || this.Message.LocalFileUri == null)
        return;
      this.largeThumbFetchAttempted = true;
      this.largeThumbSub = this.GenerateLargeThumbnailObservable(MessageViewModel.LargeThumbPixelWidth).SubscribeOn<WriteableBitmap>((IScheduler) AppState.ImageWorker).ObserveOnDispatcher<WriteableBitmap>().Subscribe<WriteableBitmap>((Action<WriteableBitmap>) (largeThumb =>
      {
        this.OnLargeThumbnailCreated(largeThumb);
        this.largeThumbSub.SafeDispose();
        this.largeThumbSub = (IDisposable) null;
      }));
    }

    protected override void OnMessageLocalFileUriChanged()
    {
      base.OnMessageLocalFileUriChanged();
      if (this.Message.LocalFileUri == null)
        return;
      this.GenerateLargeThumbnailAsync();
    }
  }
}
