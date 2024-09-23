// Decompiled with JetBrains decompiler
// Type: WhatsApp.StickerMessageViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using WhatsApp.WaCollections;


namespace WhatsApp
{
  public class StickerMessageViewModel : FileMessageViewModelBase
  {
    private int stickerWidth = 192;
    private static SolidColorBrush stickerFooterBgBrush;

    public Storyboard Animation { get; set; }

    public StickerMessageViewModel(Message m)
      : base(m)
    {
      this.ExcludedMenuItems = new Set<MessageMenu.MessageMenuItem>((IEnumerable<MessageMenu.MessageMenuItem>) new MessageMenu.MessageMenuItem[6]
      {
        MessageMenu.MessageMenuItem.ShowDetails,
        MessageMenu.MessageMenuItem.Copy,
        MessageMenu.MessageMenuItem.Reply,
        MessageMenu.MessageMenuItem.ReplyInPrivate,
        MessageMenu.MessageMenuItem.Forward,
        MessageMenu.MessageMenuItem.SaveMedia
      });
      if (Settings.StickersEnabled)
      {
        this.ExcludedMenuItems.Remove(MessageMenu.MessageMenuItem.Reply);
        this.ExcludedMenuItems.Remove(MessageMenu.MessageMenuItem.ReplyInPrivate);
        this.ExcludedMenuItems.Remove(MessageMenu.MessageMenuItem.Forward);
      }
      if (!Settings.StickerPickerEnabled)
        return;
      this.ExcludedMenuItems.Remove(MessageMenu.MessageMenuItem.SaveMedia);
    }

    public override bool ShouldShowTail
    {
      get => this.IsIncomingGroupMessage && base.ShouldShowTail && !this.IsForStarredView;
    }

    public override double MaxBubbleWidth
    {
      get
      {
        if (this.QuotedMessage != null || this.ShouldShowHeader)
          return double.PositiveInfinity;
        double num1 = (double) this.stickerWidth * this.zoomMultiplier;
        Thickness viewPanelMargin = this.ViewPanelMargin;
        double left = viewPanelMargin.Left;
        double num2 = num1 + left;
        viewPanelMargin = this.ViewPanelMargin;
        double right = viewPanelMargin.Right;
        return num2 + right;
      }
    }

    public override double ViewPanelTopMargin
    {
      get
      {
        int num = -4;
        if (this.ShouldShowHeader)
          num -= 8;
        return (double) num * this.zoomMultiplier;
      }
    }

    public override Thickness ViewPanelMargin
    {
      get
      {
        double num = 12.0 * this.zoomMultiplier;
        return new Thickness(num, this.ViewPanelTopMargin, num, -8.0 * this.zoomMultiplier);
      }
    }

    public double StickerWidth => (double) this.stickerWidth * this.zoomMultiplier;

    public override bool ShouldShowFooterOnLeft
    {
      get => !this.Message.KeyFromMe && this.QuotedMessage == null;
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
        if (actionButtonIcon == null && this.Message.LocalFileUri == null)
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
        if (buttonIconReversed == null && this.Message.LocalFileUri == null)
          buttonIconReversed = this.Message.TransferInProgress ? AssetStore.DismissIconWhite : (BitmapSource) ImageStore.DownloadIcon;
        return buttonIconReversed;
      }
    }

    public override bool ShouldShowActionButton
    {
      get
      {
        if (!this.Message.KeyFromMe)
          return this.Message.LocalFileUri == null;
        switch (MessageExtensions.GetUploadActionState(this.Message))
        {
          case MessageExtensions.MediaUploadActionState.Retryable:
          case MessageExtensions.MediaUploadActionState.Cancellable:
            return true;
          default:
            return this.Message.LocalFileUri == null;
        }
      }
    }

    public override bool ShouldFillFullTitleBackground => this.QuotedMessage != null;

    public override bool ShouldFillContentBackground => this.QuotedMessage != null;

