// Decompiled with JetBrains decompiler
// Type: WhatsApp.InlineVideoMessageViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

#nullable disable
namespace WhatsApp
{
  public class InlineVideoMessageViewModel : VideoMessageViewModel
  {
    private Matrix? rotationMatrix;

    public InlineVideoMessageViewModel(Message m)
      : base(m)
    {
    }

    public override bool ShouldShowMediaInfo => false;

    public virtual bool ShouldAttemptDownload
    {
      get => string.IsNullOrEmpty(this.Message.LocalFileUri) && !this.Message.TransferInProgress;
    }

    public override bool ShouldShowActionButton
    {
      get => !this.Message.TransferInProgress && this.Message.LocalFileUri == null;
    }

    public MessageProperties.MediaProperties.Attribution GetGifAttribution()
    {
      return this.Message.InternalProperties.GetGifAttribution();
    }

    public void AutoPlay()
    {
      if (this.ShouldAttemptDownload)
        return;
      this.Notify("Play");
    }

    public override bool ContainsInlineVideo => true;

    protected override Size GetTargetThumbnailSizeImpl()
    {
      double targetThumbnailRatio = this.TargetThumbnailRatio;
      double defaultContentWidth = MessageViewModel.DefaultContentWidth;
      return new Size(defaultContentWidth, defaultContentWidth / targetThumbnailRatio);
    }

    protected override WriteableBitmap CreateThumbnail(WriteableBitmap bitmap)
    {
      return ImageStore.CreateThumbnail((BitmapSource) bitmap, MessageViewModel.LargeThumbPixelWidth, new double?());
    }

    public IObservable<Matrix> GetRotationMatrix()
    {
      if (!this.rotationMatrix.HasValue)
        this.rotationMatrix = new Matrix?(this.Message.GetVideoRotationMatrix());
      return Observable.Return<Matrix>(this.rotationMatrix.Value);
    }
  }
}
