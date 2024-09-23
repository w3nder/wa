// Decompiled with JetBrains decompiler
// Type: WhatsApp.CommonOps.SetChatPhoto
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Windows;
using System.Windows.Media.Imaging;


namespace WhatsApp.CommonOps
{
  public static class SetChatPhoto
  {
    public static void CropAndSet(string jid, WriteableBitmap bitmap)
    {
      if (bitmap == null || string.IsNullOrEmpty(jid))
        return;
      if (bitmap.PixelWidth < 192 || bitmap.PixelHeight < 192)
      {
        int num = (int) MessageBox.Show(Plurals.Instance.GetString(AppResources.SetProfileFailureReasonTooSmallPlural, 192));
      }
      else
      {
        IDisposable sub = (IDisposable) null;
        sub = ImageEditPage.Start(new ImageEditPage.ImageEditPageConfigs((BitmapSource) bitmap)
        {
          CropMode = ImageEditControl.CroppingMode.Fixed,
          InitialCropRatio = 1.0,
          MinRelativeCropSize = new Size?(new Size(192.0 / (double) bitmap.PixelWidth, 192.0 / (double) bitmap.PixelHeight))
        }).ObserveOnDispatcher<ImageEditPage.ImageEditPageResults>().Subscribe<ImageEditPage.ImageEditPageResults>((Action<ImageEditPage.ImageEditPageResults>) (args =>
        {
          NavUtils.GoBack(args.NavService);
          SetChatPhoto.Set(jid, SetChatPhoto.ProcessChatPictureCropResult(bitmap, args.RelativeCropSize, args.RelativeCropPos));
          sub.SafeDispose();
          sub = (IDisposable) null;
        }));
      }
    }

    public static void Set(string jid, WriteableBitmap bitmap)
    {
      if (bitmap == null || string.IsNullOrEmpty(jid))
        return;
      byte[] jpegByteArray1 = bitmap.ToJpegByteArray(96, 96, -1, new int?(Settings.JpegQuality));
      byte[] jpegByteArray2 = bitmap.ToJpegByteArray(640, 640, -1, new int?(Settings.JpegQuality));
      SetChatPhoto.Set(jid, jpegByteArray1, jpegByteArray2);
    }

    public static void Set(string jid, byte[] thumbnail, byte[] fullSize, bool showSystemMessage = true)
    {
      AppState.SchedulePersistentAction(PersistentAction.SetPhoto(jid, thumbnail, fullSize, showSystemMessage), true);
      ChatPictureStore.ForcePendingPictureUpdate(jid, new int?());
    }

    public static WriteableBitmap ProcessChatPictureCropResult(
      WriteableBitmap originalBitmap,
      Size? relativeCropSize,
      System.Windows.Point? relativeCropPos)
    {
      WriteableBitmap bitmap = !relativeCropSize.HasValue || !relativeCropPos.HasValue ? originalBitmap : originalBitmap.CropRelatively(relativeCropPos.Value, relativeCropSize.Value);
      double scale = Math.Max(640.0 / (double) bitmap.PixelWidth, 640.0 / (double) bitmap.PixelHeight);
      return bitmap.Scale(scale);
    }
  }
}