    public override Brush FooterBackgroundBrush
    {
      get
      {
        return !this.ShouldFillContentBackground ? (Brush) this.BackgroundBrush : (Brush) UIUtils.TransparentBrush;
      }
    }

    private static SolidColorBrush StickerFooterBackgroundBrush
    {
      get
      {
        return StickerMessageViewModel.stickerFooterBgBrush ?? (StickerMessageViewModel.stickerFooterBgBrush = new SolidColorBrush(Color.FromArgb((byte) 77, (byte) 0, (byte) 0, (byte) 0)));
      }
    }

    public override Thickness FooterPanelPadding
    {
      get
      {
        return !this.ShouldFillContentBackground ? new Thickness(6.0 * this.zoomMultiplier, 0.0, 6.0 * this.zoomMultiplier, 0.0) : base.FooterPanelPadding;
      }
    }

    public override GridLength BubbleContentWidth
    {
      get => new GridLength(this.StickerWidth + 16.0 * this.zoomMultiplier);
    }

    public HorizontalAlignment StickerAlignment
    {
      get
      {
        return !this.Message.KeyFromMe || this.IsForStarredView ? HorizontalAlignment.Left : HorizontalAlignment.Right;
      }
    }

    public override HorizontalAlignment HorizontalAlignment
    {
      get => !this.IsForStarredView ? base.HorizontalAlignment : HorizontalAlignment.Left;
    }

    public override bool ShouldProcessGroupChatMessageGrouping() => false;

    public override bool IsThumbnailAvailable() => this.Message.LocalFileUri != null;

    protected override IObservable<MessageViewModel.ThumbnailState> GetThumbnailObservableImpl(
      MessageViewModel.ThumbnailOptions thumbOptions = MessageViewModel.ThumbnailOptions.Standard)
    {
      Message msg = this.Message;
      return Observable.Create<MessageViewModel.ThumbnailState>((Func<IObserver<MessageViewModel.ThumbnailState>, Action>) (observer =>
      {
        if (msg.MediaWaType != FunXMPP.FMessage.Type.Sticker)
          return (Action) (() => { });
        bool disposed = false;
        bool isLargeSize = false;
        MemoryStream thumbStream = msg.GetThumbnailStream(false, out isLargeSize);
        Log.l(this.LogHeader, "got thumb stream | large:{0}", (object) isLargeSize);
        if (thumbStream == null)
        {
          observer.OnNext(new MessageViewModel.ThumbnailState((System.Windows.Media.ImageSource) null, msg.KeyId, false));
          observer.OnCompleted();
        }
        else
          Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() =>
          {
            DateTime? start = PerformanceTimer.Start();
            if (!disposed)
            {
              MessageViewModel.ThumbnailState thumbnailState;
              using (thumbStream)
              {
                WebpUtils.WebpImage image = WebpUtils.DecodeWebp((Stream) thumbStream, Settings.StickerAnimationEnabled);
                int frameCount = (int) image.FrameCount;
                if (Settings.StickerAnimationEnabled && frameCount > 1)
                {
                  thumbnailState = new MessageViewModel.ThumbnailState(image.Frames[frameCount - 1].Image, msg.KeyId, false);
                  this.Animation = new Storyboard();
                  image.AnimateOn(this.Animation);
                }
                else
                  thumbnailState = frameCount <= 0 ? new MessageViewModel.ThumbnailState((System.Windows.Media.ImageSource) null, msg.KeyId, false) : new MessageViewModel.ThumbnailState(image.Frames[0].Image, msg.KeyId, false);
              }
              thumbStream = (MemoryStream) null;
              observer.OnNext(thumbnailState);
              observer.OnCompleted();
            }
            PerformanceTimer.End("decoding webp image", start);
          }));
        return (Action) (() =>
        {
          disposed = true;
          thumbStream.SafeDispose();
          thumbStream = (MemoryStream) null;
        });
      }));
    }
  }
}
